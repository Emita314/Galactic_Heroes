using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Galactic_Heroes
{
    public class Boss : Enemy
    {
        public int Health { get; private set; }

        public Boss(Texture2D texture, Vector2 startPosition, int health) : base(texture, startPosition)
        {
            Health = health;
            _speed = 1f; // Jefe se mueve más lento
            _scale = 0.3f; // Jefe es más grande
        }

        public override void Update(GameTime gameTime, Vector2 targetPosition)
        {
            // Movimiento del jefe hacia el jugador, más lento
            base.Update(gameTime, targetPosition);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // Dibujar barra de vida del jefe
            Texture2D healthBar = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            healthBar.SetData(new[] { Color.Red });

            spriteBatch.Draw(
                healthBar,
                new Rectangle((int)Position.X - 50, (int)Position.Y - 40, Health / 2, 5),
                Color.Red
            );
        }
    }
}
