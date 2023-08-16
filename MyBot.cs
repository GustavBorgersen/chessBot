using ChessChallenge.API;
using System;
using System.IO;

public class MyBot : IChessBot
{

    public Move Think(Board board, Timer timer)
    {
        Console.WriteLine("think");

        int currentScore = Evaluate(board);
        Console.WriteLine(currentScore);

        Tuple<Move, int> bestMove = alphaBeta(board, 6, -int.MaxValue, int.MaxValue); //NegaMax(board, 4);
        Console.WriteLine("Best score: " + bestMove.Item2);
        return bestMove.Item1;
    }

    Tuple<Move, int> NegaMax(Board board, int depth)
    {
        int best = -int.MaxValue;
        Move bestMove = new Move();
        Move[] moves = board.GetLegalMoves();
        if (depth == 0 || moves.Length == 0)
        {
            Tuple<Move, int> ret = new Tuple<Move, int>(bestMove, Evaluate(board));
            return ret;
        }

        foreach (Move move in moves)
        {

            board.MakeMove(move);

            int val = -NegaMax(board, depth - 1).Item2; // Note the minus sign here.

            board.UndoMove(move);

            if (val > best)
            {
                best = val;
                bestMove = move;
            } 
        }

        return new Tuple<Move, int>(bestMove, best);

    }

    private Tuple<Move, int> alphaBeta(Board board, int depth, int a, int b)
    {
        //Console.WriteLine("alphaBeta");
        Move bestMove = new Move();
        Move[] moves = board.GetLegalMoves();
        if (depth == 0 || moves.Length == 0)
        {
            Tuple<Move, int> ret = new Tuple<Move, int>(bestMove, Evaluate(board));
            return ret;
        }


        foreach (Move move in moves)
        {
            board.MakeMove(move);
            int value = -alphaBeta(board, depth - 1, -b, -a).Item2;
            board.UndoMove(move);

            if (value >= b)
            {
                break;
            }
            if (value > a)
            {
                a = value;
                bestMove = move;
            }

            
        }

        return new Tuple<Move, int>(bestMove, a);

    }

    private int Evaluate(Board board)
    {
        PieceList[] pieceLists = board.GetAllPieceLists();

        int score = (pieceLists[0].Count - pieceLists[6].Count) * 1 +
                    (pieceLists[1].Count - pieceLists[7].Count) * 3 +
                    (pieceLists[2].Count - pieceLists[8].Count) * 3 +
                    (pieceLists[3].Count - pieceLists[9].Count) * 5 +
                    (pieceLists[4].Count - pieceLists[10].Count) * 9 +
                    (pieceLists[5].Count - pieceLists[11].Count) * 100000;

        if (board.IsWhiteToMove == false) 
        {
            score = -score;
        }

        //Console.WriteLine("Score:" + score);
        return score;
    }
         
}
