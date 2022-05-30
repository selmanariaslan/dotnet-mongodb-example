using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.CoreLibrary.Utils.Data
{
    public static class JsonUtils
    {
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static string ToJson(this object obj, Formatting formatting)
        {
            return JsonConvert.SerializeObject(obj, formatting);
        }

        public static string ToJson(this object obj, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(obj, settings);
        }

        public static T FromJson<T>(this object obj)
        {
            return JsonConvert.DeserializeObject<T>(obj as string);
        }

        public static T FromJson<T>(this object obj, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(obj as string, settings);
        }
    }
}
