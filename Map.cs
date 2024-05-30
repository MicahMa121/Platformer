using System.IO;

namespace Platformer
{
    public class Map
    {
        public int[,] Map2D()
        {
            int[,] level = new int[40,40];
            if (!File.Exists(@"level" + 1 + ".txt"))
            {
                StreamWriter writer = File.CreateText(@"level" + 1 + ".txt");
                for (int i = 0; i < 40; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        writer.Write('1');
                    }
                    writer.WriteLine();
                }
                writer.Close();

            }
            StreamReader reader = new StreamReader(@"level" + 1 + ".txt");
            
            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    int data = Convert.ToInt32(((char)reader.Read()).ToString());
                    level[i,j] = data;
                }
                reader.ReadLine();
            }

            return level;
        }

        public Tile[,] Tiles;
        private Texture2D _soilTexture = Globals.Content.Load<Texture2D>("Soil");
        private Texture2D _soil2Texture = Globals.Content.Load<Texture2D>("SoilClean");
        public int width = 80;
        public List<Enemy> Enemies = new List<Enemy>();
        public UserInterface UserInterface { get; set; } = new UserInterface();
        public bool Clickable()
        {
            return !UserInterface.UIrect.Contains(InputManager.MouseRectangle)
                && !UserInterface.Settingrect.Contains(InputManager.MouseRectangle)
                && !UserInterface.Menurect.Contains(InputManager.MouseRectangle);
        }
        public Vector2 MaptoScreen(int x, int y) => new(x * width, y * width);
        public (int x, int y) ScreentoMap (Vector2 position)=> ((int)position.X/width,(int)position.Y/width);
        public Map()
        {
            AddBorder(160,40*80);
            Tiles = new Tile[Map2D().GetLength(0), Map2D().GetLength(1)];
            for (int y = 0; y < Map2D().GetLength(1);y++)
            {
                for (int x = 0; x < Map2D().GetLength(0); x++)
                {
                    Tile tile = new(_soilTexture, MaptoScreen(x, y));
                    if (Map2D()[x,y] == 1)
                    {
                        if (y - 1 >= 0 && Map2D()[x,y-1] == 1)
                        {
                            tile.Texture = _soil2Texture;
                        }
                        tile.Visible = true ;
                    }
                    Tiles[x, y] = tile;
                    if (Map2D()[x, y] == 2)
                    {
                        Enemy enemy = new(Globals.Content.Load<Texture2D>("Enemy"), MaptoScreen(x, y));
                        Enemies.Add(enemy);
                    }
                }
            }
        }
        public List<Rectangle> Borders { get; set; } = new List<Rectangle> ();
        private void AddBorder(int width, int length)
        {

            Rectangle westwall = new Rectangle(0 - width, 0, width, length);
            Rectangle eastwall = new Rectangle(length, 0, width, length);
            Rectangle northwall = new Rectangle(0, 0 - width, length, width);
            Rectangle southwall = new Rectangle(0, length, length, width);
            Borders.Add(westwall);
            Borders.Add(eastwall);
            Borders.Add(northwall);
            Borders.Add (southwall);
        }
        public void Update(Vector2 displacement)
        {
            foreach (var tile in Tiles)
            {
                if (tile == null) continue;
                tile.Position += displacement;
                tile.Rectangle =  new((int)tile.Position.X, (int)tile.Position.Y, tile.Rectangle.Width, tile.Rectangle.Height);

            }
            for (int i = 0; i < Borders.Count; i++)
            {
                Borders[i] = new Rectangle(Borders[i].X + (int)displacement.X, Borders[i].Y+ (int)displacement.Y, Borders[i].Width, Borders[i].Height);
            }
            foreach (Enemy enemy in Enemies)
            {
                if (UserInterface.open || UserInterface.EditOpen)
                {
                    enemy.Speed = 0;
                }
                else
                {
                    enemy.Speed = 2;
                }
                enemy.Update(displacement, Tiles);
            }
            if (UserInterface.MouseState == "soil")
            {
                for (int j = 0; j < Tiles.GetLength(1); j++)
                {
                    for (int i = 0; i < Tiles.GetLength(0); i++)
                    {
                        if (Clickable() && Tiles[i, j].Rectangle.Contains(InputManager.MouseRectangle) && InputManager.MouseClicked)
                        {
                            bool add = true;
                            foreach (Enemy enemy in Enemies)
                            {
                                if (Tiles[i, j].Rectangle.Intersects(enemy.Rectangle))
                                {
                                    add = false;break;
                                }
                            }
                            foreach (Rectangle rect in Treasures)
                            {
                                if (Tiles[i, j].Rectangle.Intersects(rect))
                                {
                                    add = false; break;
                                }
                            }
                            if (Tiles[i, j].Visible)
                            {
                                Tiles[i, j].Visible = false;
                                if (j + 1 < Tiles.GetLength(1) && Tiles[i, j + 1].Visible)
                                {
                                    Tiles[i, j + 1].Texture = _soilTexture;
                                }
                            }
                            else if (add)
                            {
                                Tiles[i, j].Visible = true;
                                if (j + 1 < Tiles.GetLength(1) && Tiles[i, j + 1].Visible)
                                {
                                    Tiles[i, j + 1].Texture = _soil2Texture;
                                }
                                if (j - 1 > 0 && Tiles[i, j - 1].Visible)
                                {
                                    Tiles[i, j].Texture = _soil2Texture;
                                }
                                if (j - 1 > 0 && !Tiles[i, j - 1].Visible)
                                {
                                    Tiles[i, j].Texture = _soilTexture;
                                }
                            }
                        }
                        if (Clickable() && Tiles[i, j].Rectangle.Contains(InputManager.MouseRectangle) && !Tiles[i, j].Visible)
                        {
                            Tiles[i, j].Hover = true;
                        }
                        else
                        {
                            Tiles[i, j].Hover = false;
                        }
                    }
                }
            }
            bool IsTouchingSoil = false;
            foreach (Tile tile in Tiles)
            {
                if (tile.Visible&&tile.Rectangle.Contains(InputManager.MouseRectangle)) { IsTouchingSoil = true; break; }
            }
            if (UserInterface.MouseState == "enemy")
            {
                bool Add = true;
                for (int i = 0; i < Enemies.Count; i++)
                {
                    if (Clickable() && Enemies[i].Rectangle.Contains(InputManager.MouseRectangle) && InputManager.MouseClicked)
                    {
                        Enemies.RemoveAt(i);
                        i--;
                        Add = false;
                    }
                }
                if (Clickable() && !IsTouchingSoil)
                {
                    DrawItem = true;
                    if (InputManager.MouseClicked&&Add)
                    {
                        Enemy enemy = new(Globals.Content.Load<Texture2D>("Enemy"), new(InputManager.MouseRectangle.X-20, InputManager.MouseRectangle.Y-40));
                        Enemies.Add(enemy);
                    }
                }
                else
                {
                    DrawItem = false;

                }
            }
            if (UserInterface.MouseState == "treasure")
            {
                bool Add = true;
                for (int i = 0; i < Treasures.Count; i++)
                {
                    if (Clickable() && Treasures[i].Contains(InputManager.MouseRectangle) && InputManager.MouseClicked)
                    {
                        Treasures.RemoveAt(i);
                        i--;
                        Add = false;
                    }
                }
                if (Clickable() && !IsTouchingSoil)
                {
                    DrawItem = true;
                    if (InputManager.MouseClicked && Add)
                    {
                        Rectangle rect = Globals.Rectangle(60, 45, new(InputManager.MouseRectangle.X, InputManager.MouseRectangle.Y));
                        Treasures.Add(rect);
                    }
                }
                else
                {
                    DrawItem = false;

                }
            }
        }
        public List<Rectangle> Treasures { get; set; } = new List<Rectangle>();
        public bool DrawItem { get; set; } = false;
        public void Draw()
        {
            foreach (Tile tile in Tiles)
            {
                if (tile == null) continue;
                tile.Draw();
            }
            if (DrawItem&&UserInterface.MouseState == "enemy")
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("Enemy1"), Globals.Rectangle(80,80,new(InputManager.MouseRectangle.X, InputManager.MouseRectangle.Y))
                    ,new Color(Color.White, 0.2f));
            if (DrawItem && UserInterface.MouseState == "treasure")
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("treasure"), Globals.Rectangle(60,45, new(InputManager.MouseRectangle.X, InputManager.MouseRectangle.Y))
                    , new Color(Color.White, 0.2f));
            foreach (Enemy enemy in Enemies)
            {
                enemy.Draw();
            }
            foreach (Rectangle rectangle in Treasures)
            {
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("treasure"), rectangle
                    , Color.White);
            }
        }
    }
}
