using System.Diagnostics;
using Bassoon;
using NAudio.Wave;

namespace YuzuRPG.Core.Audio;

public class BassoonAudioManager : IAudioManager
{
    private CancellationTokenSource musicTokenSource;
    private Thread musicThread;
    public float Volume;
    
    public BassoonAudioManager(float volume)
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
            using (BassoonEngine be = new BassoonEngine())
            {
                Sound snd = new Sound(mainTrack);
                snd.Volume = Volume;
                snd.Play();
                while (!token.IsCancellationRequested)
                {
                    Thread.Sleep(10);
                }
            }
        }

        else
        {
            // doing all these using calls so I don't have to manually dispose later lol

            using (BassoonEngine be = new BassoonEngine())
            {
                bool playingIntro = true;
                var introSnd = new Sound(introTrack);
                var snd = new Sound(mainTrack);
                introSnd.Volume = Volume;
                snd.Volume = Volume;
                introSnd.Play();
                timer.Start();

                while (!token.IsCancellationRequested)
                {
                    // we're using a timer because both the StoppedPlayback event and checking WaveOutEvent.PlaybackState
                    // have a slight delay
                    if (timer.Elapsed > introSnd.Duration && playingIntro)
                    {
                        snd.Play();
                        timer.Reset();
                        playingIntro = false;
                    }

                    if (timer.Elapsed > snd.Duration && !playingIntro)
                    {
                        snd.Play();
                        timer.Reset();
                    }
                }
            
            }
        }

        if (File.Exists(endTrack))
        {
            using (BassoonEngine be = new BassoonEngine())
            {
                Sound snd = new Sound(endTrack);
                snd.Volume = Volume;
                snd.Play();
                while (!token.IsCancellationRequested)
                {
                    Thread.Sleep(10);
                }
            }
        }
        
        // cleanup!
    }
}