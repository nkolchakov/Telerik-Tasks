using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProcessingJSON
{
    class EntryPoint
    {

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            string url = @"https://www.youtube.com/feeds/videos.xml?channel_id=UCLC-vbm7OWvpbqzXaoAMGGw";
            string rssDir = "../../rss.xml";

            WebClient webClient = new WebClient();
            webClient.DownloadFile(url, rssDir);

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(rssDir);

            string jsonSite = JsonConvert.SerializeObject(xmlDoc, Newtonsoft.Json.Formatting.Indented);


            JObject joSite = JObject.Parse(jsonSite);


            var titles = joSite["feed"]["entry"]
                        .Select(p => p["title"]).ToList();

            foreach (var title in titles)
            {
                Console.WriteLine(title);
            }

            var videos = ExtractVideos(joSite["feed"]["entry"]);

            var html = ExtractHTML(videos);
            File.WriteAllText("../../vidoes.html", html);
        }

        private static string ExtractHTML(IEnumerable<Video> videos)
        {

            Console.OutputEncoding = Encoding.UTF8;

            StringBuilder html = new StringBuilder();
            html.Append("<!DOCTYPE html><html><body>");

            foreach (var video in videos)
            {
                html.AppendFormat("<div>" +
                    "<p>{2}</p>" +
                    "<iframe id={1} width=\"560\" height=\"315\" src=\"{0}\" frameborder = \"0\" allowfullscreen></iframe>" +
                    "</div>", video.Link.Replace("watch?v=", "v/"), video.Id, video.Title);
            }

            html.Append("</body></html>");

            return html.ToString();
        }

        private static IEnumerable<Video> ExtractVideos(JToken jToken)
        {
            return jToken.Select(p => new Video
            {
                Id = p["id"].ToString(),
                Title = p["title"].ToString(),
                Link = p["link"]["@href"].ToString()
            });
        }
    }
}
