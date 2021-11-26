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
    public class Individual
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string CommonName { get; set; }
        public string Description { get; set; }
        public string CountryOfBirth { get; set; }
        public int Importance { get; set; }
        public string JobTitle { get; set; }
        public string WikipediaUrl { get; set; }
        public string TwitterHandle { get; set; }
        public string ImageId { get; set; }             //return URL for image
        public int Creator { get; set; }
        public DateTime Created { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class PutDataIndividual
    {
        public int    Id { get; set; }
        public string FieldName { get; set; }
        public string NewValue { get; set; }
        public int    UserID { get; set; }
    }

    public class PostDataIndividual
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string CommonName { get; set; }
        public string Description { get; set; }
        public string CountryOfBirth { get; set; }
        public int    Importance { get; set; }
        public string JobTitle { get; set; }
        public string WikipediaURL { get; set; }
        public string TwitterHandle { get; set; }
        public int ImageId { get; set; }
        public int UserID { get; set; }     //Creator or UpdatedBy
    }

    [ApiController]
    [Route("[controller]")]
    public class IndividualsController : ControllerBase
    {
        //private static readonly string[] Summaries = new[]
        //{
        //    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        //};

        private readonly ILogger<IndividualsController> _logger;

        public IndividualsController(ILogger<IndividualsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Individual> Get()
        {
            var context = new CrimeBoardContext();
            
            var individuals = (from indiv in context.CbIndividuals
                                   //
                               orderby indiv.Surname, indiv.Firstname
                               select new Individual
                                {
                                    Id = indiv.Id,
                                    Firstname = indiv.Firstname,
                                    Surname = indiv.Surname,
                                    CommonName = indiv.CommonName,
                                    Description = indiv.Description,    //will embedded quotes break JSON?
                                    CountryOfBirth = indiv.CountryOfBirth,
                                    Importance = indiv.Importance ?? 0,
                                    JobTitle = indiv.JobTitle,
                                    WikipediaUrl = indiv.WikipediaUrl,
                                    TwitterHandle = indiv.TwitterHandle,
                                    ImageId = "",
                                    Creator = indiv.Creator,
                                    Created = indiv.Created,
                                    UpdatedBy = indiv.UpdatedBy,
                                    LastUpdated = indiv.LastUpdated
                                })
                         .ToList();
            
            Response.Headers.Add("X-Total-Count", individuals.Count.ToString());

            return individuals;

            //.ToArray();
        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Individual>> Get(int id)
        {
            var _context = new CrimeBoardContext();

            var indiv = await _context.CbIndividuals.FindAsync(id);

            if (indiv == null)
            {
                return NotFound();
            }
            else
            {
                Individual individual = new Individual
                {
                    Id = indiv.Id,
                    Firstname = indiv.Firstname,
                    Surname = indiv.Surname,
                    CommonName = indiv.CommonName,
                    Description = indiv.Description,    //will embedded quotes break JSON?
                    CountryOfBirth = indiv.CountryOfBirth,
                    Importance = indiv.Importance ?? 0,
                    JobTitle = indiv.JobTitle,
                    WikipediaUrl = indiv.WikipediaUrl,
                    TwitterHandle = indiv.TwitterHandle,
                    ImageId = "",
                    Creator = indiv.Creator,
                    Created = indiv.Created,
                    UpdatedBy = indiv.UpdatedBy,
                    LastUpdated = indiv.LastUpdated
                };

                return individual;
            }
        }

        //See: https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-5.0&tabs=visual-studio
        // PUT: Individual/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, PutDataIndividual putDataIndividual)
        {
            using (var _context = new CrimeBoardContext())
            {
                //TODO: Validate data changes

                if (id != putDataIndividual.Id)      /// ???
                {
                    return BadRequest();
                }

                putDataIndividual.NewValue = putDataIndividual.NewValue.Trim();
                string oldValue = "";

                CbIndividual cbIndividual = _context.CbIndividuals.FirstOrDefault(x => x.Id == id);

                switch (putDataIndividual.FieldName.ToLower())
                {
                    case "id":
                        break;
                    case "firstname":
                        oldValue = cbIndividual.Firstname;
                        cbIndividual.Firstname = putDataIndividual.NewValue;
                        break;
                    case "surname":
                        oldValue = cbIndividual.Surname;
                        cbIndividual.Surname = putDataIndividual.NewValue;
                        break;
                    case "commonname":
                        if (_context.CbIndividuals.Any(x => x.CommonName == putDataIndividual.NewValue))
                        {
                            return Problem("Individual already exists", null, 400);
                        }

                        oldValue = cbIndividual.CommonName;
                        cbIndividual.CommonName = putDataIndividual.NewValue;
                        break;
                    case "countryofbirth":
                        oldValue = cbIndividual.CountryOfBirth;
                        cbIndividual.CountryOfBirth = putDataIndividual.NewValue;
                        break;
                    case "description":
                        oldValue = cbIndividual.Description;
                        cbIndividual.Description = putDataIndividual.NewValue;
                        break;
                    case "importance":
                        oldValue = Convert.ToString(cbIndividual.Importance);
                        int? importance = int.TryParse(putDataIndividual.NewValue, out int tmp) ? (int?)tmp : null;
                        cbIndividual.Importance = importance;
                        break;
                    case "jobtitle":
                        oldValue = cbIndividual.JobTitle;
                        cbIndividual.JobTitle = putDataIndividual.NewValue;
                        break;
                    case "wikipediaurl":
                        oldValue = cbIndividual.WikipediaUrl;
                        cbIndividual.WikipediaUrl = putDataIndividual.NewValue;
                        break;
                    case "twitterhandle":
                        oldValue = cbIndividual.TwitterHandle;
                        cbIndividual.TwitterHandle = putDataIndividual.NewValue;
                        break;
                }

                cbIndividual.UpdatedBy = putDataIndividual.UserID;
                cbIndividual.LastUpdated = DateTime.Now;

                _context.Entry(cbIndividual).State = EntityState.Modified;
                //

                try
                {
                    _context.SaveChanges();

                    //Log change by user X

                    CbAudit cbAudit = new CbAudit() {
                        TableName = "Cb_Individual",
                        FieldName = putDataIndividual.FieldName,
                        Id = cbIndividual.Id,
                        UserId = putDataIndividual.UserID,
                        DateChanged = DateTime.Now,
                        OldValue = oldValue,
                        NewValue = putDataIndividual.NewValue
                    };
                    _context.CbAudits.Add(cbAudit);

                    await _context.SaveChangesAsync();      //Save changes to both tables here

                    _logger.LogInformation("UPDATE CB_Individual. Field " + putDataIndividual.FieldName + "=" + putDataIndividual.NewValue);
               }
                catch (Exception ex)    //was: DbUpdateConcurrencyException ex
                {
                    //Log error
                    _logger.LogError("ERROR in Individual Put()", ex);
                    return Problem(ex.Message, null, 400);
                }

                return Ok(cbIndividual);        //TODO: More efficient to not return record
            }
        }

        
        // POST: api/Products
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<IActionResult> Post(PostDataIndividual postDataIndividual)
        {
            using (var _context = new CrimeBoardContext())
            {
                if (_context.CbIndividuals.Any(x => x.CommonName == postDataIndividual.CommonName))
                { 
                    return Problem("Individual already exists", null, 400);
                }

                CbIndividual cbIndividual = new CbIndividual()
                {
                    Firstname = postDataIndividual.FirstName.Trim(),
                    Surname = postDataIndividual.Surname.Trim(),
                    CommonName = postDataIndividual.CommonName.Trim(),
                    Description = postDataIndividual.Description.Trim(),
                    CountryOfBirth = postDataIndividual.CountryOfBirth,
                    Importance = 1, //postDataIndividual.Importance,
                    JobTitle = postDataIndividual.JobTitle.Trim(),
                    WikipediaUrl = postDataIndividual.WikipediaURL.Trim(),
                    TwitterHandle = postDataIndividual.TwitterHandle.Trim(),
                    ImageId = -1, //postDataIndividual.ImageId
                    Creator = postDataIndividual.UserID,
                    Created = DateTime.Now
                };

                _context.CbIndividuals.Add(cbIndividual);

                try
                {
                    _context.SaveChanges();

                    //Log change by user X

                    _logger.LogInformation("ADDED CB_Individual. FirstName=" + postDataIndividual.FirstName + ", Surname=" + postDataIndividual.Surname);

                    CbAudit cbAudit = new CbAudit()
                    {
                        TableName = "Cb_Individual",
                        FieldName = "- NEW INDIVIDUAL -",
                        Id = cbIndividual.Id,
                        UserId = postDataIndividual.UserID,
                        DateChanged = DateTime.Now,
                        OldValue = "",
                        NewValue = postDataIndividual.Surname +", "+ postDataIndividual.FirstName
                    };
                    _context.CbAudits.Add(cbAudit);

                    await _context.SaveChangesAsync();      //Save changes to both tables here

                }
                catch (Exception ex)    //was: DbUpdateConcurrencyException ex
                {
                    //Log error
                    _logger.LogError("ERROR in Individual Post()", ex);
                    return Problem(ex.Message, null, 400);
                }

                return Ok(cbIndividual);        //TODO: More efficient to not return record
            }
        }        
        
    }
}
