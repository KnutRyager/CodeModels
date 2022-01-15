using System;
using System.Collections.Generic;

namespace Common.DataStructures;

public class RangeLookupTable<TRow, TColumn, TCell>
    : AbstractTable<TRow, TColumn, TCell>
    where TRow : IComparable<TRow>
    where TColumn : IComparable<TColumn>
{
    private readonly LookupRangeTableMode _rowMode;
    private readonly LookupRangeTableMode _columnMode;
    private readonly IDictionary<TRow, int>? _rowIndexForExactMode;
    private readonly IDictionary<TColumn, int>? _columnIndexForExactMode;

    public RangeLookupTable(string name, IList<TRow> rows, IList<TColumn> columns, IList<IList<TCell>> cells,
         LookupRangeTableMode rowMode = LookupRangeTableMode.Floor, LookupRangeTableMode columnMode = LookupRangeTableMode.Floor)
        : base(name, rows, columns, cells)
    {
        _rowMode = rowMode;
        _columnMode = columnMode;
        if (rowMode == LookupRangeTableMode.Exact)
        {
            _rowIndexForExactMode = new Dictionary<TRow, int>();
            for (var i = 0; i < rows.Count; i++) _rowIndexForExactMode[rows[i]] = i;
        }
        if (columnMode == LookupRangeTableMode.Exact)
        {
            _columnIndexForExactMode = new Dictionary<TColumn, int>();
            for (var i = 0; i < columns.Count; i++) _columnIndexForExactMode[columns[i]] = i;
        }
    }

    public override int GetRowIndex(TRow row) => _rowMode == LookupRangeTableMode.Exact ? (_rowIndexForExactMode!.ContainsKey(row) ? _rowIndexForExactMode[row] : -1) : RangeLookupTableCommon.RangeBinarySearchIndexOf(_rowMode, Rows, row);
    public override int GetColumnIndex(TColumn column) => _columnMode == LookupRangeTableMode.Exact ? (_columnIndexForExactMode!.ContainsKey(column) ? _columnIndexForExactMode[column] : -1) : RangeLookupTableCommon.RangeBinarySearchIndexOf(_columnMode, Columns, column);
}
