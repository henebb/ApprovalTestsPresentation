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
            var separator = new string('-', 30);
            const int pad = 20;

            Approvals.Verify(@$"Kvitto
{separator}
{receipt.PrintItems(pad)}{separator}
{"Total",-pad}{total:C}
".Replace("\r", string.Empty ));
#endregion
        }

        [Fact]
        public void ReceiptHtmlTest()
        {
            // Arrange
            var receipt = new Receipt();

            // Act
            receipt.AddItem("Mjölk", 10.95M, 2);
            receipt.AddItem("Bröd", 18.5M, 1);
            receipt.AddItem("Smör", 45.50M, 1);

            // Assert
            Approvals.VerifyHtml(receipt.ToHtml());
        }
    }
}