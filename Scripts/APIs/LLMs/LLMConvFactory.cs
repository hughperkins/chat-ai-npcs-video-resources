using UnityEngine;

public abstract class LLMConvFactory : MonoBehaviour {
    public abstract ILLMConversation CreateConversation();
}
