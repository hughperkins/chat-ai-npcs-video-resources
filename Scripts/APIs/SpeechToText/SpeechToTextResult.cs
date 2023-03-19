// to be returned from all speech to test apis
public class SpeechToTextResult {
    public string lang;
    public string text;
    public SpeechToTextResult(string lang, string text) {
        this.lang = lang;
        this.text = text;
    }
}

