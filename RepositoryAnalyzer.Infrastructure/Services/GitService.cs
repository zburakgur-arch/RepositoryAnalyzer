using System;
using LibGit2Sharp;
using RepositoryAnalyzer.Application.Services;
using RepositoryAnalyzer.Application.Settings;
using RepositoryAnalyzer.Domain.Entities;
using RepositoryAnalyzer.Domain.ValueObjects;
using Commit = RepositoryAnalyzer.Domain.Entities.Commit;
using Repository = RepositoryAnalyzer.Domain.Entities.Repository;
using System.IO;

namespace RepositoryAnalyzer.Infrastructure.Services;

public class GitService : IGitService
{
    private readonly GitSettings _gitSettings;

    public GitService(GitSettings gitSettings)
    {
        _gitSettings = gitSettings ?? throw new ArgumentNullException(nameof(gitSettings));
    }

    public async Task<Repository> CloneRepository(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be null or empty", nameof(url));

        if (!url.EndsWith(".git"))
            throw new ArgumentException("URL must end with .git", nameof(url));

        // URL'den repo ismi çıkar
        var repoName = url.TrimEnd('/').Split('/').Last().Replace(".git", "");
        var localPath = Path.Combine(_gitSettings.WorkingDirectory, repoName);

        // Eğer dizin varsa, önce sil
        if (Directory.Exists(localPath))
        {
            Directory.Delete(localPath, true);
        }

        var options = new CloneOptions(
            new FetchOptions
            {
                CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                {
                    Username = "",
                    Password = ""
                }
            }
        );
         
        try
        {

            await Task.Run(() => LibGit2Sharp.Repository.Clone(url, localPath, options));
        }
        catch (LibGit2SharpException ex)
        {
            throw new InvalidOperationException($"Failed to clone repository from URL: {url}", ex);
        }

        return new Repository
        {
            Id = url,
            LocalPath = localPath,
            ClonedAt = DateTime.UtcNow
        };
    }

    public async Task<List<Commit>> GetCommitHistory(Repository repository, DateTime since)
    {
        if (repository == null)
            throw new ArgumentNullException(nameof(repository));

        if (string.IsNullOrWhiteSpace(repository.LocalPath) || !Directory.Exists(repository.LocalPath))
            throw new ArgumentException("Repository local path is invalid or does not exist", nameof(repository));

        var commits = new List<Commit>();

        using var repo = new LibGit2Sharp.Repository(repository.LocalPath);

        // --all: Tüm branch'lerdeki commit'leri topla
        var allCommits = repo.Commits.QueryBy(new CommitFilter
        {
            IncludeReachableFrom = repo.Refs,
            SortBy = CommitSortStrategies.Time
        });

        var compareOptions = new CompareOptions
        {
            // --no-renames
            Similarity = SimilarityOptions.None
        };

        foreach (var gitCommit in allCommits)
        {
            // --since filtresi
            if (gitCommit.Author.When.DateTime < since)
                continue;

            var commit = new Commit
            {
                // %H
                Id = gitCommit.Sha,
                // %ad (author date)
                Date = gitCommit.Author.When.DateTime,
                // %an (author name)
                Author = new Developer { Id = gitCommit.Author.Name },
                Changes = new List<Change>()
            };

            // --numstat: Her dosya için eklenen/silinen satır sayısı
            Tree parentTree = gitCommit.Parents.Any()
                ? gitCommit.Parents.First().Tree
                : null;

            var diff = repo.Diff.Compare<Patch>(parentTree, gitCommit.Tree, compareOptions);

            foreach (var entry in diff)
            {
                commit.Changes.Add(new Change
                {
                    FilePath = entry.Path,
                    AddedLines = entry.LinesAdded,
                    DeletedLines = entry.LinesDeleted
                });
            }

            commits.Add(commit);
        }

        return commits;
    }

    public Task<List<Domain.Entities.File>> GetFiles(Repository repository)
    {
        throw new NotImplementedException();
    }
}
