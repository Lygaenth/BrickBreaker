using Casse_brique.Domain;

namespace Cassebrique.Factory
{
    public interface IBossFactory
    {
        Boss CreateBoss(BossModel model);
    }
}