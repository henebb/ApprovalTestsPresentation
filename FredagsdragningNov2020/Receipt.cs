using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FredagsdragningNov2020
{
    public class Receipt
    {
        public IList<ReceiptItem> Items { get; } = new List<ReceiptItem>();

        public Receipt()
            : this (Guid.NewGuid())
        {
        }

        public Receipt(Guid id)
        {
            ReceiptId = id;
        }

        public Guid ReceiptId { get; }

        public void AddItem(string description, decimal pricePerUnit, int numItems)
        {
            Items.Add(new ReceiptItem(description, pricePerUnit, numItems));
        }

        public decimal Total()
        {
            return Items.Sum(i => i.NumItems * i.PricePerUnit);
        }

        public string PrintItems(int pad)
        {
            var sb = new StringBuilder();
            foreach (var item in Items)
            {
                sb.Append($"{item.Print(pad)}\r\n");
            }

            return sb.ToString();
        }

        public string ToHtml()
        {
            var sb = new StringBuilder(@"<table>
  <thead>
    <tr>
      <th>Item</th>
      <th>Item price</th>
      <th>Quantity</th>
      <th>Amount</th>
    </tr>
  </thead>
  <tbody>");

            foreach (var item in Items)
            {
                sb.Append($@"
    <tr>
      <td>{item.Description}</td>
      <td>{item.PricePerUnit:C}</td>
      <td>{item.NumItems}</td>
      <td>{item.NumItems * item.PricePerUnit:C}</td>
    </tr>");
            }

            sb.Append(@$"
  </tbody>
  <tfoot>
    <tr>
      <td colspan='3'>Total</td>
      <td>{Total():C}</td>
    </tr>
  </tfoot>
</table>");

            return sb.ToString();
        }
    }

    public class ReceiptItem
    {
        public string Description { get; set; }
        public decimal PricePerUnit { get; set; }
        public int NumItems { get; set; }

        public ReceiptItem(string description, decimal pricePerUnit, int numItems)
        {
            Description = description;
            PricePerUnit = pricePerUnit;
            NumItems = numItems;
        }

        public string Print(int pad)
        {
            var sb = new StringBuilder();
            sb.Append($"{Description.PadRight(pad)}{PricePerUnit * NumItems:C}");
            if (NumItems > 1)
            {
                sb.Append($"\r\n{NumItems} * {PricePerUnit:C}");
            }

            return sb.ToString();
        }
    }
}
