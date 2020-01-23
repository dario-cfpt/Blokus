﻿using Assets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class Blokus : MonoBehaviour
{
    private const int NB_COL = 22;
    private const int NB_ROW = 22;
    public readonly int[,] BlokusMap = new int[NB_COL, NB_ROW];

    private const float PIECE_SCALE = 28.7334f;
    private const float PIECE_START_POS_X = -13.8f;
    private const float PIECE_START_POS_Y = 0f;
    private const float PIECE_MAX_POS_X = -7.8f;
    private const float LABEL_PLAYER_NAME_START_POS_X = 10f;
    private const float LABEL_PLAYER_NAME_START_POS_Y = 7f;

    public Grid MainGrid;
    public GameObject MainPanel;
    public Tilemap MainTilemap;
    public Tilemap PreviewTilemap;
    public TileBase Ground;
    public TileBase Wall;
    public TileBase DefaultBlock;
    public TileBase BlueBlock;
    public TileBase GreenBlock;
    public TileBase RedBlock;
    public TileBase YellowBlock;
    public TileBase PreviewBlueBlock;
    public TileBase PreviewGreenBlock;
    public TileBase PreviewRedBlock;
    public TileBase PreviewYellowBlock;
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
    private List<Player> playerList = new List<Player>();
    private List<Player> blockedPlayers = new List<Player>();
    private Player currentPlayer;
    private Piece currentPiece;
    private bool gameIsFinished = false;

    private TileBase GetTileOfPlayer(Player player) {
        switch (player.Color) {
            case BlokusColor.BLUE:
                return BlueBlock;
            case BlokusColor.GREEN:
                return GreenBlock;
            case BlokusColor.RED:
                return RedBlock;
            case BlokusColor.YELLOW:
                return YellowBlock;
            default:
                return null;
        }
    }

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
                    tile = Wall;
                    actualTile = WALL_TILE;
                } else {
                    tile = Ground;
                    actualTile = GROUND_TILE;
                }
                MainTilemap.SetTile(p, tile);
                BlokusMap[x, y] = actualTile;
            }
        }

        // Indicate start position
        MainTilemap.SetTile(START_POSITION_BLUE, PreviewBlueBlock);
        BlokusMap[START_POSITION_BLUE.x, START_POSITION_BLUE.y] = BLUE_TILE;

        MainTilemap.SetTile(START_POSITION_GREEN, PreviewGreenBlock);
        BlokusMap[START_POSITION_GREEN.x, START_POSITION_GREEN.y] = GREEN_TILE;

        MainTilemap.SetTile(START_POSITION_RED, PreviewRedBlock);
        BlokusMap[START_POSITION_RED.x, START_POSITION_RED.y] = RED_TILE;

        MainTilemap.SetTile(START_POSITION_YELLOW, PreviewYellowBlock);
        BlokusMap[START_POSITION_YELLOW.x, START_POSITION_YELLOW.y] = YELLOW_TILE;

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
            Vector3Int coordinate = MainGrid.WorldToCell(pos);
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
                            BlokusMap[v3int.x, v3int.y] = (int)currentPlayer.Color;
                            MainTilemap.SetTile(v3int, GetTileOfPlayer(currentPlayer));
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
            && (pieceForm[0, 0] == 0 || BlokusMap[coordinate.x, coordinate.y] == GROUND_TILE)) {
                for (int x = 0; x < col; x++) {
                    for (int y = 0; y < row; y++) {
                        Vector2Int currentCoord = new Vector2Int(coordinate.x + x, coordinate.y + y);

                        // Verify the space
                        if (currentCoord.x >= NB_COL || currentCoord.y >= NB_ROW ||
                           (pieceForm[x, y] != 0 && BlokusMap[currentCoord.x, currentCoord.y] != GROUND_TILE)) {
                            if (displayLogs) Debug.Log("No space available");
                            return false;
                        }

                        if (pieceForm[x, y] != 0) {
                            // Verify that the piece is not next to another part
                            if (BlokusMap[currentCoord.x + 1, currentCoord.y] == (int)player.Color ||
                                BlokusMap[currentCoord.x, currentCoord.y + 1] == (int)player.Color ||
                                BlokusMap[currentCoord.x - 1, currentCoord.y] == (int)player.Color ||
                                BlokusMap[currentCoord.x, currentCoord.y - 1] == (int)player.Color) {
                                if (displayLogs) Debug.Log("Can't place the piece next to another one of the same player");
                                return false;
                            }

                            // Verify that the piece is connected to another by it's diagonal
                            if (BlokusMap[currentCoord.x + 1, currentCoord.y + 1] == (int)player.Color ||
                                BlokusMap[currentCoord.x + 1, currentCoord.y - 1] == (int)player.Color ||
                                BlokusMap[currentCoord.x - 1, currentCoord.y + 1] == (int)player.Color ||
                                BlokusMap[currentCoord.x - 1, currentCoord.y - 1] == (int)player.Color) {
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

            tm.SwapTile(DefaultBlock, GetTileOfPlayer(currentPlayer));
            currentDisplayedPieces.Add(parent);

            // Place and scale the piece
            parent.transform.parent = MainPanel.transform;
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
            Vector3Int previewCoordinate = MainGrid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            int col = selectedPieceMap.GetLength(0);
            int row = selectedPieceMap.GetLength(1);

            for (int x = 0; x < col; x++) {
                for (int y = 0; y < row; y++) {
                    if (selectedPieceMap[x, y] != 0) {
                        if (previewCoordinate.x + x < NB_COL && previewCoordinate.y + y < NB_ROW
                            && previewCoordinate.x >= 0 && previewCoordinate.y >= 0
                            && selectedPieceMap[x, y] != 0 && BlokusMap[previewCoordinate.x + x, previewCoordinate.y + y] == GROUND_TILE) {

                            Vector3Int pos = new Vector3Int(previewCoordinate.x + x, previewCoordinate.y + y, 0);
                            MainTilemap.SetTile(pos, GetTileOfPlayer(currentPlayer));
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
        int col = BlokusMap.GetLength(0);
        int row = BlokusMap.GetLength(1);

        for (int x = 0; x < col; x++) {
            for (int y = 0; y < row; y++) {
                Vector3Int pos = new Vector3Int(x, y, 0);

                if (BlokusMap[x, y] == GROUND_TILE) {
                    MainTilemap.SetTile(pos, Ground);
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
                selectedPieceMap = MatriceManager.RotateMatrice(selectedPieceMap, true);
            } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                selectedPieceMap = MatriceManager.RotateMatrice(selectedPieceMap, false);
            } else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) {
                selectedPieceMap = MatriceManager.RotateMatrice(selectedPieceMap, true, 2);
            }
            // Reverse
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) {
                selectedPieceMap = MatriceManager.ReverseMatrice(selectedPieceMap);
                selectedPieceMap = MatriceManager.RotateMatrice(selectedPieceMap, true, 2);
            } else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) {
                selectedPieceMap = MatriceManager.ReverseMatrice(selectedPieceMap);
            }
        }
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
                // Find a block corresponding to the player, then verify space from diagonals
                if (BlokusMap[x, y] == (int)player.Color) {
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
            List<int[,]> mapsVariants = MatriceManager.GeneratesAllMatriceVariants(piece.PieceForm);

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
}