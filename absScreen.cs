using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameFoRest
{
    abstract class absScreen
    {
        protected Game1 game;
        protected Dictionary<int, absUIObject> elements;

        protected absScreen(Game1 game)
        {
            this.game = game;
            elements = new Dictionary<int, absUIObject>();
        }

        internal virtual void LoadContent() { }
        internal virtual void UnloadContent() { }
        internal virtual void Update(GameTime gameTime) { }
        internal virtual void Draw() { }

        public void AddElement(int id, absUIObject element)
        {
            elements.Add(id, element);
        }

        public absUIObject GetElement(int id)
        {
            return elements[id];
        }


    }
}
