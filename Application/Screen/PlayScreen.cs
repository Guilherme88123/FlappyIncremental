using Application.Const;
using Application.Dto;
using Application.Enum;
using Application.Interface.Menu;
using Application.Interface.Screen;
using Application.Model.Entities;
using Application.Model.MenuElements;
using Application.Model.MenuElements.Base;
using FlappyIncremental.Dto;
using FlappyIncremental.Model.Entities.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

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
    private Rectangle OverlayRect { get; set; }
    private SoundEffect GameOverSound { get; set; } = GlobalVariables.Game.Content.Load<SoundEffect>("game_over");

    private List<BaseElementModel> ListPauseButton { get; set; } = new();

    public PlayScreen()
    {
        MenuService = GlobalVariables.GetService<IMenuService>();
    }

    #region Initialize

    public void Initialize()
    {
        LoadInitialEntities();
        LoadGameOverButtons();
        LoadPauseButtons();
    }

    public void LoadInitialEntities()
    {
        var initialX = 1920 / 7;
        var initialY = (int)(1080 / 2.5);

        Entities.Add(new BirdModel((initialX, initialY)));
    }

    public void LoadGameOverButtons()
    {
        var totalWidth = GlobalOptions.WidthSize;
        var totalHeight = GlobalOptions.HeightSize;

        var widthMenu = totalWidth / 4f;
        var heightMenu = totalHeight / 2f;
        var xMenu = totalWidth / 2 - widthMenu / 2;
        var yMenu = totalHeight / 2 - heightMenu / 2;
        var borderMenu = widthMenu / 10;

        OverlayRect = new((int)xMenu, (int)yMenu, (int)widthMenu, (int)heightMenu);

        var width = widthMenu - 2 * borderMenu;
        var heigth = heightMenu / 5;
        var x = xMenu + borderMenu;
        var y = yMenu + heightMenu - borderMenu - heigth;
        var spacing = heigth / 10;

        var menuButton = new ButtonModel()
        {
            Rectangle = new((int)x, (int)y, (int)width, (int)heigth),
            Click = () => UpgradeButton(),
            Text = "Upgrade",
            Overlay = OverlayButton,
            Color = Color.White,
        };

        var retryButton = new ButtonModel()
        {
            Rectangle = new((int)x, (int)(y - heigth - spacing), (int)width, (int)heigth),
            Click = () => RetryButton(),
            Text = "Retry",
            Overlay = OverlayButton,
            Color = Color.White,
        };

        ListGameOverButton.Add(menuButton);
        ListGameOverButton.Add(retryButton);
    }

    public void LoadPauseButtons()
    {
        var totalWidth = GlobalOptions.WidthSize;
        var totalHeight = GlobalOptions.HeightSize;

        var widthMenu = totalWidth / 4f;
        var heightMenu = totalHeight / 2f;
        var xMenu = totalWidth / 2 - widthMenu / 2;
        var yMenu = totalHeight / 2 - heightMenu / 2;
        var borderMenu = widthMenu / 10;

        var width = widthMenu - 2 * borderMenu;
        var heigth = heightMenu / 5; 
        var x = xMenu + borderMenu;
        var y = yMenu + heightMenu - borderMenu - heigth;
        var spacing = heigth / 10;

        var menuButton = new ButtonModel()
        {
            Rectangle = new((int)x, (int)y, (int)width, (int)heigth),
            Click = () => UpgradeButton(),
            Text = "Upgrade",
            Overlay = OverlayButton,
            Color = Color.White,
        };

        var retryButton = new ButtonModel()
        {
            Rectangle = new((int)x, (int)(y - heigth - spacing), (int)width, (int)heigth),
            Click = () => RetryButton(),
            Text = "Restart",
            Overlay = OverlayButton,
            Color = Color.White,
        };

        var resumeButton = new ButtonModel()
        {
            Rectangle = new((int)x, (int)(y - (heigth + spacing) * 2), (int)width, (int)heigth),
            Click = () => ResumeButton(),
            Text = "Resume",
            Overlay = OverlayButton,
            Color = Color.White,
        };

        ListPauseButton.Add(menuButton);
        ListPauseButton.Add(retryButton);
        ListPauseButton.Add(resumeButton);
    }

    #endregion

    #region Update

    public void Update(GameTime gameTime)
    {
        if (GameStatus == GameStatusType.Playing) 
        {
            UpdatePlaying(gameTime);
            return;
        }

        if (GameStatus == GameStatusType.Paused) 
        {
            UpdatePause(gameTime);
            return;
        }

        UpdateGameOver(gameTime);
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

        int espacoPassagem = 1080 / 3;
        int rng = new Random().Next(espacoPassagem / 2, espacoPassagem * 2);

        pipeCima.Position = new Vector2(1920, rng - pipeCima.Size.Y);
        pipeBaixo.Position = new Vector2(1920, rng + espacoPassagem);

        //pipeCima.Position = new Vector2(pipeCima.Position.X, rng - pipeCima.Size.Y);
        //pipeBaixo.Position = new Vector2(pipeBaixo.Position.X, rng + espacoPassagem);

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

    public void UpgradeButton()
    {
        GlobalVariables.Game.ChangeScreen(ScreenCodesConst.UpgradeScreen);
    }

    #endregion

    #region Pause

    public void UpdatePause(GameTime gameTime)
    {
        ListPauseButton.ForEach(x => x.Update(gameTime));
    }

    public void ResumeButton()
    {
        if (GameStatus == GameStatusType.Paused)
        {
            GameStatus = GameStatusType.Playing;
        }
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
        DrawOverlay();
        DrawPausedTitle();
        ListPauseButton.ForEach(x => x.Draw());
    }

    public void DrawOverlay()
    {
        var scaleX = (float)OverlayRect.Width / OverlayMenu.Width;
        var scaleY = (float)OverlayRect.Height / OverlayMenu.Height;

        var overlayPosition = new Vector2(OverlayRect.X, OverlayRect.Y);

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

    #region Game Over

    public void DrawGameOver()
    {
        DrawOverlay();
        DrawGameOverTitle();
        ListGameOverButton.ForEach(x => x.Draw());
    }

    public void DrawGameOverTitle()
    {
        var width = GlobalOptions.WidthSize;
        var height = GlobalOptions.HeightSize;

        var text = "Game Over";
        var finalScore = $"Final Score: {Score}";

        var textSize = GlobalVariables.Font.MeasureString(text);
        var finalScoreSize = GlobalVariables.Font.MeasureString(finalScore);

        var yMenu = OverlayRect.Y;
        var borderMenu = (OverlayRect.Width) / 10;

        var textPosition = new Vector2((width - textSize.X) / 2, yMenu + borderMenu);
        var finalScorePosition = new Vector2((width - finalScoreSize.X) / 2, yMenu + borderMenu * 2);

        GlobalVariables.SpriteBatchInterface.DrawString(GlobalVariables.Font, text, textPosition, Color.White);
        GlobalVariables.SpriteBatchInterface.DrawString(GlobalVariables.Font, finalScore, finalScorePosition, Color.White);
    }

    #endregion

    #region Paused

    public void DrawPausedTitle()
    {
        var width = GlobalOptions.WidthSize;
        var height = GlobalOptions.HeightSize;

        var text = $"Current Score: {Score}";

        var textSize = GlobalVariables.Font.MeasureString(text);

        var yMenu = OverlayRect.Y;
        var borderMenu = (OverlayRect.Width) / 10;

        var textPosition = new Vector2((width - textSize.X) / 2, yMenu + borderMenu);

        GlobalVariables.SpriteBatchInterface.DrawString(GlobalVariables.Font, text, textPosition, Color.White);
    }

    #endregion

    #endregion

    #region Exit

    public void Exit()
    {
        GameOverSound.Play(GlobalOptions.SfxVolumeFloat, 0f, 0f);
        GameStatus = GameStatusType.GameOver;
    }

    #endregion
}
