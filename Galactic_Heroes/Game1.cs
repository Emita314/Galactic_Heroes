using Microsoft.VisualBasic.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;
using XnaMouse = Microsoft.Xna.Framework.Input.Mouse;



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

        public enum GameState
        {
            Menu,
            Playing,
            GameOver
        }


        private GameState _currentState = GameState.Menu;
        private int _vida = 100;
        private Texture2D _healthBarTexture;
        private Texture2D _menuBackground;

        // Agregar distintos enemigos
        private Texture2D _fastEnemyTexture;
        private Texture2D _tankEnemyTexture;

        // Game Over options
        private Rectangle _playAgainButton;
        private Rectangle _exitButton;

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

            _playAgainButton = new Rectangle(300, 200, 200, 50);
            _exitButton = new Rectangle(300, 300, 200, 50);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Cargar las texturas
            _naveTexture = Content.Load<Texture2D>("Main_Character");
            _proyectilTexture = Content.Load<Texture2D>("bullet");
            _backgroundTexture = Content.Load<Texture2D>("Fondo_universo_demo");
            _enemyTexture = Content.Load<Texture2D>("Normal_Enemy"); // Cargar textura del enemigo
            _font = Content.Load<SpriteFont>("Score");
            _menuBackground = Content.Load<Texture2D>("MenuBackground");
            _healthBarTexture = Content.Load<Texture2D>("HealthBar");
            _fastEnemyTexture = Content.Load<Texture2D>("Fast_Enemy");
            _tankEnemyTexture = Content.Load<Texture2D>("Tank_Enemy");

            base.LoadContent();
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

                // Generar enemigo en una posición aleatoria
                Random random = new Random();
                Vector2 enemyPosition = new Vector2(
                    random.Next(0, (int)_mapSize.X),
                    random.Next(0, (int)_mapSize.Y)
                );

                // Selección del tipo de enemigo
                int enemyType = random.Next(0, 3); // 0: Normal, 1: Fast, 2: Tank
                Enemigo enemy;
                switch (enemyType)
                {
                    case 0:
                        enemy = new NormalEnemy(_enemyTexture, enemyPosition, _navePosition);
                        break;
                    case 1:
                        enemy = new FastEnemy(_fastEnemyTexture, enemyPosition, _navePosition);
                        break;
                    case 2:
                        enemy = new TankEnemy(_tankEnemyTexture, enemyPosition, _navePosition);
                        break;
                    default:
                        throw new Exception("Tipo de enemigo desconocido.");
                }

                _enemigos.Add(enemy);
            }
        }




        protected override void Update(GameTime gameTime)
        {
            switch (_currentState)
            {
                case GameState.Menu:
                    // Manejar entrada para cambiar al estado Playing con teclado (Enter) o ratón (clic en botones)
                    if (XnaMouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        var mousePos = XnaMouse.GetState().Position;
                        if (_playAgainButton.Contains(mousePos))
                        {
                            _currentState = GameState.Playing;
                            _vida = 100; // Reiniciar vida
                            _score = 0;  // Reiniciar puntaje
                            _enemigos.Clear(); // Limpiar enemigos previos
                            _proyectiles.Clear(); // Limpiar proyectiles previos
                        }
                        else if (_exitButton.Contains(mousePos))
                            Exit();
                    }

                    // Alternativamente, con el teclado, se usa "Enter" para empezar el juego
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        _currentState = GameState.Playing;
                        _vida = 100;
                        _score = 0;
                        _enemigos.Clear();
                        _proyectiles.Clear();
                    }
                    break;

                case GameState.Playing:
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
                    if (keyboardState.IsKeyDown(Keys.A)) _naveRotation -= 0.1f; // Rotar a la izquierda
                    if (keyboardState.IsKeyDown(Keys.D)) _naveRotation += 0.1f; // Rotar a la derecha
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
                    foreach (var proyectil in _proyectiles) proyectil.Update();
                    _proyectiles.RemoveAll(p => p.Position.X < 0 || p.Position.X > _mapSize.X || p.Position.Y < 0 || p.Position.Y > _mapSize.Y);

                    // Generar enemigos
                    SpawnEnemy(gameTime);

                    // Actualizar enemigos y verificar colisiones
                    for (int i = _enemigos.Count - 1; i >= 0; i--)
                    {
                        var enemigo = _enemigos[i];
                        enemigo.Update(_navePosition);

                        // Verificar colisión con la nave
                        if (Vector2.Distance(_navePosition, enemigo.Position) < enemigo.GetCollisionRadius())
                        {
                            _vida -= enemigo.Damage; // Restar vida según el tipo de enemigo
                            _enemigos.RemoveAt(i);
                            if (_vida <= 0) _currentState = GameState.GameOver;
                        }
                    }

                    // Detectar colisiones entre proyectiles y enemigos
                    for (int i = _enemigos.Count - 1; i >= 0; i--)
                    {
                        var enemigo = _enemigos[i];
                        for (int j = _proyectiles.Count - 1; j >= 0; j--)
                        {
                            var proyectil = _proyectiles[j];
                            if (Vector2.Distance(proyectil.Position, enemigo.Position) < enemigo.GetCollisionRadius())
                            {
                                enemigo.Vida -= 20; // Restar daño al enemigo
                                _proyectiles.RemoveAt(j); // Eliminar proyectil

                                if (enemigo.Vida <= 0) // Eliminar enemigo si su vida es 0 o menor
                                {
                                    _enemigos.RemoveAt(i);
                                    _score += enemigo.Points; // Incrementar puntaje
                                    break;
                                }
                            }
                        }
                    }

                    // Actualizar la cámara
                    UpdateCamera();

                    // Guardar el estado del teclado para el próximo frame
                    _previousKeyboardState = keyboardState;
                    break;

                case GameState.GameOver:
                    // Manejar entrada para reiniciar o salir
                    if (XnaMouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        var mousePos = XnaMouse.GetState().Position;
                        if (_playAgainButton.Contains(mousePos))
                        {
                            _vida = 100;
                            _score = 0;
                            _currentState = GameState.Playing;
                        }
                        else if (_exitButton.Contains(mousePos))
                            Exit();
                    }

                    // También puedes usar teclas como "R" para reiniciar o "Escape" para salir
                    if (Keyboard.GetState().IsKeyDown(Keys.R))
                    {
                        _vida = 100;
                        _score = 0;
                        _currentState = GameState.Playing;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
                    break;
            }

            base.Update(gameTime);
        }



        protected override void Draw(GameTime gameTime)
        {
            // Si el estado es "Playing", se aplica la cámara
            if (_currentState == GameState.Playing)
            {
                _spriteBatch.Begin(transformMatrix: _cameraTransform);  // Aplicar la cámara solo en el estado Playing
            }
            else
            {
                _spriteBatch.Begin();  // Para el resto de los estados (Menu, GameOver) no se aplica la cámara
            }
            switch (_currentState)
            {
                case GameState.Menu:
                    // Dibuja el fondo del menú
                    _spriteBatch.Draw(_menuBackground, GraphicsDevice.Viewport.Bounds, Color.White);
                    _spriteBatch.DrawString(_font, "Galactic Heroes", new Vector2(300, 100), Color.Yellow);
                    _spriteBatch.DrawString(_font, "Play", new Vector2(_playAgainButton.X, _playAgainButton.Y), Color.White);
                    _spriteBatch.DrawString(_font, "Exit", new Vector2(_exitButton.X, _exitButton.Y), Color.White);
                    break;

                case GameState.Playing:
                    // Dibuja el fondo del juego
                    _spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, (int)_mapSize.X, (int)_mapSize.Y), Color.White);

                    // Dibuja la nave con rotación
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

                    // Dibuja los proyectiles
                    foreach (var proyectil in _proyectiles)
                    {
                        proyectil.Draw(_spriteBatch);
                    }

                    // Dibuja los enemigos
                    foreach (var enemigo in _enemigos)
                    {
                        enemigo.Draw(_spriteBatch);
                    }

                    break;

                case GameState.GameOver:
                    // Dibuja el texto de Game Over
                    _spriteBatch.DrawString(_font, "Game Over", new Vector2(300, 100), Color.Red);
                    _spriteBatch.DrawString(_font, "Play Again", new Vector2(_playAgainButton.X, _playAgainButton.Y), Color.White);
                    _spriteBatch.DrawString(_font, "Exit", new Vector2(_exitButton.X, _exitButton.Y), Color.White);
                    break;
            }

            _spriteBatch.End();

            // Aquí, fuera de la transformación de la cámara, dibujamos la barra de vida y el puntaje
            if (_currentState == GameState.Playing)
            {
                _spriteBatch.Begin();  // Comenzamos un nuevo batch sin aplicar la cámara

                // Dibuja la barra de vida en una posición fija (en la esquina superior izquierda)
                _spriteBatch.Draw(_healthBarTexture, new Rectangle(10, 10, _vida * 2, 20), Color.Red);

                // Dibuja el puntaje en la pantalla en una posición fija
                _spriteBatch.DrawString(_font, $"Puntos: {_score}", new Vector2(10, 40), Color.White);

                _spriteBatch.End();  // Terminamos el batch de la UI
            }

            base.Draw(gameTime);
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
        protected float Speed;
        public int Damage;
        public int Vida;
        public int Points;

        public Enemigo(Texture2D texture, Vector2 startPosition, Vector2 targetPosition, float speed, int damage, int vida, int points)
        {
            Texture = texture;
            Position = startPosition;
            Speed = speed;
            Damage = damage;
            Vida = vida;
            Points = points;
        }

        public void Update(Vector2 navePosition)
        {
            // Movimiento hacia la nave
            Vector2 direction = Vector2.Normalize(navePosition - Position);
            Position += direction * Speed;
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
                0.1f, // Escala uniforme
                SpriteEffects.None,
                0f
            );
        }

        public float GetCollisionRadius()
        {
            return (Texture.Width / 2) * 0.1f; // Escala
        }
    }

    public class NormalEnemy : Enemigo
    {
        public NormalEnemy(Texture2D texture, Vector2 startPosition, Vector2 targetPosition)
            : base(texture, startPosition, targetPosition, 2f, 10, 50, 10)
        {
            // Velocidad: 2
            // Daño: 10
            // Vida: 50
            // Puntos: 10
        }
    }

    public class FastEnemy : Enemigo
    {
        public FastEnemy(Texture2D texture, Vector2 startPosition, Vector2 targetPosition)
            : base(texture, startPosition, targetPosition, 4f, 5, 30, 20)
        {
            // Velocidad: 4 (rápido)
            // Daño: 5 (menor)
            // Vida: 30 (más débil)
            // Puntos: 20
        }
    }

    public class TankEnemy : Enemigo
    {
        public TankEnemy(Texture2D texture, Vector2 startPosition, Vector2 targetPosition)
            : base(texture, startPosition, targetPosition, 1f, 20, 100, 30)
        {
            // Velocidad: 1 (lento)
            // Daño: 20
            // Vida: 100 (muy resistente)
            // Puntos: 30
        }
    }



}