public class Piece_Line : Piece
{
    public override int[,] PieceForm {
        get {
            return new int[1, 5] {
                { 1, 1, 1, 1, 1}
            };
        }
    }

    public override string PrefabPath { get => "Pieces_Prefab/Line"; }

}
