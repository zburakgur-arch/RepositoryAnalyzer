using RepositoryAnalyzer.Domain.Services;
using File = RepositoryAnalyzer.Domain.Entities.File;

namespace RepositoryAnalyzer.Application.Services;

public class ComplexityService : IComplexityService
{
    public Task CalculateCyclomaticComplexity(File file)
    {
        throw new NotImplementedException();
    }

    public Task CalculateLineOfCodeComplexity(File file)
    {
        if (file == null)
            throw new ArgumentNullException(nameof(file));

        if (!System.IO.File.Exists(file.Id))
            throw new FileNotFoundException("Dosya bulunamadı.", file.Id);

        // Dosyanın satır sayısını hesapla
        int lineCount = System.IO.File.ReadAllLines(file.Id).Length;

        // Complexity'nin LineOfCodes değerini set et
        file.SetLineOfCodes(lineCount);

        return Task.CompletedTask;
    }

    public Task CalculateWhitespaceComplexity(File file)
    {
        if (file == null)
            throw new ArgumentNullException(nameof(file));

        if (!System.IO.File.Exists(file.Id))
            throw new FileNotFoundException("Dosya bulunamadı.", file.Id);

        var lines = System.IO.File.ReadAllLines(file.Id);
        int totalIndentation = 0;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            int leadingSpaces = line.Length - line.TrimStart().Length;
            totalIndentation += leadingSpaces;
        }

        file.SetWhiteSpace(totalIndentation);

        return Task.CompletedTask;
    }
}
