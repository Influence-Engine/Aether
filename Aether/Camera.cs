using SDL3;
using System.Runtime.CompilerServices;

namespace Aether
{
    public class Camera
    {
        public SDL.FPoint position;
        public float zoom = 1f;

        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }

        public float WorldWidth => ScreenWidth / zoom;
        public float WorldHeight => ScreenHeight / zoom;

        public SDL.FPoint minBounds;
        public SDL.FPoint maxBounds;
        public bool useBounds = false;

        public Camera(int screenWidth, int screenHeight)
        {
            this.ScreenWidth = screenWidth;
            this.ScreenHeight = screenHeight;
            position = new SDL.FPoint(screenWidth * 0.5f, screenHeight * 0.5f);
        }

        public void Resize(int width, int height)
        {
            this.ScreenWidth = width;
            this.ScreenHeight = height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SDL.FPoint WorldToScreen(SDL.FPoint worldPos)
        {
            return new SDL.FPoint(
                (worldPos.x - position.x) * zoom + (ScreenWidth * 0.5f),
                (worldPos.y - position.y) * zoom + (ScreenHeight * 0.5f));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SDL.FPoint ScreenToWorld(SDL.FPoint screenPos)
        {
            return new SDL.FPoint(
                (screenPos.x - ScreenWidth * 0.5f) / zoom + position.x,
                (screenPos.y - ScreenWidth * 0.5f) / zoom + position.y);
        }

        public void Move(SDL.FPoint delta)
        {
            SDL.FPoint newPos = position + delta;

            if(useBounds)
            {
                newPos.x = Math.Clamp(newPos.x, minBounds.x + WorldWidth * 0.5f, maxBounds.x - WorldWidth * 0.5f);
                newPos.y = Math.Clamp(newPos.y, minBounds.y + WorldHeight * 0.5f, maxBounds.y - WorldHeight * 0.5f);
            }

            position = newPos;
        }

        public void SetPosition(SDL.FPoint worldPos)
        {
            if (useBounds)
            {
                worldPos.x = Math.Clamp(worldPos.x, minBounds.x + WorldWidth * 0.5f, maxBounds.x - WorldWidth * 0.5f);
                worldPos.y = Math.Clamp(worldPos.y, minBounds.y + WorldHeight * 0.5f, maxBounds.y - WorldHeight * 0.5f);
            }

            position = worldPos;
        }

        public (SDL.FPoint min, SDL.FPoint max) GetVisibleBounds()
        {
            SDL.FPoint center = position;
            float halfWidth = WorldWidth * 0.5f;
            float halfHeight = WorldHeight * 0.5f;

            return (
                new SDL.FPoint(center.x - halfWidth, center.y - halfHeight),
                new SDL.FPoint(center.x + halfWidth, center.y + halfHeight));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsVisible(SDL.FPoint worldPos, float radius)
        {
            (SDL.FPoint min, SDL.FPoint max) = GetVisibleBounds();

            min.x -= radius;
            min.y -= radius;
            max.x += radius;
            max.y += radius;

            return worldPos.x >= min.x && worldPos.x <= max.x &&
                worldPos.y >= min.y && worldPos.y <= max.y;
        }
    }
}
