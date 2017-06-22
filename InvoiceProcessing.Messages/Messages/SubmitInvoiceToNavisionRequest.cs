using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceProcessing.Messages.Messages
{
    public class SubmitInvoiceToNavisionRequest
    {
        public Guid InvoiceId { get; set; }
    }
}
