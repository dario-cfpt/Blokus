public class Piece_Line_Short : Piece
{
    public override int[,] PieceForm {
        get {
            return new int[1, 4] {
                { 1, 1, 1, 1}
            };
        }
    }

    public override string PrefabPath { get => "Pieces_Prefab/Short Line"; }

}
