using System.Diagnostics;
using CSCore.SoundOut;

namespace YuzuRPG.Core.Audio;

public class CSCoreAudioManager : IAudioManager
{
    private CancellationTokenSource musicTokenSource;
    private Thread musicThread;
    public int Volume;

    private CSCoreMusicPlayer engine;

    public CSCoreAudioManager(float volume)
    {
        engine = new CSCoreMusicPlayer();
        Volume = (int)volume / 100;
        engine.Volume = Volume;
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
            engine.Open(mainTrack);
            engine.Play();
                while (!token.IsCancellationRequested)
                {
                    Thread.Sleep(10);
                }
            
        }

        else
        {
            
            engine.Open(introTrack);
            timer.Start();
            engine.Play();

            while (!token.IsCancellationRequested)
            {
                // we're using a timer because both the StoppedPlayback event and checking WaveOutEvent.PlaybackState
                // have a slight delay
                if (timer.Elapsed > engine.Length)
                {
                    engine.Open(mainTrack);
                    engine.Play();
                    timer.Reset();
                }
            }
            
        }

        if (File.Exists(endTrack))
        {
            engine.Open(endTrack);
            engine.Play();

            while (engine.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(10);
            }
            
        }
        
        // cleanup!
    }
}