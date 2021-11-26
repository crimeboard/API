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
    public class Transaction
    {
        public int Id { get; set; }

        public string ByGroupsAndIndividuals { get; set; }      //Extra to CbTransaction
        public string ToGroupsAndIndividuals { get; set; }

        public string Description { get; set; }
        public int? Importance { get; set; }
        public decimal? MonetaryAmount { get; set; }
        public decimal? MinFine { get; set; }
        public decimal? MaxFine { get; set; }
        public int? MinSentenceYears { get; set; }
        public int? MaxSentenceYears { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? JuristictionId { get; set; }
        public string TwitterPostUrl { get; set; }
        public string YouTubeUrl { get; set; }
        public string NewspaperArticleUrl { get; set; }
        public bool? Active { get; set; }
        public int Creator { get; set; }
        public DateTime Created { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class GroupAndIndiv
    {
        public string value { get; set; }      //Group IDs will be proceeded by letter "G"
        public string label { get; set; }
    }

    public class PutDataTransaction
    {
        public int    Id { get; set; }
        public string FieldName { get; set; }
        public string NewValue { get; set; }
        public int    UserID { get; set; }

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

    public class PostDataTransaction
    {
        public int Id { get; set; }
        public string[] From { get; set; }
        public string[] To { get; set; }
        public string Description { get; set; }
        public int? Importance { get; set; }
        public int? MonetaryAmount { get; set; }
        public int? MinFine { get; set; }
        public int? MaxFine { get; set; }
        public int? MinSentenceYears { get; set; }
        public int? MaxSentenceYears { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? JuristictionID { get; set; }
        public string TwitterPostURL { get; set; }
        public string YouTubeURL { get; set; }
        public string NewspaperArticleURL { get; set; }
        public bool Active { get; set; }

        public int UserID { get; set; }     //Creator or UpdatedBy
    }


    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(ILogger<TransactionsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Transaction> Get()
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
            //var transactions = context.CbTransactions
            //                                  .Where(t => t.Active == true)
            //                                  .OrderBy(s => s.Created)
            //                                  .ToList();

            var transactions = (from trans in context.CbTransactions
                                where trans.Active == true
                                 orderby trans.Created
                                 select new Transaction
                                 {
                                    Id = trans.Id,
                                    ByGroupsAndIndividuals = GetLinks(trans.Id, true),    //"TODO",
                                    ToGroupsAndIndividuals = GetLinks(trans.Id, false),    //"TODO",
                                     Description = trans.Description,
                                    Importance = trans.Importance,
                                    MonetaryAmount = trans.MonetaryAmount,
                                    MinFine = trans.MinFine,
                                    MaxFine = trans.MaxFine,
                                    MinSentenceYears = trans.MinSentenceYears,
                                    MaxSentenceYears = trans.MaxSentenceYears,
                                    StartDate = trans.StartDate,
                                    EndDate = trans.EndDate,
                                    JuristictionId = trans.JuristictionId,
                                    TwitterPostUrl = trans.TwitterPostUrl,
                                    YouTubeUrl = trans.YouTubeUrl,
                                    NewspaperArticleUrl = trans.NewspaperArticleUrl,
                                    Active = trans.Active,
                                    Creator = trans.Creator,
                                    Created = trans.Created,
                                    UpdatedBy = trans.UpdatedBy,
                                    LastUpdated = trans.LastUpdated
                                 })
                         .ToList();

            //foreach (var tran in transactions.ToList())
            //{
            //    string[] lst = (from grp in context.CbGroups
            //             join grpXtran in context.CbGroupXTransactions 
            //                    on grp.Id equals grpXtran.GroupId
            //                    and grpXtran.Direction = true
            //             select new { Grp = grp.Name }).ToList();

            //}

            /*
             from transaction in context.CbTransactions
                join user in context.CbIndividuals on transaction equals user.Owner into gj
                from subpet in gj.DefaultIfEmpty()
                select new { transaction.FirstName, PetName = subpet?.Name ?? String.Empty };
             */



            Response.Headers.Add("X-Total-Count", transactions.Count.ToString());

            return transactions;

            //.ToArray();
        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> Get(int id)
        {
            var _context = new CrimeBoardContext();

            var trans = await _context.CbTransactions.FindAsync(id);

            if (trans == null)
            {
                return NotFound();
            }
            else 
            {
                Transaction transaction = new Transaction
                {
                    Id = trans.Id,
                    //ByGroupsAndIndividuals = GetLinks(trans.Id, true),    //Ignore here for now
                    //ToGroupsAndIndividuals = GetLinks(trans.Id, false),   //Ignore here for now
                    Description = trans.Description,
                    Importance = trans.Importance,
                    MonetaryAmount = trans.MonetaryAmount,
                    MinFine = trans.MinFine,
                    MaxFine = trans.MaxFine,
                    MinSentenceYears = trans.MinSentenceYears,
                    MaxSentenceYears = trans.MaxSentenceYears,
                    StartDate = trans.StartDate,
                    EndDate = trans.EndDate,
                    JuristictionId = trans.JuristictionId,
                    TwitterPostUrl = trans.TwitterPostUrl,
                    YouTubeUrl = trans.YouTubeUrl,
                    NewspaperArticleUrl = trans.NewspaperArticleUrl,
                    Active = trans.Active,
                    Creator = trans.Creator,
                    Created = trans.Created,
                    UpdatedBy = trans.UpdatedBy,
                    LastUpdated = trans.LastUpdated
                };

                return transaction;
            }
        }

        private static string GetLinks(int transactionId, bool direction)
        {

            string list = "";

            using (var context = new CrimeBoardContext())
            {
                List<string> lst = (from grp in context.CbGroups
                           join grpXtran in context.CbGroupXTransactions
                           on grp.Id equals grpXtran.GroupId
                           join tran in context.CbTransactions
                           on grpXtran.TransactionId equals transactionId
                           where grpXtran.Direction == direction
                           select grp.Name)
                         .Distinct()                                                //TODO - inefficient & susceptiple
                         .ToList();

                List<string> lst2 = (from indv in context.CbIndividuals
                                    join indvXtran in context.CbIndividualXTransactions
                                    on indv.Id equals indvXtran.IndividualId
                                    join tran in context.CbTransactions
                                    on indvXtran.TransactionId equals transactionId
                                    where indvXtran.Direction == direction
                                    select indv.Surname +", "+ indv.Firstname)
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
        public IEnumerable<GroupAndIndiv> GetAllGroupsAndIndivs(string country)
        {
            var context = new CrimeBoardContext();
            var groupsAndIndivs = (from groups in context.CbGroups
                                              .Where(t => t.Active == true)
                                              .OrderBy(s => s.Name)
                                 select new GroupAndIndiv
                                 {
                                     value = 'G'+ groups.Id.ToString(),
                                     label = groups.Name        //TODO: remove group "(<description>)" e.g. shorthand for input box on frontend
                                 })
                         .ToList();

            var groupsAndIndivs2 = (from indivs in context.CbIndividuals
                                              //.Where(t => t.Active == true)
                                              .OrderBy(s => s.Surname).ThenBy(f => f.Firstname)
                                   select new GroupAndIndiv
                                   {
                                       value = indivs.Id.ToString(),
                                       label = indivs.Surname + ", " + indivs.Firstname
                                   })
                         .ToList();

            groupsAndIndivs.AddRange(groupsAndIndivs2);

            return groupsAndIndivs;
        }


        // PUT: Transaction/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, PutDataTransaction putDataTransaction)
        {
            using (var _context = new CrimeBoardContext())
            {
                //TODO: Validate data changes

                if (id != putDataTransaction.Id)      /// ???
                {
                    return BadRequest();
                }

                string oldValue = "";
                putDataTransaction.NewValue = putDataTransaction.NewValue.Trim();

                CbTransaction cbTransaction = _context.CbTransactions.FirstOrDefault(x => x.Id == id);

                switch (putDataTransaction.FieldName.ToLower())
                {
                    case "description":
                        oldValue = cbTransaction.Description;
                        cbTransaction.Description = putDataTransaction.NewValue;
                        break;
                    case "importance":
                        oldValue = Convert.ToString(cbTransaction.Importance);
                        cbTransaction.Importance = putDataTransaction.GetInteger();
                        break;
                    case "monetaryamount":
                        oldValue = Convert.ToString(cbTransaction.MonetaryAmount);
                        cbTransaction.MonetaryAmount = putDataTransaction.GetDecimal();
                        break;
                    case "minfine":
                        oldValue = Convert.ToString(cbTransaction.MinFine);
                        cbTransaction.MinFine = putDataTransaction.GetDecimal();
                        break;
                    case "maxfine":
                        oldValue = Convert.ToString(cbTransaction.MaxFine);
                        cbTransaction.MaxFine = putDataTransaction.GetDecimal();
                        break;
                    case "minsentenceyears":
                        oldValue = Convert.ToString(cbTransaction.MinSentenceYears);
                        cbTransaction.MinSentenceYears = putDataTransaction.GetInteger();
                        break;
                    case "maxsentenceyears":
                        oldValue = Convert.ToString(cbTransaction.MaxSentenceYears);
                        cbTransaction.MaxSentenceYears = putDataTransaction.GetInteger();
                        break;
                    case "startdate":
                        oldValue = Convert.ToString(cbTransaction.StartDate);
                        cbTransaction.StartDate = putDataTransaction.GetDateTime();
                        break;
                    case "enddate":
                        oldValue = Convert.ToString(cbTransaction.EndDate);
                        cbTransaction.EndDate = putDataTransaction.GetDateTime();
                        break;
                    case "juristictionid":
                        oldValue = Convert.ToString(cbTransaction.JuristictionId);
                        cbTransaction.JuristictionId = putDataTransaction.GetInteger();
                        break;
                    case "TwitterPostURL":
                        oldValue = cbTransaction.TwitterPostUrl;
                        cbTransaction.TwitterPostUrl = putDataTransaction.NewValue;
                        break;
                    case "youtubeurl":
                        oldValue = cbTransaction.YouTubeUrl;
                        cbTransaction.YouTubeUrl = putDataTransaction.NewValue;
                        break;
                    case "newspaperarticleurl":
                        oldValue = cbTransaction.NewspaperArticleUrl;
                        cbTransaction.NewspaperArticleUrl = putDataTransaction.NewValue;
                        break;
                    case "active":
                        oldValue = Convert.ToString(cbTransaction.Active);
                        cbTransaction.Active = putDataTransaction.GetBool();
                        break;
                }

                cbTransaction.UpdatedBy = putDataTransaction.UserID;
                cbTransaction.LastUpdated = DateTime.Now;

                _context.Entry(cbTransaction).State = EntityState.Modified;

                try
                {
                    _context.SaveChanges();

                    //Log change by user X

                    CbAudit cbAudit = new CbAudit() { 
                        TableName = "Cb_Transaction",
                        FieldName = putDataTransaction.FieldName,
                        Id = cbTransaction.Id,
                        UserId = putDataTransaction.UserID,
                        DateChanged = DateTime.Now,
                        OldValue = oldValue,
                        NewValue = putDataTransaction.NewValue
                    };
                    _context.CbAudits.Add(cbAudit);

                    await _context.SaveChangesAsync();      //Save changes to both tables here

                    _logger.LogInformation("UPDATE CB_Transaction. Field " + putDataTransaction.FieldName + "=" + putDataTransaction.NewValue);
               }
                catch (Exception ex)    //was: DbUpdateConcurrencyException ex
                {
                    //Log error
                    _logger.LogError("ERROR in Transaction Put()", ex);
                    return Problem(ex.Message, null, 400);
                }

                return Ok(cbTransaction);        //TODO: More efficient to not return record
            }
        }
                
 
        // POST: api/Products
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<IActionResult> Post(PostDataTransaction postDataTransaction)
        {
            using (var _context = new CrimeBoardContext())
            {
                CbTransaction cbTransaction = new CbTransaction()
                {
                    //Id
                    Description = postDataTransaction.Description.Trim(),
                    Importance = postDataTransaction.Importance,
                    MonetaryAmount = postDataTransaction.MonetaryAmount,
                    MinFine = postDataTransaction.MinFine,
                    MaxFine = postDataTransaction.MinFine,
                    MinSentenceYears = postDataTransaction.MinSentenceYears,
                    MaxSentenceYears = postDataTransaction.MaxSentenceYears,
                    StartDate = postDataTransaction.StartDate,
                    EndDate = postDataTransaction.EndDate,
                    JuristictionId = postDataTransaction.JuristictionID,
                    TwitterPostUrl = postDataTransaction.TwitterPostURL.Trim(),
                    YouTubeUrl = postDataTransaction.YouTubeURL.Trim(),
                    NewspaperArticleUrl = postDataTransaction.NewspaperArticleURL.Trim(),
                    Active = true,  //postDataTransaction.Active,
                    Creator = postDataTransaction.UserID,
                    Created = DateTime.Now
                };

                _context.CbTransactions.Add(cbTransaction);

                try
                {
                    _context.SaveChanges();

                    _logger.LogInformation("ADDED CB_Transaction. Id=" + postDataTransaction.Id);

                    //From
                    foreach (string fromId in postDataTransaction.From)
                    {
                        int id = int.Parse(fromId.Replace("G", ""));

                        if (fromId.StartsWith("G"))
                        {
                            CbGroupXTransaction cbGroupXTransaction = new CbGroupXTransaction()
                            {
                                GroupId = id,
                                TransactionId = cbTransaction.Id,
                                Direction = true                //e.g. Ind -- Trans <=T= Group
                            };

                            _context.CbGroupXTransactions.Add(cbGroupXTransaction);
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("ADDED CB_GroupXTransaction. GroupId=" + cbGroupXTransaction.GroupId + ", TransactionId=" + cbGroupXTransaction.TransactionId);
                        }
                        else
                        {
                            CbIndividualXTransaction cbIndividualXTransaction = new CbIndividualXTransaction()
                            {
                                IndividualId = id,
                                TransactionId = cbTransaction.Id,
                                Direction = true                //e.g. Ind =T=> Trans -- Group
                            };

                            _context.CbIndividualXTransactions.Add(cbIndividualXTransaction);
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("ADDED CB_IndividualXTransaction. IndividualId=" + cbIndividualXTransaction.IndividualId + ", TransactionId=" + cbIndividualXTransaction.TransactionId);
                        }
                    }

                    //To
                    foreach (string toId in postDataTransaction.To)
                    {
                        int id = int.Parse(toId.Replace("G", ""));

                        if (toId.StartsWith("G"))
                        {
                            CbGroupXTransaction cbGroupXTransaction = new CbGroupXTransaction()
                            {
                                GroupId = id,
                                TransactionId = cbTransaction.Id,
                                Direction = false                //e.g. Ind -- Trans <=F= Group
                                                                    //     Ind -- Trans =T=> Group (equivalent)
                            };

                            _context.CbGroupXTransactions.Add(cbGroupXTransaction);
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("ADDED CB_GroupXTransaction. GroupId=" + cbGroupXTransaction.GroupId + ", TransactionId=" + cbGroupXTransaction.TransactionId);
                        }
                        else
                        {
                            CbIndividualXTransaction cbIndividualXTransaction = new CbIndividualXTransaction()
                            {
                                IndividualId = id,
                                TransactionId = cbTransaction.Id,
                                Direction = false                //e.g. Ind =F=> Trans -- Group
                                                                    //     Ind <=T= Trans -- Group (equivalent)
                            };

                            _context.CbIndividualXTransactions.Add(cbIndividualXTransaction);
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("ADDED CB_IndividualXTransaction. IndividualId=" + cbIndividualXTransaction.IndividualId + ", TransactionId=" + cbIndividualXTransaction.TransactionId);
                        }
                    }

                    //Might have to move this into foreach loops above e.g. unbatched
                    //await _context.SaveChangesAsync();
                    //_logger.LogInformation("ADDED CB_IndividualXTransaction / CB_GroupXTransaction links.");

                    //Log change by user X

                    CbAudit cbAudit = new CbAudit()
                    {
                        TableName = "Cb_Transaction",
                        FieldName = "- NEW TRANSACTION -",
                        Id = cbTransaction.Id,
                        UserId = cbTransaction.Creator,
                        DateChanged = DateTime.Now,
                        OldValue = "",
                        NewValue = cbTransaction.Description
                    };
                    _context.CbAudits.Add(cbAudit);

                    await _context.SaveChangesAsync();      //Save changes to both tables here

                    _logger.LogInformation("ADDED CB_Transaction. Description=" + cbTransaction.Description);
                }
                catch (Exception ex)    //was: DbUpdateConcurrencyException ex
                {
                    //Log error
                    _logger.LogError("ERROR in Transaction Post()", ex);
                    return Problem(ex.Message, null, 400);
                }

                return Ok(cbTransaction);        //TODO: More efficient to not return record

                //return CreatedAtAction("GetProducts", new { id = products.ProductId }, products);
            }
        }        
        
    }
}
