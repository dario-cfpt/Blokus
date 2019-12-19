public class Piece_V : Piece
{
    public override int[,] PieceForm {
        get {
            return new int[3, 3] {
                { 1, 1, 1},
                { 1, 0, 0},
                { 1, 0, 0},
            };
        }
    }

    public override string PrefabPath { get => "Pieces_Prefab/V"; }

}
