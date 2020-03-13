using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MatchThree
{
    class GameScreen : Screen
    {

        private Label labelTimer, labelScore, labelTurns;
        private Field field;
        private double currentTime;
        private int totalScore;
        private int turnsTaken;
        private GameState state;

        public GameScreen(Game1 game) : base(game)
        {
            state = GameState.MatchAfterSpawn;
            currentTime = Parameters.PlayTime;
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
            labelTurns = (Label)GetElement(3);
            field = (Field)GetElement(0);
        }

        private void MakeGui()
        {
            Label turnsLabel = new Label("turns: ", (SpriteFont)game.gameContent[0]);
            Point turnsPosition = new Point(120, 410);
            turnsLabel.SetRelativePosition(turnsPosition);
            AddElement(3, turnsLabel);

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
            if(turnsTaken == 0)
            {
                totalScore = 0;
            }
            labelScore.SetText("score: " + totalScore.ToString());
            labelTurns.SetText("turns: " + turnsTaken.ToString());
            labelTimer.SetText("time left: " + ((int)currentTime).ToString());
            Loop();

            foreach (var element in elements)
            {
                element.Value.Update(gameTime);
            }

            currentTime -= gameTime.ElapsedGameTime.TotalSeconds;
            if (currentTime <= 0)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            EndScreen endScreen = (EndScreen)game.ChangeScreen(typeof(EndScreen));
            endScreen.TotalScore = totalScore;
            endScreen.TotalTurns = turnsTaken;
        }

        private void Loop()
        {
            if (!field.busy)
            {
                int score;
                switch (state)
                {

                    case GameState.Spawn:
                        state = field.Spawn() ? GameState.MatchAfterSpawn : GameState.Input; 
                        break;
                    case GameState.MatchAfterSpawn:
                        score = field.Match();
                        if (score > 0)
                        {
                            totalScore += score;
                            state = GameState.Falling;
                        }
                        else
                        {
                            state = GameState.Input;
                        }
                        break;

                    case GameState.Falling:

                        field.FallBlocks();

                        state = GameState.Spawn;

                        break;

                    case GameState.Input:

                        if (field.Input())

                        {
                            state = GameState.Swap;
                        }

                        break;

                    case GameState.Swap:

                        field.Swap(true);

                        state = GameState.Matched;

                        break;

                    case GameState.Matched:
                        score = field.Match();
                        if (score > 0)
                        {
                            totalScore += score;
                            state = GameState.Falling;
                        }
                        else
                        {
                            state = GameState.NotMatched;
                        }
                        turnsTaken++;
                        break;

                    case GameState.NotMatched:
                        field.Swap(false);
                        state = GameState.Input;
                        turnsTaken++;
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
