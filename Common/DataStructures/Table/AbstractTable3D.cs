using System;
using System.Collections.Generic;
using Common.Util;

namespace Common.DataStructures;

public abstract class AbstractTable3D<TRow, TColumn, TDepth, TCell>
    : ILookupTable3D<TRow, TColumn, TDepth, TCell>
    where TRow : IComparable<TRow>
    where TColumn : IComparable<TColumn>
    where TDepth : IComparable<TDepth>
{
    public IList<TRow> Rows { get; }
    public IList<TColumn> Columns { get; }
    public IList<TDepth> Depths { get; }
    protected IList<IList<IList<TCell>>> Cells { get; }
    protected readonly string _name;

    public AbstractTable3D(string name, IList<TRow> rows, IList<TColumn> columns, IList<TDepth> depths, IList<IList<IList<TCell>>> cells)
    {
        _name = name;
        Rows = rows;
        Columns = columns;
        Depths = depths;
        Cells = cells;
    }

    public int RowCount => Rows.Count;
    public int ColumnCount => Columns.Count;
    public int DepthCount => Depths.Count;

    public int ColumnCountAtRow(int rowIndex) => Cells[rowIndex].Count;
    public int DepthCountAtRow(int rowIndex, int columnIndex) => Cells[rowIndex][columnIndex].Count;
    public TRow GetRow(int index) => Rows[index];
    public TRow GetRow(TRow row, int offset = 0) => GetRow(GetRowIndex(row) + offset);
    public TColumn GetColumn(int index) => Columns[index];
    public TColumn GetColumn(TColumn column, int offset = 0) => GetColumn(GetColumnIndex(column) + offset);
    public TDepth GetDepth(int index) => Depths[index];
    public TDepth GetDepth(TDepth depth, int offset = 0) => GetDepth(GetDepthIndex(depth) + offset);
    public virtual int GetRowIndex(TRow row) => Rows.IndexOf(row);
    public virtual int GetColumnIndex(TColumn column) => Columns.IndexOf(column);
    public virtual int GetDepthIndex(TDepth depth) => Depths.IndexOf(depth);

    public bool Contains(TRow row, TColumn column, TDepth depth)
        => MathUtil.InRange(GetRowIndex(row), 0, RowCount) && MathUtil.InRange(GetColumnIndex(column), 0, ColumnCountAtRow(GetRowIndex(row))) && MathUtil.InRange(GetDepthIndex(depth), 0, DepthCountAtRow(GetRowIndex(row), GetColumnIndex(column)));

    public TCell MaybeValue(TRow row, TColumn column, TDepth depth)
    {
        var rowIndex = GetRowIndex(row);
        if (rowIndex < 0 || rowIndex >= Rows.Count) return default!;
        var columnIndex = GetColumnIndex(column);
        if (columnIndex < 0 || columnIndex >= Cells[rowIndex].Count) return default!;
        var depthIndex = GetDepthIndex(depth);
        return depthIndex >= 0 && depthIndex < Cells[rowIndex][columnIndex].Count ? Cells[rowIndex][columnIndex][depthIndex] : default!;
    }

    public TCell this[TRow row, TColumn column, TDepth depth] => this[row, column, depth, 0, 0, 0];
    public TCell this[TRow row, TColumn column, TDepth depth, int rowOffset = 0, int columnOffset = 0, int depthOffset = 0]
    {
        get
        {
            var rowIndex = GetRowIndex(row);
            if (!MathUtil.InRange(rowIndex, 0, RowCount)) throw new ArgumentException($"Not found in lookuptable '{_name}': '{row}', '{column}', '{depth}'");
            rowIndex = MathUtil.AddBoundedOffset(rowIndex, rowOffset, 0, Rows.Count);
            var columnIndex = GetColumnIndex(column);
            if (!MathUtil.InRange(columnIndex, 0, ColumnCountAtRow(rowIndex))) throw new ArgumentException($"Not found in lookuptable '{_name}': '{row}', '{column}', '{depth}'");
            columnIndex = MathUtil.AddBoundedOffset(columnIndex, columnOffset, 0, ColumnCountAtRow(rowIndex));
            var depthIndex = GetDepthIndex(depth);
            if (!MathUtil.InRange(depthIndex, 0, DepthCountAtRow(rowIndex, columnIndex))) throw new ArgumentException($"Not found in lookuptable '{_name}': '{row}', '{column}', '{depth}'");
            depthIndex = MathUtil.AddBoundedOffset(depthIndex, depthOffset, 0, DepthCountAtRow(rowIndex, columnIndex));
            return Cells[rowIndex][columnIndex][depthIndex];
        }
    }
}
