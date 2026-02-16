using System;

namespace RepositoryAnalyzer.Domain.ValueObjects;

public class Churn
{
    public int AddedLines { get; set; }
    public int DeletedLines { get; set; }

    public Churn(int addedLines, int deletedLines)
    {
        AddedLines = addedLines;
        DeletedLines = deletedLines;
    }

    public int TotalChurn => AddedLines + DeletedLines;
}
