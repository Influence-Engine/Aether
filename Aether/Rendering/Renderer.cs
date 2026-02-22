using Aether.Simulation;
using SDL3;
using System.Runtime.CompilerServices;

namespace Aether.Rendering
{
    public static class Renderer
    {
        static SDL.FColor[] colorCache = new SDL.FColor[64];

        static Renderer()
        {
            for (int i = 0; i < colorCache.Length; i++)
                colorCache[i] = GenerateColorForType(i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawParticle(nint renderer, ref Particle particle) => SDL.RenderCircle(renderer, particle.ToFPoint(), particle.radius, GetColorForType(particle.type));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawParticle(nint renderer, ref Particle particle, SDL.FColor color) => SDL.RenderCircle(renderer, particle.ToFPoint(), particle.radius, color);

        public static void DrawParticles(nint renderer, Life simulation)
        {
            var particles = simulation.particles;
            int count = simulation.ParticleCount;

            for (int i = 0; i < count; i++)
            {
                ref Particle particle = ref particles[i];
                DrawParticle(renderer, ref particle);
            }
        }

        public static void DrawParticlesRect(nint renderer, Life simulation)
        {
            var particles = simulation.particles;
            int count = simulation.ParticleCount;

            for (int i = 0; i < count; i++)
            {
                ref Particle particle = ref particles[i];
                SDL.FColor color = GetColorForType(particle.type);

                var rect = new SDL.FRect
                {
                    x = particle.position.X - particle.radius,
                    y = particle.position.Y - particle.radius,
                    w = particle.radius * 2,
                    h = particle.radius * 2
                };

                SDL.SetRenderDrawColor(renderer, (byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), (byte)(color.a * 255));
                SDL.RenderRect(renderer, ref rect);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SDL.FColor GetColorForType(int type) => colorCache[type % colorCache.Length];

        static SDL.FColor GenerateColorForType(int type)
        {
            float hue = (type * 0.618f) % 1f;
            return HueToRGB(hue);
        }

        static SDL.FColor HueToRGB(float hue)
        {
            int i = (int)(hue * 6);
            float f = hue * 6 - i;
            float q = 1 - f;

            return (i % 6) switch
            {
                0 => new SDL.FColor(1f, f, 0f),
                1 => new SDL.FColor(q, 1f, 0f),
                2 => new SDL.FColor(0f, 1f, f),
                3 => new SDL.FColor(0f, q, 1f),
                4 => new SDL.FColor(f, 0f, 1f),
                _ => new SDL.FColor(1f, 0f, q)
            };
        }
    }
}
