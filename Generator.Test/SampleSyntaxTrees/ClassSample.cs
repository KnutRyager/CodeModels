#pragma warning disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeAnalysisTests.SampleSyntaxTrees;

public class ClassSample
{
    [Required()]
    public const int Const = 0;

    public string StringProperty { get; set; }
    public int? OptionalProperty { get; set; }
    public Dictionary<int, bool> DictionaryProperty { get; set; }
    public Func<float, int> FuncPropery { get; set; }
    public int PrivateSetProp { get; private set; }
    public List<List<List<int>>> DeepList { get; set; }

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
