using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using OpenAI_API;
using UnityEngine;

public class GPT3Conversation : ILLMConversation {
    string model;
    float temperature;
    int maxTokens;

    OpenAIAPI api;
    StringBuilder promptBuilder;

    public GPT3Conversation(
            string model = "text-davinci-003", float temperature = 0.7f, int maxTokens = 256) {
        this.model = model;
        this.temperature = temperature;
        this.maxTokens = maxTokens;

        promptBuilder = new StringBuilder();
        api = new OpenAIAPI(Auth.openai_api_key);
    }

    public void AddPrompt(string message) {
        promptBuilder.Append(message);
        promptBuilder.Append("\n");
    }

    public void AddQuestion(string message) {
        promptBuilder.Append($"Q: {message}\n");
        promptBuilder.Append($"A: ");
    }

    OpenAI_API.Completions.CompletionRequest buildRequest() {
        OpenAI_API.Completions.CompletionRequest request = new OpenAI_API.Completions.CompletionRequest
        {
            Prompt=promptBuilder.ToString(),
            Model = model,
            Temperature = temperature,
            MaxTokens = maxTokens,
            Echo=false
        };
        return request;
    }

    public async IAsyncEnumerable<string> SendStreaming() {
        OpenAI_API.Completions.CompletionRequest request = buildRequest();
        await foreach (var res in api.Completions.StreamCompletionEnumerableAsync(request))
        {
            if (res.Completions.Count > 0)
            {
                string messageBit = res.ToString();
                promptBuilder.Append(messageBit);
                yield return messageBit;
            }
        }
    }

    public async Task<string> SendAsync() {
        OpenAI_API.Completions.CompletionRequest request = buildRequest();
        var res = await api.Completions.CreateCompletionAsync(request);
        string completion = res.ToString();
        promptBuilder.Append(completion);
        return completion;
    }
}
