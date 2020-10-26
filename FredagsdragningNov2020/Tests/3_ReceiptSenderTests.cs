using System;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using NSubstitute;
using Xunit;

namespace FredagsdragningNov2020.Tests
{
    [UseReporter(typeof(VisualStudioReporter))]
    [UseApprovalSubdirectory("Approvals")]
    public class ReceiptSenderTests
    {
        private readonly IReceiptStorage _receiptStorage;
        private readonly IEmailSender _emailSender;
        private readonly ISettingsReader _settingsReader;
        private readonly ILogger _logger;
        private readonly ReceiptSender _sut;

        private static readonly Guid FakeId = Guid.Parse("7EEC86D4-81C7-4E72-B188-0DF00FB301AF");

        public ReceiptSenderTests()
        {
            _receiptStorage = Substitute.For<IReceiptStorage>();
            _emailSender = Substitute.For<IEmailSender>();
            _settingsReader = Substitute.For<ISettingsReader>();
            _logger = Substitute.For<ILogger>();

            _sut = new ReceiptSender(
                _receiptStorage,
                _emailSender,
                _settingsReader,
                _logger
            );
        }

        [Fact]
        public async void SendReceipt()
        {
            // Arrange
            var receipt = new Receipt(() => FakeId);
            receipt.AddItem("Mjölk", 10.95M, 2);
            receipt.AddItem("Bröd", 18.5M, 1);
            receipt.AddItem("Smör", 45.50M, 1);

            // Act
            await _sut.SendReceiptAsync(receipt);

            // Assert

        }
    }
}