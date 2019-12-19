public class Piece_3 : Piece
{
    public override int[,] PieceForm {
        get {
            return new int[1, 3] {
                {1, 1, 1},
            };
        }
    }

    public override string PrefabPath { get => "Pieces_Prefab/3"; }

}
