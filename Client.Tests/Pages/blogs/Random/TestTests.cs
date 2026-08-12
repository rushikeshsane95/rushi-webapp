using TestPage = BlazorBasic.Pages.blogs.Random.Test;

namespace Client.Tests.Pages.blogs.Random;

public sealed class TestTests : BunitContext
{
    public TestTests()
    {
        JSInterop.SetupVoid("eval", _ => true).SetVoidResult();
        JSInterop.SetupVoid("renderMath").SetVoidResult();
    }

    [Fact]
    public void Test_RendersMathContentAndInvokesMathRenderingJs()
    {
        var cut = Render<TestPage>();

        var paragraphs = cut.FindAll("p");
        Assert.Equal(2, paragraphs.Count);
        Assert.Equal("This is a math formula:", paragraphs[0].TextContent.Trim());
        Assert.Equal("$$ E = mc^2 $$", paragraphs[1].TextContent.Trim());

        var evalInvocation = Assert.Single(JSInterop.VerifyInvoke("eval", 1));
        Assert.Single(evalInvocation.Arguments);
        Assert.Contains("window.renderMath", Assert.IsType<string>(evalInvocation.Arguments[0]));

        cut.WaitForAssertion(() => JSInterop.VerifyInvoke("renderMath", 1));
    }
}
