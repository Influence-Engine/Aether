using SDL3;

namespace Aether.Simulation
{
    public class Life
    {
        public Particle[] particles;
        public ForceMatrix forceMatrix;

        float interactionRadius;
        float forceMultiplier;
        float damping;
        float maxSpeed;

        int width;
        int height;

        int particleCount;
        public int ParticleCount => particleCount;
        int typeCount;
        public int TypeCount => typeCount;

        public Life(int width, int height, int particleCount, int typeCount)
        {
            this.width = width;
            this.height = height;
            this.particleCount = particleCount;
            this.typeCount = typeCount;

            interactionRadius = 32f;
            forceMultiplier = 0.8f;
            damping = 0.95f;
            maxSpeed = 128f;

            forceMatrix = new ForceMatrix(typeCount);
            forceMatrix.SetPattern(ForcePattern.AllRepel);

            particles = new Particle[particleCount];
            InitializeRandom();
        }

        public void InitializeRandom()
        {
            for (int i = 0; i < particleCount; i++)
            {
                float x = Random.Shared.NextSingle() * width;
                float y = Random.Shared.NextSingle() * height;
                int type = Random.Shared.Next(typeCount);

                particles[i] = new Particle(new SDL.FPoint(x, y), type);
            }
        }

        public void Tick()
        {
            float deltaTime = Time.fixedDeltaTime;

            for (int i = 0; i < particleCount; i++)
            {
                SDL.FPoint force = SDL.FPoint.Zero;

                for (int j = 0; j < particleCount; j++)
                {
                    if (i == j)
                        continue;

                    SDL.FPoint delta = particles[j].position - particles[i].position;
                    float distanceSq = delta.LengthSquared;

                    if (distanceSq < interactionRadius * interactionRadius && distanceSq > 0.1f)
                    {
                        float distance = MathF.Sqrt(distanceSq);
                        SDL.FPoint direction = new SDL.FPoint(delta.x / distance, delta.y / distance);

                        float f = forceMatrix.GetForce(particles[i].type, particles[j].type);
                        float strength = f * (1 - distance / interactionRadius);

                        float collisionDistance = 4f;
                        if(distance <collisionDistance)
                        {
                            float t = 1f - (distance / collisionDistance);
                            float collisionStrength = 50f * t * t;

                            force -= direction * collisionStrength;
                            strength *= 0.334f;
                        }

                        force += direction * strength;
                    }
                }

                particles[i].velocity += force * forceMultiplier * deltaTime * 100f;
            }

            for (int i = 0; i < particleCount; i++)
            {
                particles[i].Update(deltaTime, damping, maxSpeed, width, height);
            }
        }

        public void Tick(int tickCount)
        {
            for(int i = 0; i < tickCount; i++)
            {
                Tick();
            }
        }
    }
}
