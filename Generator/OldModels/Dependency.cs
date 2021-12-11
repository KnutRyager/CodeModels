namespace CodeAnalyzation.Models
{
    public class Dependency
    {
        public string Identifier { get; set; }

        public Dependency(string identifier)
        {
            Identifier = identifier;
        }
    }
}