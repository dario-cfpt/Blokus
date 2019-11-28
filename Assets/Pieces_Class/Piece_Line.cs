using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece_Line : Piece
{
    public override int[,] piece_form {
        get {
            return new int[1, 5] {
                { 1, 1, 1, 1, 1}
            };
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
