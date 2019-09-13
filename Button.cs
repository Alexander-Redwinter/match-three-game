using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameFoRest
{
    class Button : absUIObject, IDisposable
    {
        private Color backColor;
        private Color backColorHover;
        private Color backColorPressed;
        private enumState state;
        private Texture2D backTexture;
        private Label label;

        public event EventHandler OnClick;

        public Button(Rectangle position, Color backColor, Color backColorHover, Color backColorPressed, Label label)
        {
            Rectangle = position;
            this.backColor = backColor;
            this.backColorHover = backColorHover;
            this.backColorPressed = backColorPressed;
            if (label != null)
            {
                this.label = label;
                Point textPosition = CalculateTextPosition();
                this.label.Parent = this;
                this.label.SetRelativePosition(textPosition);
            }
            state = enumState.Idle;
        }

        private Point CalculateTextPosition()
        {
            int x = (Rectangle.Width <= label.Rectangle.Width) ? 0 : (Rectangle.Width - label.Rectangle.Width) / 2;
            int y = (Rectangle.Height <= label.Rectangle.Height) ? 0 : (Rectangle.Height - label.Rectangle.Height) / 2;
            return new Point(x, y);
        }

        internal override void LoadContent(GraphicsDeviceManager graphics, ContentManager content)
        {
            backTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            backTexture.SetData(new[] { Color.White });
        }

        internal override void UnloadContent()
        {
            backTexture?.Dispose();
        }

        internal override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            if (Rectangle.Contains(mouseState.Position))
            {
                if (state == enumState.Push && mouseState.LeftButton == ButtonState.Released)
                {
                    state = enumState.Hover;
                    OnClick?.Invoke(this, null);
                }
                else
                {
                    state = mouseState.LeftButton == ButtonState.Pressed ? enumState.Push : enumState.Hover;
                }
            }
            else
            {
                state = enumState.Idle;
            }
        }

        internal override void Draw(SpriteBatch spriteBatch)
        {
            Color drawButtonColor = backColor;
            switch (state)
            {
                case enumState.Idle:
                    drawButtonColor = backColor;
                    break;
                case enumState.Hover:
                    drawButtonColor = backColorHover;
                    break;
                case enumState.Push:
                    drawButtonColor = backColorPressed;
                    break;
            }
            spriteBatch.Draw(backTexture, Rectangle, drawButtonColor);
            label?.Draw(spriteBatch);
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
