# chat-ai-npcs-video-resources
chat-ai-npcs-video-resources

Code for video "Create Intelligent NPCs using Chat AI!", https://youtu.be/RSzeqjKJABk

<div align="left">
      <a href="https://www.youtube.com/watch?v=RSzeqjKJABk">
         <img src="https://img.youtube.com/vi/RSzeqjKJABk/0.jpg" style="width:180px;">
      </a>
</div>

Currently this contains code for:
- speech to text
- text to speech
- wave converters (which are from third-parties)
- wrappers for handling conversations, and conversation state, with chatgpt and gpt3

# ThirdParty/Wav

- SavWav.cs: convert AudioClips into byte arrays, that you can send to speech to text.

Given AudioClip clip:
```
    int numSamples = (int)(clip.frequency * clipElapsedTime);
    float[] clipData = new float[numSamples];
    clip.GetData(clipData, 0);
    byte[] wavBytes = SavWav.SamplesToBytes(clip.frequency, clipData);
```
- WAV.cs: convert byte arrays of WAV data into data you can load into an AudioClip.

(In fact, if you look at GoogleTTS, it shows you how to use WAV.cs; and it calls it for you automatically)

# Speech to text

- first use SavWav.cs to convert the AudioClip into wave bytes, then pick one of the converters, e.g. OpenAIWhisperOpenAI, and use like:
```
SpeechToText speechtotext = new OpenAIWhisperOpenAI()
SpeechToTextResult result = await speechtotext.Transcribe(wavBytes);
```

# text to speech

- pick one of the converters, e.g. PolyTTS, then do e.g.
```
AudioClip clip = PolyTTS.SayText("en", false, "Hi guys!");
```

# chatgpt, gpt3 conversation wrappers

E.g. for ChatGPT:

```
ILLMConversation conversation = new ChatGPTConversation();
conversation.AddPrompt("You are Nora, a magician from the north.");
conversation.AddQuestion("What is your name?");
await foreach(string sentence in conversation.SendStreaming()) {
    // do something with sentence here :)
}
```

Same interface works for GPT3Conversation.
