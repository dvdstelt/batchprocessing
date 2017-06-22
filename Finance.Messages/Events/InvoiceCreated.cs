using System;

namespace Finance.Messages.Events
{
    public interface InvoiceCreated
    {
        Guid InvoiceId { get; set; }
        Guid CustomerId { get; set; }
    }
}
