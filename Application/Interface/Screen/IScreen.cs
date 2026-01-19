using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Application.Interface.Screen;

public interface IScreen
{
    public string ScreenCode { get; }

    void Initialize();
    void Update(GameTime gameTime);
    void Draw();
    void Exit();
}
