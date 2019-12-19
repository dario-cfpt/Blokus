public class Piece_2 : Piece
{
    public override int[,] PieceForm {
        get {
            return new int[1, 2] {
                {1, 1},
            };
        }
    }

    public override string PrefabPath { get => "Pieces_Prefab/2"; }

}
