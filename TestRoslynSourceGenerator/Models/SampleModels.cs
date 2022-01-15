#nullable disable
using Models;

namespace TestRoslynSourceGenerator.Models
{
    [Model]
    public class Person
    {
        public string Id { get; set; }
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