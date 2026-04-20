using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MinimaxAB
{
    public const int POS_INF = 999999;
    public const int NEG_INF = -999999;

    public static int search(BoardState state, int depth, int alpha, int beta)
    {
        var legalMoves = MoveGenerator.getLegalMoves(state, state.currentTurn);

        if (legalMoves.Count == 0)
        {
            if (MoveGenerator.isInCheck(state, state.currentTurn))
            {
                if (state.currentTurn == PieceColor.White)
                    return NEG_INF;
                else
                    return POS_INF;
            }
            else
            {
                return 0;
            }
        }

        if (depth == 0)
        {
            return Evaluator.Evaluate(state);
        }

        if (state.currentTurn == PieceColor.White)
        {
            int maxEval = NEG_INF;
            foreach (var move in legalMoves)
            {
                var newState = state.cloneBoard();
                newState.applyMove(move);
                newState.switchTurn();
                int eval = search(newState, depth - 1, alpha, beta);
                maxEval = Mathf.Max(maxEval, eval);
                alpha = Mathf.Max(alpha, eval);
                if (beta <= alpha)
                {
                    break;
                }
            }
            return maxEval;
        }
        else
        {
            int minEval = POS_INF;
            foreach (var move in legalMoves)
            {
                var newState = state.cloneBoard();
                newState.applyMove(move);
                newState.switchTurn();
                int eval = search(newState, depth - 1, alpha, beta);
                minEval = Mathf.Min(minEval, eval);
                beta = Mathf.Min(beta, eval);
                if (beta <= alpha)
                {
                    break;
                }
            }
            return minEval;
        }
    }
}
