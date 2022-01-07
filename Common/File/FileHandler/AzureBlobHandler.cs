using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Common.Util;

namespace Common.Files
{
    public class AzureBlobHandler : AbstractFileHandler
    {
        private string ConnectionString { get; set; }
        private BlobServiceClient? _blobServiceClient;
        private BlobServiceClient BlobServiceClient => _blobServiceClient ??= new BlobServiceClient(ConnectionString);

        public AzureBlobHandler(string rootPath, string connectionString)
            : base(rootPath)
        {
            ConnectionString = connectionString;
        }

        public override Task Copy(Stream src, string destinationPath)
        {
            throw new NotImplementedException();
        }

        new Task Copy(string srcPath, string destinationPath)
        {
            throw new NotImplementedException();
        }

        public override Task Write(string path, Stream content)
        {
            WriteBlob(path, content);
            return Task.FromResult(0);
        }

        public new Task Write(string path, string content)
        {
            using var stream = DataConvert.Text2Stream(content);
            var result = Write(path, stream);
            return result;
        }

        private (string ContainerName, string FileName) Path2ContainerFile(string path)
        {
            path = path.Replace("/", "\\");
            var splitPoint = path.IndexOf("\\");
            if (splitPoint < 0) splitPoint = path.Length;
            return (ContainerName: path[..splitPoint].ToLower(), FileName: splitPoint >= path.Length ? "" : FileUtil.Pathify(RootInPath, path[(splitPoint + 1)..]));
        }

        public async Task<Stream> ReadFile(string path, Stream? stream = null)
        {
            var (ContainerName, FileName) = Path2ContainerFile(path);
            stream = await ReadBlob(ContainerName, FileName, stream);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
            //var result = Task.Run(async () => await ReadBlob(paths.ContainerName, paths.FileName, stream));
            //result.Wait();
            //ReadBlob(paths.ContainerName, paths.FileName, stream).Wait();
        }

        public override Stream ReadFile(string path)
        {
            var stream = new MemoryStream();
            return ReadFile(path, stream).Result;
        }

        //public void WriteFile(string path)
        //{
        //    var stream = new MemoryStream();
        //    var paths = Path2ContainerFile(path);
        //    ReadBlob(paths.ContainerName, paths.FileName, stream).Wait();
        //    return stream;
        //}

        //public async Task<string> ReadFileToText(string path)
        //{
        //    using var stream = new MemoryStream();
        //    await ReadFile(path, stream);
        //    return DataConvert.Stream2Text(stream);
        //}

        public override string ReadFileToText(string path)
        {
            using var stream = new MemoryStream();
            var output = ReadFile(path, stream).Result;
            return DataConvert.Stream2Text(output);
        }

        public async Task<string> ReadFileFromAzureAsync(string containerName, string fileName)
        {
            using var stream = new MemoryStream();
            await ReadBlob(containerName.ToLower(), fileName, stream);
            return DataConvert.Stream2Text(stream);
        }

        public async Task<List<T>> ReadFileToTypeFromAzureAsync<T>(string containerName, string fileName, string separator = Constants.DEFAULT_SEPARATOR) where T : new()
            => DataConvert.Text2Type<T>(await ReadFileFromAzureAsync(containerName.ToLower(), fileName), separator);

        public async Task<Stream> ReadBlob(string containerName, string blobName, Stream? outFile = null)
        {
            if (outFile == null) outFile = new MemoryStream();
            try
            {
                var containerClient = BlobServiceClient.GetBlobContainerClient(containerName.ToLower());
                var blobClient = containerClient.GetBlobClient(blobName);
                BlobDownloadInfo download = await blobClient.DownloadAsync();
                outFile.Seek(0, SeekOrigin.Begin);
                await download.Content.CopyToAsync(outFile);
                outFile.Seek(0, SeekOrigin.Begin);
                return outFile;
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching blob: {blobName} from container: {containerName.ToLower()}: {e.Message}");
            }
        }

        public void WriteBlob(string path, Stream file) => WriteBlob(Path2ContainerFile(path).ContainerName.ToLower(), Path2ContainerFile(path).FileName, file);
        public Response<BlobContentInfo> WriteBlob(string containerName, string blobName, Stream file)
        {
            try
            {
                var containerClient = BlobServiceClient.GetBlobContainerClient(containerName.ToLower());
                var blobClient = containerClient.GetBlobClient(blobName);
                file.Seek(0, SeekOrigin.Begin);
                var response = blobClient.Upload(file, overwrite: true);
                return response;
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching blob: {blobName} from container: {containerName.ToLower()}: {e.Message}");
            }
        }

        public async Task<List<string>> ListBlobs(string path, string fileType = "*")
        {
            var (ContainerName, FileName) = Path2ContainerFile(path);
            var containerClient = BlobServiceClient.GetBlobContainerClient(ContainerName);
            var results = new List<string>();
            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                if (blobItem.Name.StartsWith(FileName)) results.Add(blobItem.Name);
            }
            return results.Where(x => fileType.Equals("*") || FileUtil.GetFileExtension(x).Equals(fileType)).ToList();
        }

        public override List<string> GetFiles(string path, string fileType = "*", bool recursively = true, List<string>? results = null) => ListBlobs(path).Result;
    }
}
