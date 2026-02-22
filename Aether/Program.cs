using Aether.Rendering;
using Aether.Simulation;
using SDL3;

namespace Aether
{
    internal static class Program
    {
        const int width = 1280;
        const int height = 720;

        const int particleCount = 5000;
        const int typeCount = 3;

        static float timeScale = 1f;

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

            ulong lastTime = SDL.GetTicks();

            while (running)
            {
                ulong currentTime = SDL.GetTicks();
                float deltaTime = (currentTime - lastTime) / 1000f;
                lastTime = currentTime;

                Input.Update();

                while(SDL.PollEvent(out SDL.Event e))
                {
                    Input.ProcessEvent(e);

                    if (e.type == SDL.EventType.Quit)
                    {
                        Console.WriteLine("Quit Polled");
                        running = false;
                    }
                }

                if (Input.GetKeyDown(SDL.KeyCode.Escape))
                    running = false; // Quick escape

                timeScale += Input.mouseScrollDelta.y * 0.2f;

                life.Update(deltaTime * timeScale);

                SDL.SetRenderDrawColor(renderer, 20, 20, 20, 255);
                SDL.RenderClear(renderer);

                Renderer.DrawParticlesRect(renderer, life);

                SDL.RenderPresent(renderer);

                Console.WriteLine($"TimeScale: {timeScale}");
                Console.WriteLine($"FPS: {1f / deltaTime}");
            }

            SDL.DestroyRenderer(renderer);
            SDL.DestroyWindow(window);
            SDL.Quit();

            Console.WriteLine("Quit Engine");
        }
    }
}
