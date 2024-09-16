using Microsoft.VisualBasic.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;


namespace Galactic_Heroes
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Texturas y posiciones
        private Texture2D _naveTexture;
        private Vector2 _navePosition;
        private float _naveSpeed = 5f;

        private Texture2D _proyectilTexture;
        private List<Proyectil> _proyectiles;
        private float _proyectilSpeed = 10f;

        private float _naveScale = 0.15f; // Reducir el tamaño de la nave (15% del tamaño original)
        private float _naveRotation = 0f; // Ángulo de rotación de la nave

        private float _proyectilScale = 0.1f; // Escala más pequeña para los proyectiles

        private Texture2D _backgroundTexture; // Fondo del mapa
        private Vector2 _mapSize = new Vector2(3000, 3000); // Tamaño del mapa grande

        // Control de la cámara
        private Matrix _cameraTransform;

        // Lista de enemigos
        private Texture2D _enemyTexture;
        private List<Enemigo> _enemigos;
        private float _enemySpawnTimer = 0f; // Temporizador para generar enemigos
        private float _enemySpawnInterval = 2f; // Intervalo de generación de enemigos en segundos

        // Control de teclas
        private KeyboardState _previousKeyboardState;

        private int _score = 0; // Puntaje del jugador
        private SpriteFont _font; // Fuente para dibujar el puntaje

        int naveVida = 100;  // Vida inicial de la nave
        int naveMaxVida = 100;

        // Variables de Game Over
        bool isGameOver = false;
        SpriteFont font;

        // Opción para reiniciar o salir del juego
        bool retrySelected = true; // Al principio, está seleccionado "Retry"


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Posición inicial de la nave
            _navePosition = new Vector2(
                _mapSize.X / 2, // Centrar la nave en el mapa
                _mapSize.Y / 2
            );

            // Lista para los proyectiles disparados y enemigos
            _proyectiles = new List<Proyectil>();
            _enemigos = new List<Enemigo>();

            // Inicializar el estado del teclado
            _previousKeyboardState = Keyboard.GetState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Cargar las texturas
            _naveTexture = Content.Load<Texture2D>("nave_espacial");
            _proyectilTexture = Content.Load<Texture2D>("bullet");
            _backgroundTexture = Content.Load<Texture2D>("Fondo_universo_demo");
            _enemyTexture = Content.Load<Texture2D>("Enemy"); // Cargar textura del enemigo
            _font = Content.Load<SpriteFont>("Score");
            // Cargar la fuente para texto
            font = Content.Load<SpriteFont>("Game_Over");
        }

        private void UpdateCamera()
        {
            // Centrar la cámara en la posición de la nave
            var viewport = _graphics.GraphicsDevice.Viewport;
            var cameraPosition = _navePosition - new Vector2(viewport.Width / 2, viewport.Height / 2);

            // Asegurarse de que la cámara no salga de los bordes del mapa
            cameraPosition.X = MathHelper.Clamp(cameraPosition.X, 0, _mapSize.X - viewport.Width);
            cameraPosition.Y = MathHelper.Clamp(cameraPosition.Y, 0, _mapSize.Y - viewport.Height);

            // Crear la transformación de la cámara
            _cameraTransform = Matrix.CreateTranslation(new Vector3(-cameraPosition, 0));
        }

        private void SpawnEnemy(GameTime gameTime)
        {
            _enemySpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_enemySpawnTimer >= _enemySpawnInterval)
            {
                _enemySpawnTimer = 0f;

                // Generar enemigo en una posición aleatoria dentro del mapa
                Random random = new Random();
                Vector2 enemyPosition = new Vector2(
                    random.Next(0, (int)_mapSize.X),
                    random.Next(0, (int)_mapSize.Y)
                );

                _enemigos.Add(new Enemigo(_enemyTexture, enemyPosition, _navePosition));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            // Obtener el estado del teclado
            var keyboardState = Keyboard.GetState();

            // Movimiento de la nave
            if (keyboardState.IsKeyDown(Keys.W))
            {
                _navePosition += new Vector2(
                    (float)Math.Cos(_naveRotation) * _naveSpeed,
                    (float)Math.Sin(_naveRotation) * _naveSpeed
                );
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                _naveRotation -= 0.1f; // Rotar a la izquierda
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                _naveRotation += 0.1f; // Rotar a la derecha
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                _navePosition -= new Vector2(
                    (float)Math.Cos(_naveRotation) * _naveSpeed,
                    (float)Math.Sin(_naveRotation) * _naveSpeed
                );
            }

            // Disparo de proyectiles
            if (keyboardState.IsKeyDown(Keys.Space) && _previousKeyboardState.IsKeyUp(Keys.Space))
            {
                Vector2 bulletPosition = _navePosition + new Vector2(
                    (float)Math.Cos(_naveRotation) * (_naveTexture.Width * _naveScale / 2),
                    (float)Math.Sin(_naveRotation) * (_naveTexture.Height * _naveScale / 2)
                );

                _proyectiles.Add(new Proyectil(
                    _proyectilTexture,
                    bulletPosition,
                    _naveRotation,
                    _proyectilSpeed,
                    _proyectilScale
                ));
            }

            // Actualizar proyectiles
            foreach (var proyectil in _proyectiles)
            {
                proyectil.Update();
            }

            // Eliminar proyectiles fuera de la pantalla
            _proyectiles.RemoveAll(p => p.Position.X < 0 || p.Position.X > _mapSize.X || p.Position.Y < 0 || p.Position.Y > _mapSize.Y);

            // Generar enemigos
            SpawnEnemy(gameTime);

            // Actualizar enemigos y su movimiento hacia la nave
            foreach (var enemigo in _enemigos)
            {
                enemigo.Update(_navePosition);
            }

            // Detectar colisiones entre proyectiles y enemigos
            for (int i = _enemigos.Count - 1; i >= 0; i--)
            {
                var enemigo = _enemigos[i];
                for (int j = _proyectiles.Count - 1; j >= 0; j--)
                {
                    var proyectil = _proyectiles[j];

                    // Ajustar la distancia de colisión para tomar en cuenta la escala del enemigo
                    if (Vector2.Distance(proyectil.Position, enemigo.Position) < enemigo.GetCollisionRadius())
                    {
                        _enemigos.RemoveAt(i); // Eliminar enemigo
                        _proyectiles.RemoveAt(j); // Eliminar proyectil
                        _score++; // Incrementar puntaje
                        break;
                    }
                }
            }


            // Actualizar la cámara
            UpdateCamera();

            // Guardar el estado del teclado para el próximo frame
            _previousKeyboardState = keyboardState;

            // Si es Game Over, manejar las opciones de reinicio o salida
            if (isGameOver)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    retrySelected = !retrySelected;  // Alternar entre "Retry" y "Exit"
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    if (retrySelected)
                    {
                        // Reiniciar juego
                        naveVida = naveMaxVida;
                        isGameOver = false;
                        _navePosition = _navePosition = new Vector2(
                            _mapSize.X / 2, // Centrar la nave en el mapa
                            _mapSize.Y / 2
                        ); // Restablecer posición de la nave
                        _enemigos.Clear();
                        _proyectiles.Clear();
                        _score = 0;
                    }
                    else
                    {
                        // Cerrar juego
                        Exit();
                    }
                }

                return; // No actualizar el resto si estamos en pantalla de Game Over

                base.Update(gameTime);
            }

            CheckCollisions();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(transformMatrix: _cameraTransform);

            // Dibujar el fondo del mapa
            _spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, (int)_mapSize.X, (int)_mapSize.Y), Color.White);

            // Dibujar la nave con rotación
            _spriteBatch.Draw(
                _naveTexture,
                _navePosition,
                null,
                Color.White,
                _naveRotation,
                new Vector2(_naveTexture.Width / 2, _naveTexture.Height / 2),
                _naveScale,
                SpriteEffects.None,
                0f
            );

            // Dibujar los proyectiles
            foreach (var proyectil in _proyectiles)
            {
                proyectil.Draw(_spriteBatch);
            }

            // Dibujar los enemigos
            foreach (var enemigo in _enemigos)
            {
                enemigo.Draw(_spriteBatch);
            }

            _spriteBatch.End();

            // Dibujar el puntaje en la pantalla
            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, $"Puntos: {_score}", new Vector2(10, 10), Color.White);
            _spriteBatch.End();
            _spriteBatch.Begin();
            // Dibujar la barra de vida
            DrawHealthBar();

            // Dibujar Game Over si aplica
            if (isGameOver)
            {
                _spriteBatch.DrawString(_font, "GAME OVER", new Vector2(400, 200), Color.Red);
                _spriteBatch.DrawString(_font, "Retry", new Vector2(400, 250), retrySelected ? Color.Yellow : Color.White);
                _spriteBatch.DrawString(_font, "Exit", new Vector2(400, 300), retrySelected ? Color.White : Color.Yellow);
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        // Verificar colisiones entre nave y enemigos
        private void CheckCollisions()
        {
            foreach (var enemigo in _enemigos.ToList())
            {
                if (Vector2.Distance(_navePosition, enemigo.Position) < (enemigo.GetCollisionRadius() + (_naveTexture.Width / 2 * _naveScale)))
                {
                    naveVida -= 10;
                    if (naveVida <= 0)
                    {
                        isGameOver = true;
                    }
                    _enemigos.Remove(enemigo);
                }
            }
        }
        private void DrawHealthBar()
        {
            int barWidth = 200;
            int barHeight = 20;
            float healthPercent = (float)naveVida / naveMaxVida;

            Texture2D healthBarTexture = new Texture2D(GraphicsDevice, 1, 1);
            healthBarTexture.SetData(new[] { Color.White });

            _spriteBatch.Draw(healthBarTexture, new Rectangle(50, 50, barWidth, barHeight), Color.Red);
            _spriteBatch.Draw(healthBarTexture, new Rectangle(50, 50, (int)(barWidth * healthPercent), barHeight), Color.Green);
        }
    }

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
            // Mover el proyectil en la dirección de su rotación
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

    public class Enemigo
    {
        public Vector2 Position;
        public Texture2D Texture;
        private float _speed = 2f;
        private float _scale = 0.1f; // Escala del enemigo

        public Enemigo(Texture2D texture, Vector2 startPosition, Vector2 targetPosition)
        {
            Texture = texture;
            Position = startPosition;
        }

        public void Update(Vector2 navePosition)
        {
            // Mover el enemigo hacia la nave
            Vector2 direction = Vector2.Normalize(navePosition - Position);
            Position += direction * _speed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Texture,
                Position,
                null,
                Color.White,
                0f,
                new Vector2(Texture.Width / 2, Texture.Height / 2),
                _scale, // Escala más pequeña para el enemigo
                SpriteEffects.None,
                0f
            );
        }

        // Método para obtener el radio de colisión basado en la escala
        public float GetCollisionRadius()
        {
            return (Texture.Width / 2) * _scale;
        }
    }

}



