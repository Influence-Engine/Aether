using SDL3;
using System.Numerics;

namespace Aether
{
    public struct Particle
    {
        public Vector2 position;
        public Vector2 velocity;

        public float radius;

        public SDL.FColor color;

        public Particle(Vector2 position, float radius, SDL.FColor color)
        {
            this.position = position;
            this.velocity = Vector2.Zero;
            this.radius = radius;
            this.color = color;
        }
    }
}
