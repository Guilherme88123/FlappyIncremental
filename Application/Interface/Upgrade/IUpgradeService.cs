using Application.Model.Upgrade.State;

namespace Application.Interface.Upgrade;

public interface IUpgradeService
{
    int GetPrice(int upgradeId);
    UpgradeStateModel GetState(int upgradeId);
    bool TryBuy(int upgradeId);
}
