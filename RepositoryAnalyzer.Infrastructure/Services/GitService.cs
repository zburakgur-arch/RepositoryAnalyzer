using System;
using LibGit2Sharp;
using RepositoryAnalyzer.Application.Services;
using RepositoryAnalyzer.Application.Settings;
using RepositoryAnalyzer.Domain.Entities;
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

    public Task<List<Commit>> GetCommitHistory(Repository repository, DateTime since)
    {
        throw new NotImplementedException();
    }

    public Task<List<Domain.Entities.File>> GetFiles(Repository repository)
    {
        throw new NotImplementedException();
    }
}
