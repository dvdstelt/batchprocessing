using System;
using System.Threading.Tasks;
using Finance.Messages.Commands;
using Finance.Messages.Events;
using InvoiceProcessing.Messages.Messages;
using NServiceBus;

namespace InvoiceProcessing.Sagas
{
    public class InvoiceProposal : ContainSagaData
    {
        public Guid BillingRunId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid InvoiceId { get; set; }
    }
    
    public class InvoiceProposalSaga : Saga<InvoiceProposal>,
        IAmStartedByMessages<CreateInvoiceProposalRequest>,
        IHandleMessages<InvoiceCreated>,
        IHandleMessages<SubmitInvoiceToNavisionResponse>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<InvoiceProposal> mapper)
        {
            mapper.ConfigureMapping<CreateInvoiceProposalRequest>(m => m.CustomerId).ToSaga(s => s.CustomerId);
            mapper.ConfigureMapping<InvoiceCreated>(m => m.CustomerId).ToSaga(s => s.CustomerId);
        }

        public async Task Handle(CreateInvoiceProposalRequest message, IMessageHandlerContext context)
        {
            Console.WriteLine($"\t[{message.BillingRunId}] CreateInvoiceProposalRequest registered.");

            Data.CustomerId = message.CustomerId;
            Data.BillingRunId = message.BillingRunId;

            await context.Send(new CreateInvoice() { CustomerId = message.CustomerId});
        }

        public async Task Handle(InvoiceCreated message, IMessageHandlerContext context)
        {
            Console.WriteLine($"\t[{Data.BillingRunId}] Invoice was created.");
            Data.InvoiceId = message.InvoiceId;

            await context.Send(new SubmitInvoiceToNavisionRequest() { InvoiceId = message.InvoiceId });
        }

        public async Task Handle(SubmitInvoiceToNavisionResponse message, IMessageHandlerContext context)
        {
            Console.WriteLine($"\t[{Data.BillingRunId}] Invoice was posted to Navision, completing saga.");
            await CompleteSaga(context);
        }

        private async Task CompleteSaga(IMessageHandlerContext context)
        {
            var response = new CreateInvoiceProposalResponse
            {
                CustomerId = Data.CustomerId,
                InvoiceId = Data.InvoiceId,
                BillingRunId = Data.BillingRunId
            };

            await context.SendLocal(response);

            MarkAsComplete();
        }
    }
}