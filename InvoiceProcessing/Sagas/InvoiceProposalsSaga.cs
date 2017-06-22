using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvoiceProcessing.Messages.Commands;
using InvoiceProcessing.Messages.Events;
using InvoiceProcessing.Messages.Messages;
using NServiceBus;

namespace InvoiceProcessing.Sagas
{
    public class InvoiceProposals : ContainSagaData
    {
        public Guid BillingRunId { get; set; }
        public List<Invoice> Invoices { get; set; }
    }

    public class Invoice
    {
        public Guid CustomerId { get; set; }
        public Guid InvoiceId { get; set; }
        public InvoiceStatus Status { get; set; }
    }

    public enum InvoiceStatus
    {
        Waiting,
        InProgress,
        Done
    }

    public class InvoiceProposalsSaga : Saga<InvoiceProposals>,
        IAmStartedByMessages<CreateInvoiceProposals>,
        IHandleMessages<CreateInvoiceProposalResponse>
    {
        const int BatchSize = 10;

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<InvoiceProposals> mapper)
        {
            mapper.ConfigureMapping<CreateInvoiceProposals>(m => m.BillingRunId).ToSaga(s => s.BillingRunId);
            mapper.ConfigureMapping<CreateInvoiceProposalResponse>(m => m.BillingRunId).ToSaga(s => s.BillingRunId);
        }

        public async Task Handle(CreateInvoiceProposals message, IMessageHandlerContext context)
        {
            Console.WriteLine($"BillingRun {message.BillingRunId} created.");
            Data.Invoices = new List<Invoice>();
            Data.BillingRunId = message.BillingRunId;            
            message.Customers.ForEach(c =>
            {
                Data.Invoices.Add(new Invoice() { CustomerId = c, Status = InvoiceStatus.Waiting });
            });

            await TrySendNextBatch(context);
        }

        private async Task TrySendNextBatch(IMessageHandlerContext context)
        {
            var customersInProgress = Data.Invoices.Count(s => s.Status == InvoiceStatus.InProgress);
            var customersWaiting = Data.Invoices.Count(s => s.Status == InvoiceStatus.Waiting);

            if (customersInProgress > 0)
                return;
            if (customersWaiting == 0)
            {
                Console.WriteLine($"[{Data.BillingRunId}] {customersWaiting} customers remaining, completing saga.");
                await context.Publish<InvoiceProposalsCreated>(s => { s.BillingRunId = Data.BillingRunId; });
                MarkAsComplete();
                return;
            }

            Console.WriteLine($"[{Data.BillingRunId}] {customersWaiting} customers remaining, sending next batch of {BatchSize}.");
            var nextInvoices = Data.Invoices.Where(s => s.Status == InvoiceStatus.Waiting).Take(BatchSize);

            foreach (var invoice in nextInvoices)
            {
                var message = new CreateInvoiceProposalRequest();
                message.CustomerId = invoice.CustomerId;
                message.BillingRunId = Data.BillingRunId;

                invoice.Status = InvoiceStatus.InProgress;

                await context.SendLocal(message);
            }
        }

        public async Task Handle(CreateInvoiceProposalResponse message, IMessageHandlerContext context)
        {
            var invoice = Data.Invoices.Single(s => s.CustomerId == message.CustomerId);
            invoice.InvoiceId = message.InvoiceId;
            invoice.Status = InvoiceStatus.Done;

            await TrySendNextBatch(context);
        }
    }
}
