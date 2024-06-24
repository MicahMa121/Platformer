using System.Xml.Schema;

namespace Platformer
{
    public class Kitsune
    {
        private float _time;
        private float _animationSpeed;
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; protected set; }
        public Vector2 Origin { get; protected set; }
        public Color Color { get; set; } = Color.White;
        public int Poisoned { get; set; } = 0;
        public float PoisonedSpeed { get; set; } = 0;
        public float Atk { get; set; } = 5f * Globals.Level;

        public float Def { get; set; }

        public float Rotation { get; protected set; } = 0f;
        public List<List<Texture2D>> Textures { get; set; }

        public SpriteEffects _spriteEffect;
        public enum EnemyStates
        {
            Summon, 
            Death,
            Hurt,
            Heal,
            Walk,
        }
        public EnemyStates States { get; set; }
        public bool RightDirection { get; set; } = true;
        public float Health { get; set; }
        public float MaxHp { get; set; } = 250f * Globals.Level;
        public int _count = 0;
        public bool IsAttacking { get; set; } = false;
        public bool Dying { get; set; } = false;
        public bool Died { get; set; } = false;
        public int Speed { get; set; } = 0;
        public int height, width;
        private Vector2 _velocity;
        public bool Hurt { get; set; }
        public Rectangle Hitbox { get; set; }
        public float SummonCD = 0;
        public float HealCD = 0;
        public Rectangle ToHitbox(Vector2 pos)
        {
            int x = width / 4;
            return new((int)pos.X+x , (int)pos.Y , width/2, height);
        }
        public Kitsune(Vector2 position)
        {
            Textures = SpriteSheet(Globals.Content.Load<Texture2D>("kitsune-v2-sprite-sheet"),4,5);
            _spriteEffect = SpriteEffects.None;
            Position = position;
            States = EnemyStates.Walk;
            Texture = Textures[(int)States][0];
            width = Texture.Width; height = Texture.Height;
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
                if (j == 2)
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
        public void Update(Vector2 displacement, Tile[,] tiles, List<Platform> Platforms)
        {
            //displacement
            Position += displacement;
            Hitbox = ToHitbox(Position);
            SummonCD += Globals.Time;
            //HealCD += Globals.Time;
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
            if (Globals.OutSideOfScreen(ToHitbox(Position)))
            {
                return;
            }
            if (RightDirection)
            {
                _velocity.X = -Speed;
                _spriteEffect = SpriteEffects.None;
            }
            else
            {
                _velocity.X = Speed;
                _spriteEffect = SpriteEffects.FlipHorizontally;
            }
            //Textures
            _time += Globals.Time;
            if (!IsAttacking && !Hurt)
            {
                States = EnemyStates.Walk;
                Speed = 1;
            }

            if (_time >= _animationSpeed)
            {
                _count++;
                if (_count >= Textures[(int)States].Count)
                {
                    if (States == EnemyStates.Summon)
                    {
                        IsAttacking = false;
                        Hurt = false;
                    }
                    if (States == EnemyStates.Heal)
                    {
                        IsAttacking = false;
                        Hurt = false;
                    }
                    if (States == EnemyStates.Hurt)
                    {
                        Hurt = false;
                        IsAttacking = false;
                    }
                    _count = 0;
                }
                Texture = Textures[(int)States][_count];
                _time = 0;

            }
            if (SummonCD > 6&&!IsAttacking&&!Hurt)
            {
                States = EnemyStates.Summon;
                IsAttacking = true;
                Speed = 0;
                SummonCD = 0;
                _count = 0;
            }
            if (HealCD > 4 && !IsAttacking && !Hurt)
            {
                States = EnemyStates.Heal;
                IsAttacking = true;
                Speed = 0;
                HealCD = 0;
                _count = 0;
            }
            //movement
            _velocity.Y += Globals.Gravity;
            //collision
            Vector2 newPos = Position + _velocity;
            Rectangle newHitbox;
            foreach (Tile collider in tiles)
            {
                if (!collider.Visible) continue;
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
                newHitbox = ToHitbox(new(newPos.X, Position.Y));
                if (newHitbox.Intersects(collider.Rectangle))
                {
                    newPos.X = Position.X;
                    RightDirection = !RightDirection;
                }
            }
            foreach (var collider in Platforms)
            {
                Rectangle prevHitbox = Hitbox;
                newHitbox = ToHitbox(new(Position.X, newPos.Y));
                if (newHitbox.Intersects(collider.Rectangle) && prevHitbox.Bottom <= collider.Position.Y)
                {
                    if (_velocity.Y >= 0)
                    {
                        newPos.Y = collider.Rectangle.Top -height;
                        _velocity.Y = 0;
                    }
                }
            }
            Position = newPos;
            Hitbox = ToHitbox(Position);
        }
        public void Draw()
        {
            //Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), AttackRange(), Color.Blue);
            //Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), Hitbox, Color.Red);
            Globals.SpriteBatch.Draw(Texture, Position, null, Color, Rotation, Origin, 1f, _spriteEffect, 0f);
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), new Rectangle((int)Position.X-(Globals.TileSize - width)/2, (int)Position.Y-20, Globals.TileSize, 10), Color.Red);
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), new Rectangle((int)Position.X - (Globals.TileSize - width) / 2, (int)Position.Y-20, (int)(Health / MaxHp * Globals.TileSize), 10), Color.Green);
        }
    }
}
