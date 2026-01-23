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
    public Texture2D Title { get; set; } = GlobalVariables.Game.Content.Load<Texture2D>("title");
    public Texture2D OverlayButton { get; set; } = GlobalVariables.Game.Content.Load<Texture2D>("button_overlay");

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
        var height = 100;
        var x = telaWidth / 2 - width / 2;
        var y = (int)(telaHeight / 1.6 - height / 2);

        var botaoStart = new BaseElementModel()
        {
            Rectangle = new(x, y, width, height),
            Click = () => StartGame(),
            Text = "Start",
            Overlay = OverlayButton,
            Color = Color.White,
        };

        var botaoOpcoes = new BaseElementModel()
        {
            Rectangle = new(x, y + (height + 10), width, height),
            Click = () => ToggleOptions(),
            Text = "Options",
            Overlay = OverlayButton,
            Color = Color.White,
        };

        var botaoExit = new BaseElementModel()
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
        ListaBotoesOptions.ForEach(x => x.Draw());
    }

    #endregion

    #region Exit

    public void Exit()
    {
    }

    #endregion
}
