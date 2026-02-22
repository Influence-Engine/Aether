using SDL3;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Aether
{
    public class Camera
    {
        public Vector2 position;
        public float zoom = 1f;

        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }

        public float WorldWidth => ScreenWidth / zoom;
        public float WorldHeight => ScreenHeight / zoom;

        public Vector2 minBounds;
        public Vector2 maxBounds;
        public bool useBounds = false;

        public Camera(int screenWidth, int screenHeight)
        {
            this.ScreenWidth = screenWidth;
            this.ScreenHeight = screenHeight;
            position = new Vector2(screenWidth * 0.5f, screenHeight * 0.5f);
        }

        public void Resize(int width, int height)
        {
            this.ScreenWidth = width;
            this.ScreenHeight = height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 WorldToScreen(Vector2 worldPos)
        {
            return new Vector2(
                (worldPos.X - position.X) * zoom + (ScreenWidth * 0.5f),
                (worldPos.Y - position.Y) * zoom + (ScreenHeight * 0.5f));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 ScreenToWorld(Vector2 screenPos)
        {
            return new Vector2(
                (screenPos.X - ScreenWidth * 0.5f) / zoom + position.X,
                (screenPos.Y - ScreenWidth * 0.5f) / zoom + position.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 ScreenToWorld(SDL.FPoint screenPos) => ScreenToWorld(new Vector2(screenPos.x, screenPos.y));

        public void Move(Vector2 delta)
        {
            Vector2 newPos = position + delta;

            if(useBounds)
            {
                newPos.X = Math.Clamp(newPos.X, minBounds.X + WorldWidth * 0.5f, maxBounds.X - WorldWidth * 0.5f);
                newPos.Y = Math.Clamp(newPos.Y, minBounds.Y + WorldHeight * 0.5f, maxBounds.Y - WorldHeight * 0.5f);
            }

            position = newPos;
        }

        public void SetPosition(Vector2 worldPos)
        {
            if (useBounds)
            {
                worldPos.X = Math.Clamp(worldPos.X, minBounds.X + WorldWidth * 0.5f, maxBounds.X - WorldWidth * 0.5f);
                worldPos.Y = Math.Clamp(worldPos.Y, minBounds.Y + WorldHeight * 0.5f, maxBounds.Y - WorldHeight * 0.5f);
            }

            position = worldPos;
        }

        public (Vector2 min, Vector2 max) GetVisibleBounds()
        {
            Vector2 center = position;
            float halfWidth = WorldWidth * 0.5f;
            float halfHeight = WorldHeight * 0.5f;

            return (
                new Vector2(center.X - halfWidth, center.Y - halfHeight),
                new Vector2(center.X + halfWidth, center.Y + halfHeight));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsVisible(Vector2 worldPos, float radius)
        {
            (Vector2 min, Vector2 max) = GetVisibleBounds();

            min.X -= radius;
            min.Y -= radius;
            max.X += radius;
            max.Y += radius;

            return worldPos.X >= min.X && worldPos.X <= max.X &&
                worldPos.Y >= min.Y && worldPos.Y <= max.Y;
        }
    }
}
