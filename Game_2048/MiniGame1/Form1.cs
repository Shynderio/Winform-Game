using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_2048.MiniGame1
{
    public partial class Form1 : Form
    {
        public int[][] map ;
        public PictureBox screen = new PictureBox();
        public PictureBox[,] pics = new PictureBox[4, 4];
        private int num_of_rows = 4;
        private int num_of_cols = 4;
        private bool mapChanged = false;

        private GameOver gameOverForm;
        private bool gameOver = false;

        private Dictionary<int, Image> imageMap = new Dictionary<int, Image>();
        private string assetsFolderPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "MiniGame1\\Assets"));
        public Form1()
        {
            imageMap.Add(0, null);
            for (int i = 2; i < 65536; i *= 2)
            {
                imageMap.Add(i, Image.FromFile(Path.Combine(assetsFolderPath, i +".png")));
            }
            MakeEmptyBoard();
            CreateMap();
            InitializeComponent();
            
            
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                // Arrow down key is pressed
                // Add your logic here for handling the arrow down key press
            }
        }

        public void MakeEmptyBoard()
        {
            map = new int[num_of_rows][];

            for (int row = 0; row < num_of_rows; row++)
            {
                map[row] = new int[num_of_cols];
                for (int col = 0; col < num_of_cols; col++)
                {
                    map[row][col] = 0;
                }
            }

        }

        public void AddTwo()
        {
            List<int> a = new List<int>();
            for (int row = 0; row < num_of_rows; row++)
            {
                for (int col = 0; col < num_of_cols; col++)
                {
                    if (map[row][col] == 0)
                    {
                        a.Add(row * num_of_cols + col);
                    }
                }
            }

            Random random = new Random();
            int rannum = random.Next(0, a.Count);
            int num_to_add = random.Next(0, 16) % 8 == 0 ? 4 : 2;
            map[a[rannum] / num_of_cols][a[rannum] % num_of_cols] = num_to_add;
            mapChanged = true;
        }

        public void MoveBoard()
        {
            for (int row = 0; row < num_of_rows; row++)
            {
                for (int col = 0; col < num_of_cols - 1; col++)
                {
                    if (map[row][col] == 0 && map[row][col + 1] != 0)
                    {
                        map[row][col] = map[row][col + 1];
                        map[row][col + 1] = 0;
                        if (col != 0)
                        {
                            col -= 2;
                        }
                    }
                }
            }
            mapChanged = true;
        }

        public void PrintMapChanges()
        {
            if (mapChanged)
            {
                string mapAsString = GetMapAsString(map);  // Replace this with a method to convert map to a string representation
                MessageBox.Show("Map Changes:\n" + mapAsString, "Map Changes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                mapChanged = false; // Reset the flag after displaying the message
            }
        }
        public void MergeBoard()
        {
            for (int row = 0; row < num_of_rows; row++)
            {
                for (int col = 0; col < num_of_cols - 1; col++)
                {
                    if (map[row][col] == map[row][col + 1] && map[row][col] != 0)
                    {
                        map[row][col] *= 2;
                        map[row][col + 1] = 0;
                    }
                }
            }
            mapChanged = true;
        }
        public void RotateBoard(int k)
        {
            int numRows = map.Length;
            int numCols = map[0].Length;

            int[][] rotatedBoard = new int[numRows][];
            for (int row = 0; row < numRows; row++)
            {
                rotatedBoard[row] = new int[numCols];
            }
           
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    switch (k)
                    {
                        case 1: // Rotate 90 degrees clockwise
                            rotatedBoard[col][numCols - 1 - row] = map[row][col];
                            break;
                        case 2: // Rotate 180 degrees
                            rotatedBoard[numRows - 1 - row][numCols - 1 - col] = map[row][col];
                            break;
                        case 3: // Rotate 90 degrees counterclockwise
                            rotatedBoard[numRows - 1 - col][row] = map[row][col];
                            break;
                        default:
                            break;
                    }
                }
            }

            // Update the original board with the rotated values
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    map[row][col] = rotatedBoard[row][col];
                }
            }
        }

        public void MoveLeft()
        {
            MoveBoard();
            MergeBoard();
            MoveBoard();

        }

        public void MoveRight()
        {
            RotateBoard(2);
            MoveBoard();
            MergeBoard();
            MoveBoard();
            RotateBoard(2);

        }

        public void MoveDown()
        {
            RotateBoard(1);
            MoveBoard();
            MergeBoard();
            MoveBoard();
            RotateBoard(3);
        }

        public void MoveUp()
        {
            RotateBoard(3);
            MoveBoard();
            MergeBoard();
            MoveBoard();
            RotateBoard(1);

        }

        public bool CompareBoard(int[][] a, int[][] b)
        {
            for (int row = 0; row < num_of_rows; row++)
            {
                for (int col = 0; col < num_of_cols; col++)
                {
                    if (a[row][col] != b[row][col])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool IsFull(int[][] board)
        {
            for (int row = 0; row < num_of_rows; row++)
            {
                for (int col = 0; col < num_of_cols; col++)
                {
                    if (board[row][col] == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool Moveable(int[][] board)
        {
            for (int row = 0; row < num_of_rows; row++)
            {
                for (int col = 0; col < num_of_cols - 1; col++)
                {
                    if (board[row][col] == board[row][col + 1] || board[row][col] == 0 || board[row][col + 1]==0)
                    {
                        return true;
                    }

                }
            }

            for (int row = 0; row < num_of_rows - 1; row++)
            {
                for (int col = 0; col < num_of_cols; col++)
                {
                    if (board[row][col] == board[row + 1][col])
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.KeyDown -= new KeyEventHandler(OnKeyboardPressed);
            this.KeyDown += new KeyEventHandler(OnKeyboardPressed);

        }

        private int[][] CreateDeepCopy(int[][] source)
        {
            int numRows = source.Length;
            int numCols = source[0].Length;

            int[][] copy = new int[numRows][];
            for (int row = 0; row < numRows; row++)
            {
                copy[row] = new int[numCols];
                for (int col = 0; col < numCols; col++)
                {
                    copy[row][col] = source[row][col];
                }
            }

            return copy;
        }

        private void OnKeyboardPressed(object sender, KeyEventArgs e)
        {
            int[][] temp = CreateDeepCopy(map);
            switch (e.KeyCode.ToString())
            {
                case "Right":
                    MoveRight();
                    break;
                case "Left":
                    MoveLeft();
                    break;
                case "Down":
                    MoveDown();
                    break;
                case "Up":
                    MoveUp();
                    break;
            }
            

            if (!CompareBoard(temp, map))
            {
                AddTwo();
            }
            LoadMap();
            if (!Moveable(map))
            {
                HandleGameOver();
            }
        }

        private void CreateMap()
        {
            AddTwo();

            int cellSize = Math.Min(this.ClientSize.Height, this.ClientSize.Width) / 4;
            int imageSize = (int)(cellSize * 0.9); // Adjust the image size to fit within the cell with a small gap
            int gapSize = (int)(cellSize * 0.05); // Gap between cells

            for (int row = 0; row < num_of_rows; row++)
            {
                for (int col = 0; col < num_of_cols; col++)
                {
                    int cellValue = map[row][col];

                    // Create a PictureBox for each cell in the map array
                    PictureBox pic = new PictureBox();
                    pic.Location = new Point(cellSize * col + gapSize, cellSize * row + gapSize);
                    pic.Size = new Size(imageSize, imageSize);
                    pic.SizeMode = PictureBoxSizeMode.StretchImage;
                    pic.BackColor = Color.Gray;
                    pic.Image = Image.FromFile(assetsFolderPath + "\\" + cellValue + ".png");
                    pics[row, col] = pic;

                    this.Controls.Add(pics[row, col]);
                }
            }
        }

        private void LoadMap()
        {
            int cellSize = Math.Min(this.ClientSize.Height, this.ClientSize.Width) / 4;
            int imageSize = (int)(cellSize * 0.9); // Adjust the image size to fit within the cell with a small gap
            int gapSize = (int)(cellSize * 0.05); // Gap between cells
            for (int row = 0; row < num_of_rows; row++)
            {
                for (int col = 0; col < num_of_cols; col++)
                {
                    int cellValue = map[row][col];
                    pics[row, col].Location = new Point(cellSize * col + gapSize, cellSize * row + gapSize);
                    pics[row, col].Size = new Size(imageSize, imageSize);
                    pics[row, col].SizeMode = PictureBoxSizeMode.StretchImage;
                    pics[row, col].BackColor = Color.Gray;
                    pics[row, col].Image = Image.FromFile(assetsFolderPath + "\\" + cellValue + ".png");

                    // Create a PictureBox for each cell in the map array
                }
            }
        }


        private Color GetColorForCellValue(int cellValue)
        {
            // Add your logic to set the background color based on the cell value
            // For example, you can use a switch statement to set colors for different values.

            // Replace the following example colors with your own color choices:
            switch (cellValue)
            {
                case 2:
                    return Color.LightBlue;
                case 4:
                    return Color.LightPink;
                case 8:
                    return Color.LightGreen;
                // Add more cases for other values...
                default:
                    return Color.Gray;
            }
        }

        private string GetMapAsString(int[][] temp)
        {
            StringBuilder mapString = new StringBuilder();

            for (int row = 0; row < num_of_rows; row++)
            {
                for (int col = 0; col < num_of_cols; col++)
                {
                    mapString.Append(temp[row][col].ToString().PadLeft(5)); // Use PadLeft to format cell values
                }
                mapString.AppendLine(); // Add a new line after each row
            }

            return mapString.ToString();
        }

        private void Form1_MaximumSizeChanged(object sender, EventArgs e)
        {
            this.ClientSize = this.MaximumSize;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            LoadMap();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void HandleGameOver()
        {
            // Game over, show the replay button and blur the form

            // Disable game controls (if applicable)
            // ...

            // Show the "Replay" button

            gameOverForm = new GameOver();
            gameOverForm.ReplayClicked += GameOverForm_ReplayClicked;
            gameOverForm.QuitClicked += GameOverForm_QuitClicked;
            this.Hide();
            // Blur the form by showing it as a modal dialog
            gameOverForm.ShowDialog();

        }
        private void GameOverForm_ReplayClicked(object sender, EventArgs e)
        {
            // The replay button was clicked in the GameOverForm
            // Show this form again and reset the game
            MakeEmptyBoard();
            AddTwo();
            this.Show();
            
            gameOverForm.Close(); // Close the GameOverForm
            // Reset the game here, e.g., by calling a method to reset the map and other variables.
        }

        private void GameOverForm_QuitClicked(object sender, EventArgs e)
        {
            // The replay button was clicked in the GameOverForm
            // Show this form again and reset the game
            gameOverForm.Close();

            this.Close(); // Close the GameOverForm
            // Reset the game here, e.g., by calling a method to reset the map and other variables.
        }

    }

}
