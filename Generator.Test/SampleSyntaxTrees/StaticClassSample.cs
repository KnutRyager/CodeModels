#pragma warning disable
using System.Collections.Generic;

namespace CodeAnalysisTests.SampleSyntaxTrees;

public static class SampleStaticClass
{
    public const int ConstInt = 0;

    public static void SetValue(int value)
    {

    }

    public static int GetValue()
    {
        return 0;
    }

    public static int StringExtensionMethod(this string sampleString)
    {
        return 0;
    }

    public class SampleCodeClass
    {

    }

    public static int CustomExtensionMethod(this SampleCodeClass sampleString)
    {
        return 0;
    }

    public static int ListMethod(List<string> sampleString)
    {
        return 0;
    }

    public static int ListCustomMethod(List<SampleCodeClass> sampleString, int value)
    {
        return 0;
    }

    public static int ArrayMethod(string[] sampleString)
    {
        return 0;
    }
}
