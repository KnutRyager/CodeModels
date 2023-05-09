#pragma warning disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeAnalysisTests.SampleSyntaxTrees;

public interface InterfaceSample
{
    //[Required()]
    //public const int Const = 0;

    //public string StringProperty { get; set; }
    //public int? OptionalProperty { get; set; }
    //public Dictionary<int, bool> DictionaryProperty { get; set; }
    //public Func<float, int> FuncPropery { get; set; }
    //public int PrivateSetProp { get; private set; }
    //public List<List<List<int>>> DeepList { get; set; }

    int GetInt();
}
