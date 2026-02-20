using Application.Interface.Upgrade;
using Application.Model.Upgrade.Definition.Base;
using Application.Model.Upgrade.State;
using FlappyIncremental.Dto;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;

namespace Application.Model.MenuElements.Button.UpgradeButtonModel;

public class UpgradeButtonModel : ButtonModel
{
    public BaseUpgradeModel Upgrade { get; set; }
    public UpgradeStateModel State { get; set; }
    public int Price { get; set; }

    public Texture2D HoverOverlay { get; set; } = null;
    public Rectangle HoverOverlayRect { get; set; }

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

    protected override bool IsHovering()
    {
        var mouse = Mouse.GetState();
        Vector2 mouseScreen = new(mouse.X, mouse.Y);

        Matrix inv = Matrix.Invert(GlobalVariables.CameraOffset);
        Vector2 mouseWorld = Vector2.Transform(mouseScreen, inv);

        return Rectangle.Contains(mouseWorld);
    }

    protected override string GetText() => Upgrade.Name;

    protected override void DrawText(string text)
    {
        SpriteBatch = SpriteBatch is null ? GlobalVariables.SpriteBatchInterface : SpriteBatch;

        var upgradeName = text;
        var cost = State.ActualLevel == Upgrade.MaxLevel ? "Max" : $"${Price}";

        var upgradeNameSize = GlobalVariables.LittleFont.MeasureString(upgradeName);
        var costSize = GlobalVariables.LittleFont.MeasureString(cost);

        var upgradeNamePosition = new Vector2(
            Rectangle.X + Rectangle.Width / 2 - upgradeNameSize.X / 2,
            Rectangle.Y + Rectangle.Height / 3 - upgradeNameSize.Y / 2);

        var costSizePosition = new Vector2(
            Rectangle.X + Rectangle.Width / 2 - costSize.X / 2,
            Rectangle.Y + (Rectangle.Height / 3) * 2 - costSize.Y / 2);

        SpriteBatch.DrawString(GlobalVariables.LittleFont, upgradeName, upgradeNamePosition, Color.White);
        SpriteBatch.DrawString(GlobalVariables.LittleFont, cost, costSizePosition, Color.White);
    }

    public override void Draw()
    {
        base.Draw();

        if (IsHover && HoverOverlay is not null) DrawHover();
    }

    private void DrawHover()
    {
        DrawHoverOverlay();
        DrawHoverText();
    }

    private void DrawHoverOverlay()
    {
        SpriteBatch = SpriteBatch is null ? GlobalVariables.SpriteBatchInterface : SpriteBatch;

        var width = Rectangle.Width * 5;
        var height = Rectangle.Height * 3;
        var x = Rectangle.X + Rectangle.Width / 2 - width / 2;
        var y = Rectangle.Y - height * 1.01;

        HoverOverlayRect = new(x, (int)y, width, height);

        var scaleX = (float)width / HoverOverlay.Width;
        var scaleY = (float)height / HoverOverlay.Height;

        SpriteBatch.Draw(
            HoverOverlay,
            new(x, (float)y),
            null,
            Color.White,
            0f,
            new Vector2(0.5f, 0.5f),
            new Vector2(scaleX, scaleY),
            SpriteEffects.None,
            0f);
    }

    private void DrawHoverText()
    {
        SpriteBatch = SpriteBatch is null ? GlobalVariables.SpriteBatchInterface : SpriteBatch;

        var upgradeName = Upgrade.Name;
        var upgradeDescription = Upgrade.Description;
        var cost = State.ActualLevel == Upgrade.MaxLevel ? "Max" : $"${Price}";
        var level = $"{State.ActualLevel}/{Upgrade.MaxLevel}";

        var upgradeNameSize = GlobalVariables.DefaultFont.MeasureString(upgradeName);
        var upgradeDescriptionSize = GlobalVariables.DefaultFont.MeasureString(upgradeDescription);
        var costSize = GlobalVariables.DefaultFont.MeasureString(cost);
        var levelSize = GlobalVariables.DefaultFont.MeasureString(level);

        var border = 20;

        var upgradeNamePosition = new Vector2(
            HoverOverlayRect.X + HoverOverlayRect.Width / 2 - upgradeNameSize.X / 2,
            HoverOverlayRect.Y + border);

        var upgradeDescriptionPosition = new Vector2(
            HoverOverlayRect.X + border * 2,
            upgradeNamePosition.Y + upgradeNameSize.Y + border);

        var costPosition = new Vector2(
            HoverOverlayRect.X + border * 2,
            HoverOverlayRect.Y + HoverOverlayRect.Height - (costSize.Y + border));

        var levelPosition = new Vector2(
            HoverOverlayRect.X + HoverOverlayRect.Width - (levelSize.X + border * 2),
            HoverOverlayRect.Y + HoverOverlayRect.Height - (levelSize.Y + border));

        SpriteBatch.DrawString(GlobalVariables.DefaultFont, upgradeName, upgradeNamePosition, Color.White);
        SpriteBatch.DrawString(GlobalVariables.DefaultFont, upgradeDescription, upgradeDescriptionPosition, Color.White);
        SpriteBatch.DrawString(GlobalVariables.DefaultFont, cost, costPosition, Color.White);
        SpriteBatch.DrawString(GlobalVariables.DefaultFont, level, levelPosition, Color.White);
    }
}
