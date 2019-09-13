using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameFoRest
{
    class GameScreen : absScreen
    {

        private int TIME_TO_PLAY = 60;
        private Label labelTimer, labelScore;
        private Field field;
        private double currentTime;
        private int totalScore;
        private enumGame state;

        public GameScreen(Game1 game) : base(game)
        {
            state = enumGame.Spawn;
            currentTime = TIME_TO_PLAY;
        }

        internal override void LoadContent()
        {
            MakeGui();
            foreach (var element in elements)
            {
                element.Value.LoadContent(game.Graphics, game.Content);
            }
            labelTimer = (Label)GetElement(1);
            labelScore = (Label)GetElement(2);
            field = (Field)GetElement(0);
        }

        private void MakeGui()
        {
            Label scoreLabel = new Label("score: ", (SpriteFont)game.gameContent[0]);
            Point scorePosition = new Point(10, 410);
            scoreLabel.SetRelativePosition(scorePosition);
            AddElement(2, scoreLabel);

            Label timerLabel = new Label("time: ", (SpriteFont)game.gameContent[0]);
            Point timerPosition = new Point(250, 410);
            timerLabel.SetRelativePosition(timerPosition);
            AddElement(1, timerLabel);

            Field grid = new Field(this);
            grid.SetRelativePosition(new Point(0,
                0));
            AddElement(0, grid);
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
            labelScore.SetText("score: " + totalScore.ToString());
            labelTimer.SetText("time left: " + ((int)currentTime).ToString());
            loop();

            foreach (var element in elements)
            {
                element.Value.Update(gameTime);
            }

            currentTime -= gameTime.ElapsedGameTime.TotalSeconds;
            if (currentTime <= 0)
            {
                gameOver();
            }
        }

        private void gameOver()
        {
            EndScreen endScreen = (EndScreen)game.ChangeScreen(typeof(EndScreen));
            endScreen.TotalScore = totalScore;
        }

        private void loop()
        {
            if (!field.busy)

            {

                int score;

                switch (state)

                {


                    case enumGame.Spawn:

                        state = field.Spawn() ? enumGame.MatchAfterSpawn : enumGame.Input;

                        break;

                    case enumGame.MatchAfterSpawn:

                        score = field.Match();

                        if (score > 0)

                        {

                            totalScore += score;

                            state = enumGame.Falling;

                        }

                        else

                        {

                            state = enumGame.Input;

                        }

                        break;

                    case enumGame.Falling:

                        field.FallBlocks();

                        state = enumGame.Spawn;

                        break;

                    case enumGame.Input:

                        if (field.Input())

                        {

                            state = enumGame.Swap;

                        }

                        break;

                    case enumGame.Swap:

                        field.Swap(true);

                        state = enumGame.Matched;

                        break;

                    case enumGame.Matched:

                        score = field.Match();

                        if (score > 0)

                        {

                            totalScore += score;

                            state = enumGame.Falling;

                        }

                        else

                        {

                            state = enumGame.NotMatched;

                        }

                        break;

                    case enumGame.NotMatched:

                        field.Swap(false);

                        state = enumGame.Input;

                        break;

                }

            }
        }
        
        internal override void Draw()
        {
            game.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            foreach (var element in elements)
            {
                element.Value.Draw(game.SpriteBatch);
            }
            game.SpriteBatch.End();
        }

        internal void AddScore(int points)
        {
            totalScore += points;
        }
    }
}
