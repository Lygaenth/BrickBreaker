using Casse_brique.Domain;
using Cassebrique.Locators;
using Godot;

namespace Cassebrique.Factory
{
    public class BallFactory : IBallFactory
    {
        public Ball CreateBall(int id, Vector2 position)
        {
            var ball = PackedSceneLocator.GetScene<Ball>();
            ball.Scale = new Vector2(0.3f, 0.3f);
            ball.GlobalPosition = position;
            var ballModel = new BallModel(id);
            ball.Setup(ballModel);
            return ball;
        }
    }
}
