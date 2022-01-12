#nullable disable
using Models;

namespace TestRoslynSourceGenerator.Models
{
    [Model]
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public Address Address { get; set; }

        public override string ToString() => $"Person: {FullName} at {Address.AddressLine}";
    }

    [Model]
    public class Address
    {
        public string Street { get; set; }
        public string Number { get; set; }
        public string AddressLine => $"{Street} {Number}";
    }
}
public static class ModelDependencies
{
    public static readonly IDictionary<string, string[]> Person = new Dictionary<string, string[]>()
    {{"FirstName", new string[]{}}, {"LastName", new string[]{}}, {"FullName", new string[]{"FirstName", "LastName"}}, {"Address", new string[]{}}, {"ToString", new string[]{"FullName", "Address", "Address.AddressLine", "Address.Street", "Address.Number", "FirstName", "LastName"}}};
    public static readonly IDictionary<string, string[]> Address = new Dictionary<string, string[]>()
    {{"Street", new string[]{}}, {"Number", new string[]{}}, {"AddressLine", new string[]{"Street", "Number"}}};
    public static readonly IDictionary<string, IDictionary<string, string[]>> Deps = new Dictionary<string, IDictionary<string, string[]>>()
    {{"Person", ModelDependencies.Person}, {"Address", ModelDependencies.Address}};
}