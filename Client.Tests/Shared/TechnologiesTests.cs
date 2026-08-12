using BlazorBasic.Shared;

namespace Client.Tests.Shared;

public sealed class TechnologiesTests : BunitContext
{
    [Fact]
    public void Technologies_RendersHeadingAndTechnologyCards()
    {
        var cut = Render<Technologies>();

        Assert.Equal("Technologies", cut.Find("h2").TextContent.Trim());
        Assert.Empty(cut.FindAll("a"));

        var technologies = new[]
        {
            new { Name = "gRPC", Alt = "gRPC logo", Src = "images/grpc.svg" },
            new { Name = ".NET Core", Alt = ".NET Core logo", Src = "images/dotnet.svg" },
            new { Name = "Azure", Alt = "Azure logo", Src = "images/azure.svg" },
            new { Name = "Node.js", Alt = "Node.js logo", Src = "images/nodejs.svg" }
        };

        foreach (var technology in technologies)
        {
            var card = cut.FindAll(".tech-card")
                .Single(element => element.QuerySelector("h3")?.TextContent.Trim() == technology.Name);
            var image = card.QuerySelector("img");

            Assert.NotNull(image);
            Assert.Equal(technology.Alt, image.GetAttribute("alt"));
            Assert.Equal(technology.Src, image.GetAttribute("src"));
        }
    }
}
