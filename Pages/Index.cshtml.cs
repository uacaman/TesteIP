using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;


namespace TesteIP.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public string ClientIpAddress { get; set; }

        public IHeaderDictionary Headers { get; private set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
        }

        public void OnGet()
        {
            Headers = HttpContext.Request.Headers;
            ClientIpAddress = GetClientIP(HttpContext.Request);
        }


        public string GetClientIP(HttpRequest request)
        {
            string ip;
            //Checks if Forwarded header is set or not
            ip = GetForwarded(request);
            if (!String.IsNullOrEmpty(ip)) return ip;

            //Checks if X-Forwarded-For header is set or not
            ip = GetXForwardedFor(request);
            if (!String.IsNullOrEmpty(ip)) return ip;

            return HttpContext.Connection.RemoteIpAddress.ToString();
        }

        private string GetXForwardedFor(HttpRequest request)
        {
            string headerValue = request.Headers["X-Forwarded-For"];
            if (!string.IsNullOrEmpty(headerValue))
            {
                return headerValue;
            }

            return null;
        }

        private string GetForwarded(HttpRequest request)
        {
            string headerValue = request.Headers["Forwarded"];
            if (!string.IsNullOrEmpty(headerValue))
            {
                string[] entries = headerValue.Split(',');
                if (entries.Length > 0)
                {
                    string[] values = entries.First().Split(';');

                    string forValue = values.FirstOrDefault(x => x.StartsWith("for"));

                    if (!string.IsNullOrEmpty(forValue))
                    {
                        return forValue;
                    }
                }
            }

            return null;
        }
    }
}
