using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public class Border
    {
        private Rectangle _rect;
        public Rectangle Rectangle { get { return _rect; } }
        public Vector2 Position;
        public Border (Rectangle rect)
        {
            _rect = rect;
            Position = new Vector2(rect.X,rect.Y);
        }
        public void Update(Vector2 displacement)
        {
            Position += displacement;
            _rect.X = (int)Position.X;
            _rect.Y = (int)Position.Y;  
        }
        
    }
}
