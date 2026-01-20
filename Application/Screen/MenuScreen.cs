using Application.Const;
using Application.Interface.Screen;
using Application.Model.MenuElements.Base;
using FlappyIncremental.Dto;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Application.Screen;

public class MenuScreen : IScreen
{
    public string ScreenCode => ScreenCodesConst.MenuScreen;

    private List<BaseElementModel> ListaBotoesPaused { get; set; } = new()
    {
        new() { Rectangle = new(100, 100, 500, 100), Click = () => StartGame(), Text = "Start Game" }
    };

    #region Initialize

    public void Initialize()
    {
    }

    #endregion

    #region Update

    public void Update(GameTime gameTime)
    {
        ListaBotoesPaused.ForEach(x => x.Update(gameTime));
    }

    public static void StartGame()
    {
        GlobalVariables.Game.ChangeScreen(ScreenCodesConst.PlayScreen);
    }

    #endregion

    #region Draw

    public void Draw()
    {
        GlobalVariables.Graphics.GraphicsDevice.Clear(Color.DarkSeaGreen);

        ListaBotoesPaused.ForEach(x => x.Draw());
    }

    #endregion

    #region Exit

    public void Exit()
    {
    }

    #endregion
}
