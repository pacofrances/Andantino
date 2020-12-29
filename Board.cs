using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andantino
{
    static class Constants
    {
        public const int EMPTY = 0;
        public const int BLACK = 1;
        public const int WHITE = 2;
        public const int INVALID = -1;
    }

    public class Board
    {
        public int[,] grid;
        public int stoneCount;
        public int lastX;
        public int lastY;
        public int prevLastX;
        public int prevLastY;

        public Board()
        {
            grid = new int[19, 19];
            stoneCount = 0;

            int invalidCount = 9;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 19; j++)
                {
                    if (j >= 19 - invalidCount)
                        grid[i, j] = Constants.INVALID;
                    else
                        grid[i, j] = Constants.EMPTY;
                }
                invalidCount--;
            }

            invalidCount = 1;
            for (int i = 10; i < 19; i++)
            {
                for (int j = 0; j < 19; j++)
                {
                    if (j < invalidCount)
                        grid[i, j] = Constants.INVALID;
                    else
                        grid[i, j] = Constants.EMPTY;
                }
                invalidCount++;
            }
        }

        public int GetType(int x, int y)
        {
            return grid[x, y];
        }

        public static int InvertColour(int colour)
        {
            if (colour == Constants.BLACK) return Constants.WHITE;
            else return Constants.BLACK;
        }

        public int GetLastX()
        {
            return lastX;
        }

        public int GetLastY()
        {
            return lastY;
        }

        public int GetPrevLastX()
        {
            return prevLastX;
        }

        public int GetPrevLastY()
        {
            return prevLastY;
        }

        public void Undo()
        {
            grid[lastX, lastY] = Constants.EMPTY;
            grid[prevLastX, prevLastY] = Constants.EMPTY;
            System.Console.WriteLine("Move undone!\n");
        }

        public bool Place(int colour, int x, int y)
        {
            if (IsLegal(x, y))
            {
                grid[x, y] = colour;
                stoneCount++;
                prevLastX = lastX;
                prevLastY = lastY;
                lastX = x;
                lastY = y;
                if (colour == Constants.BLACK)
                    System.Console.WriteLine("Black places a stone on (" + x + ", " + y + ")\n");
                else
                    System.Console.WriteLine("White places a stone on (" + x + ", " + y + ")\n");
                return true;
            }
            else return false;
        }

        public bool IsLegal(int x, int y)
        {
            if (!IsEmpty(x, y) || IsInvalid(x, y))
            {
                return false;
            }
            else if (stoneCount == 0)
            {
                return true;
            }
            else if (stoneCount == 1 && CountAround(x,y) == 0)
            {
                return false;
            }
            else if (CountAround(x,y) < 2 && stoneCount >= 2)
            {
                return false;
            }
            else return true;
        }

        public bool IsEmpty(int x, int y)
        {
            if (grid[x, y] == Constants.EMPTY) return true;
            else return false;
        }

        public bool IsInvalid(int x, int y)
        {
            if (grid[x, y] == Constants.INVALID) return true;
            else return false;
        }

        public int CountAround(int x, int y)
        {
            int count = 0;

            if (y != 18)
                if (!IsEmpty(x, y + 1) && !IsInvalid(x, y + 1)) count++;

            if (x != 18 && y != 18)
                if (!IsEmpty(x + 1, y + 1) && !IsInvalid(x + 1, y + 1)) count++;

            if (x != 18)
                if (!IsEmpty(x + 1, y) && !IsInvalid(x + 1, y)) count++;

            if (y != 0)
                if (!IsEmpty(x, y - 1) && !IsInvalid(x, y - 1)) count++;

            if (x != 0 && y != 0)
                if (!IsEmpty(x - 1, y - 1) && !IsInvalid(x - 1, y - 1)) count++;

            if (x != 0)
                if (!IsEmpty(x - 1, y) && !IsInvalid(x - 1, y)) count++;

            return count;
        }

        public bool CheckWin(int colour, int x, int y)
        {
            if (CountRow(colour, x, y, 0, false) + CountRow(colour, x, y, 0, true) - 1 >= 5)
                return true;
            else if (CountRow(colour, x, y, 1, false) + CountRow(colour, x, y, 1, true) - 1 >= 5)
                return true;
            else if (CountRow(colour, x, y, 2, false) + CountRow(colour, x, y, 2, true) - 1 >= 5)
                return true;
            else if (CheckAround(colour, x, y))
                return true;
            else return false;
        }

        public int CountRow(int colour, int x, int y, int mode, bool dir)
        {
            if (mode == 0)
            {
                if (dir)
                {
                    if (x == 18 || y == 18) return 1;
                    else if (colour != GetType(x + 1, y + 1)) return 1;
                    else return 1 + CountRow(colour, x + 1, y + 1, mode, dir);
                }
                else
                {
                    if (x == 0 || y == 0) return 1;
                    else if (colour != GetType(x - 1, y - 1)) return 1;
                    else return 1 + CountRow(colour, x - 1, y - 1, mode, dir);
                }
            }
            else if (mode == 1)
            {
                if (dir)
                {
                    if (x == 18) return 1;
                    else if (colour != GetType(x + 1, y)) return 1;
                    else return 1 + CountRow(colour, x + 1, y, mode, dir);
                }
                else
                {
                    if (x == 0) return 1;
                    else if (colour != GetType(x - 1, y)) return 1;
                    else return 1 + CountRow(colour, x - 1, y, mode, dir);
                }
            }
            else
            {
                if (dir)
                {
                    if (y == 18) return 1;
                    else if (colour != GetType(x, y + 1)) return 1;
                    else return 1 + CountRow(colour, x, y + 1, mode, dir);
                }
                else
                {
                    if (y == 0) return 1;
                    else if (colour != GetType(x, y - 1)) return 1;
                    else return 1 + CountRow(colour, x, y - 1, mode, dir);
                }
            }
        }

        public bool CheckAround(int colour, int x, int y)
        {
            bool def = false;
            
            if (x != 18 && y != 18)
            {
                if (GetType(x + 1, y + 1) != Constants.INVALID && GetType(x + 1, y + 1) != colour)
                {
                    Board checkBoard1 = AI.CloneBoard(this);
                    checkBoard1.grid[x + 1, y + 1] = colour;
                    def = FindEdge(checkBoard1, colour, x + 1, y + 1);
                    if (!def) return true;
                }
            }
            if (x != 0 && y != 0)
            {
                if (GetType(x - 1, y - 1) != Constants.INVALID && GetType(x - 1, y - 1) != colour)
                {
                    Board checkBoard2 = AI.CloneBoard(this);
                    checkBoard2.grid[x - 1, y - 1] = colour;
                    def = FindEdge(checkBoard2, colour, x - 1, y - 1);
                    if (!def) return true;
                }
            }
            if (x != 18)
            {
                if (GetType(x + 1, y) != Constants.INVALID && GetType(x + 1, y) != colour)
                {
                    Board checkBoard3 = AI.CloneBoard(this);
                    checkBoard3.grid[x + 1, y] = colour;
                    def = FindEdge(checkBoard3, colour, x + 1, y);
                    if (!def) return true;
                }
            }
            if (x != 0)
            {
                if (GetType(x - 1, y) != Constants.INVALID && GetType(x - 1, y) != colour)
                {
                    Board checkBoard4 = AI.CloneBoard(this);
                    checkBoard4.grid[x - 1, y] = colour;
                    def = FindEdge(checkBoard4, colour, x - 1, y);
                    if (!def) return true;
                }
            }
            if (y != 18)
            {
                if (GetType(x, y + 1) != Constants.INVALID && GetType(x, y + 1) != colour)
                {
                    Board checkBoard5 = AI.CloneBoard(this);
                    checkBoard5.grid[x, y + 1] = colour;
                    def = FindEdge(checkBoard5, colour, x, y + 1);
                    if (!def) return true;
                }
            }
            if (y != 0)
            {
                if (GetType(x, y - 1) != Constants.INVALID && GetType(x, y - 1) != colour)
                {
                    Board checkBoard6 = AI.CloneBoard(this);
                    checkBoard6.grid[x, y - 1] = colour;
                    def = FindEdge(checkBoard6, colour, x, y - 1);
                    if (!def) return true;
                }
            }

            return false;
        }

        public bool FindEdge(Board state, int colour, int x, int y)
        {
            bool def = false;
            if (x == 18 || x == 0 || y == 18 || y == 0 || Math.Abs(x - y) == 9)
                def = true;
            else
            {
                if (state.grid[x + 1, y + 1] != colour)
                {
                    state.grid[x + 1, y + 1] = colour;
                    def = FindEdge(state, colour, x + 1, y + 1);
                    if (def) return def;
                }
                if (state.grid[x - 1, y - 1] != colour)
                {
                    state.grid[x - 1, y - 1] = colour;
                    def = FindEdge(state, colour, x - 1, y - 1);
                    if (def) return def;
                }
                if (state.grid[x + 1, y] != colour)
                {
                    state.grid[x + 1, y] = colour;
                    def = FindEdge(state, colour, x + 1, y);
                    if (def) return def;
                }
                if (state.grid[x - 1, y] != colour)
                {
                    state.grid[x - 1, y] = colour;
                    def = FindEdge(state, colour, x - 1, y);
                    if (def) return def;
                }
                if (state.grid[x, y + 1] != colour)
                {
                    state.grid[x, y + 1] = colour;
                    def = FindEdge(state, colour, x, y + 1);
                    if (def) return def;
                }
                if (state.grid[x, y - 1] != colour)
                {
                    state.grid[x, y - 1] = colour;
                    def = FindEdge(state, colour, x, y - 1);
                    if (def) return def;
                }
            }
            return def;
        }
    }
}
