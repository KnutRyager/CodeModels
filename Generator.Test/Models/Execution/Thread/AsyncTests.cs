using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace CodeAnalyzation.Models.Execution.Thread.Test;

public class AsyncTests
{
    [Fact] public void TaskFromResult() => @"
using System.Threading.Tasks;
Task.FromResult(""Success"").Result;
".Eval().Should().Be("Success");

    [Fact] public void AwaitTask() => @"
using System.Threading.Tasks;
await Task.Run(() =>
{
    System.Console.Write(""Success"");
}
".Eval().Should().Be("Success");

    [Fact] public void AwaitTask1() => @"
using System.Threading.Tasks;
await Task.FromResult(""Success"");
".Eval().Should().Be("Success");

    [Fact] public void AwaitHttpClient() => @"
using System.Net.Http;
using System.Text.RegularExpressions;
using System;

string url = ""https://example.com/"";

HttpClient client = new HttpClient();
string html = await client.GetStringAsync(url);

string titlePattern = @""\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>"";
Match titleMatch = Regex.Match(html, titlePattern);
//Match titleMatch = Regex.Match(html, titlePattern, RegexOptions.IgnoreCase);

string title = titleMatch.Groups[""Title""].Value.Trim();

Console.Write(title);
}".Eval().Should().Be("Example Domain");

    private void Test()
    {
        var a = Task.FromResult(1);
    }
    private async Task<int> Test2()
    {
        await Task.Run(() =>
        {
            Console.Write("Success");
        });
        var httpClient = new HttpClient();
        //Task<Func<string, string>> f = async x => await Task.FromResult(x);
        //await Task<string>.Run((x) =>
        //{
        //    Console.Write(x);
        //});
        string url = "https://example.com/";

        HttpClient client = new HttpClient();
        string html = await client.GetStringAsync(url);

        string titlePattern = @"";
        System.Text.RegularExpressions.Match titleMatch = 
            System.Text.RegularExpressions.Regex.Match(html, titlePattern,
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        Match titleMatch2 = Regex.Match(html, titlePattern, RegexOptions.IgnoreCase);


        string title = titleMatch.Groups["Title"].Value.Trim();

        return 4;
    }

    private async Task<string> F() => await Task<string>.FromResult("Success");
}
