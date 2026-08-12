using BlazorBasic.Pages.blogs.technology.designpatterns;
using Client.Tests.Support;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Tests.Pages.blogs.technology.designpatterns;

public sealed class AdapterPatternCleanArchitectureGuideTests : BunitContext
{
    [Fact]
    public void AdapterPatternCleanArchitectureGuide_RendersConvertedMarkdownBackLinkAndJsInterop()
    {
        const string markdown = """
            # Adapter Pattern in Clean Architecture

            Adapters keep **external dependencies** outside the core application.

            ```csharp
            public sealed class PaymentGatewayAdapter
            {
                public Task PayAsync() => Task.CompletedTask;
            }
            ```
            """;

        Services.AddSingleton(new HttpClient(new TestHttpMessageHandler(
            new Dictionary<string, string>
            {
                ["blogs/technology/designpatterns/Adapter_Pattern_Clean_Architecture_Guide.md"] = markdown
            }))
        {
            BaseAddress = new Uri("http://localhost/")
        });

        var renderMermaid = JSInterop.SetupVoid("renderMermaid").SetVoidResult();
        var renderCodeHighlighting = JSInterop.SetupVoid("renderCodeHighlighting").SetVoidResult();

        var cut = Render<AdapterPatternCleanArchitectureGuide>();

        cut.WaitForAssertion(() =>
        {
            Assert.DoesNotContain("Loading blog post...", cut.Markup);
            cut.Find("h1").TextContent.MarkupMatches("Adapter Pattern in Clean Architecture");

            var markdownParagraph = cut.FindAll("p")
                .Single(paragraph => paragraph.TextContent.Trim() == "Adapters keep external dependencies outside the core application.");

            Assert.Contains("<strong>external dependencies</strong>", markdownParagraph.InnerHtml);

            var codeBlock = cut.Find("pre code");
            Assert.Contains("public sealed class PaymentGatewayAdapter", codeBlock.TextContent);

            var backLink = cut.Find("a");
            Assert.Equal("/blogs/technology/designpatterns", backLink.GetAttribute("href"));
            Assert.Equal("Back to Design Patterns", backLink.TextContent.Trim());

            Assert.Single(renderMermaid.Invocations);
            Assert.Single(renderCodeHighlighting.Invocations);
        });
    }
}
