using Godot;

namespace Mutemaanpa;

public partial class MusicPlayer : AudioStreamPlayer
{
    private double waitTime = 0;
    private float fromPosition = 0;
    private bool setWaitTime = false;

    public string Status = "";

    public void PlayDelayed(double waitTime, float fromPosition = 0)
    {
        this.waitTime = waitTime;
        this.fromPosition = fromPosition;
        setWaitTime = true;
    }

    public override void _Process(double delta)
    {
        if (!Playing && setWaitTime)
        {
            waitTime -= delta;
            if (waitTime <= 0)
            {
                setWaitTime = false;
                Play(fromPosition);
            }
        }
    }

    public static MusicPlayer Of(Node node)
    {
        return Main.Get(node).MusicPlayer!;
    }

    public void StopMusic()
    {
        Stop();
        setWaitTime = false;
    }
};
