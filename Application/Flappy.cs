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

    private Effect CrtEffect { get; set; }
    private RenderTarget2D RtCrtEffect { get; set; }

    private int _frames;
    private float _fps;
    private double _elapsedTime;

    public Flappy()
    {
        var graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = GlobalOptions.WidthSize;
        graphics.PreferredBackBufferHeight = GlobalOptions.HeightSize;
        graphics.HardwareModeSwitch = false;
        graphics.IsFullScreen = GlobalOptions.Fullscreen; 
        graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;
        TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);
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

        GlobalVariables.DefaultFont = Content.Load<SpriteFont>("DefaultFont");
        GlobalVariables.LittleFont = Content.Load<SpriteFont>("LittleFont");
        GlobalVariables.SpriteBatchBackground = spriteBatchBackground;
        GlobalVariables.SpriteBatchEntities = spriteBatchEntities;
        GlobalVariables.SpriteBatchInterface = spriteBatchInterface;
        GlobalVariables.Pixel = pixel;

        Music = Content.Load<Song>("back_music");
        MediaPlayer.Volume = GlobalOptions.MusicVolumeFloat;
        MediaPlayer.Play(Music);

        CrtEffect = Content.Load<Effect>("crt-lottes-mg"); 
        CrtEffect.Parameters["hardScan"]?.SetValue(-6.0f);
        CrtEffect.Parameters["hardPix"]?.SetValue(-6.0f);
        CrtEffect.Parameters["warpX"]?.SetValue(0.031f);
        CrtEffect.Parameters["warpY"]?.SetValue(0.031f);
        CrtEffect.Parameters["maskDark"]?.SetValue(0.5f);
        CrtEffect.Parameters["maskLight"]?.SetValue(1.5f);
        CrtEffect.Parameters["scaleInLinearGamma"]?.SetValue(1.0f);
        CrtEffect.Parameters["shadowMask"]?.SetValue(4.0f);
        CrtEffect.Parameters["brightboost"]?.SetValue(1.0f);
        CrtEffect.Parameters["hardBloomScan"]?.SetValue(-1.5f);
        CrtEffect.Parameters["hardBloomPix"]?.SetValue(-2.0f);
        CrtEffect.Parameters["bloomAmount"]?.SetValue(0.3f);
        CrtEffect.Parameters["shape"]?.SetValue(2.0f);

        CrtEffect.Parameters["textureSize"].SetValue(new Vector2(GlobalOptions.WidthSize, GlobalOptions.HeightSize));
        CrtEffect.Parameters["videoSize"].SetValue(new Vector2(GlobalOptions.WidthSize, GlobalOptions.HeightSize));
        CrtEffect.Parameters["outputSize"].SetValue(new Vector2(GlobalOptions.WidthSize, GlobalOptions.HeightSize));
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
        UpdateFpsCounter(gameTime);
        
        base.Update(gameTime);
    }

    private void UpdateMouseState()
    {
        var mouse = Mouse.GetState();

        GlobalVariables.IsMouseDown = mouse.LeftButton == ButtonState.Pressed;
    }

    private void UpdateFpsCounter(GameTime gameTime)
    {
        _frames++;
        _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

        if (_elapsedTime >= 1.0) // Atualiza FPS a cada 1s
        {
            _fps = _frames / (float)_elapsedTime;
            _frames = 0;
            _elapsedTime = 0;
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        GlobalVariables.SpriteBatchBackground.Begin(transformMatrix: GlobalVariables.CameraOffset);
        GlobalVariables.SpriteBatchEntities.Begin(transformMatrix: GetScreenScaleMatrix());
        GlobalVariables.SpriteBatchInterface.Begin(effect: CrtEffect);

        ActualScreen.Draw();
        //DrawFps(GlobalVariables.SpriteBatchEntities);

        GlobalVariables.SpriteBatchBackground.End();
        GlobalVariables.SpriteBatchEntities.End();
        GlobalVariables.SpriteBatchInterface.End();

        base.Draw(gameTime);
    }

    private void DrawFps(SpriteBatch spriteBatch)
    {
        string fpsText = $"FPS: {_fps:F0}";
        spriteBatch.DrawString(GlobalVariables.DefaultFont, fpsText, new Vector2(10, 10), Color.White);
    }

    private Matrix GetScreenScaleMatrix()
    {
        float scaleX = (float)GraphicsDevice.Viewport.Width / 1920f;
        float scaleY = (float)GraphicsDevice.Viewport.Height / 1080f;

        return Matrix.CreateScale(scaleX, scaleY, 1f);
    }

    public void ChangeScreen(string screenCode)
    {
        if (Screens.ContainsKey(screenCode))
        {
            if (!Screens.TryGetValue(screenCode, out var screenType))
            {
                throw new Exception($"Screen with code {screenCode} not found.");
            }

            ActualScreen = (IScreen)GlobalVariables.GetService(screenType);

            ActualScreen.Initialize();
        }
    }
}
