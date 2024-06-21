using Microsoft.Xna.Framework.Input;

namespace Platformer
{
    public class InputManager
    {
        private static MouseState _prevMouseState;
        public static bool MouseClicked { get; private set; }
        public static bool MouseDown { get; private set; }
        public static bool MouseRightClicked { get; private set; }
        public static Rectangle MouseRectangle { get; private set; }

        private static KeyboardState _keyboardState,_prevKeyboardState;

        public static bool IsKeyPressed(Keys key)
        {
            return _keyboardState.IsKeyDown(key);
        }
        public static bool IsKeyClicked(Keys key)
        {
            return _keyboardState.IsKeyDown(key)&&_prevKeyboardState.IsKeyUp(key);
        }
        public static bool IsKeyReleased(Keys key)
        {
            return _keyboardState.IsKeyUp(key)&&_prevKeyboardState.IsKeyDown(key);
        }
        public static void Update()
        {
            var mouseState = Mouse.GetState();

            MouseClicked = mouseState.LeftButton == ButtonState.Pressed && _prevMouseState.LeftButton == ButtonState.Released;
            MouseRightClicked = mouseState.RightButton == ButtonState.Pressed && _prevMouseState.RightButton == ButtonState.Released;
            MouseRectangle = new(mouseState.Position.X, mouseState.Position.Y, 1, 1);
            MouseDown = mouseState.LeftButton == ButtonState.Pressed;

            _prevMouseState = mouseState;

            _prevKeyboardState = _keyboardState;
            _keyboardState = Keyboard.GetState();



        }
    }
}
