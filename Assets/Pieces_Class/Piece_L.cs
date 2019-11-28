using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece_L : Piece
{
    public override int[,] piece_form {
        get {
            return new int[2, 4] {
                { 1, 1, 1, 1},
                { 1, 0, 0, 0},
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
