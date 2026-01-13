using Microsoft.Xna.Framework;
using FlappyIncremental.Dto;
using System;

namespace Application.Model.MenuElements.Base;

public class BaseElementModel
{
    public Rectangle Rectangle { get; set; } = new(0, 0, 100, 50);
    public Color Color { get; set; } = Color.Gray;
    public Color HoverColor => Color * 1.2f;
    public string Text { get; set; }
    public Action Click { get; set; }
    public bool IsHover { get; set; } = false;

    public void Draw(int x, int y)
    {
        Rectangle = new(x, y, Rectangle.Width, Rectangle.Height);

        GlobalVariables.SpriteBatchInterface.Draw(GlobalVariables.Pixel, new Rectangle(x, y, Rectangle.Width, Rectangle.Height), IsHover ? HoverColor : Color);
        if (!string.IsNullOrEmpty(Text))
        {
            var textSize = GlobalVariables.Font.MeasureString(Text);
            var textPosition = new Vector2(
                x + (Rectangle.Width - textSize.X) / 2,
                y + (Rectangle.Height - textSize.Y) / 2);
            GlobalVariables.SpriteBatchInterface.DrawString(GlobalVariables.Font, Text, textPosition, Color.Black);
        }
    }

    public void Draw()
    {
        Draw(Rectangle.X, Rectangle.Y);
    }
}
