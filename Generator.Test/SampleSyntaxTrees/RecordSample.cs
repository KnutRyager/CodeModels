#pragma warning disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeAnalysisTests.SampleSyntaxTrees;

public record RecordSample(int A, string B = "sample", Int32 C = 0);

public record RecordSample2(RecordSample MasterRecord);
