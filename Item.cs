using static Platformer.Button;
using static System.Net.Mime.MediaTypeNames;

namespace Platformer
{
    public class Item
    {
        private Texture2D _texture;
        private Texture2D rectTex = Globals.Content.Load<Texture2D>("rectangle");
        private Vector2 _position;
        private Rectangle _itemRect;
        private Vector2 origin = new(Globals.WindowSize.X/2, Globals.WindowSize.Y/2);
        private SpriteFont font = Globals.Font;
        public float _cost, _atk, _hp, _sta;
        private string _description;
        private int _width, _height;
        private bool DescriptionOpen;
        private Button _buyBtn;
        public bool IsBought { get; set; } = false;
        public Item(Texture2D texture, Vector2 position, float cost, float atk, float hp, float sta, string description)
        {
            _texture = texture;
            _position = position;
            _itemRect = Globals.Rectangle(Globals.TileSize,Globals.TileSize,position);
            _cost = cost;
            _atk = atk;
            _hp = hp;
            _sta = sta;
            _description = description;
            DescriptionOpen = false;
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
            if (DescriptionOpen)
            {
                _buyBtn.Update();
                if (InputManager.MouseClicked && !Globals.Rectangle(_width, _height, origin).Contains(InputManager.MouseRectangle) && !_itemRect.Contains(InputManager.MouseRectangle))
                {
                    DescriptionOpen = false;
                }
                else if (_buyBtn.ButtonPressed())
                {
                    IsBought = true;
                    DescriptionOpen = false;
                }
            }
            else if (InputManager.MouseDown&&_itemRect.Contains(InputManager.MouseRectangle))
            {
                DescriptionOpen = true;
            }
        }
        public void Draw()
        {
            Globals.SpriteBatch.Draw(_texture, _itemRect,Color.White);
            string cost = "$ "+_cost.ToString();
            float height = Globals.Font.MeasureString(cost).Y;
            Globals.SpriteBatch.DrawString(font, cost, TextPosition(_position + new Vector2(0, - 40  ),cost ), Color.Gold);
            if (DescriptionOpen)
            {
                Globals.SpriteBatch.Draw(rectTex, Globals.Rectangle(_width,_height,origin), Color.Purple);
                Globals.SpriteBatch.Draw(rectTex, Globals.Rectangle(_width - _height / 10, _height - _height/10,origin), Color.MediumPurple);
                Globals.SpriteBatch.DrawString(font, _description, TextPosition(origin,_description), Color.White);
                _buyBtn.Draw();
            }
        }
    }
}
