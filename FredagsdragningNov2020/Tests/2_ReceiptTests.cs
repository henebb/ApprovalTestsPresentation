using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Xunit;

namespace FredagsdragningNov2020.Tests
{
    [UseReporter(typeof(VisualStudioReporter))]
    [UseApprovalSubdirectory("Approvals")]
    public class ReceiptTests
    {
        private static readonly string Separator = new string('-', 30);
        private const int Pad = 20;

        [Fact]
        public void ReceiptTest()
        {
            // Arrange
            var receipt = new Receipt();

            // Act
            receipt.AddItem("Mjölk", 10.95M, 2);
            receipt.AddItem("Bröd", 18.5M, 1);
            receipt.AddItem("Smör", 45.50M, 1);

            decimal total = receipt.Total();

            // Assert


#region MiseEnPlace
            Approvals.Verify(@$"Kvitto
{Separator}
{receipt.PrintItems(Pad)}{Separator}
{"Total",-Pad}{total:C}
");
#endregion
        }
    }
}