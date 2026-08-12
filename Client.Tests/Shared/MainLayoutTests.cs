using BlazorBasic.Shared;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Tests.Shared;

public sealed class MainLayoutTests : BunitContext
{
    [Fact]
    public void RendersExpectedNavigationLinks()
    {
        var layout = RenderMainLayout();

        var links = layout.FindAll("header .nav-links a")
            .Select(link => (Text: link.TextContent, Href: link.GetAttribute("href")))
            .ToArray();

        Assert.Equal(
            [
                ("Home", "/"),
                ("About", "/#about"),
                ("Projects", "/#projects"),
                ("Resume", "/#resume"),
                ("Contact", "/contact"),
                ("Blogs", "/blogs"),
                ("Travel", "/blogs/travel"),
                ("Technology", "/blogs/technology"),
                ("Philosophy", "/blogs/philosophy"),
                ("Random", "/blogs/random")
            ],
            links);
    }

    [Fact]
    public void RendersBodyWithoutBlogContainerOnHomePage()
    {
        NavigateTo("/");

        var layout = RenderMainLayout();

        Assert.Empty(layout.FindAll(".blog-container"));
        Assert.NotNull(layout.Find("[data-testid='layout-body']"));
    }

    [Fact]
    public void WrapsBodyInBlogContainerOnNonHomePage()
    {
        NavigateTo("/contact");

        var layout = RenderMainLayout();

        var container = layout.Find(".blog-container");

        Assert.NotNull(container.QuerySelector("[data-testid='layout-body']"));
    }

    [Fact]
    public void MobileMenuToggleUpdatesExpandedStateAndNavClass()
    {
        var layout = RenderMainLayout();
        var toggle = layout.Find("button.menu-toggle");
        var navLinks = layout.Find("ul.nav-links");

        Assert.Null(toggle.GetAttribute("aria-expanded"));
        Assert.DoesNotContain("is-open", navLinks.ClassList);

        toggle.Click();

        Assert.NotNull(layout.Find("button.menu-toggle").GetAttribute("aria-expanded"));
        Assert.Contains("is-open", layout.Find("ul.nav-links").ClassList);

        layout.Find("header .nav-links a[href='/contact']").Click();

        Assert.Null(layout.Find("button.menu-toggle").GetAttribute("aria-expanded"));
        Assert.DoesNotContain("is-open", layout.Find("ul.nav-links").ClassList);
    }

    private IRenderedComponent<MainLayout> RenderMainLayout()
    {
        return Render<MainLayout>(parameters => parameters
            .Add(component => component.Body, RenderBody));
    }

    private static void RenderBody(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "main");
        builder.AddAttribute(1, "data-testid", "layout-body");
        builder.AddContent(2, "Layout body");
        builder.CloseElement();
    }

    private void NavigateTo(string uri)
    {
        Services.GetRequiredService<BunitNavigationManager>().NavigateTo(uri);
    }
}
