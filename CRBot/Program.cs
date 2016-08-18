using System.Configuration;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace CRBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var developers = ConfigurationManager.AppSettings["Developers"].Split(',');

            var generator = new Generator(ConfigurationManager.AppSettings["GitLabUrl"], ConfigurationManager.AppSettings["GitLabToken"], ConfigurationManager.AppSettings["GitLabProject"], developers);

            var text = generator.GenerateMessage();

            var message = new
            {
                text = text,
                link_names = 1
            };

            var json = JsonConvert.SerializeObject(message);

            var webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            webClient.UploadString("https://hooks.slack.com/services/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", json);
        }        
    }
}
