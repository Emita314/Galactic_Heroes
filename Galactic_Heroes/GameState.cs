using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galactic_Heroes
{
    public abstract class GameState
    {
        protected Game1 _game;

        public GameState(Game1 game)
        {
            _game = game;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
