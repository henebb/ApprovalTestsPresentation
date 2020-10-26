using System.Threading.Tasks;

namespace FredagsdragningNov2020
{
    public class ReceiptSender
    {
        private readonly IReceiptStorage _receiptStorage;
        private readonly IEmailSender _emailSender;
        private readonly ISettingsReader _settingsReader;
        private readonly ILogger _logger;

        public ReceiptSender(
            IReceiptStorage receiptStorage,
            IEmailSender emailSender,
            ISettingsReader settingsReader,
            ILogger logger
        )
        {
            _receiptStorage = receiptStorage;
            _emailSender = emailSender;
            _settingsReader = settingsReader;
            _logger = logger;
        }

        public async Task SendReceiptAsync(Receipt receipt)
        {
            _logger.LogInformation($"Handling receipt with ID {receipt.ReceiptID}.");

            await _receiptStorage.SaveAsync(new ReceiptStoreItem());

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
