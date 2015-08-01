using System;
using System.Linq;
using System.ServiceModel.Activation;
using System.Xml;
using System.Collections.Generic;
using System.Data.Objects;
using System.ServiceModel;
using System.IO;

namespace RssWCF.Web
{
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RssService : IRssService
    {
        /// <summary>
        /// A reference to the Data Access Layer project
        /// </summary>
        private RssEntities context;

        public RssService()
        {
            context = new RssEntities();
        }

        #region IService Implementation
        public List<Feed> GetFeeds()
        {
            List<Feed> feeds = null;
            try
            {
                feeds=context.Feeds.OrderBy(p => p.Name).ToList();
            }
            catch (Exception e)
            {
                string msg = e.Message;
                string reason = "GetFeeds Exception";
                throw new FaultException<DBRequestFault>
                    (new DBRequestFault(msg), reason);
            }
            return feeds;
        }

        public void AddFeed(string name, string url)
        {
            Feed newFeed = new Feed
            {
                ID = Guid.NewGuid(),
                Name = name,
                URL = url
            };
            try
            {
                context.Feeds.AddObject(newFeed);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                string msg = e.Message;
                string reason = "AddFeed Feeds.AddObject Exception";
                throw new FaultException<DBRequestFault>
                    (new DBRequestFault(msg), reason);
            }
            try
            {
                List<News> feedNews = LoadNewsFromFeed(newFeed);
                foreach (News item in feedNews)
                {
                    context.News.AddObject(item);
                }
                context.SaveChanges();
            }
            catch (Exception e)
            {
                string msg = e.Message;
                string reason = "AddFeed News.AddObjectt Exception";
                throw new FaultException<DBRequestFault>
                    (new DBRequestFault(msg), reason);
            }
        }
        public void AddFeed(Feed addfeed)
        {
            using (RssEntities addcontext = new RssEntities())
            {

                try
                {
                    addcontext.Feeds.AddObject(addfeed);
                    addcontext.SaveChanges();
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    string reason = "AddFeed Feeds.AddObject Exception";
                    throw new FaultException<DBRequestFault>
                        (new DBRequestFault(msg), reason);
                }
                try
                {
                    List<News> feedNews = LoadNewsFromFeed(addfeed);
                    foreach (News item in feedNews)
                    {
                        addcontext.News.AddObject(item);
                    }
                    addcontext.SaveChanges();
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    string reason = "AddFeed News.AddObjectt Exception";
                    throw new FaultException<DBRequestFault>
                        (new DBRequestFault(msg), reason);
                }
            }
        }
        public void RemoveFeed(Guid feed_id)
        {
            try
            {
                Feed removefeed = context.Feeds.FirstOrDefault(p => p.ID == feed_id);
                context.Feeds.DeleteObject(removefeed);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                string msg = e.Message;
                string reason = "RemoveFeed Exception";
                throw new FaultException<DBRequestFault>
                    (new DBRequestFault(msg), reason);
            }
        }


        /// <summary>
        /// Retrieves remote rss feeds, parses it and downloads news to the database
        /// </summary>
        /// <param name="feed"></param>
        /// <returns></returns>
        public Boolean GetNews()
        {
            bool result = false;
            try
            {
                if (context.News.Count() != 0)
                {
                    IEnumerable<News> AllNews = context.News;
                    foreach (News item in AllNews)
                    {
                        context.News.DeleteObject(item);
                    }
                    context.SaveChanges();
                }

                foreach (Feed feed in context.Feeds)
                {
                    List<News> feedNews = LoadNewsFromFeed(feed);
                    foreach (News item in feedNews)
                    {
                        context.News.AddObject(item);
                    }
                }
                context.SaveChanges();
            }
            catch (Exception e)
            {
                string msg = e.Message;
                string reason = "GetNews Exception";
                throw new FaultException<DBRequestFault>
                    (new DBRequestFault(msg), reason);
            }
            return result;

        }

        public void RemoveNews(Guid news_id)
        {
            News remNews = context.News.FirstOrDefault(n => n.ID == news_id);
            try
            {
                context.News.DeleteObject(remNews);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                string msg = e.Message;
                string reason = "GetNews Exception";
                throw new FaultException<DBRequestFault>
                    (new DBRequestFault(msg), reason);
            }
        }

        public void RemoveAllNews()
        {
            IEnumerable<News> news = context.News;
            try
            {
                foreach (News item in news)
                {
                    context.News.DeleteObject(item);
                }
                context.SaveChanges();
            }
            catch (Exception e)
            {
                string msg = e.Message;
                string reason = "RemoveAllNews Exception";
                throw new FaultException<DBRequestFault>
                    (new DBRequestFault(msg), reason);
            }
        }

        public void FetchAllFeedNews()
        {
            try
            {
                ObjectSet<Feed> feeds = context.Feeds;
                foreach (Feed item in feeds)
                {
                    UpdateFeed(item);
                }
                context.SaveChanges();
            }
            catch (Exception e)
            {
                string msg = e.Message;
                string reason = "FetchAllFeedNews Exception";
                throw new FaultException<DBRequestFault>
                    (new DBRequestFault(msg), reason);
            }
        }

        #endregion

        #region Advanced methods

        private void UpdateFeed(Feed feed)
        {
            try
            {

                using (XmlReader reader = XmlReader.Create(feed.URL))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(reader);
                    //compare the item.LastbuildDate
                    int result = DateTime.Compare(feed.LastTimeUpdate, GetLastBuildDate(xmlDoc));
                    if (result < 0 || feed.News.Count == 0)
                    {
                        using (RssEntities localContext = new RssEntities())
                        {
                            Feed delFeed = localContext.Feeds.First(f => f.ID == feed.ID);
                            localContext.Feeds.DeleteObject(delFeed);
                            localContext.SaveChanges();


                            using (RssEntities addContext = new RssEntities())
                            {
                                Feed addfeed = new Feed
                                {
                                    ID = feed.ID,
                                    Name = feed.Name,
                                    URL = feed.URL
                                };
                                addContext.Feeds.AddObject(addfeed);
                                addContext.SaveChanges();
                                List<News> feedNews = LoadNewsFromFeed(addfeed);
                                foreach (News item in feedNews)
                                {
                                    addContext.News.AddObject(item);
                                }
                                addContext.SaveChanges();
                            }

                        }
                    }
                }
            }
            catch (FileNotFoundException fnfEx)
            {
                string msg = fnfEx.Message;
                string reason = "UpdateFeed FileNotFoundException";
                throw new FaultException<DBRequestFault>
                    (new DBRequestFault(msg), reason);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                string reason = "UpdateFeed Exception";
                throw new FaultException<DBRequestFault>
                    (new DBRequestFault(msg), reason);
            }
        }

