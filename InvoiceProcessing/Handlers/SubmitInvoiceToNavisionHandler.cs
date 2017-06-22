using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvoiceProcessing.Messages.Messages;
using NServiceBus;

namespace InvoiceProcessing.Handlers
{
    public class SubmitInvoiceToNavisionHandler : IHandleMessages<SubmitInvoiceToNavisionRequest>
    {
        public async Task Handle(SubmitInvoiceToNavisionRequest message, IMessageHandlerContext context)
        {
            // Talk to Navision

            //await Task.Delay(2000);

            await context.Reply(new SubmitInvoiceToNavisionResponse());
        }
    }
}
