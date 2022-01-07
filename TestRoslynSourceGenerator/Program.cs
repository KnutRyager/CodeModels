// Samples:
// https://github.com/dotnet/roslyn-sdk/blob/0313c80ed950ac4f4eef11bb2e1c6d1009b328c4/samples/CSharp/SourceGenerators/SourceGeneratorSamples/SourceGeneratorSamples.csproj#L13-L30
// Cookbook:
// https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md#breaking-changes
partial class Program
{
    static void Main(string[] args)
    {
        HelloWorld2.UserMethod();
        GeneratorLog.Print();
        //HelloFrom("Generated Code");
        //Console.ReadLine();
    }

    //static partial void HelloFrom(string name);
}


public static partial class GeneratorLog
{
    public static void Print()
    {
        var fields = typeof(GeneratorLog).GetFields();
        foreach (var field in fields)
        {
            var fieldName = field.Name;
            if (fieldName.StartsWith("log_"))
            {
                Console.WriteLine($"{fieldName[4..]}:");
                var strings = (field.GetValue(null) as string[])!;
                foreach (var s in strings)
                {
                    Console.WriteLine(s);
                }
            }
        }
        foreach(var method in typeof(GeneratorLog).GetMethods())
        {
            var name = method.Name;
            if (name.StartsWith("Print_"))
            {
                method.Invoke(null, Array.Empty<object?>());
            }
        }
    }
}

public static partial class HelloWorld2
{
    public static void UserMethod()
    {
        HelloWorldGenerated.HelloWorld.SayHello();
    }
}