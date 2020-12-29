using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andantino
{
    public class AI
    {
        public Board board;
        public int settings;
        public int colour;
        public int choiceX = 0;
        public int choiceY = 0;
        public Dictionary<int, TTValue> TT;
        public int[,,] ZT;

        public AI(Board board,  int settings, int colour)
        {
            this.board = board;
            this.settings = settings;
            this.colour = colour;
            if (settings >= 5)
            {
                TT = new Dictionary<int, TTValue>();
                ZT = new int[19, 19, 2];
                Random rng = new Random();
                for (int i = 0; i < 19; i++)
                    for (int j = 0; j < 19; j++)
                        for (int k = 0; k < 2; k++)
                        {
                            ZT[i, j, k] = rng.Next();
                        }
            }
        }

        public static Board CloneBoard(Board board)
        {
            Board clone = new Board();
            for (int i = 0; i < 19; i++)
                for (int j = 0; j < 19; j++)
                    clone.grid[i, j] = board.grid[i, j];
            clone.stoneCount = board.stoneCount;
            return clone;
        }

        // settings
        // 0: Minimax
        // 1: Minimax Alpha-Beta
        // 2: Negamax Alpha-Beta
        // 3: PVS
        // 4: Alpha-Beta with Variable Depth Iterative Deepening
        // 5: Alpha-Beta with Transposition Tables
        // 6: PVS with Transposition Tables
        public void Compute(int x, int y)
        {
            switch (settings)
            {
                case 0:
                    MiniMax(CloneBoard(board), 5, true, x, y);
                    board.Place(colour, choiceX, choiceY);
                    break;

                case 1:
                    MAlphaBeta(CloneBoard(board), 5, true, x, y, -9999999, 9999999);
                    board.Place(colour, choiceX, choiceY);
                    break;

                case 2:
                    NAlphaBeta(CloneBoard(board), 5, true, x, y, -9999999, 9999999);
                    board.Place(colour, choiceX, choiceY);
                    break;

                case 3:
                    PVS(CloneBoard(board), 5, true, x, y, -9999999, 9999999);
                    board.Place(colour, choiceX, choiceY);
                    break;

                case 4:
                    AlphaBetaID(CloneBoard(board), 5, true, x, y, -9999999, 9999999);
                    board.Place(colour, choiceX, choiceY);
                    break;

                case 5:
                    AlphaBetaTT(CloneBoard(board), 5, true, x, y, -9999999, 9999999);
                    board.Place(colour, choiceX, choiceY);
                    break;

                case 6:
                    PVSwTT(CloneBoard(board), 5, true, x, y, -9999999, 9999999);
                    board.Place(colour, choiceX, choiceY);
                    break;

                default:
                    MiniMax(CloneBoard(board), 3, true, x, y);
                    board.Place(colour, choiceX, choiceY);
                    break;
            }
        }

        // Minimax evaluation function
        public int Eval(Board state, int colour, int x, int y)
        {
            if (state.CheckWin(colour, x, y))
            {
                if (this.colour == colour) return 100;
                else return -100;
            }
            else
            {
                int stonesInRow = state.CountRow(colour, x, y, 0, false) + state.CountRow(colour, x, y, 0, true) - 2;
                stonesInRow += state.CountRow(colour, x, y, 1, false) + state.CountRow(colour, x, y, 1, true) - 2;
                stonesInRow += state.CountRow(colour, x, y, 2, false) + state.CountRow(colour, x, y, 2, true) - 2;

                if (this.colour == colour) return stonesInRow * 10;
                else return stonesInRow * -10;
            }
        }

        // Negamax evaluation function
        public int NEval(Board state, int colour, int x, int y, int depth)
        {
            if (state.CheckWin(colour, x, y))
                return 100 * (depth + 1);
            else
            {
                int stonesInRow = state.CountRow(colour, x, y, 0, false) + state.CountRow(colour, x, y, 0, true) - 2;
                stonesInRow += state.CountRow(colour, x, y, 1, false) + state.CountRow(colour, x, y, 1, true) - 2;
                stonesInRow += state.CountRow(colour, x, y, 2, false) + state.CountRow(colour, x, y, 2, true) - 2;
                return stonesInRow * 10;
            }
        }

        // Negamax Evaluation function for double depth
        public int NEval(Board state, int colour, int x, int y, double depth)
        {
            if (state.CheckWin(colour, x, y))
                return Convert.ToInt32(100 * (depth + 1));
            else
            {
                int stonesInRow = state.CountRow(colour, x, y, 0, false) + state.CountRow(colour, x, y, 0, true) - 2;
                stonesInRow += state.CountRow(colour, x, y, 1, false) + state.CountRow(colour, x, y, 1, true) - 2;
                stonesInRow += state.CountRow(colour, x, y, 2, false) + state.CountRow(colour, x, y, 2, true) - 2;
                return stonesInRow * 10;
            }
        }

        public int ZobristHash(Board state)
        {
            int hash = 0;
            for (int i = 0; i < 19; i++)
                for (int j = 0; j < 19; j++)
                    if (state.grid[i, j] > 0)
                        hash ^= ZT[i, j, state.grid[i, j] - 1];
            return hash;
        }

        // type = true MAX ; type = false MIN;
        public int MiniMax(Board board, int depth, bool type, int x, int y)
        {
            Board state = CloneBoard(board);
            int currentColour;

            if (type) currentColour = this.colour;
            else if (this.colour == Constants.BLACK) currentColour = Constants.WHITE;
            else currentColour = Constants.BLACK;

            int prevColour = Board.InvertColour(currentColour);

            if (depth == 0 || state.CheckWin(prevColour, x, y)) return Eval(state, prevColour, x, y);

            List<int> scores = new List<int>();
            List<int> moveX = new List<int>();
            List<int> moveY = new List<int>();

            for (int i = 0; i < 19; i++)
                for (int j = 0; j < 19; j++)
                {
                    if (state.IsLegal(i, j))
                    {
                        state.grid[i, j] = currentColour;
                        moveX.Add(i);
                        moveY.Add(j);
                        scores.Add(MiniMax(state, depth - 1, !type, i , j));
                        state.grid[i, j] = Constants.EMPTY;
                    }
                }

            if (type)
            {
                int maxScoreIndex = scores.IndexOf(scores.Max());
                choiceX = moveX[maxScoreIndex];
                choiceY = moveY[maxScoreIndex];
                return scores.Max();
            }
            else
            {
                int minScoreIndex = scores.IndexOf(scores.Min());
                choiceX = moveX[minScoreIndex];
                choiceY = moveY[minScoreIndex];
                return scores.Min();
            }
        }

        // NegaMax implementation of Alpha-Beta
        public int NAlphaBeta(Board board, int depth, bool type, int x, int y, int alpha, int beta)
        {
            Board state = CloneBoard(board);
            int currentColour;

            if (type) currentColour = this.colour;
            else if (this.colour == Constants.BLACK) currentColour = Constants.WHITE;
            else currentColour = Constants.BLACK;

            int prevColour = Board.InvertColour(currentColour);

            if (depth == 0 || state.CheckWin(prevColour, x, y)) return -NEval(state, prevColour, x, y, depth);

            List<int> scores = new List<int>();
            List<int> moveX = new List<int>();
            List<int> moveY = new List<int>();
            // Avoiding ArgumentNullException + making other lists keep up with index.
            scores.Add(-9999999);
            moveX.Add(0);
            moveY.Add(0);

            int value;
            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 19; j++)
                {
                    if (state.IsLegal(i, j))
                    {
                        state.grid[i, j] = currentColour;
                        value = -NAlphaBeta(state, depth - 1, !type, i, j, -beta, -alpha);
                        state.grid[i, j] = Constants.EMPTY;
                        if (value > scores.Max())
                        {
                            scores.Add(value);
                            moveX.Add(i);
                            moveY.Add(j);
                        }
                        if (scores.Max() > alpha) alpha = scores.Max();
                        if (alpha >= beta) return alpha;
                    }
                }
            }

            int maxScoreIndex = scores.IndexOf(scores.Max());
            choiceX = moveX[maxScoreIndex];
            choiceY = moveY[maxScoreIndex];
            return scores.Max();
        }

        // Minimax implementation of Alpha-Beta
        public int MAlphaBeta(Board board, int depth, bool type, int x, int y, int alpha, int beta)
        {
            Board state = CloneBoard(board);
            int currentColour;

            if (type) currentColour = this.colour;
            else if (this.colour == Constants.BLACK) currentColour = Constants.WHITE;
            else currentColour = Constants.BLACK;

            int prevColour = Board.InvertColour(currentColour);

            if (depth == 0 || state.CheckWin(prevColour, x, y)) return Eval(state, prevColour, x, y);

            List<int> scoresMax = new List<int>();
            List<int> moveXMax = new List<int>();
            List<int> moveYMax = new List<int>();
            List<int> scoresMin = new List<int>();
            List<int> moveXMin = new List<int>();
            List<int> moveYMin = new List<int>();
            // Avoiding ArgumentNullException + making other lists keep up with index.
            scoresMax.Add(-9999999);
            moveXMax.Add(0);
            moveYMax.Add(0);
            scoresMin.Add(9999999);
            moveXMin.Add(0);
            moveYMin.Add(0);

            int value;
            if (type)
            {
                for (int i = 0; i < 19; i++)
                {
                    for (int j = 0; j < 19; j++)
                    {
                        if (state.IsLegal(i, j))
                        {
                            state.grid[i, j] = currentColour;
                            value = MAlphaBeta(state, depth - 1, !type, i, j, alpha, beta);
                            state.grid[i, j] = Constants.EMPTY;
                            if (value > scoresMax.Max())
                            {
                                scoresMax.Add(value);
                                moveXMax.Add(i);
                                moveYMax.Add(j);
                            }
                            if (scoresMax.Max() > alpha) alpha = scoresMax.Max();
                            if (alpha >= beta) break;
                        }
                    }
                    if (alpha >= beta) break;
                }

                int maxScoreIndex = scoresMax.IndexOf(scoresMax.Max());
                choiceX = moveXMax[maxScoreIndex];
                choiceY = moveYMax[maxScoreIndex];
                return scoresMax.Max();
            }
            else
            {
                for (int i = 0; i < 19; i++)
                {
                    for (int j = 0; j < 19; j++)
                    {
                        if (state.IsLegal(i, j))
                        {
                            state.grid[i, j] = currentColour;
                            value = MAlphaBeta(state, depth - 1, !type, i, j, alpha, beta);
                            state.grid[i, j] = Constants.EMPTY;
                            if (value < scoresMin.Min())
                            {
                                scoresMin.Add(value);
                                moveXMin.Add(i);
                                moveYMin.Add(j);
                            }
                            if (scoresMin.Min() < beta) beta = scoresMin.Min();
                            if (alpha >= beta) break;
                        }
                    }
                    if (alpha >= beta) break;
                }

                int minScoreIndex = scoresMin.IndexOf(scoresMin.Min());
                choiceX = moveXMin[minScoreIndex];
                choiceY = moveYMin[minScoreIndex];
                return scoresMin.Min();
            }
        }

        public int PVS(Board board, int depth, bool type, int x, int y, int alpha, int beta)
        {
            Board state = CloneBoard(board);
            int currentColour;

            if (type) currentColour = this.colour;
            else if (this.colour == Constants.BLACK) currentColour = Constants.WHITE;
            else currentColour = Constants.BLACK;

            int prevColour = Board.InvertColour(currentColour);

            if (depth == 0 || state.CheckWin(prevColour, x, y)) return -NEval(state, prevColour, x, y, depth);

            List<int> scores = new List<int>();
            List<int> moveX = new List<int>();
            List<int> moveY = new List<int>();
            // Avoiding ArgumentNullException + making other lists keep up with index.
            scores.Add(-9999999);
            moveX.Add(0);
            moveY.Add(0);

            int score;
            bool isFirst = true;
            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 19; j++)
                {
                    if (state.IsLegal(i, j))
                    {
                        if (isFirst)
                        {
                            state.grid[i, j] = currentColour;
                            score = -PVS(state, depth - 1, !type, i, j, -beta, -alpha);
                            state.grid[i, j] = Constants.EMPTY;
                            isFirst = false;
                        }
                        else
                        {
                            state.grid[i, j] = currentColour;
                            score = -PVS(state, depth - 1, !type, i, j, -alpha - 10, -alpha);
                            state.grid[i, j] = Constants.EMPTY;
                            if (alpha < score && score < beta)
                            {
                                state.grid[i, j] = currentColour;
                                score = -PVS(state, depth - 1, !type, i, j, -beta, -score);
                                state.grid[i, j] = Constants.EMPTY;
                            }
                        }
                        scores.Add(score);
                        moveX.Add(i);
                        moveY.Add(j);
                        alpha = Math.Max(alpha, score);
                        if (alpha >= beta) break;
                    }
                }
                if (alpha >= beta) break;
            }

            int maxScoreIndex = scores.IndexOf(scores.Max());
            choiceX = moveX[maxScoreIndex];
            choiceY = moveY[maxScoreIndex];
            return scores.Max();
        }

        // Negamax Alpha-Beta with Iterative Deepening (Variable Depth).       
        public int AlphaBetaID(Board board, double depth, bool type, int x, int y, int alpha, int beta)
        {
            Board state = CloneBoard(board);
            int currentColour;

            if (type) currentColour = this.colour;
            else if (this.colour == Constants.BLACK) currentColour = Constants.WHITE;
            else currentColour = Constants.BLACK;

            int prevColour = Board.InvertColour(currentColour);

            if (depth <= 0 || state.CheckWin(prevColour, x, y)) return -NEval(state, prevColour, x, y, depth);

            List<int> scores = new List<int>();
            List<int> moveX = new List<int>();
            List<int> moveY = new List<int>();
            // Avoiding ArgumentNullException + making other lists keep up with index.
            scores.Add(-9999999);
            moveX.Add(0);
            moveY.Add(0);

            int value;
            double depthChange;
            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 19; j++)
                {
                    if (state.IsLegal(i, j))
                    {
                        state.grid[i, j] = currentColour;

                        if (NEval(state, currentColour, i, j, depth) >= 70)
                            depthChange = 0.2;
                        else if (NEval(state, currentColour, i, j, depth) >= 50)
                            depthChange = 0.5;
                        else if (NEval(state, currentColour, i, j, depth) >= 30)
                            depthChange = 0.8;
                        else if (NEval(state, currentColour, i, j, depth) >= 10)
                            depthChange = 1;
                        else
                            depthChange = 2;

                        value = -AlphaBetaID(state, depth - depthChange, !type, i, j, -beta, -alpha);
                        state.grid[i, j] = Constants.EMPTY;
                        if (value > scores.Max())
                        {
                            scores.Add(value);
                            moveX.Add(i);
                            moveY.Add(j);
                        }
                        if (scores.Max() > alpha) alpha = scores.Max();
                        if (alpha >= beta) return alpha;
                    }
                }
            }

            int maxScoreIndex = scores.IndexOf(scores.Max());
            choiceX = moveX[maxScoreIndex];
            choiceY = moveY[maxScoreIndex];
            return scores.Max();
        }
        

        // Alpha-Beta with Transposition Table.
        public int AlphaBetaTT(Board board, int depth, bool type, int x, int y, int alpha, int beta)
        {
            Board state = CloneBoard(board);
            int currentColour;

            if (type) currentColour = this.colour;
            else if (this.colour == Constants.BLACK) currentColour = Constants.WHITE;
            else currentColour = Constants.BLACK;

            int prevColour = Board.InvertColour(currentColour);

            int olda = alpha;
            TTValue n;
            if (TT.TryGetValue(ZobristHash(state), out n))
            {
                if (n.depth >= depth)
                {
                    if (n.flag == 0)
                    {
                        choiceX = n.moveX;
                        choiceY = n.moveY;
                        return -n.value;
                    }
                    else if (n.flag == -1)
                        alpha = Math.Max(alpha, n.value);
                    else if (n.flag == 1)
                        beta = Math.Min(beta, n.value);
                    if (alpha >= beta) return n.value;
                }
            }

            if (depth == 0 || state.CheckWin(prevColour, x, y)) return -NEval(state, prevColour, x, y, depth);

            List<int> scores = new List<int>();
            List<int> moveX = new List<int>();
            List<int> moveY = new List<int>();
            // Avoiding ArgumentNullException + making other lists keep up with index.
            scores.Add(-9999999);
            moveX.Add(0);
            moveY.Add(0);

            int value;
            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 19; j++)
                {
                    if (state.IsLegal(i, j))
                    {
                        state.grid[i, j] = currentColour;
                        value = -AlphaBetaTT(state, depth - 1, !type, i, j, -beta, -alpha);
                        state.grid[i, j] = Constants.EMPTY;
                        if (value > scores.Max())
                        {
                            scores.Add(value);
                            moveX.Add(i);
                            moveY.Add(j);
                        }
                        if (scores.Max() > alpha) alpha = scores.Max();
                        if (alpha >= beta) break;
                    }
                }
                if (alpha >= beta) break;
            }

            int maxScoreIndex = scores.IndexOf(scores.Max());
            choiceX = moveX[maxScoreIndex];
            choiceY = moveY[maxScoreIndex];

            int flag;
            if (scores.Max() < olda) flag = 1;
            else if (scores.Max() > beta) flag = -1;
            else flag = 0;

            state.grid[choiceX, choiceY] = currentColour;
            TTValue newEntry = new TTValue(CloneBoard(state), scores.Max(), depth, flag, choiceX, choiceY, this);
            if (!TT.TryGetValue(ZobristHash(state), out n))
                TT.Add(ZobristHash(state), newEntry);
            state.grid[choiceX, choiceY] = Constants.EMPTY;

            return scores.Max();
        }

        public int PVSwTT(Board board, int depth, bool type, int x, int y, int alpha, int beta)
        {
            Board state = CloneBoard(board);
            int currentColour;

            if (type) currentColour = this.colour;
            else if (this.colour == Constants.BLACK) currentColour = Constants.WHITE;
            else currentColour = Constants.BLACK;

            int prevColour = Board.InvertColour(currentColour);

            int olda = alpha;
            TTValue n;
            if (TT.TryGetValue(ZobristHash(state), out n))
            {
                if (n.depth >= depth)
                {
                    if (n.flag == 0)
                    {
                        choiceX = n.moveX;
                        choiceY = n.moveY;
                        return -n.value;
                    }
                    else if (n.flag == -1)
                        alpha = Math.Max(alpha, n.value);
                    else if (n.flag == 1)
                        beta = Math.Min(beta, n.value);
                    if (alpha >= beta) return n.value;
                }
            }

            if (depth == 0 || state.CheckWin(prevColour, x, y)) return -NEval(state, prevColour, x, y, depth);

            List<int> scores = new List<int>();
            List<int> moveX = new List<int>();
            List<int> moveY = new List<int>();
            // Avoiding ArgumentNullException + making other lists keep up with index.
            scores.Add(-9999999);
            moveX.Add(0);
            moveY.Add(0);

            int score;
            bool isFirst = true;
            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 19; j++)
                {
                    if (state.IsLegal(i, j))
                    {
                        if (isFirst)
                        {
                            state.grid[i, j] = currentColour;
                            score = -PVSwTT(state, depth - 1, !type, i, j, -beta, -alpha);
                            state.grid[i, j] = Constants.EMPTY;
                            isFirst = false;
                        }
                        else
                        {
                            state.grid[i, j] = currentColour;
                            score = -PVSwTT(state, depth - 1, !type, i, j, -alpha - 10, -alpha);
                            state.grid[i, j] = Constants.EMPTY;
                            if (alpha < score && score < beta)
                            {
                                state.grid[i, j] = currentColour;
                                score = -PVSwTT(state, depth - 1, !type, i, j, -beta, -score);
                                state.grid[i, j] = Constants.EMPTY;
                            }
                        }
                        scores.Add(score);
                        moveX.Add(i);
                        moveY.Add(j);
                        alpha = Math.Max(alpha, score);
                        if (alpha >= beta) break;
                    }
                }
                if (alpha >= beta) break;
            }

            int maxScoreIndex = scores.IndexOf(scores.Max());
            choiceX = moveX[maxScoreIndex];
            choiceY = moveY[maxScoreIndex];

            int flag;
            if (scores.Max() < olda) flag = 1;
            else if (scores.Max() > beta) flag = -1;
            else flag = 0;

            state.grid[choiceX, choiceY] = currentColour;
            TTValue newEntry = new TTValue(CloneBoard(state), scores.Max(), depth, flag, choiceX, choiceY, this);
            if (!TT.TryGetValue(ZobristHash(state), out n))
                TT.Add(ZobristHash(state), newEntry);
            state.grid[choiceX, choiceY] = Constants.EMPTY;

            return scores.Max();
        }
    }
}
