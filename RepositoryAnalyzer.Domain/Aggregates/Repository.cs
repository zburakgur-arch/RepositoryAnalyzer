using RepositoryAnalyzer.Domain.Aggregates;
using RepositoryAnalyzer.Domain.Entities;

namespace RepositoryAnalyzer.Domain.Aggregates;

public class Repository : IAggregate<string>
{
    public string Id { get; }  // URL
    public string Name { get; }
    public string LocalPath { get; }
    public DateTime ClonedAt { get; }
    private Dictionary<string, Module> Modules = new();
    private List<Commit> Commits = new();

    public Repository(string id, string localPath, DateTime clonedAt, Dictionary<string, Module> modules, List<Commit> commits)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        LocalPath = localPath ?? throw new ArgumentNullException(nameof(localPath));
        ClonedAt = clonedAt;
        Name = Id.Split('/').Last().Replace(".git", "");
        Modules = modules ?? throw new ArgumentNullException(nameof(modules));
        Commits = commits ?? throw new ArgumentNullException(nameof(commits));
    }

    public void CalculateLineOfCodeComplexity()
    {
        foreach (var module in Modules.Values)
        {
            if (!System.IO.File.Exists(module.Id))
                throw new FileNotFoundException("Dosya bulunamadı.", module.Id);

            int lineCount = System.IO.File.ReadAllLines(module.Id).Length;
            module.SetLineOfCodes(lineCount);
        }
    }

    public void CalculateWhitespaceComplexity()
    {
        foreach (var module in Modules.Values)
        {
            if (!System.IO.File.Exists(module.Id))
                throw new FileNotFoundException("Dosya bulunamadı.", module.Id);

            var lines = System.IO.File.ReadAllLines(module.Id);
            int totalIndentation = 0;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                int leadingSpaces = line.Length - line.TrimStart().Length;
                totalIndentation += leadingSpaces;
            }

            module.SetWhiteSpace(totalIndentation);
        }
    }

    public void CalculateCyclomaticComplexity()
    {
        foreach (var module in Modules.Values)
        {
            // Cyclomatic complexity calculation logic goes here
            throw new NotImplementedException();
        }
    }

    public void CalculateFileCurns()
    {
        foreach (var commit in Commits)
        {
            foreach (var change in commit.Changes)
            {
                if (!Modules.TryGetValue(change.FilePath, out var module))
                {
                    // Skip if no matching module is found
                    continue;
                }

                // Logic to calculate file churns for the matched module
                throw new NotImplementedException();
            }
        }
    }
}
