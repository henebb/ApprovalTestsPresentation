using System.Threading.Tasks;

namespace FredagsdragningNov2020
{
    public interface IEmailSender
    {
        Task SendAsync(string[] to, string subject, string body);
    }
}
