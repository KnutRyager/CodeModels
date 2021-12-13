#pragma warning disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeAnalysisTests.SampleSyntaxTrees;

public static class TupleSamples
{
    public static void Samples()
    {
        var t = (A: 5, B: "test");

        (int A, string B) t2 = (A: 5, B: "test");

        (int A, string B) t3 = (5, "test");

        (int, string) t4 = (5, "test");

        (int, string B) t5 = (5, "test");

        (int?, string? Item2) t6 = (5, "test");
    }
}
