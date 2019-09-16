using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameFoRest
{
    class Cell
    {
        public Animation Animation { get; private set; }
        public bool IsSelected { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }
        public Shape Shape { get; set; }
        public BonusType Bonus { get; set; }
        public MouseState State { get; set; }

        private Field field;
        private Vector2 location;
        private Point size;
        private Vector2 destination;
        private ShapesAtlas shapeTexture;
        private Texture2D backTexture;
        private Texture2D hoverTexture;
        private int speed;

        private readonly Color backColor = new Color(255, 255, 255);
        public Cell(Field field, Shape shape, ShapesAtlas texture, Texture2D hoverTexture, Texture2D backTexture, int row, int column)
        {
            this.field = field;
            Shape = shape;
            shapeTexture = texture;
            this.backTexture = backTexture;
            this.hoverTexture = hoverTexture;
            Row = row;
            Column = column;

            size = new Point(50, 50);
            location = new Vector2((Column * size.X) + field.Rectangle.X, (Row * size.Y) + field.Rectangle.Y);

            Animation = Animation.Idle;
            State = MouseState.Idle;
            Bonus = BonusType.None;
            IsSelected = false;
        }

        internal bool Update(GameTime gameTime)
        {
            if (Animation == Animation.Idle)
            {
                return false;
            }
            switch (Animation)
            {
                case Animation.Destroy:
                    Shape = Shape.Empty;
                    Animation = Animation.Idle;
                    break;
                case Animation.Drop:
                    location.Y += (float)(speed * gameTime.ElapsedGameTime.TotalSeconds);
                    if (location.Y >= destination.Y)
                    {
                        location.Y = destination.Y;
                        Animation = Animation.Idle;
                    }
                    break;
                case Animation.Swap:
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
                Animation = Animation.Idle;
            }
        }

        private void AnimateSwap(GameTime gameTime)
        {
            //moves horizontally
            if (location.X == destination.X)
            {
                if (location.Y == destination.Y)
                {
                    Animation = Animation.Idle;
                }
                else
                {
                    if (location.Y < destination.Y)
                    {
                        location.Y += (float)(speed * gameTime.ElapsedGameTime.TotalSeconds);
                        if (location.Y >= destination.Y)
                        {
                            location.Y = destination.Y;
                            Animation = Animation.Idle;
                        }
                    }
                    else
                    {
                        location.Y -= (float)(speed * gameTime.ElapsedGameTime.TotalSeconds);
                        if (location.Y <= destination.Y)
                        {
                            location.Y = destination.Y;
                            Animation = Animation.Idle;
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
                        Animation = Animation.Idle;
                    }
                }
                else
                {
                    location.X -= (float)(speed * gameTime.ElapsedGameTime.TotalSeconds);
                    if (location.X <= destination.X)
                    {
                        location.X = destination.X;
                        Animation = Animation.Idle;
                    }
                }
            }
        }

        internal void Draw(SpriteBatch spriteBatch)
        {

            Rectangle rectangle = new Rectangle((int)location.X, (int)location.Y, 60, 60);
            switch (State)
            {
                case MouseState.Idle:
                    break;
                case MouseState.Hover:
                    spriteBatch.Draw(hoverTexture, rectangle, backColor);
                    break;
                case MouseState.Push:
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
            if (Shape != Shape.Empty && Animation != Animation.Destroy)
            {
                State = MouseState.Idle;
                Animation = Animation.Destroy;
                field.BonusDestroyers(Row, Column, Bonus);
            }
        }

        internal void Spawn(Shape shape)
        {
            State = MouseState.Idle;
            Shape = shape;
            Animation = Animation.Idle;
        }

        internal void Fall(Cell cell)
        {
            cell.State = MouseState.Idle;
            cell.Shape = Shape;
            Shape = Shape.Empty;
            cell.Bonus = Bonus;
            Bonus = BonusType.None;

            cell.destination = cell.location;
            cell.location = location;
            cell.speed = Parameters.CellSpeed;

            cell.Animation = Animation.Drop;
        }

        public bool IsAdjacent(Cell cell)
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

        internal void Swap(Cell cell)
        {

            Vector2 tempLocation = location;
            location = cell.location;
            cell.location = tempLocation;
            speed = Parameters.CellSpeed;
            cell.speed = Parameters.CellSpeed;
            Shape tempShape = Shape;
            Shape = cell.Shape;
            cell.Shape = tempShape;
            Animation = Animation.Swap;
            cell.Animation = Animation.Swap;
            cell.Animation = Animation.Swap;
            BonusType tempBonus = Bonus;
            Bonus = cell.Bonus;
            cell.Bonus = tempBonus;
            cell.destination = location;
            destination = cell.location; State = MouseState.Idle;
            cell.State = MouseState.Idle;
        }

    }
}
