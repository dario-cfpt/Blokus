using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlokusMap : MonoBehaviour {

    private const int NB_COL = 20;
    private const int NB_ROW = 20;
    private readonly int[,] blokus_map = new int[NB_COL, NB_ROW];

    public Tilemap tilemap;
    public Tile ground;
    public Tile wall;
    public Tile blue_bloc;
    public Tile green_bloc;
    public Tile red_bloc;
    public Tile yellow_bloc;

    private const int GROUND_TILE = 0;
    private const int WALL_TILE = 1;
    private const int BLUE_TILE = 2;
    private const int GREEN_TILE = 3;
    private const int RED_TILE = 4;
    private const int YELLOW_TILE = 5;

	// Use this for initialization
	void Start () {
        TileBase tile;
        int actualTile;
        for (int x = 0; x < NB_COL; x++) {
            for (int y = 0; y < NB_ROW; y++) {
                Vector3Int p = new Vector3Int(x, y, 0);
                if (x == 0 || x == NB_COL - 1 || y == 0 || y == NB_ROW - 1) {
                    tile = wall;
                    actualTile = WALL_TILE;
                } else {
                    tile = ground;
                    actualTile = GROUND_TILE;
                }
                tilemap.SetTile(p, tile);
                blokus_map[x, y] = actualTile;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
