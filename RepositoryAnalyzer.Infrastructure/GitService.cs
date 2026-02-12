using System;
using System;
using RepositoryAnalyzer.Application.Services;
using RepositoryAnalyzer.Application.Settings;
using RepositoryAnalyzer.Domain.Entities;

namespace RepositoryAnalyzer.Infrastructure;

public class GitService : IGitService
{
    private readonly GitSettings _gitSettings;

    public GitService(GitSettings gitSettings)
    {
        _gitSettings = gitSettings ?? throw new ArgumentNullException(nameof(gitSettings));
    }

    public Task<Repository> CloneRepository(string url)
    {
        throw new NotImplementedException();
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
