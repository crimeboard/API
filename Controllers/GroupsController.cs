using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Newtonsoft.Json;

/*******************************************
 * Author: https://twitter.com/kenmurrayx4 *
 * Date: 2020/21                           *
 ******************************************/

namespace BackendAPI.Controllers
{
    //public class Group
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public int? ParentID { get; set; }
    //    public string Description { get; set; }
    //    public int ImageID { get; set; }
    //    public int Creator { get; set; }
    //    public DateTime Created { get; set; }
    //    public int? UpdatedBy { get; set; }
    //    public DateTime? LastUpdated { get; set; }
    //}

    public class PutDataGroup
    {
        public int    Id { get; set; }
        public string FieldName { get; set; }
        public string NewValue { get; set; }
        public int    UserID { get; set; }

        public int? GetNullableInteger()
        {
            int? x = int.TryParse(this.NewValue, out int tmp) ? (int)tmp : null;
            return (x == -1) ? null : x;
        }
    }

    public class PostDataGroup
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public string Description { get; set; }
        public string WikipediaUrl { get; set; }
        public int? ImageId { get; set; }
        public int UserID { get; set; }     //Creator or UpdatedBy
    }


    [ApiController]
    [Route("[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly ILogger<GroupsController> _logger;

        public GroupsController(ILogger<GroupsController> logger)
        {
            _logger = logger;
        }

        //TODO: see https://codereview.stackexchange.com/questions/102389/nested-object-to-hierarchical-object-list

        [HttpGet]
        public IList<CbGroup> Get()
        {
            var context = new CrimeBoardContext();

            //var groups = context.CbGroups
            //    .OrderBy(s => s.Name)
            //    .ToList();

            var groups = (from grp in context.CbGroups
                               orderby grp.Name
                               select new CbGroup
                               {
                                    Id = grp.Id,
                                    Name = grp.Name,
                                    ParentId = grp.ParentId,
                                    Description = grp.Description, //.Substring(0, 25) + "...",    //will embedded quotes break JSON?
                                    WikipediaUrl = (""+ grp.WikipediaUrl).Replace("\n", ""),    //TODO: SQL server won't remove these characters
                                    ImageId = grp.ImageId ?? 0,
                                    Creator = grp.Creator,
                                    Created = grp.Created,
                                    UpdatedBy = grp.UpdatedBy,
                                    LastUpdated = grp.LastUpdated
                               })
                         .ToList();

            Response.Headers.Add("X-Total-Count", groups.Count.ToString());

            return groups;

            //call it with parentId=0 initially, to get parentless nodes
//            return GetChildren(context.CbGroups, 1);          //CrimeBoard record == Root
        }

        private IList<CbGroup> GetChildren(DbSet<CbGroup> source, int parentId)
        {
            var children = source.Where(x => x.ParentId == parentId).OrderBy(x => x.Name).ToList();
            //GetChildren is called recursively again for every child found
            //and this process repeats until no childs are found for given node, 
            //in which case an empty list is returned
            //children.ForEach(x => x.ChildCbGroups = GetChildren(source, x.Id));
            children.ForEach(x => GetChildren(source, x.Id));
            return children;
        }
        /*
        //[HttpGet]
        public IEnumerable<CbGroup> Getold()
        {

            //WHY IS THIS RETURNING SO MANY RECORDS?
            var context = new CrimeBoardContext();
            var groups = context.CbGroups
                                              .OrderBy(s => s.ParentId).ThenBy(s => s.Name)
                                              .ToList();

            Response.Headers.Add("X-Total-Count", groups.Count.ToString());

            JsonConvert.SerializeObject(groups, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

            return groups;

            //.ToArray();
        }
        */

        //See: https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-5.0&tabs=visual-studio
        // PUT: Group/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, PutDataGroup putDataGroup)
        {
            using (var _context = new CrimeBoardContext())
            {
                //TODO: Validate data changes

                if (id != putDataGroup.Id)      /// ???
                {
                    return BadRequest();
                }

                string oldValue = "";
                putDataGroup.NewValue = putDataGroup.NewValue.Trim();

                CbGroup cbGroup = _context.CbGroups.FirstOrDefault(x => x.Id == id);

                switch (putDataGroup.FieldName.ToLower())
                {
                    case "id":
                        break;
                    case "name":
                        if (_context.CbGroups.Any(x => x.Name == putDataGroup.NewValue))
                        {
                            return Problem("Group already exists", null, 400);
                        }

                        oldValue = cbGroup.Name;
                        cbGroup.Name = putDataGroup.NewValue;
                        break;
                    case "parentid":
                        oldValue = Convert.ToString(cbGroup.ParentId);
                        cbGroup.ParentId = putDataGroup.GetNullableInteger();
                        break;
                    case "description":
                        oldValue = cbGroup.Description;
                        cbGroup.Description = putDataGroup.NewValue;
                        break;
                    case "wikipediaurl":
                        oldValue = cbGroup.WikipediaUrl;
                        cbGroup.WikipediaUrl = putDataGroup.NewValue;
                        break;
                }

                cbGroup.UpdatedBy = putDataGroup.UserID;
                cbGroup.LastUpdated = DateTime.Now;

                _context.Entry(cbGroup).State = EntityState.Modified;

                try
                {
                    _context.SaveChanges();

                    //Log change by user X

                    CbAudit cbAudit = new CbAudit() { 
                        TableName = "Cb_Group",
                        FieldName = putDataGroup.FieldName,
                        Id = cbGroup.Id,
                        UserId = putDataGroup.UserID,
                        DateChanged = DateTime.Now,
                        OldValue = oldValue,
                        NewValue = putDataGroup.NewValue
                    };
                    _context.CbAudits.Add(cbAudit);

                    await _context.SaveChangesAsync();      //Save changes to both tables here

                    _logger.LogInformation("UPDATE CB_Group. Field " + putDataGroup.FieldName + "=" + putDataGroup.NewValue);
               }
                catch (Exception ex)    //was: DbUpdateConcurrencyException ex
                {
                    //Log error
                    _logger.LogError("ERROR in Group Put()", ex);
                    return Problem(ex.Message, null, 400);
                }

                return Ok(cbGroup);        //TODO: More efficient to not return record
            }
        }
        
 
        // POST: api/Products
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<IActionResult> Post(PostDataGroup postDataGroup)
        {
            using (var _context = new CrimeBoardContext())
            {
                if (_context.CbGroups.Any(x => x.Name == postDataGroup.Name))
                {
                    return Problem("Group already exists", null, 400);
                }

                CbGroup cbGroup = new CbGroup()
                {
                    Name = postDataGroup.Name.Trim(),
                    ParentId = postDataGroup.ParentId,
                    Description = postDataGroup.Description.Trim(),
                    WikipediaUrl = postDataGroup.WikipediaUrl.Trim(),
                    ImageId = -1, //postDataGroup.ImageId
                    Creator = postDataGroup.UserID,
                    Created = DateTime.Now
                };

                _context.CbGroups.Add(cbGroup);

                try
                {
                    _context.SaveChanges();

                    _logger.LogInformation("ADDED CB_Group. Name=" + postDataGroup.Name + ", ParentID=" + postDataGroup.ParentId);

                    CbAudit cbAudit = new CbAudit()
                    {
                        TableName = "Cb_Group",
                        FieldName = "- NEW GROUP -",
                        Id = cbGroup.Id,
                        UserId = postDataGroup.UserID,
                        DateChanged = DateTime.Now,
                        OldValue = "",
                        NewValue = postDataGroup.Name
                    };
                    _context.CbAudits.Add(cbAudit);

                    await _context.SaveChangesAsync();      //Save changes

                    _logger.LogInformation("ADDED CB_Group. Name=" + postDataGroup.Name);
                }
                catch (Exception ex)    //was: DbUpdateConcurrencyException ex
                {
                    //Log error
                    _logger.LogError("ERROR in Group Post()", ex);
                    return Problem(ex.Message, null, 400);
                }

                return Ok(cbGroup);        //TODO: More efficient to not return record
            }
        }        
        
    }
}
