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
    public Dictionary<string, IScreen> Screens = new();
    public IScreen ActualScreen { get; set; } = null;

    public string InitialScreenCode = ScreenCodesConst.PlayScreen;

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
        var spriteBatchEntities = new SpriteBatch(GraphicsDevice);
        var spriteBatchInterface = new SpriteBatch(GraphicsDevice);

        var pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData([Color.White]);

        GlobalVariables.Font = Content.Load<SpriteFont>("DefaultFont");
        GlobalVariables.SpriteBatchEntities = spriteBatchEntities;
        GlobalVariables.SpriteBatchInterface = spriteBatchInterface;
        GlobalVariables.Pixel = pixel;
    }

    protected override void Initialize()
    {
        Screens = GlobalVariables.GetService<IEnumerable<IScreen>>().ToDictionary(x => x.ScreenCode, x => x);
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
        ActualScreen.Draw();

        base.Draw(gameTime);
    }

    public void ChangeScreen(string screenCode)
    {
        if (Screens.ContainsKey(screenCode))
        {
            if (ActualScreen is not null) ActualScreen.Exit();

            if (!Screens.TryGetValue(screenCode, out var screen))
            {
                throw new Exception($"Screen with code {screenCode} not found.");
            }

            ActualScreen = screen;

            ActualScreen.Initialize();
        }
    }
}
