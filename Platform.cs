namespace Platformer
{
    public class Platform
    {
        private Texture2D _texture;
        private Vector2 _position;
        private Rectangle _rect;
        public Vector2 Position { get { return _position; } }
        public Rectangle Rectangle { get { return _rect; } }
        public Rectangle Top {  get { return new(_rect.X,_rect.Y,_rect.Width,1); } }
        public Platform(Texture2D texture, Rectangle rectangle)
        {
            _position = new(rectangle.X,rectangle.Y);
            _texture = texture;
            _rect = rectangle;
        }
        public void Update(Vector2 displacememt)
        {
            _position += displacememt;
            _rect.X = (int)Position.X;
            _rect.Y = (int)Position.Y;
        }
        public void Draw()
        {
            Globals.SpriteBatch.Draw(_texture, _rect, Color.White);
        }
    }
}
