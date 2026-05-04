using System;

namespace Aether.Simulation
{
    public class SpatialGrid
    {
        readonly int width;
        readonly int height;
        public readonly float CellSize;

        public int GridWidth { get; }
        public int GridHeight { get; }
        public int CellCount { get; }


        public int[] cellStarts;
        public int[] cellEnds;
        public int[] cellIndices;

        int[] cellCounts;

        public SpatialGrid(int width, int height, float cellSize, int maxCount)
        {
            this.width = width;
            this.height = height;
            CellSize = cellSize;

            GridWidth = (int)((width + cellSize - 1) / cellSize);
            GridHeight = (int)((height + cellSize - 1) / cellSize);
            CellCount = GridWidth * GridHeight;

            cellStarts = new int[CellCount];
            cellEnds = new int[CellCount];
            cellIndices = new int[maxCount];
            cellCounts = new int[CellCount];
        }

        public void Build(Particle[] particles, int particleCount)
        {
            Array.Clear(cellCounts, 0, CellCount);

            for(int i = 0; i < particleCount; i++)
            {
                ref Particle p = ref particles[i];
                int cx = ClampCellX((int)(p.position.X / CellSize));
                int cy = ClampCellY((int)(p.position.Y / CellSize));
                int bucket = cy * GridWidth + cx;
                cellCounts[bucket]++;
            }

            int total = 0;
            for(int b = 0; b < CellCount; b++)
            {
                cellStarts[b] = total;
                total += cellCounts[b];
                cellEnds[b] = total;
            }

            Array.Copy(cellStarts, cellCounts, CellCount);

            for(int i = 0; i < particleCount; i++)
            {
                ref Particle p = ref particles[i];
                int cx = ClampCellX((int)(p.position.X / CellSize));
                int cy = ClampCellY((int)(p.position.Y / CellSize));
                int bucket = cy * GridWidth + cx;
                int slot = cellCounts[bucket]++;
                cellIndices[slot] = i;
            }
        }

        int ClampCellX(int cx)
        {
            if (cx < 0)
                return 0;

            if (cx >= GridWidth)
                return GridWidth - 1;

            return cx;
        }

        int ClampCellY(int cy)
        {
            if (cy < 0)
                return 0;

            if (cy >= GridHeight)
                return GridHeight - 1;

            return cy;
        }

    }
}
