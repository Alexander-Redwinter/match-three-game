using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameFoRest
{
    abstract class absUIObject
    {
        protected absUIObject super;
        protected absScreen screen;

        public absUIObject Parent
        {
            get { return super; }
            set
            {
                super = value;
                SetRelativePosition(Rectangle.Location);
            }
        }
        public Rectangle Rectangle { get; set; }

        public void SetRelativePosition(Point point)
        {
            if (super != null)
            {
                point.X += super.Rectangle.X;
                point.Y += super.Rectangle.Y;
            }
            Rectangle = new Rectangle(point, Rectangle.Size);
        }

        public Vector2 GetRelativePosition()
        {
            if (super != null)
            {
                return new Vector2(Rectangle.X - super.Rectangle.X, Rectangle.Y - super.Rectangle.Y);
            }
            return GetAbsolutePosition();
        }

        public Vector2 GetAbsolutePosition()
        {
            return new Vector2(Rectangle.X, Rectangle.Y);
        }

        public void SetSize(Point size)
        {
            Rectangle = new Rectangle(Rectangle.Location, size);
        }

        internal virtual void LoadContent(GraphicsDeviceManager graphics, ContentManager content) { }
        internal virtual void UnloadContent() { }
        internal virtual void Update(GameTime gameTime) { }
        internal virtual void Draw(SpriteBatch spriteBatch) { }

    }
}
