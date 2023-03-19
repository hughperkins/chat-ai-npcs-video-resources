using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using OpenAI_API;
using UnityEngine;

public class ChatGPTConversation : ILLMConversation {
    string model;
    float temperature;
    int maxTokens;

    OpenAIAPI api;
    List<OpenAI_API.Chat.ChatMessage> messages;

    public ChatGPTConversation(
            string model = "gpt-3.5-turbo", float temperature = 0.7f, int maxTokens = 256) {
        this.model = model;
        this.temperature = temperature;
        this.maxTokens = maxTokens;

        messages = new List<OpenAI_API.Chat.ChatMessage>();
        api = new OpenAIAPI(Auth.openai_api_key);
    }

    public void AddPrompt(string message) {
        messages.Add(new OpenAI_API.Chat.ChatMessage("assistant", message));
    }

    public void AddQuestion(string message) {
        messages.Add(new OpenAI_API.Chat.ChatMessage("user", message));
    }

    OpenAI_API.Chat.ChatRequest buildRequest() {
        OpenAI_API.Chat.ChatRequest request = new OpenAI_API.Chat.ChatRequest
        {
            Messages = messages,
            Model = model,
            Temperature = temperature,
            MaxTokens = maxTokens
        };
        return request;
    }

    public async IAsyncEnumerable<string> SendStreaming() {
        OpenAI_API.Chat.ChatRequest request = buildRequest();
        StringBuilder messageBuilder = new StringBuilder();
        await foreach (var res in api.Chat.StreamChatEnumerableAsync(request))
        {
            if (res.Choices.Length > 0)
            {
                string messageBit = res.Choices[0].delta.content;
                messageBuilder.Append(messageBit);
                yield return messageBit;
            }
        }
        messages.Add(new OpenAI_API.Chat.ChatMessage("assistant", messageBuilder.ToString()));
    }

    public async Task<string> SendAsync() {
        OpenAI_API.Chat.ChatRequest request = buildRequest();
        var res = await api.Chat.CreateChatAsync(request);
        messages.Add(res.Choices[0].Message);
        return res.Choices[0].Message.Content;
    }
}
