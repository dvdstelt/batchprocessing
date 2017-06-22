using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceProcessing.Messages.Messages
{
    public class CreateInvoiceProposalResponse
    {
        public Guid CustomerId { get; set; }
        public Guid InvoiceId { get; set; }
        public Guid BillingRunId { get; set; }
    }
}
