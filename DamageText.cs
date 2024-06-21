
namespace Platformer
{
    public class DamageText
    {
        private SpriteFont _font = Globals.Content.Load<SpriteFont>("PixelSpriteFont");
        public float Opacity = 1.0f;
        private string _text;
        private Vector2 _position;
        private Color _color;
        private Random gen = new Random();
        public DamageText(string text, Vector2 position, Color color)
        {
            _text = text;
            _position = position+ new Vector2(gen.Next(-10, 11),gen.Next(-10,11));
            _color = color;
        }
        public void Update(Vector2 displacement)
        {
            Opacity -= 0.05f;
            _position += displacement;
            _position.Y -= 0.5f;
        }
        public void UpdatePosition(Vector2 displacement)
        {
            _position += displacement;
        }
        public void Draw()
        {
            Globals.SpriteBatch.DrawString(_font, _text, _position, new Color(_color,Opacity));
        }
    }
}
