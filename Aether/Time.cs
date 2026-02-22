namespace Aether
{
    public static class Time
    {
        public static float time { get; private set; }
        public static float deltaTime { get; private set; }

        public static float unscaledTime { get; private set; }
        public static float unscaledDeltaTime { get; private set; }

        public static float timeScale = 1f;

        public static float fixedDeltaTime { get; private set; } = 1f / 60f;
        public static ulong ticks { get; private set; }

        public static void Update(float deltaTime)
        {
            Time.unscaledDeltaTime = deltaTime;
            Time.unscaledTime += deltaTime;

            Time.deltaTime = unscaledDeltaTime * timeScale;
            Time.time += deltaTime;
        }

        public static void Tick() => Time.ticks++;

        public static void SetTrickRate(float tickRate) => fixedDeltaTime = 1f / Math.Max(1f, tickRate);

        public static void Reset()
        {
            time = 0;
            unscaledTime = 0;
            ticks = 0;

            timeScale = 1f;
        }
    }
}
