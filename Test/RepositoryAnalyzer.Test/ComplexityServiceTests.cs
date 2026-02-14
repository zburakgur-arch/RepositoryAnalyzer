using System;
using System.IO;
using System.Threading.Tasks;
using RepositoryAnalyzer.Application.Services;
using Xunit;
using File = RepositoryAnalyzer.Domain.Entities.File;

namespace RepositoryAnalyzer.Test
{
    public class ComplexityServiceTests : IDisposable
    {
        private readonly ComplexityService _complexityService;
        private readonly string _tempDir;

        public ComplexityServiceTests()
        {
            _complexityService = new ComplexityService();
            _tempDir = Path.Combine(Path.GetTempPath(), "ComplexityServiceTests_" + Guid.NewGuid());
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, true);
        }

        private string CreateTempFile(string content)
        {
            var filePath = Path.Combine(_tempDir, Guid.NewGuid() + ".cs");
            System.IO.File.WriteAllText(filePath, content);
            return filePath;
        }

        #region CalculateLineOfCodeComplexity

        [Fact]
        public async Task CalculateLineOfCodeComplexity_NullFile_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _complexityService.CalculateLineOfCodeComplexity(null!));
        }

        [Fact]
        public async Task CalculateLineOfCodeComplexity_FileNotExists_ThrowsFileNotFoundException()
        {
            var file = new File { Id = "/nonexistent/path/file.cs" };

            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                _complexityService.CalculateLineOfCodeComplexity(file));
        }

        [Fact]
        public async Task CalculateLineOfCodeComplexity_SingleLine_Returns1()
        {
            var path = CreateTempFile("Console.WriteLine(\"Hello\");");
            var file = new File { Id = path };

            await _complexityService.CalculateLineOfCodeComplexity(file);

            Assert.Equal(1, file.GetLineOfCodes());
        }

        [Fact]
        public async Task CalculateLineOfCodeComplexity_MultipleLines_ReturnsCorrectCount()
        {
            var content = "using System;\n\nnamespace Test\n{\n    class A\n    {\n    }\n}\n";
            var path = CreateTempFile(content);
            var file = new File { Id = path };

            await _complexityService.CalculateLineOfCodeComplexity(file);

            Assert.Equal(8, file.GetLineOfCodes());
        }

        [Fact]
        public async Task CalculateLineOfCodeComplexity_EmptyFile_Returns0()
        {
            var path = CreateTempFile("");
            var file = new File { Id = path };

            await _complexityService.CalculateLineOfCodeComplexity(file);

            Assert.Equal(0, file.GetLineOfCodes());
        }

        #endregion

        #region CalculateWhitespaceComplexity

        [Fact]
        public async Task CalculateWhitespaceComplexity_NullFile_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _complexityService.CalculateWhitespaceComplexity(null!));
        }

        [Fact]
        public async Task CalculateWhitespaceComplexity_FileNotExists_ThrowsFileNotFoundException()
        {
            var file = new File { Id = "/nonexistent/path/file.cs" };

            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                _complexityService.CalculateWhitespaceComplexity(file));
        }

        [Fact]
        public async Task CalculateWhitespaceComplexity_NoIndentation_ReturnsZero()
        {
            var content = "using System;\nclass A\n{\n}\n";
            var path = CreateTempFile(content);
            var file = new File { Id = path };

            await _complexityService.CalculateWhitespaceComplexity(file);

            Assert.Equal(0, file.GetWhiteSpace());
        }

        [Fact]
        public async Task CalculateWhitespaceComplexity_WithIndentation_ReturnsTotalLeadingSpaces()
        {
            // 4 boşluk + 8 boşluk = 12
            var content = "class A\n{\n    int x;\n        int y;\n}\n";
            var path = CreateTempFile(content);
            var file = new File { Id = path };

            await _complexityService.CalculateWhitespaceComplexity(file);

            Assert.Equal(12, file.GetWhiteSpace());
        }

        [Fact]
        public async Task CalculateWhitespaceComplexity_BlankLinesIgnored()
        {
            // Boş satırlar sayılmamalı
            var content = "class A\n\n    int x;\n\n";
            var path = CreateTempFile(content);
            var file = new File { Id = path };

            await _complexityService.CalculateWhitespaceComplexity(file);

            Assert.Equal(4, file.GetWhiteSpace());
        }

        [Fact]
        public async Task CalculateWhitespaceComplexity_EmptyFile_ReturnsZero()
        {
            var path = CreateTempFile("");
            var file = new File { Id = path };

            await _complexityService.CalculateWhitespaceComplexity(file);

            Assert.Equal(0, file.GetWhiteSpace());
        }

        [Fact]
        public async Task CalculateWhitespaceComplexity_DeeplyNested_ReturnsHighValue()
        {
            // Derin iç içe geçmiş kod — yüksek whitespace complexity
            var content =
                "class A\n" +
                "{\n" +
                "    void M()\n" +          // 4
                "    {\n" +                 // 4
                "        if (true)\n" +     // 8
                "        {\n" +             // 8
                "            x++;\n" +      // 12
                "        }\n" +             // 8
                "    }\n" +                 // 4
                "}\n";
            var path = CreateTempFile(content);
            var file = new File { Id = path };

            await _complexityService.CalculateWhitespaceComplexity(file);

            Assert.Equal(48, file.GetWhiteSpace());
        }

        #endregion
    }
}
