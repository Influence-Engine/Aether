
using SDL3;
using System.Numerics;

namespace Aether
{
    public static class Input
    {
        static Dictionary<SDL.KeyCode, bool> keyStates = new Dictionary<SDL.KeyCode, bool>();
        static HashSet<SDL.KeyCode> keysPressedThisFrame = new HashSet<SDL.KeyCode>();
        static HashSet<SDL.KeyCode> keyReleasedThisFrame = new HashSet<SDL.KeyCode>();

        static bool[] mouseStates = new bool[5];
        static HashSet<int> mouseButtonPressedThisFrame = new HashSet<int>();
        static HashSet<int> mouseButtonReleasedThisFrame = new(); // breh

        static Vector2 mousePosition;
        static Vector2 lastMousePosition;
        static Vector2 mouseDelta;
        static Vector2 mouseScrollDelta;


    }
}
