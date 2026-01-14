using Microsoft.Xna.Framework;
using FlappyIncremental.Dto;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace FlappyIncremental.Model.Entities.Base;

public abstract class BaseEntityModel
{
    public Vector2 Size { get; set; } = new Vector2(64, 64);
    public Vector2 Speed { get; set; } = new();
    public Color Color { get; set; } = Color.White;
    public Vector2 Position { get; set; } = new();

    public float Acceleration { get; set; }
    public float Friction { get; set; }
    public float MaxSpeed { get; set; }
    public Vector2 Direction { get; set; } = new();

    public bool IsDestroyed { get; set; } = false;

    public Rectangle Rectangle => new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);

    public Texture2D Sprite {  get; set; } = null;

    protected BaseEntityModel((float x, float y) position)
    {
        Position = new Vector2(position.x, position.y);
    }

    protected BaseEntityModel(int x, int y)
    {
        Position = new Vector2(x, y);
    }

    protected BaseEntityModel()
    {
        Position = new Vector2(0, 0);
    }

    public virtual void Update(GameTime gameTime, List<BaseEntityModel> entities)
    {
    }

    public virtual void Colision(BaseEntityModel model)
    {
    }

    public virtual void Draw()
    {
        if (Sprite is not null)
        {
            GlobalVariables.SpriteBatchEntities.Draw(Sprite, Rectangle, Color);
        }
        else
        {
            GlobalVariables.SpriteBatchEntities.Draw(GlobalVariables.Pixel, Rectangle, Color);
        }
    }

    public virtual void Destroy()
    {
        IsDestroyed = true;
    }

    protected void Move(Vector2 direction, float delta)
    {
        if (direction != Vector2.Zero)
        {
            direction.Normalize();
            Speed += direction * Acceleration * delta;
            Direction = direction;
        }
        else
        {
            // Aplica atrito
            if (Speed.Length() > 0)
            {
                Vector2 frictionVector = Vector2.Normalize(Speed) * Friction * delta;
                if (frictionVector.Length() > Speed.Length())
                    Speed = Vector2.Zero;
                else
                    Speed -= frictionVector;
            }
        }

        if (Speed.Length() > MaxSpeed)
            Speed = Vector2.Normalize(Speed) * MaxSpeed;

        Position += Speed * delta;
    }
}
