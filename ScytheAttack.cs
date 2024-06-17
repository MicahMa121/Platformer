
namespace Platformer
{
    public class ScytheAttack
    {
        private float _time = 0;
        private float _animationSpeed = 0.05f;
        private Texture2D _texture;
        private Vector2 _position;
        private Rectangle _rect;
        public Rectangle Rectangle { get { return _rect; } }
        public List<List<Texture2D>> _textures;
        private SpriteEffects _spriteEffects;
        private int _width;
        private int _index, _moveIndex;
        public bool Done = false;

        public ScytheAttack(Texture2D texture, Vector2 position, int width, SpriteEffects spriteEffects, int moveIndex) 
        {
            _textures = Globals.SpriteSheet(texture, 6, 5);
            _position = position;
            _width = width;
            _spriteEffects = spriteEffects;

            _rect = Globals.Rectangle(width, width, position);
            _moveIndex = moveIndex;
            _index = 0;
            _texture = _textures[_moveIndex][_index];
        }
        public void Update()
        {
            _time += Globals.Time; 
            if (_time  > _animationSpeed)
            {
                _time = 0;
                _texture = _textures[_moveIndex][_index];
                _index++;
                if (_index >= _textures[_moveIndex].Count)
                {
                    _index = 0;
                    Done = true;
                }
            }

        }
        public void Draw()
        {
            Globals.SpriteBatch.Draw(_texture, _rect,null,Color.White,0f,Vector2.Zero,_spriteEffects,0f);
        }
    }
}
