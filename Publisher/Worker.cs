using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using SharedLib;
using EasyNetQ.Topology;

namespace Publisher
{
    class Worker : BackgroundService
    {
        private IBus _bus;
        private readonly Queue _queue;
        //private readonly Exchange _exchange;

        public Worker(IBus bus)
        {
            _bus = bus;
            _queue = new Queue("ExampleQueue");
            _queue.Arguments.Add("x-max-priority", 10);
            //_exchange = new Exchange("RADIO");    //need to create that manually and bind queue to it manually so it will work
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int count = 100;
            while (!stoppingToken.IsCancellationRequested && count-- > 0)
            {
                Console.WriteLine($"Publishing message {count}");
                try
                {
                    var body = new MessageTemplate
                    {
                        MessageId = count,
                        MessageSubject = $"Hello {count}"
                    };
                    //use the simple config:
                    await _bus.SendReceive.SendAsync(_queue.Name, body, cancellationToken: stoppingToken);

                    //advanced config with message expiration (TTL):
                    //var msg = new Message<EmailMessage>(body);
                    //if (count != 9999)
                    //    msg.Properties.Expiration = "10000";

                    //await _bus.Advanced.PublishAsync(_exchange, string.Empty, true, msg);

                    await Task.Delay(100, stoppingToken);
                }
                catch (Exception e)
                {

                }
            }
        }
    }
}
