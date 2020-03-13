using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MatchThree
{
    abstract class Screen
    {
        protected Game1 game;
        protected Dictionary<int, UIObject> elements;

        protected Screen(Game1 game)
        {
            this.game = game;
            elements = new Dictionary<int, UIObject>();
        }

        internal virtual void LoadContent() { }
        internal virtual void UnloadContent() { }
        internal virtual void Update(GameTime gameTime) { }
        internal virtual void Draw() { }

        public void AddElement(int id, UIObject element)
        {
            elements.Add(id, element);
        }

        public UIObject GetElement(int id)
        {
            return elements[id];
        }


    }
}
