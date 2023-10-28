using Casse_brique.Domain;
using Cassebrique.Locators;

namespace Cassebrique.Factory
{
    public class BossFactory : IBossFactory
    {
        public Boss CreateBoss(BossModel model)
        {
            var boss = PackedSceneLocator.GetScene<Boss>(model.BossName);
            boss.Initialize(model);
            return boss;
        }
    }
}
