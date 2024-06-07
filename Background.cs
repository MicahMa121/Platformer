

namespace Platformer
{
    public class Background
    {
        public Texture2D Texture { get; set; }
        public float GameTime { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Velocity { get; set; }
        public float Speed { get; set; }
        public Rectangle Rectangle
        {
            get { return _rectangle; }
            set 
            { 
                if (value.X < 0)

                _rectangle = value; 
            }
        }
       


        private Rectangle _rectangle, _clone;
        private float _rectXLocation, _cloneXLocation;

        public Background(Texture2D tex, int width, int height,int y,float velocity,float speed) 
        {
            GameTime = 0;
            Texture = tex;
            Width = width;
            Height = height;
            _rectangle = new Rectangle(0, Globals.WindowSize.Y - height - y, width, height);
            _rectXLocation = 0f;
            _clone = new Rectangle(Globals.WindowSize.X, Globals.WindowSize.Y - height - y, width, height);
            _cloneXLocation = Globals.WindowSize.X;
            Velocity = velocity;
            Speed = speed;
        }
        public void Update(float displacement)
        {
            float x = displacement * Speed + Velocity;
            _rectXLocation += x;
            _rectangle.X = (int)Math.Floor(_rectXLocation);
            _cloneXLocation += x;
            _clone.X = (int)Math.Floor(_cloneXLocation);


            if (_rectangle.Right < 0)
            {
                _rectangle.X = Globals.WindowSize.X;
                _rectXLocation = _rectangle.X;
                _clone.X = 0;
                _cloneXLocation = _clone.X;
            }
            if (_clone.Right < 0) 
            { 
                _clone.X = Globals.WindowSize.X;
                _cloneXLocation = _clone.X;
                _rectangle.X = 0;
                _rectXLocation = _rectangle.X;
            }
            if (_rectangle.Left > Globals.WindowSize.X)
            {
                _rectangle.X = -Globals.WindowSize.X;
                _rectXLocation = _rectangle.X;
                _clone.X = 0;
                _cloneXLocation = _clone.X;
            }
            if (_clone.Left > Globals.WindowSize.X)
            {
                _clone.X = -Globals.WindowSize.X;
                _cloneXLocation = _clone.X;
                _rectangle.X = 0;
                _rectXLocation = _rectangle.X;
            }
        }
        public void Draw()
        {
            Globals.SpriteBatch.Draw(Texture, _rectangle, Color.White);
            Globals.SpriteBatch.Draw(Texture, _clone, Color.White);
        }

    }
}
