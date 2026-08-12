using BlazorBasic.Pages.blogs.technology;
using Client.Tests.Support;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Tests.Pages.blogs.technology;

public sealed class TechnologyTests : BunitContext
{
    [Fact]
    public void Technology_RendersHeadingCategoriesAndMarkdownPostLinks()
    {
        Services.AddSingleton(new HttpClient(new TestHttpMessageHandler(
            new Dictionary<string, string>
            {
                ["blogs/technology/dependency-inversion-principle.md"] = """
                    # Dependency Inversion Principle

                    Dependencies should point toward abstractions.
                    """,
                ["blogs/technology/dependency-inversion-principle-part-2.md"] = """
                    # Dependency Inversion Principle Part 2

                    Follow-up notes.
                    """,
                ["blogs/technology/n-plus-one-problem.md"] = """
                    # The `N + 1` Problem

                    Query behavior notes.
                    """,
                ["blogs/technology/stateful-vs-stateless-applications.md"] = """
                    # Stateful vs Stateless Applications

                    Application state notes.
                    """
            }))
        {
            BaseAddress = new Uri("http://localhost/")
        });

        var cut = Render<Technology>();

        cut.WaitForAssertion(() =>
        {
            cut.Find("h2").TextContent.MarkupMatches("Technology Blogs");

            var links = cut.FindAll("ul a");
            Assert.Equal(6, links.Count);

            AssertLink(cut, "AI", "/blogs/technology/ai");
            AssertLink(cut, "Design Patterns", "/blogs/technology/designpatterns");
            AssertLink(cut, "Dependency Inversion Principle", "/blogs/technology/dependency-inversion-principle");
            AssertLink(cut, "Dependency Inversion Principle Part 2", "/blogs/technology/dependency-inversion-principle-part-2");
            AssertLink(cut, "The N + 1 Problem", "/blogs/technology/n-plus-one-problem");
            AssertLink(cut, "Stateful vs Stateless Applications", "/blogs/technology/stateful-vs-stateless-applications");
        });
    }

    private static void AssertLink(
        IRenderedComponent<Technology> cut,
        string expectedText,
        string expectedHref)
    {
        var link = cut.FindAll("a").Single(anchor => anchor.TextContent.Trim() == expectedText);

        Assert.Equal(expectedHref, link.GetAttribute("href"));
    }
}
