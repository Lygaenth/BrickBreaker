using Casse_brique.Domain.Constants;
using Casse_brique.Domain.Enums;
using Casse_brique.Domain.Level;
using Godot;

namespace Casse_brique.Domain
{
    public class BallModel
    {
        public int ID { get; }
        public Vector2 LinearVelocity { get; private set; }

        private int _bonus = 0;

        public event EventHandler<EventArgs> Impulsed;

        public event EventHandler<BallCreationInfo> Duplicated;

        public event EventHandler<int> Destroyed;

        public event EventHandler<int> Bounced;

        public int Bonus { get => _bonus; }

        private bool _isAttached;
        public bool IsAttached { get => _isAttached; }

        public BallModel(int id)
        {
            ID = id;
            _isAttached = true;
            _bonus = 0;
        }

        /// <summary>
        /// Bounce
        /// </summary>
        /// <param name="axisBounce"></param>
        /// <param name="bonusModifier"></param>
        /// <param name="offset"></param>
        public void Bounce(AxisBounce axisBounce, int bonusModifier, Vector2 offset)
        {
            var movement = GetBounceNormalizedVector(axisBounce, offset);

            movement = CheckBounceMinimalAngle(movement);

            LinearVelocity = movement * ConvertModifierToSpeed(bonusModifier);

            Bounced?.Invoke(this, _bonus);
            Impulsed?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Check that bounce is done with a minimal angle
        /// </summary>
        /// <param name="movement"></param>
        /// <returns></returns>
        private Vector2 CheckBounceMinimalAngle(Vector2 movement)
        {
            if (GetVectorsAbsAngle(movement, Vector2.Right) < Math.PI / 8 || GetVectorsAbsAngle(movement, Vector2.Left) < Math.PI / 8)
            {
                var vector = new Vector2(MathF.Cos(Mathf.Pi / 8), Mathf.Sin(Mathf.Pi / 8));
                if (movement.X < 0)
                    vector.X = -vector.X;

                if (movement.Y < 0)
                    vector.Y = -vector.Y;
                movement = vector;
            }

            return movement;
        }

        /// <summary>
        /// Get angle between two normalized vectors
        /// </summary>
        /// <param name="vector2"></param>
        /// <param name="refVector"></param>
        /// <returns></returns>
        private float GetVectorsAbsAngle(Vector2 vector2, Vector2 refVector)
        {
            return Mathf.Abs(Mathf.Acos(vector2.Dot(refVector)));
        }

        /// <summary>
        /// Generated bounce normalized vector
        /// </summary>
        /// <param name="axisBounce"></param>
        /// <returns></returns>
        private Vector2 GetBounceNormalizedVector(AxisBounce axisBounce, Vector2 offset)
        {
            var movement = (LinearVelocity + offset).Normalized();

            if (axisBounce == AxisBounce.X || axisBounce == AxisBounce.XY)
                movement.X = movement.X * -1;

            if (axisBounce == AxisBounce.Y || axisBounce == AxisBounce.XY)
                movement.Y = movement.Y * -1;
            return movement;
        }

        /// <summary>
        /// Transform modifier into speed
        /// </summary>
        /// <param name="bonusModifier"></param>
        /// <returns></returns>
        private float ConvertModifierToSpeed(int bonusModifier)
        {
            _bonus = Mathf.Clamp(_bonus+bonusModifier, 0, 5);
            return (100 + _bonus * 10) * GameConstants.BaseSpeed / 100;
        }

        /// <summary>
        /// Initial impulse on ball
        /// </summary>
        /// <param name="launchVector"></param>
        public void Launch(Vector2 launchVector)
        {
            _isAttached = false;
            Move(launchVector);
        }

        /// <summary>
        /// Move ball on bar
        /// </summary>
        /// <param name="movement"></param>
        public void Move(Vector2 movement)
        {
            LinearVelocity = movement * ConvertModifierToSpeed(0); ;
            Impulsed?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Duplicate ball
        /// </summary>
        public void Duplicate(Vector2 position)
        {
            var random = new Random(DateTime.Now.Millisecond);
            // Get random angle between Pi / 8 and Pi / 4
            var randombaseAngle = (float)((random.NextDouble() * Mathf.Pi / 8) + Mathf.Pi / 8);
            var angle = Mathf.RadToDeg(randombaseAngle);
            var randomSign = Mathf.Sign(random.NextDouble() - 0.5);
            var randomAngle = randomSign >= 0 ? randombaseAngle : -randombaseAngle;

            Duplicated?.Invoke(this, new BallCreationInfo(ID, position, new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle))));
        }

        /// <summary>
        /// Destroy ball
        /// </summary>
        public void Destroy()
        {
            Destroyed?.Invoke(this, ID);
        }
    }
}
