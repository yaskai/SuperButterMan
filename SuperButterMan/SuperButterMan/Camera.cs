using Microsoft.Xna.Framework;
using System;

namespace SuperButterMan {
    public class Camera {
        Game1 game;
        Handler handler;

        public Matrix transform { get; private set; }
        public Matrix inverseTransform { get; private set; }
        public Matrix scale { get; private set; }

        public Vector2 position;
        private Vector2 prevPositon;
        public Vector2 velocity;

        private Vector2 target = new Vector2(0, 0);
        private Vector2 lastPos = new Vector2(0, 0);

        private float delta;
        public Camera(Game1 game, Handler handler) {
            this.game = game;
            this.handler = handler;
        }

        public void Update(GameTime gameTime) {
            delta = game.delta;
            var scale = Matrix.CreateScale(game.scale.X, game.scale.Y, 0);
            var inverseScale = Matrix.Invert(scale);

            Vector2 lerpPos = Vector2.LerpPrecise(prevPositon, position, game.ALPHA);

            var offset = Matrix.CreateTranslation(game.vWidth / 2, game.vHeight / 2, 0);
            var pos = transform = Matrix.CreateTranslation(
            -lerpPos.X,
            -lerpPos.Y,
            0);

            transform = pos * offset * scale;
            inverseTransform = Matrix.Invert(transform);
            this.scale = scale;
        }

        public void Move() {
            prevPositon = position;

            foreach(Entities.Player p in handler.players) {
                position.X = p.drawPosition.X + 16;
                position.Y = p.drawPosition.Y + 16;
            }
        }
    }
}