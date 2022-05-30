using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arch.CoreLibrary.Managers
{
    public static class SessionManager
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            string sessionKey = $"xs{key}";
            session.SetString(sessionKey, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string sessionKey = $"xs{key}";
            var value = session.GetString(sessionKey);

            return value== null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }

        public static void RemoveKey(this ISession session, string key)
        {
            string sessionKey = $"xs{key}";
            session.Remove(sessionKey);
        }
    }
}
