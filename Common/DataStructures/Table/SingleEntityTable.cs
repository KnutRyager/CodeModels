using Common.Files;
using Common.Util;

namespace Common.DataStructures
{
    public class SingleEntityTable<TEntity> where TEntity : class, new()
    {
        private TEntity? _entity;
        public TEntity Entity => _entity ??= CalculateEntity();
        public string FilePath { get; }
        private readonly IFileReader _fileReader;
        private readonly bool _transpose;

        public SingleEntityTable(string filePath, IFileReader fileReader, bool transpose = false)
        {
            FilePath = filePath;
            _fileReader = fileReader;
            _transpose = transpose;
        }

        private TEntity CalculateEntity()
        {
            var table = _fileReader.ReadFileToTable(FilePath);
            if (_transpose) table = CollectionUtil.Transpose(table);
            return DataConvert.Table2Type<TEntity>(table)[0];
        }
    }
}