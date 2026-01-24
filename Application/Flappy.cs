using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using FlappyIncremental.Dto;
using Application.Enum;
using Application.Interface.Menu;
using System.Collections.Generic;
using FlappyIncremental.Model.Entities.Base;
using Application.Model.Entities;
using System.Linq;
using System.Threading.Tasks;
using Application.Interface.Screen;
using Application.Const;

namespace FlappyIncremental;

public class Flappy : Game
{
    public Dictionary<string, Type> Screens = new();
    public IScreen ActualScreen { get; set; } = null;

    public string InitialScreenCode = ScreenCodesConst.MenuScreen;

    public Flappy()
    {
        var graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = 1920;
        graphics.PreferredBackBufferHeight = 1080;
        //graphics.IsFullScreen = true;
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1d / 120d);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        graphics.ApplyChanges();

        GlobalVariables.Graphics = graphics;
    }

    protected override void LoadContent()
    {
        var spriteBatchBackground = new SpriteBatch(GraphicsDevice);
        var spriteBatchEntities = new SpriteBatch(GraphicsDevice);
        var spriteBatchInterface = new SpriteBatch(GraphicsDevice);

        var pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData([Color.White]);

        GlobalVariables.Font = Content.Load<SpriteFont>("DefaultFont");
        GlobalVariables.SpriteBatchBackground = spriteBatchBackground;
        GlobalVariables.SpriteBatchEntities = spriteBatchEntities;
        GlobalVariables.SpriteBatchInterface = spriteBatchInterface;
        GlobalVariables.Pixel = pixel;
    }

    protected override void Initialize()
    {
        Screens = GlobalVariables.GetService<IEnumerable<IScreen>>().ToDictionary(x => x.ScreenCode, x => x.GetType());
        ChangeScreen(InitialScreenCode);

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        ActualScreen.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        var color = new Color(25, 25, 20);
        GlobalVariables.Graphics.GraphicsDevice.Clear(color);

        GlobalVariables.SpriteBatchBackground.Begin();
        GlobalVariables.SpriteBatchEntities.Begin();
        GlobalVariables.SpriteBatchInterface.Begin();

        ActualScreen.Draw();

        GlobalVariables.SpriteBatchBackground.End();
        GlobalVariables.SpriteBatchEntities.End();
        GlobalVariables.SpriteBatchInterface.End();

        base.Draw(gameTime);
    }

    public void ChangeScreen(string screenCode)
    {
        if (Screens.ContainsKey(screenCode))
        {
            if (ActualScreen is not null) ActualScreen.Exit();

            if (!Screens.TryGetValue(screenCode, out var screenType))
            {
                throw new Exception($"Screen with code {screenCode} not found.");
            }

            ActualScreen = (IScreen)GlobalVariables.GetService(screenType);

            ActualScreen.Initialize();
        }
    }
}
