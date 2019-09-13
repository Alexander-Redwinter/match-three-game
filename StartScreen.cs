using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameFoRest
{
    class StartScreen : absScreen
    {


        public StartScreen(Game1 game) : base(game) { }

        internal override void LoadContent()
        {
            MakeGui();
            foreach (var element in elements)
            {
                element.Value.LoadContent(game.Graphics, game.Content);
            }
        }

        private void MakeGui()
        {
            Label buttonLabel = new Label("play", (SpriteFont)game.gameContent[0]);
            Point buttonSize = new Point(200, 50);
            Point buttonPosition = new Point(200 - buttonSize.X / 2, 200 - buttonSize.Y / 2);
            Button playButton = new Button(new Rectangle(buttonPosition, buttonSize),
                Color.AntiqueWhite, new Color(150, 150, 150), Color.Aqua, buttonLabel);
            playButton.OnClick += start;
            Label sign = new Label("GameForest test task, 2019", (SpriteFont)game.gameContent[0]);
            sign.SetRelativePosition(new Point(75,410));
            AddElement(0, playButton);
            AddElement(1, sign);
        }

        private void start(object sender, EventArgs e)
        {
            game.ChangeScreen(typeof(GameScreen));
        }

        internal override void UnloadContent()
        {
            foreach (var element in elements)
            {
                element.Value.UnloadContent();
            }
            game.Content.Unload();
        }

        internal override void Update(GameTime gameTime)
        {
            foreach (var element in elements)
            {
                element.Value.Update(gameTime);
            }
        }

        internal override void Draw()
        {
            game.SpriteBatch.Begin();
            foreach (var element in elements)
            {
                element.Value.Draw(game.SpriteBatch);
            }
            game.SpriteBatch.End();
        }
    }
}
