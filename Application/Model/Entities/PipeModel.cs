using FlappyIncremental.Dto;
using FlappyIncremental.Model.Entities.Base;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Numerics;

namespace Application.Model.Entities;

public class PipeModel : BaseEntityModel
{
    public bool HasScored { get; set; }
    public bool IsTop { get; set; }

    public PipeModel((float x, float y) position) : base(position)
    {
        Sprite = GlobalVariables.Game.Content.Load<Texture2D>("pipe");
        Size = new Vector2(80, 800);
        MaxSpeed = 300f;
    }

    public override void Update(Microsoft.Xna.Framework.GameTime gameTime, List<BaseEntityModel> entities)
    {
        base.Update(gameTime, entities);

        Position += new Vector2(-MaxSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);

        if (Position.X + Size.X < 0)
        {
            IsDestroyed = true;
        }
    }

    public override void Draw()
    {
        var scaleX = Size.X / Sprite.Width;
        var scaleY = Size.Y / Sprite.Height;

        GlobalVariables.SpriteBatchEntities.Draw(
            Sprite,
            Position,
            null,
            Color,
            0f,
            new System.Numerics.Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            IsTop ? SpriteEffects.FlipVertically : SpriteEffects.None,
            0f);
        GlobalVariables.SpriteBatchEntities.Draw(GlobalVariables.Pixel, Rectangle, Color);
    }
}
