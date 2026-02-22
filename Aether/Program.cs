using Aether.Rendering;
using Aether.Simulation;
using SDL3;
using System.Numerics;

namespace Aether
{
    internal static class Program
    {
        const int width = 1920;
        const int height = 1080;

        const int particleCount = 50000;
        const int typeCount = 5;

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

            Camera camera = new Camera(width, height);

            bool running = true;
            bool isPaused = false;
            bool stepOneTick = false;

            float deltaTime = 0f;
            ulong lastTime = SDL.GetTicks();

            float accumulator = 0f;

            bool isDragging = false;
            Vector2 lastMouseWorldPos = Vector2.Zero;

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

                if (Input.GetKeyDown(SDL.KeyCode.C))
                {
                    Time.Reset();
                    life = new Life(width, height, particleCount, typeCount);
                    Console.WriteLine("Simulation Cleared");
                }

                if (Input.GetKeyDown(SDL.KeyCode.R))
                {
                    ForceMatrix newMatrix = new ForceMatrix(typeCount);
                    newMatrix.Randomize();
                    life.forceMatrix = newMatrix;
                }

                if (Input.GetKeyDown(SDL.KeyCode.Q))
                {
                    life.forceMatrix.SetPattern(ForcePattern.Atomic);
                }

                if (Input.GetKeyDown(SDL.KeyCode.Escape))
                    running = false; // Quick escape

                if(Input.mouseScrollDelta.y != 0)
                {
                    float zoomFactor = 0.1f;
                    if (Input.mouseScrollDelta.y > 0)
                        camera.zoom += zoomFactor;
                    else
                        camera.zoom -= zoomFactor;

                    camera.zoom = Math.Clamp(camera.zoom, 0.1f, 10f);
                }

                if(Input.GetMouseButtonDown(2))
                {
                    isDragging = true;
                    lastMouseWorldPos = camera.ScreenToWorld(Input.mousePosition);
                }

                if(Input.GetMouseButtonUp(2))
                {
                    isDragging = false;
                }

                if(isDragging && Input.GetMouseButton(2))
                {
                    Vector2 currentWorldPos = camera.ScreenToWorld(Input.mousePosition);
                    Vector2 deltaWorld = currentWorldPos - lastMouseWorldPos;
                    camera.Move(-deltaWorld);
                    lastMouseWorldPos = camera.ScreenToWorld(Input.mousePosition);
                }

                float panSpeed = 500f / camera.zoom * (float)Time.deltaTime;

                if (Input.GetKey(SDL.KeyCode.W))
                    camera.Move(new Vector2(0, -panSpeed));
                if (Input.GetKey(SDL.KeyCode.S))
                    camera.Move(new Vector2(0, panSpeed));
                if (Input.GetKey(SDL.KeyCode.A))
                    camera.Move(new Vector2(-panSpeed, 0));
                if (Input.GetKey(SDL.KeyCode.D))
                    camera.Move(new Vector2(panSpeed, 0));

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
                Renderer.DrawParticleRectBatch(renderer, life, camera);

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
