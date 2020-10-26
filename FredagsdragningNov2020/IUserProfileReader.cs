using System.Collections.Generic;
using System.Threading.Tasks;

namespace FredagsdragningNov2020
{
    public interface IUserProfileReader
    {
        Task<Dictionary<string, string>> GetUserSettings(string user);
    }

    public static class UserSettingKeys
    {
        public const string ReadsReceiptsOnline = "ReadsReceiptsOnline";
        public const string EmailAddresses = "EmailAddresses";
    }
}
