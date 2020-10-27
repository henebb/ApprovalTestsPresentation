using System;
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

    public static class UserSettingsExtensions
    {
        public static bool IsTrue(this Dictionary<string, string> userSettings, string key)
        {
            return userSettings.ContainsKey(key) &&
                   userSettings[key].Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string[] ListValues(this Dictionary<string, string> userSettings, string key)
        {
            if (!userSettings.ContainsKey(key) ||
                string.IsNullOrWhiteSpace(userSettings[key]))
            {
                return new string[0];
            }

            return userSettings[key].Split(',', StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
