using System.Collections.Generic;

namespace Common.DataStructures
{
    public interface ILookupTable<TIn, TOut> : IMappingTable<TIn, TOut>, IEnumerable<TOut>
    {
        int Count { get; }
        int IndexOf(TIn Entry);
        IList<TOut> Rows { get; }
        TOut GetEntry(int index);
        TOut GetEntry(TIn Entry, int offset = 0);
        bool Contains(TIn Entry);
        int IndexOfClosest(TIn value);
        (TIn, TOut)? Closest(TIn value);
        TOut ClosestFloor(TIn value);
    }

    public interface ILookupTable<TRow, TColumn, TOut>
    {
        int RowCount { get; }
        int ColumnCount { get; }
        int ColumnCountAtRow(int rowIndex);
        int GetRowIndex(TRow row);
        IList<TRow> Rows { get; }
        TRow GetRow(int index);
        TRow GetRow(TRow row, int offset = 0);
        IList<TColumn> Columns { get; }
        int GetColumnIndex(TColumn column);
        TColumn GetColumn(int index);
        TColumn GetColumn(TColumn column, int offset = 0);
        bool Contains(TRow row, TColumn column);
        TOut this[TRow row, TColumn column] { get; }
        TOut this[TRow row, TColumn column, int rowOffset = 0, int columnOffset = 0] { get; }
    }

    public interface ILookupTable3D<TRow, TColumn, TDepth, TOut>
    {
        int RowCount { get; }
        int ColumnCount { get; }
        int ColumnCountAtRow(int rowIndex);
        int DepthCount { get; }
        int DepthCountAtRow(int rowIndex, int columnIndex);
        int GetRowIndex(TRow row);
        IList<TRow> Rows { get; }
        TRow GetRow(int index);
        TRow GetRow(TRow row, int offset = 0);
        IList<TColumn> Columns { get; }
        int GetColumnIndex(TColumn column);
        TColumn GetColumn(int index);
        TColumn GetColumn(TColumn column, int offset = 0);
        IList<TDepth> Depths { get; }
        int GetDepthIndex(TDepth column);
        TDepth GetDepth(int index);
        TDepth GetDepth(TDepth depth, int offset = 0);
        bool Contains(TRow row, TColumn column, TDepth depth);
        TOut this[TRow row, TColumn column, TDepth depth] { get; }
        TOut this[TRow row, TColumn column, TDepth depth, int rowOffset = 0, int columnOffset = 0, int depthOffset = 0] { get; }
    }
}