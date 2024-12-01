using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SuperButterMan;

public class Game1 : Game {
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public int vWidth = 640 * 2;
    public int vHeight = 360 * 2;
    public int rWidth;
    public int rHeight;
    public Vector2 scale = Vector2.One;

    public float delta = 0;
    public float delta1 = (int)(1000 / (float)100);
    public double fps = 0;
    private double displayFps = 0;
    private float fpsTimer = 0;

    private float previousT = 0;
    private float accumulator = 0.0f;
    private float maxFrameTime = 128;
    public float ALPHA = 0.0f; 

    public enum State {
        Title,
        Main,
        GameOver
    }
    public State state = State.Title;

    Handler handler;
    public  Camera camera;
    public TileMap tileMap;

    public Spritesheet player_ss;
    public Spritesheet tile_ss;

    public Game1() {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize() {
        rWidth = 1920;
        rHeight = 1080;
        scale = new Vector2(rWidth / vWidth, rHeight / vHeight);
        
        this.IsFixedTimeStep = false;
        _graphics.SynchronizeWithVerticalRetrace = false;

        _graphics.IsFullScreen = true;
        _graphics.PreferredBackBufferWidth = rWidth;
        _graphics.PreferredBackBufferHeight = rHeight;
        Window.IsBorderless = true; 

        _graphics.ApplyChanges();

        base.Initialize();

        handler = new Handler();
        camera = new Camera(this, handler);
        tileMap = new TileMap(this, handler);
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Texture2D player_img = Content.Load<Texture2D>("butterman-spritesheet-64x64");
        player_ss = new Spritesheet(GraphicsDevice, player_img, new Vector2(64, 64));
        player_ss.LoadContent(Content);

        Texture2D tile_img = Content.Load<Texture2D>("tileset00_64x64");
        tile_ss = new Spritesheet(GraphicsDevice, tile_img, new Vector2(64, 64));
        tile_ss.LoadContent(Content);
    }

    protected override void Update(GameTime gameTime) {
        delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.1f;
        if(previousT == 0) {
            previousT = (float)gameTime.TotalGameTime.TotalMilliseconds;
        }

        float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
        float frameTime = now - previousT;
        if(frameTime > maxFrameTime) {
            frameTime = maxFrameTime;
        }

        previousT = now;
        accumulator += frameTime;
        while(accumulator >= delta1) {
            FixedUpdate(gameTime);
            accumulator -= delta1;
        }

        ALPHA = (accumulator / delta1);

        KeyboardState kb = Keyboard.GetState();
        if(kb.IsKeyDown(Keys.Escape)) Exit();

        switch(state) {
            case State.Title:
                if(kb.IsKeyDown(Keys.Enter)) StartGame();
                break;
            case State.Main:
                break;
            case State.GameOver:
                break; 
        }

        handler.Update(gameTime);
        //camera.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);

        camera.Update(gameTime);
        _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.transform);
        
        switch(state) {
            case State.Title:
                break;
            case State.Main:
                tileMap.DrawTiles(_spriteBatch);
                handler.Draw(_spriteBatch);
                break;
            case State.GameOver:
                break;        
        }

        _spriteBatch.End();


        base.Draw(gameTime);
    }

    public void FixedUpdate(GameTime gameTime) {
        handler.FixedUpdate();
        camera.Move();
    }

    private void StartGame() {
        Entities.Player p = new Entities.Player(this, handler);
        handler.players.Add(p);

        tileMap.LoadMap("maps/testLevel00.txt");

        state = State.Main; 
    }
}
