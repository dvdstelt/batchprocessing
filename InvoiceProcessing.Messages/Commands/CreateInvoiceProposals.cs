using System;
using System.Collections.Generic;

namespace InvoiceProcessing.Messages.Commands
{
    public class CreateInvoiceProposals
    {
        public Guid BillingRunId { get; set; }
        public List<Guid> Customers { get; set; }
    }
}
