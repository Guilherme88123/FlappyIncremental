using FlappyIncremental.Dto;
using FlappyIncremental.Model.Entities.Base;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Numerics;

namespace Application.Model.Entities;

public class BirdModel : BaseEntityModel
{
    public const float DelayPulo = 0.1f;
    public float DelayPuloAtual { get; set; }

    public BirdModel((float x, float y) position) : base(position)
    {
        Color = Microsoft.Xna.Framework.Color.Yellow;
        Acceleration = 1500f;
        Friction = 1200f;
        MaxSpeed = 400f;
    }

    public override void Update(Microsoft.Xna.Framework.GameTime gameTime, List<BaseEntityModel> entities)
    {
        base.Update(gameTime, entities);

        if (Rectangle.Bottom >= GlobalVariables.Graphics.PreferredBackBufferHeight ||
            Rectangle.Top <= 0)
        {
            GlobalVariables.Game.GameOver();
        }

        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var teclado = Keyboard.GetState();

        DelayPuloAtual -= delta;

        if (teclado.IsKeyDown(Keys.Space) && DelayPuloAtual < 0)
        {
            Jump();
            DelayPuloAtual = DelayPulo;
        }

        Move(new(0, 1), delta);
    }

    public override void Colision(BaseEntityModel model)
    {
        base.Colision(model);

        if (model is PipeModel pipe)
        {
            Destroy();
            _ = GlobalVariables.Game.GameOver();
        }
    }

    private void Jump()
    {
        Speed = new(0, -MaxSpeed);
    }
}
