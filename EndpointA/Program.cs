using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finance.Messages.Commands;
using InvoiceProcessing.Messages.Commands;
using NServiceBus;
using Shared;

namespace EndpointA
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            Console.Title = "EndpointA - InvoiceProcessing";

            var endpointConfiguration = new EndpointConfiguration("InvoiceProcessing");
            endpointConfiguration.ApplyCommonConfiguration();

            // Routing
            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            transport.Routing().RouteToEndpoint(typeof(CreateInvoiceProposals).Assembly, "InvoiceProcessing");
            transport.Routing().RouteToEndpoint(typeof(CreateInvoice).Assembly, "Finance");

            var endpoint = await Endpoint.Start(endpointConfiguration);

            Console.WriteLine("Press a key to quit...");
            Console.ReadKey();

            await endpoint.Stop();
        }
    }
}
