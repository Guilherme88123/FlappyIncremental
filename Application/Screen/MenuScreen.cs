using Application.Const;
using Application.Interface.Screen;
using Application.Model.MenuElements.Base;
using FlappyIncremental.Const;
using FlappyIncremental.Dto;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Drawing;
using static System.Formats.Asn1.AsnWriter;

namespace Application.Screen;

public class MenuScreen : IScreen
{
    public string ScreenCode => ScreenCodesConst.MenuScreen;

    private List<BaseElementModel> ListaBotoesPaused { get; set; } = new();
    public Texture2D Background { get; set; } = GlobalVariables.Game.Content.Load<Texture2D>("background");

    #region Initialize

    public void Initialize()
    {
        LoadBotoes();
    }

    public void LoadBotoes()
    {
        var telaWidth = GlobalVariables.Graphics.PreferredBackBufferWidth;
        var telaHeight = GlobalVariables.Graphics.PreferredBackBufferHeight;

        var width = 400;
        var height = 50;
        var x = telaWidth / 2 - width / 2;
        var y = (int)(telaHeight / 1.5 - height / 2);

        var botaoStart = new BaseElementModel()
        {
            Rectangle = new(x, y, width, height),
            Click = () => StartGame(),
            Text = "Start Game",
        };

        var botaoExit = new BaseElementModel()
        {
            Rectangle = new(x, y + height + 10, width, height),
            Click = () => GlobalVariables.Game.Exit(),
            Text = "Exit",
        };

        ListaBotoesPaused.Add(botaoStart);
        ListaBotoesPaused.Add(botaoExit);
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
        DrawBackground();
        ListaBotoesPaused.ForEach(x => x.Draw());
        DrawVersionText();
    }

    public void DrawBackground()
    {
        var scaleX = (float)GlobalVariables.Graphics.PreferredBackBufferWidth / Background.Width;
        var scaleY = (float)GlobalVariables.Graphics.PreferredBackBufferHeight / Background.Height;

        GlobalVariables.SpriteBatchBackground.Draw(
            Background,
            new Vector2(0, 0),
            null,
            Microsoft.Xna.Framework.Color.White,
            0f,
            new System.Numerics.Vector2(0.5f, 0.5f),
            new Vector2(scaleX, scaleY),
            SpriteEffects.None,
            0f);
    }

    public void DrawVersionText()
    {
        var width = GlobalVariables.Graphics.PreferredBackBufferWidth;
        var height = GlobalVariables.Graphics.PreferredBackBufferHeight;

        var text = Configuration.Version;

        var textSize = GlobalVariables.Font.MeasureString(text);

        var textPosition = new Vector2(10, height - 50);

        GlobalVariables.SpriteBatchInterface.DrawString(GlobalVariables.Font, text, textPosition, 
            Microsoft.Xna.Framework.Color.White);
    }

    #endregion

    #region Exit

    public void Exit()
    {
    }

    #endregion
}
