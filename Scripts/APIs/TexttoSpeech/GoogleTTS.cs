using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WWUtils.Audio;

public class GoogleTTSInput {
    public string text;
}

public class GoogleTTSVoice {
    public string languageCode = "en-GB";
    public string name = "en-GB-Standard-A";
    public string ssmlGender = "FEMALE";
}

// public class GoogleTTSVoice {
//     public string languageCode = "yue-HK";
//     public string name = "yue-HK-Standard-A";
//     public string ssmlGender = "FEMALE";
// }

// https://cloud.google.com/text-to-speech/docs/reference/rest/v1/text/synthesize#AudioEncoding
public class GoogleTTSAudioConfig {
    // public string audioEncoding = "ALAW";
    public string audioEncoding = "LINEAR16";
    public float speakingRate = 1.1f;
    public float pitch = 0.0f;
    public float volumeGainDb = 0.0f;
}

public class GoogleTTSRequest {
    public GoogleTTSInput input = new GoogleTTSInput();
    public GoogleTTSVoice voice = new GoogleTTSVoice();
    public GoogleTTSAudioConfig audioConfig = new GoogleTTSAudioConfig();
}

public class GoogleTTSResponse {
    public string audioContent;
}

public class GoogleTTS {
    public static async Task<AudioClip> SayText(bool male, string text) {
        string google_speech_base_url = "https://texttospeech.googleapis.com";

        GoogleTTSRequest request = new GoogleTTSRequest();
        request.input.text = text;
        request.voice.name = male ? "en-GB-Standard-B" : "en-GB-Standard-A";
        request.voice.ssmlGender = male ? "MALE" : "FEMALE";

        string jsonMessage = JsonConvert.SerializeObject(request);
        using(HttpClient client = new HttpClient()) {
            HttpRequestMessage requestMessage = new HttpRequestMessage(
                HttpMethod.Post, $"{google_speech_base_url}/v1/text:synthesize"
            );
            requestMessage.Headers.Add("x-goog-api-key", Auth.google_api_key);
            requestMessage.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.SendAsync(requestMessage).Result;
            string responseString = await response.Content.ReadAsStringAsync();
            try {
                GoogleTTSResponse response_obj = JsonConvert.DeserializeObject<GoogleTTSResponse>(responseString);
                byte[] audioSamples = Convert.FromBase64String(response_obj.audioContent);
                WAV wav = new WAV(audioSamples);
                AudioClip clip = AudioClip.Create("foo", wav.SampleCount, wav.ChannelCount, wav.Frequency, false);
                clip.SetData(wav.LeftChannel, 0);
                return clip;
            } catch(Exception e ) {
                Debug.Log($"exception parsing {responseString}");
                Debug.Log(e.ToString());
                throw new Exception("[error calling api, see console]");
            }
        }
    }
}
