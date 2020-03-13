using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MatchThree
{
    class ShapesAtlas : TextureAtlas
    {
        public ShapesAtlas(Texture2D texture) : base(texture, 4, 5) { }

        public void Draw(SpriteBatch spriteBatch, Shape shape, BonusType bonus, Vector2 location, float opacity)
        {
            int column = (int)shape - 1;
            int row = (int)bonus;
            if (column != -1)
            {
                base.Draw(spriteBatch, location, row, column, opacity);
            }
        }
    }
}
