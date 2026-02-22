using SDL3;

namespace Aether
{
    internal static class Program
    {
        const int width = 1280;
        const int height = 720;

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

                while(SDL.PollEvent(out SDL.Event e))
                {
                    if (e.type == SDL.EventType.Quit)
                    {
                        Console.WriteLine("Quit Polled");
                        running = false;
                    }
                }

                SDL.SetRenderDrawColor(renderer, 20, 20, 20, 255);
                SDL.RenderClear(renderer);

                SDL.RenderCircle(renderer, shapeCenter, shapeRadius, SDL.FColor.Green);

                SDL.RenderPresent(renderer);

                SDL.Delay(16);
            }

            SDL.DestroyRenderer(renderer);
            SDL.DestroyWindow(window);
            SDL.Quit();

            Console.WriteLine("Quit Engine");
        }
    }
}
