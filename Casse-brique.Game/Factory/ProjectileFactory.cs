using Godot;
using System.Collections.Generic;

namespace Cassebrique.Factory
{
    /// <summary>
    /// Factory for projectiles, random only for now
    /// </summary>
    public class ProjectileFactory : IProjectileFactory
    {
        PackedScene _violonPackedScene;
        PackedScene _pianoPackedScene;
        PackedScene _clarinettePackedScene;

        List<PackedScene> _scenes;

        public ProjectileFactory()
        {
            _violonPackedScene = ResourceLoader.Load<PackedScene>("res://Scenes/GamePlay/Projectiles/Violon.tscn");
            _pianoPackedScene = ResourceLoader.Load<PackedScene>("res://Scenes/GamePlay/Projectiles/Piano.tscn");
            _clarinettePackedScene = ResourceLoader.Load<PackedScene>("res://Scenes/GamePlay/Projectiles/Clarinette.tscn");

            _scenes = new List<PackedScene>
            {
                _violonPackedScene,
                _pianoPackedScene,
                _clarinettePackedScene
            };
        }

        public Projectile GetRandomProjectile()
        {
            var index = GD.RandRange(0, _scenes.Count - 1);
            var projectile = _scenes[index].Instantiate<Projectile>();
            return projectile;
        }
    }
}
