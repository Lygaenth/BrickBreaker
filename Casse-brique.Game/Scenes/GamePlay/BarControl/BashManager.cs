using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cassebrique.Scenes.GamePlay.BarControl
{
    public class BashManager
    {
        private readonly Timer _bashEffectTimer;
        private readonly Timer _bashCdTimer;
        private readonly AudioStreamPlayer _bashSound;
        private readonly Sprite2D _bashSprite;

        private bool _isBashing;
        private bool _isBashInCoolDown;

        public BashManager(Timer bashEffectTimer, Timer bashCdTimer, AudioStreamPlayer bashSound, Sprite2D bashSprite)
        {
            _bashEffectTimer = bashEffectTimer;
            _bashCdTimer = bashCdTimer;
            _bashSound = bashSound;
            _bashSprite = bashSprite;

            _bashEffectTimer.Timeout += OnBashTimeout;
            _bashCdTimer.Timeout += OnBashCdTimeout;
        }

        private void OnBashCdTimeout()
        {
            _isBashInCoolDown = false;
            _bashSprite.Show();
        }

        private void OnBashTimeout()
        {
            _isBashing = false;
        }

        public bool CanBash()
        {
            return !_isBashInCoolDown;
        }

        public void Bash()
        {
            _isBashing = true;
            _isBashInCoolDown = true;
            _bashSound.Play();
            _bashEffectTimer.Start();
            _bashCdTimer.Start();
            _bashSprite.Hide();
        }

        public bool IsBashing()
        {
            return _isBashing;
        }

        public bool IsRecovering()
        {
            return !_isBashing && _isBashInCoolDown;
        }
    }
}
