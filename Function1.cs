using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using Azure.Messaging;
using Microsoft.Azure.Functions.Worker;
using static EventGrid_685.PublishChangeFeedEvents;

namespace EventGrid_685
{

    public class StreamId
    {
        public string Value { set; get; }
    }

    public static class PublishChangeFeedEvents
    {
        [Function(nameof(PublishChangeFeedEvents))]
        [EventGridOutput(TopicEndpointUri = "TopicEndpointUri", TopicKeySetting = "TopicKeySetting")]
        public static string Run([CosmosDBTrigger(
    databaseName: "my-db",
    collectionName: "my-container",
    ConnectionStringSetting = "ConnStrCosmosDb",
    LeaseCollectionName = "leases",
    CreateLeaseCollectionIfNotExists = true,
    StartFromBeginning = true)] string json)
        {

            var events = new string[] { "test", "item"}
                .Select(x => new CloudEvent("https://localhost", x, new BinaryData(x), MediaTypeNames.Application.Json, CloudEventDataFormat.Json)
            {
                Subject = x
            });

            var outputJson = JsonSerializer.Serialize(events);

            return outputJson;
        }

        public class Person
        {
            public string Name { get; set; }
        }

    }

    public class MyDocument
    {
        public string Id { get; set; }

        public string Text { get; set; }

        public int Number { get; set; }

        public bool Boolean { get; set; }
    }
}
