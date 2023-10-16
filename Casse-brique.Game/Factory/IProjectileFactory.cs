namespace Cassebrique.Factory
{
    /// <summary>
    /// Projectile factory
    /// </summary>
    public interface IProjectileFactory
    {
        /// <summary>
        /// Return a random projectile
        /// </summary>
        /// <returns></returns>
        Projectile GetRandomProjectile();
    }
}