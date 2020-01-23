using Assets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using TMPro;

public class BlokusMap : MonoBehaviour
{
    private const int NB_COL = 22;
    private const int NB_ROW = 22;
    public readonly int[,] blokus_map = new int[NB_COL, NB_ROW];

    private const float PIECE_SCALE = 28.7334f;
    private const float PIECE_START_POS_X = -13.8f;
    private const float PIECE_START_POS_Y = 0f;
    private const float PIECE_MAX_POS_X = -7.8f;
    private const float LABEL_PLAYER_NAME_START_POS_X = 10f;
    private const float LABEL_PLAYER_NAME_START_POS_Y = 7f;

    public Grid grid;
    public GameObject panel;
    public Tilemap tilemap;
    public Tilemap PreviewTilemap;
    public TileBase ground;
    public TileBase wall;
    public TileBase default_bloc;
    public TileBase blue_bloc;
    public TileBase green_bloc;
    public TileBase red_bloc;
    public TileBase yellow_bloc;
    public TileBase preview_blue_bloc;
    public TileBase preview_green_bloc;
    public TileBase preview_red_bloc;
    public TileBase preview_yellow_bloc;

    public TextMeshProUGUI LabelPlayerBlue;
    public TextMeshProUGUI LabelPlayerGreen;
    public TextMeshProUGUI LabelPlayerRed;
    public TextMeshProUGUI LabelPlayerYellow;

    private const int GROUND_TILE = 0;
    private const int WALL_TILE = 1;
    private const int BLUE_TILE = (int)BlokusColor.BLUE;
    private const int GREEN_TILE = (int)BlokusColor.GREEN;
    private const int RED_TILE = (int)BlokusColor.RED;
    private const int YELLOW_TILE = (int)BlokusColor.YELLOW;

    private readonly Vector3Int START_POSITION_BLUE = new Vector3Int(0, 0, 0);
    private readonly Vector3Int START_POSITION_GREEN = new Vector3Int(0, NB_ROW - 1, 0);
    private readonly Vector3Int START_POSITION_RED = new Vector3Int(NB_COL - 1, NB_ROW - 1, 0);
    private readonly Vector3Int START_POSITION_YELLOW = new Vector3Int(NB_COL - 1, 0, 0);

    private int[,] selectedPieceMap = null;

    private List<GameObject> currentDisplayedPieces = new List<GameObject>();
    private List<Player> playerList;
    private List<Player> blockedPlayers = new List<Player>();
    private Player currentPlayer;
    private Piece currentPiece;
    private bool gameIsFinished = false;

    // Use this for initialization
    void Start() {
        TileBase tile;
        int actualTile;

        playerList = PlayerList.Players;

        for (int i = 0; i < playerList.Count; i++) {
            Player p = playerList[i];
            float x = LABEL_PLAYER_NAME_START_POS_X;
            float y = LABEL_PLAYER_NAME_START_POS_Y - i * 3;
            Vector3 pos = new Vector3(x, y);

            switch (p.Color) {
                case BlokusColor.BLUE:
                    LabelPlayerBlue.text = p.Name;
                    LabelPlayerBlue.enabled = true;
                    LabelPlayerBlue.transform.position = pos;
                    break;
                case BlokusColor.GREEN:
                    LabelPlayerGreen.text = p.Name;
                    LabelPlayerGreen.enabled = true;
                    LabelPlayerGreen.transform.position = pos;
                    break;
                case BlokusColor.RED:
                    LabelPlayerRed.text = p.Name;
                    LabelPlayerRed.enabled = true;
                    LabelPlayerRed.transform.position = pos;
                    break;
                case BlokusColor.YELLOW:
                    LabelPlayerYellow.text = p.Name;
                    LabelPlayerYellow.enabled = true;
                    LabelPlayerYellow.transform.position = pos;
                    break;
                default:
                    break;
            }
        }

        currentPlayer = playerList[0];

        // Create the map
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
        tilemap.SetTile(START_POSITION_BLUE, preview_blue_bloc);
        blokus_map[START_POSITION_BLUE.x, START_POSITION_BLUE.y] = BLUE_TILE;

        tilemap.SetTile(START_POSITION_GREEN, preview_green_bloc);
        blokus_map[START_POSITION_GREEN.x, START_POSITION_GREEN.y] = GREEN_TILE;

        tilemap.SetTile(START_POSITION_RED, preview_red_bloc);
        blokus_map[START_POSITION_RED.x, START_POSITION_RED.y] = RED_TILE;

        tilemap.SetTile(START_POSITION_YELLOW, preview_yellow_bloc);
        blokus_map[START_POSITION_YELLOW.x, START_POSITION_YELLOW.y] = YELLOW_TILE;

        DisplayPiecesOfPlayer(currentPlayer);
    }

