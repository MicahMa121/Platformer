namespace Platformer
{
    public class Boomerang
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity;
        private Rectangle rect;
        public Rectangle Rectangle { get { return rect; } }
        public bool Hit { get; set; }
        private SpriteEffects spriteEffects;
        private float angularVelocity;
        private float angle;
        private float trackDistance;
        public Boomerang(Texture2D texture, Vector2 position, Vector2 velocity, float angular,SpriteEffects spriteEffects)
        {
            this.texture = texture;
            this.position = position;
            this.velocity = velocity;
            this.angularVelocity = angular;

            trackDistance = 0;
            angle = 0;
            Hit = false;
            rect = Globals.Rectangle(Globals.TileSize*3/4,Globals.TileSize*3/4, position);
            this.spriteEffects = spriteEffects;
        }
        public void Update(Vector2 displacement, Tile[,] tiles)
        {
            position += displacement;
            trackDistance += velocity.X;
            position += velocity;
            angle += angularVelocity;
            rect = Globals.Rectangle(Globals.TileSize * 3 / 4, Globals.TileSize * 3 / 4, position);
            if (Math.Abs(trackDistance) >= 4*Globals.TileSize)
            {
                trackDistance = 0;
                velocity *= -1;
                angle *= -1;
            }

        }
        public void Draw()
        {
            Globals.SpriteBatch.Draw(texture, rect, null, Color.White, angle, position, spriteEffects, 0f);

        }
    }
}
