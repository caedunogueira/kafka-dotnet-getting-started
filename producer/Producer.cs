﻿using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

if (args.Length != 1)
    Console.WriteLine("Please provide the configuration file path as a command line argument");

var configuration = new ConfigurationBuilder()
    .AddIniFile(args[0])
    .Build();

const string topic = "purchases";

var users = new string[] { "eabara", "jsmith", "sgarcia", "jbernard", "htanaka", "awalther" };
var items = new string[] { "book", "alarm clock", "t-shirts", "gift card", "batteries" };

using var producer = new ProducerBuilder<string, string>(configuration.AsEnumerable()).Build();

var numProduced = 0;
var rnd = new Random();
const int numMessages = 10;

for (var i = 0; i < numMessages; ++i)
{
    var user = users[rnd.Next(users.Length)];
    var item = items[rnd.Next(items.Length)];

    producer.Produce(topic, 
        new Message<string, string> { Key = user, Value = item },
        (deliveryReport) =>
        {
            if (deliveryReport.Error.Code != ErrorCode.NoError)
                Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
            else
            {
                Console.WriteLine($"Produced event to topic {topic}: key = {user,-10} value = {item}");
                numProduced += 1;
            }
        });
}

producer.Flush(TimeSpan.FromSeconds(10));
Console.WriteLine($"{numProduced} messages were produced to topic {topic}");