using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlokusMap : MonoBehaviour
{

    private const int NB_COL = 22;
    private const int NB_ROW = 22;
    private readonly int[,] blokus_map = new int[NB_COL, NB_ROW];

    public Grid grid;
    public Tilemap tilemap;
    public TileBase ground;
    public TileBase wall;
    public TileBase blue_bloc;
    public TileBase green_bloc;
    public TileBase red_bloc;
    public TileBase yellow_bloc;

    private const int GROUND_TILE = 0;
    private const int WALL_TILE = 1;
    private const int BLUE_TILE = 2;
    private const int GREEN_TILE = 3;
    private const int RED_TILE = 4;
    private const int YELLOW_TILE = 5;

    // Use this for initialization
    void Start() {
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
    void Update() {
        // Get coordinate on mouse click
        if (Input.GetMouseButtonDown(0)) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int coordinate = grid.WorldToCell(pos);
            Debug.Log(coordinate);

            if ((coordinate.x > 0 && coordinate.x < NB_COL)
                && (coordinate.y > 0 && coordinate.y < NB_ROW)
                && blokus_map[coordinate.x, coordinate.y] == GROUND_TILE) {

                blokus_map[coordinate.x, coordinate.y] = BLUE_TILE;
                tilemap.SetTile(coordinate, blue_bloc);
            }
        }

    }
    
    // WIP
    //private void PutBloc(Vector3Int position, int[,] matrix) {
    //    // check position
    //    if ((position.x > 0 && position.x < NB_COL)
    //        && (position.y > 0 && position.y < NB_ROW)) {

    //    }
    //}
}
