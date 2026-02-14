using System;
using System.Collections.Generic;
using System.Linq;
using Application.Dto;
using Application.Interface.Upgrade;
using Application.Model.Upgrade.Definition.Base;
using Application.Model.Upgrade.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Graphics;

namespace Application.Service.Upgrade;

public class UpgradeService : IUpgradeService
{
    private Dictionary<int, BaseUpgradeModel> UpgradeList { get; set; } = new();
    private Dictionary<int, UpgradeStateModel> StateList { get; set; } = new();

    public UpgradeService(IServiceProvider provider)
    {
        UpgradeList = provider.GetRequiredService<IEnumerable<BaseUpgradeModel>>().ToDictionary(x => x.Id);
    }

    public int GetPrice(int upgradeId)
    {
        var upgrade = UpgradeList[upgradeId];
        var state = GetState(upgradeId);

        double cost = upgrade.BasePrice * Math.Pow((double)upgrade.PriceFactor, state.ActualLevel - 1);
        return (int)Math.Ceiling(cost);
    }

    public UpgradeStateModel GetState(int upgradeId)
    {
        return StateList.TryGetValue(upgradeId, out var state) 
            ? state 
            : (StateList[upgradeId] = new UpgradeStateModel { UpgradeId = upgradeId });
    }

    public bool TryBuy(int upgradeId)
    {
        var upgrade = UpgradeList[upgradeId];
        var state = GetState(upgradeId);

        if (state.ActualLevel >= upgrade.MaxLevel) return false;

        int cost = GetPrice(upgradeId);
        if (GlobalStatus.TotalScore < cost) return false;

        GlobalStatus.TotalScore -= cost;

        int oldLevel = state.ActualLevel;
        state.ActualLevel++;

        upgrade.Apply(state.ActualLevel, oldLevel);

        return true;
    }
}
