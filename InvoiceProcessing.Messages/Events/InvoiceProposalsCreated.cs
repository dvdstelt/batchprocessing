using System;

namespace InvoiceProcessing.Messages.Events
{
    public interface InvoiceProposalsCreated
    {
        Guid BillingRunId { get; set; }
    }
}
