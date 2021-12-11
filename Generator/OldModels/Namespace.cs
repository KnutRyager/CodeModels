using System.Collections.Generic;

namespace CodeAnalyzation.Models
{
    public class Namespace
    {
        public IEnumerable<string> Parts { get; set; }

        public Namespace(IEnumerable<string> parts)
        {
            Parts = parts;
        }
    }
}