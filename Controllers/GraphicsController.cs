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
    public class Node
    {
        public int    Id { get; set; }
        public string Name { get; set; }
        public int    IsGroup { get; set; }     //0 = Group, 1 = Individual
        public int    Level { get; set; }        //Importance
        public string ImageUrl { get; set; }
        public string WikipediaUrl { get; set; }
        public string Description { get; set; }
    }

    public class Link
    { 
        public int Source { get; set; }
        public int Target { get; set; }
        public int Type { get; set; }        // 0 = group, 1 = owner etc..
        public string Label { get; set; }
        public string LinkId { get; set; }
    }

    public class Graph
    {
        public Graph()
        {
            nodes = new List<Node>();
            links = new List<Link>();
        }

        public List<Node> nodes;
        public List<Link> links;
    }


    [ApiController]
    [Route("[controller]")]
    public class GraphicsController : ControllerBase
    {
        private readonly ILogger<GraphicsController> _logger;

        private string[] relationshipLabel = {
            "Unassigned/Unknown",
            "Owner",
            "Business Partner",
            "Employee",
            "Friendship",
            "Married",
            "Boyfriend/Girlfriend",
            "Son/Daughter of",
            "Family Relative",
            "Funded/Payed",
            "President/PM",
            "King/Queen/Monarchy",
            "Government Official",
            "Member",
            "Sponsor",
            "Affiliate"
        };

        public GraphicsController(ILogger<GraphicsController> logger)
        {
            _logger = logger;
        }

        //TODO: see https://codereview.stackexchange.com/questions/102389/nested-object-to-hierarchical-object-list

        [HttpGet]
        public Graph Get()
        {
            /***********************************************************************************************************
             * Thoughts: better to use a database View to replace much of this code.  However, SQL Server has always
             * been clunky with hierarchical data.  Their latest kludge/fix is "hierarchyid".
             * As for using a sproc to do this, Entity Framework relies on sprocs returning all fields in all tables 
             * referenced - to maintain internal state I guess.
             * 
             * Requirement: populate the Graph class with Nodes with appropriate Level depending on their position in
             * the CB_Group table.
             * + Add CB_Individual in the Relationships and Transactions as new (unique) Nodes to the Graph.nodes.
             * + Add Links from the Relationships and Transactions between Nodes.
             * *********************************************************************************************************/

            Graph graph = new Graph();          //Holds Nodes list and Links list

            var context = new CrimeBoardContext();
            graph.nodes = GetNodes(context.CbGroups, 1, 0);     //Nodes a.k.a. Groups (1.3 seconds to execute)
            graph.links = GetLinks(context.CbGroups, 1);        //Links between group Nodes (1.07 seconds to execute)

            /*
            DateTime start = DateTime.Now;
            foreach (VwGroupTree group in context.VwGroupTrees)
            {
                graph.nodes.Add(new Node {
                    Id = group.Id ?? 1,
                    Name = group.Name,
                    IsGroup = group.IsGroup ?? 0,
                    Level = group.Level - 1 ?? 0,
                    ImageUrl = "",
                    WikipediaUrl = ("" + group.WikipediaUrl).Replace("\n", "")
                });
            }
            foreach (VwLinkTree link in context.VwLinkTrees)
            {
                graph.links.Add(new Link
                {
                    Source = link.Source ?? 0,
                    Target = link.Target ?? 0,
                    Type = link.Type ?? 0
                });
            }
            graph.nodes.RemoveAll(x => x.Id == 1);
            graph.links.RemoveAll(x => x.Source == 1);

            DateTime end = DateTime.Now;
            var elapse = end - start;
            var test = elapse;
            */

            //Re-attach Countries under Government to root + remove Government node
            graph.nodes.RemoveAll(x => x.Id == 36);
            graph.links.RemoveAll(x => x.Source == 36);
            graph.links.RemoveAll(x => x.Target == 36);

            /*
             * LINKS BETWEEN GROUPS AND INDIVIDUALS + INDIVIDUALS TO INDIVIDUALS
             * 
            
            FROM:                                       TO:
            Groups (dir=1)                              Individuals (dir=0)
            Groups (dir=?)                              Groups (dir=?)          //e.g. ? = untested
            Individuals (dir=1)                         Individuals (dir=0)     //e.g. ? = untested
            Individuals (dir=?)                         Groups (dir=?) 
             */

            /* TODO - clearer but needs rethink..
             *
            var groupsToIndiv = (from rel in context.CbRelationships
                                //Groups to Individuals
                                join grpXrel in context.CbGroupXRelationships on rel.Id equals grpXrel.RelationshipId
                                join indXrel2 in context.CbIndividualXRelationships on rel.Id equals indXrel2.RelationshipId
                                where grpXrel.Direction == true && indXrel2.Direction == false
                                select new
                                {
                                    Source = grpXrel.GroupId,
                                    Target = indXrel2.IndividualId + 100000,
                                    Type = 1       //rel.RelationshipTypeId ?? 0
                                }
            );
            
            //Individuals to Individuals
            var indivToIndiv = (from rel in context.CbRelationships
                                     //Groups to Individuals
                                 join indXrel in context.CbIndividualXRelationships on rel.Id equals indXrel.RelationshipId
                                 join indXrel2 in context.CbIndividualXRelationships on rel.Id equals indXrel2.RelationshipId
                                 where indXrel.Direction == true && indXrel2.Direction == false
                                 select new
                                 {
                                     Source = indXrel.IndividualId + 100000,
                                     Target = indXrel2.IndividualId + 100000,
                                     Type = 1       //rel.RelationshipTypeId ?? 0
                                 }
            );
            
            //Groups to Individuals (Transactions)
            var groupToIndivTrans = (from rel in context.CbTransactions
                                         //Groups to Individuals
                                     join grpXtran in context.CbGroupXTransactions on rel.Id equals grpXtran.TransactionId
                                     join indXtran2 in context.CbIndividualXTransactions on rel.Id equals indXtran2.TransactionId
                                     where grpXtran.Direction == true && indXtran2.Direction == false
                                     select new
                                     {
                                         Source = grpXtran.GroupId,
                                         Target = indXtran2.IndividualId + 100000,
                                         Type = 2       //rel.RelationshipTypeId ?? 0
                                     }
            );

            //Individuals to Groups (Transactions)
            var indivToGroupTrans = (from rel in context.CbTransactions
                                         //Groups to Individuals
                                     join indXtran in context.CbIndividualXTransactions on rel.Id equals indXtran.TransactionId
                                     join grpXtran in context.CbGroupXTransactions on rel.Id equals grpXtran.TransactionId
                                     where grpXtran.Direction == true && grpXtran.Direction == false
                                     select new
                                     {
                                         Source = indXtran.IndividualId + 100000,
                                         Target = grpXtran.GroupId,
                                         Type = 2       //rel.RelationshipTypeId ?? 0
                                     }
            );

            //Individuals to Individuals
            var indivToIndivTrans = (from rel in context.CbTransactions
                                         //Groups to Individuals
                                     join indXtran in context.CbIndividualXTransactions on rel.Id equals indXtran.TransactionId
                                     join indXtran2 in context.CbIndividualXTransactions on rel.Id equals indXtran2.TransactionId
                                     where indXtran.Direction == true && indXtran2.Direction == false
                                     select new
                                     {
                                         Source = indXtran.IndividualId + 100000,
                                         Target = indXtran2.IndividualId + 100000,
                                         Type = 2       //rel.RelationshipTypeId ?? 0
                                     }
            );

            //TODO: Now we have all the CB_Relationship + CB_Transaction nodes and links, so need to add all these
            // Individuals to graph.nodes and all these links to graph.links.

            var unionResult = groupsToIndiv
                .Union(indivToIndiv)
                .Union(groupToIndivTrans)
                .Union(indivToGroupTrans)
                .Union(indivToIndivTrans);

            var unionResultDistinct = unionResult.Distinct();

            foreach (var u in unionResultDistinct) {
                if (u.Source >= 100000)
                {
                    graph.nodes.Add(new Node() { 
                         Id = u.Source,
                         Description = u.de
                    });
                }
            };


            return graph;
            */


            //DateTime start = DateTime.Now;
            // (2.03 seconds to execute)
            foreach (CbRelationship rel in context.CbRelationships)
            {
                //Groups to Individuals
                foreach (var grpXrel in context.CbGroupXRelationships.Where(q => q.RelationshipId == rel.Id && q.Direction == true))
                {
                    if (!graph.nodes.Exists(n => n.Id == grpXrel.GroupId))
                    {
                        AddMissingGroupNode(ref context, ref graph.nodes, grpXrel.GroupId);
                    }

                    foreach (var indXrel2 in context.CbIndividualXRelationships.Where(q => q.RelationshipId == rel.Id && q.Direction == false))
                    {
                        if (!graph.nodes.Exists(n => n.Id == indXrel2.IndividualId + 100000))
                        {
                            AddMissingIndivNode(ref context, ref graph.nodes, indXrel2.IndividualId);
                        }

                        graph.links.Add(new Link()
                        {
                            Label = relationshipLabel[rel.RelationshipTypeId ?? 0],
                            Source = grpXrel.GroupId,
                            Target = indXrel2.IndividualId + 100000,
                            Type = 1,       //rel.RelationshipTypeId ?? 0
                            LinkId = "Rel-"+ rel.Id            //No link from relationships
                        });
                    }
                }

                //Individuals to Groups
                /*
                 * Commenting out as don't currently allow this in the Frontend Relations tab
                 * 
                foreach (var indXrel in context.CbIndividualXRelationships.Where(q => q.RelationshipId == rel.Id && q.Direction == true))
                {
                    if (!graph.nodes.Exists(n => n.Id == indXrel.IndividualId + 100000))
                    {
                        AddMissingIndivNode(ref context, ref graph.nodes, indXrel.IndividualId);
                    }

                    foreach (var grpXrel in context.CbGroupXRelationships.Where(q => q.RelationshipId == rel.Id && q.Direction == ?))
                    {
                        if (!graph.nodes.Exists(n => n.Id == grpXrel.GroupId))
                        {
                            AddMissingGroupNode(ref context, ref graph.nodes, grpXrel.GroupId);
                        }

                        graph.links.Add(new Link()
                        {
                            Source = grpXrel.GroupId,
                            Target = indXrel.IndividualId + 100000,
                            Type = 1       //rel.RelationshipTypeId ?? 0
                        });
                    }
                }
                */

                //Individuals to Individuals
                foreach (var indXrel in context.CbIndividualXRelationships.Where(q => q.RelationshipId == rel.Id && q.Direction == true))
                {
                    if ( ! graph.nodes.Exists(n => n.Id == indXrel.IndividualId + 100000))
                    {
                        AddMissingIndivNode(ref context, ref graph.nodes, indXrel.IndividualId);
                    }

                    foreach (var indXrel2 in context.CbIndividualXRelationships.Where(q => q.RelationshipId == rel.Id && q.Direction == false))
                    {
                        if (!graph.nodes.Exists(n => n.Id == indXrel2.IndividualId + 100000))
                        {
                            AddMissingIndivNode(ref context, ref graph.nodes, indXrel2.IndividualId);
                        }

                        graph.links.Add(new Link() { 
                            Label = relationshipLabel[rel.RelationshipTypeId ?? 0],
                            Source = indXrel.IndividualId + 100000,
                            Target = indXrel2.IndividualId + 100000,
                            Type = 1,       //rel.RelationshipTypeId ?? 0
                            LinkId = "Rel-" + rel.Id            //No link from relationships
                        });
                    }
                }
            }


            //graph.links.AddRange(
            //    (from grpXind in context.CbGroupXRelationships  //Links between group Nodes and Individuals
            //     orderby grpXind.Created
            //     select new Link
            //     {
            //         Source = grpXind.GroupId,
            //         Target = grpXind.IndividualId + 100000,   //100,000 added so as not to mix group Ids with individual Ids
            //         Type = 0
            //     }).ToList()
            //);


            // TODO: Transactions between groups/individuals X groups/individuals
            // Maybe source from a view instead of tables?
            /*
                * pseudo code...
                * 
            graph.links.AddRange(
                (from grpXind in context.CbGroupXTransactions  //Links between group Nodes and Individuals
                    orderby grpXind.Created
                    select new Link
                    {
                        Source = grpXind.GroupId,
                        Target = grpXind.IndividualId + 100000,   //100,000 added so as not to mix group Ids with individual Ids
                        Type = 1
                    }).ToList()
                );
            */

            foreach (CbTransaction tran in context.CbTransactions)
            {
                //Groups to Groups
                foreach (var grpXtran in context.CbGroupXTransactions.Where(q => q.TransactionId == tran.Id && q.Direction == true))
                {
                    if (!graph.nodes.Exists(n => n.Id == grpXtran.GroupId))
                    {
                        AddMissingGroupNode(ref context, ref graph.nodes, grpXtran.GroupId);
                    }

                    foreach (var grpXtran2 in context.CbGroupXTransactions.Where(q => q.TransactionId == tran.Id && q.Direction == false))
                    {
                        if (!graph.nodes.Exists(n => n.Id == grpXtran2.GroupId))
                        {
                            AddMissingGroupNode(ref context, ref graph.nodes, grpXtran2.GroupId);
                        }

                        string socialLinks = new string("");
                        socialLinks += tran.TwitterPostUrl.StartsWith("http") ? "Twitter|" : "";
                        socialLinks += tran.YouTubeUrl.StartsWith("http") ? "YouTube|" : "";
                        socialLinks += tran.NewspaperArticleUrl.StartsWith("http") ? "News Media" : "";
                        socialLinks = socialLinks.Trim('|');

                        graph.links.Add(new Link()
                        {
                            Label = socialLinks,
                            Source = grpXtran.GroupId,
                            Target = grpXtran2.GroupId,
                            Type = 1,       //rel.TransactionTypeId ?? 0
                            LinkId = "Trans-" + tran.Id            //to show popup
                        });
                    }
                }

                //Groups to Individuals
                foreach (var grpXtran in context.CbGroupXTransactions.Where(q => q.TransactionId == tran.Id && q.Direction == true))
                {
                    //if (!graph.nodes.Exists(n => n.Id == grpXtran.GroupId))
                    //{
                    //    AddMissingGroupNode(ref context, ref graph.nodes, grpXtran.GroupId);
                    //}

                    foreach (var indXtran2 in context.CbIndividualXTransactions.Where(q => q.TransactionId == tran.Id && q.Direction == false))
                    {
                        if (!graph.nodes.Exists(n => n.Id == indXtran2.IndividualId + 100000))
                        {
                            AddMissingIndivNode(ref context, ref graph.nodes, indXtran2.IndividualId);
                        }

                        string socialLinks = new string("");
                        socialLinks += tran.TwitterPostUrl.StartsWith("http") ? "Twitter|" : "";
                        socialLinks += tran.YouTubeUrl.StartsWith("http") ? "YouTube|" : "";
                        socialLinks += tran.NewspaperArticleUrl.StartsWith("http") ? "News Media" : "";
                        socialLinks = socialLinks.Trim('|');

                        graph.links.Add(new Link()
                        {
                            Label = socialLinks,               //TODO: e.g. "money laudering"
                            Source = grpXtran.GroupId,
                            Target = indXtran2.IndividualId + 100000,
                            Type = 2,       //rel.TransactionTypeId ?? 0
                            LinkId = "Trans-" + tran.Id            //to show popup
                        });
                    }
                }

                /* Needs work + checking...
                 */
                //Individuals to Groups
                foreach (var indXtran in context.CbIndividualXTransactions.Where(q => q.TransactionId == tran.Id && q.Direction == true))
                {
                    if (!graph.nodes.Exists(n => n.Id == indXtran.IndividualId + 100000))
                    {
                        AddMissingIndivNode(ref context, ref graph.nodes, indXtran.IndividualId);
                    }

                    foreach (var grpXtran in context.CbGroupXTransactions.Where(q => q.TransactionId == tran.Id && q.Direction == false))
                    {
                        if (!graph.nodes.Exists(n => n.Id == grpXtran.GroupId))
                        {
                            AddMissingGroupNode(ref context, ref graph.nodes, grpXtran.GroupId);
                        }

                        string socialLinks = new string("");
                        socialLinks += tran.TwitterPostUrl.StartsWith("http") ? "Twitter|" : "";
                        socialLinks += tran.YouTubeUrl.StartsWith("http") ? "YouTube|" : "";
                        socialLinks += tran.NewspaperArticleUrl.StartsWith("http") ? "News Media" : "";
                        socialLinks = socialLinks.Trim('|');

                        graph.links.Add(new Link()
                        {
                            Label = socialLinks,               //TODO: e.g. "money laudering"
                            Source = indXtran.IndividualId + 100000,
                            Target = grpXtran.GroupId,
                            Type = 2,       //rel.TransactionTypeId ?? 0
                            LinkId = "Trans-" + tran.Id            //to show popup
                        });
                    }
                }
                /* */

                //Individuals to Individuals
                foreach (var indXtran in context.CbIndividualXTransactions.Where(q => q.TransactionId == tran.Id && q.Direction == true))
                {
                    if (!graph.nodes.Exists(n => n.Id == indXtran.IndividualId + 100000))
                    {
                        AddMissingIndivNode(ref context, ref graph.nodes, indXtran.IndividualId);
                    }

                    foreach (var indXtran2 in context.CbIndividualXTransactions.Where(q => q.TransactionId == tran.Id && q.Direction == false))
                    {
                        if (!graph.nodes.Exists(n => n.Id == indXtran2.IndividualId + 100000))
                        {
                            AddMissingIndivNode(ref context, ref graph.nodes, indXtran2.IndividualId);
                        }

                        string socialLinks = new string("");
                        socialLinks += tran.TwitterPostUrl.StartsWith("http") ? "Twitter|" : "";
                        socialLinks += tran.YouTubeUrl.StartsWith("http") ? "YouTube|" : "";
                        socialLinks += tran.NewspaperArticleUrl.StartsWith("http") ? "News Media" : "";
                        socialLinks = socialLinks.Trim('|');

                        graph.links.Add(new Link()
                        {
                            Label = socialLinks,               //TODO: e.g. "money laudering"
                            Source = indXtran.IndividualId + 100000,
                            Target = indXtran2.IndividualId + 100000,
                            Type = 2,       //rel.TransactionTypeId ?? 0
                            LinkId = "Trans-" + tran.Id            //to show popup
                        });
                    }
                }
            }
            //DateTime end = DateTime.Now;
            //var elapse = end - start;               // (2.03 seconds to execute)

            return graph;
        }
        
        private void AddMissingGroupNode(ref CrimeBoardContext context, ref List<Node> nodes, int id)
        {
            CbGroup grp = context.CbGroups.Where(x => x.Id == id).FirstOrDefault();
            if (grp != null)
            {
                nodes.Add(new Node()
                {
                    Id = id,        // == ind.Id
                    Name = grp.Name,
                    Description = grp.Description,
                    WikipediaUrl = (""+ grp.WikipediaUrl).Replace("\n", ""),    //TODO: SQL server won't remove these characters
                    IsGroup = 0,
                    Level = 4       //TODO: may need to change this / think about
                });
            }
        }
        
        private void AddMissingIndivNode(ref CrimeBoardContext context, ref List<Node> nodes, int id)
        {
            CbIndividual ind = context.CbIndividuals.Where(x => x.Id == id).FirstOrDefault();
            if (ind != null)
            {
                nodes.Add(new Node()
                {
                    Id = id + 100000,        // == ind.Id
                    Name = ind.Firstname + " " + ind.Surname,
                    Description = ind.Description,
                    WikipediaUrl = ind.WikipediaUrl,
                    IsGroup = 1,
                    Level = 4       //TODO: may need to change this / think about
                });
            }
        }

        private List<Node> GetNodes(DbSet<CbGroup> source, int parentId, int level)
        {
            var nodes = new List<Node>();

            var children = source.Where(x => x.ParentId == parentId).OrderBy(x => x.Name).ToList();

            foreach (var child in children)
            {
                nodes.Add(new Node()
                {
                    Id = child.Id,
                    Name = child.Name,
                    IsGroup = 0,
                    Description = child.Description,
                    WikipediaUrl = (""+ child.WikipediaUrl).Replace("\n", ""),    //TODO: SQL server won't remove these characters
                    Level = child.ParentId == 36 ? 0 : level    //Re-attach Countries under Government to root
                });

                nodes.AddRange(GetNodes(source, child.Id, level + 1));
            }

            //GetChildren is called recursively again for every child found
            //and this process repeats until no childs are found for given node, 
            //in which case an empty list is returned
            //children.ForEach(x => x.ChildCbGroups = GetChildren(source, x.Id));
            //children.ForEach(x => GetNodes(source, x.Id, level+1));
            return nodes;
        }

        private List<Link> GetLinks(DbSet<CbGroup> source, int parentId)
        {
            //Could pass <T> into GetNodes e.g. combine, but this is clearer having them separate

            var children = source.Where(x => x.ParentId == parentId).OrderBy(x => x.Name).ToList();

            var links = new List<Link>();

            if (parentId > 1)   //Skip root node "Crime Board"
            { 
                foreach (var child in children)
                {
                    links.Add(new Link()
                    {
                        Label = "",
                        Source = parentId,
                        Target = child.Id,
                        Type = 0,
                        LinkId = ""
                    });
                }
            }
            //GetChildren is called recursively again for every child found
            //and this process repeats until no childs are found for given node, 
            //in which case an empty list is returned
            //children.ForEach(x => x.ChildCbGroups = GetChildren(source, x.Id));
            children.ForEach(x => links.AddRange( GetLinks(source, x.Id) ));
            return links;
        }

    }
}
