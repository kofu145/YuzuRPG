using CSCore;
using CSCore.Codecs;
using CSCore.Codecs.WAV;
using CSCore.SoundOut;

namespace YuzuRPG.Core.Audio;
// mostly ripped from https://github.com/filoe/cscore/blob/master/Samples/AudioPlayerSample/MusicPlayer.cs as a reference

public class CSCoreMusicPlayer
{
    private ISoundOut _soundOut;
    private IWaveSource _waveSource;

	public event EventHandler<PlaybackStoppedEventArgs> PlaybackStopped;

    public PlaybackState PlaybackState
    {
        get
        {
            if (_soundOut != null)
                return _soundOut.PlaybackState;
            return PlaybackState.Stopped;
        }
    }

    public TimeSpan Position
    {
        get
        {
            if (_waveSource != null)
                return _waveSource.GetPosition();
            return TimeSpan.Zero;
        }
        set
        {
            if (_waveSource != null)
                _waveSource.SetPosition(value);
        }
    }

    public TimeSpan Length
    {
        get
        {
            if (_waveSource != null)
                return _waveSource.GetLength();
            return TimeSpan.Zero;
        }
    }

    public int Volume
    {
        get
        {
            if (_soundOut != null)
                return Math.Min(100, Math.Max((int)(_soundOut.Volume * 100), 0));
            return 100;
        }
        set
        {
            if (_soundOut != null)
            {
                _soundOut.Volume = Math.Min(1.0f, Math.Max(value / 100f, 0f));
            }
        }
    }

    public void Open(string filename)
    {
        CleanupPlayback();

        _waveSource = new WaveFileReader(filename);
        _soundOut = new WasapiOut() {Latency = 100};
        _soundOut.Initialize(_waveSource);
		if (PlaybackStopped != null) _soundOut.Stopped += PlaybackStopped;
    }

    public void Play()
    {
        if (_soundOut != null)
            _soundOut.Play();
    }

    public void Pause()
    {
        if (_soundOut != null)
            _soundOut.Pause();
    }

    public void Stop()
    {
        if (_soundOut != null)
            _soundOut.Stop();
    }

    private void CleanupPlayback()
    {
        if (_soundOut != null)
        {
            _soundOut.Dispose();
            _soundOut = null;
        }
        if (_waveSource != null)
        {
            _waveSource.Dispose();
            _waveSource = null;
        }
    }

    protected void Dispose(bool disposing)
    {
        CleanupPlayback();
    }
    
}