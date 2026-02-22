using SDL3;
using System.Numerics;

namespace Aether
{
    internal static class Program
    {
        const int width = 1280;
        const int height = 720;

        static List<SDL.FPoint> clickPoints = new List<SDL.FPoint>();

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

            bool running = true;

            SDL.FPoint shapeCenter = new SDL.FPoint { x = width / 2, y = height / 2 };
            float shapeRadius= 32f;

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

                if (Input.GetMouseButtonDown(0))
                {
                    clickPoints.Add(new SDL.FPoint(Input.mousePosition.X, Input.mousePosition.Y));
                }

                SDL.SetRenderDrawColor(renderer, 20, 20, 20, 255);
                SDL.RenderClear(renderer);

                for (int i = 0; i < clickPoints.Count; ++i)
                {
                    SDL.FPoint point = clickPoints[i];
                    SDL.RenderCircle(renderer, point, shapeRadius, SDL.FColor.Green);
                }

                SDL.RenderPresent(renderer);

                //Console.WriteLine($"FPS: {1f / deltaTime}");
                SDL.Delay(16);
            }

            SDL.DestroyRenderer(renderer);
            SDL.DestroyWindow(window);
            SDL.Quit();

            Console.WriteLine("Quit Engine");
        }
    }
}
