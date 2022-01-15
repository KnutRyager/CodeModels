using System;
using System.Collections.Generic;
using Common.Util;

namespace Common.DataStructures;

public abstract class AbstractTable<TRow, TColumn, TCell>
    : ILookupTable<TRow, TColumn, TCell>
    where TRow : IComparable<TRow>
    where TColumn : IComparable<TColumn>
{
    public IList<TRow> Rows { get; }
    public IList<TColumn> Columns { get; }
    protected IList<IList<TCell>> Cells { get; }
    protected readonly string _name;

    public AbstractTable(string name, IList<TRow> rows, IList<TColumn> columns, IList<IList<TCell>> cells)
    {
        _name = name;
        Rows = rows;
        Columns = columns;
        Cells = cells;
    }

    public int RowCount => Rows.Count;
    public int ColumnCount => Columns.Count;

    IList<TRow> ILookupTable<TRow, TColumn, TCell>.Rows => throw new NotImplementedException();
    public int ColumnCountAtRow(int rowIndex) => Cells[rowIndex].Count;
    public TRow GetRow(int index) => Rows[index];
    public TRow GetRow(TRow row, int offset = 0) => GetRow(GetRowIndex(row) + offset);
    public TColumn GetColumn(int index) => Columns[index];
    public TColumn GetColumn(TColumn column, int offset = 0) => GetColumn(GetColumnIndex(column) + offset);
    public virtual int GetRowIndex(TRow row) => Rows.IndexOf(row);
    public virtual int GetColumnIndex(TColumn column) => Columns.IndexOf(column);

    public bool Contains(TRow row, TColumn column)
        => MathUtil.InRange(GetRowIndex(row), 0, RowCount) && MathUtil.InRange(GetColumnIndex(column), 0, ColumnCountAtRow(GetRowIndex(row)));

    public TCell MaybeValue(TRow row, TColumn column)
    {
        var rowIndex = GetRowIndex(row);
        if (rowIndex < 0 || rowIndex >= Rows.Count) return default!;
        var columnIndex = GetColumnIndex(column);
        return columnIndex >= 0 && columnIndex < Cells[rowIndex].Count ? Cells[rowIndex][columnIndex] : default!;
    }

    public TCell this[TRow row, TColumn column] => this[row, column, 0, 0];
    public TCell this[TRow row, TColumn column, int rowOffset = 0, int columnOffset = 0]
    {
        get
        {
            var rowIndex = GetRowIndex(row);
            if (!MathUtil.InRange(rowIndex, 0, RowCount)) throw new ArgumentException($"Not found in lookuptable '{_name}': '{row}', '{column}'");
            rowIndex = MathUtil.AddBoundedOffset(rowIndex, rowOffset, 0, Rows.Count);
            var columnIndex = GetColumnIndex(column);
            if (!MathUtil.InRange(columnIndex, 0, ColumnCountAtRow(rowIndex))) throw new ArgumentException($"Not found in lookuptable '{_name}': '{row}', '{column}'");
            columnIndex = MathUtil.AddBoundedOffset(columnIndex, columnOffset, 0, ColumnCountAtRow(rowIndex));
            return Cells[rowIndex][columnIndex];
        }
    }
}
