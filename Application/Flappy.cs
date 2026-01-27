using Application.Const;
using Application.Dto;
using Application.Interface.Screen;
using FlappyIncremental.Dto;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlappyIncremental;

public class Flappy : Game
{
    public Dictionary<string, Type> Screens = new();
    public IScreen ActualScreen { get; set; } = null;

    public string InitialScreenCode = ScreenCodesConst.MenuScreen;

    private Song Music {  get; set; }

    public Flappy()
    {
        var graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = 1920;
        graphics.PreferredBackBufferHeight = 1080;
        graphics.HardwareModeSwitch = false;
        graphics.IsFullScreen = true;
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

        Music = Content.Load<Song>("back_music");
        MediaPlayer.Volume = GlobalOptions.MusicVolumeFloat;
        MediaPlayer.Play(Music);
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

        UpdateMouseState();
        
        base.Update(gameTime);
    }

    private void UpdateMouseState()
    {
        var mouse = Mouse.GetState();

        GlobalVariables.IsMouseDown = mouse.LeftButton == ButtonState.Pressed;
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
