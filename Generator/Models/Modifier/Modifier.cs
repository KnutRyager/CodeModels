using System;
using System.Collections.Generic;
using System.Linq;
using Common.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

[Flags]
public enum Modifier
{
    None = 0,
    // Access
    Private = 1,
    Protected = 2,
    Internal = 4,
    Public = 8,
    // Static
    Static = 16,
    // Member
    Class = 32,
    Struct = 64,
    Enum = 128,
    Interface = 256,
    // Const
    Const = 512,
    Readonly = 1024,
    // Abstract
    Abstract = 2048,
    Virtual = 4096,
    Override = 8192,
    New = 16384,
    // Record
    Record = 32768,
    // Property type
    Field = 65536,
    Property = 131072,
    // Async
    Async = 262144,
    Await = 524288,
    // Using
    Using = 1048576,
    // Partial
    Partial = 2097152,
}