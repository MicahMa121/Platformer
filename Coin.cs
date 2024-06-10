
using static Platformer.Enemy;

namespace Platformer
{
    public class Coin
    {
        private float _time;
        private float _animationSpeed = 0.04f;
        private Rectangle _rect;
        private Rectangle _rectDraw;
        public Rectangle Rectangle { get { return _rect; } }
        public Vector2 Position { get; set; }
        private Texture2D _texture;
        private Vector2 _velocity;
        private int _upDown;
        private int _speed;
        private Random _gen = new Random();
        private float _opacity = 1f;
        public float Opacity { get { return _opacity; } }
        public bool Collected { get; set; } = false;
        public int Value { get; set; }
        public Coin(Texture2D texture, Vector2 position, int value)
        {
            _texture = texture;
            Position = position;
            _rect = Globals.Rectangle(10+ 2*value , 10 + 2*value, position);
            Value = value;
            _rectDraw = _rect;
            _time = 0;
            _upDown = 0;
            _speed = 1;
            _velocity = new((float)_gen.Next(-5,5)/10,_gen.Next(-10,-5));
        }
        public void Update(Vector2 displacement, Tile[,] tiles)
        {
            _time += Globals.Time;
            if (Collected)
            {
                if (_opacity <= 0f)
                {
                    return;
                }
                if (_time  >=  _animationSpeed)
                {
                    _opacity += -0.05f;
                    _rectDraw.Y += -3;

                }
                return;
            }

            //displacement
            Position += displacement;
            //movement
            _velocity.Y += (float)Globals.Gravity/4;
            //collision
            Vector2 newPos = Position + _velocity;
            Rectangle newHitbox;
            foreach (Tile collider in tiles)
            {
                if (!collider.Visible) continue;
                if (newPos.X != Position.X)
                {
                    newHitbox = Globals.Rectangle(10 + 2 * Value, 10 + 2 * Value, new(newPos.X, Position.Y));
                    if (newHitbox.Intersects(collider.Rectangle))
                    {
                        newPos.X = Position.X;
                    }
                }
                newHitbox = Globals.Rectangle(10 + 2 * Value, 10 + 2 * Value, new(Position.X, newPos.Y));
                if (newHitbox.Intersects(collider.Rectangle))
                {
                    if (_velocity.Y < 0)
                    {
                        newPos.Y = collider.Rectangle.Bottom+10;
                        _velocity.Y = 0;
                        _velocity.X = 0;
                    }
                    if (_velocity.Y > 0)
                    {
                        newPos.Y = collider.Rectangle.Top - Rectangle.Height/2 - 1;
                        _velocity.Y = 0;
                        _velocity.X = 0;
                    }
                }
            }
            Position = newPos;
            _rect = Globals.Rectangle(10 + 2 * Value, 10 + 2 * Value, Position);
            _rectDraw = _rect;
            _rectDraw.Y = _rect.Y + _upDown;
            if (_time >= _animationSpeed)
            {
                _upDown += _speed;
                if (_upDown >= 5)
                {
                    _speed = -1;
                }
                else if (_upDown <= -5)
                {
                    _speed = 1;
                }
                _time = 0;
            }
        }
        public void Draw()
        {
            Globals.SpriteBatch.Draw(_texture, _rectDraw,new Color(Color.White,_opacity)) ;
        }
    }
}
