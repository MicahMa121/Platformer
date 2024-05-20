

using Microsoft.Xna.Framework.Graphics;

namespace Platformer
{
    public class Character //https://craftpix.net/freebies/free-3-character-sprite-sheets-pixel-art/
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; protected set; } 
        public Vector2 Origin { get; protected set; }
        public Color Color { get; set; } = Color.White;
        public Rectangle Rectangle { get; set; }
        public float Rotation { get; protected set; } = 0f;
        public List<Texture2D> Textures { get; set; }
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
            Textures = Globals.SpriteSheet(spritesheet, 6, 6);
            Textures.AddRange(Globals.SpriteSheet(spritesheet2, 6, 6));
            States = CharacterStates.Idle;
            Texture = Textures[0];
            Origin = Vector2.Zero;//new(spritesheet.Width / 12, spritesheet.Height / 12);
            Hitbox = new((int)Position.X, (int)Position.Y,60,60);
        }
        private int _count = 0;
        public void Update(Map map)
        {

            //States
            if (InputManager.IsKeyPressed(Keys.D))
            {
                States = CharacterStates.Run;
                if (!RightDirection)
                {
                    for (int i = 0; i < Textures.Count; i++)
                    {
                        Textures[i] = FlipTexture(Textures[i]);
                    }
                    RightDirection = true;
                }
            }
            else if (InputManager.IsKeyPressed(Keys.A))
            {
                States = CharacterStates.Run;
                if (RightDirection)
                {
                    for (int i = 0; i < Textures.Count; i++)
                    {
                        Textures[i] = FlipTexture(Textures[i]);
                    }
                    RightDirection = false;
                }

            }
            else if (Idle)
            {
                States = CharacterStates.Idle;
            }
            if (InputManager.IsKeyClicked(Keys.LeftShift))
            {
                if (States == CharacterStates.Walk)
                {
                    States = CharacterStates.Idle;
                    Idle = true;
                }
                else
                {
                    States = CharacterStates.Walk ;
                    Idle = false;
                }


            }
            if (InputManager.IsKeyClicked(Keys.W))
            {
                States = CharacterStates.Jump;
                Idle = false;
            }
            //Textures
            if (_count % 8 == 0)
                Texture = Textures[_count / 8];
            _count++;
            if (States == CharacterStates.Idle || States == CharacterStates.Hurt)
            {
                if (_count >= ((int)States) * 48 + 24 || _count <= ((int)States) * 48)
                {
                    _count = (int)States * 48;
                }
            }
            else if (States == CharacterStates.Jump)
            {
                if (_count >= ((int)States + 1) * 48 || _count <= ((int)States) * 48)
                {
                    if (Jumped)
                    {
                        _count = (int)States * 48 + 48;
                        Jumped = !Jumped;
                        States = CharacterStates.Idle;
                        Idle = true;
                    }
                    else
                    {
                        _count = (int)States * 48;

                        Jumped = !Jumped;
                    }

                }
            }
            else if (States == CharacterStates.Walk || States == CharacterStates.Run)
            {
                if ((_count >= ((int)States + 1) * 48 || _count <= ((int)States) * 48))
                {
                    if (Jumped)
                    {

                        States = CharacterStates.Jump;
                    }
                    else
                    {
                        _count = (int)States * 48;
                    }
                }
            }
            else if (_count >= ((int)States + 1) * 48|| _count <= ((int)States) * 48)
            {
                _count = (int)States * 48;
            }
            //Movement
            if (InputManager.IsKeyPressed(Keys.A))
            {
                _velocity.X = -Speed;
            }
            else if (InputManager.IsKeyPressed(Keys.D))
            {
                _velocity.X = Speed;
            }
            else { _velocity.X = 0; }
            if (!_grounded)
            {
                _velocity.Y += Gravity;
            }
            if (_grounded&&InputManager.IsKeyClicked(Keys.W))
            {
                _velocity.Y = -Speed*2;
                _grounded = false;
            }
            Vector2 newPos = Position + _velocity;
            //Rectangle newHitbox = new((int)newPos.X + 27, (int)newPos.Y + 5, 26, 57);
            foreach(Tile collider in map.Tiles)
            {
                if (newPos.X != Position.X)
                {
                    Rectangle newHitbox = new((int)newPos.X + 27, (int)Position.Y + 5, 26, 57);
                    if (newHitbox.Intersects(collider.Rectangle))
                    {
                        Position -= new Vector2((int)_velocity.X,0);
                    }
                }
            }
        }
        public int Gravity { get; set; } = 1;
        public int Speed { get; set; } = 4;
        private Vector2 _velocity;
        private bool _grounded = true;
        public Rectangle Hitbox;

        public void Draw()
        {
            Globals.SpriteBatch.Draw(Globals.Content.Load<Texture2D>("rectangle"), Hitbox, Color.Red);
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
