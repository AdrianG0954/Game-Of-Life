using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace GOLStartUpTemplate3
{
    public partial class Form1 : Form
    {
        #region Variables
        static int WidthNum = 30;
        static int HeightNum = 30;
        //neighbors' neighbors count
        int[,] NeighborsCount = new int[WidthNum, HeightNum];
        // The universe array
        bool[,] universe = new bool[WidthNum, HeightNum];
        //scratchpad array
        bool[,] scratchPad = new bool[WidthNum, HeightNum];
        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;
        Color nah = Color.FromArgb(180,255,0,0);

        // The Timer class
        Timer timer = new Timer();

        //Current Seed 
        Random rand = new Random();
        static int Storage = 2500;

        // Generation count
        int generations = 0;

        //num of alive
        int numberOfAlive = 0;

        //bools
        bool IsHudOn = true;
        bool NeighborCountBool = true;
        bool IsGridOn = true;
        #endregion

        #region Form1 and Timer Tick
        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 20; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
            toolStripStatusLabel2.Text = "Interval = " + timer.Interval;

            //Current Seed
            toolStripStatusSeed.Text = "Seed = " + Storage;

            graphicsPanel1.BackColor = Properties.Settings.Default.BackGroundColor;
            gridColor = Properties.Settings.Default.GridColor;
            cellColor = Properties.Settings.Default.CellColor;
            timer.Interval = Properties.Settings.Default.TimerInterval;
            WidthNum = Properties.Settings.Default.WidthCell;
            HeightNum = Properties.Settings.Default.HeightCell;
        }
        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }
        #endregion

        #region Next Generation and Count Neighbor Functions
        // Calculate the next generation of cells
        private void NextGeneration()
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    scratchPad[x, y] = false;
                    int numOfAlive = CountNeighborsToroidal(x, y);
                    NextNeighborTorodial(x, y);

                    //Apply the rules
                    if (universe[x, y] == true)
                    {
                        if (numOfAlive < 2)
                        {
                            scratchPad[x, y] = false;
                            numberOfAlive--;
                        }
                        else if (numOfAlive > 3)
                        {
                            scratchPad[x, y] = false;
                            numberOfAlive--;
                        }
                        else if (numOfAlive == 2 || numOfAlive == 3)
                        {
                            scratchPad[x, y] = true;
                        }
                    }
                    else
                    {
                        if (numOfAlive == 3)
                        {
                            scratchPad[x, y] = true;
                            numberOfAlive++;
                        }
                    }
                }
            }

            //copy what is in scratchpad to universe.
            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;

            // Increment generation count
            generations++;

            // Update status strip generations   
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

            //update alive count
            toolStripStatusAlive.Text = "Alive = " + numberOfAlive.ToString();

            //invaliate
            graphicsPanel1.Invalidate();
        }
        //this function counts neighbors finitly
        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    if (xOffset == 0 && yOffset == 0) { continue; }

                    if (xCheck < 0) { continue; }

                    if (yCheck < 0) { continue; }

                    if (xCheck >= xLen) { continue; }

                    if (yCheck >= yLen) { continue; }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }

        //this is a count neighbor toroidal function
        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0) { continue; }
                    // if xCheck is less than 0 then set to xLen - 1
                    if (xCheck < 0)
                    {
                        xCheck = xLen - 1;
                    }
                    // if yCheck is less than 0 then set to yLen - 1
                    if (yCheck < 0)
                    {
                        yCheck = yLen - 1;
                    }
                    // if xCheck is greater than or equal too xLen then set to 0
                    if (xCheck >= xLen)
                    {
                        xCheck = 0;
                    }
                    // if yCheck is greater than or equal too yLen then set to 0
                    if (yCheck >= yLen)
                    {
                        yCheck = 0;
                    }
                    if (universe[xCheck, yCheck] == true) count++;
                }
            }

            return count;
        }
        //this function is to count the neighbors neighbors of finite 
        private void NextNeighborFinite(int x, int y)
        {
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);

            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    if (xOffset == 0 && yOffset == 0) { continue; }

                    if (xCheck < 0) { continue; }

                    if (yCheck < 0) { continue; }

                    if (xCheck >= xLen) { continue; }

                    if (yCheck >= yLen) { continue; }

                    //NeighborsCount[xCheck, yCheck] = CountNeighborsFinite(xCheck, yCheck);
                }
            }
        }
        private void NextNeighborTorodial(int x, int y)
        {

            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0) { continue; }
                    // if xCheck is less than 0 then set to xLen - 1
                    if (xCheck < 0)
                    {
                        xCheck = xLen - 1;
                    }
                    // if yCheck is less than 0 then set to yLen - 1
                    if (yCheck < 0)
                    {
                        yCheck = yLen - 1;
                    }
                    // if xCheck is greater than or equal too xLen then set to 0
                    if (xCheck >= xLen)
                    {
                        xCheck = 0;
                    }
                    // if yCheck is greater than or equal too yLen then set to 0
                    if (yCheck >= yLen)
                    {
                        yCheck = 0;
                    }
                    NeighborsCount[xCheck, yCheck] = CountNeighborsToroidal(xCheck, yCheck);
                }
            }
        }
        #endregion

        #region Graphic Panel Functions
        //this is what paints to the graphic panel 
        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, (float).5);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);
            Brush HUD = new SolidBrush(nah);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    if(IsGridOn == true)
                    {
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    }

                    //this prevents the rectangle from printing a zero
                    if (NeighborsCount[x, y] != 0)
                    {
                        Font font = new Font("Arial", 20f);
                        StringFormat stringFormat = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };

                        Rectangle rect = cellRect;
                        int neighbors = NeighborsCount[x, y];
                        if(NeighborCountBool == true)
                        {
                            // this is to show what kills and what lets a cell live red / kill green / live
                            if (universe[x, y] == true && NeighborsCount[x, y] == 2 || NeighborsCount[x, y] == 3)
                            {
                                e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, rect, stringFormat);
                            }
                            else
                                e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, rect, stringFormat);
                        }
                    }
                }
            }
            Font helloFont = new Font("Arial", 20f);
            StringFormat stringFormat1 = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Far
            };
            if(IsHudOn == true)
            {
                e.Graphics.DrawString($"Generations = {generations}\nCell Count = {numberOfAlive}\n Boundary Type:\nUniverse Size: (Width:{WidthNum},Height{HeightNum})", 
                    helloFont, HUD, graphicsPanel1.ClientRectangle, stringFormat1);
            }
            
            //Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }
        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];
                if (universe[x, y] == true)
                {
                    numberOfAlive++;
                }
                else
                    numberOfAlive--;
                toolStripStatusAlive.Text = "Alive = " + numberOfAlive.ToString();
                NextNeighborTorodial(x, y);

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }
        #endregion

        #region Click Functions
        //this code is to exit the program if the user selects Exit.
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //this is to start.
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }
        //code to pause the game.
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }
        //code to iterate once
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }
        //code to clear the screen.
        private void NewToolStripButton_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {

                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                    NeighborsCount[x, y] = 0;
                    numberOfAlive = 0;
                    generations = 0;
                    toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
                    toolStripStatusAlive.Text = "Alive = " + numberOfAlive.ToString();
                }
            }
            graphicsPanel1.Invalidate();
        }
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();

            graphicsPanel1.BackColor = Properties.Settings.Default.BackGroundColor;
            gridColor = Properties.Settings.Default.GridColor;
            cellColor = Properties.Settings.Default.CellColor;
            timer.Interval = Properties.Settings.Default.TimerInterval;
            toolStripStatusLabel2.Text = "Interval = " + Properties.Settings.Default.TimerInterval;
            WidthNum = Properties.Settings.Default.WidthCell;
            HeightNum = Properties.Settings.Default.HeightCell;
        }
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();

            graphicsPanel1.BackColor = Properties.Settings.Default.BackGroundColor;
            gridColor = Properties.Settings.Default.GridColor;
            cellColor = Properties.Settings.Default.CellColor;
            timer.Interval = Properties.Settings.Default.TimerInterval;
            toolStripStatusLabel2.Text = "Interval = " + Properties.Settings.Default.TimerInterval;
            WidthNum = Properties.Settings.Default.WidthCell;
            HeightNum = Properties.Settings.Default.HeightCell;
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open();
        }
        private void hUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hUDToolStripMenuItem.Checked == false)
            {
                IsHudOn = false;
            }
            else
                IsHudOn = true;

            graphicsPanel1.Invalidate();
        }
        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (neighborCountToolStripMenuItem.Checked == false)
            {
                NeighborCountBool = false;
            }
            else
                NeighborCountBool = true;

            graphicsPanel1.Invalidate();
        }
        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridToolStripMenuItem.Checked == false)
            {
                IsGridOn = false;
            }
            else
                IsGridOn = true;

            graphicsPanel1.Invalidate();
        }
        #endregion

        #region Color dialog and Modal dialog code
        //Code to change color of the background on click in settings and on rightclick
        private void BackColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            colorDialog.Color = graphicsPanel1.BackColor;

            if (DialogResult.OK == colorDialog.ShowDialog())
            {
                graphicsPanel1.BackColor = colorDialog.Color;
            }
            graphicsPanel1.Invalidate();
        }
        //code to change the cell color in settings and on right click.
        private void cellColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            colorDialog.Color = cellColor;

            if (DialogResult.OK == colorDialog.ShowDialog())
            {
                cellColor = colorDialog.Color;
            }
            graphicsPanel1.Invalidate();
        }
        //code to change the grid color
        private void gridColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            colorDialog.Color = gridColor;

            if (DialogResult.OK == colorDialog.ShowDialog())
            {
                gridColor = colorDialog.Color;
            }
            graphicsPanel1.Invalidate();
        }
        //options code
        private void ModalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Modal hello = new Modal();

            hello.TimeInterval = timer.Interval;
            hello.WidthCell = WidthNum;
            hello.HeightCell = HeightNum;

            if (DialogResult.OK == hello.ShowDialog())
            {
                timer.Interval = hello.TimeInterval;
                toolStripStatusLabel2.Text = "Interval = " + timer.Interval;
                if(WidthNum != hello.WidthCell || HeightNum != hello.HeightCell)
                {
                    WidthNum = hello.WidthCell;
                    HeightNum = hello.HeightCell;
                    NeighborsCount = new int[WidthNum, HeightNum];
                    universe = new bool[WidthNum, HeightNum];
                    scratchPad = new bool[WidthNum, HeightNum];
                    numberOfAlive = 0;
                    toolStripStatusAlive.Text = "Alive = " + numberOfAlive.ToString();
                }
            }
            graphicsPanel1.Invalidate();
        }
        private void helpToolStripButton_Click(object sender, EventArgs e)
        {
            Tutorial Welcome = new Tutorial();

            Welcome.ShowDialog();
        }
        #endregion

        #region Form Closed code
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.BackGroundColor = graphicsPanel1.BackColor;
            Properties.Settings.Default.GridColor = gridColor;
            Properties.Settings.Default.CellColor = cellColor;
            Properties.Settings.Default.TimerInterval = timer.Interval;
            Properties.Settings.Default.WidthCell = WidthNum;
            Properties.Settings.Default.HeightCell = HeightNum;
            toolStripStatusLabel2.Text = "Interval = " + Properties.Settings.Default.TimerInterval;

            Properties.Settings.Default.Save();
        }
        #endregion

        #region Functions for randomizing
        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            RandomModal hmm = new RandomModal();

            hmm.RandomSeed = Storage;

            if (DialogResult.OK == hmm.ShowDialog())
            {
                Storage = hmm.RandomSeed;
                FromRandSeed();
                toolStripStatusSeed.Text = "Seed = " + Storage;
                toolStripStatusAlive.Text = "Alive = " + numberOfAlive.ToString();
            }
            graphicsPanel1.Invalidate();
        }

        private void RandomSeed()
        {
            numberOfAlive = 0;
            Storage = rand.Next(-999999,999999);

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (rand.Next(0, 2) == 0)
                    {
                        universe[x, y] = true;
                        numberOfAlive++;
                        NextNeighborTorodial(x, y);
                    }
                    else
                        universe[x, y] = false;
                }
            }
            toolStripStatusAlive.Text = "Alive = " + numberOfAlive.ToString();
            toolStripStatusSeed.Text = "Seed = " + Storage.ToString();

            graphicsPanel1.Invalidate();
        }

        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RandomSeed();
        }

        private void fromCurrentSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FromCurrentSeed();
        }
        private void FromRandSeed()
        {
            numberOfAlive = 0;

            for (int y = 0; y < universe.GetLength(1); y++)
            {

                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (rand.Next(0, 2) == 0)
                    {
                        universe[x, y] = true;
                        numberOfAlive++;
                        NextNeighborTorodial(x, y);
                    }
                    else
                        universe[x, y] = false;
                }
            }

            toolStripStatusAlive.Text = "Alive = " + numberOfAlive.ToString();

            graphicsPanel1.Invalidate();
        }
        private void FromCurrentSeed()
        {
            numberOfAlive = 0;

            for (int y = 0; y < universe.GetLength(1); y++)
            {

                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (rand.Next(0, 2) == 0)
                    {
                        universe[x, y] = true;
                        numberOfAlive++;
                        NextNeighborTorodial(x, y);
                    }
                    else
                        universe[x, y] = false;
                }
            }
            toolStripStatusAlive.Text = "Alive = " + numberOfAlive.ToString();
            toolStripStatusSeed.Text = "Seed = " + Storage;

            graphicsPanel1.Invalidate();
        }
        #endregion

        #region Save and open
        private void SaveAs()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                // Use WriteLine to write the strings to the file. 
                // It appends a CRLF for you.
                writer.WriteLine("!This whole save thing is pretty cool.");

                // Iterate through the universe one row at a time.
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    // Create a string to represent the current row.
                    String currentRow = string.Empty;

                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        // If the universe[x,y] is alive then append 'O' (capital O)
                        // to the row string.
                        if (universe[x, y] == true)
                        {
                            currentRow += 'O';
                        }
                        else if (universe[x, y] == false)
                        {
                            currentRow += '.';
                        }
                        // Else if the universe[x,y] is dead then append '.' (period)
                        // to the row string.
                    }
                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                    writer.WriteLine(currentRow);
                }
                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }
        private void Open()
        {
            int yPos = 0;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;

                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then it is a comment
                    // and should be ignored.
                    if (row.StartsWith("!") == true) { continue; }

                    // If the row is not a comment then it is a row of cells.
                    if (row.StartsWith("!") == false)
                    {
                        maxHeight++;
                        // Increment the maxHeight variable for each row read.
                    }
                    // Get the length of the current row string
                    int hello = row.Length;

                    if (row.StartsWith("!") == false)
                    {
                        // and adjust the maxWidth variable if necessary.
                        maxWidth = hello;
                    }
                }

                // Resize the current universe and scratchPad
                universe = new bool[maxWidth, maxHeight];
                scratchPad = new bool[maxWidth, maxHeight];
                NeighborsCount = new int[maxWidth, maxHeight];
                // to the width and height of the file calculated above.

                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                // Iterate through the file again, this time reading in the cells.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then
                    if (row.StartsWith("!") == true) { continue; }
                    // it is a comment and should be ignored.

                    // If the row is not a comment then 
                    if (row.StartsWith("!") == false)
                    {
                        // it is a row of cells and needs to be iterated through.
                        for (int xPos = 0; xPos < row.Length; xPos++)
                        {
                            // If row[xPos] is a 'O' (capital O) then
                            if (row[xPos] == 'O')
                            {
                                // set the corresponding cell in the universe to alive.
                                universe[xPos, yPos] = true;
                            }
                            // If row[xPos] is a '.' (period) then
                            if (row[xPos] == '.')
                            {
                                // set the corresponding cell in the universe to dead.
                                universe[xPos, yPos] = false;
                            }
                            yPos++;
                        }
                    }
                }
                // Close the file.
                reader.Close();
            }
        }

        #endregion
    }
}
