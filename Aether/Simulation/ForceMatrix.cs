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
            forces = pattern switch
            {
                ForcePattern.Default => DefaultPattern,
                ForcePattern.Flocking => FlockingPattern,
                ForcePattern.PredatorPrey => PredatorPreyPattern,
                ForcePattern.Symbiosis => SymbiosisPattern,
                ForcePattern.Neutral => NeutralPattern,
                ForcePattern.AllAttract => AllAttractPattern,
                ForcePattern.AllRepel => AllRepelPattern,
                ForcePattern.Atomic => AtomicPattern,
                _ => NeutralPattern
            };
        }

        public void Randomize(float strength = 1f)
        {
            for (int i = 0; i < typeCount; i++)
            {
                for (int j = 0; j < typeCount; j++)
                {
                    forces[i, j] = (Random.Shared.NextSingle() - 0.5f) * 2.0f * strength;
                }
            }
        }

        public float GetForce(int typeA, int typeB) => forces[typeA, typeB];

        public float Normalized(int i, int j) => (forces[i, j] + 1f) * 0.5f;

        public float[,] ToArray() => forces;

        #region Patterns

        public float[,] DefaultPattern
        {
            get
            {
                float[,] matrix = new float[typeCount, typeCount];
                for(int i = 0; i < typeCount; i++)
                {
                    for(int j = 0; j < typeCount;j++)
                    {
                        if (i == j)
                        {
                            matrix[i, j] = 0.3f;
                        }
                        else
                        {
                            int diff = Math.Abs(i - j);
                            matrix[i, j] = (diff % 2 == 0) ? 0.666f : -0.334f;
                        }
                    }
                }

                return matrix;
            }
        }

        public float[,] FlockingPattern
        {
            get
            {
                float[,] matrix = new float[typeCount, typeCount];
                for (int i = 0; i < typeCount; i++)
                {
                    for (int j = 0; j < typeCount; j++)
                    {
                        if (i == j)
                        {
                            matrix[i, j] = 1f;
                        }
                        else
                        {
                            matrix[i, j] = -0.5f;
                        }
                    }
                }

                return matrix;
            }
        }

        public float[,] PredatorPreyPattern
        {
            get
            {
                float[,] matrix = new float[typeCount, typeCount];
                for (int i = 0; i < typeCount; i++)
                {
                    for (int j = 0; j < typeCount; j++)
                    {
                        if (i == j)
                        {
                            matrix[i, j] = 0.1f;
                        }
                        else
                        {
                            int next = (i + 1) % typeCount;
                            int prev = (i - 1 + typeCount) % typeCount;

                            if (j == next) // Attract Prey
                                matrix[i, j] = 0.8f;
                            else if (j == prev) // Repel Predator
                                matrix[i, j] = -0.8f;
                            else
                                matrix[i, j] = 0f;
                        }
                    }
                }

                return matrix;
            }
        }

        public float[,] SymbiosisPattern
        {
            get
            {
                float[,] matrix = new float[typeCount, typeCount];
                for (int i = 0; i < typeCount; i++)
                {
                    for (int j = 0; j < typeCount; j++)
                    {
                        if (i == j) // Mild self attraction
                        {
                            matrix[i, j] = 0.25f;
                        }
                        else
                        {
                            // Pair even with odd indices
                            if (i % 2 == 0 && j == i + 1) 
                                matrix[i, j] = 0.666f;
                            else if (i % 2 == 1 && j == i - 1)
                                matrix[i, j] = 0.666f;
                            else // Repel Others
                                matrix[i, j] = -0.334f;
                        }
                    }
                }

                return matrix;
            }
        }

        public float[,] GetFilledMatrix(float value)
        {
            float[,] matrix = new float[typeCount, typeCount];
            for (int i = 0; i < typeCount; i++)
            {
                for (int j = 0; j < typeCount; j++)
                {
                    matrix[i, j] = value;
                }
            }

            return matrix;
        }

        public float[,] NeutralPattern => GetFilledMatrix(0f);

        public float[,] AllAttractPattern => GetFilledMatrix(0.5f);

        public float[,] AllRepelPattern => GetFilledMatrix(-0.5f);

        public float[,] AtomicPattern
        {
            get
            {
                float[,] matrix = new float[typeCount, typeCount];
                for (int i = 0; i < typeCount; i++)
                {
                    for (int j = 0; j < typeCount; j++)
                    {
                        if (i == j)
                        {
                            if(i == 0) // Proton
                                matrix[i, j] = 0.5f;
                            else if(i == 1) // Neutron
                                matrix[i, j] = 0.3f;
                            else if (i == 2) // Electron
                                matrix[i, j] = -0.8f;
                            else
                                matrix[i, j] = 0.1f;
                        }
                        else
                        {
                            if (i == 0 && j == 2)
                                matrix[i, j] = -1f;
                            else if (i == 2 && j == 0)
                                matrix[i, j] = -1f;

                            else if (i == 0 && j == 1)
                                matrix[i, j] = 0.7f;
                            else if (i == 1 && i == 0)
                                matrix[i, j] = 0.7f;

                            else if (i == 2 && j == 1)
                                matrix[i, j] = 0.2f;
                            else if (i == 1 && j == 2)
                                matrix[i, j] = 0.2f;

                            else
                                matrix[i, j] = (i + j) % 2 == 0 ? 0.3f : -0.3f;
                        }
                    }
                }

                return matrix;
            }
        }

        #endregion
    }
}
