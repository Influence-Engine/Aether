using SDL3;

namespace Aether.Simulation
{
    public struct Particle
    {
        public SDL.FPoint position;
        public SDL.FPoint velocity;

        public float radius;

        public int type;

        public Particle(SDL.FPoint position, int type, float radius = 1f)
        {
            this.position = position;
            this.velocity = SDL.FPoint.Zero;
            this.radius = radius;
            this.type = type;
        }

        public void Update(float deltaTime, float damping, float maxSpeed, int width, int height)
        {
            position += velocity * deltaTime;
            velocity *= damping;

            // Speed limit
            if (velocity.LengthSquared > maxSpeed * maxSpeed)
                velocity = SDL.FPoint.Normalize(velocity) * maxSpeed;

            // Wrap around screen
            if (position.x < 0) position.y = width;
            if (position.x > width) position.x = 0;
            if (position.y < 0) position.y = height;
            if (position.y > height) position.y = 0;

        }
    }
}
