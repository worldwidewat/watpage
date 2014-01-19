using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Web.Mvc;

namespace Watpage.Controllers
{
    public class PageController : Controller
    {
        public ActionResult Index(string name)
        {
            var pageName = GetPageUrl(name);
            var pageStream = GetPageStream(pageName);

            return new FileStreamResult(pageStream, "text/html");
        }

        private string GetPageUrl(string name)
        {
            var pageUrlFormat = ConfigurationManager.AppSettings["PageUrlFormat"];

            return string.Format(pageUrlFormat, name);
        }

        private Stream GetPageStream(string pageUrl)
        {
            var client = new HttpClient();

            var result = client.GetAsync(pageUrl).Result;

            result.EnsureSuccessStatusCode();

            return result.Content.ReadAsStreamAsync().Result;
        }
    }
}