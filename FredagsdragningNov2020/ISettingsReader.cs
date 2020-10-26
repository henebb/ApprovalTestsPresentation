using System.Threading.Tasks;

namespace FredagsdragningNov2020
{
    public interface ISettingsReader
    {
        Task<string[]> GetSettingByKey(ToAddressType toAddressType);
    }
}
