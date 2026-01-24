using Application.Const;
using Application.Enum;
using Application.Interface.Menu;
using Application.Interface.Screen;
using Application.Model.Entities;
using Application.Model.MenuElements.Base;
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

    public Texture2D OverlayButton { get; set; } = GlobalVariables.Game.Content.Load<Texture2D>("button_overlay");
    public Texture2D OverlayMenu { get; set; } = GlobalVariables.Game.Content.Load<Texture2D>("menu_overlay");

    public readonly List<BaseEntityModel> Entities = new();
    public readonly List<BaseEntityModel> EntitiesToAdd = new();

    private readonly IMenuService MenuService;

    private float EscDelay = 0.3f;
    private float EscDelayAtual = 0f;

    private float PipeDelay = 3f;
    private float PipeDelayAtual = 0f;

    public int Score { get; set; } = 0;

    private GameStatusType GameStatus = GameStatusType.Playing;

    private List<BaseElementModel> ListGameOverButton { get; set; } = new();
    private Rectangle GameOverOverlayRect { get; set; }

    public PlayScreen()
    {
        MenuService = GlobalVariables.GetService<IMenuService>();
    }

    #region Initialize

    public void Initialize()
    {
        LoadInitialEntities();
        LoadGameOverButtons();
    }

    public void LoadInitialEntities()
    {
        Entities.Add(new BirdModel((300, 500)));
    }

    public void LoadGameOverButtons()
    {
        var totalWidth = GlobalVariables.Graphics.PreferredBackBufferWidth;
        var totalHeight = GlobalVariables.Graphics.PreferredBackBufferHeight;

        var widthMenu = totalWidth / 5f;
        var heightMenu = totalHeight / 2f;
        var xMenu = totalWidth / 2 - widthMenu / 2;
        var yMenu = totalHeight / 2 - heightMenu / 2;
        var borderMenu = 30;

        GameOverOverlayRect = new((int)xMenu, (int)yMenu, (int)widthMenu, (int)heightMenu);

        var width = widthMenu - 2 * borderMenu;
        var heigth = 100;
        var x = xMenu + borderMenu;
        var y = yMenu + heightMenu - borderMenu - heigth;
        var spacing = 5;

        var menuButton = new BaseElementModel()
        {
            Rectangle = new((int)x, (int)y, (int)width, (int)heigth),
            Click = () => MainMenuButton(),
            Text = "Main Menu",
            Overlay = OverlayButton,
            Color = Color.White,
        };

        var retryButton = new BaseElementModel()
        {
            Rectangle = new((int)x, (int)y - heigth - spacing, (int)width, (int)heigth),
            Click = () => RetryButton(),
            Text = "Retry",
            Overlay = OverlayButton,
            Color = Color.White,
        };

        ListGameOverButton.Add(menuButton);
        ListGameOverButton.Add(retryButton);
    }

    #endregion

    #region Update

    public void Update(GameTime gameTime)
    {
        if (GameStatus != GameStatusType.GameOver)
        {
            UpdatePlaying(gameTime);
        }
        else
        {
            UpdateGameOver(gameTime);
        }
    }

    public void UpdatePlaying(GameTime gameTime)
    {
        var teclado = Keyboard.GetState();
        if (teclado.IsKeyDown(Keys.Escape) && EscDelayAtual < 0)
        {
            GameStatus = GameStatus == GameStatusType.Paused ? GameStatusType.Playing : GameStatusType.Paused;
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
        else if (GameStatus == GameStatusType.Paused)
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

    #region GameOver

    public void UpdateGameOver(GameTime gameTime)
    {
        ListGameOverButton.ForEach(x => x.Update(gameTime));
    }

    public void RetryButton()
    {
        GlobalVariables.Game.ChangeScreen(ScreenCodesConst.PlayScreen);
    }

    public void MainMenuButton()
    {
        GlobalVariables.Game.ChangeScreen(ScreenCodesConst.MenuScreen);
    }

    #endregion

    #endregion

    #region Draw

    public void Draw()
    {
        Entities.ForEach(x => x.Draw());
        DrawScore();

        if (GameStatus == GameStatusType.Paused)
        {
            DrawPausedInterface();
        }
        else if (GameStatus == GameStatusType.GameOver)
        {
            DrawGameOver();
        }
    }

    public void DrawInterface()
    {
        DrawScore();
    }

    public void DrawScore()
    {
        GlobalVariables.SpriteBatchInterface.DrawString(GlobalVariables.Font, $"Score: {Score}", new Vector2(20, 20), Color.White);
    }

    public void DrawPausedInterface()
    {

    }

    #region Game Over

    public void DrawGameOver()
    {
        DrawGameOverOverlay();
        DrawGameOverTitle();
        ListGameOverButton.ForEach(x => x.Draw());
    }

    public void DrawGameOverTitle()
    {
        var width = GlobalVariables.Graphics.PreferredBackBufferWidth;
        var height = GlobalVariables.Graphics.PreferredBackBufferHeight;

        var text = "Game Over";
        var finalScore = $"Final Score: {Score}";

        var textSize = GlobalVariables.Font.MeasureString(text);
        var finalScoreSize = GlobalVariables.Font.MeasureString(finalScore);

        var textPosition = new Vector2((width - textSize.X) / 2, (height - textSize.Y) / 2 - 150);
        var finalScorePosition = new Vector2((width - finalScoreSize.X) / 2, textPosition.Y + 50);

        GlobalVariables.SpriteBatchInterface.DrawString(GlobalVariables.Font, text, textPosition, Color.White);
        GlobalVariables.SpriteBatchInterface.DrawString(GlobalVariables.Font, finalScore, finalScorePosition, Color.White);
    }

    public void DrawGameOverOverlay()
    {
        var scaleX = (float)GameOverOverlayRect.Width / OverlayMenu.Width;
        var scaleY = (float)GameOverOverlayRect.Height / OverlayMenu.Height;

        var overlayPosition = new Vector2(GameOverOverlayRect.X, GameOverOverlayRect.Y);

        GlobalVariables.SpriteBatchInterface.Draw(
            OverlayMenu,
            overlayPosition,
            null,
            Color.White,
            0f,
            new Vector2(0.5f, 0.5f),
            new Vector2(scaleX, scaleY),
            SpriteEffects.None,
            0f);
    }

    #endregion

    #endregion

    #region Exit

    public void Exit()
    {
        GameStatus = GameStatusType.GameOver;
    }

    #endregion
}
