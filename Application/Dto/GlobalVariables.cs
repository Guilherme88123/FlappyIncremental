using Application.Interface.Screen;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FlappyIncremental.Dto;

public static class GlobalVariables
{
    public static GraphicsDeviceManager Graphics;
    public static Texture2D Pixel;
    public static SpriteFont DefaultFont;
    public static SpriteFont LittleFont;

    public static SpriteBatch SpriteBatchBackground;
    public static SpriteBatch SpriteBatchEntities;
    public static SpriteBatch SpriteBatchInterface;

    public static Flappy Game;

    public static IServiceProvider ServiceProvider { get; set; }

    public static bool IsMouseDown { get; set; }

    public static T GetService<T>() where T : notnull
        => ServiceProvider.GetRequiredService<T>();

    public static object GetService(Type type) => ServiceProvider.GetRequiredService(type);

    public static Vector2 CameraPosition { get; set; } = Vector2.Zero;
    public static float CameraZoom { get; set; } = 1.0f;
    public static Matrix CameraOffset => Matrix.CreateScale(CameraZoom) * Matrix.CreateTranslation(-CameraPosition.X, -CameraPosition.Y, 0f);
}
