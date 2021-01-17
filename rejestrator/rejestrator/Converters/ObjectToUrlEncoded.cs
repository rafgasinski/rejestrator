namespace rejestrator.Converters
{
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public static class ObjectToUrlEncoded
    {
        public static string ToKeyValueURL(this object obj)
        {
            var keyvalues = obj.GetType().GetProperties()
                .ToList()
                .Select(p => $"{p.Name}={p.GetValue(obj)}")
                .ToArray();

            return string.Join("&", keyvalues);
        }
    }
}
