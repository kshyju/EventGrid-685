using System;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using Azure.Messaging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace EventGrid_685
{
    public static class PublishChangeFeedEvents
    {
        [Function(nameof(PublishChangeFeedEvents))]
        [EventGridOutput(TopicEndpointUri = "TopicEndpointUri", TopicKeySetting = "TopicKeySetting")]
        public static string Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var u = new Person {  Name="Smoke"};

            var events = new string[] { "one" }
                .Select(x => new CloudEvent("https://localhost", x, new BinaryData(u), MediaTypeNames.Application.Json, CloudEventDataFormat.Json)
                {
                    Subject = "sub"
                });

            var outputJson = JsonSerializer.Serialize(events);

            return outputJson;
        }       
    }

    public class Person
    {
        public string Name { get; set; }
    }
}
