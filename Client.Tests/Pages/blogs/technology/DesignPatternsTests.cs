using BlazorBasic.Pages.blogs.technology;
using Client.Tests.Support;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Tests.Pages.blogs.technology;

public sealed class DesignPatternsTests : BunitContext
{
    [Fact]
    public void DesignPatterns_RendersHeadingIntroMarkdownTitleAndPostLink()
    {
        Services.AddSingleton(new HttpClient(new TestHttpMessageHandler(
            new Dictionary<string, string>
            {
                ["blogs/technology/designpatterns/Adapter_Pattern_Clean_Architecture_Guide.md"] = """
                    # Adapter Pattern in Clean Architecture

                    Notes about adapting external dependencies.
                    """
            }))
        {
            BaseAddress = new Uri("http://localhost/")
        });

        var cut = Render<DesignPatterns>();

        cut.WaitForAssertion(() =>
        {
            cut.Find("h2").TextContent.MarkupMatches("Design Patterns");

            var intro = cut.FindAll("p")
                .Single(paragraph => paragraph.TextContent.Trim() == "Explore the Design Patterns series.");
            Assert.Equal("Explore the Design Patterns series.", intro.TextContent.Trim());

            var link = cut.FindAll("ul a")
                .Single(anchor => anchor.TextContent.Trim() == "Adapter Pattern in Clean Architecture");

            Assert.Equal(
                "/blogs/technology/designpatterns/adapter-pattern-clean-architecture-guide",
                link.GetAttribute("href"));
        });
    }
}
