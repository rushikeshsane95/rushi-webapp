using BlazorBasic;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Tests;

public sealed class AppTests : BunitContext
{
    public AppTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void App_RendersHomeRoute()
    {
        var component = Render<App>();

        Assert.Equal("http://localhost/", Services.GetRequiredService<NavigationManager>().Uri);
        Assert.Contains("Rushikesh Sane", component.Markup);
        Assert.Empty(component.FindAll("p[role='alert']"));
    }

    [Fact]
    public void App_RendersNotFoundContentForUnknownRoute()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("/missing-page");

        var component = Render<App>();

        var alert = component.Find("p[role='alert']");
        Assert.Equal("Sorry, there's nothing at this address.", alert.TextContent);
        Assert.NotEmpty(component.FindAll("header.main-nav"));
    }
}
