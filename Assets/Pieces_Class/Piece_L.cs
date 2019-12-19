public class Piece_L : Piece
{
    public override int[,] PieceForm {
        get {
            return new int[2, 4] {
                { 1, 1, 1, 1},
                { 1, 0, 0, 0},
            };
        }
    }

    public override string PrefabPath { get => "Pieces_Prefab/L"; }

}
