using MediatR;
using RepositoryAnalyzer.API.Queries;
using RepositoryAnalyzer.Domain.UseCases;

namespace RepositoryAnalyzer.API.Handlers;

public class AnalyzeRepositoryHandler : IRequestHandler<AnalyzeRepositoryQuery, AnalyzeRepositoryResult>
{
    private readonly IAnalyzRepositoryUseCase _useCase;

    public AnalyzeRepositoryHandler(IAnalyzRepositoryUseCase useCase)
    {
        _useCase = useCase;
    }

    public async Task<AnalyzeRepositoryResult> Handle(AnalyzeRepositoryQuery request, CancellationToken cancellationToken)
    {
        _useCase.ValidateUrl(request.Url);

        var repository = await _useCase.Clone(request.Url);
        var files = await _useCase.GetFiles(repository);
        var commits = await _useCase.GetCommitHistory(repository);

        var fileResults = new List<FileComplexityResult>();

        return null;
    }
}
