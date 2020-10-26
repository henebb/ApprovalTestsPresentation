using System;
using System.Threading.Tasks;

namespace FredagsdragningNov2020
{
    public class ReceiptSender
    {
        private readonly IReceiptStorage _receiptStorage;
        private readonly IEmailSender _emailSender;
        private readonly ISettingsReader _settingsReader;
        private readonly ILogger _logger;
        private readonly Func<DateTime> _nowProvider;

        public ReceiptSender(
            IReceiptStorage receiptStorage,
            IEmailSender emailSender,
            ISettingsReader settingsReader,
            ILogger logger,
            Func<DateTime> nowProvider
        )
        {
            _receiptStorage = receiptStorage;
            _emailSender = emailSender;
            _settingsReader = settingsReader;
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
                CreatedBy = user
            };
            await _receiptStorage.SaveAsync(receiptStoreItem);

            // Send mail:
            _logger.LogInformation("Sending email...");

            string[] to = await _settingsReader.GetSettingByKey(ToAddressType.Receipt);
            string subject = "Here is your receipt";
            string body = @$"<html>
<body>
   <p>Hi! Here is your receipt.</p>
   {receipt.ToHtml()}
</body>
</html>";
            await _emailSender.SendAsync(to, subject, body);

            _logger.LogInformation("Process done.");
        }
    }
}
