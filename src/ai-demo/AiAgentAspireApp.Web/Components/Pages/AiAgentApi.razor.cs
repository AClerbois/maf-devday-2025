using System.ComponentModel;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace AiAgentAspireApp.Web.Components.Pages;

public partial class AiAgentApi
{
    private readonly MarkupString responseText;
    private string userInput = string.Empty;
    private ElementReference messagesContainer;
    private readonly List<ChatMessageModel> Messages = [];
    private bool isStreaming = false;

    [Inject]
    public required AgentApiClient AgentApiClient { get; set; }

    private class ChatMessageModel
    {
        public string Content { get; set; } = string.Empty;
        public bool IsUser { get; set; }
    }

    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(userInput) || isStreaming)
        {
            return;
        }

        // Add user message
        Messages.Add(new ChatMessageModel
        {
            Content = userInput,
            IsUser = true
        });

        // Prepare AI response container
        var aiResponse = new StringBuilder();
        var testResponse = string.Empty;
        Messages.Add(new ChatMessageModel
        {
            Content = "",
            IsUser = false
        });

        try
        {
            isStreaming = true;

            // Stream AI response
            await foreach (var chunk in AgentApiClient.StreamAgentChatAsync(userInput))
            {
                aiResponse.Append(chunk);
                Messages[^1].Content = aiResponse.ToString();
                await InvokeAsync(StateHasChanged);
                await ScrollToBottom();
            }
        }
        catch (Exception ex)
        {
            Messages[^1].Content = $"Error: {ex.Message}";
        }
        finally
        {
            isStreaming = false;
            userInput = string.Empty;
        }
    }

    private async Task ScrollToBottom()
    {
        await JSRuntime!.InvokeVoidAsync("scrollToBottom", messagesContainer);
    }

    private async Task HandleKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !isStreaming)
        {
            await SendMessage();
        }
    }

    // Inject JavaScript for scrolling
    [Inject] private IJSRuntime? JSRuntime { get; set; }

    [Description("Gets the author of the story.")]
    public string GetAuthor() => "Jack Torrance";

    [Description("Formats the story for display.")]
    public string FormatStory(string title, string author, string story) =>
        $"Title: {title}\nAuthor: {author}\n\n{story}";

}
