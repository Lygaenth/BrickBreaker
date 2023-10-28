﻿using Casse_brique.Domain;
using Cassebrique.Locators;
using Godot;

namespace Cassebrique.Factory
{
    public class BallFactory : IBallFactory
    {
        public Ball CreateBall(BallModel model, Vector2 position)
        {
            var ball = PackedSceneLocator.GetScene<Ball>();
            ball.Scale = new Vector2(0.3f, 0.3f);
            ball.GlobalPosition = position;
            ball.Setup(model);
            return ball;
        }
    }
}
