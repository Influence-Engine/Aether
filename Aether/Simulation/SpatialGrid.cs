namespace Aether.Simulation
{
    public class SpatialGrid
    {
        readonly int width;
        readonly int height;

        readonly int gridWidth;
        public int GridWidth => gridWidth;
        readonly int gridHeight;
        public int GridHeight => gridHeight;

        readonly float cellSize;
        public float CellSize => cellSize;
        readonly int cellCount;
        public int CellCount => cellCount;

        readonly int[] gridHeads;
        public int[] GridHeads => gridHeads;
        readonly int[] next;
        public int[] Next => next;

        public SpatialGrid(int width, int height, float cellSize, int maxCount)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;

            gridWidth = (int)((width + cellSize - 1) / cellSize);
            gridHeight = (int)((height + cellSize - 1) / cellSize);
            cellCount = gridWidth * gridHeight;

            gridHeads = new int[cellCount];
            next = new int[maxCount];

            Clear();
        }

        public void Insert(int index, float x, float y)
        {
            int cellX = (int)(x / cellSize);
            int cellY = (int)(y / cellSize);

            if (cellX < 0) cellX = 0;
            else if (cellX >= gridWidth) cellX = gridWidth - 1;

            if (cellY < 0) cellY = 0;
            else if (cellY >= gridHeight) cellY = gridHeight - 1;

            int cellIndex = cellY * gridWidth + cellX;

            next[index] = gridHeads[cellIndex];
            gridHeads[cellIndex] = index;
        }

        // callback causes GC triggers, so we do it manually instead <.<
        public void ForEachNeighbour(float x, float y, float radius, Action<int> callback)
        {
            int minX = (int)((x - radius) / cellSize);
            int maxX = (int)((x + radius) / cellSize);

            int minY = (int)((y - radius) / cellSize);
            int maxY = (int)((y + radius) / cellSize);

            if (minX < 0) minX = 0;
            if (maxX >= gridWidth) maxX = gridWidth - 1;

            if (minY < 0) minY = 0;
            if (maxY >= gridHeight) maxY = gridHeight - 1;

            for (int cx = minX; cx <= maxX; cx++)
            {
                for (int cy = minY; cy <= maxY; cy++)
                {
                    int cellIndex = cy * gridWidth + cx;
                    int j = gridHeads[cellIndex];

                    while(j !=  -1)
                    {
                        callback(j);
                        j = next[j];
                    }
                }
            }
        }

        public void Clear()
        {
            for(int i = 0; i < cellCount; i++)
                gridHeads[i] = -1;
        }


    }
}
