using System.Diagnostics;
using NAudio.Wave;

namespace YuzuRPG.Core.Audio;

public class AudioManager : IAudioManager
{
    private CancellationTokenSource musicTokenSource;
    private Thread musicThread;
    public float Volume;
    
    public AudioManager(float volume)
    {
        Volume = volume / 100;
    }

    public void PlayMusic(string audioTrack)
    {
        musicTokenSource = new CancellationTokenSource();
        musicThread = new Thread(() => PlayTrack(musicTokenSource.Token, audioTrack));
        musicThread.Start();
    }

    public void StopMusic()
    {
        musicTokenSource.Cancel(); // request cancellation. 
        musicThread.Join(); // join is a blocking call until music finishes to terminate
        musicTokenSource.Dispose(); 
    }

    private void PlayEffect(CancellationToken token, string audioTrack)
    {
        
    }

    private void PlayTrack(CancellationToken token, string audioTrack)
    {
        var timer = new Stopwatch();
        var introTrack = @$"./Content/Audio/{audioTrack}/intro.wav";
        var mainTrack = @$"./Content/Audio/{audioTrack}/loop.wav";
        var endTrack = @$"./Content/Audio/{audioTrack}/end.wav";

        if (!File.Exists(introTrack))
        {
            // doing all these using calls so I don't have to manually dispose later lol
            
            using(var audioMainFile = new AudioFileReader(mainTrack))
            using (var loopStream = new LoopStream(audioMainFile))
            using (var outputMainDevice = new WaveOutEvent())
            {
                outputMainDevice.Volume = Volume;
                // super slight delay at the end to sound a bit better(?)
                outputMainDevice.Init(loopStream);
                outputMainDevice.Play();

                while (!token.IsCancellationRequested)
                {
                    Thread.Sleep(10);
                }
            }
        }

        else
        {
            // doing all these using calls so I don't have to manually dispose later lol

            using(var audioFile = new AudioFileReader(introTrack))
            using(var audioMainFile = new AudioFileReader(mainTrack))
            using (var loopStream = new LoopStream(audioMainFile))
            using (var outputMainDevice = new WaveOutEvent())
            using(var outputDevice = new WaveOutEvent())
            {
                outputDevice.Volume = Volume;
                outputMainDevice.Volume = Volume;
                // super slight delay at the end to sound a bit better(?)
                outputDevice.Init(audioFile);
                outputMainDevice.Init(loopStream);
                outputDevice.Play();
                timer.Start();

                while (!token.IsCancellationRequested)
                {
                    // we're using a timer because both the StoppedPlayback event and checking WaveOutEvent.PlaybackState
                    // have a slight delay
                    if (timer.Elapsed > audioFile.TotalTime)
                    {
                        outputMainDevice.Play();
                        timer.Reset();
                    }
                }
            
            }
        }

        if (File.Exists(endTrack))
        {
            using(var audioMainFile = new AudioFileReader(endTrack))
            using (var outputMainDevice = new WaveOutEvent())
            {
                outputMainDevice.Volume = Volume;
                // super slight delay at the end to sound a bit better(?)
                outputMainDevice.Init(audioMainFile);
                outputMainDevice.Play();

                while (outputMainDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(10);
                }
            }
        }
        
        // cleanup!
    }

}