#pragma warning disable
#region Assembly System.Linq, Version=4.2.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// C:\Program Files\dotnet\sdk\NuGetFallbackFolder\microsoft.netcore.app\2.2.0\ref\netcoreapp2.2\System.Linq.dll
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CodeAnalysisTests.SampleSyntaxTrees;

public interface EnumerableSample
{
    TSource Aggregate<TSource>(IEnumerable<TSource> source, Func<TSource, TSource, TSource> func);
    TAccumulate Aggregate<TSource, TAccumulate>(IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func);
    TResult Aggregate<TSource, TAccumulate, TResult>(IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector);
    bool All<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate);

}
