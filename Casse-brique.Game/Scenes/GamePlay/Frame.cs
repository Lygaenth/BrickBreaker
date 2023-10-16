using Cassebrique;
using Godot;

/// <summary>
/// Frame
/// </summary>
public partial class Frame : Node2D
{
	[Export]
	AxisBounce FrameType { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}


    /// <summary>
    /// 
    /// </summary>
    /// <param name="body"></param>
    private void OnArea2dBodyEntered(Node2D body)
	{
		if (body is Ball ball)
		{
			ball.Bounce(true, -1, FrameType);
		}
	}
}
