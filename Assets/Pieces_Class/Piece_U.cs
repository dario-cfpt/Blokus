public class Piece_U : Piece
{
    public override int[,] PieceForm {
        get {
            return new int[3, 2] {
                { 1, 1},
                { 1, 0},
                { 1, 1},
            };
        }
    }

    public override string PrefabPath { get => "Pieces_Prefab/U"; }

}
