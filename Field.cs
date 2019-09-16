using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameFoRest
{
    class Field : absUIObject, IDisposable
    {
        public Cell[,] Cells;
        public List<Destroyer> Destroyers;

        private Cell currentCell;
        private Cell selectedCell;
        private Texture2D backTexture;
        private Texture2D destroyerTexture;
        private Texture2D hoverTexture;
        private ShapesAtlas shapesAtlas;
        private Random random;
        private Array shapes;

        public Point CellSize { get; private set; }
        public bool busy { get; private set; }

        public Field(absScreen screen)
        {
            this.screen = screen;
            Cells = new Cell[8, 8];
            CellSize = new Point(50, 50);
            SetSize(new Point(400, 400));
            Destroyers = new List<Destroyer>();
            random = new Random();
            shapes = Enum.GetValues(typeof(Shape));
            busy = false;
        }

        internal override void LoadContent(GraphicsDeviceManager graphics, ContentManager content)
        {
            shapesAtlas = new ShapesAtlas(content.Load<Texture2D>("figures"));
            backTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            backTexture.SetData(new[] { Color.White });
            destroyerTexture = content.Load<Texture2D>("destroyer");
            hoverTexture = content.Load<Texture2D>("hover");
            backTexture = content.Load<Texture2D>("select");

            if (Parameters.UseMap)
            {
                for (int i = 0; i < Parameters.Map.GetLength(0); i++)
                {
                    for (int j = 0; j < Parameters.Map.GetLength(1); j++)
                    {
                        Cell cell = new Cell(this, Parameters.Map[i,j], shapesAtlas, hoverTexture, backTexture, i, j);
                        Cells[i, j] = cell;
                    }
                }
            }
            else
            {
                for (int i = 0; i < Cells.GetLength(0); i++)
                {
                    for (int j = 0; j < Cells.GetLength(1); j++)
                    {
                        Cell cell = new Cell(this, Shape.Empty, shapesAtlas, hoverTexture, backTexture, i, j);
                        Cells[i, j] = cell;
                    }
                }
            }
        }

        internal override void UnloadContent()
        {
            backTexture?.Dispose();
        }

        internal override void Update(GameTime gameTime)
        {
            busy = false;
            foreach (var cell in Cells)
            {
                if (cell.Animation != Animation.Idle)
                {
                    busy = true;
                }
                cell.Update(gameTime);
            }


            List<Point> toDestroy = new List<Point>(Destroyers.Count);
            foreach (var destroyer in Destroyers)
            {
                busy = true;
                if (destroyer.Update(gameTime))
                {
                    toDestroy.Add(new Point(destroyer.Position.X, destroyer.Position.Y));
                }
            }
            var detonated = Destroyers.FindAll(d => d.Remove && d.State == DestroyerState.Destroying);
            addDestroyers(detonated);
            Destroyers.RemoveAll(d => d.Remove);
            int score = toDestroy.Count(c => Cells[c.X, c.Y].Animation == Animation.Idle) / 10;
            ((GameScreen)screen).AddScore(score);
            toDestroy.ForEach(point => Cells[point.X, point.Y].Destroy());
        }

        private void addDestroyers(List<Destroyer> detonated)
        {
            foreach (var item in detonated)
            {
                for (int i = item.Position.X - 1; i <= item.Position.X + 1; i++)
                {
                    for (int j = item.Position.Y - 1; j <= item.Position.Y + 1; j++)
                    {
                        if (i == item.Position.X && j == item.Position.Y)
                        {
                            continue;
                        }
                        Destroyers.Add(new Destroyer(this, new Vector2(j * CellSize.X + Rectangle.X, i * CellSize.Y + Rectangle.Y),
                            destroyerTexture, DestroyerState.Triggered));
                    }
                }
            }
        }

        internal override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var cell in Cells)
            {
                cell.Draw(spriteBatch);
            }
            foreach (var destroyer in Destroyers)
            {
                destroyer.Draw(spriteBatch);
            }
        }

        internal bool Spawn()
        {
            bool spawning = false;

            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(1); j++)
                {
                    if (Cells[i, j].Shape == Shape.Empty)
                    {
                        spawning = true;
                        Shape shape = (Shape)shapes.GetValue(random.Next(shapes.Length - 1) + 1);
                        Cells[i, j].Spawn(shape);
                        Cells[i, j].Bonus = BonusType.None;
                    }
                }
            }
            return spawning;
        }

        internal int Match()
        {
            int matchScore = 0;
            List<Cell> toDestroy = new List<Cell>(64);
            List<Cell> newBonuses = new List<Cell>();

            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                List<Cell> temp = new List<Cell>(8) { Cells[i, 0] };
                for (int j = 1; j < Cells.GetLength(1); j++)
                {
                    bool stop = false;
                    if (Cells[i, j].Shape == temp[0].Shape)
                    {
                        temp.Add(Cells[i, j]);
                    }
                    else
                    {
                        stop = true;
                    }
                    if (stop || j == Cells.GetLength(1) - 1)
                    {
                        if (temp.Count >= 3)
                        {
                            if (selectedCell != null)
                            {
                                newBonuses.Add(CreateBonus(temp, BonusType.LineHorizontal));
                            }
                            toDestroy.AddRange(temp);
                            matchScore += temp.Count;
                        }
                        temp.Clear();
                        temp.Add(Cells[i, j]);
                    }
                }
            }
            for (int j = 0; j < Cells.GetLength(1); j++)
            {
                List<Cell> temp = new List<Cell>(8) { Cells[0, j] };
                for (int i = 1; i < Cells.GetLength(0); i++)
                {
                    bool stop = false;
                    if (Cells[i, j].Shape == temp[0].Shape)
                    {
                        temp.Add(Cells[i, j]);
                    }
                    else
                    {
                        stop = true;
                    }
                    if (stop || i == Cells.GetLength(0) - 1)
                    {
                        if (temp.Count >= 3)
                        {
                            if (selectedCell != null)
                            {
                                newBonuses.Add(CreateBonus(temp, BonusType.LineVertical));
                            }
                            var intersect = toDestroy.Intersect(temp);
                            List<Cell> intersectList = intersect.ToList();
                            foreach (var bonusCell in intersectList)
                            {
                                temp.Remove(bonusCell);
                                toDestroy.Remove(bonusCell);
                                bonusCell.Bonus = BonusType.Bomb;
                            }
                            toDestroy.AddRange(temp);
                            matchScore += temp.Count;
                        }
                        temp.Clear();
                        temp.Add(Cells[i, j]);
                    }
                }
            }
            newBonuses.ForEach(c => toDestroy.Remove(c));
            toDestroy.ForEach(cell => cell.Destroy());
            if (selectedCell != null && matchScore > 0)
            {
                selectedCell = null;
            }
            return matchScore;
        }

        private Cell CreateBonus(List<Cell> matchedCells, BonusType orientation)
        {
            Cell bonusCell = null;
            switch (matchedCells.Count)
            {
                case 4:
                    bonusCell = matchedCells.Find(cell => (cell == selectedCell || cell == currentCell));
                    if (bonusCell.Bonus != BonusType.None)
                    {
                        BonusDestroyers(bonusCell.Row, bonusCell.Column, bonusCell.Bonus);
                    }
                    bonusCell.Bonus = orientation;
                    break;
                case 5:
                    bonusCell = matchedCells.Find(cell => (cell == selectedCell || cell == currentCell));
                    if (bonusCell.Bonus != BonusType.None)
                    {
                        BonusDestroyers(bonusCell.Row, bonusCell.Column, bonusCell.Bonus);
                    }
                    bonusCell.Bonus = BonusType.Bomb;
                    break;
            }
            return bonusCell;
        }

        internal void BonusDestroyers(int row, int column, BonusType bonus)
        {
            switch (bonus)
            {
                case BonusType.LineVertical:
                    Destroyers.Add(new Destroyer(this, new Vector2(column * CellSize.X + Rectangle.X, (row - 0.5f) * CellSize.Y + Rectangle.Y),
                        destroyerTexture, DestroyerState.Up));
                    Destroyers.Add(new Destroyer(this, new Vector2(column * CellSize.X + Rectangle.X, (row + 0.5f) * CellSize.Y + Rectangle.Y),
                        destroyerTexture, DestroyerState.Down));
                    break;
                case BonusType.LineHorizontal:
                    Destroyers.Add(new Destroyer(this, new Vector2((column - 0.5f) * CellSize.X + Rectangle.X, row * CellSize.Y + Rectangle.Y),
                        destroyerTexture, DestroyerState.Left));
                    Destroyers.Add(new Destroyer(this, new Vector2((column + 0.5f) * CellSize.X + Rectangle.X, row * CellSize.Y + Rectangle.Y),
                        destroyerTexture, DestroyerState.Right));
                    break;
                case BonusType.Bomb:
                    Destroyers.Add(new Destroyer(this, new Vector2(column * CellSize.X + Rectangle.X, row * CellSize.Y + Rectangle.Y),
                        destroyerTexture, DestroyerState.Destroying));
                    break;
            }
        }

        internal void FallBlocks()
        {
            for (int j = 0; j < Cells.GetLength(1); j++)
            {
                for (int i = Cells.GetLength(0) - 1; i > 0; i--)
                {
                    if (Cells[i, j].Shape == Shape.Empty)
                    {
                        int k = i - 1;
                        while (k >= 0 && Cells[k, j].Shape == Shape.Empty)
                        {
                            k--;
                        }
                        if (k < 0) break;
                        Cells[k, j].Fall(Cells[i, j]);
                    }
                }
            }
        }

        internal bool Input()
        {
            Microsoft.Xna.Framework.Input.MouseState mouseState = Mouse.GetState();

            if (Rectangle.Contains(mouseState.Position))
            {
                int i = (mouseState.Position.Y - Rectangle.Y) / CellSize.Y;
                int j = (mouseState.Position.X - Rectangle.X) / CellSize.X;

                if (currentCell != null && Cells[i, j] != currentCell)
                {
                    currentCell.State = MouseState.Idle;
                }
                currentCell = Cells[i, j];

                if (mouseState.LeftButton == ButtonState.Released && currentCell.State == MouseState.Push)
                {
                    currentCell.State = MouseState.Hover;
                    if (currentCell.IsSelected)
                    {
                        currentCell.Switch();
                        selectedCell = null;
                    }
                    else if (selectedCell != null && currentCell.IsAdjacent(selectedCell))
                    {
                        return true;
                    }
                    else
                    {
                        currentCell.Switch();
                        selectedCell?.Switch();
                        selectedCell = currentCell;
                    }
                }
                else if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    currentCell.State = MouseState.Push;
                }
                else
                {
                    currentCell.State = MouseState.Hover;
                }
            }
            else
            {
                if (currentCell != null)
                {
                    currentCell.State = MouseState.Idle;
                }
            }
            return false;
        }

        internal void Swap(Boolean swap)
        {
            if (swap)
            {
                selectedCell.Swap(currentCell);
                selectedCell.Switch();
            }
            else
            {
                currentCell.Swap(selectedCell);
                selectedCell = null;
            }

        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    backTexture?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
