using Cassebrique.Domain.Bricks;

namespace Casse_brique.Domain.API
{
    /// <summary>
    /// Brick
    /// </summary>
    public class BrickDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// X position
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y position
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Brick type
        /// </summary>
        public BrickType BrickType { get; set; }
    }
}
