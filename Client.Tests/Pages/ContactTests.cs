using BlazorBasic.Pages;

namespace Client.Tests.Pages;

public sealed class ContactTests : BunitContext
{
    [Fact]
    public void Contact_RendersMeaningfulContactContent()
    {
        var cut = Render<Contact>();

        Assert.Contains("Let's Connect", cut.Markup);
        Assert.Contains("Have a question, an opportunity, or a collaboration idea?", cut.Markup);
        Assert.Contains("I usually respond within 1-2 business days.", cut.Markup);

        Assert.Equal("Name", cut.Find("label[for='name']").TextContent);
        Assert.Equal("Email", cut.Find("label[for='email']").TextContent);
        Assert.Equal("Message", cut.Find("label[for='message']").TextContent);
        Assert.Equal("Send Message", cut.Find("button[type='submit']").TextContent);
    }

    [Fact]
    public void Contact_RendersExpectedContactFormEndpointAndFields()
    {
        var cut = Render<Contact>();

        var form = cut.Find("form.contact-form");
        Assert.Equal("https://formspree.io/f/xregledn", form.GetAttribute("action"));
        Assert.Equal("POST", form.GetAttribute("method"));

        var nameInput = cut.Find("input#name");
        Assert.Equal("text", nameInput.GetAttribute("type"));
        Assert.Equal("name", nameInput.GetAttribute("name"));
        Assert.True(nameInput.HasAttribute("required"));

        var emailInput = cut.Find("input#email");
        Assert.Equal("email", emailInput.GetAttribute("type"));
        Assert.Equal("email", emailInput.GetAttribute("name"));
        Assert.True(emailInput.HasAttribute("required"));

        var messageInput = cut.Find("textarea#message");
        Assert.Equal("message", messageInput.GetAttribute("name"));
        Assert.Equal("7", messageInput.GetAttribute("rows"));
        Assert.True(messageInput.HasAttribute("required"));
    }
}
