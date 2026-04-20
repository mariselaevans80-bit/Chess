using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessMove
{
    public Vector2Int from;
    public Vector2Int to;

    public ChessMove(Vector2Int from, Vector2Int to) {
        this.from = from;
        this.to   = to;
    }
}