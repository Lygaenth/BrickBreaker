using Godot;

namespace Cassebrique.Factory
{
    public interface IBallFactory
    {
        Ball CreateBall(Vector2 position);
    }
}