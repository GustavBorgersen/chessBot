using ChessChallenge.API;
using System;
using System.Collections.Generic;
using System.Linq;

public class MyBot : IChessBot
{
    Random rand = new Random();
    public Move Think(Board board, Timer timer)
    {
        //Console.WriteLine("think");

        //int currentScore = Evaluate(board);
        //Console.WriteLine(currentScore);

        DateTime startTime = DateTime.Now;
        Tuple<Move, int> bestMove = alphaBeta(board, 5, -int.MaxValue, int.MaxValue); //NegaMax(board, 4);
        //Console.WriteLine("Found move: " + bestMove.Item1.ToString());
        //Console.WriteLine("Time taken: " + (DateTime.Now - startTime).TotalMilliseconds);
        return bestMove.Item1;
    }

    private Tuple<Move, int> alphaBeta(Board board, int depth, int a, int b)
    {
        //Console.WriteLine("alphaBeta");
        Move bestMove = new Move();
        List<Move> moves = GetOrderedMoves(board);
        if (depth == 0 || moves.Count == 0 || board.IsDraw())
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
                return new Tuple<Move, int>(bestMove, b);
            }
            if (value > a)
            {
                a = value;
                bestMove = move;
            }
        }

        return new Tuple<Move, int>(bestMove, a);
    }

    private List<Move> GetOrderedMoves(Board board)
    {
        List<Move> moves = board.GetLegalMoves().ToList();
        moves = moves.OrderByDescending(m => m.IsCapture).
                      ThenByDescending(m => m.IsPromotion).
                      ThenBy(m => m.MovePieceType).
                      ToList();

        

        return moves;
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
