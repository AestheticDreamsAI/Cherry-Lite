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

    static string ainame = "llama3";
    static string lang = "en";
    static string system = "";

    public static async Task<string> GetVision(string prompt, string imgDataUri, string model)
    {
        if (string.IsNullOrEmpty(prompt))
            return string.Empty;
        prompt = $"{prompt}";
        string url = "https://localhost:11434/api/generate";

        var jsonData = JsonConvert.SerializeObject(new
        {
            model = model,
            stream = false,
            prompt = $"{prompt}",
            images = new[] { imgDataUri }
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
                    var responseObj = JsonConvert.DeserializeObject<ResponseData>(responseContent);
                    string processedResponse = responseObj.Response.Trim();
                    if (processedResponse.Contains(":"))
                    {
                        if (processedResponse.Split(":")[0].Contains($"{UcFirst(ainame)}"))
                            processedResponse = processedResponse.Split(":")[1].Trim();
                    }
                    return processedResponse;
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


    public static void init(string model = "llama3", string language = "en")
    {
        lang = language;
        ainame = model;
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



    public static async Task<string> GetResponse(string name, string prompt, string model)
    {
        if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(prompt))
            return string.Empty;

        prompt = $"{system}\nUser ({name}): {prompt}";
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
                    // Zugriff auf den Inhalt der Nachricht
                    string messageContent = responseData.Message.Content; // Hier extrahieren wir den Inhalt
                    string processedResponse = messageContent.Replace($"{UcFirst(ainame)}: ", "").Replace("Lily says: ", "").Replace($"{name}: ", "").Replace($"{name} says: ", "").Trim();
                    return processedResponse.Replace("Lily Paminy: ", getName() + ": ");
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

    public static async Task<string> SendTtsRequestAndGetDataUri(string text)
    {
        try
        {

            var jsonContent = new StringContent(
                $@"{{
                ""text"": ""{text}"",
                ""speaker_wav"": ""{ainame.ToLower()}"",
                ""language"": ""{lang}""
            }}", Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://localhost:8020/tts_to_audio/", jsonContent);
            response.EnsureSuccessStatusCode();
            string result = System.IO.Path.GetTempPath() + $"\\{Guid.NewGuid().ToString()}.mp3";
            var bytes = await response.Content.ReadAsByteArrayAsync();

            var uri = Convert.ToBase64String(bytes);
            await File.WriteAllBytesAsync(result, bytes);

            // Convert MP3 to OPUS format
            //
            return result;
        }
        catch (Exception ex)
        {

        }
        return null;
    }

    public static string getName()
    {
        return UcFirst(ainame);
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
