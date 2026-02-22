using Aether.Rendering;
using Aether.Simulation;
using SDL3;

namespace Aether
{
    internal static class Program
    {
        const int width = 512;
        const int height = 512;

        const int particleCount = 7000;
        const int typeCount = 32;

        public static void Main(string[] args)
        {
            if(!SDL.Init(SDL.InitFlags.Video))
            {
                Console.WriteLine("Init Video Failed...");
                return;
            }

            nint window = SDL.CreateWindow("Aether Window", width, height, 0);
            Console.WriteLine($"Window Created: {window}");

            nint renderer = SDL.CreateRenderer(window, null);
            Console.WriteLine($"Renderer Created: {renderer}");

            Life life = new Life(width, height, particleCount, typeCount);

            bool running = true;
            bool isPaused = false;
            bool stepOneTick = false;

            float deltaTime = 0f;
            ulong lastTime = SDL.GetTicks();

            float accumulator = 0f;

            while (running)
            {
                ulong currentTime = SDL.GetTicks();
                deltaTime = (currentTime - lastTime) / 1000f;
                lastTime = currentTime;

                Time.Update(deltaTime);
                Input.Update();

                while (SDL.PollEvent(out SDL.Event e))
                {
                    Input.ProcessEvent(e);

                    if (e.type == SDL.EventType.Quit)
                    {
                        Console.WriteLine("Quit Polled");
                        running = false;
                    }
                }

                if (Input.GetKeyDown(SDL.KeyCode.Space))
                {
                    isPaused = !isPaused;
                    Time.timeScale = isPaused ? 0f : 1f;
                    Console.WriteLine(isPaused ? "Paused" : "Playing");
                }

                if (Input.GetKeyDown(SDL.KeyCode.D) && isPaused)
                    stepOneTick = true;

                if (Input.GetKeyDown(SDL.KeyCode.R))
                {
                    Time.Reset();
                    life = new Life(width, height, particleCount, typeCount);
                    Console.WriteLine("Simulation reset");
                }

                if (Input.GetKeyDown(SDL.KeyCode.Q))
                {
                    ForceMatrix newMatrix = new ForceMatrix(typeCount);
                    newMatrix.Randomize();
                    life.forceMatrix = newMatrix;
                }

                if (Input.GetKeyDown(SDL.KeyCode.W))
                {
                    life.forceMatrix.SetPattern(ForcePattern.Atomic);
                }

                if (Input.GetKeyDown(SDL.KeyCode.Escape))
                    running = false; // Quick escape

                if(Input.mouseScrollDelta.y != 0)
                {
                    Time.timeScale += Input.mouseScrollDelta.y * 0.1f;
                    Console.WriteLine($"Time Scale: {Time.timeScale:F2}x");
                }

                // Fixed timestep simulation
                if (!isPaused || stepOneTick)
                {
                    accumulator += Time.unscaledDeltaTime;

                    while (accumulator >= Time.fixedDeltaTime)
                    {
                        life.Tick();
                        Time.Tick();
                        accumulator -= Time.fixedDeltaTime;

                        if (stepOneTick)
                        {
                            stepOneTick = false;
                            break;
                        }
                    }
                }

                SDL.SetRenderDrawColor(renderer, 20, 20, 20, 255);
                SDL.RenderClear(renderer);

                // Draw Particles
                Renderer.DrawParticlesRect(renderer, life);

                SDL.RenderPresent(renderer);

                //Console.WriteLine($"TimeScale: {timeScale}");
                //Console.WriteLine($"FPS: {1f / deltaTime}");
            }

            SDL.DestroyRenderer(renderer);
            SDL.DestroyWindow(window);
            SDL.Quit();

            Console.WriteLine("Quit Engine");
        }
    }
}
