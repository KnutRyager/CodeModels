namespace ApiGenerator;

using CodeModels.AbstractCodeModels.Collection;
using static CodeModels.Factory.AbstractCodeModelFactory;

public record ModelModel(string Name, NamedValueCollection Properties)
{
    public static ModelModel Create(string name, NamedValueCollection? properties = default)
        => new(name, properties ?? NamedValues());
}