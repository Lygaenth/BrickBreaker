using Cassebrique.Enums;
using Cassebrique.Locators;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cassebrique.Factory
{
    /// <summary>
    /// Factory for projectiles, random only for now
    /// </summary>
    public class ProjectileFactory : IProjectileFactory
    {
        private readonly List<ProjectileTypes> _projectileTypes;

        public ProjectileFactory()
        {
            _projectileTypes = Enum.GetValues(typeof(ProjectileTypes)).Cast<ProjectileTypes>().ToList();
        }

        public Projectile GetRandomProjectile()
        {
            var index = GD.RandRange(0, _projectileTypes.Count - 1);
            var projectile = PackedSceneLocator.GetScene<Projectile>(_projectileTypes[index].GetHashCode().ToString());
            return projectile;
        }
    }
}
