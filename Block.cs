using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameFoRest
{
    class Block
    {

        private Field field;
        private Vector2 location;
        private Point size;
        private Vector2 destination;
        private ShapesAtlas shapeTexture;
        private Texture2D backTexture;
        private Texture2D hoverTexture;
        private int speed;

        public enumAnimation Animation { get; private set; }
        public bool IsSelected { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }

        public Objects Shape { get; set; }
        public Bonus Bonus { get; set; }
        public enumState State { get; set; }
        private readonly Color backColor = new Color(255, 255, 255);
        public Block(Field super, Objects shape, ShapesAtlas texture, Texture2D hoverTexture, Texture2D backTexture, int row, int column)
        {
            this.field = super;
            Shape = shape;
            shapeTexture = texture;
            this.backTexture = backTexture;
            this.hoverTexture = hoverTexture;
            Row = row;
            Column = column;

            size = new Point(50, 50);
            location = new Vector2((Column * size.X) + super.Rectangle.X, (Row * size.Y) + super.Rectangle.Y);

            Animation = enumAnimation.Idle;
            State = enumState.Idle;
            Bonus = Bonus.None;
            IsSelected = false;
        }

        internal bool Update(GameTime gameTime)
        {
            if (Animation == enumAnimation.Idle)
            {
                return false;
            }
            switch (Animation)
            {
                case enumAnimation.Destroy:
                    Shape = Objects.Empty;
                    Animation = enumAnimation.Idle;
                    break;
                case enumAnimation.Drop:
                    location.Y += (float)(speed * gameTime.ElapsedGameTime.TotalSeconds);
                    if (location.Y >= destination.Y)
                    {
                        location.Y = destination.Y;
                        Animation = enumAnimation.Idle;
                    }
                    break;
                case enumAnimation.Swap:
                    AnimateSwap(gameTime);
                    break;
            }
            return true;
        }

        private void Drop(GameTime gameTime)
        {
            location.Y += (float)(speed * gameTime.ElapsedGameTime.TotalSeconds);
            if (location.Y >= destination.Y)
            {
                location.Y = destination.Y;
                Animation = enumAnimation.Idle;
            }
        }

        private void AnimateSwap(GameTime gameTime)
        {
            //moves horizontally
            if (location.X == destination.X)
            {
                if (location.Y == destination.Y)
                {
                    Animation = enumAnimation.Idle;
                }
                else
                {
                    if (location.Y < destination.Y)
                    {
                        location.Y += (float)(speed * gameTime.ElapsedGameTime.TotalSeconds);
                        if (location.Y >= destination.Y)
                        {
                            location.Y = destination.Y;
                            Animation = enumAnimation.Idle;
                        }
                    }
                    else
                    {
                        location.Y -= (float)(speed * gameTime.ElapsedGameTime.TotalSeconds);
                        if (location.Y <= destination.Y)
                        {
                            location.Y = destination.Y;
                            Animation = enumAnimation.Idle;
                        }
                    }
                }
            }
            //moves vertically
            else
            {
                if (location.X < destination.X)
                {
                    location.X += (float)(speed * gameTime.ElapsedGameTime.TotalSeconds);
                    if (location.X >= destination.X)
                    {
                        location.X = destination.X;
                        Animation = enumAnimation.Idle;
                    }
                }
                else
                {
                    location.X -= (float)(speed * gameTime.ElapsedGameTime.TotalSeconds);
                    if (location.X <= destination.X)
                    {
                        location.X = destination.X;
                        Animation = enumAnimation.Idle;
                    }
                }
            }
        }

        internal void Draw(SpriteBatch spriteBatch)
        {

            Rectangle rectangle = new Rectangle((int)location.X, (int)location.Y, 60, 60);
            switch (State)
            {
                case enumState.Idle:
                    break;
                case enumState.Hover:
                    spriteBatch.Draw(hoverTexture, rectangle, backColor);
                    break;
                case enumState.Push:
                    spriteBatch.Draw(backTexture, rectangle, Color.White);
                    break;
            }
            if (IsSelected)
            {
                spriteBatch.Draw(backTexture, rectangle, Color.White);
            }
            shapeTexture.Draw(spriteBatch, Shape, Bonus, location, 1f);
        }

        internal void Destroy()
        {
            if (Shape != Objects.Empty && Animation != enumAnimation.Destroy)
            {
                State = enumState.Idle;
                Animation = enumAnimation.Destroy;
                field.BonusDestroyers(Row, Column, Bonus);
            }
        }

        internal void Spawn(Objects shape)
        {
            State = enumState.Idle;
            Shape = shape;
            Animation = enumAnimation.Idle;
        }

        internal void Fall(Block cell)
        {
            cell.State = enumState.Idle;
            cell.Shape = Shape;
            Shape = Objects.Empty;
            cell.Bonus = Bonus;
            Bonus = Bonus.None;

            cell.destination = cell.location;
            cell.location = location;
            cell.speed = cell.Row * 500;

            cell.Animation = enumAnimation.Drop;
        }

        public bool IsAdjacent(Block cell)
        {
            if ((Column == cell.Column && (Row == cell.Row - 1 || Row == cell.Row + 1)) ||
                ((Row == cell.Row && (Column == cell.Column - 1 || Column == cell.Column + 1))))
            {
                return true;
            }
            return false;
        }

        internal void Switch()
        {
            IsSelected = !IsSelected;
        }

        internal void Swap(Block cell)
        {

            Vector2 tempLocation = location;
            location = cell.location;
            cell.location = tempLocation;
            speed = 500;
            cell.speed = 500;
            Objects tempShape = Shape;
            Shape = cell.Shape;
            cell.Shape = tempShape;
            Animation = enumAnimation.Swap;
            cell.Animation = enumAnimation.Swap;
            cell.Animation = enumAnimation.Swap;
            Bonus tempBonus = Bonus;
            Bonus = cell.Bonus;
            cell.Bonus = tempBonus;
            cell.destination = location;
            destination = cell.location; State = enumState.Idle;
            cell.State = enumState.Idle;
        }

    }
}
