using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    internal class NaudioToolkit
    {
    public static void PlayAudio(string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return;
        WaveOutEvent outputDevice = new WaveOutEvent();
        AudioFileReader audioFile = new AudioFileReader(filePath);

        outputDevice.Init(audioFile);
        // Event-Handler für PlaybackStopped hinzufügen
        outputDevice.PlaybackStopped += (sender, args) =>
        { 
            // Aufräumen
            audioFile.Dispose();
            outputDevice.Dispose();
        };

        outputDevice.Play();
    }
}
