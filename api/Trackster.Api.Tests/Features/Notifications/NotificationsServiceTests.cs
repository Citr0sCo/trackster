using System.Net;
using NUnit.Framework;
using Trackster.Api.Features.Notifications;

namespace HomeBoxLanding.Api.Tests.Features.Notifications;

[TestFixture]
public class NotificationsServiceTests
{
    private const string BaseUrl = "https://api.pushover.net/1/";

    [TestCase]
    public async Task Send_PostsMessageAsUrlEncodedFormData()
    {
        var handler = new CapturingHandler();
        var service = new NotificationsService(new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) });

        await service.Send("Episode 'Who's Marv Marving?' of show '7 & 8 #1' marked as watched.");

        Assert.That(handler.RequestUri, Is.EqualTo(new Uri($"{BaseUrl}messages.json")));
        Assert.That(handler.ContentType, Is.EqualTo("application/x-www-form-urlencoded"));
        Assert.That(ParseFormBody(handler.Body)["message"], Is.EqualTo("Episode 'Who's Marv Marving?' of show '7 & 8 #1' marked as watched."));
    }

    [TestCase("Who's Marv Marving?")]
    [TestCase("Tom & Jerry")]
    [TestCase("Episode #1 = 100%")]
    [TestCase("What? Really!")]
    public async Task Send_PreservesSpecialCharactersInMessage(string episodeTitle)
    {
        var handler = new CapturingHandler();
        var service = new NotificationsService(new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) });
        var message = $"Episode '{episodeTitle}' of show 'Some Show' marked as watched.";

        await service.Send(message);

        Assert.That(ParseFormBody(handler.Body)["message"], Is.EqualTo(message));
    }

    [TestCase]
    public async Task Send_IncludesTokenAndUserInBody()
    {
        var handler = new CapturingHandler();
        var service = new NotificationsService(new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) });

        await service.Send("Message");

        Assert.That(ParseFormBody(handler.Body).Keys, Is.EquivalentTo(new[] { "token", "user", "message" }));
    }

    private static Dictionary<string, string> ParseFormBody(string body)
    {
        return body
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(pair => pair.Split('=', 2))
            .ToDictionary(
                parts => Uri.UnescapeDataString(parts[0]),
                parts => Uri.UnescapeDataString(parts[1].Replace('+', ' ')));
    }

    private sealed class CapturingHandler : HttpMessageHandler
    {
        public Uri? RequestUri { get; private set; }
        public string? ContentType { get; private set; }
        public string Body { get; private set; } = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri;
            ContentType = request.Content?.Headers.ContentType?.MediaType;
            Body = request.Content is null ? string.Empty : await request.Content.ReadAsStringAsync(cancellationToken);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"status\":1}")
            };
        }
    }
}
