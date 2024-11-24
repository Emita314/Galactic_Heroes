using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Galactic_Heroes
{
    public class Proyectil
    {
        public Vector2 Position;
        private float _rotation;
        private float _speed;
        private Texture2D _texture;
        private float _scale;

        public Proyectil(Texture2D texture, Vector2 startPosition, float rotation, float speed, float scale)
        {
            _texture = texture;
            Position = startPosition;
            _rotation = rotation;
            _speed = speed;
            _scale = scale;
        }

        public void Update()
        {
            // Movimiento del proyectil en la dirección de su rotación
            Position += new Vector2(
                (float)Math.Cos(_rotation) * _speed,
                (float)Math.Sin(_rotation) * _speed
            );
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
    }
}
