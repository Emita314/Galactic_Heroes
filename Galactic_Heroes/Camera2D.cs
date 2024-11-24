using Microsoft.Xna.Framework;

namespace Galactic_Heroes
{
    public class Camera2D
    {
        private Vector2 _position;
        private float _zoom;
        private float _rotation;
        private Vector2 _viewportSize;
        private Vector2 _worldSize;

        public Camera2D(Vector2 viewportSize, Vector2 worldSize)
        {
            _position = Vector2.Zero;
            _zoom = 1.0f;
            _rotation = 0f;
            _viewportSize = viewportSize;
            _worldSize = worldSize;
        }

        public Matrix GetTransform()
        {
            return Matrix.CreateTranslation(-_position.X, -_position.Y, 0) * // Trasladar al origen
                   Matrix.CreateRotationZ(_rotation) *                     // Rotar
                   Matrix.CreateScale(_zoom) *                             // Escalar
                   Matrix.CreateTranslation(_viewportSize.X / 2, _viewportSize.Y / 2, 0); // Reposicionar al centro
        }

        public void Follow(Vector2 targetPosition)
        {
            // Centrar cámara en el objetivo
            _position = targetPosition - _viewportSize / 2;

            // Restringir la posición de la cámara a los límites del mundo
            _position.X = MathHelper.Clamp(_position.X, 0, _worldSize.X - _viewportSize.X);
            _position.Y = MathHelper.Clamp(_position.Y, 0, _worldSize.Y - _viewportSize.Y);
        }

        public void SetZoom(float zoom)
        {
            _zoom = MathHelper.Clamp(zoom, 0.5f, 2.0f); // Limitar el nivel de zoom
        }

        public void SetRotation(float rotation)
        {
            _rotation = rotation;
        }
    }
}
