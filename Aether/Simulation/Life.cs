using SDL3;

namespace Aether.Simulation
{
    public class Life
    {
        public Particle[] particles;
        public ForceMatrix forceMatrix;

        // Spatial Grid time
        SpatialGrid grid;

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

            grid = new SpatialGrid(width, height, interactionRadius, particleCount);

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

        void BuildGrid()
        {
            grid.Clear();

            for(int i = 0; i < particleCount; i++)
            {
                ref Particle particle = ref particles[i];
                grid.Insert(i, particle.position.x, particle.position.y);
            }
        }

        public void Tick()
        {
            float deltaTime = Time.fixedDeltaTime;
            float interactionSquared = interactionRadius * interactionRadius;
            float forcePower = forceMultiplier * deltaTime * 100f;

            BuildGrid();

            Parallel.For(0, ParticleCount, i =>
            {
                ref Particle particle = ref particles[i];

                float particleX = particle.position.x;
                float particleY = particle.position.y;

                float forceX = 0f;
                float forceY = 0f;

                int minX = (int)((particleX - interactionRadius) / grid.CellSize);
                int maxX = (int)((particleX + interactionRadius) / grid.CellSize);
                int minY = (int)((particleY - interactionRadius) / grid.CellSize);
                int maxY = (int)((particleY + interactionRadius) / grid.CellSize);

                if (minX < 0) minX = 0;
                if (maxX >= grid.GridWidth) maxX = grid.GridWidth - 1;
                if (minY < 0) minY = 0;
                if (maxY >= grid.GridHeight) maxY = grid.GridHeight - 1;

                for (int cx = minX; cx <= maxX; cx++)
                {
                    for (int cy = minY; cy <= maxY; cy++)
                    {
                        int cellIndex = cy * grid.GridWidth + cx;
                        int j = grid.GridHeads[cellIndex];

                        while (j != -1)
                        {
                            if (j != i) // skip self
                            {
                                ref Particle other = ref particles[j];

                                float deltaX = other.position.x - particleX;
                                float deltaY = other.position.y - particleY;

                                float distanceSquared = deltaX * deltaX + deltaY * deltaY;
                                if (distanceSquared <= 0.1f || distanceSquared >= interactionSquared)
                                {
                                    j = grid.Next[j];
                                    continue;
                                }

                                float distance = MathF.Sqrt(distanceSquared);
                                float invertDistance = 1f / distance;

                                float directionX = deltaX * invertDistance;
                                float directionY = deltaY * invertDistance;

                                float f = forceMatrix.GetForce(particle.type, other.type);
                                float strength = f * (1 - distance / interactionRadius);

                                float collisionDistance = 6f;
                                if (distance < collisionDistance)
                                {
                                    float t = 1f - (distance / collisionDistance);
                                    float collisionStrength = 50f * t * t;

                                    forceX -= directionX * collisionStrength;
                                    forceY -= directionY * collisionStrength;

                                    strength *= 0.334f;
                                }

                                forceX += directionX * strength;
                                forceY += directionY * strength;
                            }

                            j = grid.Next[j];
                        }
                    }
                }

                particle.velocity.x += forceX * forcePower;
                particle.velocity.y += forceY * forcePower;
            });

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
