namespace Platformer
{
    public class Shop
    {
        private Rectangle ShopInterface = Globals.Rectangle(Globals.WindowSize.X*3/4,Globals.WindowSize.Y/2,new Vector2(Globals.WindowSize.X/2,Globals.WindowSize.Y/2));
        private Rectangle Border = Globals.Rectangle(Globals.WindowSize.X * 3 / 4-10, Globals.WindowSize.Y / 2-10, new Vector2(Globals.WindowSize.X / 2, Globals.WindowSize.Y / 2));
        private List<Image> ShopImages = new List<Image>();
        private List<Button> ShopButtons = new List<Button>();
        private Texture2D RectTex;
        private float attack, health, strength;
        private Button swordbtn, hpbtn, staminabtn;
        public bool OpenShop { get; set; } = false;
        public Shop()
        {
            int height = (int)Globals.Font.MeasureString("test").Y;
            RectTex = Globals.Content.Load<Texture2D>("rectangle");
            Image sword = new(Globals.Content.Load<Texture2D>("sword"),Globals.Rectangle(Globals.TileSize,Globals.TileSize,new Vector2(ShopInterface.Left + 50,ShopInterface.Top + 50)),SpriteEffects.None);
            ShopImages.Add(sword);
            Image heart = new(Globals.Content.Load<Texture2D>("heart"), Globals.Rectangle(Globals.TileSize*3/4, Globals.TileSize*3/4, new Vector2(ShopInterface.Left + 150, ShopInterface.Top + 50)), SpriteEffects.None);
            ShopImages.Add(heart);
            Image stamina = new(Globals.Content.Load<Texture2D>("stamina"), Globals.Rectangle(Globals.TileSize, Globals.TileSize, new Vector2(ShopInterface.Left + 250, ShopInterface.Top + 50)), SpriteEffects.None);
            ShopImages.Add(stamina);
            Image z = new(Globals.Content.Load<Texture2D>("stamina"), Globals.Rectangle(Globals.TileSize, Globals.TileSize, new Vector2(ShopInterface.Left + 350, ShopInterface.Top + 50)), SpriteEffects.None);
            ShopImages.Add(z);
            Image x = new(Globals.Content.Load<Texture2D>("stamina"), Globals.Rectangle(Globals.TileSize, Globals.TileSize, new Vector2(ShopInterface.Left + 450, ShopInterface.Top + 50)), SpriteEffects.None);
            ShopImages.Add(x);
            swordbtn = new(new(sword.Rectangle.Center.X, sword.Rectangle.Bottom + height), "Atk " + attack);
            ShopButtons.Add(swordbtn);
            hpbtn = new(new(heart.Rectangle.Center.X, sword.Rectangle.Bottom + height), "MaxHp " + health);
            ShopButtons.Add(hpbtn);
            staminabtn = new(new(stamina.Rectangle.Center.X, stamina.Rectangle.Bottom + height), "Stamina " + strength);
            ShopButtons.Add(staminabtn);
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
            }
        }

    }
}
