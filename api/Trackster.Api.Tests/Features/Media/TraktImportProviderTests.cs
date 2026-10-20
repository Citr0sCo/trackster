using System.Net;
using System.Text;
using NUnit.Framework;
using Trackster.Api.Features.Media.Importers.TraktImporter;

namespace HomeBoxLanding.Api.Tests.Features.Media;

[TestFixture]
public class TraktImportProviderTests
{
    private const string CloudflarePage = "<!DOCTYPE html><html><head><title>Attention Required! | Cloudflare</title>" +
        "<script>window._cf_chl_opt={cvId:'3',cZone:'api.trakt.tv',cType:'managed'};</script>" +
        "<script>function _cf_chl_enter(){document.querySelector('form').submit();}</script></head><body>blocked</body></html>";

    [Test]
    public void DescribeBody_DoesNotEchoMarkupOrInlineJavaScript()
    {
        using var response = BuildResponse(CloudflarePage, "text/html");

        var description = TraktImportProvider.DescribeBody(response, CloudflarePage);

        Assert.That(description, Does.Not.Contain("<"));
        Assert.That(description, Does.Not.Contain("script"));
        Assert.That(description, Does.Not.Contain("_cf_chl_enter"));
        Assert.That(description, Does.Contain("text/html"));
        Assert.That(description, Does.Contain(CloudflarePage.Length.ToString("N0")));
    }

    [Test]
    public void DescribeBody_TreatsMarkupWithoutAContentTypeAsMarkup()
    {
        using var response = BuildResponse(CloudflarePage, null);

        var description = TraktImportProvider.DescribeBody(response, CloudflarePage);

        Assert.That(description, Does.Not.Contain("<script"));
        Assert.That(description, Does.Contain("instead of JSON"));
    }

    [Test]
    public void DescribeBody_KeepsShortJsonErrorsQuoteForQuote()
    {
        const string body = "{\"error\":\"invalid_client\"}";

        using var response = BuildResponse(body, "application/json");

        var description = TraktImportProvider.DescribeBody(response, body);

        Assert.That(description, Is.EqualTo($"\"{body}\""));
    }

    [Test]
    public void DescribeBody_CollapsesWhitespaceAndTruncatesLongBodies()
    {
        var body = string.Join('\n', Enumerable.Repeat("detail", 200));

        using var response = BuildResponse(body, "text/plain");

        var description = TraktImportProvider.DescribeBody(response, body);

        Assert.That(description, Does.Not.Contain("\n"));
        Assert.That(description, Does.EndWith("…\""));
        Assert.That(description.Length, Is.LessThan(230));
    }

    [TestCase("")]
    [TestCase("   \r\n ")]
    public void DescribeBody_ReportsAnEmptyBody(string body)
    {
        using var response = BuildResponse(body, "text/plain");

        Assert.That(TraktImportProvider.DescribeBody(response, body), Is.EqualTo("an empty response body"));
    }

    private static HttpResponseMessage BuildResponse(string body, string? contentType)
    {
        var content = new StringContent(body, Encoding.UTF8);
        content.Headers.ContentType = contentType == null
            ? null
            : new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

        return new HttpResponseMessage(HttpStatusCode.Forbidden) { Content = content };
    }
}
