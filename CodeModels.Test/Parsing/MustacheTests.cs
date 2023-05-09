using System;
using Xunit;

namespace CodeModels.Parsing.Test;

public class MustacheTests
{
    [Fact]
    public void MustacheReplace_Simple_Success()
        => Assert.Equal("start MUSTACHE_inner end", "start {{inner}} end".RemoveMustache());

    [Fact]
    public void MustacheReplace_TripleBracket_Success()
        => Assert.Equal("start MUSTACHE_inner end", "start {{{inner}}} end".RemoveMustache());

    [Fact]
    public void MustacheReplace_IgnoreComment_Success()
        => Assert.Equal("start  end", "start {{!comment}} end".RemoveMustache());

    [Fact]
    public void MustacheReplace_IgnoreSectionMarkers_Success()
        => Assert.Equal("inside", "{{#section}}inside{{/section}}".RemoveMustache());

    [Fact]
    public void MustacheReplace_IgnoreInvertedSectionMarkers_Success()
        => Assert.Equal("inside", "{{^section}}inside{{/section}}".RemoveMustache());

    [Fact]
    public void MustacheReplace_IgnoreTemplateExpansion_Success()
        => Assert.Equal("start  end", "start {{>expand}} end".RemoveMustache());

    [Fact]
    public void MustacheReplace_UnclosedTag_Failure()
        => Assert.Throws<ArgumentException>(() => "start {{ end".RemoveMustache());

    [Fact]
    public void MustacheReplace_StrayEndTag_Failure()
        => Assert.Throws<ArgumentException>(() => "start {{ }} }} end".RemoveMustache());
}
