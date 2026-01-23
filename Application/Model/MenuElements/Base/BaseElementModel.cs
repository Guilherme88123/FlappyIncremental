using FlappyIncremental.Dto;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Application.Model.MenuElements.Base;

public class BaseElementModel
{
    public Rectangle Rectangle { get; set; } = new(0, 0, 100, 50);
    public Color Color { get; set; } = Color.Red;
    public string Text { get; set; }

    public bool IsHover { get; set; } = false;
    public Color HoverColor => Color * 0.1f;

    public Action Click { get; set; }

    public const float Delay = 0.3f;
    public float DelayAtual { get; set; }

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
            GlobalVariables.SpriteBatchInterface.DrawString(GlobalVariables.Font, Text, textPosition, Color.White);
        }
    }

    public void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();
        var mousePos = new Point(mouse.X, mouse.Y);

        DelayAtual -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        var hovering = Rectangle.Contains(mousePos);
        if (hovering)
        {
            IsHover = true;

            if (mouse.LeftButton == ButtonState.Pressed && DelayAtual < 0)
            {
                Click?.Invoke();
                DelayAtual = Delay;
            }
        }
        else
        {
            IsHover = false;
        }
    }

    public void Draw()
    {
        Draw(Rectangle.X, Rectangle.Y);
    }
}
