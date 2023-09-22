using Cassebrique;
using Godot;

/// <summary>
/// Frame
/// </summary>
public partial class Frame : Node2D
{
	[Export]
	FramePositionType FrameType { get; set; }

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
			var velocity = ball.LinearVelocity;
			switch(FrameType)
			{
				case FramePositionType.Top:					
					velocity.Y *= -1;
					break;
				case FramePositionType.Side:
					velocity.X = velocity.Normalized().X * -1 * ball.Speed;
					break;
			}
			ball.Bounce(true);
			var vector = velocity.Normalized();
			ball.LinearVelocity = vector* ball.Speed;
		}
	}
}
