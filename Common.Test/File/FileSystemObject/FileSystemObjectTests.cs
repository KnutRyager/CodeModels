using Common.Files;
using FluentAssertions;
using Xunit;

namespace Common.Test.Files
{
    public class FileSystemObjectTests
    {
        [Fact]
        public void SaveFolderOfFilesThenRead()
        {
            var folder = Folder.Create("testSaveFolder",
                Folder.Create("a", new FileObject("file1", "test1", "txt")),
                new FileObject("file2", "test2", "txt")
            );
            folder.Save("temp/test");

            var file1Read = File.ReadAllText("temp/test/testSaveFolder/a/file1.txt");
            file1Read.Should().Be("test1");

            var folderWithEmptyFiles = Folder.Create("testSaveFolder",
                Folder.Create("a", new FileObject("file1", "", "txt")),
                new FileObject("file2", "", "txt")
            );
            var folderRead = folderWithEmptyFiles.Read("temp/test");

            folderRead.Should().BeEquivalentTo(folder);
        }
    }
}
