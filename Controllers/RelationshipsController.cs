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

/********************************************************************
 * IMPORTANT  IMPORTANT  IMPORTANT  IMPORTANT  IMPORTANT            *
 *                                                                  *
 * Update Get() method to use sproc or joins.  Easy to do and will  *
 * speed up loading of graphs.                                      *
 *******************************************************************/

namespace BackendAPI.Controllers
{
    public class Relationship
    {
        public int Id { get; set; }

        public string ByGroupsAndIndividuals { get; set; }      //Extra to CbRelationship
        public string ToGroupsAndIndividuals { get; set; }

        public string Description { get; set; }
        public int? RelationshipType { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Ended { get; set; }
        public int Creator { get; set; }
        public DateTime Created { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class GroupAndInd
    {
        public string value { get; set; }      //Group IDs will be proceeded by letter "G"
        public string label { get; set; }
    }

    public class PutDataRelationship
    {
        public int Id { get; set; }
        public string FieldName { get; set; }
        public string NewValue { get; set; }
        public int UserID { get; set; }

        public int? GetInteger()
        {
            return int.TryParse(this.NewValue, out int tmp) ? (int)tmp : null;
        }
        public decimal? GetDecimal()
        {
            return decimal.TryParse(this.NewValue, out decimal tmp) ? (decimal)tmp : null;
        }
        public DateTime? GetDateTime()
        {
            return DateTime.TryParse(this.NewValue, out DateTime tmp) ? (DateTime)tmp : null;
        }
        public bool? GetBool()
        {
            return bool.TryParse(this.NewValue, out bool tmp) ? (bool)tmp : null;
        }
        //public static T values<T>()
        //{
        //    Random random = new Random();
        //    int number = random.Next(1, 4);
        //    return (T)Convert.ChangeType(number, typeof(T));
        //}
    }

    public class PostDataRelationship
    {
        public int Id { get; set; }
        public string[] From { get; set; }
        public string[] To { get; set; }
        public string Description { get; set; }
        public int? RelationshipTypeID { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? Ended { get; set; }

        public int UserID { get; set; }     //Creator or UpdatedBy
    }


    [ApiController]
    [Route("[controller]")]
    public class RelationshipsController : ControllerBase
    {
        private readonly ILogger<RelationshipsController> _logger;

        public RelationshipsController(ILogger<RelationshipsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Relationship> Get()
        {
            //TODO: REPLACE WITH SQL QUERY TO RETRIEVE RESULT USING JOINS



            //SEE: https://docs.microsoft.com/en-us/ef/core/querying/raw-sql
            /* 
             * e.g.
             
            var blogs = context.Blogs
                .FromSqlRaw("SELECT * FROM dbo.Blogs")
                .ToList();
            Raw SQL queries can be used to execute a stored procedure.

            C#

            Copy
            var blogs = context.Blogs
                .FromSqlRaw("EXECUTE dbo.GetMostPopularBlogs")
                .ToList();
             
             */




            var context = new CrimeBoardContext();
            //var transactions = context.CbRelationships
            //                                  .Where(t => t.Active == true)
            //                                  .OrderBy(s => s.Created)
            //                                  .ToList();

            var relationships = (from relation in context.CbRelationships
                                orderby relation.Created
                                select new Relationship
                                {
                                    Id = relation.Id,
                                    ByGroupsAndIndividuals = GetLinks(relation.Id, true),    //"TODO",
                                    ToGroupsAndIndividuals = GetLinks(relation.Id, false),    //"TODO",
                                    Description = relation.Description,
                                    RelationshipType = relation.RelationshipTypeId,
                                    Started = relation.Started,
                                    Ended = relation.Ended,
                                    Creator = relation.Creator,
                                    Created = relation.Created,
                                    UpdatedBy = relation.UpdatedBy,
                                    LastUpdated = relation.LastUpdated
                                })
                         .ToList();

            //foreach (var tran in transactions.ToList())
            //{
            //    string[] lst = (from grp in context.CbGroups
            //             join grpXtran in context.CbGroupXRelationships 
            //                    on grp.Id equals grpXtran.GroupId
            //                    and grpXtran.Direction = true
            //             select new { Grp = grp.Name }).ToList();

            //}

            /*
             from transaction in context.CbRelationships
                join user in context.CbIndividuals on transaction equals user.Owner into gj
                from subpet in gj.DefaultIfEmpty()
                select new { transaction.FirstName, PetName = subpet?.Name ?? String.Empty };
             */



            Response.Headers.Add("X-Total-Count", relationships.Count.ToString());

            return relationships;
        }


        private static string GetLinks(int relationshipId, bool direction)
        {
            /*
            TODO + TEST ---------------------->
            
            FROM:                                       TO:
            Groups (dir=?)                              Individuals (dir=?)     //e.g. ? = untested
            Groups (dir=?)                              Groups (dir=?)          //e.g. ? = untested
            Individuals (dir=?)                         Individuals (dir=?)     //e.g. ? = untested
            Individuals (dir=0)                         Groups (dir=1) 
             */





            string list = "";

            using (var context = new CrimeBoardContext())
            {
                List<string> lst = (from grp in context.CbGroups
                                    join grpXrel in context.CbGroupXRelationships
                                    on grp.Id equals grpXrel.GroupId
                                    join tran in context.CbRelationships
                                    on grpXrel.RelationshipId equals relationshipId
                                    where grpXrel.Direction == direction
                                    select grp.Name)
                         .Distinct()                                                //TODO - inefficient & susceptiple
                         .ToList();

                List<string> lst2 = (from indv in context.CbIndividuals
                                     join indvXrel in context.CbIndividualXRelationships
                                     on indv.Id equals indvXrel.IndividualId
                                     join tran in context.CbRelationships
                                     on indvXrel.RelationshipId equals relationshipId
                                     where indvXrel.Direction == direction
                                     select indv.Surname + ", " + indv.Firstname)
                         .Distinct()                                                //TODO - inefficient & susceptiple
                         .ToList();

                lst.AddRange(lst2);

                foreach (string item in lst)
                {
                    list = list + item + " ";      //using lst.join(..) is wasting my time with errors
                }
            }

            return list.Trim();
        }

        [Route("[action]")]         //see: http://www.binaryintellect.net/articles/9db02aa1-c193-421e-94d0-926e440ed297.aspx
        [HttpGet]
        public IEnumerable<GroupAndInd> GetAllGroupsAndIndivs(string country)
        {
            var context = new CrimeBoardContext();

            var groupsAndIndivs = new List<GroupAndInd>();
            groupsAndIndivs.Add(new GroupAndInd()
            {
                value = "-1",
                label = "------- Groups -------"
            });

            var groupsList = (from groups in context.CbGroups
                                              .Where(t => t.Active == true)
                                              .OrderBy(s => s.Name)
                                   select new GroupAndInd
                                   {
                                       value = 'G' + groups.Id.ToString(),
                                       label = groups.Name        //TODO: remove group "(<description>)" e.g. shorthand for input box on frontend
                                   })
                         .ToList();

            groupsAndIndivs.AddRange(groupsList);

            groupsAndIndivs.Add(new GroupAndInd()
            {
                value = "0",
                label = "------- Individuals -------"
            });

            var indivsList = (from indivs in context.CbIndividuals
                                              //.Where(t => t.Active == true)
                                              .OrderBy(s => s.Surname).ThenBy(f => f.Firstname)
                                    select new GroupAndInd
                                    {
                                        value = indivs.Id.ToString(),
                                        label = indivs.Surname + ", " + indivs.Firstname
                                    })
                         .ToList();

            groupsAndIndivs.AddRange(indivsList);

            return groupsAndIndivs;
        }


        // PUT: Relationship/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, PutDataRelationship putDataRelationship)
        {
            using (var _context = new CrimeBoardContext())
            {
                //TODO: Validate data changes

                if (id != putDataRelationship.Id)      /// ???
                {
                    return BadRequest();
                }

                string oldValue = "";
                putDataRelationship.NewValue = putDataRelationship.NewValue.Trim();

                CbRelationship cbRelationship = _context.CbRelationships.FirstOrDefault(x => x.Id == id);

                switch (putDataRelationship.FieldName.ToLower())
                {
                    case "description":
                        oldValue = cbRelationship.Description;
                        cbRelationship.Description = putDataRelationship.NewValue;
                        break;
                    case "relationshiptypeid":
                        oldValue = Convert.ToString(cbRelationship.RelationshipTypeId);
                        cbRelationship.RelationshipTypeId = putDataRelationship.GetInteger();
                        break;
                    case "started":
                        oldValue = Convert.ToString(cbRelationship.Started);
                        cbRelationship.Started = putDataRelationship.GetDateTime();
                        break;
                    case "ended":
                        oldValue = Convert.ToString(cbRelationship.Ended);
                        cbRelationship.Ended = putDataRelationship.GetDateTime();
                        break;
                }

                cbRelationship.UpdatedBy = putDataRelationship.UserID;
                cbRelationship.LastUpdated = DateTime.Now;

                _context.Entry(cbRelationship).State = EntityState.Modified;
                //_context.SaveChanges();

                try
                {
                    //await _context.SaveChangesAsync();

                    //Log change by user X

                    CbAudit cbAudit = new CbAudit()
                    {
                        TableName = "Cb_Relationship",
                        FieldName = putDataRelationship.FieldName,
                        Id = cbRelationship.Id,
                        UserId = putDataRelationship.UserID,
                        DateChanged = DateTime.Now,
                        OldValue = oldValue,
                        NewValue = putDataRelationship.NewValue
                    };
                    _context.CbAudits.Add(cbAudit);

                    await _context.SaveChangesAsync();      //Save changes to both tables here

                    _logger.LogInformation("UPDATE CB_Relationship. Field " + putDataRelationship.FieldName + "=" + putDataRelationship.NewValue);
                }
                catch (Exception ex)    //was: DbUpdateConcurrencyException ex
                {
                    //Log error
                    _logger.LogError("ERROR in Relationship Put()", ex);
                    return Problem(ex.Message, null, 400);
                }

                return Ok(cbRelationship);        //TODO: More efficient to not return record
            }
        }


        // POST: api/Products
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<IActionResult> Post(PostDataRelationship postDataRelationship)
        {
            using (var _context = new CrimeBoardContext())
            {
                CbRelationship cbRelationship = new CbRelationship()
                {
                    //Id                     
                    Description = postDataRelationship.Description.Trim(),
                    RelationshipTypeId = postDataRelationship.RelationshipTypeID,
                    Started = postDataRelationship.Started,
                    Ended = postDataRelationship.Ended,
                    Creator = postDataRelationship.UserID,
                    Created = DateTime.Now
                };

                _context.CbRelationships.Add(cbRelationship);

                try
                {
                    _context.SaveChanges();

                    _logger.LogInformation("ADDED CB_Relationship. Id=" + postDataRelationship.Id);

                    //From
                    foreach (string fromId in postDataRelationship.From)
                    {
                        int id = int.Parse(fromId.Replace("G", ""));

                        if (fromId.StartsWith("G"))
                        {
                            CbGroupXRelationship cbGroupXRelationship = new CbGroupXRelationship()
                            {
                                GroupId = id,
                                RelationshipId = cbRelationship.Id,
                                Direction = true                //e.g. Ind -- Trans <=T= Group
                            };

                            _context.CbGroupXRelationships.Add(cbGroupXRelationship);
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("ADDED CB_GroupXRelationship. GroupId=" + cbGroupXRelationship.GroupId + ", RelationshipId=" + cbGroupXRelationship.RelationshipId);
                        }
                        else
                        {
                            CbIndividualXRelationship cbIndividualXRelationship = new CbIndividualXRelationship()
                            {
                                IndividualId = id,
                                RelationshipId = cbRelationship.Id,
                                Direction = true                //e.g. Ind =T=> Trans -- Group
                            };

                            _context.CbIndividualXRelationships.Add(cbIndividualXRelationship);
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("ADDED CB_IndividualXRelationship. IndividualId=" + cbIndividualXRelationship.IndividualId + ", RelationshipId=" + cbIndividualXRelationship.RelationshipId);
                        }
                    }

                    //To
                    foreach (string toId in postDataRelationship.To)
                    {
                        int id = int.Parse(toId.Replace("G", ""));

                        if (toId.StartsWith("G"))
                        {
                            CbGroupXRelationship cbGroupXRelationship = new CbGroupXRelationship()
                            {
                                GroupId = id,
                                RelationshipId = cbRelationship.Id,
                                Direction = false                //e.g. Ind -- Trans <=F= Group
                                                                 //     Ind -- Trans =T=> Group (equivalent)
                            };

                            _context.CbGroupXRelationships.Add(cbGroupXRelationship);
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("ADDED CB_GroupXRelationship. GroupId=" + cbGroupXRelationship.GroupId + ", RelationshipId=" + cbGroupXRelationship.RelationshipId);
                        }
                        else
                        {
                            CbIndividualXRelationship cbIndividualXRelationship = new CbIndividualXRelationship()
                            {
                                IndividualId = id,
                                RelationshipId = cbRelationship.Id,
                                Direction = false                //e.g. Ind =F=> Trans -- Group
                                                                 //     Ind <=T= Trans -- Group (equivalent)
                            };

                            _context.CbIndividualXRelationships.Add(cbIndividualXRelationship);
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("ADDED CB_IndividualXRelationship. IndividualId=" + cbIndividualXRelationship.IndividualId + ", RelationshipId=" + cbIndividualXRelationship.RelationshipId);
                        }
                    }

                    //Might have to move this into foreach loops above e.g. unbatched
                    //await _context.SaveChangesAsync();
                    //_logger.LogInformation("ADDED CB_IndividualXRelationship / CB_GroupXRelationship links.");


                    //Log change by user X

                    CbAudit cbAudit = new CbAudit()
                    {
                        TableName = "Cb_Relationship",
                        FieldName = "- NEW RELATIONSHIP -",
                        Id = cbRelationship.Id,
                        UserId = cbRelationship.Creator,
                        DateChanged = DateTime.Now,
                        OldValue = "",
                        NewValue = cbRelationship.Description
                    };
                    _context.CbAudits.Add(cbAudit);

                    await _context.SaveChangesAsync();      //Save changes to both tables here

                    _logger.LogInformation("ADDED CB_Relationship. Description=" + cbRelationship.Description);

                }
                catch (Exception ex)    //was: DbUpdateConcurrencyException ex
                {
                    //Log error
                    _logger.LogError("ERROR in Relationship Post()", ex);
                    return Problem(ex.Message, null, 400);
                }

                return Ok(cbRelationship);        //TODO: More efficient to not return record
            }
        }

    }
}
