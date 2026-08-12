using BlazorBasic.Pages.blogs.Random;

namespace Client.Tests.Pages.blogs.Random;

public sealed class RandomIndexTests : BunitContext
{
    [Fact]
    public void RandomIndex_RendersHeading()
    {
        var cut = Render<RandomIndex>();

        cut.Find("h2").TextContent.MarkupMatches("Random Blogs");
    }

    [Fact]
    public void RandomIndex_RendersAllRandomBlogLinks()
    {
        var cut = Render<RandomIndex>();

        var links = cut.FindAll("ul a");
        Assert.Equal(3, links.Count);

        AssertLink(cut, "Panchang/Almanac part 1", "/blogs/random/panchang");
        AssertLink(cut, "Panchang/Almanac part 2", "/blogs/random/panchang2");
        AssertLink(cut, "Test", "/blogs/random/test");
    }

    private static void AssertLink(
        IRenderedComponent<RandomIndex> cut,
        string expectedText,
        string expectedHref)
    {
        var link = cut.FindAll("a").Single(anchor => anchor.TextContent.Trim() == expectedText);

        Assert.Equal(expectedHref, link.GetAttribute("href"));
    }
}
