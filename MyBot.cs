using ChessChallenge.API;
using System;
using System.Collections.Generic;
using System.Linq;

public class MyBot : IChessBot
{
    Move startMove;
    bool prioStartMove;
    DateTime timeOut;

    public Move Think(Board board, Timer timer)
    {
        //Console.WriteLine("think");

        //int currentScore = Evaluate(board);
        //Console.WriteLine(currentScore);
        //TimeSpan timeout = new TimeSpan(0, 0, 0, 0, 500);
        int mate = 500000;
        Tuple<Move, int> bestMove = alphaBeta(board, 1, -int.MaxValue, int.MaxValue, mate);
        Move returnMove = bestMove.Item1;

        int thinkTime = timer.MillisecondsRemaining / 60;

        for (int i = 2; i < 20; i++)
        {
            //Console.WriteLine("Depth: " + i);
            startMove = bestMove.Item1;
            prioStartMove = true;
            timeOut = DateTime.Now + new TimeSpan(0,0,0,0, thinkTime);
            bestMove = alphaBeta(board, i, -int.MaxValue, int.MaxValue, mate);
            if (DateTime.Now > timeOut)
            {
                Console.WriteLine("Break at depth: " + i);
                break;
            }
            returnMove = bestMove.Item1;

        }
        
        //Console.WriteLine("Found move: " + bestMove.Item1.ToString());
        //Console.WriteLine("Time taken: " + (DateTime.Now - startTime).TotalMilliseconds);
        return returnMove;
    }

    private Tuple<Move, int> alphaBeta(Board board, int depth, int a, int b, int mate)
    {
        //Console.WriteLine("alphaBeta");
        Move bestMove = new Move();
        List<Move> moves = GetOrderedMoves(board);
        if (depth == 0)
        {
            Tuple<Move, int> ret = new Tuple<Move, int>(bestMove, Evaluate(board));
            return ret;
        }

        if (DateTime.Now > timeOut)
        {
            //Console.WriteLine("Timeout at depth " + depth);
            Tuple<Move, int> ret = new Tuple<Move, int>(bestMove, 0);
            return ret;
        }

        foreach (Move move in moves)
        {
            board.MakeMove(move);
            int value = -alphaBeta(board, depth - 1, -b, -a, mate - 1).Item2;
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

        if (moves.Count == 0)
        {
            if (board.IsInCheck() == false || board.IsDraw())
            {
                return new Tuple<Move, int>(bestMove, 0);
            }
            return new Tuple<Move, int>(bestMove, -mate);
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

        
        if (prioStartMove && moves.Remove(startMove)) 
        {
            moves = moves.Prepend(startMove).ToList();
            prioStartMove = false;
        }
        

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
