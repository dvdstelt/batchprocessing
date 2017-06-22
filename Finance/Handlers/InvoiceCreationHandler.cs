using System.Threading.Tasks;
using Finance.ImportantStuff;
using Finance.Messages.Commands;
using Finance.Messages.Events;
using NServiceBus;

namespace Finance.Handlers
{
    public class InvoiceCreationHandler : IHandleMessages<CreateInvoice>
    {
        public async Task Handle(CreateInvoice message, IMessageHandlerContext context)
        {
            var customerId = message.CustomerId;

            var invoiceId = await new InvoiceCreator().Create(customerId);

            await context.Publish<InvoiceCreated>(m =>
            {
                m.CustomerId = customerId;
                m.InvoiceId = invoiceId;
            });
        }
    }
}
