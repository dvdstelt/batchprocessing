using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Messages.Commands
{
    public class CreateInvoice
    {
        public Guid CustomerId { get; set; }
    }
}
