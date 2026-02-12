using Application.Dto;
using FlappyIncremental.Dto;
using FlappyIncremental.Model.Entities.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Application.Model.Entities;

public class BirdModel : BaseEntityModel
{
    public const float DelayPulo = 0.15f;
    public float DelayPuloAtual { get; set; }
    public bool HasPressedSpace { get; set; }

    private const float MaxAngle = (float)(Math.PI / 6f);
    private const float RotationSpeed = 10f;

    private SoundEffect JumpSound { get; set; } = GlobalVariables.Game.Content.Load<SoundEffect>("jump");

    public const decimal MaxHealth = 100;
    public decimal Health { get; set; } = MaxHealth * 0.7m;

    public BirdModel((float x, float y) position) : base(position)
    {
        Sprite = GlobalVariables.Game.Content.Load<Texture2D>("bird");
        Acceleration = 1500f;
        Friction = 1200f;
        MaxSpeed = 400f;
        Size = new System.Numerics.Vector2(80, 52);
    }

    public override void Update(Microsoft.Xna.Framework.GameTime gameTime, List<BaseEntityModel> entities)
    {
        base.Update(gameTime, entities);

        GetAngulo(gameTime);

        if (Rectangle.Bottom >= 1080 ||
            Rectangle.Top <= 0)
        {
            Destroy();
            GlobalVariables.Game.ActualScreen.Exit();
        }

        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var teclado = Keyboard.GetState();

        DelayPuloAtual -= delta;

        if (teclado.IsKeyDown(Keys.Space))
        {
            if (DelayPuloAtual < 0 && !HasPressedSpace)
            {
                Jump();
                DelayPuloAtual = DelayPulo;
                HasPressedSpace = true;
            }
        }
        else
        {
            HasPressedSpace = false;
        }

        Move(new(0, 1), delta);
    }

    public override void Colision(BaseEntityModel model)
    {
        base.Colision(model);

        if (model is PipeModel pipe)
        {
            Destroy();
            GlobalVariables.Game.ActualScreen.Exit();
        }
    }

    private void Jump()
    {
        Speed = new(0, -MaxSpeed);
        JumpSound.Play(GlobalOptions.SfxVolumeFloat, 0f, 0f);
    }

    private void GetAngulo(GameTime gameTime)
    {
        float targetAngle = Speed.Y / MaxSpeed * MaxAngle;
        targetAngle = Math.Clamp(targetAngle, -MaxAngle, MaxAngle);

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        ActualAngle = MathHelper.Lerp(ActualAngle, targetAngle, RotationSpeed * dt);
    }

    public override void Draw()
    {
        base.Draw();
        DrawHealthBar();
    }

    private void DrawHealthBar()
    {
        var totalWidth = 1920;
        var totalHeight = 1080;

        var x = 5;
        var y = 50;
        var spacing = 3;
        var width = totalWidth / 5;
        var height = totalHeight / 20;

        var healthPercentage = (float)(Health / MaxHealth);
        var healthBarWidth = (int)(width * healthPercentage);

        Rectangle backgroundRect = new(x, y, width, height);
        Rectangle healthRect = new(x + spacing, y + spacing, healthBarWidth - spacing * 2, height - spacing * 2);

        GlobalVariables.SpriteBatchInterface.Draw(GlobalVariables.Pixel, backgroundRect, Color.DarkGray);
        GlobalVariables.SpriteBatchInterface.Draw(GlobalVariables.Pixel, healthRect, Color.Red);
    }
}
