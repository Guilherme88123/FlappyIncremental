using Application.Dto;
using Application.Model.MenuElements.Base;
using FlappyIncremental.Dto;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Application.Model.MenuElements;

public class SwitchModel : BaseElementModel
{
    public Action<bool> Click { get; set; }
    public bool Value { get; set; }

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
            ToggleValue();
            Click?.Invoke(Value);
            DelayAtual = Delay;
        }
    }

    public void ToggleValue()
    {
        Value = !Value;
    }

    protected override string GetText()
    {
        string valueText = Value ? "ON" : "OFF";
        return $"{Text}: {valueText}";
    }
}
