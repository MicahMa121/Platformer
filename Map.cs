﻿using System.IO;
using System.Security;
using System.Text;
using static Platformer.Enemy;

namespace Platformer
{
    public class Map
    {
        public int[,] Map2D()
        {
            int[,] level = new int[102,102];
            
            if (!File.Exists(@"level" + Globals.Level + ".txt"))
            {
                StreamWriter writer = File.CreateText(@"level" + Globals.Level + ".txt");
                for (int j = 0; j < 102; j++)
                {
                    writer.Write('1');
                }
                writer.WriteLine();
                for (int i = 0; i < 100; i++)
                {
                    for (int j = 0; j < 102; j++)
                    {
                        if ((j==0) || (j == 101))
                        {
                            writer.Write('1');
                        }
                        else
                        {
                            writer.Write('0');
                        }
                    }
                    writer.WriteLine();
                }
                for (int j = 0; j < 102; j++)
                {
                    writer.Write('1');
                }
                writer.Close();

            }
            StreamReader reader = new StreamReader(@"level" + Globals.Level + ".txt");

            for (int i = 0; i < 102; i++)
            {
                for (int j = 0; j < 102; j++)
                {
                    int data = Convert.ToInt32(((char)reader.Read()).ToString());
                    level[i, j] = data;
                }
                reader.ReadLine();
            }
            return level;
        }

