using System;
using System.IO;
using System.Threading.Tasks;
using LibGit2Sharp;
using Moq;
using RepositoryAnalyzer.Application.Settings;
using RepositoryAnalyzer.Domain.Entities;
using RepositoryAnalyzer.Infrastructure.Services;
using Xunit;
using Commit = RepositoryAnalyzer.Domain.Entities.Commit;
using Repository = RepositoryAnalyzer.Domain.Aggregates.Repository;

using Module = RepositoryAnalyzer.Domain.Entities.Module;
using RepositoryAnalyzer.Domain.Exceptions;

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
            var localPath = await _gitService.CloneRepository(url);

            // Assert
            Assert.Equal(expectedLocalPath, localPath);
            Assert.True(Directory.Exists(localPath));

            // Cleanup
            if (Directory.Exists(localPath))
            {
                Directory.Delete(localPath, true);
            }
        }

        [Fact]
        public async Task CloneRepository_InvalidUrl_ThrowsArgumentException()
        {
            // Arrange
            var invalidUrl = "https://github.com/example/repo"; // Missing .git

            // Act & Assert
            await Assert.ThrowsAsync<InvalidUrlException>(() => _gitService.CloneRepository(invalidUrl));
        }

        [Fact]
        public async Task CloneRepository_EmptyUrl_ThrowsArgumentException()
        {
            // Arrange
            var emptyUrl = "";

            // Act & Assert
            await Assert.ThrowsAsync<InvalidUrlException>(() => _gitService.CloneRepository(emptyUrl));
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

        [Fact]
        public async Task GetCommitHistory_NullLocalPath_ThrowsArgumentNullException()
        {
            // Arrange
            string localPath = null!; // Explicitly mark as nullable
            var since = DateTime.UtcNow.AddDays(-7);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _gitService.GetCommitHistory(localPath, since));
        }

        [Fact]
        public async Task GetCommitHistory_InvalidLocalPath_ThrowsArgumentException()
        {
            // Arrange
            var localPath = "/nonexistent/path";
            var since = DateTime.UtcNow.AddDays(-7);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _gitService.GetCommitHistory(localPath, since));
        }

        [Fact]
        public async Task GetCommitHistory_ValidLocalPath_ReturnsCommitList()
        {
            // Arrange - Clone a real repository first
            var url = "https://github.com/zburakgur-arch/RepositoryAnalyzer.git";
            var localPath = await _gitService.CloneRepository(url);
            var since = DateTime.UtcNow.AddMonths(-6);

            // Act
            var commits = await _gitService.GetCommitHistory(localPath, since);

            // Assert
            Assert.NotNull(commits);
            Assert.IsType<List<Commit>>(commits);

            if (commits.Count > 0)
            {
                var firstCommit = commits.First();
                Assert.NotNull(firstCommit.Id);
                Assert.NotNull(firstCommit.Author);
                Assert.NotNull(firstCommit.Author.Id);
                Assert.NotNull(firstCommit.Changes);
                Assert.True(firstCommit.Date >= since);
            }

            // Cleanup
            if (Directory.Exists(localPath))
            {
                Directory.Delete(localPath, true);
            }
        }

        [Fact]
        public async Task GetFiles_NullLocalPath_ThrowsArgumentException()
        {
            // Arrange
            string localPath = null!; // Explicitly mark as nullable

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _gitService.GetFiles(localPath));
        }

        [Fact]
        public async Task GetFiles_InvalidLocalPath_ThrowsArgumentException()
        {
            // Arrange
            var localPath = "/nonexistent/path";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _gitService.GetFiles(localPath));
        }

        [Fact]
        public async Task GetFiles_ValidLocalPath_ReturnsDictionaryWithFiles()
        {
            // Arrange - Clone a real repository first
            var url = "https://github.com/zburakgur-arch/RepositoryAnalyzer.git";
            var localPath = await _gitService.CloneRepository(url);

            // Act
            var files = await _gitService.GetFiles(localPath);

            // Assert
            Assert.NotNull(files);
            Assert.IsType<Dictionary<string, Module>>(files);
            Assert.True(files.Count > 0);

            // Each file's key should match File.Id
            foreach (var kvp in files)
            {
                Assert.Equal(kvp.Key, kvp.Value.Id);
            }

            // Cleanup
            if (Directory.Exists(localPath))
            {
                Directory.Delete(localPath, true);
            }
        }
    }
}