using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StarredSeaMUON.Database
{
    internal class DBTableConversionHelper
    {
        public static string Serialize<T>(T input)
        {
            return JsonSerializer.Serialize((T)input, (JsonSerializerOptions)default);
        }
        public static T Deserialize<T>(string input)
        {
            return JsonSerializer.Deserialize<T>(input, (JsonSerializerOptions)default);
        }
    }
}
