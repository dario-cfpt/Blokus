public class Piece_T_Short : Piece
{
    public override int[,] PieceForm {
        get {
            return new int[3, 2] {
                { 0, 1},
                { 1, 1},
                { 0, 1},
            };
        }
    }

    public override string PrefabPath { get => "Pieces_Prefab/Short T"; }

}
