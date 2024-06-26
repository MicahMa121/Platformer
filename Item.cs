using static Platformer.Button;
using static System.Net.Mime.MediaTypeNames;

namespace Platformer
{
    public class Item
    {
        public Texture2D _texture;
        private Texture2D rectTex = Globals.Content.Load<Texture2D>("rectangle");
        public Vector2 _position;
        public Vector2 Position { get {  return _position; } set { _position = value; } }
        private Rectangle _itemRect;
        private Vector2 origin = new(Globals.WindowSize.X/2, Globals.WindowSize.Y/2);
        private SpriteFont font = Globals.Font;
        public float _cost, _atk, _hp, _sta,_maxHp;
        public string _description;
        private int _width, _height;
        public bool DescriptionOpen;
        public Button _buyBtn;
        public string _skillz, _skillx;
        public bool IsBought { get; set; } = false;
        public bool Sold { get; set; } = false;
        public Item(Texture2D texture, Vector2 position, float cost, float atk,float maxhp, float hp, float sta, string description,string skillz,string skillx)
        {
            _texture = texture;
            _position = position;
            _itemRect = Globals.Rectangle(Globals.TileSize,Globals.TileSize,position);
            _cost = cost;
            _atk = atk;
            _maxHp = maxhp;
            _hp = hp;
            _sta = sta;
            _description = description;
            DescriptionOpen = false;
            _skillz = skillz;
            _skillx = skillx;
            _width = (int)Globals.Font.MeasureString(_description).X * 2;
            _height = (int)Globals.Font.MeasureString(_description).Y * 2;
            _buyBtn = new(new(origin.X,origin.Y+_height/2 + Globals.TileSize),"Buy $" +cost);
        }
        private Vector2 TextPosition(Vector2 position,string text)
        {
            return new Vector2(position.X - Globals.Font.MeasureString(text).X / 2, position.Y - Globals.Font.MeasureString(text).Y / 2);
        }
        public void Update()
        {
            _itemRect = Globals.Rectangle(Globals.TileSize, Globals.TileSize, _position);
            if (DescriptionOpen)
            {
                _buyBtn.Update();
                if (_buyBtn.ButtonPressed()&&!Sold)
                {
                    IsBought = true;
                    DescriptionOpen = false;
                }
                else if (InputManager.MouseClicked && !Globals.Rectangle(_width, _height, origin).Contains(InputManager.MouseRectangle) && !_itemRect.Contains(InputManager.MouseRectangle))
                {
                    DescriptionOpen = false;
                }

            }
        }
        public void Draw()
        {
            if (Sold)
            {
                Globals.SpriteBatch.Draw(_texture, _itemRect, Color.Gray);
                string cost = "SOLD";
                float height = Globals.Font.MeasureString(cost).Y;
                Globals.SpriteBatch.DrawString(font, cost, TextPosition(_position + new Vector2(0, -40), cost), Color.Red);
            }
            else
            {
                Globals.SpriteBatch.Draw(_texture, _itemRect, Color.White);
                string cost = "$ " + _cost.ToString();
                float height = Globals.Font.MeasureString(cost).Y;
                Globals.SpriteBatch.DrawString(font, cost, TextPosition(_position + new Vector2(0, -40), cost), Color.Gold);
            }
        }
        public void DrawDescription()
        {
            Globals.SpriteBatch.Draw(rectTex, Globals.Rectangle(_width, _height, origin), Color.Purple);
            Globals.SpriteBatch.Draw(rectTex, Globals.Rectangle(_width - _height / 10, _height - _height / 10, origin), Color.MediumPurple);
            Globals.SpriteBatch.DrawString(font, _description, TextPosition(origin, _description), Color.White);
            if (!Sold)
                _buyBtn.Draw();
        }
    }
}
