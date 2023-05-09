# Code Models
A C# code generation framework based on Roslyn.

Code models corresponds to Roslyn syntax nodes, so a code model is convertible to source code through syntax trees.

Code models can be constructed either from CodeModelFactory, from Reflection data, or from syntax nodes + semantic analysis.

## Emulation
A C# C# emulator is also included, to execute the code models. This is handy for unit testing.

## Further abstraction levels
The next levels of abstraction are **abstract code models** that are no longer 1:1 with syntax nodes, followed by **program models**, which operate on higher levels with fewer implementation details, that can be converted to source code through code models.

### Levels:
- Source code
- Syntax nodes + Semantic analysis
- Code models: Models tying together syntax nodes, semantic analysis and reflection
- Abstract code models: Can be composed of various code models. E.g. "NamedValues - a collection of named properties/values", which could be implemented with a class/record/tuple
- Program models: Program specification with important implementation details such as which frameworks are used. E.g. "A .NET WebApi using EntityFramework"
- Program specifications: Lightweight, api interfaces only. E.g. OpenAPI specifications
