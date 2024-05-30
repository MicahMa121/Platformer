
namespace Platformer
{
    public class Tile
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; protected set; } = Vector2.Zero;
        public Color Color { get; set; } = Color.White;
        public Rectangle Rectangle { get; set; }
        public float Rotation { get; protected set; } = 0f;
        public bool Visible { get; set; } = false;
        public bool Hover { get; set; } = false;
        public Tile(Texture2D texture, Vector2 position)
        {
            Position = position;
            Rectangle = new((int)Position.X, (int)Position.Y,texture.Width,texture.Height);
            Texture = texture;
        }
        public void Draw()
        {
            if (Visible)
            {
                Globals.SpriteBatch.Draw(Texture, Position, null, Color, Rotation, Origin, 1f, SpriteEffects.None, 0f);
            }
            else if (Hover)
            {
                Globals.SpriteBatch.Draw(Texture, Position, null, new Color(Color.White,0.2f), Rotation, Origin, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
