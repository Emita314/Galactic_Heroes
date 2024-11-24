using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Galactic_Heroes
{
    public class StoryMode : GameState
    {
        private int _currentLevel = 1;
        private Player _player;
        private List<Enemy> _enemies;
        private Boss _boss;
        private bool _bossDefeated = false;


        public StoryMode(Game1 game) : base(game)
        {
            _player = new Player(game.Content.Load<Texture2D>("nave_espacial"), new Vector2(400, 300));
            LoadLevel(_currentLevel);
        }

        public override void Update(GameTime gameTime)
        {
            _player.Update(gameTime);

            foreach (var enemy in _enemies)
                enemy.Update(gameTime, _player.Position);

            if (_boss != null)
                _boss.Update(gameTime, _player.Position);

            if (_boss != null && _boss.Health <= 0)
                _bossDefeated = true;

            if (_bossDefeated)
            {
                _currentLevel++;
                if (_currentLevel > 10)
                {
                    _game.ChangeState(new Menu(_game));
                }
                else
                {
                    LoadLevel(_currentLevel);
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            _player.Draw(spriteBatch);

            foreach (var enemy in _enemies)
                enemy.Draw(spriteBatch);

            if (_boss != null)
                _boss.Draw(spriteBatch);

            spriteBatch.End();
        }

        private void LoadLevel(int level)
        {
            _enemies = new List<Enemy>();
            var rnd = new System.Random();

            for (int i = 0; i < level * 3; i++)
            {
                _enemies.Add(new Enemy(_game.Content.Load<Texture2D>("Enemy"), new Vector2(rnd.Next(800), rnd.Next(600))));
            }

            if (level % 3 == 0)
                _boss = new Boss(_game.Content.Load<Texture2D>("Boss"), new Vector2(400, 100), level * 50);
            else
                _boss = null;
        }
    }
}
