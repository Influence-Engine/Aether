using SDL3;
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

        public Particle(SDL.FPoint position, int type, float radius = 1f)
        {
            this.position = new Vector2(position.x, position.y);
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

            float edgeForce = 100f;
            float edgeDistance = 50f;

            if(position.X < edgeDistance) // Left Edge
            {
                float t = 1f - (position.X / edgeDistance);
                velocity.X += t * edgeForce * deltaTime;
            }
            else if(position.X > width - edgeDistance) // Right Edge
            {
                float t = (position.X / (width - edgeDistance)) / edgeDistance;
                velocity.X -= t * edgeForce * deltaTime;
            }

            if(position.Y < edgeDistance)
            {
                float t = 1f - (position.Y / edgeDistance);
                velocity.Y += t* edgeForce * deltaTime;
            }
            else if(position.Y > height - edgeDistance)
            {
                float t = (position.Y - (height -edgeDistance)) / edgeDistance;
                velocity.Y -= t * edgeForce * deltaTime;
            }
        }

        public SDL.FPoint ToFPoint() => new SDL.FPoint(position.X, position.Y);
    }
}
