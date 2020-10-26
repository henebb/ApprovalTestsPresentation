using System;
using System.Threading.Tasks;

namespace FredagsdragningNov2020
{
    public class ReceiptSender
    {
        private readonly IReceiptStorage _receiptStorage;
        private readonly IEmailSender _emailSender;
        private readonly IUserProfileReader _userProfileReader;
        private readonly ILogger _logger;
        private readonly Func<DateTime> _nowProvider;

        public ReceiptSender(
            IReceiptStorage receiptStorage,
            IEmailSender emailSender,
            IUserProfileReader userProfileReader,
            ILogger logger,
            Func<DateTime> nowProvider
        )
        {
            _receiptStorage = receiptStorage;
            _emailSender = emailSender;
            _userProfileReader = userProfileReader;
            _logger = logger;
            _nowProvider = nowProvider;
        }

        public async Task SendReceiptAsync(Receipt receipt, string user)
        {
            _logger.LogInformation($"Handling receipt with ID {receipt.ReceiptId}.");

            // Store in DB:
            _logger.LogInformation("Storing receipt...");

            var receiptStoreItem = new ReceiptStoreItem
            {
                Id = receipt.ReceiptId,
                Items = receipt.Items,
                Created = _nowProvider(),
                Owner = user
            };
            await _receiptStorage.SaveAsync(receiptStoreItem);

            var userSettings = await _userProfileReader.GetUserSettings(user);

            if (userSettings.ContainsKey(UserSettingKeys.ReadsReceiptsOnline) &&
                userSettings[UserSettingKeys.ReadsReceiptsOnline].Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogInformation($"User {user} reads receipts online, not sending email.");
            }
            else
            {
                if (userSettings.ContainsKey(UserSettingKeys.EmailAddresses) &&
                    !string.IsNullOrEmpty(userSettings[UserSettingKeys.EmailAddresses]))
                {
                    // Send mail:
                    _logger.LogInformation("Sending email...");

                    var to = userSettings[UserSettingKeys.EmailAddresses].Split(',', StringSplitOptions.RemoveEmptyEntries);
                    var subject = "Here is your receipt";
                    var body = @$"<html>
<body>
   <p>Hi! Here is your receipt.</p>
   {receipt.ToHtml()}
</body>
</html>";
                    await _emailSender.SendAsync(to, subject, body);
                }
                else
                {
                    _logger.LogInformation($"User {user} is missing email addresses.");
                }
            }

            _logger.LogInformation("Process done.");
        }
    }
}
