using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galactic_Heroes
{
    public class Menu : GameState
    {
        private SpriteFont _font;
        private string[] _options = { "Modo Historia", "Modo Arcade" };
        private int _selectedIndex = 0;

        public Menu(Game1 game) : base(game) { }

        public override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Up))
                _selectedIndex = (_selectedIndex - 1 + _options.Length) % _options.Length;

            if (keyboardState.IsKeyDown(Keys.Down))
                _selectedIndex = (_selectedIndex + 1) % _options.Length;

            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                if (_selectedIndex == 0)
                    _game.ChangeState(new StoryMode(_game));
                else
                    _game.ChangeState(new ArcadeMode(_game));
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _font = _game.Content.Load<SpriteFont>("MenuFont");
            spriteBatch.Begin();

            for (int i = 0; i < _options.Length; i++)
            {
                Color color = (i == _selectedIndex) ? Color.Yellow : Color.White;
                spriteBatch.DrawString(_font, _options[i], new Vector2(400, 200 + i * 50), color);
            }

            spriteBatch.End();
        }
    }
}
