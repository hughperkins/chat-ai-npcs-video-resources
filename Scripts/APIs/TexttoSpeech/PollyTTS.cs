using System;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using UnityEngine;
using WWUtils.Audio;
using System.IO;

class PollyTTS {
    // convert two bytes to one float in the range -1 to 1
    // from WAV.cs
    static float bytesToFloat(byte firstByte, byte secondByte) {
        // convert two bytes to one short (little endian)
        short s = (short)((secondByte << 8) | firstByte);
        // convert to range from -1 to (just below) 1
        return s / 32768.0F;
    }

    static float[] bytesArrayToFloatArray(byte[] bytes) {
        long N = bytes.Length / 2;
        float[] res = new float[N];
        for(int i = 0 ; i < N; i++) {
            long pos = i << 1;
            res[i] = bytesToFloat(bytes[pos], bytes[pos + 1]);
        }
        return res;
    }

    public static async Task<AudioClip> SayText(string lang, bool male, string text) {
        var client = new Amazon.Polly.AmazonPollyClient();
        int SampleRate = 16000;
        string VoiceId = "Emma";
        string engine = "neural";
        int Rate = 100;
        switch(lang) {
            case "zh":
            case "chinese":
                VoiceId = "Zhiyu";
                break;
            case "en":
            case "english":
                VoiceId = male ? "Arthur" : "Emma";
                engine = "neural";
                Rate = 110;
                break;
            case "fr":
            case "french":
                engine = "standard";
                VoiceId = male ? "Mathieu" : "Celine";
                break;
            case "es":
            case "spanish":
                engine = "standard";
                VoiceId = male ? "Enrique" : "Conchita";
                break;
        }
        var response = await client.SynthesizeSpeechAsync(new Amazon.Polly.Model.SynthesizeSpeechRequest{
            OutputFormat = "pcm",
            Engine = engine,
            Text = $"<speak><prosody rate=\"{Rate}%\">{text}</prosody></speak>",
            VoiceId = VoiceId,
            SampleRate = SampleRate.ToString(),
            TextType="ssml"
        });
        Stream audioStream = response.AudioStream;
        string contentType = response.ContentType;
        int requestCharacters = response.RequestCharacters;
        Debug.Log($"audioStream {audioStream} contentype {contentType} requestcharacerser {requestCharacters}");
        using var memoryStream = new MemoryStream();
        audioStream.CopyTo(memoryStream);
        byte[] audioBytes = memoryStream.ToArray();
        float[] audioSampled = bytesArrayToFloatArray(audioBytes);
        AudioClip clip = AudioClip.Create("foo", audioSampled.Length, 1, SampleRate, false);
        clip.SetData(audioSampled, 0);
        return clip;
        // audioSource.PlayOneShot(clip);
    }
}
