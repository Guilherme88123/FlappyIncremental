using System;

namespace Application.Model.Upgrade.Definition.Base;

public abstract class BaseUpgradeModel
{
    public int Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }

    public int MaxLevel { get; set; }

    public int BasePrice { get; set; }
    public decimal PriceFactor { get; set; }

    public int XPosition { get; set; }
    public int YPosition { get; set; }

    public abstract void Apply(int newLevel, int oldLevel);
}
