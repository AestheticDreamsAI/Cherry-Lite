using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

public class HTTPCheck
{
    public static Dictionary<string, bool> status = new Dictionary<string, bool>()
        {
            {"Ollama", false },
             {"XTTS", false }
        };
    public static async Task Check()
    {
        var urls = new Dictionary<string, string>
        {
            { "Ollama", "http://localhost:11434" },
            { "XTTS", "http://localhost:8020/" }
        };

        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(5);
            foreach (var kvp in urls)
            {
                string serviceName = kvp.Key;
                string url = kvp.Value;
                if (serviceName == "XTTS")
                    url = $"{url}docs";
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    Console.ForegroundColor = ConsoleColor.Green;
                    status[serviceName] = true;
                    Console.WriteLine($"{serviceName} is running");
                }
                catch (HttpRequestException e)
                {
                    status[serviceName] = false;
                    Console.ForegroundColor = ConsoleColor.Red; 
                }
                catch (Exception ex)
                {
                    status[serviceName] = false;
                    Console.ForegroundColor = ConsoleColor.Red;
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
