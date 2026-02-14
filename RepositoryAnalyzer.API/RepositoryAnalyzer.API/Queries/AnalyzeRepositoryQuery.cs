using MediatR;

namespace RepositoryAnalyzer.API.Queries;

public record AnalyzeRepositoryQuery(string Url) : IRequest<AnalyzeRepositoryResult>;

public record FileComplexityResult(string FilePath, int LineOfCodes, int AddedLines, int DeletedLines);

public record AnalyzeRepositoryResult(
    string Name,
    string CreatedDate,
    string LastUpdatedDate,
    List<FileComplexityResult> Files);
