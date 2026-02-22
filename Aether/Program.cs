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

            float shapeX = width / 2;
            float shapeY = height / 2;
            float shapeRadius= 20f;

            while (running)
            {
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

                SDL.SetRenderDrawColor(renderer, 0, 255, 120, 255);
                SDL.FRect rect = new()
                {
                    x = shapeX - shapeRadius,
                    y = shapeY - shapeRadius,
                    w = shapeRadius * 2,
                    h = shapeRadius * 2,
                };

                SDL.RenderFillRect(renderer, ref rect);

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
