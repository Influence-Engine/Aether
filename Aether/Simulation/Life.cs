using SDL3;

namespace Aether.Simulation
{
    public class Life
    {
        public Particle[] particles;
        public ForceMatrix forceMatrix;

        readonly ThreadLocal<List<int>> threadLocalNeighbours;

        // Spatial Grid time
        int gridWidth;
        int gridHeight;
        List<int>[,] grid;

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

            threadLocalNeighbours = new ThreadLocal<List<int>>(() => new List<int>(64));

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
            for (int x = 0; x < gridWidth; x++)
                for (int y = 0; y < gridHeight; y++)
                    grid[x, y].Clear();

            for(int i = 0; i < particleCount; i++)
            {
                int cellX = (int)(particles[i].position.x / interactionRadius);
                int cellY = (int)(particles[i].position.y / interactionRadius);

                cellX = Math.Clamp(cellX, 0, gridWidth - 1);
                cellY = Math.Clamp(cellY, 0, gridHeight - 1);

                grid[cellX, cellY].Add(i);
            }
        }

        void GetNeighbours(int index, List<int> result)
        {
            result.Clear();

            SDL.FPoint pos = particles[index].position;
            int cellX = (int)(pos.x / interactionRadius);
            int cellY = (int)(pos.y / interactionRadius);

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
            float interactionSquared = interactionRadius * interactionRadius;
            float forcePower = forceMultiplier * deltaTime * 100f;

            BuildGrid();

            Parallel.For(0, particleCount, i =>
            {
                ref Particle particle = ref particles[i];

                float particleX = particle.position.x;
                float particleY = particle.position.y;

                float forceX = 0f;
                float forceY = 0f;

                List<int> neighbours = threadLocalNeighbours.Value;
                GetNeighbours(i, neighbours);

                foreach (int j in neighbours)
                {
                    ref Particle other = ref particles[j];

                    float deltaX = other.position.x - particleX;
                    float deltaY = other.position.y - particleY;

                    SDL.FPoint delta = particles[j].position - particles[i].position;
                    float distanceSqaured = deltaX * deltaX + deltaY * deltaY;
                    if (distanceSqaured <= 0.1f || distanceSqaured >= interactionSquared)
                        continue;

                    float distance = MathF.Sqrt(distanceSqaured);
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
