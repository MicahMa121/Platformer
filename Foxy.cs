﻿
namespace Platformer
{
    public class Foxy
    {
        private float _time;
        private float _animationSpeed;
        public Texture2D Texture { get; set; }
        public float FireCD = 0;
        public Vector2 Position { get; protected set; }
        public Vector2 Origin { get; protected set; }
        public Color Color { get; set; } = Color.White;
        public int Poisoned = 0;
        public float PoisonedSpeed { get; set; } = 0;
        public Rectangle Rectangle { get; set; }
        public float Atk { get; set; } = 10f * Globals.Level;

        public float Def { get; set; }

        public float Rotation { get; protected set; } = 0f;
        public List<List<Texture2D>> Textures { get; set; }

        public SpriteEffects _spriteEffects;
        public enum EnemyStates
        {
            Attack,
            Death,
            Spawn,
            Walk,
            Hurt,
        }
        public EnemyStates States { get; set; }
        public bool RightDirection { get; set; } = true;
        public float Health { get; set; }
        public float MaxHp { get; set; } = 40f * Globals.Level;
        public int width, height;
        public Rectangle Hitbox { get; set; }
        public Foxy(Vector2 position,SpriteEffects spriteEffects)
        {
            _spriteEffects = spriteEffects;
            Position = position;
            Texture2D spritesheet = Globals.Content.Load<Texture2D>("foxy-sprite-sheet");
            Rectangle = new((int)Position.X, (int)Position.Y, spritesheet.Width / 4, spritesheet.Height / 5);
            height = spritesheet.Height / 5;
            width = spritesheet.Width / 4;
            Textures = SpriteSheet(spritesheet, 4, 5);
            States = EnemyStates.Spawn;
            Speed = 0;
            Texture = Textures[(int)States][0];
            Origin = Vector2.Zero;
            _time = 0;
            _animationSpeed = 0.2f;
            _velocity.X = -Speed;
            Health = MaxHp;
        }
        public static List<List<Texture2D>> SpriteSheet(Texture2D spritesheet, int w, int h)
        {
            List<List<Texture2D>> textures = new List<List<Texture2D>>();
            for (int j = 0; j < h; j++)
            {
                List<Texture2D> List = new List<Texture2D>();
                int blanks = 0;
                if (j == 4)
                {
                    blanks = 2;
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
        public int _count = 0;
        public bool IsAttacking { get; set; } = false;
        public bool Dying { get; set; } = false;
        public bool Died { get; set; } = false;
        public List<Slash> Slashes { get; set; } = new List<Slash>();
        public void Update(Vector2 displacement, Tile[,] tiles, List<Platform> Platforms)
        {
            //displacement
            Position += displacement;
            Rectangle = new((int)Position.X, (int)Position.Y, Rectangle.Width, Rectangle.Height);
            Hitbox = ToHitbox(Position);
            for (int i = 0; i < Slashes.Count; i++)
            {
                Slashes[i].Update(displacement, tiles);
                if (Slashes[i].Hit)
                {
                    Slashes.RemoveAt(i);
                    i--;
                }
            }
            if (Health <= 0 && !Dying)
            {
                Dying = true;
                States = EnemyStates.Death;
                _count = 0;
            }
            if (Dying)
            {
                _time += Globals.Time;
                if (_time >= _animationSpeed)
                {
                    _count++;
                    if (_count >= Textures[(int)States].Count)
                    {
                        Died = true;
                    }
                    else
                    {
                        Texture = Textures[(int)States][_count];
                    }
                    _time = 0;
                }
                return;
            }
            if (Globals.OutSideOfScreen(Hitbox))
            {
                return;
            }
            if (RightDirection)
            {
                _velocity.X = -Speed;
                _spriteEffects = SpriteEffects.None;
            }
            else
            {
                _velocity.X = Speed;
                _spriteEffects = SpriteEffects.FlipHorizontally;
            }
            //Textures
            _time += Globals.Time;
            if (!Hurt && !IsAttacking && States != EnemyStates.Spawn)
            {
                States = EnemyStates.Walk;
                Speed = 2;
            }
            if (_time >= _animationSpeed)
            {
                _count++;
                if (_count >= Textures[(int)States].Count)
                {
                    if (States == EnemyStates.Attack)
                    {
                        IsAttacking = false;
                        Hurt = false;
                        Vector2 slashV = Vector2.Zero;
                        if (!RightDirection)
                        {
                            slashV = new Vector2(3, 0);
                        }
                        else
                        {
                            slashV = new Vector2(-3, 0);
                        }
                        Slash slash = new Slash(Globals.Content.Load<Texture2D>("fox-fire-projectile"), 4,1,new(Rectangle.Center.X, Hitbox.Top), slashV, _spriteEffects, 32,32);
                        Slashes.Add(slash);
                    }
                    if (States == EnemyStates.Hurt)
                    {
                        Hurt = false;
                        IsAttacking = false;
                    }
                    if (States == EnemyStates.Spawn)
                    {
                        States = EnemyStates.Walk;
                        Speed = 2;
                    }
                    _count = 0;
                }
                Texture = Textures[(int)States][_count];
                _time = 0;

            }

            //movement
            _velocity.Y += Globals.Gravity;
            //collision
            Vector2 newPos = Position + _velocity;
            Rectangle newHitbox;
            foreach (Tile collider in tiles)
            {
                if (!collider.Visible) continue;
                if (newPos.X != Position.X)
                {
                    newHitbox = ToHitbox(new(newPos.X, Position.Y));
                    if (newHitbox.Intersects(collider.Rectangle))
                    {
                        newPos.X = Position.X;

                        RightDirection = !RightDirection;
                    }
                }
                newHitbox = ToHitbox(new(Position.X, newPos.Y));
                if (newHitbox.Intersects(collider.Rectangle))
                {
                    if (_velocity.Y >= 0)
                    {
                        newPos.Y = collider.Rectangle.Top - height;
                        _velocity.Y = 0;
                    }
                    else if (_velocity.Y < 0)
                    {
                        newPos.Y = collider.Rectangle.Bottom;
                    }
                }
            }
            foreach (var collider in Platforms)
            {
                Rectangle prevHitbox = ToHitbox(Position);
                newHitbox = ToHitbox(new(Position.X, newPos.Y));
                if (newHitbox.Intersects(collider.Rectangle) && prevHitbox.Bottom <= collider.Position.Y)
                {
                    if (_velocity.Y >= 0)
                    {
                        newPos.Y = collider.Rectangle.Top - height;
                        _velocity.Y = 0;
                    }
                }
            }
            Position = newPos;

        }
        public int Speed { get; set; } = 2;
        private Vector2 _velocity;
        public bool Hurt { get; set; }
        public Rectangle ToHitbox(Vector2 pos)
        {
            return new((int)pos.X + width/8, (int)pos.Y+height/4,width*3/4,height*3/4);
        }
        public void Draw()
        {
            //Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), Hitbox, Color.Red);
            Globals.SpriteBatch.Draw(Texture, Position, null, Color, Rotation, Origin, 1f, _spriteEffects, 0f);
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), new Rectangle((int)Position.X, (int)Position.Y, Globals.TileSize, 10), Color.Red);
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), new Rectangle((int)Position.X, (int)Position.Y, (int)(Health / MaxHp * Globals.TileSize), 10), Color.Green);
            foreach (var slash in Slashes)
            {
                slash.Draw();
            }
        }
    }
}