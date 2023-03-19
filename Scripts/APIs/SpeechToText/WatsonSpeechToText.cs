using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class WatsonSTTResponse {
    public List<WatsonSTTResult> results = new List<WatsonSTTResult>();
}

public class WatsonSTTResult {
    public List<WatsonSTTAlternative> alternatives = new List<WatsonSTTAlternative>();
}

public class WatsonSTTAlternative {
    public string transcript;
}

public class WatsonSpeechToText : SpeechToText {
    public async override Task<SpeechToTextResult> Transcribe(byte[] wavAudio) {
    // public async override SpeechToTextResult Transcribe(byte[] wavAudio) {
        using HttpClient client = new HttpClient();
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post,
            "https://api.us-east.speech-to-text.watson.cloud.ibm.com/instances/2956face-832f-4550-849c-bdcd83c32f4a/v1/recognize");
        requestMessage.Content = new ByteArrayContent(wavAudio);
        requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/wav");
        string encoded = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                               .GetBytes($"apikey:{Auth.ibm_api_key}"));
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic", encoded);
        Debug.Log(requestMessage.ToString());
        HttpResponseMessage response = client.SendAsync(requestMessage).Result;
        string responseString = await response.Content.ReadAsStringAsync();
        Debug.Log($"responseString {responseString}");

        WatsonSTTResponse watsonResponse = JsonConvert.DeserializeObject<WatsonSTTResponse>(responseString);
        string speech = watsonResponse.results[0].alternatives[0].transcript;
        // Debug.Log(speech);

        return new SpeechToTextResult("en", speech);
    }
}
