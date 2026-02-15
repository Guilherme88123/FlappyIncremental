using Application.Dto;
using Application.Model.Upgrade.Definition.Base;

namespace Application.Model.Upgrade.Definition.Score;

public class ScoreUpgradeModel : BaseUpgradeModel
{
    public ScoreUpgradeModel()
    {
        Id = 0;
        Name = "Score++";
        Description = "Your first upgrade! \nThis increment your score gain on Game :)";
        MaxLevel = 10;
        BasePrice = 5;
        PriceFactor = 1.5m;
        XPosition = 0;
        YPosition = 0;
    }

    public override void Apply(int newLevel, int oldLevel)
    {
        var levelChange = newLevel - oldLevel;
        GlobalStatus.ScoreGain += levelChange;
    }
}
