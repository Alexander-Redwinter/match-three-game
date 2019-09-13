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
        public Block[,] cells;
        public List<Destroyer> destroyers;
        private Block currentCell;
        private Block selectedCell;
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
            cells = new Block[8, 8];
            CellSize = new Point(50, 50);
            SetSize(new Point(400, 400));
            destroyers = new List<Destroyer>();
            random = new Random();
            shapes = Enum.GetValues(typeof(Objects));
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

            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    Block cell = new Block(this, Objects.Empty, shapesAtlas,hoverTexture, backTexture, i, j);
                    cells[i, j] = cell;
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
            foreach (var cell in cells)
            {
                if (cell.Animation != enumAnimation.Idle)
                {
                    busy = true;
                }
                cell.Update(gameTime);
            }


            List<Point> toDestroy = new List<Point>(destroyers.Count);
            foreach (var destroyer in destroyers)
            {
                busy = true;
                if (destroyer.Update(gameTime))
                {
                    toDestroy.Add(new Point(destroyer.Position.X, destroyer.Position.Y));
                }
            }
            var detonated = destroyers.FindAll(d => d.Remove && d.Direction == destroyerState.Blow);
            addDestroyers(detonated);
            destroyers.RemoveAll(d => d.Remove);
            int score = toDestroy.Count(c => cells[c.X, c.Y].Animation == enumAnimation.Idle) / 10;
            ((GameScreen)screen).AddScore(score);
            toDestroy.ForEach(point => cells[point.X, point.Y].Destroy());
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
                        destroyers.Add(new Destroyer(this, new Vector2(j * CellSize.X + Rectangle.X, i * CellSize.Y + Rectangle.Y),
                            destroyerTexture, destroyerState.Triggered));
                    }
                }
            }
        }

        internal override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var cell in cells)
            {
                cell.Draw(spriteBatch);
            }
            foreach (var destroyer in destroyers)
            {
                destroyer.Draw(spriteBatch);
            }
        }

        internal bool Spawn()
        {
            bool b = false;

            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    if (cells[i, j].Shape == Objects.Empty)
                    {
                        b = true;
                        Objects shape = (Objects)shapes.GetValue(random.Next(shapes.Length - 1) + 1);
                        cells[i, j].Spawn(shape);
                        cells[i, j].Bonus = Bonus.None;
                    }
                }
            }
            return b;
        }

        internal int Match()
        {
            int matchScore = 0;
            List<Block> toDestroy = new List<Block>(64);
            List<Block> newBonuses = new List<Block>();

            for (int i = 0; i < cells.GetLength(0); i++)
            {
                List<Block> temp = new List<Block>(8) { cells[i, 0] };
                for (int j = 1; j < cells.GetLength(1); j++)
                {
                    bool stop = false;
                    if (cells[i, j].Shape == temp[0].Shape)
                    {
                        temp.Add(cells[i, j]);
                    }
                    else
                    {
                        stop = true;
                    }
                    if (stop || j == cells.GetLength(1) - 1)
                    {
                        if (temp.Count >= 3)
                        {
                            if (selectedCell != null)
                            {
                                newBonuses.Add(CreateBonus(temp, Bonus.LineHorizontal));
                            }
                            toDestroy.AddRange(temp);
                            matchScore += temp.Count;
                        }
                        temp.Clear();
                        temp.Add(cells[i, j]);
                    }
                }
            }
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                List<Block> temp = new List<Block>(8) { cells[0, j] };
                for (int i = 1; i < cells.GetLength(0); i++)
                {
                    bool stop = false;
                    if (cells[i, j].Shape == temp[0].Shape)
                    {
                        temp.Add(cells[i, j]);
                    }
                    else
                    {
                        stop = true;
                    }
                    if (stop || i == cells.GetLength(0) - 1)
                    {
                        if (temp.Count >= 3)
                        {
                            if (selectedCell != null)
                            {
                                newBonuses.Add(CreateBonus(temp, Bonus.LineVertical));
                            }
                            var intersect = toDestroy.Intersect(temp);
                            List<Block> intersectList = intersect.ToList();
                            foreach (var bonusCell in intersectList)
                            {
                                temp.Remove(bonusCell);
                                toDestroy.Remove(bonusCell);
                                bonusCell.Bonus = Bonus.Bomb;
                            }
                            toDestroy.AddRange(temp);
                            matchScore += temp.Count;
                        }
                        temp.Clear();
                        temp.Add(cells[i, j]);
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

        private Block CreateBonus(List<Block> matchedCells, Bonus orientation)
        {
            Block bonusCell = null;
            switch (matchedCells.Count)
            {
                case 4:
                    bonusCell = matchedCells.Find(cell => (cell == selectedCell || cell == currentCell));
                    if (bonusCell.Bonus != Bonus.None)
                    {
                        BonusDestroyers(bonusCell.Row, bonusCell.Column, bonusCell.Bonus);
                    }
                    bonusCell.Bonus = orientation;
                    break;
                case 5:
                    bonusCell = matchedCells.Find(cell => (cell == selectedCell || cell == currentCell));
                    if (bonusCell.Bonus != Bonus.None)
                    {
                        BonusDestroyers(bonusCell.Row, bonusCell.Column, bonusCell.Bonus);
                    }
                    bonusCell.Bonus = Bonus.Bomb;
                    break;
            }
            return bonusCell;
        }

        internal void BonusDestroyers(int row, int column, Bonus bonus)
        {
            switch (bonus)
            {
                case Bonus.LineVertical:
                    destroyers.Add(new Destroyer(this, new Vector2(column * CellSize.X + Rectangle.X, (row - 0.5f) * CellSize.Y + Rectangle.Y),
                        destroyerTexture, destroyerState.Up));
                    destroyers.Add(new Destroyer(this, new Vector2(column * CellSize.X + Rectangle.X, (row + 0.5f) * CellSize.Y + Rectangle.Y),
                        destroyerTexture, destroyerState.Down));
                    break;
                case Bonus.LineHorizontal:
                    destroyers.Add(new Destroyer(this, new Vector2((column - 0.5f) * CellSize.X + Rectangle.X, row * CellSize.Y + Rectangle.Y),
                        destroyerTexture, destroyerState.Left));
                    destroyers.Add(new Destroyer(this, new Vector2((column + 0.5f) * CellSize.X + Rectangle.X, row * CellSize.Y + Rectangle.Y),
                        destroyerTexture, destroyerState.Right));
                    break;
                case Bonus.Bomb:
                    destroyers.Add(new Destroyer(this, new Vector2(column * CellSize.X + Rectangle.X, row * CellSize.Y + Rectangle.Y),
                        destroyerTexture, destroyerState.Blow));
                    break;
            }
        }

        internal void FallBlocks()
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                for (int i = cells.GetLength(0) - 1; i > 0; i--)
                {
                    if (cells[i, j].Shape == Objects.Empty)
                    {
                        int k = i - 1;
                        while (k >= 0 && cells[k, j].Shape == Objects.Empty)
                        {
                            k--;
                        }
                        if (k < 0) break;
                        cells[k, j].Fall(cells[i, j]);
                    }
                }
            }
        }

        internal bool Input()
        {
            MouseState mouseState = Mouse.GetState();

            if (Rectangle.Contains(mouseState.Position))
            {
                int i = (mouseState.Position.Y - Rectangle.Y) / CellSize.Y;
                int j = (mouseState.Position.X - Rectangle.X) / CellSize.X;

                if (currentCell != null && cells[i, j] != currentCell)
                {
                    currentCell.State = enumState.Idle;
                }
                currentCell = cells[i, j];

                if (mouseState.LeftButton == ButtonState.Released && currentCell.State == enumState.Push)
                {
                    currentCell.State = enumState.Hover;
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
                    currentCell.State = enumState.Push;
                }
                else
                {
                    currentCell.State = enumState.Hover;
                }
            }
            else
            {
                if (currentCell != null)
                {
                    currentCell.State = enumState.Idle;
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
