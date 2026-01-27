using Application.Dto;
using Application.Model.MenuElements.Base;
using FlappyIncremental.Dto;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading.Tasks.Sources;

namespace Application.Model.MenuElements;

public class RadioModel : BaseElementModel
{
    public int Max { get; set; } = 100;
    public int Min { get; set; } = 0;

    public int Value { get; set; } = 50;
    public Action<int> ValueUpdate { get; set; }

    public Texture2D DotOverlay { get; set; } = null;

    public Rectangle DotRectangle { get; set; }
    public Rectangle LineRectangle { get; set; }
    public bool IsDotPressed { get; set; }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        UpdateLineRectangle();
        UpdateDotRectangle();

        var mouse = Mouse.GetState();
        var mousePos = new Point(mouse.X, mouse.Y);

        var isDotHover = DotRectangle.Contains(mousePos);
        var isLineHover = LineRectangle.Contains(mousePos);

        var oldValue = Value;

        if (IsDotPressed)
        {
            int dotSize = Rectangle.Height / 4;
            int left = LineRectangle.X;
            int right = LineRectangle.Right - dotSize;

            int clampedX = Math.Clamp(mouse.X, left, right);

            float percent = (clampedX - left) / (float)(right - left);
            Value = (int)(Min + percent * (Max - Min));
        }

        if (Value != oldValue)
        {
            ValueUpdate?.Invoke(Value);
        }

        if (mouse.LeftButton == ButtonState.Pressed && (isDotHover || isLineHover))
        {
            if (!IsDotPressed) ClickSound.Play(GlobalOptions.SfxVolumeFloat, 0f, 0f);
            IsDotPressed = true;
        }
        else if (mouse.LeftButton == ButtonState.Released)
        {
            IsDotPressed = false;
        }
    }

    private void UpdateDotRectangle()
    {
        int dotSize = Rectangle.Height / 4;
        float percent = (float)(Value - Min) / (Max - Min);

        int left = LineRectangle.X;
        int right = LineRectangle.Right - dotSize;

        int dotX = (int)MathHelper.Lerp(left, right, percent);
        int dotY = Rectangle.Y + Rectangle.Height - dotSize - 20;

        DotRectangle = new Rectangle(dotX, dotY, dotSize, dotSize);
    }

    private void UpdateLineRectangle()
    {
        var lineHeight = Rectangle.Height / 8;
        var lineY = Rectangle.Y + Rectangle.Height - lineHeight - 30;
        LineRectangle = new Rectangle(Rectangle.X + 40, lineY, Rectangle.Width - 80, lineHeight);
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
        GlobalVariables.SpriteBatchInterface.Draw(GlobalVariables.Pixel, LineRectangle, Color.DarkGray);
    }

    protected override string GetText()
    {
        return $"{Text}: {Value}";
    }
}
