using MediatR;
using RepositoryAnalyzer.API.Queries;
using RepositoryAnalyzer.Domain.Aggregates;
using RepositoryAnalyzer.Domain.Entities;
using RepositoryAnalyzer.Domain.Services;
using RepositoryAnalyzer.Domain.Validators;

namespace RepositoryAnalyzer.API.Handlers;

public class AnalyzeRepositoryHandler : IRequestHandler<AnalyzeRepositoryQuery, AnalyzeRepositoryResult>
{
    private readonly IGitService _gitService;

    public AnalyzeRepositoryHandler(IGitService gitService)
    {
        _gitService = gitService ?? throw new ArgumentNullException(nameof(gitService));
    }

    public async Task<AnalyzeRepositoryResult> Handle(AnalyzeRepositoryQuery request, CancellationToken cancellationToken)
    {
        UrlValidator.Validate(request.Url);
        string localPath = await _gitService.CloneRepository(request.Url);

        Dictionary<string, Module> files = await _gitService.GetFiles(localPath);
        List<Commit> commits = await _gitService.GetCommitHistory(localPath, DateTime.UtcNow.AddMonths(-1));

        Repository repository = new Repository(request.Url, localPath, DateTime.UtcNow, files, commits);

        repository.CalculateLineOfCodeComplexity();
        repository.CalculateFileCurns();

        return null; // TODO: Map Repository to AnalyzeRepositoryResult
    }
}
