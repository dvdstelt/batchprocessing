using System;
using System.Threading.Tasks;

namespace Finance.ImportantStuff
{
    public class InvoiceCreator
    {
        public async Task<Guid> Create(Guid customerId)
        {
            //await Task.Delay(1000);

            // Generated InvoiceId and returning that.
            return Guid.NewGuid();
        }
    }
}
