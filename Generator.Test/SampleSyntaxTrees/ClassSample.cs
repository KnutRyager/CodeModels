#pragma warning disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeAnalysisTests.SampleSyntaxTrees;

public class ClassSample
{
    [Required()]
    public const int Const = 0;
    public int ArrowProp => 0;

    public string StringProperty { get; set; }
    public int? OptionalProperty { get; set; }
    public Dictionary<int, bool> DictionaryProperty { get; set; }
    public Func<float, int> FuncPropery { get; set; }
    public int PrivateSetProp { get; private set; }
    public List<int> List { get; set; } = new List<int>() { 1, 2, 3 };
    public int[] Array { get; set; } = new int[] { 1, 2, 3 };
    public List<List<List<int>>> DeepList { get; set; }
    public static IDictionary<string, string[]> Dictionary = new Dictionary<string, string[]>()
    {
        { "a", new string[]{ "b", "c" } },
        { "b", new string[]{ "c", "d" } }
    };

    public int GetInt()
    {
        return 0;
    }

    public void AccersDictionary()
    {
        var value = DictionaryProperty[4];
    }

    public void UnaryAdd()
    {
        var x = 0;
        x++;
    }

    public void RecordWith()
    {
        var a = new RecordSample(1, "hi");

        var b = a with { A = 2, B = "sup" };

        var c = new RecordSample(1, "hi") with { A = 2, B = "sup" };
    }
}
