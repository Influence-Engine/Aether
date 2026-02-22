using SDL3;

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

        public static SDL.FPoint mousePosition { get; private set; }
        static SDL.FPoint lastMousePosition;
        public static SDL.FPoint mouseDelta { get; private set; }
        public static SDL.FPoint mouseScrollDelta { get; private set; }

        public static void Update()
        {
            keysPressedThisFrame.Clear();
            keyReleasedThisFrame.Clear();
            mouseButtonPressedThisFrame.Clear();
            mouseButtonReleasedThisFrame.Clear();

            mouseDelta = mousePosition - lastMousePosition;
            lastMousePosition = mousePosition;

            mouseScrollDelta = SDL.FPoint.Zero;
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
                    mousePosition = new SDL.FPoint(e.motion.x, e.motion.y);
                    break;

                case SDL.EventType.MouseWheel:
                    mouseScrollDelta = new SDL.FPoint(e.wheel.x, e.wheel.y);
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

        #region Keyboard

        public static bool AnyKeyDown => keysPressedThisFrame.Count > 0;

        public static bool GetKey(SDL.KeyCode key) => keyStates.TryGetValue(key, out bool state) && state;

        public static bool GetKeyDown(SDL.KeyCode key) => keysPressedThisFrame.Contains(key);

        public static bool GetKeyUp(SDL.KeyCode key) => keyReleasedThisFrame.Contains(key);

        #endregion

        #region Mouse

        public static bool AnyMouseButtonDown => mouseButtonPressedThisFrame.Count > 0;

        public static bool GetMouseButton(int button)
        {
            if (button < 0 || button >= mouseStates.Length)
                return false;

            return mouseStates[button];
        }

        public static bool GetMouseButtonDown(int button) => mouseButtonPressedThisFrame.Contains(button);

        public static bool GetMouseButtonUp(int button) => mouseButtonReleasedThisFrame.Contains(button);

        #endregion
    }
}
