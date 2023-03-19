// call OpenAI Whisper, via openai api, https://platform.openai.com/docs/guides/speech-to-text
//
// curl equivalent:
// curl https://api.openai.com/v1/audio/transcriptions \
//   -H "Authorization: Bearer $OPENAI_API_KEY" \
//   -H "Content-Type: multipart/form-data" \
//   -F model="whisper-1" \
//   -F file="@/path/to/file/openai.mp3"
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;

public class WhisperOpenAIResponse
{
    public string text = "";
}

public class OpenAIWhisperOpenAI : SpeechToText {
    string OpenAIServiceBaseUrl = "https://api.openai.com/";  // should end in /
    // string OpenAIServiceBaseUrl = "http://127.0.0.1:8080/";  // should end in /

    private static readonly HttpClient httpClient;

    static OpenAIWhisperOpenAI()
    {
        httpClient = new HttpClient();
    }

    void Start() {
        CheckInitializedFields.Check(this);
    }

    public override async Task<SpeechToTextResult> Transcribe(byte[] bytes) {
        Debug.Log("OpenAIWhisperOpenAI");

        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{OpenAIServiceBaseUrl}v1/audio/transcriptions");

        MultipartFormDataContent form = new MultipartFormDataContent();
        using ByteArrayContent byteArrayContent = new ByteArrayContent(bytes);
        form.Add(byteArrayContent, "file", "foo.wav");
        form.Add(new StringContent("whisper-1"), "model");

        requestMessage.Content = form;
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer", Auth.openai_api_key);
        HttpResponseMessage httpResponse = await httpClient.SendAsync(requestMessage);
        string responseString = await httpResponse.Content.ReadAsStringAsync();
        Debug.Log(responseString);

        WhisperOpenAIResponse response = JsonConvert.DeserializeObject<WhisperOpenAIResponse>(responseString);
        string speech = response is null ? "" : response.text;
        // unfortunately openai api doesn't currently tell us the language...
        return new SpeechToTextResult("en", speech);
    }
}
