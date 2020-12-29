using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Andantino
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Welcome to Andantino!\n");
            System.Console.WriteLine("Please, choose mode:");
            System.Console.WriteLine("0. Player vs Player");
            System.Console.WriteLine("1. Player vs AI");
            System.Console.WriteLine("2. AI vs Player");
            System.Console.WriteLine("3. AI vs AI");

            int mode = Convert.ToInt32(System.Console.ReadLine());
            System.Console.WriteLine();

            Board board = new Board();
            board.Place(Constants.BLACK, 9, 9);

            // turn = true in WHITE's turn.
            bool turn = true;
            bool win = false;
            bool valid = false;
            string[] user = new string[2];
            int x, y;

            switch (mode)
            {
                case 0:
                    while (!win)
                    {
                        valid = false;
                        if (turn)
                        {
                            while (!valid)
                            {
                                System.Console.WriteLine("White, please enter a move:");
                                user = System.Console.ReadLine().Split(' ');
                                x = Convert.ToInt32(user[0]);
                                y = Convert.ToInt32(user[1]);
                                if (x == -1 || y == -1)
                                    board.Undo();
                                else
                                {
                                    valid = board.Place(Constants.WHITE, x, y);
                                    if (!valid)
                                        System.Console.WriteLine("Invalid Move.");
                                    else
                                        win = board.CheckWin(Constants.WHITE, x, y);
                                }
                            }
                        }
                        else
                        {
                            while (!valid)
                            {
                                System.Console.WriteLine("Black, please enter a move:");
                                user = System.Console.ReadLine().Split(' ');
                                x = Convert.ToInt32(user[0]);
                                y = Convert.ToInt32(user[1]);
                                if (x == -1 || y == -1)
                                    board.Undo();
                                else
                                {
                                    valid = board.Place(Constants.BLACK, x, y);
                                    if (!valid)
                                        System.Console.WriteLine("Invalid Move.");
                                    else
                                        win = board.CheckWin(Constants.BLACK, x, y);
                                }
                            }
                        }

                        if (win)
                            if (turn)
                                System.Console.WriteLine("WHITE WINS!!!");
                            else
                                System.Console.WriteLine("BLACK WINS!!!");

                        turn = !turn;
                    }
                    break;

                case 1:
                    while (!win)
                    {
                        AI ai = new AI(board, 6, Constants.WHITE);
                        valid = false;
                        if (turn)
                        {
                            System.Console.WriteLine("White, please enter a move:");
                            ai.Compute(board.GetLastX(), board.GetLastY());
                            win = board.CheckWin(Constants.WHITE, board.GetLastX(), board.GetLastY());
                        }
                        else
                        {
                            while (!valid)
                            {
                                System.Console.WriteLine("Black, please enter a move:");
                                user = System.Console.ReadLine().Split(' ');
                                x = Convert.ToInt32(user[0]);
                                y = Convert.ToInt32(user[1]);
                                if (x == -1 || y == -1)
                                    board.Undo();
                                else
                                {
                                    valid = board.Place(Constants.BLACK, x, y);
                                    if (!valid)
                                        System.Console.WriteLine("Invalid Move.");
                                    else
                                        win = board.CheckWin(Constants.BLACK, x, y);
                                }
                            }
                        }

                        if (win)
                            if (turn)
                                System.Console.WriteLine("WHITE WINS!!!");
                            else
                                System.Console.WriteLine("BLACK WINS!!!");

                        turn = !turn;
                    }
                    break;

                case 2:
                    while (!win)
                    {
                        AI ai = new AI(board, 6, Constants.BLACK);
                        valid = false;
                        if (turn)
                        {
                            while (!valid)
                            {
                                System.Console.WriteLine("White, please enter a move:");
                                user = System.Console.ReadLine().Split(' ');
                                x = Convert.ToInt32(user[0]);
                                y = Convert.ToInt32(user[1]);
                                if (x == -1 || y == -1)
                                    board.Undo();
                                else
                                {
                                    valid = board.Place(Constants.WHITE, x, y);
                                    if (!valid)
                                        System.Console.WriteLine("Invalid Move.");
                                    else
                                        win = board.CheckWin(Constants.WHITE, x, y);
                                }
                            }
                        }
                        else
                        {
                            System.Console.WriteLine("Black, please enter a move:");
                            ai.Compute(board.GetLastX(), board.GetLastY());
                            win = board.CheckWin(Constants.BLACK, board.GetLastX(), board.GetLastY());
                        }

                        if (win)
                            if (turn)
                                System.Console.WriteLine("WHITE WINS!!!");
                            else
                                System.Console.WriteLine("BLACK WINS!!!");

                        turn = !turn;
                    }
                    break;

                case 3:
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    while (!win)
                    {
                        AI ai1 = new AI(board, 10, Constants.WHITE);
                        AI ai2 = new AI(board, 6, Constants.BLACK);
                        if (turn)
                        {
                            System.Console.WriteLine("White, please enter a move:");
                            ai1.Compute(board.GetLastX(), board.GetLastY());
                            win = board.CheckWin(Constants.WHITE, board.GetLastX(), board.GetLastY());
                
                        }
                        else
                        {
                            System.Console.WriteLine("Black, please enter a move:");
                            ai2.Compute(board.GetLastX(), board.GetLastY());
                            win = board.CheckWin(Constants.BLACK, board.GetLastX(), board.GetLastY());
                        }

                        if (win)
                        {
                            if (turn)
                                System.Console.WriteLine("WHITE WINS!!!");
                            else
                                System.Console.WriteLine("BLACK WINS!!!");
                            sw.Stop();
                            System.Console.WriteLine("Elapsed={0}", sw.Elapsed);
                        }

                        turn = !turn;
                    }
                    break;
            }

            System.Console.ReadKey();
        }
    }
}