        private List<News> LoadNewsFromFeed(Feed feed)
        {
            try
            {
                if (String.IsNullOrEmpty(feed.URL))
                {
                    throw new ArgumentException("You must provide a feed URL");
                }
                //start the parsing process
                using (XmlReader reader = XmlReader.Create(feed.URL))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(reader);
                    feed.LastTimeUpdate = GetLastBuildDate(xmlDoc);
                    //parse the item
                    List<News> news = ParseRssNewsItems(xmlDoc);
                    news.ForEach(el =>
                    {
                        el.FeedId = feed.ID;
                    });
                    return news;
                }

            }
            catch (ArgumentException argEx)
            {
                string msg = argEx.Message;
                string reason = "LoadNewsFromFeed ArgumentException";
                throw new FaultException<DBRequestFault>
                    (new DBRequestFault(msg), reason);
            }
        }

        /// <summary>
        /// Parses the xml document in order to retrieve the RSS items
        /// </summary>
        /// <param name="xDoc"></param>
        /// <param name="news"></param>
        private List<News> ParseRssNewsItems(XmlDocument xDoc)
        {
            List<News> news = new List<News>();
            XmlNodeList nodes = xDoc.SelectNodes("rss/channel/item");
            foreach (XmlNode node in nodes)
            {
                News item = new News()
                {
                    ID = Guid.NewGuid(),
                    Title = ParseDocElement(node, "title"),
                    Content = ParseDocElement(node, "description"),
                    SourceLink = ParseDocElement(node, "link")
                };
                news.Add(item);
            }
            return news;
        }
        /// <summary>
        /// Parses the XmlNode with the specified XPath query and assigns
        /// the value tothe property parameter
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="xPath"></param>
        /// <param name="property"></param>
        private string ParseDocElement(XmlNode parent, string xPath)
        {
            XmlNode node = parent.SelectSingleNode(xPath);
            if (node != null)
            {
                return node.InnerText;
            }
            else
            {
                return "Unresolvable";
            }
        }

        ///
        private DateTime GetLastBuildDate(XmlDocument xDoc)
        {
            DateTime buildDate;
            XmlNode buildDateNode = xDoc.SelectSingleNode("rss/channel/lastBuildDate");
            if (buildDateNode != null)
            {
                buildDate = DateTime.Parse(buildDateNode.InnerText);
            }
            else
            {
                buildDate = DateTime.Now;
            }
            return buildDate;
        }
        #endregion
    }
}