    // Update is called once per frame
    void Update() {
        // Get coordinate on mouse click
        if (gameIsFinished == false && Input.GetMouseButtonDown(0)) {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 mousePos2d = new Vector2(pos.x, pos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2d, Vector2.zero);
            // Get the value of the piece selected
            if (hit.collider != null) {
                currentPiece = hit.collider.gameObject.GetComponent<Piece>();

                if (currentPiece != null) {
                    selectedPieceMap = currentPiece.PieceForm;
                    // Replace the value of the selected piece with the value of current player
                    for (int x = 0; x <= selectedPieceMap.GetUpperBound(0); x++) {
                        for (int y = 0; y <= selectedPieceMap.GetUpperBound(1); y++) {
                            if (selectedPieceMap[x, y] != 0) {
                                selectedPieceMap[x, y] = (int)currentPlayer.Color;
                            }
                        }
                    }
                }
            }

            PlaceSelectedPiece(pos);
        }

        RotateSelectedPiece();

        RefreshGroundTiles();

        DisplayPreviewPiece();
    }

    /// <summary>
    /// Try to place the selected piece to the specified position
    /// </summary>
    private void PlaceSelectedPiece(Vector3 pos) {
        if (selectedPieceMap != null) {
            Vector3Int coordinate = grid.WorldToCell(pos);
            int col = selectedPieceMap.GetLength(0);
            int row = selectedPieceMap.GetLength(1);

            // Verify the limit of the map and if the piece can be placed to the selected position
            if ((coordinate.x > 0 && coordinate.x < NB_COL)
            && (coordinate.y > 0 && coordinate.y < NB_ROW)
            && (VerifyPiecePlacement(selectedPieceMap, (Vector2Int)coordinate, currentPlayer, true))) {

                // Place the piece
                for (int x = 0; x < col; x++) {
                    for (int y = 0; y < row; y++) {
                        if (selectedPieceMap[x, y] != 0) {
                            Vector3Int v3int = new Vector3Int(coordinate.x + x, coordinate.y + y, 0);
                            blokus_map[v3int.x, v3int.y] = (int)currentPlayer.Color;
                            tilemap.SetTile(v3int, GetTileOfPlayer(currentPlayer));
                        }
                    }
                }

                // Remove the piece from the user
                currentPlayer.Pieces.RemoveAll(x => x.PrefabPath == currentPiece.PrefabPath);
                currentPiece = null;
                selectedPieceMap = null;

                VerifyGameStatus();
                CheckSpaceForAllPlayers();
            }

        }
    }

