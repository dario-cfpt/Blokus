using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public abstract int[,] PieceForm {
        get;
    }

    public abstract string PrefabPath { get; }

}
