using Godot;

/// <summary>
/// Zone destroying balls if hit
/// </summary>
public partial class LoseZone : Area2D
{
	[Signal]
	public delegate void BallHitLoseZoneEventHandler(int ID);

	/// <summary>
	/// On ball hitting this zone
	/// </summary>
	/// <param name="body"></param>
	public void OnLoseZoneHit(Node2D body)
	{
		if (body is Ball ball)
		{
			ball.QueueFree();
			EmitSignal(SignalName.BallHitLoseZone, ball.ID);
			return;
		}

		if(body is Projectile projectile)
		{
			GD.Print("projectile hit lose zone");
			projectile.QueueFree();
			return;
		}
    }
}
