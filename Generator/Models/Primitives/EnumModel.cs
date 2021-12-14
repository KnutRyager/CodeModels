using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;


namespace CodeAnalyzation.Models
{
    public class EnumModel
    {
        [JsonProperty(Order = -10)]
        public string Identifier { get; set; }
        [JsonProperty(Order = -10)]
        public Namespace Namespace { get; set; }
        public string[] Values { get; set; }

        public EnumModel(string identifier, Namespace @namespace, IEnumerable<string> values)
        {
            Identifier = identifier;
            Namespace = @namespace;
            Values = values?.ToArray() ?? System.Array.Empty<string>();
        }
    }
}