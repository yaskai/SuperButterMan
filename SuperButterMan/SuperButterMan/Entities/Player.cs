using System;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SuperButterMan.Entities {
    public class Player {
        public bool active = true;

        KeyboardState kb;
        Game1 game;
        Handler handler;
        TileMap tilemap;

        Spritesheet ss;
        Animation run_anim;

        public Vector2 position;
        public Vector2 velocity;

        private Vector2 prevPosition;
        public Vector2 drawPosition;
        public char dir = 'r';
        private enum State {
            idle,
            run,
            jump,
            fall,
            land,
            turn,
            death
        };
        private State state = State.idle;

        private float delta;
        
        public bool onGround = true;
        private float groundTimer = 5;
        private float gravity = 0.35f;

        private float decelTimer = 1.0f;
        private bool _decel = false;
        private float decelAmnt = 0.05f;
        private float decelTotalTime = 0.0f;

        private float accelTotalTime = 0.0f;
        private float accelAmnt = 0.05f;
        private  float jumpTimer = 0;

        public Player(Game1 game, Handler handler) {
            this.game = game;
            this.handler = handler;
            tilemap = game.tileMap;

            ss = game.player_ss;
            run_anim = new Animation(5, 10, new Vector2(0, 1), new Vector2(64, 64), ss, ss.texture, game);

            state = State.idle;
        }

        public void Update(GameTime gameTime) {
            delta = game.delta;
            ManageTimers();

            kb = Keyboard.GetState();
            GamePadState gp = GamePad.GetState(PlayerIndex.One);
            if(gp.IsConnected) GpInput(gp);
            else KbInput(kb);

            switch(state) {
                case State.idle:
                    break;
                case State.run:
                    run_anim.Update(gameTime);
                    break;
                case State.jump:
                    break;
                case State.fall:
                    break;
                case State.land:
                    break;
                case State.turn:
                    break;
                case State.death:
                    break; 
            }
            
        }

        public void FixedUpdate() {
            if(velocity.X != 0) MoveX(velocity.X);
            moveY(velocity.Y);
            prevPosition = position;

            CheckGround();
            if(!onGround) {
                groundTimer -= game.delta1;
                velocity.Y += gravity * (game.delta1 / 10);
            }

            switch(state) {
                case State.idle:
                    if(velocity.X != 0 && _decel) {
                        if(velocity.X < 0) {
                            float next = velocity.X + decelAmnt;
                            if (next >= 0) { 
                                velocity.X = 0;
                            } else velocity.X += decelAmnt;
                        } else if(velocity.X > 0) {
                            float next = velocity.X - decelAmnt;
                            if (next <= 0) { 
                                velocity.X = 0;
                            } else velocity.X -= decelAmnt;
                        } 
                    }
                    break;
                case State.run:
                    _decel = false;
                    decelTimer = 5;
                    break;
                case State.jump:                    
                    _decel = false;
                    decelTimer = 5;
                    break;
                case State.fall:
                    break;
                case State.land:
                    break;
                case State.turn:
                    break;
                case State.death:
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch) {
            drawPosition = Vector2.Lerp(prevPosition, position, game.ALPHA);

            bool flip = false;
            if(dir == 'l') flip = true;

            SpriteEffects fx = SpriteEffects.None;
            if(flip) fx = SpriteEffects.FlipHorizontally;

            switch(state) {
                case State.idle:
                    ss.DrawFrameBasic(spriteBatch, drawPosition, 0, 0, flip);
                    break;
                case State.run:
                    run_anim.Draw(spriteBatch, drawPosition, Vector2.One, fx, Color.White);
                    break;
                case State.jump:
                    ss.DrawFrameBasic(spriteBatch, drawPosition, 0, 0, flip);
                    break;
                case State.fall:
                    break;
                case State.land:
                    break;
                case State.turn:
                    break;
                case State.death:
                    break; 
            }
        }
        
        private void Collision() {

        }

        private void KbInput(KeyboardState kb) {
            if(state == State.jump) {
                if(jumpTimer <= 0) EndJump(kb);
            }
            
            if(kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.D)) {
                if(kb.IsKeyDown(Keys.A)) {
                    dir = 'l';
                    velocity.X -= accelAmnt; 
                } else if (kb.IsKeyDown(Keys.D)) {
                    dir = 'r';
                    velocity.X += accelAmnt;
                }

                if(onGround) state = State.run;
            } else {
                if(onGround) state = State.idle;
            }

            if(onGround) {
                if(kb.IsKeyDown(Keys.Space)) {
                    StartJump(12f);
                }
            }
        }

        private void GpInput(GamePadState gp) {

        }

        private void MoveX(float amount) {
            int passes = 4;
            float smallAmount = amount / passes;

            Vector2 nextA = Vector2.Zero;
            Vector2 nextB = Vector2.Zero;

            float newAmount = 0.0f;

            for(int step = 0; step < passes; step++) {
                nextA = new Vector2(position.X + 64 + (smallAmount * step) + 1, position.Y + 1);
                nextB = new Vector2(position.X + 64 + (smallAmount * step) + 1, position.Y + 63);
                
                if(amount < 0) {
                    nextA.X -= 64;
                    nextB.X -= 64;
                }

                if(tilemap.HasCollider(nextA) || tilemap.HasCollider(nextB)) {
                    newAmount = 0;
                } else  {
                    newAmount += smallAmount;
                }
            }

            if(newAmount == 0) velocity.X = 0;
            position.X += newAmount;
        }

        private void moveY(float amount) {
            int passes = 4;
            float smallAmount = amount / passes;

            float newAmount = 0.0f;

            for(int step = 0; step < passes; step++) {
                Vector2 nextA = new Vector2(position.X + 2, (position.Y - 1) + (smallAmount * step));
                Vector2 nextB = new Vector2(position.X + 60, (position.Y - 1) + (smallAmount * step));
                Vector2 nextC = new Vector2(nextA.X + 30, nextA.Y);

                if(amount > 0) {
                    nextA.Y += 64;
                    nextB.Y += 64;
                    nextC.Y += 64;
                }

                if(tilemap.HasCollider(nextA) || tilemap.HasCollider(nextB) || tilemap.HasCollider(nextC)) {
                    newAmount = 0;
                    velocity.Y = 0;

                    if(amount > 0) {
                        if(position.Y + 64 > nextC.Y) {
                            position.Y -= (smallAmount * step);
                        }
                    }

                } else newAmount += smallAmount;
            }

            if(newAmount == 0) velocity.Y = 0;
            position.Y += newAmount;
        }

        private void CheckGround() {
            onGround = false;
            
            if(tilemap.HasCollider(new Vector2(position.X + 32, position.Y + 64))) {
                onGround = true;
                if((position.Y + 64) % 2 != 0) {
                    position.Y = (int)(position.Y / 64) * 64;
                }
            }
        }

        private void ManageTimers() {
            if(state != State.run) {
                accelTotalTime = 0.0f;

                if(velocity.X != 0) {
                    decelTotalTime += game.delta;
                    decelAmnt = (decelTotalTime / 2.5f) * game.delta;
                    _decel = true;
                } else {
                    decelTotalTime = 0;
                }

            } else if(state == State.run) {
                accelTotalTime += game.delta;
                
                float vel_div = (accelTotalTime / 10) * game.delta;
                accelAmnt = (vel_div) * game.delta;
                decelTotalTime = 0;
                _decel = false;
            }

            jumpTimer -= game.delta;
            if(!onGround) {
                _decel = false;
                accelAmnt *= 0.5f;
            }
            
            if(velocity.X > 5f) velocity.X = 5f;
            if(velocity.X < -5f) velocity.X = -5f;
        }

        private void StartJump(float amount) {
            onGround = false;
            velocity.Y = amount * -1;
            position.Y--;
            jumpTimer = 5;
            state = State.jump;
        }

        private void EndJump(KeyboardState kb) {
            if(kb.IsKeyUp(Keys.Space)) {
                velocity.Y *= 0.75f;
                jumpTimer = 0;
                state = State.idle;
            }
        }
    }
}