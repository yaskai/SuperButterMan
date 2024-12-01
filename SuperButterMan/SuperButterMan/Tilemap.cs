using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperButterMan {

    public class TileMap {
        Game1 game;
        Handler handler;
        Spritesheet ss;

        public char[,] mapData = new char[512, 512];
        public char[,] bgmData = new char[512, 512];
        public Entities.Tile[,] tileList = new Entities.Tile[512, 512];

        public int mapWidth;
        public int mapHeight;

        public int frameW = 24;
        public int frameH = 14;

        int frameLft, frameRgt, frameTop, frameBot = 0;

        public TileMap(Game1 game, Handler handler) {
            this.game = game;
            this.handler = handler;
            ss = game.tile_ss;
        }

        public void LoadMap(string path) {
            StreamReader file = new StreamReader($"{path}");

            int cols = Int32.Parse(file.ReadLine());
            int rows = Int32.Parse(file.ReadLine());

            mapWidth = cols;
            mapHeight = rows;

            Console.WriteLine($"{mapWidth}");
            Console.WriteLine($"{mapHeight}");

            string newMap = "";
            for(int r = 0; r < rows; r++) {
                string line = file.ReadLine();
                newMap += line;
            }

            string newBgMap = "";
            for(int r = 0; r < rows; r++) {
                string line = file.ReadLine();
                newBgMap += line;
            }

            GenerateMap(newMap);
        }

        public void GenerateMap(string sMap) {
            char[] _map = sMap.ToCharArray();
            for(int i = 0; i < sMap.Length;  i++) {
                int c = i % mapWidth;
                int r = i / mapWidth;
                Vector2 pos = new Vector2(c * (64), r * (64) - 1);

                switch(_map[i]) {
                    case '1':
                        MakeTile(i);
                        break;
                    case '@':
                        foreach(Entities.Player p in handler.players) {
                            p.position = pos;
                            game.camera.position = p.position;
                        } 
                        break; 
                }

                if(c < mapWidth && r < mapHeight) mapData[c, r] = _map[i];
            }
        }

        private void MakeTile(int i) {
            int c = i % mapWidth;
            int r = i / mapWidth;
            Vector2 pos = new Vector2(c * (64), r * (64));
            Entities.Tile tile = new Entities.Tile(pos, new Vector2(c, r), Vector2.One, ss);
            //tileList[c, r] = tile;
        }

        public void DrawTiles(SpriteBatch spritebatch) {
            for(int i = 0; i < ((mapWidth - 1) * (mapHeight - 1)); i++) {
                int c = i % mapWidth;
                int r = i / mapWidth;
                Vector2 pos = new Vector2(c * (64), r * (64));
                
                switch(mapData[c, r]) {
                    case '1':
                        //if(tileList[c, r] != null) tileList[c, r].Draw(spritebatch);
                        ss.DrawFrameBasic(spritebatch, pos, 0, 0, false);
                        break;
                }
            }
        }

        public Tuple<int, int> CoordsFromPoint(Vector2 point) {
           int c = (int)(point.X / 64);
           int r = (int)(point.Y / 64);
           return Tuple.Create(c, r); 
        }

        public bool HasCollider(Vector2 point) {
            bool result = false;

            int c = CoordsFromPoint(point).Item1;
            int r = CoordsFromPoint(point).Item2;

            if(c > 0 && c < mapWidth && r > 0 && r < mapHeight) {
                if(mapData[c, r] == '1') result = true; 
            }

            return result;
        }
    }
}