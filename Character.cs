

using System.Diagnostics;

namespace Platformer
{
    public class Character //https://craftpix.net/freebies/free-3-character-sprite-sheets-pixel-art/
    {
        private float _time;
        private float _animationSpeed;

        public Texture2D Texture { get; set; }
        public Vector2 Position { get; protected set; } 
        public Vector2 Origin { get; protected set; }
        public Color Color { get; set; } = Color.White;
        public Rectangle Rectangle { get; set; }
        public float Rotation { get; protected set; } = 0f;
        public List<List<Texture2D>> Textures { get; set; }
        public enum CharacterStates
        { 
            Idle,
            Walk,
            Run,
            Push,
            Jump,
            Hurt,
            Death,
            Craft,
            Climb,
            Attack1,
            Attack2,
            Attack3,
        }
        public CharacterStates States { get; set; }
        public bool Hurt { get; set; } = false;
        public bool RightDirection { get; set; } = true;
        public bool Jumped { get; set; } = false;
        public bool Idle { get; set; } = true;
        public float Health { get; set; } 
        public SpriteEffects SpriteEffect { get; set; }
        private List<Slash> slashes = new List<Slash>();
        public Character(Texture2D spritesheet,Texture2D spritesheet2, Vector2 position)
        {
            Position = position;
            Rectangle = new((int)Position.X, (int)Position.Y, spritesheet.Width/6,spritesheet.Height/6);
            Textures = SpriteSheet(spritesheet, 6, 6);
            Textures.AddRange(Globals.SpriteSheet(spritesheet2, 6, 6));
            States = CharacterStates.Idle;
            Texture = Textures[(int)States][0];
            Origin = Vector2.Zero;//new(spritesheet.Width / 12, spritesheet.Height / 12);
            _time = 0;
            Health = Globals.TileSize;
            Stamina = Globals.TileSize;
            _animationSpeed = 0.1f;
            SpriteEffect = SpriteEffects.None;
            Textures[(int)CharacterStates.Hurt].Insert(1, Textures[(int)CharacterStates.Hurt][0]);
            Textures[(int)CharacterStates.Hurt].Insert(2, Textures[(int)CharacterStates.Hurt][2]);
        }
        public static List<List<Texture2D>> SpriteSheet(Texture2D spritesheet, int w, int h)
        {
            List<List<Texture2D>> textures = new List<List<Texture2D>>();
            for (int j = 0; j < h; j++)
            {
                List<Texture2D> List = new List<Texture2D>();
                int blanks = 0;
                if (j == 0)
                {
                    blanks = 2;
                }
                else if (j == 5)
                {
                    blanks = 3;
                }
                for (int i = 0; i < w - blanks; i++)
                {
                    int width = spritesheet.Width / w, height = spritesheet.Height / h;
                    Rectangle sourceRect = new Rectangle(i * width, j * height, width, height);
                    Texture2D cropTexture = new Texture2D(Globals.Device, width, height);
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
        private int _count = 0;
        public bool Attacking { get; set; } = false;
        public bool CanMove { get; set; } = true;
        public float Stamina { get; set; } 
        public bool Casting { get; set; } = false;
        public bool Dashing { get; set; } = false;
        public float Atk { get; set; } = 5;
        public bool Dodge { get; set; } = false;
        public void Update(Map map)
        {
            //Textures
            _time += Globals.Time;

            if (_time >= _animationSpeed)
            {
                Stamina += 3f;
                if (Stamina >= Globals.TileSize)
                {
                    Stamina = Globals.TileSize;
                }
                if (_count >= 3)
                {
                    Dashing = false;
                    Color = Color.White;
                }
                if (_count >= Textures[(int)States].Count)
                {
                    if (States == CharacterStates.Jump&&_velocity.Y>= 0)
                    {
                        Jumped = false;
                    }
                    if (States == CharacterStates.Attack1)
                    {
                        Attacking = false;
                    }
                    if (States == CharacterStates.Craft)
                    {
                        Casting = false;
                        Vector2 slashV = Vector2.Zero;
                        if (RightDirection)
                        {
                            slashV = new Vector2(Speed * 2, 0);
                        }
                        else
                        {
                            slashV = new Vector2(-Speed * 2, 0);
                        }
                        Slash slash = new Slash(Globals.Content.Load<Texture2D>("Slash"), new(Hitbox(Position).Center.X, Hitbox(Position).Center.Y), slashV, SpriteEffect);
                        slashes.Add(slash);
                    }
                    if (States == CharacterStates.Attack2)
                    {
                        Jumped = false;
                        Attacking = false;
                    }
                    if (States == CharacterStates.Hurt)
                    {
                        Hurt = false;
                    }
                    _count = 0;
                    Speed = 5;
                }
                Texture = Textures[(int)States][_count];
                _count++;
                _time = 0;

            }

            _velocity.X = 0;
            if (!Jumped && !Attacking&& !Hurt&&!Casting&&!Dashing)
                Idle = true;
            //_grounded = false;
            //States
            if (CanMove)
            {
                if (InputManager.IsKeyPressed(Keys.D))
                {
                    _velocity.X = Speed;
                    if (!Jumped && !Attacking&&!Hurt&&!Casting)
                        States = CharacterStates.Run;
                    if (!RightDirection)
                    {
                        SpriteEffect = SpriteEffects.None;
                        RightDirection = true;
                    }
                    Idle = false;
                }
                else if (InputManager.IsKeyPressed(Keys.A))
                {
                    _velocity.X = -Speed;
                    if (!Jumped && !Attacking && !Hurt && !Casting)
                        States = CharacterStates.Run;
                    if (RightDirection)
                    {
                        SpriteEffect = SpriteEffects.FlipHorizontally;
                        RightDirection = false;
                    }
                    Idle = false;
                }
                else if (Dashing)
                {
                    if (RightDirection)
                    {
                        _velocity.X = Speed;
                    }
                    else
                    {
                        _velocity.X = -Speed;
                    }
                }
                if (InputManager.IsKeyClicked(Keys.E) && Stamina >= 20f)
                {
                    if (!Casting)
                    {
                        _count = 0;
                        States = CharacterStates.Craft;
                        Idle = false;
                        Casting = true;
                        Hurt = false;
                        Attacking = false;
                        Jumped = false;
                        Stamina -= 20;
                    }
                }
                if (InputManager.IsKeyClicked(Keys.Q) && Stamina >= 20f&&!Dashing)
                {
                    Dashing = true;
                    Speed = 10;
                    Idle = false;   
                    Casting = false;
                    Hurt = false;
                    Attacking = false;
                    Jumped = false;
                    States = CharacterStates.Run;
                    Color = new Color(Color.White, 0.5f);
                    _count = 0;
                    Stamina -= 20;
                }
                if (InputManager.IsKeyClicked(Keys.W) && Stamina >= 20f)
                {
                    _velocity.Y = - 15;

                    Stamina -= 20;
                    if (!Casting)
                    {
                        States = CharacterStates.Jump;
                        Idle = false;
                        Jumped = true;
                        Attacking = false;
                        Hurt = false;
                    }
                }
                if (InputManager.IsKeyClicked(Keys.Space))
                {
                    if (!Attacking)
                        _count = 0;

                    if (Jumped)
                    {
                        States = CharacterStates.Attack2;
                        Idle = false;
                        Attacking = true;
                        Casting = false;
                    }
                    else
                    {
                        States = CharacterStates.Attack1;
                        Idle = false;
                        Attacking = true;
                        Casting = false;
                    }
                    Hurt = false;
                }
                _velocity.Y += Globals.Gravity;
                if (Idle)
                {
                    States = CharacterStates.Idle;
                }
                Vector2 newPos = Position + _velocity;
                Rectangle newHitbox = Hitbox(newPos);
                foreach (Tile collider in map.Tiles)
                {
                    if (!collider.Visible) continue;

                    newHitbox = Hitbox(new(newPos.X, Position.Y));
                    if (newHitbox.Intersects(collider.Rectangle))
                    {
                        newPos.X = Position.X;
                    }

                    newHitbox = Hitbox(new(Position.X, newPos.Y));
                    if (newHitbox.Intersects(collider.Rectangle))
                    {
                        if (_velocity.Y > 0)
                        {
                            newPos.Y = collider.Rectangle.Top - Rectangle.Height;
                            _velocity.Y = 0;
                        }
                        else if (_velocity.Y < 0)
                        {
                            newPos.Y = collider.Rectangle.Bottom - (Rectangle.Height-1-newHitbox.Height);
                        }
                    }
                }
                foreach (Border collider in map.Borders)
                {
                    if (newPos.X != Position.X)
                    {
                        newHitbox = Hitbox(new(newPos.X, Position.Y));
                        if (newHitbox.Intersects(collider.Rectangle))
                        {
                            newPos.X = Position.X;
                        }
                    }
                    newHitbox = Hitbox(new(Position.X, newPos.Y));
                    if (newHitbox.Intersects(collider.Rectangle))
                    {
                        if (_velocity.Y >= 0)
                        {
                            newPos.Y = collider.Rectangle.Top - Rectangle.Height;
                            _velocity.Y = 0;
                        }
                        else if (_velocity.Y < 0)
                        {
                            newPos.Y = collider.Rectangle.Bottom - (Rectangle.Height - 1 - newHitbox.Height);
                        }
                    }
                }
                MapDisplacement = new(0, 0);
                if (newPos.Y >= Globals.WindowSize.Y*3/4 - newHitbox.Height)//down
                {
                    MapDisplacement += new Vector2(0, Globals.WindowSize.Y * 3 / 4 - newHitbox.Height - newPos.Y);
                    newPos.Y = Globals.WindowSize.Y * 3 / 4 - newHitbox.Height;
                }
                else if (newPos.Y <= Globals.WindowSize.Y / 4 - (Globals.TileSize - newHitbox.Height))//up
                {
                    MapDisplacement += new Vector2(0, Globals.WindowSize.Y / 4 - (Globals.TileSize - newHitbox.Height) - newPos.Y);
                    newPos.Y = Globals.WindowSize.Y / 4 - (Globals.TileSize - newHitbox.Height);
                }
                if (newPos.X >= Globals.WindowSize.X * 3 / 4 - 57)//right
                {
                    MapDisplacement += new Vector2(Globals.WindowSize.X * 3 / 4 - (Globals.TileSize - newHitbox.Width) - newPos.X, 0);
                    newPos.X = Globals.WindowSize.X * 3 / 4 - (Globals.TileSize - newHitbox.Width);
                }
                else if (newPos.X <= Globals.WindowSize.X / 4 - 23)//left
                {
                    MapDisplacement += new Vector2(Globals.WindowSize.X / 4 - newHitbox.Width - newPos.X, 0);
                    newPos.X = Globals.WindowSize.X / 4 - newHitbox.Width;
                }
                Position = newPos;
                map.Update(MapDisplacement);
                for (int i =  0; i < slashes.Count; i++)
                {
                    slashes[i].Update(MapDisplacement, map);
                    if (slashes[i].Hit)
                    {
                        slashes.RemoveAt(i);
                        i--;
                    }
                }
            }
            //Debug.WriteLine("y velocity " + _velocity.Y);
        }
        public Vector2 MapDisplacement { get; set; }
        public int Speed { get; set; } = 5;
        private Vector2 _velocity;
        public Rectangle Hitbox(Vector2 pos)
        {
            return new((int)pos.X + 30, (int)pos.Y +22, 20, 57);
        }
        public Rectangle AttackRange()
        {
            int x;
            if (RightDirection)
            {
                x = Hitbox(Position).X + 20;
            }
            else
            {
                x = Hitbox(Position).X - 30;
            }
            return new(x, Hitbox(Position).Y, 30, 57);
        }
        public void Draw()
        {
            if (Attacking)
                Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), AttackRange(), Color.Blue);
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), Hitbox(Position), Color.Red);
            Globals.SpriteBatch.Draw(Texture, Position, null, Color, Rotation, Origin, 1f, SpriteEffect, 0f);
            foreach (Slash slash in slashes)
            {
                slash.Draw();
            }
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), new Rectangle((int)Position.X, (int)Position.Y, Globals.TileSize, 10), Color.Red);
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), new Rectangle((int)Position.X, (int)Position.Y, (int)Health, 10),Color.Green);
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), new Rectangle((int)Position.X, (int)Position.Y+10, (int)Stamina, 2), Color.LightGoldenrodYellow);
        }
        private Texture2D FlipTexture(Texture2D texture)
        {
            int width = texture.Width;
            int height = texture.Height;
            Rectangle sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(0, sourceRect, data, 0, data.Length);
            Texture2D snap = new Texture2D(Globals.Device,width, height);
            Color[] pixelsFlipped = new Color[data.Length];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                   Array.Copy(data, j + i*width, pixelsFlipped, i*width + (width -j-1) , 1);
                }

            }

            snap.SetData(pixelsFlipped);
            return snap;
        }
    }
}
