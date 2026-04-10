using System.Text.Json;
using GeoComponent.Contracts;
using GeoComponent.Rendering.Scripts;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace GeoComponent.Hosting.Desktop;

/// <summary>
/// WinForms WebView2 adapter that loads packaged <c>host.html</c>, maps static assets to a virtual HTTPS host, and forwards render messages.
/// </summary>
/// <param name="webView">Host WebView2 control.</param>
/// <param name="hostBridge">Message builder for render payloads.</param>
/// <param name="hostOptions">Virtual host configuration.</param>
public sealed class GeoDesktopWebViewAdapter(WebView2 webView, WebViewHostBridge hostBridge, GeoDesktopHostOptions hostOptions) : IDisposable
{
    private readonly WebView2 _webView = webView;
    private readonly WebViewHostBridge _hostBridge = hostBridge;
    private readonly GeoDesktopHostOptions _hostOptions = hostOptions;

    private bool _networkHooksRegistered;
    private bool _hostPageNavigated;
    private bool _hostReady;
    private string? _pendingRenderMessage;

    /// <summary>
    /// Ensures CoreWebView2 is initialized, registers the virtual host mapping, and navigates to <c>host.html</c> when needed.
    /// </summary>
    public async Task EnsureReadyAsync()
    {
        if (_webView.CoreWebView2 is null)
        {
            await _webView.EnsureCoreWebView2Async();
            var core = _webView.CoreWebView2
                       ?? throw new InvalidOperationException("WebView2 initialization failed.");

            core.SetVirtualHostNameToFolderMapping(
                _hostOptions.HostName,
                ResolveDesktopWwwRootPath(),
                CoreWebView2HostResourceAccessKind.Allow);

            RegisterNetworkHooks(core);
            core.WebMessageReceived += OnWebMessageReceived;
            _webView.DefaultBackgroundColor = Color.White;
        }

        if (!_hostPageNavigated)
        {
            NavigateToHostPage();
        }
    }

    /// <summary>
    /// Renders map mode on the hosted page.
    /// </summary>
    public async Task RenderMapAsync(string? geoJson = null, GeoMapOptions? mapOptions = null)
    {
        await EnsureReadyAsync();
        QueueOrSendRenderMessage(_hostBridge.BuildMapRenderMessage(geoJson, mapOptions));
    }

    /// <summary>
    /// Renders editor mode on the hosted page.
    /// </summary>
    public async Task RenderEditorAsync(string? geoJson, GeoMapOptions? mapOptions = null, GeoEditorOptions? editorOptions = null)
    {
        await EnsureReadyAsync();
        QueueOrSendRenderMessage(_hostBridge.BuildEditorRenderMessage(geoJson, mapOptions, editorOptions));
    }

    /// <summary>
    /// Unhooks WebView2 events and network filters registered by this adapter.
    /// </summary>
    public void Dispose()
    {
        if (_webView.CoreWebView2 is not { } core)
            return;

        if (_networkHooksRegistered)
            core.WebResourceRequested -= OnWebResourceRequested;

        core.WebMessageReceived -= OnWebMessageReceived;
    }

    private void NavigateToHostPage()
    {
        var core = _webView.CoreWebView2
                   ?? throw new InvalidOperationException("WebView2 is not ready.");

        _hostReady = false;
        _hostPageNavigated = true;
        core.Navigate(_hostBridge.BuildHostPageUrl(_hostOptions));
    }

    private void QueueOrSendRenderMessage(string messageJson)
    {
        _pendingRenderMessage = messageJson;

        if (_hostReady && _webView.CoreWebView2 is { } core)
            SendPendingRenderMessage(core);
    }

    private void SendPendingRenderMessage(CoreWebView2 core)
    {
        if (!_hostReady || string.IsNullOrWhiteSpace(_pendingRenderMessage))
            return;

        core.PostWebMessageAsJson(_pendingRenderMessage);
        _pendingRenderMessage = null;
    }

    private void RegisterNetworkHooks(CoreWebView2 core)
    {
        if (_networkHooksRegistered)
            return;

        core.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.Image);
        core.WebResourceRequested += OnWebResourceRequested;
        _networkHooksRegistered = true;
    }

    private void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        if (!TryGetMessageType(e.WebMessageAsJson, out var messageType))
            return;

        if (!string.Equals(messageType, ScriptMessageNames.HostReady, StringComparison.OrdinalIgnoreCase))
            return;

        _hostReady = true;

        if (sender is CoreWebView2 core)
            SendPendingRenderMessage(core);
    }

    private void OnWebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
    {
        if (!Uri.TryCreate(e.Request.Uri, UriKind.Absolute, out var uri))
            return;

        if (!IsOpenStreetMapHost(uri.Host))
            return;

        try
        {
            e.Request.Headers.SetHeader("Referer", _hostOptions.VirtualHostReferer);
        }
        catch
        {
            // Ignore requests with immutable headers.
        }
    }

    private static bool TryGetMessageType(string json, out string? messageType)
    {
        messageType = null;

        try
        {
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.ValueKind != JsonValueKind.Object)
                return false;

            if (!doc.RootElement.TryGetProperty("type", out var typeElement))
                return false;

            messageType = typeElement.GetString();
            return !string.IsNullOrWhiteSpace(messageType);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool IsOpenStreetMapHost(string host)
    {
        if (host.Contains("tile.openstreetmap.org", StringComparison.OrdinalIgnoreCase))
            return true;

        return string.Equals(host, "www.openstreetmap.org", StringComparison.OrdinalIgnoreCase);
    }

    private static string ResolveDesktopWwwRootPath()
    {
        var outputRoot = Path.Combine(AppContext.BaseDirectory, "wwwroot");

        if (Directory.Exists(outputRoot))
            return outputRoot;

        throw new DirectoryNotFoundException(
            $"GeoComponent web assets folder was not found at '{outputRoot}'.");
    }
}
