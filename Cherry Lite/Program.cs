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
    public static AudioListener audioListener;
    static async Task Main(string[] args)
    {
        AI.init("cherry","en");
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
            //Train a new model based on intents
            model = ML.Train(mlContext,intents);
        }
        audioListener = new AudioListener(mlContext, model, intents);
        audioListener.StartListening();
        //await ML.Run(mlContext,model,intents);
        while (true)
        {
            var key = Console.ReadKey(true).KeyChar;

            if (key == 's')
            {
                audioListener.StartListening();
            }
            else if (key == 't')
            {
                audioListener.StopListening();
            }
            else if (key == 'q')
            {
                break;
            }
        }
    }

   
}
