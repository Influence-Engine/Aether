using Essence;
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
                (worldPos.x - position.x) * zoom + (ScreenWidth * 0.5f),
                (worldPos.y - position.y) * zoom + (ScreenHeight * 0.5f));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 ScreenToWorld(Vector2 screenPos)
        {
            return new Vector2(
                (screenPos.x - ScreenWidth * 0.5f) / zoom + position.x,
                (screenPos.y - ScreenWidth * 0.5f) / zoom + position.y);
        }

        public void Move(Vector2 delta)
        {
             Vector2 newPos = position + delta;

            if(useBounds)
            {
                newPos.x = Math.Clamp(newPos.x, minBounds.x + WorldWidth * 0.5f, maxBounds.x - WorldWidth * 0.5f);
                newPos.y = Math.Clamp(newPos.y, minBounds.y + WorldHeight * 0.5f, maxBounds.y - WorldHeight * 0.5f);
            }

            position = newPos;
        }

        public void SetPosition(Vector2 worldPos)
        {
            if (useBounds)
            {
                worldPos.x = Math.Clamp(worldPos.x, minBounds.x + WorldWidth * 0.5f, maxBounds.x - WorldWidth * 0.5f);
                worldPos.y = Math.Clamp(worldPos.y, minBounds.y + WorldHeight * 0.5f, maxBounds.y - WorldHeight * 0.5f);
            }

            position = worldPos;
        }

        public (Vector2 min, Vector2 max) GetVisibleBounds()
        {
            Vector2 center = position;
            float halfWidth = WorldWidth * 0.5f;
            float halfHeight = WorldHeight * 0.5f;

            return (
                new Vector2(center.x - halfWidth, center.y - halfHeight),
                new Vector2(center.x + halfWidth, center.y + halfHeight));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsVisible(Vector2 worldPos, float radius)
        {
            (Vector2 min, Vector2 max) = GetVisibleBounds();

            min.x -= radius;
            min.y -= radius;
            max.x += radius;
            max.y += radius;

            return worldPos.x >= min.x && worldPos.x <= max.x &&
                worldPos.y >= min.y && worldPos.y <= max.y;
        }
    }
}
