using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;


namespace Galactic_Heroes
{
    public class ArcadeMode : GameState
    {
        private Player _player;
        private List<Enemy> _enemies;
        private List<Proyectil> _proyectiles;
        private int _score;
        private bool _isGameOver;
        private SpriteFont _font;

        private float _enemySpawnTimer;
        private float _enemySpawnInterval = 2f; // Comienza con enemigos generándose cada 2 segundos
        private float _difficultyIncreaseTimer;

        public ArcadeMode(Game1 game) : base(game)
        {
            _player = new Player(game.Content.Load<Texture2D>("nave_espacial"), new Vector2(400, 300));
            _enemies = new List<Enemy>();
            _proyectiles = new List<Proyectil>();
            _score = 0;
            _isGameOver = false;

            _font = game.Content.Load<SpriteFont>("ScoreFont");
        }

        public override void Update(GameTime gameTime)
        {
            if (_isGameOver)
            {
                HandleGameOverInput();
                return;
            }

            // Actualizar el jugador
            _player.Update(gameTime);

            // Actualizar proyectiles
            foreach (var proyectil in _proyectiles)
                proyectil.Update();

            // Eliminar proyectiles fuera del área visible
            _proyectiles.RemoveAll(p => p.Position.X < 0 || p.Position.X > 3000 || p.Position.Y < 0 || p.Position.Y > 3000);

            // Generar enemigos dinámicamente
            _enemySpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_enemySpawnTimer >= _enemySpawnInterval)
            {
                _enemySpawnTimer = 0;
                SpawnEnemy();
            }

            // Incrementar la dificultad con el tiempo
            _difficultyIncreaseTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_difficultyIncreaseTimer >= 30) // Incrementar cada 30 segundos
            {
                _difficultyIncreaseTimer = 0;
                _enemySpawnInterval = MathHelper.Max(0.5f, _enemySpawnInterval - 0.2f); // Enemigos más frecuentes
            }

            // Actualizar enemigos
            foreach (var enemy in _enemies)
                enemy.Update(gameTime, _player.Position);

            // Detectar colisiones entre proyectiles y enemigos
            DetectProjectileEnemyCollisions();

            // Detectar colisiones entre el jugador y enemigos
            DetectPlayerEnemyCollisions();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Dibujar jugador
            _player.Draw(spriteBatch);

            // Dibujar enemigos
            foreach (var enemy in _enemies)
                enemy.Draw(spriteBatch);

            // Dibujar proyectiles
            foreach (var proyectil in _proyectiles)
                proyectil.Draw(spriteBatch);

            // Dibujar puntaje
            spriteBatch.DrawString(_font, $"Puntaje: {_score}", new Vector2(10, 10), Color.White);

            if (_isGameOver)
            {
                // Dibujar mensaje de Game Over
                string message = $"GAME OVER\nPuntaje Final: {_score}\nPresiona Enter para volver al Menú";
                spriteBatch.DrawString(_font, message, new Vector2(200, 200), Color.Red);
            }

            spriteBatch.End();
        }

        private void SpawnEnemy()
        {
            Random random = new Random();
            Vector2 spawnPosition = new Vector2(
                random.Next(0, 3000), // Tamaño del mundo
                random.Next(0, 3000)
            );

            _enemies.Add(new Enemy(_game.Content.Load<Texture2D>("Enemy"), spawnPosition));
        }

        private void DetectProjectileEnemyCollisions()
        {
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                var enemy = _enemies[i];
                for (int j = _proyectiles.Count - 1; j >= 0; j--)
                {
                    var proyectil = _proyectiles[j];

                    if (Vector2.Distance(proyectil.Position, enemy.Position) < enemy.GetCollisionRadius())
                    {
                        _enemies.RemoveAt(i); // Eliminar enemigo
                        _proyectiles.RemoveAt(j); // Eliminar proyectil
                        _score += 10; // Incrementar puntaje
                        break;
                    }
                }
            }
        }

        private void DetectPlayerEnemyCollisions()
        {
            foreach (var enemy in _enemies)
            {
                if (Vector2.Distance(_player.Position, enemy.Position) < enemy.GetCollisionRadius())
                {
                    _player.TakeDamage(10);
                    _enemies.Remove(enemy);

                    if (_player.Health <= 0)
                    {
                        _isGameOver = true;
                        break;
                    }
                }
            }
        }

        private void HandleGameOverInput()
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                _game.ChangeState(new Menu(_game));
            }
        }
    }
}
