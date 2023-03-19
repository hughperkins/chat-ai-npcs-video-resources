using System.Threading.Tasks;
using UnityEngine;

public abstract class SpeechToText : MonoBehaviour {
    public abstract Task<SpeechToTextResult> Transcribe(byte[] wavAudio);
}
