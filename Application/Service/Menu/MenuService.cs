using Application.Interface.Menu;
using Application.Model.MenuElements.Base;
using Application.Model.MenuElements.Button;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FlappyIncremental.Dto;
using System.Collections.Generic;

namespace Application.Service.Menu;

public class MenuService : IMenuService
{
    private List<BaseElementModel> MenuElements = new();
    private Rectangle Rectangle = new();

    private float Delay = 0.3f;
    private float DelayAtual { get; set; }

    private List<(int, int)> WindowsSizes = new()
    {
        (800, 600),
        (1024, 768),
        (1280, 720),
        (1920, 1080)
    };

    private int CurrentWindowSizeIndex = 0;

    public MenuService()
    {
        MenuElements.Add(new ButtonModel());
        MenuElements.Add(new ButtonModel());
        MenuElements.Add(new ButtonModel());
        AtualizarMenu();
    }

    public void DrawMenu()
    {
        GlobalVariables.SpriteBatchInterface.Draw(GlobalVariables.Pixel, Rectangle, Color.DarkGray);
        
        for (var i = 0; i < MenuElements.Count; i++)
        {
            var element = MenuElements[i];
            var elementX = Rectangle.X + (Rectangle.Width / 2) - (element.Rectangle.Width / 2);
            var elementY = Rectangle.Y + 20 + (i * (element.Rectangle.Height + 10));
            element.Draw();
        }
    }

    public void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();
        var mousePos = new Point(mouse.X, mouse.Y);

        DelayAtual -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        foreach (var element in MenuElements)
        {
            var hovering = element.Rectangle.Contains(mousePos);
            if (hovering)
            {
                element.IsHover = true;

                if (mouse.LeftButton == ButtonState.Pressed && DelayAtual < 0)
                {
                    element.Click?.Invoke();
                    DelayAtual = Delay;
                }
            }
            else
            {
                element.IsHover = false;
            }
        }
    }

    private void ToggleWindowSize()
    {
        CurrentWindowSizeIndex = (CurrentWindowSizeIndex + 1) % WindowsSizes.Count;
        var (width, height) = WindowsSizes[CurrentWindowSizeIndex];
        GlobalVariables.Graphics.PreferredBackBufferWidth = width;
        GlobalVariables.Graphics.PreferredBackBufferHeight = height;
        GlobalVariables.Graphics.ApplyChanges();
        AtualizarMenu();
    }

    private void AtualizarMenu()
    {
        var width = GlobalVariables.Graphics.PreferredBackBufferWidth / 2;
        var height = GlobalVariables.Graphics.PreferredBackBufferHeight - 100;
        var x = GlobalVariables.Graphics.PreferredBackBufferWidth / 4;
        var y = 50;

        Rectangle = new Rectangle(x, y, width, height);

        MenuElements[0].Rectangle = new Rectangle(0, 0, width - 20, 50);
        MenuElements[0].Text = $"Toggle Window Size";
        MenuElements[0].Click = () => ToggleWindowSize();

        MenuElements[1].Rectangle = new Rectangle(0, 0, width - 20, 50);
        MenuElements[1].Text = "Toggle Fullscreen";
        MenuElements[1].Click = () => GlobalVariables.Graphics.ToggleFullScreen();

        MenuElements[2].Rectangle = new Rectangle(0, 0, width - 20, 50);
        MenuElements[2].Text = "Exit";
        MenuElements[2].Click = () => GlobalVariables.Game.Exit();


    }
}
