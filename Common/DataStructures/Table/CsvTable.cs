using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Files;
using Common.Reflection;

namespace Common.DataStructures
{
    public class CsvTable<TRow, TColumn, TCell>
        : ILookupTable<TRow, TColumn, TCell>
        where TRow : IComparable<TRow>
        where TColumn : IComparable<TColumn>
    {
        public string FilePath { get; }
        private readonly string _name;
        public LookupRangeTableMode RowMode { get; }
        public LookupRangeTableMode ColumnMode { get; }
        private RangeLookupTable<TRow, TColumn, TCell>? _rangeLookupTable;
        public RangeLookupTable<TRow, TColumn, TCell> RangeLookupTable => _rangeLookupTable ??= CalculateTable().Result;
        private readonly IFileReader _fileReader;

        public CsvTable(string name, string filePath, IFileReader fileReader, LookupRangeTableMode rowMode, LookupRangeTableMode columnMode)
        {
            _name = name;
            FilePath = filePath;
            RowMode = rowMode;
            ColumnMode = columnMode;
            _fileReader = fileReader;
        }

        public int RowCount => RangeLookupTable.RowCount;
        public int ColumnCount => RangeLookupTable.ColumnCount;
        public IList<TRow> Rows => RangeLookupTable.Rows;
        public IList<TColumn> Columns => RangeLookupTable.Columns;
        public TCell this[TRow row, TColumn column] => RangeLookupTable[row, column];
        public int ColumnCountAtRow(int rowIndex) => RangeLookupTable.ColumnCountAtRow(rowIndex);
        public int GetRowIndex(TRow row) => RangeLookupTable.GetRowIndex(row);
        public int GetColumnIndex(TColumn column) => RangeLookupTable.GetColumnIndex(column);
        public bool Contains(TRow row, TColumn column) => RangeLookupTable.Contains(row, column);
        public TCell MaybeValue(TRow row, TColumn column) => RangeLookupTable.MaybeValue(row, column);
        public TCell this[TRow row, TColumn column, int rowOffset = 0, int columnOffset = 0] => RangeLookupTable[row, column, rowOffset, columnOffset];
        public TRow GetRow(TRow row, int rowOffset = 0) => RangeLookupTable.GetRow(row, rowOffset);
        public TColumn GetColumn(TColumn column, int columnOffset = 0) => RangeLookupTable.GetColumn(column, columnOffset);
        public TRow GetRow(int index) => RangeLookupTable.GetRow(index);
        public TColumn GetColumn(int index) => RangeLookupTable.GetColumn(index);

        private async Task<RangeLookupTable<TRow, TColumn, TCell>> CalculateTable()
        {
            var table = _fileReader.ReadFileToTable(FilePath);
            var column = table[0].Skip(1).Select(ReflectionUtil.ConvertTo<TColumn>).ToList();
            var row = table.Select(x => x[0]).Skip(1).Select(ReflectionUtil.ConvertTo<TRow>).ToList();
            var cells = table.Skip(1).Select(x => (IList<TCell>)x.Skip(1).Select(ReflectionUtil.ConvertTo<TCell>).ToList()).ToList();
            return await Task.FromResult(new RangeLookupTable<TRow, TColumn, TCell>(_name, row, column, cells, RowMode, ColumnMode));
        }
    }
}