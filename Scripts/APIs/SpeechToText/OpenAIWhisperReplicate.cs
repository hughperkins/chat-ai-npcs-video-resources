// call OpenAI Whisper, via Replicate
using System;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class WhisperRequest {
    public string version = "30414ee7c4fffc37e260fcab7842b5be470b9b840f2b608f5baa9bbef9a259ed";
    public WhisperRequestInput input = new WhisperRequestInput();
}


public class WhisperRequestInput {
    public string audio;
    // public string model = "large";
    public string model = "medium";
    public bool translate = false;
    public string language = null;
}

public class WhisperResponse {
    public string id;
    public string status;
    public WhisperResponseOutput output;
}

public class WhisperResponseOutput {
    public string detected_language;
    public List<WhisperResponseSegment> segments = new List<WhisperResponseSegment>();
}

public class WhisperResponseSegment {
    public string text;
}

public class OpenAIWhisperReplicate : SpeechToText {
    public override async Task<SpeechToTextResult> Transcribe(byte[] bytes) {
        Debug.Log("OpenAIWhisperReplicate");
        WhisperRequest whisperRequest = new WhisperRequest();
        string base64 = Convert.ToBase64String(bytes);
        whisperRequest.input.audio = $"data:audio/wav;base64,{base64}";

        using HttpClient client = new HttpClient();
        string jsonMessage = JsonConvert.SerializeObject(whisperRequest);
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.replicate.com/v1/predictions");
        requestMessage.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Token", Auth.replicate_api_key);
        HttpResponseMessage response = client.SendAsync(requestMessage).Result;
        string responseString = await response.Content.ReadAsStringAsync();

        WhisperResponse whisperResponse = JsonConvert.DeserializeObject<WhisperResponse>(responseString);
        string predictionId = whisperResponse.id;
        Debug.Log("Uploaded audio");

        while(whisperResponse.status == "starting" || whisperResponse.status == "processing") {
            await Task.Delay(300);
            requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://api.replicate.com/v1/predictions/{predictionId}");
            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Token", Auth.replicate_api_key);
            response = client.SendAsync(requestMessage).Result;
            responseString = await response.Content.ReadAsStringAsync();
            Debug.Log(responseString);

            whisperResponse = JsonConvert.DeserializeObject<WhisperResponse>(responseString);
        }
        Debug.Log(whisperResponse.output.detected_language);
        return new SpeechToTextResult(whisperResponse.output.detected_language, whisperResponse.output.segments[0].text);
    }
}
