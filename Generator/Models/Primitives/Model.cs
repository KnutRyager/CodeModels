using Newtonsoft.Json;

namespace CodeAnalyzation.Models
{
    public class Model
    {
        [JsonProperty(Order = -10)]
        public string Identifier { get; set; }
        [JsonProperty(Order = -10)]
        public Namespace? Namespace { get; set; }

        public Model(string identifier, Namespace? @namespace)
        {
            Identifier = identifier;
            Namespace = @namespace;
        }
    }
}