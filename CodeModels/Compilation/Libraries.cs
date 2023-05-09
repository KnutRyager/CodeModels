using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzation.Compilation;

public static class Libraries
{
    public static PortableExecutableReference Mscor = GetReference<object>();
    public static PortableExecutableReference Environment = GetReference(typeof(Environment.SpecialFolder));
    public static PortableExecutableReference Linq = GetReference(typeof(Enumerable));
    public static PortableExecutableReference IO = GetReference(typeof(File));
    public static PortableExecutableReference Text = GetReference(typeof(System.Text.Encoder));
    public static PortableExecutableReference RegularExpressions = GetReference(typeof(System.Text.RegularExpressions.Match));
    public static PortableExecutableReference Net = GetReference<HttpClient>();
    public static PortableExecutableReference StringManipulation = GetReference(typeof(System.String));
    public static PortableExecutableReference Threading = GetReference(typeof(System.Threading.Barrier));
    public static PortableExecutableReference Task = GetReference(typeof(System.Threading.Tasks.Task));
    public static PortableExecutableReference Console = GetReference(typeof(Console));
    public static PortableExecutableReference Xml = GetReference(typeof(System.Xml.ConformanceLevel));
    public static PortableExecutableReference Security = GetReference(typeof(System.Security.AllowPartiallyTrustedCallersAttribute));
    public static PortableExecutableReference X509Store = GetReference(typeof(System.Security.Cryptography.X509Certificates.X509Store));
    public static PortableExecutableReference Collections = GetReference(typeof(List<>));
    public static PortableExecutableReference CollectionsGeneric = GetReference(typeof(System.Collections.Generic.List<>));
    public static PortableExecutableReference Concurrent = GetReference(typeof(ConcurrentDictionary<,>));
    public static PortableExecutableReference Numerics = GetReference(typeof(System.Numerics.BigInteger));

    public static readonly PortableExecutableReference[] StandardSystemLibraries = new[] {
        Mscor,
        Environment,
        Linq,
        IO,
        Text,
        RegularExpressions,
        Net,
        StringManipulation,
        Threading,
        Task,
        Console,
        Xml,
        Security,
        X509Store,
        Collections,
        Concurrent,
        Numerics
    };

    private static PortableExecutableReference GetReference<T>() => GetReference(typeof(T));
    private static PortableExecutableReference GetReference(Type type) => MetadataReference.CreateFromFile(type.Assembly.Location);
}
