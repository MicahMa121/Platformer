

namespace Platformer
{
    internal class Globals
    {
        public static float Time { get; set; }
        public static ContentManager Content { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static Point WindowSize { get; set; }
        public static GraphicsDevice Device { get; set; }
        public static void Update(GameTime gt)
        {
            Time = (float)gt.ElapsedGameTime.TotalSeconds;
        }
        
        public static List<Texture2D> SpriteSheet(Texture2D spritesheet,int w, int h)
        {
            List<Texture2D> textures = new List<Texture2D>();
            for (int j = 0; j < h; j++)
            {
                for (int i = 0; i < w; i++)
                {
                    int width = spritesheet.Width / w, height = spritesheet.Height / h;
                    Rectangle sourceRect = new Rectangle(i * width, j * height, width, height);
                    Texture2D cropTexture = new Texture2D(Device, width, height);
                    Color[] data = new Color[width * height];
                    spritesheet.GetData(0, sourceRect, data, 0, data.Length);
                    cropTexture.SetData(data);
                    if (textures.Count < h*w)
                    {
                        textures.Add(cropTexture);
                    }
                }
            }
            return textures;
        }
    }
}
