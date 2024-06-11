
using static System.Net.Mime.MediaTypeNames;

namespace Platformer
{
    public class Button
    {
        private Texture2D _tex = Globals.Content.Load<Texture2D>("rectangle");
        public Vector2 Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Text { get; set; }
        public Rectangle Rectangle(int width,int height)
        {
            return new Rectangle((int)Position.X - width/2, (int)Position.Y - height / 2, width, height);
        }
        public Vector2 TextPosition()
        {
            return new Vector2(Position.X - Globals.Font.MeasureString(Text).X/2, Position.Y - Globals.Font.MeasureString(Text).Y / 2);
        }
        public enum ButtonStates
        {
            Pressed,
            Unpressed,
            Hover
        }
        public ButtonStates State { get; set; }
        public Button(Vector2 pos, string text)
        {
            State = ButtonStates.Unpressed;
            Position = pos;
            Text = text;
            Width = (int)Globals.Font.MeasureString(text).X*3/2;
            Height = (int)Globals.Font.MeasureString(text).Y;
        }
        public void Update()
        {
            Width = (int)Globals.Font.MeasureString(Text).X * 3 / 2;
            Height = (int)Globals.Font.MeasureString(Text).Y * 2;
            if (Rectangle(Width,Height).Contains(InputManager.MouseRectangle))
            {
                if (InputManager.MouseDown)
                {
                    State = ButtonStates.Pressed;
                }
                else
                {
                    State = ButtonStates.Hover;
                }
            }
            else
            {
                State = ButtonStates.Unpressed;
            }
        }
        public void Draw()
        {
            Globals.SpriteBatch.Draw(_tex, Rectangle(Width,Height), Color.Black);
            if (State == ButtonStates.Pressed)
            {
                Globals.SpriteBatch.Draw(_tex, Rectangle(Width - Height/10, Height * 9 / 10), Color.DarkGray);
                Globals.SpriteBatch.DrawString(Globals.Font, Text, TextPosition(), Color.White);
            }
            else if (State == ButtonStates.Unpressed)
            {
                Rectangle rect = Rectangle(Width - Height / 10, Height * 9 / 10);
                rect.Offset(0, -Height / 20);
                Globals.SpriteBatch.Draw(_tex, rect, Color.DimGray);
                Globals.SpriteBatch.DrawString(Globals.Font, Text, TextPosition() + new Vector2(0, -Height / 20), Color.White);
            }
            else if (State == ButtonStates.Hover)
            {
                Rectangle rect = Rectangle(Width - Height / 10, Height * 9 / 10);
                rect.Offset(0, -Height / 20);
                Globals.SpriteBatch.Draw(_tex, rect, Color.DarkGray);
                Globals.SpriteBatch.DrawString(Globals.Font, Text, TextPosition() + new Vector2(0, -Height / 20), Color.White);
            }
        }
    }
}
