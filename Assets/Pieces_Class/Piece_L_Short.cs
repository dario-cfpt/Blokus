public class Piece_L_Short : Piece
{
    public override int[,] PieceForm {
        get {
            return new int[2, 3] {
                { 1, 1, 1},
                { 1, 0, 0},
            };
        }
    }

    public override string PrefabPath { get => "Pieces_Prefab/Short L"; }

}
