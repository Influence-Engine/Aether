
using SDL3;
using System.Numerics;

namespace Aether
{
    public static class Input
    {
        static Dictionary<SDL.KeyCode, bool> keyStates = new Dictionary<SDL.KeyCode, bool>();
        static HashSet<SDL.KeyCode> keysPressedThisFrame = new HashSet<SDL.KeyCode>();
        static HashSet<SDL.KeyCode> keyReleasedThisFrame = new HashSet<SDL.KeyCode>();

        static bool[] mouseStates = new bool[5]; // 0 Left, 1 Right, 2 Middle
        static HashSet<int> mouseButtonPressedThisFrame = new HashSet<int>();
        static HashSet<int> mouseButtonReleasedThisFrame = new(); // breh

        public static Vector2 mousePosition { get; private set; }
        static Vector2 lastMousePosition;
        public static Vector2 mouseDelta { get; private set; }
        public static Vector2 mouseScrollDelta { get; private set; }

        public static void Update()
        {
            keysPressedThisFrame.Clear();
            keyReleasedThisFrame.Clear();
            mouseButtonPressedThisFrame.Clear();
            mouseButtonReleasedThisFrame.Clear();

            mouseDelta = mousePosition - lastMousePosition;
            lastMousePosition = mousePosition;

            mouseScrollDelta = Vector2.Zero;
        }

        public static void ProcessEvent(SDL.Event e)
        {
            switch(e.type)
            {
                case SDL.EventType.KeyDown:
                    if(!keyStates.ContainsKey(e.key.key) || !keyStates[e.key.key])
                    {
                        keysPressedThisFrame.Add(e.key.key);
                        keyStates[e.key.key] = true;
                    }
                    break;
               case SDL.EventType.KeyUp:
                    if (keyStates.ContainsKey(e.key.key) || keyStates[e.key.key])
                    {
                        keyReleasedThisFrame.Add(e.key.key);
                        keyStates[e.key.key] = false;
                    }
                    break;

                case SDL.EventType.MouseButtonDown:
                    int button = GetMouseButtonIndex(e.button.button);
                    if(button >= 0 && button < mouseStates.Length)
                    {
                        mouseButtonPressedThisFrame.Add(button);
                        mouseStates[button] = true;
                    }
                    break;

                case SDL.EventType.MouseButtonUp:
                    button = GetMouseButtonIndex(e.button.button);
                    if (button >= 0 && button < mouseStates.Length)
                    {
                        mouseButtonReleasedThisFrame.Add(button);
                        mouseStates[button] = false;
                    }
                    break;

                case SDL.EventType.MouseMotion:
                    mousePosition = new Vector2(e.motion.x, e.motion.y);
                    break;

                case SDL.EventType.MouseWheel:
                    mouseScrollDelta = new Vector2(e.wheel.x, e.wheel.y);
                    break;
            }
        }

        static int GetMouseButtonIndex(uint sdlButton)
        {
            return sdlButton switch
            {
                1 => 0, // Left
                2 => 2, // Middle
                3 => 1, // Right
                4 => 3, // X1,
                5 => 4, // X2
                _ => -1
            };
        }
    }
}
