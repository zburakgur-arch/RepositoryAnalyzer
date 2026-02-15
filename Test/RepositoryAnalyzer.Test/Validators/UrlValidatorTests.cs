using System;
using RepositoryAnalyzer.Domain.Exceptions;
using RepositoryAnalyzer.Domain.Validators;
using Xunit;

namespace RepositoryAnalyzer.Test.Validators;

public class UrlValidatorTests
{
    [Theory]
    [InlineData("https://github.com/owner/repo.git")]
    [InlineData("http://github.com/owner/repo.git")]
    [InlineData("ssh://github.com/owner/repo.git")]
    [InlineData("git@github.com:owner/repo.git")]
    public void Validate_ValidUrls_DoesNotThrow(string url)
    {
        // Act & Assert
        UrlValidator.Validate(url);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid-url")] // Missing scheme
    [InlineData("https://github.com/owner/repo")] // Missing .git
    [InlineData("ftp://github.com/owner/repo.git")] // Unsupported scheme
    [InlineData("git@github.com:owner/repo")] // Missing .git
    public void Validate_InvalidUrls_ThrowsInvalidUrlException(string url)
    {
        // Act & Assert
        Assert.Throws<InvalidUrlException>(() => UrlValidator.Validate(url));
    }
}