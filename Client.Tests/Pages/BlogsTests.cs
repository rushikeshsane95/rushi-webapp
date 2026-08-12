using BlazorBasic.Pages;

namespace Client.Tests.Pages;

public sealed class BlogsTests : BunitContext
{
    [Fact]
    public void RendersPageTitleAndHeader()
    {
        var cut = Render<Blogs>();

        cut.Find("h1").TextContent.MarkupMatches("Browse all content");
        Assert.Contains("Jump to any page or blog entry.", cut.Markup);
    }

    [Fact]
    public void RendersRepresentativePageAndBlogRouteLinks()
    {
        var cut = Render<Blogs>();

        var links = cut.FindAll("a")
            .Select(anchor => anchor.GetAttribute("href"))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Assert.Contains("/", links);
        Assert.Contains("/contact", links);
        Assert.Contains("/blogs/technology", links);
        Assert.Contains("/blogs/technology/ai/codex-best-practices-dotnet", links);
    }
}
