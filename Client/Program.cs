using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvoiceProcessing.Messages.Commands;
using NServiceBus;
using Shared;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var numberOfInvoices = 150;

            Console.Title = "Client";
            Console.WriteLine($"Press a key to start a batch of {numberOfInvoices} customers.");
            Console.ReadKey();

            var endpointConfiguration = new EndpointConfiguration("Client");
            endpointConfiguration.ApplyCommonConfiguration();
            endpointConfiguration.SendOnly();

            // Routing
            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            transport.Routing().RouteToEndpoint(typeof(CreateInvoiceProposals).Assembly, "InvoiceProcessing");

            // Start endpoint
            var sendOnlyEndpoint = await Endpoint.Start(endpointConfiguration);

            var customerIds = new List<Guid>();
            for (int i = 0; i < numberOfInvoices; i++)
            {
                customerIds.Add(Guid.NewGuid());
            }

            var msg = new CreateInvoiceProposals();
            msg.BillingRunId = Guid.NewGuid();
            msg.Customers = customerIds;

            await sendOnlyEndpoint.Send(msg);
            await sendOnlyEndpoint.Stop();

            Console.WriteLine("Send out batch, press any key to quit...");
            Console.ReadKey();
        }
    }
}
