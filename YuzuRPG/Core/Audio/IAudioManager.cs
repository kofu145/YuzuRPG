using System;
namespace YuzuRPG.Core.Audio
{
	public interface IAudioManager
	{
        public void PlayMusic(string audioTrack);
        public void StopMusic();

    }
}

