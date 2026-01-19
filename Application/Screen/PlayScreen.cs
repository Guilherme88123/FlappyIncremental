using Application.Const;
using Application.Enum;
using Application.Interface.Menu;
using Application.Interface.Screen;
using Application.Model.Entities;
using FlappyIncremental.Dto;
using FlappyIncremental.Model.Entities.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Screen;

public class PlayScreen : IScreen
{
    public string ScreenCode => ScreenCodesConst.PlayScreen;

    public readonly List<BaseEntityModel> Entities = new();
    public readonly List<BaseEntityModel> EntitiesToAdd = new();

    private readonly IMenuService MenuService;

    private float EscDelay = 0.3f;
    private float EscDelayAtual = 0f;

    private float PipeDelay = 3f;
    private float PipeDelayAtual = 0f;

    public int Score { get; set; } = 0;

    private GameStatusType GameStatus = GameStatusType.Playing;

    public PlayScreen()
    {
        MenuService = GlobalVariables.GetService<IMenuService>();
    }

    #region Initialize

    public void Initialize()
    {
        Entities.Add(new BirdModel((300, 500)));
    }

    #endregion

    #region Update

    public void Update(GameTime gameTime)
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

            if (PipeDelayAtual < 0)
            {
                GerarPipe();
                PipeDelayAtual = PipeDelay;
            }

            ValidarScore();
        }
        else if (GameStatus == GameStatusType.MainMenu)
        {
            MenuService.Update(gameTime);
        }
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

    private void GerarPipe()
    {
        PipeModel pipeCima = new PipeModel((1920, 0));
        PipeModel pipeBaixo = new PipeModel((1920, 0));

        pipeBaixo.HasScored = true; //Controlar pontuação apenas pelo pipe de cima
        pipeCima.IsTop = true;

        int espacoPassagem = 400;
        int rng = new Random().Next(100, 700);

        pipeCima.Position = new Vector2(pipeCima.Position.X, rng - pipeCima.Size.Y);
        pipeBaixo.Position = new Vector2(pipeBaixo.Position.X, rng + espacoPassagem);

        Entities.Add(pipeCima);
        Entities.Add(pipeBaixo);
    }

    private void ValidarScore()
    {
        foreach (var entity in Entities)
        {
            if (entity is PipeModel pipe)
            {
                if (!pipe.HasScored && pipe.Position.X + pipe.Size.X < 350)
                {
                    Score++;
                    pipe.HasScored = true;
                }
            }
        }
    }

    #endregion

    #region Draw

    public void Draw()
    {
        GlobalVariables.Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

        GlobalVariables.SpriteBatchEntities.Begin();
        GlobalVariables.SpriteBatchInterface.Begin();

        Entities.ForEach(x => x.Draw());
        DrawScore();

        if (GameStatus == GameStatusType.MainMenu)
        {
            MenuService.DrawMenu();
        }
        else if (GameStatus == GameStatusType.GameOver)
        {
            DrawGameOverTitle();
        }

        GlobalVariables.SpriteBatchEntities.End();
        GlobalVariables.SpriteBatchInterface.End();
    }

    public void DrawInterface()
    {
        DrawScore();
    }

    public void DrawScore()
    {
        GlobalVariables.SpriteBatchInterface.DrawString(GlobalVariables.Font, $"Score: {Score}", new Vector2(20, 20), Color.White);
    }

    public void DrawGameOverTitle()
    {
        var width = GlobalVariables.Graphics.PreferredBackBufferWidth;
        var height = GlobalVariables.Graphics.PreferredBackBufferHeight;

        var text = "Game Over";
        var finalScore = $"Final Score: {Score}";

        var textSize = GlobalVariables.Font.MeasureString(text);
        var finalScoreSize = GlobalVariables.Font.MeasureString(finalScore);

        var textPosition = new Vector2((width - textSize.X) / 2, (height - textSize.Y) / 2 - 50);
        var finalScorePosition = new Vector2((width - finalScoreSize.X) / 2, (height - finalScoreSize.Y) / 2 + 10);

        GlobalVariables.SpriteBatchInterface.DrawString(GlobalVariables.Font, text, textPosition, Color.White);
        GlobalVariables.SpriteBatchInterface.DrawString(GlobalVariables.Font, finalScore, finalScorePosition, Color.White);
    }

    #endregion

    #region Exit

    public void Exit()
    {
        GameStatus = GameStatusType.GameOver;

        Task.Delay(3000).Wait();

        GlobalVariables.Game.ChangeScreen(ScreenCodesConst.MenuScreen);
    }

    #endregion
}
