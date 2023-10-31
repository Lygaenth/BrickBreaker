using Casse_brique.Domain.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casse_brique.Tests.Domain
{
    public class TestSoundManager
    {
        [Test]
        public void TestSetSoundOnOff()
        {
            var soundManager = new SoundManager();

            Assert.IsTrue(soundManager.SoundOn);
            soundManager.SetSoundOnOff(false);
            Assert.IsFalse(soundManager.SoundOn);

            soundManager.SetSoundOnOff(true);
            Assert.IsTrue(soundManager.SoundOn);
        }


        [Test]
        public void TestSetVolume()
        {
            var soundManager = new SoundManager();

            Assert.That(soundManager.VolumePercent, Is.EqualTo(100));
            Assert.That(soundManager.VolumeDB, Is.EqualTo(0));

            soundManager.SetVolumePerCent(50);
            Assert.That(soundManager.VolumePercent, Is.EqualTo(50));
            Assert.That((int)soundManager.VolumeDB, Is.EqualTo((int)-6));

            soundManager.SetVolumePerCent(0);
            Assert.That(soundManager.VolumePercent, Is.EqualTo(0));
            Assert.That((int)soundManager.VolumeDB, Is.EqualTo((int)-2147483648));
        }
    }
}
