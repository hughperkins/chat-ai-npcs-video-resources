using System.Collections.Generic;
using System.Threading.Tasks;

public interface ILLMConversation {
    void AddPrompt(string message);
    void AddQuestion(string message);
    IAsyncEnumerable<string> SendStreaming();
    Task<string> SendAsync();
}
