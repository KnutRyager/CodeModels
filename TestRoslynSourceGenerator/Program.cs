// Samples:
// https://github.com/dotnet/roslyn-sdk/blob/0313c80ed950ac4f4eef11bb2e1c6d1009b328c4/samples/CSharp/SourceGenerators/SourceGeneratorSamples/SourceGeneratorSamples.csproj#L13-L30
// Cookbook:
// https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md#breaking-changes
partial class Program
{
    static void Main(string[] args)
    {
        new UserClass().UserMethod();
        //HelloFrom("Generated Code");
        //Console.ReadLine();
    }

    //static partial void HelloFrom(string name);
}

public partial class UserClass
{
    public void UserMethod()
    {
        // call into a generated method inside the class
        //this.GeneratedMethod();
        HelloWorldGenerated.HelloWorld.SayHello();
    }

    public void Write(string s)
    {
        Console.WriteLine($"Sup, {s}");
    }
}