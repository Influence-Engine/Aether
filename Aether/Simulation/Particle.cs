using System.Numerics;

namespace Aether.Simulation
{
    public struct Particle
    {
        public Vector2 position;
        public Vector2 velocity;

        public float radius;

        public int type;

        public Particle(Vector2 position, int type, float radius = 1f)
        {
            this.position = position;
            this.velocity = Vector2.Zero;
            this.radius = radius;
            this.type = type;
        }

        public void Update(float deltaTime, float damping, float maxSpeed, int width, int height)
        {
            position += velocity * deltaTime;
            velocity *= damping;

            // Speed limit
            if (velocity.LengthSquared() > maxSpeed * maxSpeed)
                velocity = Vector2.Normalize(velocity) * maxSpeed;

            // Wrap around screen
            if (position.X < 0) position.X = width;
            if (position.X > width) position.X = 0;
            if (position.Y < 0) position.Y = height;
            if (position.Y > height) position.Y = 0;

        }
    }
}
