using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using Newtonsoft.Json;



class Program
{
    static void Main(string[] args)
    {
        AI.init();
        if (!File.Exists(".\\data\\"))
            Directory.CreateDirectory(".\\data\\");

        if (!File.Exists(".\\model\\"))
            Directory.CreateDirectory(".\\model\\");
        var intents = ML.LoadIntents(".\\data\\intents.json");

        var mlContext = new MLContext();
        ITransformer model;
        // Check if the model file exists
        if (File.Exists(".\\model\\model.bin"))
        {
            // Load the model from the file
            model = ML.LoadModel(mlContext);
        }
        else
        {
            model = ML.Train(mlContext,intents);
        }

        Console.WriteLine("Chatbot started. Type 'exit' to quit.");
        ML.Run(mlContext,model,intents);
    }

   
}
