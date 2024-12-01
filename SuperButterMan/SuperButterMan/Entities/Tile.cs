using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperButterMan.Entities {
    public class Tile {
        public bool active = true;

        Spritesheet ss;

        public Vector2 position;
        public Vector2 grid_coords;
        public Vector2 ss_coords;
        public Tile(Vector2 position, Vector2 grid_coords, Vector2 ss_coords, Spritesheet ss) {
            this.position = position;
            this.grid_coords = grid_coords;
            this.ss_coords = ss_coords;
            this.ss = ss;
        }
        public void Draw(SpriteBatch spriteBatch) {
            //ss.DrawFrameBasic(spriteBatch, position, (int)grid_coords.X, (int)grid_coords.Y, false);
        }

    }
}