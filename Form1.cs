using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace n_queens
{
    public partial class Form1 : Form
    {
        private GameController game = new GameController();
        public static int boardOffset = 100; // pixels to offset by
        public static int borderWidth = 1;

        //for drawing
        Font arialFont = new Font("Arial", 30, FontStyle.Bold);
        Pen borderPen = new Pen(Color.Black, borderWidth); // draws the border on each square
        SolidBrush blackBrush = new SolidBrush(Color.Black); // for filling the squares
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush redBrush = new SolidBrush(Color.Red);

        public Form1() // initialize
        {
            InitializeComponent();
            this.Width = boardOffset * 2 + GameController.squareSize * GameController.boardSize;
            this.Height = boardOffset * 2 + GameController.squareSize * GameController.boardSize;
        }

        private void drawQueens(Graphics g)
        {
            string drawString = "Q";

            int[] positions = game.getPositionInRow();
            for (int i = 0; i < positions.Length; i++)
            {
                if (positions[i] != -1) // if there should be a queen in this position
                {
                    System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                    if (isBlackSquare(i, positions[i]) && !game.hints)
                    { // if the square is black, use a white letter
                        drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                    }
                    Console.WriteLine("Writing Queen in row {0} col {1})", i, positions[i]);
                    g.DrawString(drawString, arialFont, drawBrush, GameController.squareSize * positions[i] + borderWidth, GameController.squareSize * i + borderWidth
                        );
                    drawBrush.Dispose();
                }
            }
        }

        private bool isBlackSquare(int row, int col) // returns true if the position is a black square
        {
            return (col % 2 == 0 && row % 2 == 0 || col % 2 == 1 && row % 2 == 1);
        }

        private void drawBoard(Graphics g, GameController game)
        {
            g.TranslateTransform(100, 100); // offset drawing

            int fillSize = GameController.squareSize - borderWidth;
            // draw the squares
            for (int i = 0; i < GameController.boardSize; i++) // rows
            {
                for (int j = 0; j < GameController.boardSize; j++) //columns
                {
                    int x = GameController.squareSize * j;
                    int y = GameController.squareSize * i;
                    g.DrawRectangle(borderPen, x, y, GameController.squareSize, GameController.squareSize);
                    if (game.hints == true && !game.spaceAvailable(i, j)) // if hints are on and we can't go here, fill it red
                    {
                        g.FillRectangle(redBrush, x + borderWidth, y + borderWidth, fillSize, fillSize);
                    }
                    else if (isBlackSquare(i, j)) // if on an even column, even row or odd column, odd row
                    {
                        g.FillRectangle(blackBrush, x + borderWidth, y + borderWidth, fillSize, fillSize);
                    }
                    else
                    { // fill with white
                        g.FillRectangle(whiteBrush, x + borderWidth, y + borderWidth, fillSize, fillSize);
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            drawBoard(e.Graphics, game);
            drawQueens(e.Graphics);
            updateQueenCountLabel();
        }

        public bool onBoard(int x, int y) // returns true if the x,y coordinate is on the board
        {
            return x >= boardOffset && x <= boardOffset + GameController.boardSize * GameController.squareSize && y >= boardOffset && y <= boardOffset + GameController.boardSize * GameController.squareSize;
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            int col = (e.X - boardOffset) / GameController.squareSize;// translate the click x,y coordinates to row, col
            int row = (e.Y - boardOffset) / GameController.squareSize;
            Console.WriteLine("Mouse click at  {0},{1}", e.X, e.Y);
            Console.WriteLine("Row:{0}, Col:{1}", row, col);
            if (row < GameController.boardSize && col < GameController.boardSize)
            {
                if (e.Button == MouseButtons.Left) // mark this position
                {
                    if (onBoard(e.X, e.Y))
                    {
                        game.tryPlacingQueenAt(row, col, this.CreateGraphics());
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    game.clearQueenAt(row, col);
                }
            }
            this.Invalidate();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            game.hints = !game.hints; // toggle hints
            this.Invalidate();
        }

        private void updateQueenCountLabel()
        {
            label1.Text = String.Format("You have {0} Queens on the board", game.numQueens);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            game.resetPositions();
            this.Invalidate();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           
        }

    
    }

    public class GameController // object to control game state

    {
       
      
        public static int boardSize = 8;
        public static int squareSize = 50;
        public bool hints = false;
        public int numQueens = 0;
        private int[] positionInRow = new int[boardSize]; // an array of the position of the queen in each row
        private object textBox1;

        

        public int[] getPositionInRow()
        {
            return positionInRow;
        }

        public void resetPositions() // mark all positions at -1 to signify no queen
        {
            for (int i = 0; i < positionInRow.Length; i++)
            {
                positionInRow[i] = -1;
            }
            numQueens = 0;
        }

        public void tryPlacingQueenAt(int row, int col, Graphics g) // places a queen if the space is available
        {
            if (spaceAvailable(row, col))
            {
                positionInRow[row] = col; // update array
                numQueens++;
                checkForWin();
            }
            else // can't place a queen here, play a sound
            {
                System.Media.SystemSounds.Beep.Play();
            }
        }

        public void checkForWin()
        {
            if (numQueens > boardSize - 1) // celebrate!
            {
                System.Media.SystemSounds.Hand.Play();
                MessageBox.Show("You won!");
            }
        }

        public void clearQueenAt(int row, int col)
        { // if there's a queen there
            if (spaceOccupied(row, col)) // if there's a queen here
            {
                positionInRow[row] = -1;
                numQueens--;
            }
        }

        public bool spaceOccupied(int row, int col) // returns true if there's a queen there
        {
            return (positionInRow[row] == col) ? true : false;
        }

        public bool spaceAvailable(int row, int col)
        {
            if (spaceOccupied(row, col))
            {
                return false;
            }
            else if (positionInRow[row] != -1) // row isn't empty
            {
                return false;
            }
            // check if positionInRow contains column number..this means there is a queen in the column
            foreach (int thisCol in positionInRow)
            {
                if (thisCol == col)
                {
                    return false;
                }
            }
            // check diagonals
            for (int i = 0; i < positionInRow.Length; i++) // loop through row positions
            {
                if (positionInRow[i] != -1) //there's a queen here
                {
                    if (Math.Abs(row - i) == Math.Abs(col - positionInRow[i]))
                    {
                        return false;
                    }
                }
            }
            // if we haven't returned false yet, then the space is available
            return true;
        }

        public GameController()
        {
            resetPositions();
        }
    }
}



