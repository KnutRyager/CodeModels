using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.DataStructures
{
    public static class Table
    {
        public static (List<TCell> cells, List<TRow> rows, List<TColumn> columns) MakeTable<TCell, TRow, TColumn, TCellType>(IList<IList<object>> values,
            Func<TCellType, TCellType, TCellType, TRow> makeRow, Func<TCellType, TCellType, TCellType, TColumn> makeColumn, Func<TCellType, TRow, TColumn, TCell> makeCell, Func<string, TCellType> cellParser,
            string defaultEndString = "999999999")
        {
            var rows = new List<TRow>();
            var columns = new List<TColumn>();
            var cells = new List<TCell>();
            var topRow = ((List<object>)values[0]);
            var headersRow = topRow.Skip(1).ToList();
            var bottomRows = values.Skip(1).ToList();
            var headersColumn = bottomRows.Select(x => ((List<object>)x)[0]).ToList();

            TCellType previous = default!;
            for (var i = 0; i < headersRow.Count; i++)
            {
                var header = i < headersRow.Count ? headersRow[i] : defaultEndString;
                var currentValue = cellParser((string)header);
                var nextValue = cellParser((string)(i + 1 < headersRow.Count ? headersRow[i + 1] : defaultEndString));
                columns.Add(makeColumn(previous!, currentValue, nextValue));
                previous = currentValue;
            }
            previous = default!;
            for (var i = 0; i < headersColumn.Count; i++)
            {
                var header = i < headersColumn.Count ? headersColumn[i] : defaultEndString;
                var currentValue = cellParser((string)header);
                var nextValue = cellParser((string)(i + 1 < headersColumn.Count ? headersColumn[i + 1] : defaultEndString));
                rows.Add(makeRow(previous!, currentValue, nextValue));
                previous = currentValue;
            }

            for (var i = 0; i < bottomRows.Count; i++)
            {
                var row = bottomRows[i];
                var valuesInner = row.Skip(1).ToList();
                for (var j = 0; j < valuesInner.Count; j++)
                {
                    cells.Add(makeCell(cellParser((valuesInner[j] as string)!), rows[i], columns[j]));
                }
            }
            return (cells, rows, columns);
        }
    }
}