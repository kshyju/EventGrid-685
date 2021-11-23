using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using static EventGrid_685.PublishChangeFeedEvents;

namespace EventGrid_685
{
    public class CloudEvent2
    {
        public string Type { set; get; }
        public BinaryData Data { set; get; }
        public StreamId StreamId { set; get; }
    }

    public class StreamId
    {
        public string Value { set; get; }
    }
    public static class CosmosEventStoreChangeFeed
    {
        public static IEnumerable<CloudEvent2> ParseEvents(String json)
        {
            return new List<CloudEvent2>()
            {
                new CloudEvent2()
                {
                    StreamId = new StreamId() { Value="a"},
                    Data = new BinaryData(new Person{ Name = "sco"}),
                    Type = "Type1"
                }
            };

        }
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
            //var eventsList = CosmosEventStoreChangeFeed.ParseEvents(json).ToList();
            //var eventsList = CosmosEventStoreChangeFeed.ParseEvents(json).ToList();
            //var eventsList = CosmosEventStoreChangeFeed.ParseEvents(json).ToList();

            var events2 = CosmosEventStoreChangeFeed.ParseEvents(json)
                .Select(x => new CloudEvent("https://localhost", x.Type, x.Data, MediaTypeNames.Application.Json, CloudEventDataFormat.Json)
            {
                Subject = x.StreamId.Value
            });


            //var events = eventslist.select(x => 
            //new cloudevent("https://localhost", x.type, x.data, mediatypenames.application.json, cloudeventdataformat.json)
            //{
            //    subject = x.streamid.value
            //});           
            
            EventGridEvent eventGridEvent = new EventGridEvent("sub","my-type","dataversion1", new Person { Name ="scottEventGridEvent"});
            var events= new List<EventGridEvent>()
            {
                eventGridEvent,
            };
            
            var outputJson = JsonSerializer.Serialize(events);

            return outputJson;
        }

        public class Person
        {
            public string Name { get; set; }
        }

        //[Function("Function1")]
        //public static void Run([CosmosDBTrigger(
        //    databaseName: "my-db",
        //    collectionName: "my-container",
        //    ConnectionStringSetting = "ConnStrCosmosDb",
        //    LeaseCollectionName = "leases")] IReadOnlyList<MyDocument> input, FunctionContext context)
        //{
        //    var logger = context.GetLogger("Function1");
        //    if (input != null && input.Count > 0)
        //    {
        //        logger.LogInformation("Documents modified: " + input.Count);
        //        logger.LogInformation("First document Id: " + input[0].Id);
        //    }
        //}
    }

    public class MyDocument
    {
        public string Id { get; set; }

        public string Text { get; set; }

        public int Number { get; set; }

        public bool Boolean { get; set; }
    }
}
