using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.Json;
using System.Threading.Tasks;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Newtonsoft.Json;
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
        private readonly IUserProfileReader _userProfileReader;
        private readonly ILogger _logger;
        private readonly ReceiptSender _sut;

        private static readonly Guid FakeId = Guid.Parse("7EEC86D4-81C7-4E72-B188-0DF00FB301AF");
        private static readonly DateTime FakeNow = new DateTime(2020, 10, 26, 16, 0,0, DateTimeKind.Utc);
        private static readonly string Separator = new string('-', 20);

        public ReceiptSenderTests()
        {
            _receiptStorage = Substitute.For<IReceiptStorage>();
            _emailSender = Substitute.For<IEmailSender>();
            _userProfileReader = Substitute.For<IUserProfileReader>();
            _logger = Substitute.For<ILogger>();

            _sut = new ReceiptSender(
                _receiptStorage,
                _emailSender,
                _userProfileReader,
                _logger,
                () => FakeNow
            );
        }

        [Fact]
        public async void SendReceipt_WithEmail()
        {
            // Arrange
            var receipt = new Receipt(FakeId);
            receipt.AddItem("Mjölk", 10.95M, 2);
            receipt.AddItem("Bröd", 18.5M, 1);
            receipt.AddItem("Smör", 45.50M, 1);

            var user = "Test Tester";

            // Fake user settings:
            var toAddresses = "a@b.com,x@y.com";
            _userProfileReader.GetUserSettingsAsync(user)
                .Returns(Task.FromResult(
                    new Dictionary<string, string>
                    {
                        // Not reading receipts online:
                        {UserSettingKeys.ReadsReceiptsOnline, bool.FalseString},
                        {UserSettingKeys.EmailAddresses, toAddresses}
                    }));

            int i = 1;
            var objectsToVerify = new Dictionary<string, object>();

            // Save what is logged.
            var log = new StringCollection();
            _logger
                .When(x => x.LogInformation(Arg.Any<string>()))
                .Do(x => log.Add($"INFO; {x[0]}"));

            // Save what the storage saves.
            _receiptStorage
                .When(x => x.SaveAsync(Arg.Any<ReceiptStoreItem>()))
                .Do(x => objectsToVerify.Add($"{i++}. Stored receipt", x[0]));

            // Save what the email sender sends.
            _emailSender
                .WhenForAnyArgs(x => x.SendAsync(default, default, default))
                .Do(x =>
                {
                    objectsToVerify.Add($"{i++}. Sent email", new
                    {
                        Subject = x[1],
                        SentTo = x[0],
                        Body = x[2].ToString()?.Split("\r\n")
                    });
                });

            // Act
            await _sut.SendReceiptAsync(receipt, user);

            // Assert
            objectsToVerify.Add($"{i++}. Log", log);

            Approvals.VerifyAll(
                $"Send receipt with email\n{Separator}",
                objectsToVerify,
                (label, obj) => $"{label}:\n{JsonConvert.SerializeObject(obj, Formatting.Indented)}\n{Separator}"
            );
        }

        [Fact]
        public async void SendReceipt_NoEmail()
        {
            // Arrange
            var receipt = new Receipt(FakeId);
            receipt.AddItem("Mjölk", 10.95M, 2);
            receipt.AddItem("Bröd", 18.5M, 1);
            receipt.AddItem("Smör", 45.50M, 1);

            var user = "Test Tester";

            _userProfileReader.GetUserSettingsAsync(user)
                .Returns(Task.FromResult(
                    new Dictionary<string, string>
                    {
                        // Reading receipts online
                        {UserSettingKeys.ReadsReceiptsOnline, bool.TrueString},
                    }));

            int i = 1;
            var objectsToVerify = new Dictionary<string, object>();

            // Save what is logged.
            var log = new StringCollection();
            _logger
                .When(x => x.LogInformation(Arg.Any<string>()))
                .Do(x => log.Add($"INFO; {x[0]}"));

            // Save what the storage saves.
            _receiptStorage
                .When(x => x.SaveAsync(Arg.Any<ReceiptStoreItem>()))
                .Do(x => objectsToVerify.Add($"{i++}. Stored receipt", x[0]));

            // Save what the email sender sends.
            _emailSender
                .WhenForAnyArgs(x => x.SendAsync(default, default, default))
                .Do(x =>
                {
                    objectsToVerify.Add($"{i++}. Sent email", new
                    {
                        Subject = x[1],
                        SentTo = x[0],
                        Body = x[2].ToString()?.Split("\r\n")
                    });
                });

            // Act
            await _sut.SendReceiptAsync(receipt, user);

            // Assert
            objectsToVerify.Add($"{i++}. Log", log);

            Approvals.VerifyAll(
                $"Send receipt with online preference\n{Separator}",
                objectsToVerify,
                (label, obj) => $"{label}:\n{JsonConvert.SerializeObject(obj, Formatting.Indented)}\n{Separator}"
            );
        }

        [Fact]
        public async void SendReceipt_WithEmailButNoAddresses()
        {
            // Arrange
            var receipt = new Receipt(FakeId);
            receipt.AddItem("Mjölk", 10.95M, 2);
            receipt.AddItem("Bröd", 18.5M, 1);
            receipt.AddItem("Smör", 45.50M, 1);

            var user = "Test Tester";

            _userProfileReader.GetUserSettingsAsync(user)
                .Returns(Task.FromResult(
                    new Dictionary<string, string>
                    {
                        // Not reading receipts online:
                        {UserSettingKeys.ReadsReceiptsOnline, bool.FalseString},
                        // No addresses defined:
                        {UserSettingKeys.EmailAddresses, string.Empty},
                    }));

            int i = 1;
            var objectsToVerify = new Dictionary<string, object>();

            // Save what is logged.
            var log = new StringCollection();
            _logger
                .When(x => x.LogInformation(Arg.Any<string>()))
                .Do(x => log.Add($"INFO; {x[0]}"));
            _logger
                .When(x => x.LogError(Arg.Any<string>()))
                .Do(x => log.Add($"ERROR; {x[0]}"));

            // Save what the storage saves.
            _receiptStorage
                .When(x => x.SaveAsync(Arg.Any<ReceiptStoreItem>()))
                .Do(x => objectsToVerify.Add($"{i++}. Stored receipt", x[0]));

            // Save what the email sender sends.
            _emailSender
                .WhenForAnyArgs(x => x.SendAsync(default, default, default))
                .Do(x =>
                {
                    objectsToVerify.Add($"{i++}. Sent email", new
                    {
                        Subject = x[1],
                        SentTo = x[0],
                        Body = x[2].ToString()?.Split("\r\n")
                    });
                });

            // Act
            await _sut.SendReceiptAsync(receipt, user);

            // Assert
            objectsToVerify.Add($"{i++}. Log", log);

            Approvals.VerifyAll(
                $"Send receipt with email, but no addresses defined\n{Separator}",
                objectsToVerify,
                (label, obj) => $"{label}:\n{JsonConvert.SerializeObject(obj, Formatting.Indented)}\n{Separator}"
            );
        }
    }
}
