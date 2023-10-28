using Godot;

namespace Cassebrique.Factory
{
    public interface IBallFactory
    {
        Ball CreateBall(int id, Vector2 position);
    }
}