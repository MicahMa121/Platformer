
using static Platformer.Enemy;

namespace Platformer
{
    public class Slash
    {
        private Texture2D texture;
        private List<Texture2D> textureList;
        private float time = 0;
        private int index = 0;
        private Vector2 position;
        private Vector2 velocity;
        private Rectangle rect;
        public Rectangle Rectangle { get { return rect; } }
        public bool Hit { get; set; }
        private SpriteEffects spriteEffects;
        private int width,height;
        public Slash(Texture2D texture, Vector2 position, Vector2 velocity, SpriteEffects spriteEffects,int w,int h)
        {
            this.texture = texture;
            this.position = position;
            this.velocity = velocity;
            Hit = false;
            rect = Globals.Rectangle(w, h, position);
            this.spriteEffects = spriteEffects;
            width = w;
            height = h;
        }
        public Slash(Texture2D spritesheet,int x, int y ,Vector2 position, Vector2 velocity, SpriteEffects spriteEffects, int w, int h)
        {
            textureList = Globals.SpriteSheet(spritesheet, x, y)[0];
            texture = textureList[0];
            this.position = position;
            this.velocity = velocity;
            Hit = false;
            rect = Globals.Rectangle(w, h, position);
            this.spriteEffects = spriteEffects;
            width = w;
            height = h;
        }
        public void Update(Vector2 displacement,Tile[,] tiles)
        {
            if (textureList != null)
            {
                time += Globals.Time;
                if (time > 0.1f)
                {
                    time = 0;
                    index++;
                    if (index>= textureList.Count)
                    {
                        index = 0;
                    }
                    texture = textureList[index];
                }
            }
            position += displacement;
            position += velocity;
            rect = Globals.Rectangle(width,height, position);
            foreach (var tile in tiles)
            {
                if (!tile.Visible) { continue; }
                if (rect.Intersects(tile.Rectangle))
                {
                    Hit = true;
                    return;
                }
            }

        }
        public void Draw()
        {
            Globals.SpriteBatch.Draw(texture, rect,null, Color.White,0f,Vector2.Zero,spriteEffects,0f);
        }
    }
}
