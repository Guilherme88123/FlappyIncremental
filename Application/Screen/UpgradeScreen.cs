using Application.Const;
using Application.Dto;
using Application.Enum;
using Application.Interface.Screen;
using Application.Model.MenuElements;
using Application.Model.MenuElements.Base;
using Application.Model.MenuElements.Button;
using Application.Model.MenuElements.Button.UpgradeButtonModel;
using Application.Model.Upgrade.Definition.Base;
using FlappyIncremental.Dto;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;

namespace Application.Screen;

public class UpgradeScreen : IScreen
{
    public string ScreenCode => ScreenCodesConst.UpgradeScreen;

    private float EscDelay = 0.2f;
    private float EscDelayAtual = 0f;
    private bool IsMenuOpen { get; set; }

    public Texture2D OverlayButton { get; set; } = GlobalVariables.Game.Content.Load<Texture2D>("button_overlay");
    public Texture2D OverlayMenu { get; set; } = GlobalVariables.Game.Content.Load<Texture2D>("menu_overlay");
    public Texture2D OverlaySquareButton { get; set; } = GlobalVariables.Game.Content.Load<Texture2D>("square_button_overlay");
    private Rectangle OverlayMenuRect { get; set; }

    private List<BaseElementModel> ButtonList { get; set; } = new();
    private List<BaseElementModel> MenuButtonList { get; set; } = new();

    private List<UpgradeButtonModel> UpgradeButtonList { get; set; } = new();

    #region Initialize

    public void Initialize()
    {
        LoadButtons();
        LoadMenuButtons();
        LoadUpgradeButtons();
    }

    public void LoadButtons()
    {
        var windowWidth = GlobalVariables.Graphics.PreferredBackBufferWidth;
        var windowHeight = GlobalVariables.Graphics.PreferredBackBufferHeight;

        var width = windowWidth / 5;
        var height = windowHeight / 12;
        var spacing = height / 5;
        var x = windowWidth / 2 - width / 2;
        var y = spacing;

        var botaoStart = new ButtonModel()
        {
            Rectangle = new(x, y, width, height),
            Click = () => StartGame(),
            Text = "Start",
            Overlay = OverlayButton,
            Color = Color.White,
        };

        ButtonList.Add(botaoStart);
    }

    public void LoadMenuButtons()
    {
        var totalWidth = GlobalOptions.WidthSize;
        var totalHeight = GlobalOptions.HeightSize;

        var widthMenu = totalWidth / 4f;
        var heightMenu = totalHeight / 2f;
        var xMenu = totalWidth / 2 - widthMenu / 2;
        var yMenu = totalHeight / 2 - heightMenu / 2;
        var borderMenu = widthMenu / 10;

        OverlayMenuRect = new((int)xMenu, (int)yMenu, (int)widthMenu, (int)heightMenu);

        var width = widthMenu - 2 * borderMenu;
        var heigth = heightMenu / 5;
        var x = xMenu + borderMenu;
        var y = yMenu + borderMenu;
        var spacing = heigth / 10;
         
        var menuButton = new ButtonModel()
        {
            Rectangle = new((int)x, (int)(y + spacing + heigth), (int)width, (int)heigth),
            Click = () => ReturnMenu(),
            Text = "Main Menu",
            Overlay = OverlayButton,
            Color = Color.White,
        };

        var resumeButton = new ButtonModel()
        {
            Rectangle = new((int)x, (int)y, (int)width, (int)heigth),
            Click = () => ToggleMenu(),
            Text = "Resume",
            Overlay = OverlayButton,
            Color = Color.White,
        };

        MenuButtonList.Add(menuButton);
        MenuButtonList.Add(resumeButton);
    }

    public void LoadUpgradeButtons()
    {
        var windowWidth = GlobalOptions.WidthSize;
        var windowHeight = GlobalOptions.HeightSize;

        var height = windowHeight / 10;
        var width = height;
        var space = height / 2;

        var upgrades = GlobalVariables.GetService<IEnumerable<BaseUpgradeModel>>();

        foreach (var upgrade in upgrades )
        {
            var x = windowWidth / 2 - (space * upgrade.XPosition);
            var y = windowHeight / 2 - (space * upgrade.YPosition);

            var upgradeButton = new UpgradeButtonModel(upgrade)
            {
                Rectangle = new((int)x, (int)y, (int)width, (int)height),
                Overlay = OverlaySquareButton,
            };

            UpgradeButtonList.Add(upgradeButton);
        }
    }

    public void StartGame()
    {
        GlobalVariables.Game.ChangeScreen(ScreenCodesConst.PlayScreen);
    }

    public void ReturnMenu()
    {
        GlobalVariables.Game.ChangeScreen(ScreenCodesConst.MenuScreen);
    }

    #endregion

    #region Update

    public void Update(GameTime gameTime)
    {
        if (IsMenuOpen)
        {
            UpdateMenu(gameTime);
            return;
        }

        ValidatePause(gameTime);

        UpgradeButtonList.ForEach(x => x.Update(gameTime));
        ButtonList.ForEach(x => x.Update(gameTime));
    }

    private void ToggleMenu()
    {
        IsMenuOpen = !IsMenuOpen;
    }

    private void ValidatePause(GameTime gameTime)
    {
        var teclado = Keyboard.GetState();
        if (teclado.IsKeyDown(Keys.Escape) && EscDelayAtual < 0)
        {
            ToggleMenu();
            EscDelayAtual = EscDelay;
        }

        EscDelayAtual -= (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    #region Menu

    private void UpdateMenu(GameTime gameTime)
    {
        ValidatePause(gameTime);
        MenuButtonList.ForEach(x => x.Update(gameTime));
    }

    #endregion

    #endregion

    #region Draw

    public void Draw()
    {
        UpgradeButtonList.ForEach(x => x.Draw());
        ButtonList.ForEach(x => x.Draw());
        DrawScore();

        if (IsMenuOpen) DrawMenu();
    }

    private void DrawScore()
    {
        GlobalVariables.SpriteBatchInterface.DrawString(GlobalVariables.Font, $"Score: {GlobalStatus.TotalScore}", new Vector2(20, 20), Color.White);
    }

    private void DrawMenu()
    {
        DrawMenuOverlay();
        MenuButtonList.ForEach(x => x.Draw());
    }

    private void DrawMenuOverlay()
    {
        var scaleX = (float)OverlayMenuRect.Width / OverlayMenu.Width;
        var scaleY = (float)OverlayMenuRect.Height / OverlayMenu.Height;

        var overlayPosition = new Vector2(OverlayMenuRect.X, OverlayMenuRect.Y);

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
