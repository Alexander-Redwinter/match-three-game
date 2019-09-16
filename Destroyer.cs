using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameFoRest
{
    class Destroyer
    {
        private readonly Texture2D texture;
        private Field field;
        private Vector2 location;
        private double timer;

        public DestroyerState State { get; private set; }
        public Point Position { get; private set; }
        public bool Remove { get; private set; }

        public Destroyer(Field parent, Vector2 startLocation, Texture2D texture, DestroyerState destroyerState)
        {
            this.field = parent;
            this.location = startLocation;
            this.texture = texture;
            State = destroyerState;
            Remove = false;
            Position = new Point(-1, -1);
        }

        internal bool Update(GameTime gameTime)
        {
            bool b = CheckBordersAndSetPosition();
            float delta = (float)(200 * gameTime.ElapsedGameTime.TotalSeconds);
            switch (State)
            {
                case DestroyerState.Up:
                    location.Y -= delta;
                    if (location.Y <= field.Rectangle.Top - 50)
                    {
                        Remove = true;
                    }
                    break;
                case DestroyerState.Down:
                    location.Y += delta;
                    if (location.Y >= field.Rectangle.Bottom)
                    {
                        Remove = true;
                    }
                    break;
                case DestroyerState.Left:
                    location.X -= delta;
                    if (location.X <= field.Rectangle.Left - field.CellSize.X)
                    {
                        Remove = true;
                    }
                    break;
                case DestroyerState.Right:
                    location.X += delta;
                    if (location.X >= field.Rectangle.Right)
                    {
                        Remove = true;
                    }
                    break;
                case DestroyerState.Destroying:
                    timer += gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (timer >= Parameters.BonusTriggerTime)
                    {
                        Remove = true;
                    }
                    break;
                case DestroyerState.Triggered:
                    timer += gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (timer >= Parameters.BonusTriggerTime)
                    {
                        Remove = true;
                    }
                    break;
            }
            return b;
        }

        private bool CheckBordersAndSetPosition()
        {
            bool r = false;
            int i = (int)((location.Y - field.Rectangle.Y) / field.CellSize.Y);
            int j = (int)((location.X - field.Rectangle.X) / field.CellSize.X);

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
