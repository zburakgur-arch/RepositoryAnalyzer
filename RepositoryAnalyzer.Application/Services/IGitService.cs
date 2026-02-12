using System;
using RepositoryAnalyzer.Domain.Entities;
using File = RepositoryAnalyzer.Domain.Entities.File;

namespace RepositoryAnalyzer.Application.Services;

public interface IGitService
{
    Task<Repository> CloneRepository(string url);
    Task<List<Commit>> GetCommitHistory(Repository repository, DateTime since);
    Task<List<File>> GetFiles(Repository repository);
}