    private bool VerifyPiecePlacement(int[,] pieceForm, Vector2Int coordinate, Player player, bool displayLogs = false) {
        int col = pieceForm.GetLength(0);
        int row = pieceForm.GetLength(1);

        bool isConnected() {
            bool pieceConnected = false;

            // Verify the limit of the map
            if ((coordinate.x > 0 && coordinate.x < NB_COL)
            && (coordinate.y > 0 && coordinate.y < NB_ROW)
            // If the first cell of the piece is empty, then we can skip the groud tile verification on the map
            && (pieceForm[0, 0] == 0 || blokus_map[coordinate.x, coordinate.y] == GROUND_TILE)) {
                for (int x = 0; x < col; x++) {
                    for (int y = 0; y < row; y++) {
                        Vector2Int currentCoord = new Vector2Int(coordinate.x + x, coordinate.y + y);

                        // Verify the space
                        if (currentCoord.x >= NB_COL || currentCoord.y >= NB_ROW ||
                           (pieceForm[x, y] != 0 && blokus_map[currentCoord.x, currentCoord.y] != GROUND_TILE)) {
                            if (displayLogs) Debug.Log("No space available");
                            return false;
                        }

                        if (pieceForm[x, y] != 0) {
                            // Verify that the piece is not next to another part
                            if (blokus_map[currentCoord.x + 1, currentCoord.y] == (int)player.Color ||
                                blokus_map[currentCoord.x, currentCoord.y + 1] == (int)player.Color ||
                                blokus_map[currentCoord.x - 1, currentCoord.y] == (int)player.Color ||
                                blokus_map[currentCoord.x, currentCoord.y - 1] == (int)player.Color) {
                                if (displayLogs) Debug.Log("Can't place the piece next to another one of the same player");
                                return false;
                            }

                            // Verify that the piece is connected to another by it's diagonal
                            if (blokus_map[currentCoord.x + 1, currentCoord.y + 1] == (int)player.Color ||
                                blokus_map[currentCoord.x + 1, currentCoord.y - 1] == (int)player.Color ||
                                blokus_map[currentCoord.x - 1, currentCoord.y + 1] == (int)player.Color ||
                                blokus_map[currentCoord.x - 1, currentCoord.y - 1] == (int)player.Color) {
                                pieceConnected = true;
                            }
                        }
                    }
                }
            }

            if (displayLogs && !pieceConnected) Debug.Log("Piece not connected");

            return pieceConnected;
        }

        return isConnected();
    }

    private void DisplayFinalScore() {
        // Clear the view
        foreach (GameObject pieces in currentDisplayedPieces) {
            Destroy(pieces);
        }

        // Display the results
        Debug.Log("Game is finish!");
        foreach (Player p in playerList) {
            Debug.Log(p.Name + " has won!");
        }

        gameIsFinished = true;
    }

    private void VerifyGameStatus() {
        bool pieceRemanings = false;

        // The game is finished if there is only one player left
        if (playerList.Count == 1) {
            DisplayFinalScore();
        } else {
            // The game is also finished if all the remainings players have no more pieces.
            foreach (Player p in playerList) {
                if (p.Pieces.Count > 0)
                    pieceRemanings = true;
            }
            if (!pieceRemanings) {
                DisplayFinalScore();
            }
        }
    }

    private void SwitchPlayer() {
        if (!gameIsFinished) {
            int currentIndex = playerList.IndexOf(currentPlayer);
            int nextIndex = (currentIndex + 1 < playerList.Count) ? currentIndex + 1 : 0;

            currentPlayer = playerList[nextIndex];
            DisplayPiecesOfPlayer(currentPlayer);
        }
    }

    private TileBase GetTileOfPlayer(Player player) {
        switch (player.Color) {
            case BlokusColor.BLUE:
                return blue_bloc;
            case BlokusColor.GREEN:
                return green_bloc;
            case BlokusColor.RED:
                return red_bloc;
            case BlokusColor.YELLOW:
                return yellow_bloc;
            default:
                return null;
        }
    }

