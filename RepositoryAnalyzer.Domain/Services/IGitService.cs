using System;
using RepositoryAnalyzer.Domain.Aggregates;
using RepositoryAnalyzer.Domain.Entities;
using Module = RepositoryAnalyzer.Domain.Entities.Module;

namespace RepositoryAnalyzer.Domain.Services;

public interface IGitService
{
    Task<string> CloneRepository(string url);
    Task<List<Commit>> GetCommitHistory(Repository repository, DateTime since);
    Task<Dictionary<string, Module>> GetFiles(Repository repository);
}
