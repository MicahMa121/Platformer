

using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

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
            Attack3
        }
        public CharacterStates States { get; set; }

        public bool RightDirection { get; set; } = true;
        public bool Jumped { get; set; } = false;
        public bool Idle { get; set; } = true;
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
            _animationSpeed = 0.1f;
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

                    if (List.Count < w)
                    {
                        List.Add(cropTexture);
                    }
                }
                textures.Add(List);
            }
            return textures;
        }
        private int _count = 0;
        public void Update(Map map, GameTime gameTime)
        {
            //Textures
            _time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_time >= _animationSpeed)
            {
                if (_count >= Textures[(int)States].Count)
                {
                    if (States == CharacterStates.Jump)
                    {
                        Jumped = false;
                    }
                    _count = 0;
                }
                Texture = Textures[(int)States][_count];
                _count++;
                _time = 0;

            }

            _velocity.X = 0;
            if (!Jumped)
                Idle = true;
            _grounded = false;
            //States
            if (InputManager.IsKeyPressed(Keys.D))
            {
                _velocity.X = Speed;
                if (!Jumped)
                    States = CharacterStates.Run;
                if (!RightDirection)
                {
                    for (int i = 0; i < Textures.Count; i++)
                    {
                        for (int j = 0; j < Textures[i].Count; j++)
                        {
                            Textures[i][j] = FlipTexture(Textures[i][j]);
                        }
                    }
                    RightDirection = true;
                }
                Idle = false;
            }
            else if (InputManager.IsKeyPressed(Keys.A))
            {
                _velocity.X = -Speed;
                if (!Jumped)
                    States = CharacterStates.Run;
                if (RightDirection)
                {
                    for (int i = 0; i < Textures.Count; i++)
                    {
                        for (int j = 0; j < Textures[i].Count; j++)
                        {
                            Textures[i][j] = FlipTexture(Textures[i][j]);
                        }
                    }
                    RightDirection = false;
                }
                Idle = false;
            }
            if (InputManager.IsKeyClicked(Keys.LeftShift))
            {
                if (States == CharacterStates.Walk)
                {
                    States = CharacterStates.Idle;
                }
                else
                {
                    States = CharacterStates.Walk ;
                    Idle = false;
                }
            }
            if (InputManager.IsKeyClicked(Keys.W))
            {
                _velocity.Y = -Gravity*15;
                _grounded = false;
                States = CharacterStates.Jump;
                Idle = false;
                Jumped = true;
            }
            if (!_grounded)
            {
                _velocity.Y += Gravity;
            }
            if (Idle)
            {
                States = CharacterStates.Idle; 
            }
            Vector2 newPos = Position + _velocity;
            Rectangle newHitbox;
            foreach(Tile collider in map.Tiles)
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
                        newPos.Y = collider.Rectangle.Top - 64;
                        _grounded = true;
                        _velocity.Y = 0;
                    }
                    else if (_velocity.Y < 0)
                    {
                        newPos.Y = collider.Rectangle.Bottom;
                    }
                }
            }
            MapDisplacement = new(0,0);
            if (newPos.Y >= 640-160 - 63 )//down
            {
                MapDisplacement += new Vector2(0, 640 - 160 - 63-newPos.Y);
                newPos.Y = 640-160- 63;
            }
            else if (newPos.Y <= 160 - 17)//up
            {
                MapDisplacement += new Vector2(0, 160-17 - newPos.Y);
                newPos.Y = 160 - 17;
            }
            if (newPos.X >= 640 - 160 - 57)//right
            {
                MapDisplacement += new Vector2(640 - 160 - 57 - newPos.X,0);
                newPos.X = 640 - 160 - 57;
            }
            else if (newPos.X <= 160-23)//left
            {
                MapDisplacement += new Vector2(160 - 23 - newPos.X,0);
                newPos.X = 160-23;
            }
            Position = newPos;
            map.Update(MapDisplacement);
        }
        public Vector2 MapDisplacement { get; set; }
        public int Gravity { get; set; } = 1;
        public int Speed { get; set; } = 4;
        private Vector2 _velocity;
        private bool _grounded = true;
        public Rectangle Hitbox(Vector2 pos)
        {
            return new((int)pos.X + 27, (int)pos.Y + 5, 26, 57);
        }

        public void Draw()
        {
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), Hitbox(Position), Color.Red);
            Globals.SpriteBatch.Draw(Texture, Position, null, Color, Rotation, Origin, 1f, SpriteEffects.None, 0f);
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
