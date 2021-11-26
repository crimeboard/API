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
    public class ViewAuditUser
    {
        public int UserId { get; set; }
        public string TableName { get; set; }
        public int Id { get; set; }
        public string FieldName { get; set; }
        public DateTime DateChanged { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string TwitterHandle { get; set; }
        public string GoogleHandle { get; set; }
        public bool AutoApprove { get; set; }
        public bool? Active { get; set; }
    }

    /*
     * TODO: When we change a relationship or transaction type or a boolean we need to log the name of the relationship etc in
     *       the suit table e.g. not a number or 0 or 1 for boolean or number for country              <= IMPORTANT
     * */

    [ApiController]
    [Route("[controller]")]
    public class AuditController : ControllerBase
    {
        private readonly ILogger<AuditController> _logger;

        public AuditController(ILogger<AuditController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<ViewAuditUser> Get()
        {
            var context = new CrimeBoardContext();
            
            //var users = context.CbViewAuditUser
            //                                  .OrderBy(s => s.Surname).ThenBy(s => s.AutoApprove)
            //                                  .ToList();

            //users.ForEach(user => { user.ImageId = 0; });
                     
            var viewAuditUsers = (from viewAuditUser in context.CbViewAuditUsers   
                                select new ViewAuditUser
                                {
                                    UserId = viewAuditUser.UserId,
                                    TableName = viewAuditUser.TableName.Replace("Cb_", ""),
                                    Id = viewAuditUser.Id,
                                    FieldName = System.Text.RegularExpressions.Regex.Replace(viewAuditUser.FieldName, "([A-Z])", " $1").ToLower(),
                                    DateChanged = viewAuditUser.DateChanged,
                                    OldValue = viewAuditUser.OldValue,
                                    NewValue = viewAuditUser.NewValue,
                                    TwitterHandle = viewAuditUser.TwitterHandle,
                                    GoogleHandle = viewAuditUser.GoogleHandle,
                                    AutoApprove = viewAuditUser.AutoApprove,
                                    Active = viewAuditUser.Active,
                                }).OrderByDescending(x => x.DateChanged)
                                .ToList();

            Response.Headers.Add("X-Total-Count", viewAuditUsers.Count.ToString());

            return viewAuditUsers;
        }

        /*
        [HttpGet("{id}")]
        //public async Task<ActionResult<User>> Get(int id)       //Maybe use this instead?  See IndividualsController for example.
        public User Get(int id)
        {
            var context = new CrimeBoardContext();

            var users = (from user in context.CbViewAuditUsers
                         where user.Id == id
                         select new User
                         {
                             TwitterHandle = user.TwitterHandle,
                             ID = user.Id,
                             ParentID = user.ParentId,
                             AutoApprove = user.AutoApprove,
                             Active = user.Active ?? false
                         });

            Response.Headers.Add("X-Total-Count", users != null ? "1" : "0");

            return users as User;
        }
        */
    }
}
