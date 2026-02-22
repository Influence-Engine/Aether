using System.Numerics;

namespace Aether.Simulation
{
    public class Life
    {
        public Particle[] particles;
        public ForceMatrix forceMatrix;

        // Spatial Grid time
        int gridWidth;
        int gridHeight;
        List<int>[,] grid;
        List<int> neighbours = new List<int>(64);

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

            int cellSize = (int)interactionRadius;
            gridWidth = (width + cellSize - 1) / cellSize;
            gridHeight = (height + cellSize - 1) / cellSize;
            grid = new List<int>[gridWidth, gridHeight];

            for(int x = 0;  x < gridWidth; x++)
                for(int y = 0; y < gridHeight; y++)
                    grid[x, y] = new List<int>();

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

        void BuildGrid()
        {
            for (int x = 0; x < gridWidth; x++)
                for (int y = 0; y < gridHeight; y++)
                    grid[x, y].Clear();

            for(int i = 0; i < particleCount; i++)
            {
                int cellX = (int)(particles[i].position.X / interactionRadius);
                int cellY = (int)(particles[i].position.Y / interactionRadius);

                cellX = Math.Clamp(cellX, 0, gridWidth - 1);
                cellY = Math.Clamp(cellY, 0, gridHeight - 1);

                grid[cellX, cellY].Add(i);
            }
        }

        void GetNeighbours(int index, List<int> result)
        {
            result.Clear();

            Vector2 pos = particles[index].position;
            int cellX = (int)(pos.X / interactionRadius);
            int cellY = (int)(pos.Y / interactionRadius);

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int nx = cellX + dx;
                    int ny = cellY + dy;

                    if(nx >= 0 && nx < gridWidth && ny >= 0 && ny < gridHeight)
                    {
                        foreach(int other in grid[nx, ny])
                        {
                            if (other != index)
                                result.Add(other);
                        }
                    }
                }
            }
        }

        public void Tick()
        {
            float deltaTime = Time.fixedDeltaTime;

            BuildGrid();

            for (int i = 0; i < particleCount; i++)
            {
                Vector2 force = Vector2.Zero;

                GetNeighbours(i, neighbours);

                foreach(int j in neighbours)
                {
                    Vector2 delta = particles[j].position - particles[i].position;
                    float distanceSq = delta.LengthSquared();

                    if (distanceSq < interactionRadius * interactionRadius && distanceSq > 0.1f)
                    {
                        float distance = MathF.Sqrt(distanceSq);
                        Vector2 direction = delta / distance;

                        float f = forceMatrix.GetForce(particles[i].type, particles[j].type);
                        float strength = f * (1 - distance / interactionRadius);

                        float collisionDistance = 6f;
                        if(distance < collisionDistance)
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
