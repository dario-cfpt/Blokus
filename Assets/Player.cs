using Assets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string Name { get; private set; }
    public List<Piece> Pieces { get; private set; }
    public BlokusColor Color { get; private set; }

    public Player(BlokusColor color, string name = "Undefined player") {
        Name = name;
        Color = color;
        Pieces = new List<Piece>() {
            new Piece_1(),
            new Piece_2(),
            new Piece_3(),
            new Piece_Crooked_3(),
            new Piece_F(),
            new Piece_L(),
            new Piece_L_Short(),
            new Piece_Line(),
            new Piece_Line_Short(),
            new Piece_N(),
            new Piece_P(),
            new Piece_Square(),
            new Piece_T(),
            new Piece_T_Short(),
            new Piece_U(),
            new Piece_V(),
            new Piece_W(),
            new Piece_X(),
            new Piece_Y(),
            new Piece_Z(),
            new Piece_Z_Short(),
        };
    }

}