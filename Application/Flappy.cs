using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using FlappyIncremental.Dto;
using Application.Enum;
using Application.Interface.Menu;
using System.Collections.Generic;
using FlappyIncremental.Model.Entities.Base;
using Application.Model.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace FlappyIncremental;

public class Flappy : Game
{
    public readonly List<BaseEntityModel> Entities = new();
    public readonly List<BaseEntityModel> EntitiesToAdd = new();

    private readonly IMenuService MenuService;

    private float EscDelay = 0.3f;
    private float EscDelayAtual = 0f;

    private float PipeDelay = 3f;
    private float PipeDelayAtual = 0f;

    public int Score { get; set; } = 0;

    private GameStatusType GameStatus = GameStatusType.Playing;

    public Flappy()
    {
        var graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = 1920;
        graphics.PreferredBackBufferHeight = 1080;
        //graphics.IsFullScreen = true;
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1d / 120d);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        graphics.ApplyChanges();

        GlobalVariables.Graphics = graphics;

        MenuService = GlobalVariables.GetService<IMenuService>();
    }

    protected override void LoadContent()
    {
        var spriteBatchEntities = new SpriteBatch(GraphicsDevice);
        var spriteBatchInterface = new SpriteBatch(GraphicsDevice);

        var pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData([Color.White]);

        GlobalVariables.Font = Content.Load<SpriteFont>("DefaultFont");
        GlobalVariables.SpriteBatchEntities = spriteBatchEntities;
        GlobalVariables.SpriteBatchInterface = spriteBatchInterface;
        GlobalVariables.Pixel = pixel;
    }

    protected override void Initialize()
    {
        Entities.Add(new BirdModel((300, 500)));

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        var teclado = Keyboard.GetState();
        if (teclado.IsKeyDown(Keys.Escape) && EscDelayAtual < 0)
        {
            GameStatus = GameStatus == GameStatusType.MainMenu ? GameStatusType.Playing : GameStatusType.MainMenu;
            EscDelayAtual = EscDelay;
        }

        EscDelayAtual -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (GameStatus == GameStatusType.Playing)
        {
            PipeDelayAtual -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (EntitiesToAdd.Any())
            {
                Entities.AddRange(EntitiesToAdd);
                EntitiesToAdd.Clear();
            }

            VerifyCollision();

            Entities.ForEach(entity => entity.Update(gameTime, EntitiesToAdd));
            Entities.RemoveAll(x => x.IsDestroyed);
        }
        else if (GameStatus == GameStatusType.MainMenu)
        {
            MenuService.Update(gameTime);
        }

        if (PipeDelayAtual < 0)
        {
            GerarPipe();
            PipeDelayAtual = PipeDelay;
        }

        base.Update(gameTime);
    }


    private void VerifyCollision()
    {
        for (int i = 0; i < Entities.Count; i++)
        {
            for (int j = i + 1; j < Entities.Count; j++)
            {
                var entity = Entities[i];
                var otherEntity = Entities[j];

                if (entity.Rectangle.Intersects(otherEntity.Rectangle))
                {
                    entity.Colision(otherEntity);
                    otherEntity.Colision(entity);
                }
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        GlobalVariables.SpriteBatchEntities.Begin();
        GlobalVariables.SpriteBatchInterface.Begin();

        Entities.ForEach(x => x.Draw());
        DrawScore();

        if (GameStatus == GameStatusType.MainMenu)
        {
            MenuService.DrawMenu();
        }

        GlobalVariables.SpriteBatchEntities.End();
        GlobalVariables.SpriteBatchInterface.End();

        base.Draw(gameTime);
    }

    #region Gameplay

    public async Task GameOver()
    {
        await Task.Delay(2000);

        Exit();
    }

    private void GerarPipe()
    {
        PipeModel pipeCima = new PipeModel((1920, 0));
        PipeModel pipeBaixo = new PipeModel((1920, 0));

        int espacoPassagem = 400;
        int rng = new Random().Next(200, 600);

        pipeCima.Position = new Vector2(pipeCima.Position.X, rng - pipeCima.Size.Y);
        pipeBaixo.Position = new Vector2(pipeBaixo.Position.X, rng + espacoPassagem);

        Entities.Add(pipeCima);
        Entities.Add(pipeBaixo);
    }

    #endregion

    #region Interface

    public void DrawInterface()
    {
        DrawScore();
    }

    public void DrawScore()
    {
        GlobalVariables.SpriteBatchInterface.DrawString(GlobalVariables.Font, $"Pontos: {Score}", new Vector2(10, 10), Color.White);
    }

    #endregion
}
