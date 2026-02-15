using System;
using System.Text.RegularExpressions;
using RepositoryAnalyzer.Domain.Exceptions;

namespace RepositoryAnalyzer.Domain.Validators;

public static class UrlValidator
{
    public static void Validate(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new InvalidUrlException();

        if (!IsValidGitUrl(url))
            throw new InvalidUrlException();
    }

    private static bool IsValidGitUrl(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            if (uri.Scheme is not ("http" or "https" or "ssh" or "git"))
                return false;

            if (string.IsNullOrWhiteSpace(uri.Host))
                return false;

            return uri.AbsolutePath.EndsWith(".git", StringComparison.OrdinalIgnoreCase);
        }

        // SCP-like syntax: git@github.com:owner/repo.git
        return Regex.IsMatch(
            url,
            @"^git@[^:]+:[^/]+/.+\.git$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }
}