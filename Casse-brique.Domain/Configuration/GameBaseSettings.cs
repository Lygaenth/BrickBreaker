using Godot;

namespace Casse_brique.Domain.Configuration
{
    public class GameBaseSettings : Control
    {
        protected bool _changed;
        public bool Changed { get => _changed; }

        public virtual void CancelChanges()
        {

        }
    }
}
