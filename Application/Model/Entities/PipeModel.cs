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
        Size = new Vector2(100, 800);
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
        DrawEffect = IsTop ? SpriteEffects.FlipVertically : SpriteEffects.None;

        base.Draw();
    }
}