        private Random _gen = new Random();
        public Tile[,] Tiles;
        private Texture2D _soilTexture = Globals.Content.Load<Texture2D>("Soil");
        private Texture2D _soil2Texture = Globals.Content.Load<Texture2D>("SoilClean");
        private int tileSize = Globals.TileSize;
        public List<Enemy> Enemies = new List<Enemy>();
        public UserInterface UserInterface { get; set; } = new UserInterface();
        public bool Clickable()
        {
            return !UserInterface.UIrect.Contains(InputManager.MouseRectangle)
                && !UserInterface.Settingrect.Contains(InputManager.MouseRectangle)
                && !UserInterface.Menurect.Contains(InputManager.MouseRectangle)&&
                !Shop.Rectangle(Shop.Width,Shop.Height).Contains(InputManager.MouseRectangle);
        }
        public Vector2 MaptoScreen(int x, int y) => new(x * tileSize, y * tileSize);
        public (int x, int y) ScreentoMap (Vector2 position)=> ((int)position.X/ tileSize, (int)position.Y/ tileSize);
        public Map()
        {

            NewGame();
        }
        public void NewGame()
        {
            Coins.Clear();
            Borders.Clear();
            Treasures.Clear();
            Enemies.Clear();
            Money = 0;
            Shop = new Button(new(500, 600), "$ " + Money);
            int[,] map = Map2D();
            AddBorder(2* tileSize, 100 * tileSize);
            Tiles = new Tile[map.GetLength(0), map.GetLength(1)];
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    Tile tile = new(_soilTexture, MaptoScreen(x, y));
                    if (map[x, y] == 1)
                    {
                        if (y - 1 >= 0 && map[x, y - 1] == 1)
                        {
                            tile.Texture = _soil2Texture;
                        }
                        tile.Visible = true;
                    }
                    Tiles[x, y] = tile;
                    if (map[x, y] == 2)
                    {
                        Enemy enemy = new(Globals.Content.Load<Texture2D>("Dog (2)"), MaptoScreen(x, y));
                        Enemies.Add(enemy);
                    }
                    if (map[x, y] == 3)
                    {
                        Treasure treasure = new(Globals.Content.Load<Texture2D>("treasure"), MaptoScreen(x, y)+new Vector2(40,40));
                        Treasures.Add(treasure);
                    }
                }
            }
        }
        public Character Player { get; set; } 
        public List<Rectangle> Borders { get; set; } = new List<Rectangle> ();
        public int Money { get; set; }
        private void AddBorder(int width, int length)
        {

            Rectangle westwall = new Rectangle(tileSize - width, tileSize, width, length);
            Rectangle eastwall = new Rectangle(tileSize +length, tileSize, width, length);
            Rectangle northwall = new Rectangle(tileSize, tileSize - width, length, width);
            Rectangle southwall = new Rectangle(tileSize, tileSize +length, length, width);
            Borders.Add(westwall);
            Borders.Add(eastwall);
            Borders.Add(northwall);
            Borders.Add (southwall);
        }
        public List<Coin> Coins { get; set; } = new List<Coin> ();
        public Button Shop { get; set; } 
        public void Update(Vector2 displacement)
        {

            foreach(var coin in Coins)
            {
                coin.Update(displacement, Tiles);
                if (!coin.Collected&&coin.Rectangle.Intersects(Player.Hitbox(Player.Position)))
                {
                    coin.Collected = true;
                    Money += coin.Value;
                    Shop.Text = "$ " + Money;

                }
            }
            Shop.Update();
            for (int i = 0; i < Coins.Count; i++)
            {
                if (Coins[i].Collected && Coins[i].Opacity <= 0f)
                {
                    Coins.RemoveAt(i);
                    i--;
                }
            }
            foreach (var tile in Tiles)
            {
                if (tile == null) continue;
                tile.Position += displacement;
                tile.Rectangle =  new((int)tile.Position.X, (int)tile.Position.Y, tile.Rectangle.Width, tile.Rectangle.Height);
                
            }
            foreach (Treasure treasure in Treasures)
            {
                treasure.Update(displacement, Tiles);
                if (!treasure.Opened&&Player.Attacking && Player.AttackRange().Intersects(treasure.Rectangle))
                {
                    treasure.Opened = true;
                    treasure.Time = 0;
                }
                else if (treasure.Opened)
                {
                    if (treasure.Time >= 0.05f)
                    {
                        treasure.Time = 0;
                        treasure.Opacity -= 0.1f;
                        Coin coin = new Coin(Globals.Content.Load<Texture2D>("coin"), treasure.Position, 5*_gen.Next(1,3));
                        Coins.Add(coin);
                    }
                }
            }
            for (int i = 0; i < Treasures.Count; i++)
            {
                if (Treasures[i].Opacity <= 0)
                {
                    Treasures.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < Borders.Count; i++)
            {
                Borders[i] = new Rectangle(Borders[i].X + (int)displacement.X, Borders[i].Y+ (int)displacement.Y, Borders[i].Width, Borders[i].Height);
            }
            for (int i = 0; i <Enemies.Count; i++)
            {
                if (Enemies[i].Died)
                {
                    Coin coin = new(Globals.Content.Load<Texture2D>("coin"), new(Enemies[i].Rectangle.Center.X, Enemies[i].Rectangle.Center.Y), 5 * _gen.Next(1, 3));
                    Coins.Add(coin);
                    Enemies.RemoveAt(i);
                    i--;
                }
            }
            foreach (Enemy enemy in Enemies)
            {
                if (UserInterface.open || UserInterface.EditOpen)
                {
                    enemy.Speed = 0;
                }
                else
                {
                    if (Player.Attacking && enemy.Hitbox(enemy.Position).Intersects(Player.AttackRange()))
                    {
                        if (!enemy.Hurt)
                        {
                            if (enemy._spriteEffect != Player.SpriteEffect)
                            {
                                enemy.Health -= 20;
                            }
                            else
                            {
                                enemy.Health -= 5;
                            }

                            enemy.Hurt = true;
                            enemy.States = EnemyStates.Hurt;
                            enemy.Speed = 0;
                        }

                    }
                    if (enemy.Hurt != true && enemy.AttackRange().Intersects(Player.Hitbox(Player.Position)) && !Player.Hurt)
                    {
                        if (!enemy.IsAttacking)
                        {
                            enemy.IsAttacking = true;
                            enemy.States = EnemyStates.Attack;
                            enemy._count = 0;
                            enemy.Speed = 0;
                        }
                        else
                        {
                            Player.States = Character.CharacterStates.Hurt;
                            Player.Hurt = true;
                            Player.Idle = false;
                            Player.Attacking = false;
                            Player.Jumped = false;
                            Player.Health -= 5;
                        }
                    }
                }
                enemy.Update(displacement, Tiles,Player);

            }
            if (UserInterface.MouseState == "soil")
            {
                for (int j = 0; j < Tiles.GetLength(1); j++)
                {
                    for (int i = 0; i < Tiles.GetLength(0); i++)
                    {
                        if (!Tiles[i, j].IsPlayerHere&&
                            Clickable() && Tiles[i, j].Rectangle.Contains(InputManager.MouseRectangle) && InputManager.MouseClicked)
                        {
                            bool add = true;
                            foreach (Enemy enemy in Enemies)
                            {
                                if (Tiles[i, j].Rectangle.Intersects(enemy.Rectangle))
                                {
                                    add = false;break;
                                }
                            }
                            foreach (Treasure treasure in Treasures)
                            {
                                if (Tiles[i, j].Rectangle.Intersects(treasure.Rectangle))
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
                        Enemy enemy = new(Globals.Content.Load<Texture2D>("Dog (2)"), new(InputManager.MouseRectangle.X-20, InputManager.MouseRectangle.Y-40));
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
                    if (Clickable() && Treasures[i].Rectangle.Contains(InputManager.MouseRectangle) && InputManager.MouseClicked)
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
                        Treasure treasure = new(Globals.Content.Load<Texture2D>("treasure"), new(InputManager.MouseRectangle.X, InputManager.MouseRectangle.Y));
                        Treasures.Add(treasure);
                    }
                }
                else
                {
                    DrawItem = false;

                }
            }
        }
        public List<Treasure> Treasures { get; set; } = new List<Treasure>();
        public bool DrawItem { get; set; } = false;
        public void Draw()
        {
            foreach (Tile tile in Tiles)
            {
                if (tile == null) continue;
                tile.Draw();
            }
            if (DrawItem&&UserInterface.MouseState == "enemy")
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("Dog1"), Globals.Rectangle(tileSize,tileSize,new(InputManager.MouseRectangle.X, InputManager.MouseRectangle.Y))
                    ,new Color(Color.White, 0.2f));
            if (DrawItem && UserInterface.MouseState == "treasure")
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("treasure"), Globals.Rectangle(60,45, new(InputManager.MouseRectangle.X, InputManager.MouseRectangle.Y))
                    , new Color(Color.White, 0.2f));
            foreach (Enemy enemy in Enemies)
            {
                enemy.Draw();
            }
            foreach (Treasure treasure in Treasures)
            {
                treasure.Draw();
            }
            foreach (var coin in Coins)
            {
                coin.Draw() ;
            }
            Shop.Draw();
        }
    }
}
