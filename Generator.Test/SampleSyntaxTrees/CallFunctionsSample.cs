#pragma warning disable
using System;
using System.Collections.Generic;

namespace CodeAnalysisTests.SampleSyntaxTrees;

public static class CallFunctionsSample
{
    private static int Prop = 5;
    private static int Func(int n) { return n; }
    private class A { public int Prop; public int getValue() { return this.Prop; } }

    public static void CallFunctions()
    {
        var n = 1337;
        var d = 1337d;
        var boolean = true;
        var s = "s";
        var a = Math.Sqrt(10);
        var b = Func(n);
        var pi = Math.PI;
        var type = Prop;
        var aInstance = new A();
        var type2 = aInstance.Prop;
        var v2 = aInstance.getValue();
        var instance = new Exception();
        var casted = (int)10.5;
    }
}
