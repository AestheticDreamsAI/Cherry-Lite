using NAudio.Wave;

public static class NaudioHelper
{
    public static void PlayAudio(string filePath)
    {
        if (File.Exists(filePath))
        {
            using (var audioFile = new AudioFileReader(filePath))
            using (var outputDevice = new WaveOutEvent())
            {
                var tcs = new TaskCompletionSource<bool>();

                outputDevice.Init(audioFile);
                outputDevice.PlaybackStopped += (s, e) =>
                {
                    tcs.SetResult(true);
                };

                outputDevice.Play();
                tcs.Task.Wait(); // Wait for playback to complete

                outputDevice.Dispose();
                audioFile.Dispose();
            }
        }
        else
        {
            Console.WriteLine($"Audio file not found: {filePath}");
        }
    }
}
