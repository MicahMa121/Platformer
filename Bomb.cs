namespace Platformer
{
    public class Bomb
    {
        private Texture2D _texture;
        public Texture2D Texture { get { return _texture; } set { _texture = value; } }
        private Vector2 _position;
        public Vector2 Position { get { return _position; } set { _position = value; } }
        private Rectangle _rect;
        public Rectangle Rectangle { get { return _rect; } }
        private SpriteEffects _spriteEffects;
        public float Timer;
        public float Count;
        public bool Exploded = false;
        public float Width;
        public Bomb(Texture2D texture, Vector2 pos, int width, SpriteEffects spriteEffects)
        {
            _texture = texture;
            _position = pos;
            Width = width;
            _rect = Globals.Rectangle(width, width, _position);
            _spriteEffects = spriteEffects;
        }
        public void Update(Vector2 displacement)
        {
            _position += displacement;
            _rect.X = (int)_position.X;
            _rect.Y = (int)_position.Y;
            _rect = Globals.Rectangle((int)Width, (int)Width, _position);
        }
        public void Draw()
        {
            //Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), _rect, Color.White);
            Globals.SpriteBatch.Draw(_texture, _rect, null, Color.White, 0f, Vector2.Zero, _spriteEffects, 0);
        }
    }
}
