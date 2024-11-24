using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galactic_Heroes
{
    public class Enemy
    {
        public Vector2 Position;
        protected Texture2D _texture;
        protected float _speed = 2f;
        protected float _scale = 0.1f;

        public Enemy(Texture2D texture, Vector2 startPosition)
        {
            _texture = texture;
            Position = startPosition;
        }

        public virtual void Update(GameTime gameTime, Vector2 targetPosition)
        {
            // Movimiento hacia el jugador
            Vector2 direction = Vector2.Normalize(targetPosition - Position);
            Position += direction * _speed;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _texture,
                Position,
                null,
                Color.White,
                0f,
                new Vector2(_texture.Width / 2, _texture.Height / 2),
                _scale,
                SpriteEffects.None,
                0f
            );
        }

        public float GetCollisionRadius()
        {
            return (_texture.Width / 2) * _scale;
        }
    }
}
