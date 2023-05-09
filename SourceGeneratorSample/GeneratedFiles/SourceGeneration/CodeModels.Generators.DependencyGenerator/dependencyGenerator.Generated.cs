public static class ModelDependencies
{
    public static readonly IDictionary<string, string[]> Person = new Dictionary<string, string[]>()
    {{"Id", new string[]{}}, {"FirstName", new string[]{}}, {"LastName", new string[]{}}, {"FullName", new string[]{"FirstName", "LastName"}}, {"Address", new string[]{}}, {"ToString", new string[]{"FullName", "Address", "Address.AddressLine", "Address.Street", "Address.Number", "FirstName", "LastName"}}};
    public static readonly IDictionary<string, string[]> Address = new Dictionary<string, string[]>()
    {{"Street", new string[]{}}, {"Number", new string[]{}}, {"AddressLine", new string[]{"Street", "Number"}}};
    public static readonly IDictionary<string, Dictionary<string, string[]>> Deps = new Dictionary<string, Dictionary<string, string[]>>()
    {{"Person", ModelDependencies.Person}, {"Address", ModelDependencies.Address}};
}