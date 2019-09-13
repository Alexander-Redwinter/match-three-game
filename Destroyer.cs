using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameFoRest
{
    class Destroyer
    {
        private Field super;
        private Vector2 location;
        private Texture2D texture;
        private double timer;
        public destroyerState Direction { get; private set; }
        public Point Position { get; private set; }
        public bool Remove { get; private set; }

        public Destroyer(Field parent, Vector2 location, Texture2D texture, destroyerState direction)
        {
            this.super = parent;
            this.location = location;
            this.texture = texture;
            Direction = direction;
            Remove = false;
            Position = new Point(-1, -1);
        }

        internal bool Update(GameTime gameTime)
        {
            bool b = checkBordersAndSetPosition();
            float delta = (float)(200 * gameTime.ElapsedGameTime.TotalSeconds);
            switch (Direction)
            {
                case destroyerState.Up:
                    location.Y -= delta;
                    if (location.Y <= super.Rectangle.Top - 50)
                    {
                        Remove = true;
                    }
                    break;
                case destroyerState.Down:
                    location.Y += delta;
                    if (location.Y >= super.Rectangle.Bottom)
                    {
                        Remove = true;
                    }
                    break;
                case destroyerState.Left:
                    location.X -= delta;
                    if (location.X <= super.Rectangle.Left - super.CellSize.X)
                    {
                        Remove = true;
                    }
                    break;
                case destroyerState.Right:
                    location.X += delta;
                    if (location.X >= super.Rectangle.Right)
                    {
                        Remove = true;
                    }
                    break;
                case destroyerState.Blow:
                    timer += gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (timer >= 250)
                    {
                        Remove = true;
                    }
                    break;
                case destroyerState.Triggered:
                    timer += gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (timer >= 250)
                    {
                        Remove = true;
                    }
                    break;
            }
            return b;
        }

        private bool checkBordersAndSetPosition()
        {
            bool r = false;
            int i = (int)((location.Y - super.Rectangle.Y) / super.CellSize.Y);
            int j = (int)((location.X - super.Rectangle.X) / super.CellSize.X);

            if (Position.X != j || Position.Y != i)
            {
                r = true;
            }
            Position = new Point(i, j);
            return r && Position.Y < 8 && Position.Y >= 0 && Position.X >= 0 && Position.X < 8;
        }


        internal void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, location, Color.White);
        }
    }
}
