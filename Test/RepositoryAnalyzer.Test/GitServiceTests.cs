using System;
using System.IO;
using System.Threading.Tasks;
using LibGit2Sharp;
using Moq;
using RepositoryAnalyzer.Application.Settings;
using RepositoryAnalyzer.Domain.Entities;
using RepositoryAnalyzer.Infrastructure.Services;
using Xunit;

namespace RepositoryAnalyzer.Test
{
    public class GitServiceTests
    {
        private readonly GitSettings _gitSettings;
        private readonly GitService _gitService;

        public GitServiceTests()
        {
            _gitSettings = new GitSettings
            {
                WorkingDirectory = Path.Combine(Path.GetTempPath(), "RepositoryAnalyzerTests")
            };

            _gitService = new GitService(_gitSettings);
        }

        [Fact]
        public async Task CloneRepository_ValidUrl_ClonesRepository()
        {
            // Arrange
            var url = "https://github.com/zburakgur-arch/RepositoryAnalyzer.git";
            var repoName = "RepositoryAnalyzer";
            var expectedLocalPath = Path.Combine(_gitSettings.WorkingDirectory, repoName);

            // Act
            var repository = await _gitService.CloneRepository(url);

            // Assert
            Assert.Equal(url, repository.Id);
            Assert.Equal(expectedLocalPath, repository.LocalPath);
            Assert.True(Directory.Exists(expectedLocalPath));

            // Cleanup
            if (Directory.Exists(expectedLocalPath))
            {
                Directory.Delete(expectedLocalPath, true);
            }
        }

        [Fact]
        public async Task CloneRepository_InvalidUrl_ThrowsArgumentException()
        {
            // Arrange
            var invalidUrl = "https://github.com/example/repo"; // Missing .git

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _gitService.CloneRepository(invalidUrl));
        }

        [Fact]
        public async Task CloneRepository_EmptyUrl_ThrowsArgumentException()
        {
            // Arrange
            var emptyUrl = "";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _gitService.CloneRepository(emptyUrl));
        }

        [Fact]
        public async Task CloneRepository_NonExistentRepo_ThrowsInvalidOperationException()
        {
            // Arrange
            var nonExistentUrl = "https://github.com/example/nonexistent-repo.git";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _gitService.CloneRepository(nonExistentUrl);
            });

            Assert.Contains("Failed to clone repository", exception.Message);
        }
    }
}