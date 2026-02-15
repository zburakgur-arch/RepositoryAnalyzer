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

        Repository repository = new Repository(request.Url, localPath, DateTime.UtcNow);

        List<Commit> commits = await _gitService.GetCommitHistory(repository, DateTime.UtcNow.AddMonths(-1));
        Dictionary<string, Module> files = await _gitService.GetFiles(repository);

        repository.SetCommits(commits);
        repository.SetModules(files);

        repository.CalculateLineOfCodeComplexity();
        repository.CalculateFileCurns();

        return null;
    }
}
