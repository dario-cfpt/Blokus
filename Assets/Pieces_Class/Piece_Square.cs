public class Piece_Square : Piece
{
    public override int[,] PieceForm {
        get {
            return new int[2, 2] {
                { 1, 1},
                { 1, 1},
            };
        }
    }

    public override string PrefabPath { get => "Pieces_Prefab/Square"; }

}
