using Application.Const;
using Application.Dto;
using Application.Interface.Screen;
using Application.Model.MenuElements;
using Application.Model.MenuElements.Base;
using FlappyIncremental.Const;
using FlappyIncremental.Dto;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;

namespace Application.Screen;

public class MenuScreen : IScreen
{
    public string ScreenCode => ScreenCodesConst.MenuScreen;

    private List<BaseElementModel> ListaBotoes { get; set; } = new();
    private List<BaseElementModel> ListaBotoesOptions { get; set; } = new();
    public Texture2D Title { get; set; } = GlobalVariables.Game.Content.Load<Texture2D>("title");
    public Texture2D OverlayButton { get; set; } = GlobalVariables.Game.Content.Load<Texture2D>("button_overlay");
    public Texture2D OverlaySquareButton { get; set; } = GlobalVariables.Game.Content.Load<Texture2D>("square_button_overlay");
    public Texture2D OverlayMenu { get; set; } = GlobalVariables.Game.Content.Load<Texture2D>("menu_overlay");
    private Rectangle OptionsMenuRect { get; set; }

    public bool IsOptionsEnable { get; set; }

    #region Initialize

    public void Initialize()
    {
        LoadButtons();
        LoadOptionButtons();
    }

    public void LoadButtons()
    {
        var telaWidth = GlobalVariables.Graphics.PreferredBackBufferWidth;
        var telaHeight = GlobalVariables.Graphics.PreferredBackBufferHeight;

        var width = 300;
        var height = 100;
        var x = telaWidth / 2 - width / 2;
        var y = (int)(telaHeight / 1.6 - height / 2);

        var botaoStart = new ButtonModel()
        {
            Rectangle = new(x, y, width, height),
            Click = () => StartGame(),
            Text = "Start",
            Overlay = OverlayButton,
            Color = Color.White,
        };

        var botaoOpcoes = new ButtonModel()
        {
            Rectangle = new(x, y + (height + 10), width, height),
            Click = () => ToggleOptions(),
            Text = "Options",
            Overlay = OverlayButton,
            Color = Color.White,
        };

        var botaoExit = new ButtonModel()
        {
            Rectangle = new(x, y + (height + 10) * 2, width, height),
            Click = () => GlobalVariables.Game.Exit(),
            Text = "Exit",
            Overlay = OverlayButton,
            Color = Color.White,
        };

        ListaBotoes.Add(botaoStart);
        ListaBotoes.Add(botaoOpcoes);
        ListaBotoes.Add(botaoExit);
    }

    public void LoadOptionButtons()
    {
        var totalWidth = GlobalVariables.Graphics.PreferredBackBufferWidth;
        var totalHeight = GlobalVariables.Graphics.PreferredBackBufferHeight;

        var widthMenu = totalWidth / 2.9f;
        var heightMenu = totalHeight / 1.2f;
        var xMenu = totalWidth / 2 - widthMenu / 2;
        var yMenu = totalHeight / 2 - heightMenu / 2;
        var borderMenu = 50;

        OptionsMenuRect = new((int)xMenu, (int)yMenu, (int)widthMenu, (int)heightMenu);

        var widthCloseButton = widthMenu / 10f;
        var xCloseButton = xMenu + widthMenu - widthCloseButton;
        var yCloseButton = yMenu;

        var closeButton = new ButtonModel()
        {
            Rectangle = new((int)xCloseButton, (int)yCloseButton, (int)widthCloseButton, (int)widthCloseButton),
            Click = ToggleOptions,
            Text = "X",
            Overlay = OverlaySquareButton,
            Color = Color.White,
        };

        var widthButtons = widthMenu - borderMenu * 2;
        var heightButtons = 100;
        var xButtons = xMenu + borderMenu;
        var yButtons = yMenu + borderMenu + 50;
        var spaceBetweenButtons = 10;

        var fullscreenButton = new SwitchModel()
        {
            Rectangle = new((int)xButtons, (int)yButtons, (int)widthButtons, (int)heightButtons),
            Click = ToggleFullscreen,
            Text = "Fullscreen",
            Overlay = OverlayButton,
            Color = Color.White,
        };

        var musicButton = new RadioModel()
        {
            Rectangle = new((int)xButtons, (int)yButtons + heightButtons + spaceBetweenButtons, (int)widthButtons, (int)heightButtons),
            Text = "Music Volume",
            Overlay = OverlayButton,
            DotOverlay = OverlaySquareButton,
            Color = Color.White,
        };

        var sfxButton = new RadioModel()
        {
            Rectangle = new((int)xButtons, (int)yButtons + (heightButtons + spaceBetweenButtons) * 2, (int)widthButtons, (int)heightButtons),
            Text = "Effects Volume",
            Overlay = OverlayButton,
            DotOverlay = OverlaySquareButton,
            Color = Color.White,
        };

        ListaBotoesOptions.Add(closeButton);
        ListaBotoesOptions.Add(fullscreenButton);
        ListaBotoesOptions.Add(musicButton);
        ListaBotoesOptions.Add(sfxButton);
    }

    #endregion

    #region Update

    public void Update(GameTime gameTime)
    {
        if (!IsOptionsEnable)
        {
            ListaBotoes.ForEach(x => x.Update(gameTime));
        }
        else
        {
            ListaBotoesOptions.ForEach(x => x.Update(gameTime));
        }
    }

    public static void StartGame()
    {
        GlobalVariables.Game.ChangeScreen(ScreenCodesConst.PlayScreen);
    }

    public void ToggleOptions()
    {
        IsOptionsEnable = !IsOptionsEnable;
    }

    public void ToggleFullscreen(bool isFullscreen)
    {
        GlobalVariables.Graphics.IsFullScreen = isFullscreen;
        GlobalVariables.Graphics.ApplyChanges();
        GlobalOptions.Fullscreen = isFullscreen;
    }

    #endregion

    #region Draw

    public void Draw()
    {
        ListaBotoes.ForEach(x => x.Draw());

        DrawVersionText();
        DrawTitle();

        if (IsOptionsEnable)
        {
            DrawOptionsMenu();
        }
    }

    public void DrawVersionText()
    {
        var width = GlobalVariables.Graphics.PreferredBackBufferWidth;
        var height = GlobalVariables.Graphics.PreferredBackBufferHeight;

        var text = Configuration.Version;

        var textPosition = new Vector2(15, height - 25);

        GlobalVariables.SpriteBatchInterface.DrawString(GlobalVariables.Font, text, textPosition, 
            Microsoft.Xna.Framework.Color.White);
    }

    public void DrawTitle()
    {
        var width = GlobalVariables.Graphics.PreferredBackBufferWidth;
        var height = GlobalVariables.Graphics.PreferredBackBufferHeight;

        var x = width / 2 - Title.Width / 2;
        var y = 250;

        var titlePosition = new Vector2(x, y);

        GlobalVariables.SpriteBatchEntities.Draw(
            Title,
            titlePosition,
            null,
            Color.White,
            0f,
            new Vector2(0.5f, 0.5f),
            new Vector2(1, 1),
            SpriteEffects.None,
            0f);
    }

    public void DrawOptionsMenu()
    {
        DrawMenu();
        ListaBotoesOptions.ForEach(x => x.Draw());
    }

    public void DrawMenu()
    {
        var scaleX = (float)OptionsMenuRect.Width / OverlayMenu.Width;
        var scaleY = (float)OptionsMenuRect.Height / OverlayMenu.Height;

        var overlayPosition = new Vector2(OptionsMenuRect.X, OptionsMenuRect.Y);

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

    #region Exit

    public void Exit()
    {
    }

    #endregion
}
