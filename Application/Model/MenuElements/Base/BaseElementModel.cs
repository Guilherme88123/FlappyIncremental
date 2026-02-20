using Application.Dto;
using FlappyIncremental.Dto;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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

    public const float Delay = 0.3f;
    public float DelayAtual { get; set; }

    public Texture2D Overlay { get; set; } = null;

    protected SoundEffect ClickSound { get; set; } = GlobalVariables.Game.Content.Load<SoundEffect>("button_click");

    public SpriteBatch SpriteBatch { get; set; }

    public virtual void Update(GameTime gameTime)
    {
        DelayAtual -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        IsHover = IsHovering();
    }

    protected virtual bool IsHovering()
    {
        var mouse = Mouse.GetState();
        Vector2 mouseScreen = new(mouse.X, mouse.Y);

        return Rectangle.Contains(mouseScreen);
    }

    public virtual void Draw()
    {
        if (Overlay != null)
        {
            DrawOverlay();
        }
        else
        {
            DrawRectangle();
        }

        var text = GetText();

        if (!string.IsNullOrEmpty(text))
        {
            DrawText(text);
        }
    }

    protected virtual void DrawText(string text)
    {
        SpriteBatch = SpriteBatch is null ? GlobalVariables.SpriteBatchInterface : SpriteBatch;

        var textSize = GlobalVariables.DefaultFont.MeasureString(text);
        var textPosition = new Vector2(
            Rectangle.X + (Rectangle.Width - textSize.X) / 2,
            Rectangle.Y + (Rectangle.Height - textSize.Y) / 2);
        SpriteBatch.DrawString(GlobalVariables.DefaultFont, text, textPosition, Color.White);
    }

    protected void DrawRectangle()
    {
        SpriteBatch = SpriteBatch is null ? GlobalVariables.SpriteBatchInterface : SpriteBatch;

        SpriteBatch.Draw(GlobalVariables.Pixel, Rectangle, IsHover ? HoverColor : Color);
    }

    protected void DrawOverlay()
    {
        SpriteBatch = SpriteBatch is null ? GlobalVariables.SpriteBatchInterface : SpriteBatch;

        var scaleX = (float)Rectangle.Width / (float)Overlay.Width;
        var scaleY = (float)Rectangle.Height / (float)Overlay.Height;

        var position = new Vector2(Rectangle.X, Rectangle.Y);

        SpriteBatch.Draw(
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

    protected virtual string GetText()
    {
        return Text;
    }
}
