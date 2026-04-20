using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public int depth = 5;

    public void MakeAIMove()
    {
        Move bestMove = GetBestMove();

        if (bestMove != null)
        {
            ApplyMove(bestMove);
        }
    }

    void ApplyMove(Move move)
    {
        GameManager gm = GameManager.instance;

        if (move.capturedPiece != null)
        {
            gm.CapturePieceAt(new Vector2Int(move.ToX, move.ToY));
        }

        gm.Move(move.movedPiece, new Vector2Int(move.ToX, move.ToY));
    }

    List<GameObject> GetPieces(bool isWhite)
    {
        return isWhite
            ? GameManager.instance.currentPlayer.pieces
            : GameManager.instance.otherPlayer.pieces;
    }

    List<Move> GetAllMoves(bool isWhite)
    {
        List<Move> moves = new List<Move>();

        foreach (GameObject piece in GetPieces(isWhite))
        {
            Vector2Int from = GameManager.instance.GridForPiece(piece);
            List<Vector2Int> destinations = GameManager.instance.MovesForPiece(piece);

            foreach (Vector2Int to in destinations)
            {
                Move move = new Move();
                move.FromX = from.x;
                move.FromY = from.y;
                move.ToX = to.x;
                move.ToY = to.y;
                move.movedPiece = piece;
                move.capturedPiece = GameManager.instance.PieceAtGrid(to);

                moves.Add(move);
            }
        }

        return moves;
    }

    List<Move> GetLegalMoves(bool isWhite)
    {
        List<Move> legalMoves = new List<Move>();

        foreach (Move move in GetAllMoves(isWhite))
        {
            MakeMove(move);

            if (!IsKingInCheck(isWhite))
                legalMoves.Add(move);

            UndoMove(move);
        }

        return legalMoves;
    }

    bool IsKingInCheck(bool isWhite)
    {
        GameObject king = null;

        foreach (GameObject piece in GetPieces(isWhite))
        {
            Piece p = piece.GetComponent<Piece>();

            if (p.type == PieceType.King)
            {
                king = piece;
                break;
            }
        }

        if (king == null) return false;

        Vector2Int kingPos = GameManager.instance.GridForPiece(king);

        foreach (GameObject enemy in GetPieces(!isWhite))
        {
            List<Vector2Int> moves = GameManager.instance.MovesForPiece(enemy);

            if (moves.Contains(kingPos))
                return true;
        }

        return false;
    }

    void MakeMove(Move move)
    {
        GameManager gm = GameManager.instance;

        if (move.capturedPiece != null)
        {
            Piece captured = move.capturedPiece.GetComponent<Piece>();

            if (captured.IsWhite)
                gm.currentPlayer.pieces.Remove(move.capturedPiece);
            else
                gm.otherPlayer.pieces.Remove(move.capturedPiece);

            move.capturedPiece.SetActive(false);
        }

        gm.Move(move.movedPiece, new Vector2Int(move.ToX, move.ToY));
    }

    void UndoMove(Move move)
    {
        GameManager gm = GameManager.instance;

        gm.Move(move.movedPiece, new Vector2Int(move.FromX, move.FromY));

        if (move.capturedPiece != null)
        {
            Piece captured = move.capturedPiece.GetComponent<Piece>();

            move.capturedPiece.SetActive(true);

            if (captured.IsWhite)
                gm.currentPlayer.pieces.Add(move.capturedPiece);
            else
                gm.otherPlayer.pieces.Add(move.capturedPiece);
        }
    }

    int Minimax(int depth, int alpha, int beta, bool isWhiteTurn)
    {
        if (depth == 0)
            return Evaluate();

        List<Move> moves = GetLegalMoves(isWhiteTurn);

        if (moves.Count == 0)
        {
            if (IsKingInCheck(isWhiteTurn))
                return isWhiteTurn ? -100000 : 100000;
            else
                return 0;
        }

        if (isWhiteTurn) 
        {
            int maxEval = int.MinValue;

            foreach (Move move in moves)
            {
                MakeMove(move);
                int eval = Minimax(depth - 1, alpha, beta, false);
                UndoMove(move);

                maxEval = Mathf.Max(maxEval, eval);
                alpha = Mathf.Max(alpha, eval);

                if (beta <= alpha) break;
            }

            return maxEval;
        }
        else 
        {
            int minEval = int.MaxValue;

            foreach (Move move in moves)
            {
                MakeMove(move);
                int eval = Minimax(depth - 1, alpha, beta, true);
                UndoMove(move);

                minEval = Mathf.Min(minEval, eval);
                beta = Mathf.Min(beta, eval);

                if (beta <= alpha) break;
            }

            return minEval;
        }
    }

    int Evaluate()
    {
        int score = 0;

        foreach (GameObject piece in GameManager.instance.currentPlayer.pieces)
        {
            score += GetValue(piece.GetComponent<Piece>().type);
        }

        foreach (GameObject piece in GameManager.instance.otherPlayer.pieces)
        {
            score -= GetValue(piece.GetComponent<Piece>().type);
        }

        return score;
    }

    int GetValue(PieceType type)
    {
        switch (type)
        {
            case PieceType.Pawn: return 100;
            case PieceType.Knight: return 320;
            case PieceType.Bishop: return 330;
            case PieceType.Rook: return 500;
            case PieceType.Queen: return 900;
            case PieceType.King: return 20000;
        }
        return 0;
    }

    Move GetBestMove()
    {
        List<Move> moves = GetLegalMoves(false); 

        Move bestMove = null;
        int bestScore = int.MaxValue; 

        foreach (Move move in moves)
        {
            MakeMove(move);
            int score = Minimax(depth - 1, int.MinValue, int.MaxValue, true);
            UndoMove(move);

            if (score < bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return bestMove;
    }
}