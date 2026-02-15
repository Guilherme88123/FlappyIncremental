using Application.Dto;
using Application.Model.Upgrade.Definition.Base;

namespace Application.Model.Upgrade.Definition.Health;

public class HealthUpgradeModel : BaseUpgradeModel
{
    public HealthUpgradeModel()
    {
        Id = 1;
        Name = "Stronger";
        Description = "Make you Bigger and Stronger! \n(Increment your Health)";
        MaxLevel = 5;
        BasePrice = 10;
        PriceFactor = 1.3m;
        XPosition = 1;
        YPosition = 0;
    }

    public override void Apply(int newLevel, int oldLevel)
    {
        var levelChange = newLevel - oldLevel;
        GlobalStatus.MaxHealth += levelChange * 20;
    }
}
