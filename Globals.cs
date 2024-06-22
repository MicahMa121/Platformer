

namespace Platformer
{
    internal class Globals
    {
        public static int Level { get; set; }
        public static float Time { get; set; }
        public static ContentManager Content { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static Point WindowSize { get; set; }
        public static GraphicsDevice Device { get; set; }
        public static SpriteFont Font { get; set; }
        public static float Gravity { get; set; } = 1;
        public static int TileSize { get; set; } 
        public static bool Pause { get; set; } = false;
        public static bool OutSideOfScreen(Rectangle rectangle)
        {
            return (rectangle.Top >= WindowSize.Y*3/2) || (rectangle.Left >= WindowSize.X*3/2) || (rectangle.Bottom <= -WindowSize.Y/2)||(rectangle.Right <= -WindowSize.X/2) ;
        }
        public static void Update(GameTime gt)
        {
            Time = (float)gt.ElapsedGameTime.TotalSeconds;
        }
        public static Rectangle Rectangle(int width, int height, Vector2 origin)
        {
            return new Rectangle((int)origin.X - width / 2, (int)origin.Y - height / 2, width, height);
        }
        public static List<List<Texture2D>> SpriteSheet(Texture2D spritesheet, int w, int h)
        {
            List<List<Texture2D>> textures = new List<List<Texture2D>>();
            for (int j = 0; j < h; j++)
            {
                List<Texture2D> List = new List<Texture2D>();
                for (int i = 0; i < w; i++)
                {
                    int width = spritesheet.Width / w, height = spritesheet.Height / h;
                    Rectangle sourceRect = new Rectangle(i * width, j * height, width, height);
                    Texture2D cropTexture = new Texture2D(Device, width, height);
                    Color[] data = new Color[width * height];
                    spritesheet.GetData(0, sourceRect, data, 0, data.Length);
                    cropTexture.SetData(data);

                    if (List.Count <= w)
                    {
                        List.Add(cropTexture);
                    }
                }
                textures.Add(List);
            }
            return textures;
        }
    }
}
