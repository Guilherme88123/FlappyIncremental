using Application.Interface.Upgrade;
using Application.Model.Upgrade.Definition.Base;
using Application.Model.Upgrade.State;
using FlappyIncremental.Dto;

namespace Application.Model.MenuElements.Button.UpgradeButtonModel;

public class UpgradeButtonModel : ButtonModel
{
    public BaseUpgradeModel Upgrade { get; set; }
    public UpgradeStateModel State { get; set; }
    public int Price { get; set; }

    public UpgradeButtonModel(BaseUpgradeModel upgrade)
    {
        Upgrade = upgrade;

        Click += BuyClick;

        Reload();
    }

    private void BuyClick()
    {
        var manager = GlobalVariables.GetService<IUpgradeService>();
        manager.TryBuy(Upgrade.Id);

        Reload();
    }

    private void Reload()
    {
        var manager = GlobalVariables.GetService<IUpgradeService>();
        State = manager.GetState(Upgrade.Id);
        Price = manager.GetPrice(Upgrade.Id);
    }

    protected override string GetText()
    {
        return $"{Upgrade.Name} \n${Price}";
    }
}
