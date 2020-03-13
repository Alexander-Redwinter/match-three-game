using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MatchThree
{
    class EndScreen : Screen
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
                Point scorePosition = new Point(200 - scoreLabel.Rectangle.Width / 2, 225);
                scoreLabel.SetRelativePosition(scorePosition);
            }
        }

        private int totalTurns;

        public int TotalTurns
        {
            get
            {
                return totalTurns;
            }
            set
            {
                totalTurns = value;
                Label turnsLabel = ((Label)elements[3]);
                turnsLabel.SetText(String.Format("and it took just {0} turns!", totalTurns));
                Point turnsPosition = new Point(200 - turnsLabel.Rectangle.Width / 2, 250);
                turnsLabel.SetRelativePosition(turnsPosition);
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
                150);
            gameOverLabel.SetRelativePosition(labelPosition);
            AddElement(0, gameOverLabel);

            Label scoreLabel = new Label("fetching...", (SpriteFont)game.gameContent[0]);
            scoreLabel.SetRelativePosition(new Point(0, 0));
            AddElement(1, scoreLabel);

            Label turnsLabel = new Label("fetching...", (SpriteFont)game.gameContent[0]);
            turnsLabel.SetRelativePosition(new Point(0, 0));
            AddElement(3, turnsLabel);

            Label buttonLabel = new Label("ok", (SpriteFont)game.gameContent[0]);
            Point buttonSize = new Point(200, 40);
            Button okButton = new Button(new Rectangle(new Point(100,
               350), buttonSize),
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
