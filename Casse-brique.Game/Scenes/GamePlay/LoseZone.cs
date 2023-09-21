using Godot;

/// <summary>
/// Zone destroying balls if hit
/// </summary>
public partial class LoseZone : Area2D
{
	[Signal]
	public delegate void BallHitLoseZoneEventHandler();

	/// <summary>
	/// On ball hitting this zone
	/// </summary>
	/// <param name="body"></param>
	public void OnLoseZoneHit(Node2D body)
	{
        if (body is not Ball ball)
            return;

		ball.QueueFree();
		EmitSignal(SignalName.BallHitLoseZone);
    }
}
