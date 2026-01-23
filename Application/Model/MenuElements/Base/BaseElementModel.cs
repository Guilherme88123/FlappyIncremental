using FlappyIncremental.Dto;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Application.Model.MenuElements.Base;

public class BaseElementModel
{
    public Rectangle Rectangle { get; set; } = new(0, 0, 100, 50);
    public Color Color { get; set; } = Color.Red;
    public string Text { get; set; }

    public bool IsHover { get; set; } = false;
    public Color HoverColor => Color * 0.7f;

    public Action Click { get; set; }

    public const float Delay = 0.3f;
    public float DelayAtual { get; set; }

    public Texture2D Overlay { get; set; } = null;

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
        if (Overlay != null)
        {
            DrawOverlay();
        }
        else
        {
            DrawRectangle();
        }

        if (!string.IsNullOrEmpty(Text))
        {
            DrawText();
        }
    }

    public void DrawText()
    {
        var textSize = GlobalVariables.Font.MeasureString(Text);
        var textPosition = new Vector2(
            Rectangle.X + (Rectangle.Width - textSize.X) / 2,
            Rectangle.Y + (Rectangle.Height - textSize.Y) / 2);
        GlobalVariables.SpriteBatchInterface.DrawString(GlobalVariables.Font, Text, textPosition, Color.White);
    }

    public void DrawRectangle()
    {
        GlobalVariables.SpriteBatchInterface.Draw(GlobalVariables.Pixel, Rectangle, IsHover ? HoverColor : Color);
    }

    public void DrawOverlay()
    {
        var scaleX = (float)Rectangle.Width / (float)Overlay.Width;
        var scaleY = (float)Rectangle.Height / (float)Overlay.Height;

        var position = new Vector2(Rectangle.X, Rectangle.Y);

        GlobalVariables.SpriteBatchEntities.Draw(
            Overlay,
            position,
            null,
            IsHover ? HoverColor : Color,
            0f,
            new Vector2(0.5f, 0.5f),
            new Vector2(scaleX, scaleY),
            SpriteEffects.None,
            0f);
    }
}
