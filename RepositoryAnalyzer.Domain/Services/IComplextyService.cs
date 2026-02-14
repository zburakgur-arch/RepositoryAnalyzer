using System;
using RepositoryAnalyzer.Domain.Entities;
using File = RepositoryAnalyzer.Domain.Entities.File;

namespace RepositoryAnalyzer.Domain.Services;

public interface IComplexityService
{
    Task CalculateLineOfCodeComplexity(File file);
    Task CalculateWhitespaceComplexity(File file);
    Task CalculateCyclomaticComplexity(File file);
}
