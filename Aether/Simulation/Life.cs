using Essence;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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

                particles[i] = new Particle(new Vector2(x, y), type);
            }
        }

        public void Tick()
        {
            float deltaTime = Time.fixedDeltaTime;
            float interactionSquared = interactionRadius * interactionRadius;
            float forcePower = forceMultiplier * deltaTime * 100f;

            grid.Build(particles, particleCount);

            Parallel.For(0, ParticleCount, i =>
            {
                ref Particle particle = ref particles[i];

                float particleX = particle.position.X;
                float particleY = particle.position.Y;

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

                for (int cy = minY; cy <= maxY; cy++)
                {
                    int rowStart = cy * grid.GridWidth;
                    for (int cx = minX; cx <= maxX; cx++)
                    {
                        int bucket = rowStart + cx;
                        int start = grid.cellStarts[bucket];
                        int end = grid.cellEnds[bucket];

                        for(int k = start; k < end;  k++)
                        {
                            int j = grid.cellIndices[k];
                            if (j == i) // Skip self
                                continue;

                            ref Particle other = ref particles[j];

                            float deltaX = other.position.X - particleX;
                            float deltaY = other.position.Y - particleY;

                            float distSq = deltaX * deltaX + deltaY * deltaY;
                            if (distSq <= 0.1f || distSq >= interactionSquared)
                                continue;

                            float invertDistance = FastInvSqrt(distSq);
                            float distance = 1f / invertDistance;

                            float directionX = deltaX * invertDistance;
                            float directionY = deltaY * invertDistance;

                            float f = forceMatrix.GetForce(particle.type, other.type);
                            float strength = f * (1 - distance / interactionRadius);

                            const float collisionDistance = 6f;
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
                    }
                }

                particle.velocity.X += forceX * forcePower;
                particle.velocity.Y += forceY * forcePower;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float FastInvSqrt(float x)
        {
            float xhalf = 0.5f * x;
            int i = 0x5f3759df - (BitConverter.SingleToInt32Bits(x) >> 1);
            float y = BitConverter.Int32BitsToSingle(i);
            return y * (1.5f - xhalf * y * y);
        }
    }
}
