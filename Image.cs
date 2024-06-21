using System.Text;

namespace Platformer
{
    public class Image
    {
        private Texture2D _texture;
        public Texture2D Texture { get { return _texture; } set { _texture = value; } }
        private Vector2 _position;
        public Vector2 Position { get { return _position; } set { _position = value; } }
        private Rectangle _rect;
        public Rectangle Rectangle {  get { return _rect; } }
        public int X { get { return _rect.X; } set { _rect.X = value; _position.X = value; } }
        public float Opacity { get; private set; }
        private SpriteEffects _spriteEffects;
        public Image(Texture2D texture, Rectangle rect, SpriteEffects spriteEffects)
        {
            _texture = texture;
            _position = new(rect.X,rect.Y);
            _rect = rect;
            _spriteEffects = spriteEffects;
            Opacity = 1f;
        }
        public void Update()
        {
            
            Opacity -= 0.05f;
        }
        public void UpdatePosition(Vector2 displacement)
        {
            _position += displacement;
            _rect.X = (int)_position.X;
            _rect.Y = (int)_position.Y;
        }
        public void Draw()
        {
            Globals.SpriteBatch.Draw(_texture, _rect, null, new Color(Color.White, Opacity), 0f, Vector2.Zero, _spriteEffects, 0);
        }
    }
}
