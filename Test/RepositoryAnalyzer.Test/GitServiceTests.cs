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
using Repository = RepositoryAnalyzer.Domain.Entities.Repository;

using File = RepositoryAnalyzer.Domain.Entities.File;

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

        [Fact]
        public async Task GetCommitHistory_NullRepository_ThrowsArgumentNullException()
        {
            // Arrange
            Repository repository = null;
            var since = DateTime.UtcNow.AddDays(-7);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _gitService.GetCommitHistory(repository, since));
        }

        [Fact]
        public async Task GetCommitHistory_InvalidLocalPath_ThrowsArgumentException()
        {
            // Arrange
            var repository = new Repository
            {
                Id = "https://github.com/example/repo.git",
                LocalPath = "/nonexistent/path",
                ClonedAt = DateTime.UtcNow
            };
            var since = DateTime.UtcNow.AddDays(-7);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _gitService.GetCommitHistory(repository, since));
        }

        [Fact]
        public async Task GetCommitHistory_ValidRepository_ReturnsCommitList()
        {
            // Arrange - Önce gerçek bir repo clone et
            var url = "https://github.com/zburakgur-arch/RepositoryAnalyzer.git";
            var repository = await _gitService.CloneRepository(url);
            var since = DateTime.UtcNow.AddMonths(-6);

            // Act
            var commits = await _gitService.GetCommitHistory(repository, since);

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
            if (Directory.Exists(repository.LocalPath))
            {
                Directory.Delete(repository.LocalPath, true);
            }
        }

        [Fact]
        public async Task GetFiles_NullRepository_ThrowsArgumentNullException()
        {
            // Arrange
            Repository repository = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _gitService.GetFiles(repository));
        }

        [Fact]
        public async Task GetFiles_InvalidLocalPath_ThrowsArgumentException()
        {
            // Arrange
            var repository = new Repository
            {
                Id = "https://github.com/example/repo.git",
                LocalPath = "/nonexistent/path",
                ClonedAt = DateTime.UtcNow
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _gitService.GetFiles(repository));
        }

        [Fact]
        public async Task GetFiles_ValidRepository_ReturnsDictionaryWithFiles()
        {
            // Arrange - Önce gerçek bir repo clone et
            var url = "https://github.com/zburakgur-arch/RepositoryAnalyzer.git";
            var repository = await _gitService.CloneRepository(url);

            // Act
            var files = await _gitService.GetFiles(repository);

            // Assert
            Assert.NotNull(files);
            Assert.IsType<Dictionary<string, File>>(files);
            Assert.True(files.Count > 0);

            // Her dosyanın key'i ile File.Id'si eşleşmeli
            foreach (var kvp in files)
            {
                Assert.Equal(kvp.Key, kvp.Value.Id);
            }

            // Cleanup
            if (Directory.Exists(repository.LocalPath))
            {
                Directory.Delete(repository.LocalPath, true);
            }
        }
    }
}