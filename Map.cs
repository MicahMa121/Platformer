

using Microsoft.Xna.Framework.Graphics;

namespace Platformer
{
    public class Map
    {
        public int[,] Map2D =
        {
            { 0, 0, 1, 1,1 , 1, 1, 1,1,1,1},
            { 1, 0, 1, 1,1 , 0, 1, 1,1,1,1},
            { 1, 0, 0, 0,1 , 0, 1, 1,1,1,0},
            { 1, 0, 0, 2,0 , 0, 0, 0,0,1,0},
            { 1, 1, 1, 0,0 , 1, 1, 1,1,1,0},
            { 1, 1, 1, 1,0 , 0, 1, 1,1,1, 1},
            { 1, 1, 1, 1,1 , 0, 1, 1,1,1, 1},
            { 1, 0 , 0, 0, 0, 0, 1, 1,1,1, 1},
            { 1, 1, 1, 1,1 , 1, 1, 1,1,1, 1},
            { 1, 1, 1, 1,1 , 1, 1, 1,1,1, 1},
            { 1, 1, 1, 1,1 , 1, 1, 1,1,1, 1},
        };
        public List<Tile> Tiles = new List<Tile>();
        private Texture2D _soilTexture = Globals.Content.Load<Texture2D>("Soil");
        private Texture2D _soil2Texture = Globals.Content.Load<Texture2D>("SoilClean");
        public int width = 80;
        public List<Enemy> Enemies = new List<Enemy>();
        public Vector2 MaptoScreen(int x, int y) => new(x * width, y * width);
        public (int x, int y) ScreentoMap (Vector2 position)=> ((int)position.X/width,(int)position.Y/width);
        public Map()
        {
            for (int y = 0; y < Map2D.GetLength(1);y++)
            {
                for (int x = 0; x < Map2D.GetLength(0); x++)
                {
                    if (Map2D[y,x] == 1)
                    {
                        if (y - 1 >= 0 && Map2D[y-1,x] == 0)
                        {
                            Tile tile = new(_soilTexture,MaptoScreen(x,y));
                            Tiles.Add(tile);
                        }
                        else
                        {
                            Tile tile = new(_soil2Texture, MaptoScreen(x,y));
                            Tiles.Add(tile);
                        }

                    }
                    else if (Map2D[y, x] == 2)
                    {
                        Enemy enemy = new(Globals.Content.Load<Texture2D>("Enemy"), MaptoScreen(x, y));
                        Enemies.Add(enemy);
                    }
                }
            }
        }
        public void Update(Vector2 displacement)
        {
            foreach (var tile in Tiles)
            {
                tile.Position += displacement;
                tile.Rectangle =  new((int)tile.Position.X, (int)tile.Position.Y, tile.Rectangle.Width, tile.Rectangle.Height);
            }
            foreach (Enemy enemy in Enemies)
            {
                enemy.Update(displacement, Tiles);
            }
        }
        public void Draw()
        {
            foreach (Tile tile in Tiles)
            {
                tile.Draw();
            }
            foreach(Enemy enemy in Enemies)
            {
                enemy.Draw();
            }
        }
    }
}
