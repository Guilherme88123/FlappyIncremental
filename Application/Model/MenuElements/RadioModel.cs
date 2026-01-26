using Application.Dto;
using Application.Model.MenuElements.Base;
using FlappyIncremental.Dto;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading.Tasks.Sources;

namespace Application.Model.MenuElements;

public class RadioModel : BaseElementModel
{
    public int Max { get; set; } = 100;
    public int Min { get; set; } = 0;

    public int Value { get; set; } = 50;

    public Texture2D DotOverlay { get; set; } = null;

    public Rectangle DotRectangle { get; set; }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        UpdateDotRectangle();

        var mouse = Mouse.GetState();
        var mousePos = new Point(mouse.X, mouse.Y);

        var isDotHover = DotRectangle.Contains(mousePos);

        if (mouse.LeftButton == ButtonState.Pressed &&
            DelayAtual < 0 &&
            !GlobalVariables.IsMouseDown &&
            isDotHover)
        {
            ClickSound.Play(GlobalOptions.SfxVolume, 0f, 0f);
            Value++;
            DelayAtual = Delay;
        }
    }

    private void UpdateDotRectangle()
    {
        var dotSize = Rectangle.Height / 4;
        var percent = (float)(Value - Min) / (Max - Min);
        var dotX = Rectangle.X + (int)((Rectangle.Width - dotSize) * percent);
        var dotY = Rectangle.Y + Rectangle.Height - dotSize - 20;

        DotRectangle = new Rectangle(dotX, dotY, dotSize, dotSize);
    }

    public override void Draw()
    {
        base.Draw();

        DrawLine();

        if (DotOverlay is not null)
        {
            DrawDot();
        }
    }

    protected void DrawDot()
    {
        GlobalVariables.SpriteBatchInterface.Draw(DotOverlay, DotRectangle, Color.White);
    }

    protected void DrawLine()
    {
        var lineHeight = Rectangle.Height / 10;
        var lineY = Rectangle.Y + Rectangle.Height - lineHeight - 30;
        var lineRect = new Rectangle(Rectangle.X + 40, lineY, Rectangle.Width - 80, lineHeight);
        GlobalVariables.SpriteBatchInterface.Draw(GlobalVariables.Pixel, lineRect, Color.DarkGray);
    }

    protected override string GetText()
    {
        return $"{Text}: {Value}";
    }
}
