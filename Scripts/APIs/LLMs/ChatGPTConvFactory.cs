using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatGPTConvFactory : LLMConvFactory
{
    public override ILLMConversation CreateConversation() {
        return new ChatGPTConversation();
    }
}
