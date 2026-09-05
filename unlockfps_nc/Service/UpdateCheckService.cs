using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace unlockfps_nc.Service;

public sealed record UpdateInfo(Version Version, string Url);

public sealed class UpdateCheckService : IDisposable
{
	private const string ReleasesApiUrl = "https://api.github.com/repos/Genshin-Stella-Mod/Genshin-FPS-Unlocker/releases/latest";
	private readonly HttpClient _httpClient;

	public UpdateCheckService()
	{
		_httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
		_httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Genshin-FPS-Unlocker", Application.ProductVersion));
		_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
	}

	public void Dispose()
	{
		_httpClient.Dispose();
	}

	internal async Task<UpdateInfo?> CheckForUpdateAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			using HttpResponseMessage response = await _httpClient.GetAsync(ReleasesApiUrl, cancellationToken);
			if (!response.IsSuccessStatusCode)
			{
				Program.Logger.Warn($"Update check request failed with status code {(int)response.StatusCode}");
				return null;
			}

			await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
			var release = await JsonSerializer.DeserializeAsync<GitHubRelease>(stream, cancellationToken: cancellationToken);
			if (string.IsNullOrEmpty(release?.TagName)) return null;

			if (!Version.TryParse(release.TagName.TrimStart('v', 'V'), out Version? latestVersion)) return null;

			Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0, 0);
			return latestVersion <= currentVersion ? null : new UpdateInfo(latestVersion, release.HtmlUrl ?? "https://github.com/Genshin-Stella-Mod/Genshin-FPS-Unlocker/releases/latest");
		}
		catch (Exception ex)
		{
			Program.Logger.Warn(ex, "Update check failed");
			return null;
		}
	}

	private sealed class GitHubRelease
	{
		[JsonPropertyName("tag_name")] public string? TagName { get; set; }

		[JsonPropertyName("html_url")] public string? HtmlUrl { get; set; }
	}
}
