public class Piece_T : Piece
{
    public override int[,] PieceForm {
        get {
            return new int[3, 3] {
                { 0, 0, 1},
                { 1, 1, 1},
                { 0, 0, 1},
            };
        }
    }

    public override string PrefabPath { get => "Pieces_Prefab/T"; }

}
