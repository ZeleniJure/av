using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ServiceModel.Syndication;
using System.Xml;
using System.IO;
using naloga1.Models;

namespace naloga1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewsStatisticsController : ControllerBase
    {
        private class Cache
        {
            public bool HasNewData { get; set; }
            public IList<NewsSummary> Data { get; set; }
        }

        /*
        Will use this for caching parsed XML data (instead of MemoryCache)
        */
        private static Cache _cache = new Cache
        {
            HasNewData = true,
            Data = loadData()
        };

        private static IList<NewsSummary> loadData(String filePath = "rss.xml")
        {
            // default here works for debug...
            using (XmlReader reader = XmlReader.Create(filePath))
            {
                return loadData(reader);
            }
        }

        private static IList<NewsSummary> loadData(XmlReader reader)
        {
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            Dictionary<String, NewsSummary> fakeDatabase = new Dictionary<String, NewsSummary>();

            foreach (SyndicationItem item in feed.Items)
            {
                foreach (SyndicationCategory category in item.Categories)
                {
                    // Let's trust the data (mistaaaake!) and assume category can be used for the key
                    if (fakeDatabase.ContainsKey(category.Name))
                    {
                        NewsSummary ns = fakeDatabase[category.Name];
                        if (ns.NewestNewsDate < item.PublishDate) ns.NewestNewsDate = item.PublishDate;
                        ns.NumberOfNews += 1;
                    }
                    else
                    {
                        NewsSummary ns = new NewsSummary
                        {
                            Category = category.Name,
                            NewestNewsDate = item.PublishDate,
                            NumberOfNews = 1
                        };
                        fakeDatabase.Add(category.Name, ns);
                    }
                }
            }
            return fakeDatabase.Values.ToArray();
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IList<NewsSummary>))]
        [ProducesResponseType(304, Type = typeof(IList<NewsSummary>))]
        [ProducesResponseType(400)]
        public IActionResult Get()
        {
            try
            {
                Cache news = _cache;
                if (news.HasNewData)
                {
                    news.HasNewData = false;
                    return Ok(news.Data);
                }
                return StatusCode(304, news.Data);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Boolean))]
        public IActionResult Post()
        {
            // Asume this is not gonna be called that often,
            // thus global queue shouldn't be blocked and async does not make sense
            try
            {
                IList<NewsSummary> data;
                Stream s = Request.BodyReader.AsStream();
                using (XmlReader reader = XmlReader.Create(s))
                {
                    data = loadData(reader);
                }
                _cache = new Cache
                {
                    HasNewData = true,
                    Data = data
                };
                return Ok(true);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
