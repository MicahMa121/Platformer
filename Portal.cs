
namespace Platformer
{
    public class Portal
    {
        private List<Texture2D> _textures = new List<Texture2D>();
        private Texture2D _texture;
        private Vector2 _position;
        private Rectangle _rect;
        public Vector2 Position { get { return _position; } }
        public Rectangle Rectangle { get { return _rect; } }
        public Portal(Texture2D spritesheet, Vector2 position)
        {
            for (int j = 0; j < 1; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    int width = spritesheet.Width / 8, height = spritesheet.Height / 1;
                    Rectangle sourceRect = new Rectangle(i * width, j * height, width, height);
                    Texture2D cropTexture = new Texture2D(Globals.Device, width, height);
                    Color[] data = new Color[width * height];
                    spritesheet.GetData(0, sourceRect, data, 0, data.Length);
                    cropTexture.SetData(data);

                    if (_textures.Count <= 8)
                    {
                        _textures.Add(cropTexture);
                    }
                }
            }
            _texture = _textures[_index];
            _position = position;
            _rect = Globals.Rectangle(Globals.TileSize,Globals.TileSize,position);
            _time = 0;
        }
        private float _animationSpeed = 0.1f;
        private float _time;
        private int _index = 0;
        public void Update(Vector2 displacement)
        {
            _position += displacement;
            _rect = Globals.Rectangle(Globals.TileSize, Globals.TileSize, _position);
            _time += Globals.Time;
            if (_time >= _animationSpeed)
            {
                _index++;
                _time = 0;
                if (_index >= _textures.Count)
                {
                    _index = 0;
                }
                _texture = _textures[_index];
            }
        }
        public void Draw()
        {
            Globals.SpriteBatch.Draw(_texture, _rect,Color.White);
        }
    }
}
