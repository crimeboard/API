using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

/*******************************************
 * Author: https://twitter.com/kenmurrayx4 *
 * Date: 2020/21                           *
 ******************************************/

namespace BackendAPI.Controllers
{
    public class User
    {        
        public string TwitterID { get; set; }
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string TwitterHandle { get; set; }
        public string GoogleHandle { get; set; }
        public bool AutoApprove { get; set; }
        public string GuidVerificationCode { get; set; }
        public bool Active { get; set; }
        public int Creator { get; set; }
        public DateTime Created { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class PutDataUser
    {
        public int    Id { get; set; }
        public string FieldName { get; set; }
        public string NewValue { get; set; }
        public int    UserID { get; set; }
    }

    public class PostDataUser
    {
        public string TwitterHandle { get; set; }
        public int    ParentID { get; set; }
        public bool AutoApprove { get; set; }
        public string GuidVerificationCode { get; set; }
        public bool Active { get; set; }
        public int UserID { get; set; }     //Creator or UpdatedBy
    }

    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        /******************************************************************
         * Disabling to avoid compromising users..
         * 
        [HttpGet]
        public IEnumerable<User> Get()
        {
            var context = new CrimeBoardContext();
            
            //var users = context.CbUsers
            //                                  .OrderBy(s => s.Surname).ThenBy(s => s.AutoApprove)
            //                                  .ToList();

            //users.ForEach(user => { user.ImageId = 0; });
            

            var users = (from user in context.CbUsers
                                   //
                               orderby user.TwitterHandle
                               select new User
                                {
                                    TwitterHandle = user.TwitterHandle,
                                    ID = user.Id,
                                    ParentID = user.ParentId,   // ?? 0,
                                    AutoApprove = user.AutoApprove,
                                    GuidVerificationCode = "",  //user.GuidVerificationCode,
                                    Active = user.Active,
                                    Creator = user.Creator,
                                    Created = user.Created,
                                    UpdatedBy = user.UpdatedBy,
                                    LastUpdated = user.LastUpdated
                                })
                         .ToList();
            
            Response.Headers.Add("X-Total-Count", users.Count.ToString());

            return users;
        }
        */

        [HttpGet("{id}")]
        //public async Task<ActionResult<User>> Get(int id)       //Maybe use this instead?  See IndividualsController for example.
        public User Get(int id)
        {
            var context = new CrimeBoardContext();

            var users = (from user in context.CbUsers
                         where user.Id == id
                         select new User
                         {
                             TwitterHandle = user.TwitterHandle,
                             ID = user.Id,
                             ParentID = user.ParentId,
                             AutoApprove = user.AutoApprove,
                             Active = user.Active ?? false

                             /* No need to send this..
                             GuidVerificationCode = user.GuidVerificationCode,
                             Creator = user.Creator,
                             Created = user.Created,
                             UpdatedBy = user.UpdatedBy,
                             LastUpdated = user.LastUpdated
                             */
                         });

            Response.Headers.Add("X-Total-Count", users != null ? "1" : "0");

            return users as User;
        }


        //See: https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-5.0&tabs=visual-studio
        // PUT: User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, PutDataUser putDataUser)
        {
            using (var _context = new CrimeBoardContext())
            {
                //TODO: Validate data changes

                if (id != putDataUser.Id)      /// ???
                {
                    return BadRequest();
                }

                string oldValue = "";
                putDataUser.NewValue = putDataUser.NewValue.Trim();

                CbUser cbUser = _context.CbUsers.FirstOrDefault(x => x.Id == id);

                switch (putDataUser.FieldName.ToLower())
                {
                    case "twitterhandle":
                        if (_context.CbUsers.Any(x => x.TwitterHandle == putDataUser.NewValue))
                        {
                            return Problem("User already exists", null, 400);
                        }

                        oldValue = cbUser.TwitterHandle;
                        cbUser.TwitterHandle = putDataUser.NewValue;
                        break;
                    case "id":
                        break;
                    case "parentid":
                        oldValue = cbUser.ParentId.ToString();
                        int parentid = int.Parse(putDataUser.NewValue);    // int.TryParse(putDataUser.NewValue, out int tmp) ? (int?)tmp : null;
                        cbUser.ParentId = parentid;
                        break;
                    case "autoapprove":
                        oldValue = cbUser.AutoApprove.ToString();
                        cbUser.AutoApprove = bool.Parse(putDataUser.NewValue);
                        break;
                    case "guidverificationcode":
                        oldValue = cbUser.GuidVerificationCode;
                        cbUser.GuidVerificationCode = putDataUser.NewValue;
                        break;
                    case "active":
                        oldValue = Convert.ToString(cbUser.Active);
                        cbUser.Active = bool.Parse(putDataUser.NewValue);
                        break;
                }

                cbUser.UpdatedBy = putDataUser.UserID;
                cbUser.LastUpdated = DateTime.Now;

                _context.Entry(cbUser).State = EntityState.Modified;

                try
                {
                    _context.SaveChanges();

                    //Log change by user X

                    CbAudit cbAudit = new CbAudit() { 
                        TableName = "Cb_User",
                        FieldName = putDataUser.FieldName,
                        Id = cbUser.Id,
                        UserId = putDataUser.UserID,
                        DateChanged = DateTime.Now,
                        OldValue = oldValue,
                        NewValue = putDataUser.NewValue
                    };
                    _context.CbAudits.Add(cbAudit);

                    await _context.SaveChangesAsync();      //Save changes to both tables here

                    _logger.LogInformation("UPDATE CB_User. Field " + putDataUser.FieldName + "=" + putDataUser.NewValue);
                }
                catch (Exception ex)    //was: DbUpdateConcurrencyException ex
                {
                    //Log error
                    _logger.LogError("ERROR in User Put()", ex);
                    return Problem(ex.Message, null, 400);
                }

                return Ok(cbUser);        //TODO: More efficient to not return record
            }
        }

        
 
        // POST: api/Products
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<IActionResult> Post(PostDataUser postDataUser)
        {
            using (var _context = new CrimeBoardContext())
            {
                if (_context.CbUsers.Any(x => x.TwitterHandle == postDataUser.TwitterHandle))
                {
                    return Problem("User already exists", null, 400);
                }

                CbUser cbUser = new CbUser()
                {
                    TwitterHandle = postDataUser.TwitterHandle.Trim(),
                    ParentId = postDataUser.ParentID,
                    AutoApprove = postDataUser.AutoApprove,
                    GuidVerificationCode = postDataUser.GuidVerificationCode.Trim(),
                    Active = postDataUser.Active,
                    Creator = postDataUser.UserID,
                    Created = DateTime.Now
                };

                _context.CbUsers.Add(cbUser);

                try
                {
                    _context.SaveChanges();
                    _logger.LogInformation("ADDED CB_User. TwitterHandle=" + postDataUser.TwitterHandle);

                    CbAudit cbAudit = new CbAudit()
                    {
                        TableName = "Cb_User",
                        FieldName = "",
                        Id = cbUser.Id,
                        UserId = postDataUser.UserID,
                        DateChanged = DateTime.Now,
                        OldValue = "- NEW GROUP -",
                        NewValue = postDataUser.TwitterHandle
                    };
                    _context.CbAudits.Add(cbAudit);

                    await _context.SaveChangesAsync();      //Save changes
                }
                catch (Exception ex)    //was: DbUpdateConcurrencyException ex
                {
                    //Log error
                    _logger.LogError("ERROR in User Post()", ex);
                    return Problem(ex.Message, null, 400);
                }

                return Ok(cbUser);        //TODO: More efficient to not return record
            }
        }        
        
    }
}
