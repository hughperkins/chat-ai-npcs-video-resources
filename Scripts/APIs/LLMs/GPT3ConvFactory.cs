using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPT3ConvFactory : LLMConvFactory
{
    public override ILLMConversation CreateConversation() {
        return new GPT3Conversation();
    }
}
