using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace Galactic_Heroes
{
    public class Player
    {
        private Texture2D _texture;
        public Vector2 Position;
        public int Health { get; private set; } = 100;
        private float _speed = 5f;
        private float _rotation = 0f;
        private float _scale = 0.15f;

        public Player(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            Position = position;
        }

        public void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            // Movimiento
            if (keyboardState.IsKeyDown(Keys.W))
            {
                Position += new Vector2(
                    (float)Math.Cos(_rotation) * _speed,
                    (float)Math.Sin(_rotation) * _speed
                );
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                Position -= new Vector2(
                    (float)Math.Cos(_rotation) * _speed,
                    (float)Math.Sin(_rotation) * _speed
                );
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                _rotation -= 0.1f;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                _rotation += 0.1f;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _texture,
                Position,
                null,
                Color.White,
                _rotation,
                new Vector2(_texture.Width / 2, _texture.Height / 2),
                _scale,
                SpriteEffects.None,
                0f
            );
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;
            if (Health < 0) Health = 0;
        }
    }
}
