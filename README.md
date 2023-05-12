# Code Models
A C# code generation framework based on Roslyn.

Code models corresponds to Roslyn syntax nodes, so a code model is convertible to source code through syntax trees.

Code models can be constructed either from CodeModelFactory, from Reflection data, or from syntax nodes + semantic analysis.

## Further abstraction levels
The next levels of abstraction are **abstract code models** that are no longer 1:1 with syntax nodes, followed by **program models**, which operate on higher levels with fewer implementation details, that can be converted to source code through code models.

### Levels:
- Source code
- Syntax nodes + Semantic analysis
- Code models: Models tying together syntax nodes, semantic analysis and reflection
- Abstract code models: Can be composed of various code models. E.g. "NamedValues - a collection of named properties/values", which could be implemented with a class/record/tuple
- Program models: Program specification with important implementation details such as which frameworks are used. E.g. "A .NET WebApi using EntityFramework"
- Program specifications: Lightweight, api interfaces only. E.g. OpenAPI specifications

## Emulation
A C# C# emulator is also included, to execute the code models. This is handy for unit testing.

#### Supported:
- Basic statements (assignment, return, break)
- Basic operators (+,-,*,/,...)
- Control flow (if, else, for, foreach, do, while, switch)
- Methods (body, expresison body, return)
- Exception handling (try, catch)
- Basic operators (unary, binary, ternary)
- Basic Expressions (identifier, invocation, member access)
- Global statements/declarations
- Classes (fields, properties, methods, constructors)
- Enums
- Printing to emulated console
- Running reflected code (types, methods, fields, properties, constructors)
- Lambdas

#### Roadmap:
- Inheritance (And inherit defaults like ToString)
- Recursive Identifiers
- Records
- Interfaces
- Structs
- Patterns
- Casting/as

#### Doubtfull:
- Mixing generic generated and pre existing types 
- Reflection API
- Reordering dependant declarations
- Multiple files
- Operator overloading
- Lambdas: capture variables

## Contribute
The codebase is a bit messy for now. However it's very simple to contribute by making unit tests, especially for the emulator when parsing from source, as it's completely independent on how the project is implemented. See: https://github.com/KnutRyager/CodeModels/blob/master/CodeModels.Test/Execution/Classes/MakeClassEvalTests.cs

This tests builds CodeModels directly:
https://github.com/KnutRyager/CodeModels/blob/master/CodeModels.Test/Execution/Classes/ClassPropertyTests.cs

A third important test type is source generation tests, done mostly from code models:
https://github.com/KnutRyager/CodeModels/blob/master/CodeModels.Test/Models/Primitives/MethodTests.cs

What's also planned is source -> CodeModel -> source tests, which would test both parsing and source generation, without requiring any knowledge to write.

You can make tests for unsupported features and simply skip them with [Fact(Skip = "Not implemented")], although look at the roadmap on what's most realistic to be supported.
