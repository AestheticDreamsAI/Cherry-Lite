using Microsoft.ML;
using Microsoft.ML.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

// Models for intents and responses
public class Intent
{
    public string Tag { get; set; }
    public List<string> Patterns { get; set; }
    public List<string> Responses { get; set; }
    public List<string> Actions { get; set; }
}

public class IntentCollection
{
    public List<Intent> Intents { get; set; }
}

public class IntentData
{
    public string Text { get; set; }
    public string Label { get; set; }
}

public class IntentPrediction
{
    [ColumnName("PredictedLabel")]
    public string PredictedLabel { get; set; }
}

public class ML
{
    public static ITransformer LoadModel(MLContext mlContext,string modelPath= ".\\model\\model.bin")
    {
        var model = mlContext.Model.Load(modelPath, out var modelInputSchema);
        Console.WriteLine($"Model loaded from {modelPath}");
        return model;
    }
    public static ITransformer Train(MLContext mlContext, IntentCollection intents, string modelPath= ".\\model\\model.bin")
    {
        // Train a new model
        ITransformer model = null;
        var data = PrepareData(intents);
        var dataView = mlContext.Data.LoadFromEnumerable(data);

        var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label")
            .Append(mlContext.Transforms.Text.FeaturizeText("Features", "Text"))
            .Append(mlContext.Transforms.Concatenate("Features", "Features"))
            .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        model = pipeline.Fit(dataView);

        // Save the model to a file
        mlContext.Model.Save(model, dataView.Schema, modelPath);
        Console.WriteLine($"Model trained and saved to {modelPath}");
        return model;
    }
    public static IntentCollection LoadIntents(string filePath)
    {
        if (File.Exists(filePath))
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<IntentCollection>(json);
            }
        }
        else
        {
            Console.WriteLine($"ERROR: {filePath} not found...");
            return new IntentCollection();
        }
    }

    public static IEnumerable<IntentData> PrepareData(IntentCollection intents)
    {
        var data = new List<IntentData>();
        foreach (var intent in intents.Intents)
        {
            foreach (var pattern in intent.Patterns)
            {
                data.Add(new IntentData { Text = pattern, Label = intent.Tag });
            }
        }
        return data;
    }

    public static IntentPrediction Predict(MLContext mlContext, ITransformer model, string text)
    {
        var predictionEngine = mlContext.Model.CreatePredictionEngine<IntentData, IntentPrediction>(model);
        return predictionEngine.Predict(new IntentData { Text = text });
    }

    public static string GenerateResponse(string tag, IntentCollection intents)
    {
        var intent = intents.Intents.FirstOrDefault(i => i.Tag == tag);
        if (intent != null)
        {
            Random rnd = new Random();
            int index = rnd.Next(intent.Responses.Count);
            var response = intent.Responses[index];
            return response;
        }
        return "";
    }

    public static bool ExecuteAction(string tag, IntentCollection intents)
    {
        var intent = intents.Intents.FirstOrDefault(i => i.Tag == tag);
        if (intent != null && intent.Actions != null && intent.Actions.Count > 0)
        {
            foreach (var action in intent.Actions)
            {
                try
                {
                    Process.Start(action);
                    Console.WriteLine($"Executing action: {action}");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to execute action: {action}, Error: {ex.Message}");

                }
            }
        }                   
        return false;
    }

    public static async Task Run(MLContext mlContext, ITransformer model, IntentCollection intents)
    {
        while (true)
        {
            var OllamaResponse = "";
            Console.Write("You: ");
            string userInput = Console.ReadLine();
            if (userInput.ToLower() == "exit")
                break;
            OllamaResponse = await AI.GetResponse(userInput);
            var prediction = Predict(mlContext, model, OllamaResponse);
            var response = GenerateResponse(prediction.PredictedLabel, intents);


            var audio = await AI.xttsRequestAndPlay(OllamaResponse);
            NaudioToolkit.PlayAudio(audio);
            Console.WriteLine("Bot: " + OllamaResponse);
            ExecuteAction(prediction.PredictedLabel, intents);
        }
    }

    public static async Task Run2(MLContext mlContext, ITransformer model, IntentCollection intents, string userInput)
    {
        try
        {
            Console.WriteLine($"You: {userInput}");

            // Get the response from the AI model
            var ollamaResponse = await AI.GetResponse(userInput);

            // Perform prediction
            var prediction = Predict(mlContext, model, ollamaResponse);

            // Generate a response based on the prediction
            var response = GenerateResponse(prediction.PredictedLabel, intents);

            // Request and play audio
            var audioFilePath = await AI.xttsRequestAndPlay(ollamaResponse);
            Console.WriteLine("Bot: " + ollamaResponse);
            // Execute any actions based on the prediction
            ExecuteAction(prediction.PredictedLabel, intents);

            if (!string.IsNullOrEmpty(audioFilePath))
            {
                await Task.Run(() => NaudioToolkit.PlayAudio(audioFilePath)); // Ensure it waits for playback to complete
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Run2: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Restarting listener...");
            Program.audioListener.StartListening();
        }
    }

}