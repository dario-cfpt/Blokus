public class Piece_Z_Short : Piece
{
    public override int[,] PieceForm {
        get {
            return new int[3, 2] {
                { 0, 1},
                { 1, 1},
                { 1, 0},
            };
        }
    }

    public override string PrefabPath { get => "Pieces_Prefab/Short Z"; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
