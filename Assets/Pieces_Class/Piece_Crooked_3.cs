public class Piece_Crooked_3 : Piece
{
    public override int[,] PieceForm {
        get {
            return new int[2, 2] {
                { 1, 1},
                { 1, 0},
            };
        }
    }

    public override string PrefabPath { get => "Pieces_Prefab/Crooked 3"; }

}
