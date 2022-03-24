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

        //this is the default width of the grid
        static int WidthNum = 30;
        //this is the default height of the grid
        static int HeightNum = 30;
        //neighbors neighbors count
        int[,] NeighborsCount = new int[WidthNum, HeightNum];
        // The universe array
        bool[,] universe = new bool[WidthNum, HeightNum];
        //scratchpad array
        bool[,] scratchPad = new bool[WidthNum, HeightNum];
        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;
        Color nah = Color.FromArgb(180, 255, 0, 0);

        // The Timer class
        Timer timer = new Timer();

        //Current Seed 
        static int Storage = 2500;

        // Generation count
        int generations = 0;

        //num of alive
        int numberOfAlive = 0;

        //bools
        bool IsHudOn = true;
        bool NeighborCountBool = true;
        bool IsGridOn = true;
        bool IsTorodial = true;
        #endregion

        #region Form1 and Timer Tick
        //this code is for the form/form settings
        public Form1()
        {

            // Turn on double buffering.
            this.DoubleBuffered = true;

            // Allow repainting when the windows is resized.
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            // Allow repainting when the windows is resized.
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            //initializes the form
            InitializeComponent();

            //to disable pause button since there is nothing to pause
            toolStripButton2.Enabled = false;

            // Setup the timer
            timer.Interval = 20; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
            toolStripStatusLabel2.Text = "Interval = " + timer.Interval;

            //Current Seed
            toolStripStatusSeed.Text = "Seed = " + Storage;

            //this code is to save the options the user changes in the settings
            graphicsPanel1.BackColor = Properties.Settings.Default.BackGroundColor;
            gridColor = Properties.Settings.Default.GridColor;
            cellColor = Properties.Settings.Default.CellColor;
            timer.Interval = Properties.Settings.Default.TimerInterval;
            toolStripStatusLabel2.Text = "Interval = " + timer.Interval;
            WidthNum = Properties.Settings.Default.WidthCell;
            HeightNum = Properties.Settings.Default.HeightCell;
            universe = new bool[WidthNum, HeightNum];
            scratchPad = new bool[WidthNum, HeightNum];
            NeighborsCount = new int[WidthNum, HeightNum];
            numberOfAlive = Properties.Settings.Default.NumOfAlive;
        }
        //this calls next generation every tick
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }
        #endregion

        #region Next Generation and Count Neighbor Functions
        // Calculate the next generation of cells
        private void NextGeneration()
        {
            //goes through entire grid
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int numOfAlive;
                    //this check if torodial or finite is checked
                    if (IsTorodial == true)
                    {
                        //once checked it makes the num of alive = count neighbors and calls next neighbor torodial
                        numOfAlive = CountNeighborsToroidal(x, y);
                        NextNeighborTorodial(x, y);
                    }
                    else
                    {
                        //this is if the other is checked and it makes the num of alive = count neighbors and calls next neighbor finite
                        numOfAlive = CountNeighborsFinite(x, y);
                        NextNeighborFinite(x, y);
                    }
                    scratchPad[x, y] = false;

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

            //this goes through all the next neighbors and updates them 
            for(int y = 0; y < universe.GetLength(1); y++)
            {
                for(int x = 0; x < universe.GetLength(0); x++)
                {
                    if (IsTorodial == true)
                    {
                        NeighborsCount[x,y] = CountNeighborsToroidal(x, y);
                    }
                    else
                    {
                        NeighborsCount[x,y] = CountNeighborsFinite(x, y);
                    }
                }
            }

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

                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0) { continue; }

                    // if xCheck is less than 0 then continue
                    if (xCheck < 0) { continue; }

                    // if yCheck is less than 0 then continue
                    if (yCheck < 0) { continue; }

                    // if xCheck is greater than or equal too xLen then continue
                    if (xCheck >= xLen) { continue; }

                    // if yCheck is greater than or equal too yLen then continue
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
        
        //this function counts the neighbors of the cell that was clicked finitly 
        private void NextNeighborFinite(int x, int y)
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

                    // if xCheck is less than 0 then continue
                    if (xCheck < 0) { continue; }

                    // if yCheck is less than 0 then continue
                    if (yCheck < 0) { continue; }

                    // if xCheck is greater than or equal too xLen then continue
                    if (xCheck >= xLen) { continue; }

                    // if yCheck is greater than or equal too yLen then continue
                    if (yCheck >= yLen) { continue; }

                    if (universe[xCheck, yCheck] == true) count++;

                    //if its not torodial it sets the neighbor count to count neighbor finite
                    if (IsTorodial == false)
                    {
                        NeighborsCount[xCheck, yCheck] = CountNeighborsFinite(xCheck, yCheck);
                    }
                }
            }
        }
        //this function counts the neighbors of the cell that was clicked (torodial) 
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
                    if (IsTorodial == true)
                    {
                        NeighborsCount[xCheck, yCheck] = CountNeighborsToroidal(xCheck, yCheck);
                    }

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
            float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

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
                    RectangleF cellRect = RectangleF.Empty;
                    cellRect.X = (float)x * cellWidth;
                    cellRect.Y = (float)y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    if (IsGridOn == true)
                    {
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    }

                    //this prevents the rectangle from printing a zero
                    if (NeighborsCount[x, y] != 0)
                    {
                        Font font = new Font("Arial", 18f);
                        StringFormat stringFormat = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };

                        RectangleF rect = cellRect;
                        int neighbors = NeighborsCount[x,y];
                        if (NeighborCountBool == true)
                        {
                            // this is to show what kills and what lets a cell live / cell that will live has green / cell that will die has red 
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
            Font helloFont = new Font("Arial", 14f);
            StringFormat stringFormat1 = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Far
            };
            string boundary = " ";
            //this draws the grid depending if the grid is turned on or off
            if (IsHudOn == true)
            {
                e.Graphics.DrawString($"Generations = {generations}\nCell Count = {numberOfAlive}\nBoundary Type: {boundaryType(boundary)}\nUniverse Size: (Width:{WidthNum},Height:{HeightNum})",
                helloFont, HUD, graphicsPanel1.ClientRectangle, stringFormat1);
            }
            //Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();

        }
        //this occurs every left click
        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
                float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = (int)((float)e.X / cellWidth);
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = (int)((float)e.Y / cellHeight);

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                //if the cell is alive add one to the number of alive if one is dead subtract one
                if (universe[x, y] == true)
                {
                    numberOfAlive++;
                }
                else
                    numberOfAlive--;
                //updates number of alive strip
                toolStripStatusAlive.Text = "Alive = " + numberOfAlive.ToString();

                //calls next neighbor depending on boundary type
                if (IsTorodial == true)
                {
                    NextNeighborTorodial(x, y);
                }
                else
                {
                    NextNeighborFinite(x, y);
                }

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
            toolStripButton1.Enabled = false;
            toolStripButton2.Enabled = true;
            toolStripButton3.Enabled = false;
        }
        //code to pause the game.
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            toolStripButton1.Enabled = true;
            toolStripButton3.Enabled = true;
            toolStripButton2.Enabled = false;
        }
        //code to iterate once
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration();        
        }
        //code to clear the screen.
        private void NewToolStripButton_Click(object sender, EventArgs e)
        {
            New();
        }
        //this code is to reset the setting to default
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();

            graphicsPanel1.BackColor = Properties.Settings.Default.BackGroundColor;
            gridColor = Properties.Settings.Default.GridColor;
            cellColor = Properties.Settings.Default.CellColor;
            timer.Interval = Properties.Settings.Default.TimerInterval;
            toolStripStatusLabel2.Text = "Interval = " + Properties.Settings.Default.TimerInterval;
            numberOfAlive = Properties.Settings.Default.NumOfAlive;
            toolStripStatusAlive.Text = "Alive = " + numberOfAlive;
            WidthNum = Properties.Settings.Default.WidthCell;
            HeightNum = Properties.Settings.Default.HeightCell;
            universe = new bool[WidthNum, HeightNum];
            scratchPad = new bool[WidthNum, HeightNum];
            NeighborsCount = new int[WidthNum, HeightNum];

            graphicsPanel1.Invalidate();
        }
        //this code reloads the most recently saved settings
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
        //this is what function is called when a user clicks the save button
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }
        //this is what function is called when a user clicks the open button
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open();
        }
        //this turns the hud on or off when they click it
        private void hUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hUDToolStripMenuItem.Checked == false || hUDToolStripMenuItem1.Checked == false)
            {
                IsHudOn = false;
            }
            else
                IsHudOn = true;

            graphicsPanel1.Invalidate();
        }
        //this turns the neighbor count on or off depending on the user 
        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (neighborCountToolStripMenuItem.Checked == false || neighborCountToolStripMenuItem1.Checked == false)
            {
                NeighborCountBool = false;
            }
            else
                NeighborCountBool = true;

            graphicsPanel1.Invalidate();
        }
        //this turns the grid on or off
        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridToolStripMenuItem.Checked == false || gridToolStripMenuItem1.Checked == false)
            {
                IsGridOn = false;
            }
            else
                IsGridOn = true;

            graphicsPanel1.Invalidate();
        }
        //this code is the check that either turns torodial on or off
        private void torodialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (torodialToolStripMenuItem.Checked == false && finiteToolStripMenuItem.Checked == true)
            {
                
                IsTorodial = false;
            }
            else
                IsTorodial = true;

            graphicsPanel1.Invalidate();
        }
        //this code is the check that either turns Finite on or off
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (finiteToolStripMenuItem.Checked == false && torodialToolStripMenuItem.Checked == true)
            {
                
                IsTorodial = true;
            }
            else
                IsTorodial = false;
            graphicsPanel1.Invalidate();
        }
        #endregion

        #region Color dialog and Modal dialog code
        //Code to change color of the background on click in settings and on rightclick
        private void BackColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            colorDialog.Color = graphicsPanel1.BackColor;

            //if user selects ok it saves the back color to the one selected
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
            //if user selects ok it saves the cell color to the one selected
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
            //if user selects ok it saves the grids color to the one selected
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
                if (WidthNum != hello.WidthCell || HeightNum != hello.HeightCell)
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
        //this shows the welcome code when a user clicks help 
        private void helpToolStripButton_Click(object sender, EventArgs e)
        {
            Tutorial Welcome = new Tutorial();

            Welcome.ShowDialog();
        }
        #endregion

        #region Form Closed code
        //this code sets the default settings to whatever is last saved.
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //sets my settings default to the last saved settings
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
        //this code is for the modal for randomizing.
        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RandomModal hmm = new RandomModal();

            //sets the up and down num as storage my variable for the seed.
            hmm.RandomSeed = Storage;

            //if the user hits ok it sets storage as the randomseed and updates the status
            if (DialogResult.OK == hmm.ShowDialog())
            {
                Storage = hmm.RandomSeed;
                FromRandSeed();
                toolStripStatusSeed.Text = "Seed = " + Storage;
                toolStripStatusAlive.Text = "Alive = " + numberOfAlive.ToString();
            }
            graphicsPanel1.Invalidate();
        }
        //this code is for the time part of the random.
        private void FromTime()
        {
            //calls new and sets storage to the date and time
            New();
            Storage = DateTime.Now.Millisecond;
            Random rand = new Random(Storage);

            //randomizes the entire screen
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    
                    if (rand.Next(0, 2) == 0)
                    {
                        universe[x, y] = true;
                        numberOfAlive++;
                        //gives the random neighbors and updates them when needed.
                        if (torodialToolStripMenuItem.Checked == true)
                        {
                            NextNeighborTorodial(x, y);
                        }
                        else
                        {
                            NextNeighborFinite(x, y);
                        }
                    }
                    else
                    {
                        universe[x, y] = false;
                    }   
                }
            }
            toolStripStatusAlive.Text = "Alive = " + numberOfAlive.ToString();
            toolStripStatusSeed.Text = "Seed = " + Storage;

            graphicsPanel1.Invalidate();
        }
        //this code is what gets called when you click time
        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FromTime();
        }
        //this code is what gets called when you click current seed.
        private void fromCurrentSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FromCurrentSeed();
        }
        //this code is what gets called in the modal
        private void FromRandSeed()
        {
            New();
            Random rand = new Random(Storage);

                //randomizes the entire screen
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        if (rand.Next(0, 2) == 0)
                        {
                            universe[x, y] = true;
                            numberOfAlive++;
                            //gives the random neighbors and updates them when needed.
                            if (torodialToolStripMenuItem.Checked == true)
                            {
                                NeighborsCount[x, y] = CountNeighborsToroidal(x, y);
                                NextNeighborTorodial(x, y);
                            }
                            else
                            {
                                
                                NextNeighborFinite(x, y);
                                NeighborsCount[x, y] = CountNeighborsFinite(x, y);
                            }
                        }
                        else
                        {
                            universe[x, y] = false;
                        }
                    }
                }
            
            toolStripStatusAlive.Text = "Alive = " + numberOfAlive.ToString();
            toolStripStatusSeed.Text = "Seed = " + Storage;

            graphicsPanel1.Invalidate();
        }
        //this is the code for the from current seed 
        private void FromCurrentSeed()
        {
            New();
            Random rand = new Random(Storage);

            //randomizes the entire screen
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (rand.Next(0, 2) == 0)
                    {
                        universe[x, y] = true;
                        numberOfAlive++;
                        //gives the random neighbors and updates them when needed.
                        if (torodialToolStripMenuItem.Checked == true)
                        {
                            NeighborsCount[x, y] = CountNeighborsToroidal(x, y);
                            NextNeighborTorodial(x, y);
                        }
                        else
                        {
                            NeighborsCount[x, y] = CountNeighborsFinite(x, y);
                            NextNeighborFinite(x, y);
                        }
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
        //this code is for when you press save.
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
        //this code is for when you want to open a save.
        private void Open()
        {
            int num = 0;
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
                        maxWidth++;
                    }
                }

                // Resize the current universe and scratchPad
                universe = new bool[maxWidth, maxHeight];
                scratchPad = new bool[maxWidth, maxHeight];
                NeighborsCount = new int[maxWidth, maxHeight];
                // to the width and height of the file calculated above.

                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                //Create yPos variable
                int yPos = 0;

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
                                num++;
                            }
                            // If row[xPos] is a '.' (period) then
                            if (row[xPos] == '.')
                            {
                                // set the corresponding cell in the universe to dead.
                                universe[xPos, yPos] = false;
                            }
                            //this code gives the file neighbors so it isnt just gray cells
                            if (torodialToolStripMenuItem.Checked == true)
                            {
                                NextNeighborTorodial(xPos, yPos);
                            }
                            else
                            {
                                NextNeighborFinite(xPos, yPos);
                            }
                            //making sure the width matches the x
                            WidthNum = xPos + 1;
                        }
                        yPos++;
                        //making sure the height matches the y.
                        HeightNum = yPos;
                    }
                }
                //changes the number of alive.
                numberOfAlive = num;
                toolStripStatusAlive.Text = "Alive = " + numberOfAlive;
                graphicsPanel1.Invalidate();
                //Close the file.
                reader.Close();
            }
        }
        #endregion

        #region Check Changed
        //this code is to make sure when you click on torodial it unchecks finite
        private void Torodial_CheckedChanged(object sender, EventArgs e)
        {
            finiteToolStripMenuItem.Checked = !torodialToolStripMenuItem.Checked;
        }
        //this code is the opposite of the one above and unchecks torodial when finite is clicked 
        private void Finite_CheckedChanged(object sender, EventArgs e)
        {
            torodialToolStripMenuItem.Checked = !finiteToolStripMenuItem.Checked;
        }
        #endregion

        #region Miscellaneous Functions
        //this changes the HUDS boundary depending on what type is checked
        private Object boundaryType(string hello)
        {
            //if torodial is true the boundary type is torodial if not it is finite
            if (IsTorodial == true)
            {
                hello = "Torodial";
            }
            else if(IsTorodial == false)
            {
                hello = "Finite";
            }
            return hello;
        }
        //this code clears the screen and is used for new and other functions
        private void New()
        {
            //this goes through the grid and clears everything leaving it blank
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
        #endregion
    }
}
