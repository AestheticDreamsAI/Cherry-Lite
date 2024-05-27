﻿using Microsoft.ML;
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
            return intent.Responses[index];
        }
        return "Sorry, I don't understand.";
    }

    public static void ExecuteAction(string tag, IntentCollection intents)
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
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to execute action: {action}, Error: {ex.Message}");
                }
            }
        }
    }

    public static void Run(MLContext mlContext, ITransformer model, IntentCollection intents)
    {
        while (true)
        {
            Console.Write("You: ");
            string userInput = Console.ReadLine();
            if (userInput.ToLower() == "exit")
                break;

            var prediction = ML.Predict(mlContext, model, userInput);
            var response = ML.GenerateResponse(prediction.PredictedLabel, intents);
            Console.WriteLine("Bot: " + response);

            ML.ExecuteAction(prediction.PredictedLabel, intents);
        }
    }
}