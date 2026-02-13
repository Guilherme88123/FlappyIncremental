using Application.Const;
using Application.Interface.Screen;
using Application.Model.MenuElements;
using Application.Model.MenuElements.Base;
using FlappyIncremental.Dto;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Application.Screen;

public class UpgradeScreen : IScreen
{
    public string ScreenCode => ScreenCodesConst.UpgradeScreen;

    public Texture2D OverlayButton { get; set; } = GlobalVariables.Game.Content.Load<Texture2D>("button_overlay");

    private List<BaseElementModel> ListaBotoes { get; set; } = new();

    #region Initialize

    public void Initialize()
    {
        LoadButtons();
    }

    public void LoadButtons()
    {

        var windowWidth = GlobalVariables.Graphics.PreferredBackBufferWidth;
        var windowHeight = GlobalVariables.Graphics.PreferredBackBufferHeight;

        var width = windowWidth / 5;
        var height = windowHeight / 12;
        var spacing = height / 5;
        var x = windowWidth / 2 - width / 2;
        var y = windowHeight - (height + spacing);

        var botaoStart = new ButtonModel()
        {
            Rectangle = new(x, y, width, height),
            Click = () => StartGame(),
            Text = "Start",
            Overlay = OverlayButton,
            Color = Color.White,
        };

        var botaoMenu = new ButtonModel()
        {
            Rectangle = new(x, y - (height + spacing), width, height),
            Click = () => ReturnMenu(),
            Text = "Main Menu",
            Overlay = OverlayButton,
            Color = Color.White,
        };

        ListaBotoes.Add(botaoStart);
        ListaBotoes.Add(botaoMenu);
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
        ListaBotoes.ForEach(x => x.Update(gameTime));
    }

    #endregion

    #region Draw

    public void Draw()
    {
        ListaBotoes.ForEach(x => x.Draw());
    }

    #endregion

    #region Exit

    public void Exit()
    {
    }

    #endregion
}
