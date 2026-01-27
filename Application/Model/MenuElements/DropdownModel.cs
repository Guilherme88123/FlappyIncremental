using Application.Dto;
using Application.Model.MenuElements.Base;
using FlappyIncremental.Dto;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Application.Model.MenuElements;

public class DropdownModel : BaseElementModel
{
    public bool IsOpen { get; set; }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        var mouse = Mouse.GetState();

        if (mouse.LeftButton == ButtonState.Pressed &&
            DelayAtual < 0 &&
            !GlobalVariables.IsMouseDown &&
            IsHover)
        {
            ClickSound.Play(GlobalOptions.SfxVolumeFloat, 0f, 0f);
            ToggleOpen();
            DelayAtual = Delay;
        }
    }

    private void ToggleOpen()
    {
        IsOpen = !IsOpen;
    }

    public override void Draw()
    {
        base.Draw();

        if (IsOpen)
        {
            DrawDropdownOverlay();
            DrawDropdownItems();
        }
    }

    private void DrawDropdownItems()
    {
    }

    private void DrawDropdownOverlay()
    {
        var Rec = new Rectangle(Rectangle.X, Rectangle.Y + Rectangle.Height, Rectangle.Width, Rectangle.Height * 3);
        GlobalVariables.SpriteBatchInterface.Draw(Overlay, Rec, Color.White);
    }
}
