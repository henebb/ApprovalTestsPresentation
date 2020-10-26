using System;
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

    }
}
