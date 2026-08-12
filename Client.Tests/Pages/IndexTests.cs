using IndexPage = BlazorBasic.Pages.Index;

namespace Client.Tests.Pages;

public sealed class IndexTests : BunitContext
{
    [Fact]
    public void Index_RendersHeroAndProfileContent()
    {
        var cut = Render<IndexPage>();

        var aboutSection = cut.Find("section#about");
        Assert.Contains("portfolio-hero", aboutSection.ClassList);

        var heroImage = cut.Find(".hero-avatar img");
        Assert.Equal("./images/my_image.jpg", heroImage.GetAttribute("src"));
        Assert.Equal("My Photo", heroImage.GetAttribute("alt"));

        Assert.Equal("Rushikesh Sane", cut.Find(".hero-copy h1").TextContent);
        Assert.Contains(
            "Full stack developer focused on backend architecture, reliable systems, and practical engineering.",
            cut.Markup);
        Assert.Contains("Who am I?", cut.Markup);
        Assert.Contains("Originally from Pune, India", cut.Markup);
        Assert.Contains("software industry since 2017", cut.Markup);
        Assert.Contains("learning and sharing my knowledge", cut.Markup);
    }

    [Fact]
    public void Index_RendersKeySectionAnchorsAndLinks()
    {
        var cut = Render<IndexPage>();

        Assert.NotNull(cut.Find("section#about"));
        Assert.NotNull(cut.Find("section#technologies"));
        Assert.NotNull(cut.Find("section#projects"));
        Assert.NotNull(cut.Find("section#resume"));

        AssertLink(cut, "View work", "#projects");
        AssertLink(cut, "Contact me", "/contact");
        AssertLink(cut, "Almanac / Panchang", "/blogs/random/panchang");
        AssertLink(cut, "Trip to Chennai", "/blogs/travel/chennai");
        AssertLink(
            cut,
            "Governing AI in Codebases",
            "/blogs/technology/ai/agents-md-governing-ai-dotnet");
        AssertLink(
            cut,
            "Adapter Pattern Guide",
            "/blogs/technology/designpatterns/adapter-pattern-clean-architecture-guide");

        var personalityButton = cut.Find("button.primary-btn");
        Assert.Equal("The Commander", personalityButton.TextContent.Trim());
        Assert.Contains(
            "https://www.16personalities.com/entj-personality",
            personalityButton.GetAttribute("onclick"));

        var timelineButton = cut.Find("button.secondary-btn");
        Assert.Equal("View Timeline", timelineButton.TextContent.Trim());
    }

    [Fact]
    public void Index_RendersTechnologiesChildContent()
    {
        var cut = Render<IndexPage>();

        var technologiesSection = cut.Find("section#technologies");
        Assert.Equal("Technologies", technologiesSection.QuerySelector("h2")?.TextContent);
        Assert.Contains(
            "Tools I use to build reliable, modern applications.",
            technologiesSection.TextContent);

        var technologyCards = technologiesSection.QuerySelectorAll(".tech-card");
        Assert.Equal(4, technologyCards.Length);

        AssertTechnology(technologyCards[0], "gRPC", "images/grpc.svg", "gRPC logo");
        AssertTechnology(technologyCards[1], ".NET Core", "images/dotnet.svg", ".NET Core logo");
        AssertTechnology(technologyCards[2], "Azure", "images/azure.svg", "Azure logo");
        AssertTechnology(technologyCards[3], "Node.js", "images/nodejs.svg", "Node.js logo");
    }

    private static void AssertLink(IRenderedComponent<IndexPage> cut, string text, string expectedHref)
    {
        var link = cut.FindAll("a").Single(anchor => anchor.TextContent.Trim() == text);

        Assert.Equal(expectedHref, link.GetAttribute("href"));
    }

    private static void AssertTechnology(
        AngleSharp.Dom.IElement card,
        string expectedName,
        string expectedLogoPath,
        string expectedAltText)
    {
        Assert.Equal(expectedName, card.QuerySelector("h3")?.TextContent);

        var image = card.QuerySelector("img");
        Assert.NotNull(image);
        Assert.Equal(expectedLogoPath, image.GetAttribute("src"));
        Assert.Equal(expectedAltText, image.GetAttribute("alt"));
        Assert.Equal("lazy", image.GetAttribute("loading"));
    }
}
