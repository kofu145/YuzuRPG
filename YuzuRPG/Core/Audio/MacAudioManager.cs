using System;
using System.Diagnostics;
using Pie.Audio;

namespace YuzuRPG.Core.Audio
{
	public class MacAudioManager : IAudioManager, IDisposable
	{ 
        private CancellationTokenSource musicTokenSource;
        private Thread musicThread;
        public float Volume;
        private AudioDevice device;
        
        public MacAudioManager(float volume)
        {
            Volume = volume / 100;
            device = new AudioDevice(48000, 256);
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

                PCM pcm = PCM.LoadWav(mainTrack);
                AudioBuffer buffer = device.CreateBuffer(new BufferDescription(DataType.Pcm, pcm.Format), pcm.Data);

                device.PlayBuffer(buffer, 0, new ChannelProperties(volume: Volume, speed: 1.0, looping: false));
                
                while (!token.IsCancellationRequested)
                {
                    Thread.Sleep(10);
                }
                device.DeleteBuffer(buffer);

            }

            else
            {

                PCM introPCM = PCM.LoadWav(introTrack);
                PCM mainPCM = PCM.LoadWav(mainTrack);

                AudioBuffer introBuffer = device.CreateBuffer(new BufferDescription(DataType.Pcm, introPCM.Format), introPCM.Data);
                AudioBuffer mainBuffer = device.CreateBuffer(new BufferDescription(DataType.Pcm, mainPCM.Format), mainPCM.Data);
                device.PlayBuffer(introBuffer, 0, new ChannelProperties(volume: Volume, speed: 1.0, looping: false));
                device.QueueBuffer(mainBuffer, 0);

                device.BufferFinished += (system, channel, buffer) =>
                    system.SetChannelProperties(channel, new ChannelProperties(volume: Volume, speed: 1.0, looping: false));
                while (!token.IsCancellationRequested)
                {
                    Thread.Sleep(10);
                }
                device.DeleteBuffer(introBuffer);
                device.DeleteBuffer(mainBuffer);

            }


            if (File.Exists(endTrack))
            {
                PCM pcm = PCM.LoadWav(mainTrack);
                AudioBuffer buffer = device.CreateBuffer(new BufferDescription(DataType.Pcm, pcm.Format), pcm.Data);

                device.PlayBuffer(buffer, 0, new ChannelProperties(volume: Volume, speed: 1.0, looping: false));
                
                while (device.IsPlaying(0))
                {
                    Thread.Sleep(10);
                }
                device.DeleteBuffer(buffer);
            }
            
            // cleanup!
        }

        public void Dispose()
        {
            device.Dispose();
        }
	}
}

