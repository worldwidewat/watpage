using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Watpage.Controllers
{
    public class PageController : Controller
    {
        public async Task<ActionResult> Index(string page = "watpage")
        {
            return await GetResult(string.Format(ConfigurationManager.AppSettings["PageUrlFormat"], page));
        }

        public async Task<ActionResult> Resource(string page, string name)
        {
            return await GetResult(string.Format(ConfigurationManager.AppSettings["ResourceUrlFormat"], page, name));
        }

        private async Task<ActionResult> GetResult(string url)
        {
            var client = new HttpClient();

            var ifModifiedSince = GetIfModifiedSince();
            var ifNoneMatchEtag = Request.Headers["If-None-Match"];

            // If there is an If-Modified-Since header, pass that value along
            if (ifModifiedSince.HasValue)
            {
                client.DefaultRequestHeaders.IfModifiedSince = ifModifiedSince.Value;
            }

            // If there was an etag specified in the If-None-Match header, pass that value along
            if (!string.IsNullOrEmpty(ifNoneMatchEtag))
            {
                client.DefaultRequestHeaders.IfNoneMatch.Add(new EntityTagHeaderValue(string.Format("\"{0}\"", ifNoneMatchEtag)));
            }

            // Do it
            var result = await client.GetAsync(url);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // We got something back
                // Add the ETag and Content-MD5 header to the response
                var eTag = result.Headers.GetValues("ETag").First();

                Response.AddHeader("ETag", eTag);
                Response.AddHeader("Content-MD5", Convert.ToBase64String(result.Content.Headers.ContentMD5));

                // If the Last-Modified header exists, pass that back too
                if (result.Content.Headers.LastModified.HasValue)
                {
                    Response.AddHeader("Last-Modified", result.Content.Headers.LastModified.Value.ToUniversalTime().ToString("R"));
                }
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.NotModified)
            {
                // The resource didn't change (If-Modified-Since or If-None-Match triggered it) - return a 304 with no content
                return new HttpStatusCodeResult(HttpStatusCode.NotModified);
            }
            else
            {
                // Make sure we didn't get an error
                result.EnsureSuccessStatusCode();
            }

            // Return the content
            return new FileStreamResult(await result.Content.ReadAsStreamAsync(), result.Content.Headers.ContentType.MediaType);
        }

        private DateTime? GetIfModifiedSince()
        {
            DateTime? result = null;

            var headerValue = Request.Headers["If-Modified-Since"];

            if (headerValue != null)
            {
                result = DateTime.Parse(headerValue).ToUniversalTime();
            }

            return result;
        }
    }
}