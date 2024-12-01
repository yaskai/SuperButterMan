using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperButterMan {
    public class Handler {

        public List<Entities.Player> players;
        public Handler() {
            players = new List<Entities.Player>();
        }

        public void Update(GameTime gameTime) {
            foreach(Entities.Player p in players) p.Update(gameTime);
        }

        public void FixedUpdate() {
            foreach(Entities.Player p in players) p.FixedUpdate();
        }
        public void Draw(SpriteBatch spriteBatch) {
            foreach(Entities.Player p in players) p.Draw(spriteBatch);
        }

        private void RemoveEntities() {
            for(int i = 0; i < players.Count; i++) {
                if(!players[i].active) {
                    players.RemoveAt(i);
                    i--;
                }
            }

        }

        public void ClearScene() {
            RemoveEntities();
        } 

        public void ResetScene() {
            RemoveEntities();
        }
    } 
}