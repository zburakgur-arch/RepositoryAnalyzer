using System;
using RepositoryAnalyzer.Domain.Aggregates;
using RepositoryAnalyzer.Domain.Entities;
using Module = RepositoryAnalyzer.Domain.Entities.Module;

namespace RepositoryAnalyzer.Domain.Services;

public interface IGitService
{
    Task<string> CloneRepository(string url);
    Task<List<Commit>> GetCommitHistory(string localPath, DateTime since);
    Task<Dictionary<string, Module>> GetFiles(string localPath);
}
