namespace Aether.Simulation
{
    public class ForceMatrix
    {
        float[,] forces;
        public int typeCount { get; private set; }

        public ForceMatrix(int typeCount)
        {
            this.typeCount = typeCount;
            this.forces = new float[typeCount, typeCount];
        }

        public float this[int i, int j]
        {
            get => forces[i, j];
            set => forces[i, j] = value;
        }

        public void SetPattern(ForcePattern pattern)
        {
            switch(pattern)
            {
                case ForcePattern.Default: // Classic *wink*
                    forces = new float[,]
                    {
                        { 0.5f, -0.2f, 0.8f },
                        { -0.2f, 0.3f, -0.5f },
                        { 0.8f, -0.5f, 0.2f }
                    };
                    break;
                case ForcePattern.Flocking:
                    forces = new float[,]
                    {
                        { 1.0f, -0.5f, -0.5f },
                        { -0.5f, 1.0f, -0.5f },
                        { 0.5f, -0.5f, 1.0f }
                    };
                    break;
            }
        }

        public void Randomize()
        {
            for (int i = 0; i < typeCount; i++)
            {
                for (int j = 0; j < typeCount; j++)
                {
                    forces[i, j] = (Random.Shared.NextSingle() - 0.5f) * 2.0f;
                }
            }
        }

        public float GetForce(int typeA, int typeB) => forces[typeA, typeB];
    }
}
