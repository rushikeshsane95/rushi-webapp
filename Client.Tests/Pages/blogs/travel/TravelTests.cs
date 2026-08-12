using BlazorBasic.Pages.blogs.travel;

namespace Client.Tests.Pages.blogs.travel;

public sealed class TravelTests : BunitContext
{
    [Fact]
    public void Travel_RendersHeading()
    {
        var cut = Render<Travel>();

        cut.Find("h2").TextContent.MarkupMatches("Travel Blogs");
    }

    [Fact]
    public void Travel_RendersAllTravelBlogLinks()
    {
        var cut = Render<Travel>();

        var links = cut.FindAll("ul a");
        Assert.Equal(3, links.Count);

        AssertLink(cut, "Chennai Trip", "/blogs/travel/chennai");
        AssertLink(cut, "Jotunheim", "/blogs/travel/jotunheim");
        AssertLink(cut, "Travel Blog 3", "/blogs/travel/travelblog3");
    }

    private static void AssertLink(
        IRenderedComponent<Travel> cut,
        string expectedText,
        string expectedHref)
    {
        var link = cut.FindAll("a").Single(anchor => anchor.TextContent.Trim() == expectedText);

        Assert.Equal(expectedHref, link.GetAttribute("href"));
    }
}
