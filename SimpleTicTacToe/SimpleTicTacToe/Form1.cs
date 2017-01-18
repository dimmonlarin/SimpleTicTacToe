using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleTicTacToe
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                Game.PlayerMoves(Convert.ToInt16((sender as Button).Tag));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            var moveResult = Game.ComputerMoves();
            if (!moveResult.HasValue)
            {
                MessageBox.Show("Draw!");
                panel1.Enabled = false;
            }
            else if (moveResult.Value)
            {
                MessageBox.Show("Computer wins!");
                panel1.Enabled = false;

            }

            Redraw();

        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Game.ResetBoard();
            Redraw();
            panel1.Enabled = true;
        }

        private void Redraw()
        {
            foreach (var control in panel1.Controls)
            {
                if (control is Button)
                {
                    var btn = control as Button;
                    if (Game.board[Convert.ToInt16(btn.Tag)] == null)
                        btn.Text = string.Empty;
                    else
                        btn.Text = Game.board[Convert.ToInt16(btn.Tag)] == 1 ? "X" : "0";
                }
            }
        }
    }

    public class Game
    {
        public static readonly int?[] corners = new int?[] { 1, 3, 5, 7 };
        public static readonly int center = 0;
        public static readonly List<int[]> winPatterns = new List<int[]>() { new int[]{ 1,2,3 },
                                                            new int[] { 3, 4, 5 },
                                                            new int[]{ 5,6,7 },
                                                            new int[]{ 7,8,1 },
                                                            new int[]{ 4,0,8 },
                                                            new int[]{ 2,0,6 },
                                                            new int[]{ 1,0,5 },
                                                            new int[]{ 3,0,7 }};
        public static int?[] board = new int?[9] { null, null, null, null, null, null, null, null, null };

        public static void ResetBoard()
        {
            board = new int?[9] { null, null, null, null, null, null, null, null, null };
        }

        public static void PlayerMoves(int position)
        {
            if (board[position] != null)
                throw new Exception("The position is taken");
            else
                board[position] = 1;
        }

        /// <summary>
        /// Calculates next move
        /// </summary>
        /// <returns></returns>
        public static bool? ComputerMoves()
        {
            //if no moves - draw
            if (Game.board.Count(p => p == null) == 0)
                return null;
            //find a win position
            var winPos = FindWinPositions(0);
            if (winPos != null)
            {
                board[winPos.Value] = 0;
                return true;
            }
            //occupy center if empty
            if (board[0] == null)
            {
                board[0] = 0;
                return false;
            }

            //find user's win position
            winPos = FindWinPositions(1);
            if (winPos != null)
            {
                board[winPos.Value] = 0;
                return false;
            }

            //occupy corner if available
            var corner = FindAvailableCorner(0);
            if (corner != null)
            {
                board[corner.Value] = 0;
                return false;
            }


            return null;
        }

        /// <summary>
        /// Finds and occupies a win position
        /// </summary>
        /// <param name="val">0 for 0, 1 for X</param>
        /// <returns>null if not found, cell index - if found</returns>
        public static int? FindWinPositions(int val)
        {
            foreach (var win in winPatterns)
            {
                var vals = win.Select(p => board[p]).ToArray();
                if (vals.Count(p => p == val) == 2 && vals.Count(p => p == null) == 1)
                {
                    return win[Array.IndexOf(vals, null)];
                }
            }
            return null;
        }

        /// <summary>
        /// Looing for possible winn patterns
        /// </summary>
        /// <param name="val">0 for 0, 1 for X</param>
        /// <returns></returns>
        public static IEnumerable<int[]> FindAvailableWinPattern(int val)
        {
            foreach (var win in winPatterns)
            {
                var pos = win.Select(p => board[p]).ToArray();
                if (pos.Count(p => p == null) == 3 || //empty row or column
                   (pos.Count(p => p == val) == 1 && pos.Count(p => p == null) == 2))//row or column with one ticked cell
                {
                    yield return win;
                }
            }
            yield break;
        }

        public static int? FindAvailableCorner(int val)
        {
            return corners.FirstOrDefault(c => board[c.Value] == null);
        }

    }
}

    
