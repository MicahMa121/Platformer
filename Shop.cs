namespace Platformer
{
    public class Shop
    {
        private Rectangle ShopInterface = Globals.Rectangle(Globals.WindowSize.X*3/4,Globals.WindowSize.Y/2,new Vector2(Globals.WindowSize.X/2,Globals.WindowSize.Y/2));
        private Rectangle Border = Globals.Rectangle(Globals.WindowSize.X * 3 / 4-10, Globals.WindowSize.Y / 2-10, new Vector2(Globals.WindowSize.X / 2, Globals.WindowSize.Y / 2));
        private List<Image> ShopImages = new List<Image>();
        private List<Button> ShopButtons = new List<Button>();
        private List<Item> ShopItems = new List<Item>();
        private Texture2D RectTex;
        private float attack, health, strength;
        private Button swordbtn, hpbtn, staminabtn;
        public bool OpenShop { get; set; } = false;
        private Item _water;
        public Shop()
        {
            int height = (int)Globals.Font.MeasureString("test").Y;
            RectTex = Globals.Content.Load<Texture2D>("rectangle");
            Image sword = new(Globals.Content.Load<Texture2D>("sword"),Globals.Rectangle(Globals.TileSize,Globals.TileSize,new Vector2(ShopInterface.Left + 40,ShopInterface.Top + 50)),SpriteEffects.None);
            ShopImages.Add(sword);
            Image heart = new(Globals.Content.Load<Texture2D>("heart"), Globals.Rectangle(Globals.TileSize*3/4, Globals.TileSize*3/4, new Vector2(ShopInterface.Left + 140, ShopInterface.Top + 50)), SpriteEffects.None);
            ShopImages.Add(heart);
            Image stamina = new(Globals.Content.Load<Texture2D>("stamina"), Globals.Rectangle(Globals.TileSize, Globals.TileSize, new Vector2(ShopInterface.Left + 240, ShopInterface.Top + 50)), SpriteEffects.None);
            ShopImages.Add(stamina);
            Image z = new(Globals.Content.Load<Texture2D>("stamina"), Globals.Rectangle(Globals.TileSize, Globals.TileSize, new Vector2(ShopInterface.Left + 340, ShopInterface.Top + 50)), SpriteEffects.None);
            ShopImages.Add(z);
            Image x = new(Globals.Content.Load<Texture2D>("stamina"), Globals.Rectangle(Globals.TileSize, Globals.TileSize, new Vector2(ShopInterface.Left + 440, ShopInterface.Top + 50)), SpriteEffects.None);
            ShopImages.Add(x);
            swordbtn = new(new(sword.Rectangle.Center.X, sword.Rectangle.Bottom + height), "Atk " + attack);
            ShopButtons.Add(swordbtn);
            hpbtn = new(new(heart.Rectangle.Center.X, sword.Rectangle.Bottom + height), "MaxHp " + health);
            ShopButtons.Add(hpbtn);
            staminabtn = new(new(stamina.Rectangle.Center.X, stamina.Rectangle.Bottom + height), "Stamina " + strength);
            ShopButtons.Add(staminabtn);
            _water = new(Globals.Content.Load<Texture2D>("water"),new(heart.Rectangle.Center.X, sword.Rectangle.Bottom + 4*height), 100f, 1, 5, 0, "Clean water\nAtk Bonus: 1\nHp Increase: 5\nStamina: 0");
            ShopItems.Add(_water);
        }
        public void Update(float atk, float maxhp, float stamina)
        {
            attack = atk;
            health = maxhp;
            strength = stamina;
            swordbtn.Text = "Atk " + attack;
            hpbtn.Text = "Hp " + health;
            staminabtn.Text = "STA " + strength;
            if (OpenShop)
            {
                foreach (var item in ShopButtons)
                {
                    item.Update();
                }
                foreach (var item in ShopItems)
                {
                    item.Update();
                }
            }
        }

        public void Draw()
        {
            if (OpenShop)
            {
                Globals.SpriteBatch.Draw(RectTex, ShopInterface, Color.SaddleBrown);
                Globals.SpriteBatch.Draw(RectTex, Border, Color.SandyBrown);
                foreach(var item in ShopImages)
                {
                    item.Draw();
                }
                foreach(var item in ShopButtons)
                {
                    item.Draw();
                }
                foreach(var item in ShopItems)
                {
                    item.Draw();
                }
            }
        }

    }
}
