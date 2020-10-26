using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FredagsdragningNov2020
{
    public interface IReceiptStorage
    {
        Task SaveAsync(ReceiptStoreItem receiptStoreItem);
    }

    public class ReceiptStoreItem
    {
        public Guid Id { get; set; }
        public IList<ReceiptItem> Items { get; set; }
        public DateTime Created { get; set; }
        public string Owner { get; set; }
    }
}
