using UnityEngine;
using UnityEngine.Tilemaps;

public class BlokusMap : MonoBehaviour
{
    private const int NB_COL = 22;
    private const int NB_ROW = 22;
    public readonly int[,] blokus_map = new int[NB_COL, NB_ROW];

    public Grid grid;
    public GameObject panel;
    public Tilemap tilemap;
    public TileBase ground;
    public TileBase wall;
    public TileBase blue_bloc;
    public TileBase green_bloc;
    public TileBase red_bloc;
    public TileBase yellow_bloc;
    public TileBase preview_blue_bloc;
    public TileBase preview_green_bloc;
    public TileBase preview_red_bloc;
    public TileBase preview_yellow_bloc;

    private const int GROUND_TILE = 0;
    private const int WALL_TILE = 1;
    private const int BLUE_TILE = 2;
    private const int GREEN_TILE = 3;
    private const int RED_TILE = 4;
    private const int YELLOW_TILE = 5;

    private int[,] selectedPieceMap = null;
    private GameObject previewPiece;

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
        // Indicate start position
        tilemap.SetTile(new Vector3Int(0, 0, 0), preview_blue_bloc);
        blokus_map[0, 0] = BLUE_TILE;
        tilemap.SetTile(new Vector3Int(0, NB_ROW - 1, 0), preview_green_bloc);
        blokus_map[0, NB_ROW - 1] = GREEN_TILE;
        tilemap.SetTile(new Vector3Int(NB_COL - 1, NB_ROW - 1, 0), preview_red_bloc);
        blokus_map[NB_COL - 1, NB_ROW - 1] = RED_TILE;
        tilemap.SetTile(new Vector3Int(NB_COL - 1, 0, 0), preview_yellow_bloc);
        blokus_map[NB_COL - 1, 0] = YELLOW_TILE;

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

                if (previewPiece != null) {
                    // Destroy the previous piece to avoid multiple clone
                    Destroy(previewPiece);
                }
                previewPiece = Instantiate(hit.collider.gameObject, panel.transform);

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
                    bool VerifySpace() {
                        for (int x = 0; x < col; x++) {
                            for (int y = 0; y < row; y++) {
                                if (coordinate.x + x >= NB_COL || coordinate.y + y >= NB_ROW ||
                                   (selectedPieceMap[x, y] == 1 && blokus_map[coordinate.x + x, coordinate.y + y] != GROUND_TILE)) {
                                    Debug.Log("No space available");
                                    return false;
                                }
                            }
                        }
                        return true;
                    }

                    if (VerifySpace()) {
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
                        selectedPieceMap = null;
                        Destroy(previewPiece);
                    }
                }
            }

        }

        RotateSelectedPiece();

        RefreshGroundTiles();

        DisplayPreviewPiece();
    }

    private void DisplayPreviewPiece() {
        if (previewPiece != null && selectedPieceMap != null) {
            // Show visual piece preview [WIP]
            // Vector2 vec2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // previewPiece.transform.position = new Vector3(vec2.x - 6.1f, vec2.y - 7.8f, 0);

            // Change the bloc of the grid to show the real preview
            Vector3Int previewCoordinate = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            int col = selectedPieceMap.GetLength(0);
            int row = selectedPieceMap.GetLength(1);

            for (int x = 0; x < col; x++) {
                for (int y = 0; y < row; y++) {
                    if (selectedPieceMap[x, y] == 1) {
                        if (previewCoordinate.x + x < NB_COL && previewCoordinate.y + y < NB_ROW
                            && previewCoordinate.x >= 0 && previewCoordinate.y >= 0
                            && selectedPieceMap[x, y] == 1 && blokus_map[previewCoordinate.x + x, previewCoordinate.y + y] == GROUND_TILE) {

                            Vector3Int pos = new Vector3Int(previewCoordinate.x + x, previewCoordinate.y + y, 0);
                            tilemap.SetTile(pos, preview_green_bloc);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Re-set the ground tiles in order to delete the "previews tiles"
    /// </summary>
    private void RefreshGroundTiles() {
        int col = blokus_map.GetLength(0);
        int row = blokus_map.GetLength(1);

        for (int x = 0; x < col; x++) {
            for (int y = 0; y < row; y++) {
                Vector3Int pos = new Vector3Int(x, y, 0);

                if (blokus_map[x, y] == GROUND_TILE) {
                    tilemap.SetTile(pos, ground);
                }
            }
        }
    }

    /// <summary>
    /// Rotate the selected piece when the user press the right or left arrow key
    /// </summary>
    private void RotateSelectedPiece() {
        // TODO: create options to configure shortcuts
        if (selectedPieceMap != null) {
            // Rotate
            if (Input.GetKeyDown(KeyCode.RightArrow)) {
                selectedPieceMap = RotatePiece(selectedPieceMap, true);
            } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                selectedPieceMap = RotatePiece(selectedPieceMap, false);
            } else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) {
                selectedPieceMap = RotatePiece(selectedPieceMap, true, 2);
            }
            // Reverse
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) {
                selectedPieceMap = ReversePiece(selectedPieceMap);
                selectedPieceMap = RotatePiece(selectedPieceMap, true, 2);
            } else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) {
                selectedPieceMap = ReversePiece(selectedPieceMap);
            }
        }
    }

    private int[,] RotatePiece(int[,] src, bool rotateClockWise = true, int nbRotation = 1) {
        int nbCol = src.GetUpperBound(0) + 1; // + 1 to get the size (and not the index) of the dimension
        int nbRow = src.GetUpperBound(1) + 1;

        // Inverse the col and row if the array don't have the same size
        // This avoid an out of bound exception
        int[,] dst = (nbCol == nbRow) ? new int[nbCol, nbRow]
                                      : new int[nbRow, nbCol];

        for (int col = 0; col < nbCol; col++) {
            for (int row = 0; row < nbRow; row++) {
                int dstCol, dstRow;

                if (rotateClockWise) {
                    dstCol = row;
                    dstRow = nbCol - (col + 1);
                } else {
                    dstCol = nbRow - (row + 1);
                    dstRow = col;
                }

                dst[dstCol, dstRow] = src[col, row];
            }
        }

        return (nbRotation > 1) ? RotatePiece(dst, rotateClockWise, nbRotation - 1) : dst;
    }

    private int[,] ReversePiece(int[,] src) {
        int nbCol = src.GetUpperBound(0) + 1; // + 1 to get the size (and not the index) of the dimension
        int nbRow = src.GetUpperBound(1) + 1;
        int[,] dst = new int[nbCol, nbRow];

        for (int col = 0; col < nbCol; col++) {
            for (int row = 0; row < nbRow; row++) {
                int dstRow, dstCol;

                dstRow = (nbRow - 1) - row;
                dstCol = col;

                dst[dstCol, dstRow] = src[col, row];
            }
        }

        return dst;
    }
}
