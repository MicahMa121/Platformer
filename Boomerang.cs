using System.Diagnostics;

namespace Platformer
{
    public class Boomerang
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity;
        private Rectangle rect;
        public Rectangle Rectangle 
        { get 
            { 
                return new(rect.X - rect.Width/2+(int)Math.Round(-Math.Sin(angle) * rect.Width / 2)-(int)Math.Round(Math.Cos(angle) * rect.Width / 2)
                    , rect.Y - rect.Height/2 + (int)Math.Round(-Math.Sin(angle) * rect.Width / 2)
                    , rect.Width,rect.Height); 
            } 
        }
        private SpriteEffects spriteEffects;
        private float angularVelocity;
        private float angle;
        private float trackDistance;
        public bool Returning = false;
        public Boomerang(Texture2D texture, Vector2 position, Vector2 velocity, float angular,SpriteEffects spriteEffects)
        {
            this.texture = texture;
            this.position = position;
            this.velocity = velocity;
            this.angularVelocity = angular;

            trackDistance = 0;
            angle = 0;
            rect = Globals.Rectangle(Globals.TileSize*3/4,Globals.TileSize*3/4, position);
            this.spriteEffects = spriteEffects;
        }
        public void Update(Vector2 displacement, Vector2 playerLocation)
        {
            Debug.WriteLine(velocity.X);
            if (Returning)
            {
                velocity = playerLocation - position;
                velocity.Normalize();
                velocity *= 10;
            }
            else
            {
                trackDistance += velocity.X;
                if (Math.Abs(trackDistance) >= 6 * Globals.TileSize)
                {
                    Returning = true;
                    trackDistance = 0;

                    angle *= -1;
                }
            }
            position += displacement;
            position += velocity;
            angle += angularVelocity;
            rect = Globals.Rectangle(Globals.TileSize * 3 / 4, Globals.TileSize * 3 / 4, position);


        }
        public void Draw()
        {
            
            //Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), Rectangle, Color.White);
            Globals.SpriteBatch.Draw(texture, rect, null, Color.White, angle, position, spriteEffects, 0f);

        }
    }
}