    private void DisplayPiecesOfPlayer(Player player) {
        foreach (GameObject pieces in currentDisplayedPieces) {
            Destroy(pieces);
        }

        float x = PIECE_START_POS_X;
        float y = PIECE_START_POS_Y;
        float rowMaxSizeY = 0;

        foreach (Piece p in player.Pieces) {
            // Get the components corresponding to the piece
            GameObject parent = Instantiate(Resources.Load(p.PrefabPath)) as GameObject;
            BoxCollider2D box2d = parent.GetComponent<BoxCollider2D>();
            Tilemap tm = parent.GetComponentInChildren<Tilemap>();

            tm.SwapTile(default_bloc, GetTileOfPlayer(currentPlayer));
            currentDisplayedPieces.Add(parent);

            // Place and scale the piece
            parent.transform.parent = panel.transform;
            parent.transform.position = new Vector3(x, y);
            parent.transform.localScale = new Vector3(PIECE_SCALE, PIECE_SCALE, PIECE_SCALE);

            // Calculate the position of the next piece
            x += box2d.size.x;
            rowMaxSizeY = (box2d.size.y > rowMaxSizeY) ? box2d.size.y : rowMaxSizeY;

            if (x > PIECE_MAX_POS_X) {
                x = PIECE_START_POS_X;
                y -= rowMaxSizeY;
                rowMaxSizeY = 0;
            }
        }
    }

