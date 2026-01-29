using Application.Dto;
using Application.Model.MenuElements.Base;
using FlappyIncremental.Dto;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Application.Model.MenuElements;

public class DropdownModel : BaseElementModel
{
    public bool IsOpen { get; set; }

    public List<DropdownItemDto> ListItens { get; set; }
    public int SelectedItem { get; set; }
    public Action<DropdownItemDto> ValueUpdate { get; set; }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        var mouse = Mouse.GetState();

        if (mouse.LeftButton == ButtonState.Pressed &&
            DelayAtual < 0 &&
            !GlobalVariables.IsMouseDown &&
            IsHover)
        {
            ClickSound.Play(GlobalOptions.SfxVolumeFloat, 0f, 0f);
            ToggleOpen();
            DelayAtual = Delay;
        }

        UpdateOptions();
    }

    private void UpdateOptions()
    {
        var mouse = Mouse.GetState();
        var mousePos = new Point(mouse.X, mouse.Y);

        foreach (var item in ListItens)
        {
            item.IsHover = item.Rectangle.Contains(mousePos);

            if (mouse.LeftButton == ButtonState.Pressed &&
                DelayAtual < 0 &&
                !GlobalVariables.IsMouseDown &&
                item.IsHover)
            {
                ClickSound.Play(GlobalOptions.SfxVolumeFloat, 0f, 0f);
                ToggleOpen();
                SelectItem(item.Id);
                DelayAtual = Delay;
            }
        }
    }

    private void ToggleOpen()
    {
        IsOpen = !IsOpen;

        if (IsOpen) UpdateOptionsRectangle();
    }

    private void SelectItem(int id)
    {
        if (SelectedItem != id)
        {
            var item = ListItens.First(x => x.Id == id);
            ValueUpdate?.Invoke(item);
        }

        SelectedItem = id;
    }

    private void UpdateOptionsRectangle()
    {
        var border = 30;

        foreach (var item in ListItens)
        {
            var x = Rectangle.X + border;
            var y = Rectangle.Y + Rectangle.Height + (Rectangle.Height / 2) * item.Id;
            var width = Rectangle.Width - border * 2;
            var height = Rectangle.Height / 2;

            item.Rectangle = new(x, y, width, height);
        }
    }

    public override void Draw()
    {
        base.Draw();

        if (IsOpen)
        {
            DrawDropdownOverlay();
            DrawDropdownItems();
        }
    }

    protected override string GetText()
    {
        var optionSelected = ListItens.FirstOrDefault(x => x.Id == SelectedItem);

        if (optionSelected is null) return $"{Text}: N/A";

        return $"{Text}: {optionSelected.Text}";
    }

    private void DrawDropdownItems()
    {
        foreach (var item in ListItens)
        {
            var textSize = GlobalVariables.Font.MeasureString(item.Text);

            var x = item.Rectangle.X + item.Rectangle.Width / 2 - textSize.X / 2;
            var y = item.Rectangle.Y + item.Rectangle.Height / 2 - textSize.Y / 2;

            GlobalVariables.SpriteBatchInterface.DrawString(GlobalVariables.Font, item.Text, new(x, y), Color.White);
        }
    }

    private void DrawDropdownOverlay()
    {
        foreach (var item in ListItens)
        {
            var color = item.IsHover ? Color.DarkGray * 0.7f : Color.DarkGray;
            GlobalVariables.SpriteBatchInterface.Draw(GlobalVariables.Pixel, item.Rectangle, color);
        }
    }
}

public class DropdownItemDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public Rectangle Rectangle { get; set; }
    public bool IsHover { get; set; }
    public object Value { get; set; }
}