using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AI
{

    public static HttpClient client = new HttpClient();
    static string system = "";
    static string speaker = "cherry";
    static string language = "en";
    static string model = "llama3";

    public static void init(string xttsSpeaker, string lang, string ollamaModel="llama3", string systemPrompt= "You are Cherry Lite, an advanced offline assistant based on LAMBot, a Language Action Model bot designed for recognizing user intents and executing specific actions. As an open-source AI, you excel at understanding user intents, launching programs, and providing helpful responses. Answer the user's input in no more than three sentences, always addressing them as Sir. Respond only with the dialogue, nothing else.")
    {
        system = systemPrompt;
        speaker = xttsSpeaker;
        language = lang;
        model = ollamaModel;
        client.Timeout = TimeSpan.FromMinutes(10);
    }

    static string UcFirst(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }
        input = input.ToLower();
        // Check if the string is one character or more
        if (input.Length > 1)
        {
            return char.ToUpper(input[0]) + input.Substring(1);
        }
        else
        {
            // The string is exactly one character long
            return input.ToUpper();
        }
    }



    public static async Task<string> GetResponse(string prompt)
    {

        if (!HTTPCheck.status["Ollama"])
        {
            Console.ForegroundColor = ConsoleColor.Red; 
            Console.WriteLine("WARNING: OLLAMA - Text Generation service isnt running...");
            return "";
        }
        if (string.IsNullOrEmpty(prompt))
            return string.Empty;

        prompt = $"{system}\nUser: {prompt}";
        string url = "http://localhost:11434/api/chat";

        var jsonData = JsonConvert.SerializeObject(new
        {
            model = model,
            stream = false,
            prompt = $"System: {system}",
            messages = new[]
            {
        new { role = "user", content = $"System: {prompt}" }
    },
            keep_alive = -1
        });

        using (var httpClient = new HttpClient())
        {
            httpClient.Timeout = TimeSpan.FromMinutes(10);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<ResponseData2>(responseContent);
                    
                    string messageContent = responseData.Message.Content;
                    return messageContent;
                }
                else
                {
                    return $"HTTP error: {response.StatusCode}";
                }
            }
            catch (HttpRequestException e)
            {
                return $"HttpRequestException error: {e.Message}";
            }
        }
    }

    public static async Task<string> xttsRequestAndPlay(string text)
    {
        try
        {
            if (!HTTPCheck.status["XTTS"])
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("WARNING: XTTS - TextToSpeech service isn't running...");
                return "";
            }
            var jsonContent = new StringContent(
                $@"{{
                ""text"": ""{text}"",
                ""speaker_wav"": ""{speaker}"",
                ""language"": ""{language}""
            }}", Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("http://localhost:8020/tts_to_audio/", jsonContent);
            response.EnsureSuccessStatusCode();
            string result = System.IO.Path.GetTempPath() + $"\\{Guid.NewGuid().ToString()}.mp3";
            var bytes = await response.Content.ReadAsByteArrayAsync();

            await File.WriteAllBytesAsync(result, bytes);
            

            //
            return result;
        }
        catch (Exception ex)
        {

        }
        return null;
    }
}



public class ResponseData2
{
    public string Model { get; set; }
    public DateTime CreatedAt { get; set; }
    public MessageDetails Message { get; set; }
    public bool Done { get; set; }
    public long TotalDuration { get; set; }
    public long LoadDuration { get; set; }
    public int PromptEvalCount { get; set; }
    public long PromptEvalDuration { get; set; }
    public int EvalCount { get; set; }
    public long EvalDuration { get; set; }
}

public class MessageDetails
{
    public string Role { get; set; }
    public string Content { get; set; }
}
public class ResponseData
{
    public string Response { get; set; }
}
