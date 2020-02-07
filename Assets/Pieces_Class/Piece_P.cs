public class Piece_P : Piece
{
    public override int[,] PieceForm {
        get {
            return new int[2, 3] {
                { 1, 1, 1},
                { 0, 1, 1},
            };
        }
    }

    public override string PrefabPath { get => "Pieces_Prefab/P"; }

}
