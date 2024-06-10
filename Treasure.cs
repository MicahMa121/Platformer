namespace Platformer
{
    public class Treasure
    {
        public float Time { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle Rectangle { get; set; }
        private Vector2 _velocity;
        public float Opacity = 1f;
        public bool Opened { get; set; }= false;
        public Treasure(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
            Rectangle = Globals.Rectangle(60,45,position);
            _velocity = Vector2.Zero;
        }
        public void Update(Vector2 displacement, Tile[,] tiles)
        {
            if (Opened)
            {
                Time += Globals.Time;
            }
            Position += displacement;
            //movement
            _velocity.Y += Globals.Gravity;
            //collision
            Vector2 newPos = Position + _velocity;
            foreach (Tile collider in tiles)
            {
                if (!collider.Visible) continue;
                Rectangle = Globals.Rectangle(60, 45, new(Position.X, newPos.Y));
                if (Rectangle.Intersects(collider.Rectangle))
                {
                    if (_velocity.Y >= 0)
                    {
                        newPos.Y = collider.Rectangle.Top-Rectangle.Height/2;
                        _velocity.Y = 0;
                    }
                    else if (_velocity.Y < 0)
                    {
                        newPos.Y = collider.Rectangle.Bottom;
                    }
                }
            }
            Position = newPos;
            Rectangle = Globals.Rectangle(60, 45, Position);
        }
        public void Draw()
        {
            Globals.SpriteBatch.Draw(Texture, Rectangle, new Color(Color.White,Opacity));
        }
    }
}
