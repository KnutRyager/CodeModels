using CodeModels.Execution;
using FluentAssertions;
using Xunit;

namespace CodeModels.Test.Execution.Thread;

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

    [Fact] public void AwaitHttpClient2() => @"
using System;
using System.Net.Http;
using System.Linq; 
var client = new HttpClient(); 
string html = await client.GetStringAsync(""https://example.com"");
//var title1 = html.Split(""<title>""); 
//var title2 = title1[1]; 
//var title3 = title2.Split(""</title>""); 
//var title = title3[0]; 
var title = html.Split(""<title>"")[1].Split(""</title>"")[0]; 
Console.Write(title);
".Eval().Should().Be("Example Domain");

    [Fact(Skip = "Can only be run locally")] public void GetCertificates() => (@"
using System.Security.Cryptography.X509Certificates;
X509Store store = new X509Store(StoreName.My,StoreLocation.LocalMachine);
store.Open(OpenFlags.ReadOnly);
foreach (X509Certificate2 cert in store.Certificates) {
    System.Console.WriteLine(cert.Subject);
}
store.Close();
".Eval() is string { Length: >= 3 } s ? s[..3] : "").Should().Be("CN=");
}
