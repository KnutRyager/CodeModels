using System.ComponentModel.DataAnnotations;
using Models;

namespace CodeAnalyzation.DataTransformation
{
    [Model]
    public class NamespaceContainment
    {
        [Key] public int Id { get; set; }
        public int ParentNamespaceId { get; set; }
        public int ChildNamespaceId { get; set; }
        public Namespace ParentNamespace { get; set; } = default!;
        public Namespace ChildNamespace { get; set; } = default!;

        public NamespaceContainment() { }

        public NamespaceContainment(Namespace parentNamespace, Namespace childNamespace)
        {
            ParentNamespace = parentNamespace;
            ChildNamespace = childNamespace;
        }

        public override string ToString() => $"{ParentNamespace}.{ChildNamespace}";
    }
}