using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameFoRest
{
    class EndScreen : absScreen
    {

        private int totalScore;

        public int TotalScore
        {
            get
            {
                return totalScore;
            }
            set
            {
                totalScore = value;
                Label scoreLabel = ((Label)elements[1]);
                scoreLabel.SetText("total score: " + totalScore.ToString());
                scoreLabel.SetRelativePosition(new Point(10,
                410));
            }
        }

        public EndScreen(Game1 game) : base(game) { }

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

            Label gameOverLabel = new Label("game over", (SpriteFont)game.gameContent[0]);
            Point labelPosition = new Point(150,
                200);
            gameOverLabel.SetRelativePosition(labelPosition);
            AddElement(0, gameOverLabel);
            Label scoreLabel = new Label("fetching...", (SpriteFont)game.gameContent[0]);
            Point scorePosition = new Point(10,
                410);
            scoreLabel.SetRelativePosition(scorePosition);
            AddElement(1, scoreLabel);
            Label buttonLabel = new Label("ok", (SpriteFont)game.gameContent[0]);
            Point buttonSize = new Point(200, 50);
            Point buttonPosition = new Point(100,
               250);
            Button okButton = new Button(new Rectangle(buttonPosition, buttonSize),
                Color.AntiqueWhite, new Color(255, 255, 255), Color.Aqua, buttonLabel);
            okButton.OnClick += retry;
            AddElement(2, okButton);
        }

        private void retry(object sender, EventArgs e)
        {
            game.ChangeScreen(typeof(StartScreen));
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
