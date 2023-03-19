// call OpenAI Whisper, via our own aiohttp python service
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class OurWhisperRequest {
    public string audio;
}

public class OurWhisperResponse {
    public string lang;
    public string text;
}

public class OpenAIWhisperOurs : SpeechToText {
    // string base_url;

    [SerializeField] string OurOpenAIServiceBaseUrl = "http://localhost:8080/";

    void Start() {
        CheckInitializedFields.Check(this);
    }

    // // base url MUST end in /
    // public OpenAIWhisperOurs(string base_url = "http://localhost:8080/") {
    //     this.base_url = base_url;
    // }

    public override async Task<SpeechToTextResult> Transcribe(byte[] bytes) {
        Debug.Log("OpenAIWhisperSelfHost");
        OurWhisperRequest whisperRequest = new OurWhisperRequest();
        string base64 = Convert.ToBase64String(bytes);
        whisperRequest.audio = $"data:audio/wav;base64,{base64}";

        using HttpClient client = new HttpClient();
        string jsonMessage = JsonConvert.SerializeObject(whisperRequest);
        if(!OurOpenAIServiceBaseUrl.EndsWith('/')) {
            OurOpenAIServiceBaseUrl += "/";
        }
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{OurOpenAIServiceBaseUrl}whisper");
        requestMessage.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Token", Auth.replicate_api_key);
        HttpResponseMessage response = client.SendAsync(requestMessage).Result;
        string responseString = await response.Content.ReadAsStringAsync();
        Debug.Log($"responseString {responseString}");

        OurWhisperResponse whisperResponse = JsonConvert.DeserializeObject<OurWhisperResponse>(responseString);
        Debug.Log($"lang {whisperResponse.lang} text {whisperResponse.text}");
        // return whisperResponse.output.segments[0].text;
        return new SpeechToTextResult(whisperResponse.lang, whisperResponse.text);
    }
}
