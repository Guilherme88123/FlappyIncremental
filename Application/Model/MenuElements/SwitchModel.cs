using Application.Dto;
using FlappyIncremental.Dto;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Application.Model.MenuElements.Base;

namespace Application.Model.MenuElements;

public class SwitchModel : BaseElementModel
{
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
            ToggleValue();
            ClickSound.Play(GlobalOptions.SfxVolume, 0f, 0f);
            DelayAtual = Delay;
        }
    }

    public void ToggleValue()
    {
        Value = !Value;
    }
}
