using System.Collections.Generic;
using Common.Extensions;

namespace Common.DataStructures;

public class CanonicalNameConverter
{
    private readonly IDictionary<string, string> _conversions;
    private readonly bool _caseInsensitive;
    private readonly bool _trim;
    private readonly bool _onlyToFirstSpace;

    public CanonicalNameConverter(IDictionary<string, string> conversions, bool caseInsensitive = true, bool trim = true, bool onlyToFirstSpace = true)
    {
        _caseInsensitive = caseInsensitive;
        _trim = trim;
        _onlyToFirstSpace = onlyToFirstSpace;
        var keys = new List<string>(conversions.Keys);
        if (caseInsensitive) foreach (var key in keys) conversions[key.ToLower()] = conversions[key];
        if (trim) foreach (var key in keys) conversions[key.Trim()] = conversions[key];
        if (onlyToFirstSpace) foreach (var key in keys) conversions[key.Split(" ")[0]] = conversions[key];
        _conversions = conversions;
    }

    public string this[string s]
    {
        get
        {
            var comparison = s;
            if (_caseInsensitive) comparison = comparison.ToLower();
            if (_trim) comparison = comparison.Trim();
            if (_conversions.ContainsKey(comparison)) return _conversions[comparison];
            if (_onlyToFirstSpace) comparison = comparison.Split(" ")[0];
            return _conversions.ContainsKey(comparison) ? _conversions[comparison] : s;
        }
    }

    public List<List<string>> ConvertTable(List<List<string>> table)
    {
        ConvertHeader(table[0]);
        return table;
    }

    public void ConvertHeader(List<string> header)
    {
        for (var i = 0; i < header.Count; i++) header[i] = this[header[i]];
    }
}
