using Application.Const;
using Application.Dto;
using Application.Interface.Screen;
using Application.Model.MenuElements;
using Application.Model.MenuElements.Base;
using FlappyIncremental.Const;
using FlappyIncremental.Dto;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public bool HasUpdateSize { get; set; }

    #region Initialize

    public void Initialize()
    {
        LoadButtons();
        LoadOptionButtons();
    }

    public void LoadButtons()
    {
        ListaBotoes.Clear();

        var windowWidth = GlobalVariables.Graphics.PreferredBackBufferWidth;
        var windowHeight = GlobalVariables.Graphics.PreferredBackBufferHeight;

        var width = windowWidth / 5;
        var height = windowHeight / 9;
        var x = windowWidth / 2 - width / 2;
        var y = (int)(windowHeight / 2 - height / 2);

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
        ListaBotoesOptions.Clear();

        var windowWidth = GlobalVariables.Graphics.PreferredBackBufferWidth;
        var windowHeight = GlobalVariables.Graphics.PreferredBackBufferHeight;

        var widthMenu = windowWidth / 2.5f;
        var heightMenu = windowHeight / 1.2f;
        var xMenu = windowWidth / 2 - widthMenu / 2;
        var yMenu = windowHeight / 2 - heightMenu / 2;
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
        var heightButtons = windowHeight / 9;
        var xButtons = xMenu + borderMenu;
        var yButtons = yMenu + borderMenu;
        var spaceBetweenButtons = heightButtons / 10;

        var sizeButton = new DropdownModel()
        {
            Rectangle = new((int)xButtons, (int)yButtons + (heightButtons + spaceBetweenButtons) * 3, (int)widthButtons, (int)heightButtons),
            Text = "Window Size",
            Overlay = OverlayButton,
            Color = Color.White,
            ValueUpdate = UpdateWindowSize,
            ListItens = new List<DropdownItemDto>()
            {
                new() { Id = 0, Text = "800x600", Value = new Vector2(800, 600) },
                new() { Id = 1, Text = "1280x720", Value = new Vector2(1280, 720) },
                new() { Id = 2, Text = "1600x900", Value = new Vector2(1600, 900) },
                new() { Id = 3, Text = "1920x1080", Value = new Vector2(1920, 1080) },
            },
        };

        sizeButton.SelectedItem = sizeButton.ListItens.First(x =>
            ((Vector2)x.Value).X == GlobalOptions.WidthSize &&
            ((Vector2)x.Value).Y == GlobalOptions.HeightSize).Id;

        var fullscreenButton = new SwitchModel()
        {
            Rectangle = new((int)xButtons, (int)yButtons + (heightButtons + spaceBetweenButtons) * 0, (int)widthButtons, (int)heightButtons),
            Click = ToggleFullscreen,
            Text = "Fullscreen",
            Overlay = OverlayButton,
            Color = Color.White,
            Value = GlobalOptions.Fullscreen,
        };

        var musicButton = new RadioModel()
        {
            Rectangle = new((int)xButtons, (int)yButtons + (heightButtons + spaceBetweenButtons) * 1, (int)widthButtons, (int)heightButtons),
            Text = "Music Volume",
            Overlay = OverlayButton,
            DotOverlay = OverlaySquareButton,
            Color = Color.White,
            ValueUpdate = UpdateMusicVolume,
            Value = GlobalOptions.MusicVolume,
        };

        var sfxButton = new RadioModel()
        {
            Rectangle = new((int)xButtons, (int)yButtons + (heightButtons + spaceBetweenButtons) * 2, (int)widthButtons, (int)heightButtons),
            Text = "Effects Volume",
            Overlay = OverlayButton,
            DotOverlay = OverlaySquareButton,
            Color = Color.White,
            ValueUpdate = UpdateSfxVolume,
            Value = GlobalOptions.SfxVolume,
        };

        ListaBotoesOptions.Add(closeButton);
        ListaBotoesOptions.Add(sizeButton);
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

        if (HasUpdateSize)
        {
            LoadButtons();
            LoadOptionButtons();
            HasUpdateSize = false;
        }
    }

    public static void StartGame()
    {
        GlobalVariables.Game.ChangeScreen(ScreenCodesConst.UpgradeScreen);
    }

    public void ToggleOptions()
    {
        IsOptionsEnable = !IsOptionsEnable;
    }

    public void UpdateWindowSize(DropdownItemDto item)
    {
        var size = item.Value as Vector2?;

        if (size is null) return;

        var width = (int)size.Value.X;
        var height = (int)size.Value.Y;

        GlobalVariables.Graphics.PreferredBackBufferWidth = width;
        GlobalVariables.Graphics.PreferredBackBufferHeight = height;
        GlobalVariables.Graphics.ApplyChanges();
        GlobalOptions.WidthSize = width;
        GlobalOptions.HeightSize = height;

        HasUpdateSize = true;
    }

    public void ToggleFullscreen(bool isFullscreen)
    {
        GlobalVariables.Graphics.IsFullScreen = isFullscreen;
        GlobalVariables.Graphics.ApplyChanges();
        GlobalOptions.Fullscreen = isFullscreen;
    }

    public void UpdateMusicVolume(int volume)
    {
        GlobalOptions.MusicVolume = volume;
        MediaPlayer.Volume = GlobalOptions.MusicVolumeFloat;
    }

    public void UpdateSfxVolume(int volume)
    { 
        GlobalOptions.SfxVolume = volume;
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
        var windowWidth = GlobalOptions.WidthSize;
        var windowHeight = GlobalOptions.HeightSize;

        var width = windowWidth / 3;
        var height = windowHeight / 4;

        var x = windowWidth / 2 - width / 2;
        var y = windowHeight / 8;

        var scaleX = (float)width / Title.Width;
        var scaleY = (float)height / Title.Height;

        var titlePosition = new Vector2(x, y);

        GlobalVariables.SpriteBatchInterface.Draw(
            Title,
            titlePosition,
            null,
            Color.White,
            0f,
            new Vector2(0.5f, 0.5f),
            new Vector2(scaleX, scaleY),
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
