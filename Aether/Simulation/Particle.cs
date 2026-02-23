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

            float edgeForce = 100f;
            float edgeDistance = 50f;

            if(position.x < edgeDistance) // Left Edge
            {
                float t = 1f - (position.x / edgeDistance);
                velocity.x += t * edgeForce * deltaTime;
            }
            else if(position.x > width - edgeDistance) // Right Edge
            {
                float t = (position.x - (width - edgeDistance)) / edgeDistance;
                velocity.x -= t * edgeForce * deltaTime;
            }

            if(position.y < edgeDistance)
            {
                float t = 1f - (position.y / edgeDistance);
                velocity.y += t* edgeForce * deltaTime;
            }
            else if(position.y > height - edgeDistance)
            {
                float t = (position.y - (height - edgeDistance)) / edgeDistance;
                velocity.y -= t * edgeForce * deltaTime;
            }
        }
    }
}
