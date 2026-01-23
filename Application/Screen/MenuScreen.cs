using Application.Const;
using Application.Interface.Screen;
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
    public Texture2D Background { get; set; } = GlobalVariables.Game.Content.Load<Texture2D>("background");

    public bool IsOptionsEnable { get; set; }

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
            Text = "Start",
        };

        var botaoOpcoes = new BaseElementModel()
        {
            Rectangle = new(x, y + (height + 10), width, height),
            Click = () => ToggleOptions(),
            Text = "Options",
        };

        var botaoExit = new BaseElementModel()
        {
            Rectangle = new(x, y + (height + 10) * 2, width, height),
            Click = () => GlobalVariables.Game.Exit(),
            Text = "Exit",
        };

        ListaBotoes.Add(botaoStart);
        ListaBotoes.Add(botaoOpcoes);
        ListaBotoes.Add(botaoExit);
    }

    #endregion

    #region Update

    public void Update(GameTime gameTime)
    {
        ListaBotoes.ForEach(x => x.Update(gameTime));

        if (IsOptionsEnable)
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

    #endregion

    #region Draw

    public void Draw()
    {
        DrawBackground();

        ListaBotoes.ForEach(x => x.Draw());

        DrawVersionText();
        DrawTitle();

        if (IsOptionsEnable)
        {
            DrawOptionsMenu();
        }
    }

    public void DrawBackground()
    {
        var color = new Color(25, 25, 20);
        GlobalVariables.Graphics.GraphicsDevice.Clear(color);

        return;

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

        var textPosition = new Vector2(15, height - 55);

        GlobalVariables.SpriteBatchInterface.DrawString(GlobalVariables.Font, text, textPosition, 
            Microsoft.Xna.Framework.Color.White);
    }

    public void DrawTitle()
    {
        var width = GlobalVariables.Graphics.PreferredBackBufferWidth;
        var height = GlobalVariables.Graphics.PreferredBackBufferHeight;

        var text = Configuration.Title;

        var textSize = GlobalVariables.Font.MeasureString(text);

        var x = width / 2 - textSize.X / 2;
        var y = 300;

        var textPosition = new Vector2(x, y);

        var border = 20;
        var rect = new Rectangle((int)x - border, y - border, (int)textSize.X + border * 2, (int)textSize.Y + border * 2);

        GlobalVariables.SpriteBatchInterface.Draw(GlobalVariables.Pixel, rect, Color.Red);
        GlobalVariables.SpriteBatchInterface.DrawString(GlobalVariables.Font, text, textPosition, Color.White);
    }

    public void DrawOptionsMenu()
    {
        ListaBotoesOptions.ForEach(x => x.Draw());
    }

    #endregion

    #region Exit

    public void Exit()
    {
    }

    #endregion
}
