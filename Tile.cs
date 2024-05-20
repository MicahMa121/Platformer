
using static Platformer.Character;

namespace Platformer
{
    public class Tile
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; protected set; }
        public Vector2 Origin { get; protected set; } = Vector2.Zero;
        public Color Color { get; set; } = Color.White;
        public Rectangle Rectangle { get; set; }
        public float Rotation { get; protected set; } = 0f;
        public Tile(Texture2D texture, Vector2 position)
        {
            Position = position;
            Rectangle = new((int)Position.X, (int)Position.Y,texture.Width,texture.Height);
            Texture = texture;
        }
        public void Draw()
        {
            Globals.SpriteBatch.Draw(Texture, Position, null, Color, Rotation, Origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
