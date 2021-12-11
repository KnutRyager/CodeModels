namespace CodeAnalyzation.Models
{
    public class Parameter
    {
        public string Identifier { get; set; }
        public TType Type { get; set; }

        public Parameter(string identifier, TType type)
        {
            Identifier = identifier;
            Type = type;
        }
    }
}