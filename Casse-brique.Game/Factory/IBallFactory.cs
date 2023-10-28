using Casse_brique.Domain;
using Godot;

namespace Cassebrique.Factory
{
    public interface IBallFactory
    {
        Ball CreateBall(BallModel model, Vector2 position);
    }
}