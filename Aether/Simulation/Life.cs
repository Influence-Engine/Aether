using SDL3;

namespace Aether.Simulation
{
    public class Life
    {
        Particle[] particles;
        public Particle[] Particles => particles;
        ForceMatrix forceMatrix;

        float interactionRadius;
        float forceMultiplier;
        float damping;
        float maxSpeed;

        float minSeperation;
        float selfRepelStrength;
        float maxSelfRepel;
        float collisionSoftness;

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
            forceMultiplier = 1.1f;
            damping = 0.95f;
            maxSpeed = 64f;

            minSeperation = 8f;
            selfRepelStrength = 10f;
            maxSelfRepel = 100f;
            collisionSoftness = 0.5f;

            forceMatrix = new ForceMatrix(typeCount);
            forceMatrix.SetPattern(ForcePattern.Default);

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

        public void Update(float deltaTime)
        {
            for (int i = 0; i < particleCount; i++)
            {
                SDL.FPoint force = SDL.FPoint.Zero;
                SDL.FPoint selfRepelForce = SDL.FPoint.Zero;

                for (int j = 0; j < particleCount; j++)
                {
                    if (i == j)
                        continue;

                    SDL.FPoint delta = particles[j].position - particles[i].position;
                    float distance = MathF.Sqrt(delta.LengthSquared);

                    if(distance < interactionRadius && distance > 0.1f)
                    {
                        SDL.FPoint direction = new SDL.FPoint(delta.x / distance, delta.y / distance);
                        float f = forceMatrix.GetForce(particles[i].type, particles[j].type);
                        float strength = f * (1 - distance / interactionRadius);

                        force += direction * strength;

                        if(distance < minSeperation)
                        {
                            float repelStrength = selfRepelStrength * (1 - distance / minSeperation);
                            selfRepelForce += direction * repelStrength;
                        }
                    }
                }

                if(selfRepelForce != SDL.FPoint.Zero)
                {
                    float repelMagnitude = MathF.Sqrt(selfRepelForce.LengthSquared);
                    if (repelMagnitude > maxSelfRepel)
                        selfRepelForce = new SDL.FPoint(selfRepelForce.x / repelMagnitude, selfRepelForce.y / repelMagnitude) * maxSelfRepel;

                    particles[i].velocity += selfRepelForce * deltaTime;
                }

                particles[i].velocity += force * forceMultiplier * deltaTime ;
            }

            for (int i = 0; i < particleCount; i++)
            {
                particles[i].Update(deltaTime, damping, maxSpeed, width, height);
            }
        }
    }
}
