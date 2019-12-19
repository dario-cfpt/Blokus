public class Piece_1 : Piece
{
    public override int[,] PieceForm {
        get {
            return new int[1, 1] {
                {1},
            };
        }
    }

    public override string PrefabPath { get => "Pieces_Prefab/1"; }

}
