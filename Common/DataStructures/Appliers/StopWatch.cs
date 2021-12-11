using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.DataStructures
{
    public class StopWatch
    {
        private IDictionary<string, TimeSpan> _entries { get; } = new Dictionary<string, TimeSpan>();

        private DateTime previousTime;

        public StopWatch()
        {
            previousTime = DateTime.UtcNow;
        }

        public void Add(string description)
        {
            var duration = DateTime.UtcNow - previousTime;
            if (_entries.ContainsKey(description)) _entries[description] += duration;
            else _entries[description] = duration;
            previousTime = DateTime.UtcNow;
        }

        public void Misc() => Add("Diverse");

        public IList<(string Description, TimeSpan Time)> Entries => _entries.Select(x => (x.Key, x.Value)).ToArray();

    }
}