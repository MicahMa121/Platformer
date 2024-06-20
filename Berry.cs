
namespace Platformer
{
    public class Berry
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity;
        private Rectangle rect;
        public Rectangle Rectangle { get { return rect; } }
        public bool Hit { get; set; }
        private SpriteEffects spriteEffects;
        private int width, height;
        private int bounce;
        public Berry(Texture2D texture, Vector2 position, Vector2 velocity, SpriteEffects spriteEffects, int w, int h, int bounce)
        {
            this.texture = texture;
            this.position = position;
            this.velocity = velocity;
            this.bounce = bounce;
            Hit = false;
            rect = Globals.Rectangle(w, h, position);
            this.spriteEffects = spriteEffects;
            width = w;
            height = h;
        }
        public void Update(Vector2 displacement, Tile[,] tiles,List<Platform> platforms)
        {
            position += displacement;
            position += velocity;
            rect = Globals.Rectangle(width, height, position);
            foreach (var tile in tiles)
            {
                if (!tile.Visible) { continue; }
                if (rect.Intersects(tile.Rectangle))
                {
                    if (bounce > 0)
                    {
                        velocity *= -1;
                        position += velocity;
                        bounce--;
                    }
                    else
                    {
                        Hit = true;
                        return;
                    }

                }
            }
            foreach(var platform in platforms)
            {
                if (rect.Intersects(platform.Rectangle))
                {
                    if (bounce > 0)
                    {
                        velocity *= -1;
                        position += velocity;
                        bounce--;
                    }
                    else
                    {
                        Hit = true;
                        return;
                    }

                }

            }

        }
        public void Draw()
        {
            Globals.SpriteBatch.Draw(texture, rect, null, Color.White, 0f, Vector2.Zero, spriteEffects, 0f);
        }
    }
}