    private void DisplayPreviewPiece() {
        if (selectedPieceMap != null) {

            // Change the bloc of the grid to show the real preview
            Vector3Int previewCoordinate = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            int col = selectedPieceMap.GetLength(0);
            int row = selectedPieceMap.GetLength(1);

            for (int x = 0; x < col; x++) {
                for (int y = 0; y < row; y++) {
                    if (selectedPieceMap[x, y] != 0) {
                        if (previewCoordinate.x + x < NB_COL && previewCoordinate.y + y < NB_ROW
                            && previewCoordinate.x >= 0 && previewCoordinate.y >= 0
                            && selectedPieceMap[x, y] != 0 && blokus_map[previewCoordinate.x + x, previewCoordinate.y + y] == GROUND_TILE) {

                            Vector3Int pos = new Vector3Int(previewCoordinate.x + x, previewCoordinate.y + y, 0);
                            tilemap.SetTile(pos, GetTileOfPlayer(currentPlayer));
                        }
                        PreviewTilemap.SetTile(new Vector3Int(23 + x, 1 + y, 0), GetTileOfPlayer(currentPlayer));
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
        PreviewTilemap.ClearAllTiles();
    }

    /// <summary>
    /// Turn or flip the selected piece according to the input of the user
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

    /// <summary>
    /// Remove the blocked players from the list, after that switch to the next player
    /// </summary>
    private void CheckSpaceForAllPlayers() {
        for (int i = playerList.Count - 1; i >= 0; i--) {
            Player p = playerList[i];
            if (p.Pieces.Count > 0 && SearchAvailableSpace(p) == false) {
                Debug.Log("Player " + p.Name + " is blocked !");
                playerList.Remove(p);
                blockedPlayers.Add(p);
                VerifyGameStatus();
            }
        }

        SwitchPlayer();
    }

    private bool SearchAvailableSpace(Player player) {
        for (int x = 0; x < NB_COL; x++) {
            for (int y = 0; y < NB_ROW; y++) {
                if (blokus_map[x, y] == (int)player.Color) {
                    if (x + 1 < NB_COL && y + 1 < NB_COL) {
                        if (HasSpaceForAnyPieceVariant(new Vector2Int(x + 1, y + 1), player)) {
                            return true;
                        }
                    }
                    if (x + 1 < NB_COL && y - 1 > 0) {
                        if (HasSpaceForAnyPieceVariant(new Vector2Int(x + 1, y - 1), player)) {
                            return true;
                        }
                    }
                    if (x - 1 > 0 && y + 1 < NB_COL) {
                        if (HasSpaceForAnyPieceVariant(new Vector2Int(x - 1, y + 1), player)) {
                            return true;
                        }
                    }
                    if (x - 1 > 0 && y - 1 > 0) {
                        if (HasSpaceForAnyPieceVariant(new Vector2Int(x - 1, y - 1), player)) {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    private bool HasSpaceForAnyPieceVariant(Vector2Int coordinate, Player player) {
        bool spaceAvailable = false;

        foreach (Piece piece in player.Pieces) {
            List<int[,]> mapsVariants = GenerateAllMapVariant(piece.PieceForm);

            foreach (int[,] pieceForm in mapsVariants) {
                int col = pieceForm.GetLength(0);
                int row = pieceForm.GetLength(1);

                spaceAvailable = VerifyPiecePlacement(pieceForm, coordinate, player);

                if (spaceAvailable) {
                    return spaceAvailable;
                } else {
                    // To verify all possible placement of the piece we have to change the coordinate according to its size
                    int maxSize = (col > row) ? col : row;
                    for (int i = 1; i <= maxSize; i++) {
                        if (VerifyPiecePlacement(pieceForm, new Vector2Int(coordinate.x + i, coordinate.y), player)
                        || VerifyPiecePlacement(pieceForm, new Vector2Int(coordinate.x - i, coordinate.y), player)
                        || VerifyPiecePlacement(pieceForm, new Vector2Int(coordinate.x, coordinate.y + i), player)
                        || VerifyPiecePlacement(pieceForm, new Vector2Int(coordinate.x, coordinate.y - i), player)
                        || VerifyPiecePlacement(pieceForm, new Vector2Int(coordinate.x + i, coordinate.y + i), player)
                        || VerifyPiecePlacement(pieceForm, new Vector2Int(coordinate.x + i, coordinate.y - i), player)
                        || VerifyPiecePlacement(pieceForm, new Vector2Int(coordinate.x - i, coordinate.y + i), player)
                        || VerifyPiecePlacement(pieceForm, new Vector2Int(coordinate.x - i, coordinate.y - i), player)) {
                            spaceAvailable = true;
                            return spaceAvailable;
                        }
                    }
                }
            }
        }

        return spaceAvailable;
    }

    private List<int[,]> GenerateAllMapVariant(int[,] originalMap) {
        const int NB_ROTATED_VARIANT = 3; // We don't count the original as a variant
        List<int[,]> mapsVariants = new List<int[,]> {
            originalMap,
        };
        int[,] reversedOriginalMap = ReversePiece(originalMap);

        if (!CheckIfArraysAreEquals(originalMap, reversedOriginalMap)) {
            mapsVariants.Add(reversedOriginalMap);
        }

        // Start at 1 because we have to rotate the piece at least one time
        for (int i = 1; i <= NB_ROTATED_VARIANT; i++) {
            int[,] mapVariant = RotatePiece(originalMap, true, i);
            int[,] reversedMapVariant = ReversePiece(mapVariant);
            bool duplicateVariant = false;
            bool duplicateReversedVariant = false;

            foreach (int[,] map in mapsVariants) {
                if (CheckIfArraysAreEquals(map, mapVariant)) {
                    duplicateVariant = true;
                    break;
                }
            }

            if (!duplicateVariant)
                mapsVariants.Add(mapVariant);

            foreach (int[,] map in mapsVariants) {
                if (CheckIfArraysAreEquals(map, reversedMapVariant)) {
                    duplicateReversedVariant = true;
                    break;
                }
            }

            if (!duplicateReversedVariant)
                mapsVariants.Add(reversedMapVariant);

        }

        return mapsVariants;
    }

    /// <summary>
    /// Compare two array to check if they're the same
    /// <para>
    /// Source : https://stackoverflow.com/a/12446807
    /// </para> 
    /// </summary>
    /// <returns>Return true if the sources are the same</returns>
    private bool CheckIfArraysAreEquals(int[,] source1, int[,] source2) {
        bool isEqual =
           source1.Rank == source2.Rank &&
           Enumerable.Range(0, source1.Rank).All(dimension => source1.GetLength(dimension) == source2.GetLength(dimension)) &&
           source1.Cast<int>().SequenceEqual(source2.Cast<int>());

        return isEqual;
    }

}
