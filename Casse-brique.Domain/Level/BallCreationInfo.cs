using Godot;

namespace Casse_brique.Domain.Level
{
    public class BallCreationInfo
    {
        public int ID { get; }
        public Vector2 Position { get; }
        public Vector2 InitialVelocity { get; }

        public BallCreationInfo(int id, Vector2 position, Vector2 initialPosition)
        {
            ID = id;
            Position = position;
            InitialVelocity = initialPosition;
        }
    }
}
