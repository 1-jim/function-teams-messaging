using System.Net;
using System.Text;
using function_teams_messaging.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace function_teams_messaging
{
    public class SendChannelMessage
    {
        private readonly ILogger _logger;
        private readonly string? _webhookUrl = Environment.GetEnvironmentVariable("TEAMS_WEBHOOK");

        public SendChannelMessage(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SendChannelMessage>();
        }

        [Function("SendChannelMessage")]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            TeamsMessageModel payload;
            using (var reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                var requestBody = reader.ReadToEnd();
                //_logger.LogInformation(requestBody);
                if (requestBody == null)
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    badResponse.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                    badResponse.WriteString("Unable to Send Channel Message");
                    return badResponse;
                }

                dynamic data = JsonConvert.DeserializeObject(requestBody);
                string title = data?.title;
                string message = data?.message;

                payload = new()
                {
                    ThemeColor = "0078D7",
                    Summary = "Message from Azure Function",
                    Sections = new[]{ new Section
                    {
                        ActivityTitle = title,
                        Facts = new[] { new Fact
                            {
                                Name = "Message",
                                Value = message
                            }
                        }
                    }
                }
                };
            }

            HttpResponseMessage teamsResponse;
            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                teamsResponse = await client.PostAsync(_webhookUrl, content);
            }

            if (!teamsResponse.IsSuccessStatusCode)
            {
                var resFail = req.CreateResponse(HttpStatusCode.BadRequest);
                resFail.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                resFail.WriteString($"Unable to Send Channel Message: {teamsResponse.ReasonPhrase}");
                return resFail;
            }

            var res = req.CreateResponse(HttpStatusCode.OK);
            res.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            res.WriteString("Send Channel Message OK");
            return res;
        }
    }
}
