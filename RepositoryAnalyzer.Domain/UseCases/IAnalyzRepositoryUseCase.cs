using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryAnalyzer.Domain.Entities;
using RepositoryAnalyzer.Domain.ValueObjects;
using File = RepositoryAnalyzer.Domain.Entities.File;

namespace RepositoryAnalyzer.Domain.UseCases
{
    public interface IAnalyzRepositoryUseCase
    {
        void ValidateUrl(string url);
        Task<Repository> Clone(string url);
        Task<List<Commit>> GetCommitHistory(Repository repository);
        Task<List<File>> GetFiles(Repository repository);
        Task CalculateComplexity(File file);
    }
}
