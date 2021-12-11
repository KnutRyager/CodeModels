using System;
using System.Collections.Generic;

namespace Common.DataStructures
{
    public class RangeLookupTable3D<TRow, TColumn, TDepth, TCell>
        : AbstractTable3D<TRow, TColumn, TDepth, TCell>
        where TRow : IComparable<TRow>
        where TColumn : IComparable<TColumn>
        where TDepth : IComparable<TDepth>
    {
        private readonly LookupRangeTableMode _rowMode;
        private readonly LookupRangeTableMode _columnMode;
        private readonly LookupRangeTableMode _depthMode;
        private readonly IDictionary<TRow, int>? _rowIndexForExactMode;
        private readonly IDictionary<TColumn, int>? _columnIndexForExactMode;
        private readonly IDictionary<TDepth, int>? _depthIndexForExactMode;

        public RangeLookupTable3D(string name, IList<TRow> rows, IList<TColumn> columns, IList<TDepth> depths, IList<IList<IList<TCell>>> cells,
             LookupRangeTableMode rowMode = LookupRangeTableMode.Floor, LookupRangeTableMode columnMode = LookupRangeTableMode.Floor, LookupRangeTableMode depthMode = LookupRangeTableMode.Floor)
            : base(name, rows, columns, depths, cells)
        {
            _rowMode = rowMode;
            _columnMode = columnMode;
            _depthMode = depthMode;
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
            if (depthMode == LookupRangeTableMode.Exact)
            {
                _depthIndexForExactMode = new Dictionary<TDepth, int>();
                for (var i = 0; i < depths.Count; i++) _depthIndexForExactMode[depths[i]] = i;
            }
        }

        public override int GetRowIndex(TRow row) => _rowMode == LookupRangeTableMode.Exact ? (_rowIndexForExactMode!.ContainsKey(row) ? _rowIndexForExactMode[row] : -1) : RangeLookupTableCommon.RangeBinarySearchIndexOf(_rowMode, Rows, row);
        public override int GetColumnIndex(TColumn column) => _columnMode == LookupRangeTableMode.Exact ? (_columnIndexForExactMode!.ContainsKey(column) ? _columnIndexForExactMode[column] : -1) : RangeLookupTableCommon.RangeBinarySearchIndexOf(_columnMode, Columns, column);
        public override int GetDepthIndex(TDepth depth) => _depthMode == LookupRangeTableMode.Exact ? (_depthIndexForExactMode!.ContainsKey(depth) ? _depthIndexForExactMode[depth] : -1) : RangeLookupTableCommon.RangeBinarySearchIndexOf(_depthMode, Depths, depth);
    }
}