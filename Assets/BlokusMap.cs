using UnityEngine;
using UnityEngine.Tilemaps;

public class BlokusMap : MonoBehaviour
{
    private const int NB_COL = 22;
    private const int NB_ROW = 22;
    public readonly int[,] blokus_map = new int[NB_COL, NB_ROW];

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

    private int[,] selectedPieceMap = null;

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

            Vector2 mousePos2d = new Vector2(pos.x, pos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2d, Vector2.zero);

            // Get the value of the piece selected
            if (hit.collider != null) {
                Debug.Log(hit.collider.gameObject.name);
                Piece p = hit.collider.gameObject.GetComponent<Piece>();

                if (p != null) {
                    selectedPieceMap = p.piece_form;
                }
            }

            // Try to place the piece selected
            if (selectedPieceMap != null) {
                Vector3Int coordinate = grid.WorldToCell(pos);
                Debug.Log(coordinate);

                int col = selectedPieceMap.GetLength(0);
                int row = selectedPieceMap.GetLength(1);

                // Verify the limit of the map
                if ((coordinate.x > 0 && coordinate.x < NB_COL)
                    && (coordinate.y > 0 && coordinate.y < NB_ROW)
                    && blokus_map[coordinate.x, coordinate.y] == GROUND_TILE) {

                    // Verify if there is space for the piece
                    bool spaceAvailable = true;
                    void VerifySpace() {
                        for (int x = 0; x < col; x++) {
                            for (int y = 0; y < row; y++) {
                                if (coordinate.x + x >= NB_COL || coordinate.y + y >= NB_ROW ||
                                   (selectedPieceMap[x, y] == 1 && blokus_map[coordinate.x + x, coordinate.y + y] != GROUND_TILE)) {
                                    Debug.Log("No space available");
                                    spaceAvailable = false;
                                    return; // exit the nested loop
                                }
                            }
                        }
                    }
                    VerifySpace();

                    if (spaceAvailable) {
                        // Place the piece
                        for (int x = 0; x < col; x++) {
                            for (int y = 0; y < row; y++) {
                                if (selectedPieceMap[x, y] == 1) {
                                    Vector3Int v3int = new Vector3Int(coordinate.x + x, coordinate.y + y, 0);
                                    blokus_map[v3int.x, v3int.y] = BLUE_TILE;
                                    tilemap.SetTile(v3int, green_bloc);
                                }
                            }
                        }
                    }
                }
            }

        }

        RotateSelectedPiece();
    }

    /// <summary>
    /// Rotate the selected piece when the user press the right or left arrow key
    /// </summary>
    private void RotateSelectedPiece() {
        if (selectedPieceMap != null) {
            if (Input.GetKeyDown(KeyCode.RightArrow)) {
                selectedPieceMap = RotatePiece(selectedPieceMap, true);
            } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                selectedPieceMap = RotatePiece(selectedPieceMap, false);
            }
        }
    }

    private int[,] RotatePiece(int[,] src, bool rotateClockWise = true) {
        int nbCol = src.GetUpperBound(0) + 1; // + 1 to get the size (and not the index) of the dimension
        int nbRow = src.GetUpperBound(1) + 1;

        // Inverse the col and row if the array don't have the same size
        // This avoid an out of bound exception
        int[,] dst = (nbCol == nbRow) ? new int[nbCol, nbRow]
                                      : new int[nbRow, nbCol];

        for (int col = 0; col < nbCol; col++) {
            for (int row = 0; row < nbRow; row++) {
                int dstRow;
                int dstCol;

                if (rotateClockWise) {
                    dstRow = nbCol - (col + 1);
                    dstCol = row;
                } else {
                    dstRow = col;
                    dstCol = nbRow - (row + 1);
                }

                dst[dstCol, dstRow] = src[col, row];
            }
        }

        return dst;
    }

}
