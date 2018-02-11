using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;

namespace TileGame
{
    public partial class MainGameWindow : Form
    {
        Button[,] btn = new Button[30, 30];
        int scorePerSquare = 100;
        int playerScore = 0;
        int moveScore = 0;
        int timeLeft = 30;
        string playerName;

        Random r = new Random();

        public MainGameWindow()
        {
            InitializeComponent();
            this.Size = new Size(290, 210);
        }

        private void drawGrid()
        {
            for (int x = 0; x < btn.GetLength(0); x++)
            {
                for (int y = 0; y < btn.GetLength(1); y++)
                {
                    btn[x, y] = new Button();
                    btn[x, y].SetBounds((25 * x) + 10, (25 * y) + 50, 20, 20);

                    switch (r.Next(5))
                    {
                        case 0:
                            btn[x, y].BackColor = Color.Red;
                            break;
                        case 1:
                            btn[x, y].BackColor = Color.Orange;
                            break;
                        case 2:
                            btn[x, y].BackColor = Color.Yellow;
                            break;
                        case 3:
                            btn[x, y].BackColor = Color.Green;
                            break;
                        case 4:
                            btn[x, y].BackColor = Color.Blue;
                            break;
                    }

                    btn[x, y].Tag = x + "," + y;
                    btn[x, y].Click += new EventHandler(this.btnEvent_Click);
                    Controls.Add(btn[x, y]);
                }
            }
        }

        private void checkNeighbours(int x, int y, Color selectedColor)
        {
            if ((x >= 0 && x < btn.GetLength(0)) && (y >= 0 && y < btn.GetLength(1))) // keep recursion within bounds of grid
            {
                if (btn[x, y].BackColor == selectedColor)
                {
                    btn[x, y].BackColor = Color.Purple;
                    checkNeighbours((x - 1), y, selectedColor);
                    checkNeighbours((x + 1), y, selectedColor);
                    checkNeighbours(x, (y - 1), selectedColor);
                    checkNeighbours(x, (y + 1), selectedColor);
                    moveScore += scorePerSquare;
                    scorePerSquare += 100;
                }
            }
        }

        private void btnEvent_Click(object sender, EventArgs e)
        {
            string[] splitCoords = ((Button)sender).Tag.ToString().Split(',');
            int x = Convert.ToInt16(splitCoords[0]);
            int y = Convert.ToInt16(splitCoords[1]);

            string clickedBtnColor = btn[x, y].BackColor.ToString();


            if (((Button)sender).BackColor != Color.Purple)
            {
                moveScore = 0;
                scorePerSquare = 100;
                checkNeighbours(x, y, ((Button)sender).BackColor);
                playerScore += moveScore;
                Console.WriteLine("+" + moveScore + "pts");
                txtScore.Text = Convert.ToString(playerScore);
                ((Button)sender).BackColor = Color.Purple;

                replaceLinkedColours();
            }
        }

        private void replaceLinkedColours()
        {
            for (int i = 0; i < btn.GetLength(0); i++)
            {
                for (int j = 0; j < btn.GetLength(1); j++)
                {
                    if (btn[i, j].BackColor == Color.Purple)
                    {
                        switch (r.Next(5))
                        {
                            case 0:
                                btn[i, j].BackColor = Color.Red;
                                break;
                            case 1:
                                btn[i, j].BackColor = Color.Orange;
                                break;
                            case 2:
                                btn[i, j].BackColor = Color.Yellow;
                                break;
                            case 3:
                                btn[i, j].BackColor = Color.Green;
                                break;
                            case 4:
                                btn[i, j].BackColor = Color.Blue;
                                break;
                        }
                    }
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("Tom Roberts' badass tile game (c) 2014", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnEnterName_Click(object sender, EventArgs e)
        {
            drawGrid();
            this.Size = new Size(780, 850);
            
            lblScore.Visible = true;
            playerName = txtPlayerName.Text;
            lblScore.Text = playerName + "'s score:";
            txtScore.Visible = true;
            lblTimeCaption.Visible = true;
            lblTime.Visible = true;

            lblPlayerName.Visible = false;
            txtPlayerName.Visible = false;
            btnEnterName.Visible = false;

            lblTime.Text = timeLeft + " secs";
            tmrGame.Start();
        }

        private void tmrGame_Tick(object sender, EventArgs e)
        {
            if (timeLeft == 1)
            {
                lblTime.Text = timeLeft + " sec";
                timeLeft -= 1;
            }
            else if (timeLeft >= 0)
            {
                lblTime.Text = timeLeft + " secs";
                timeLeft -= 1;
            }
            else
            {
                tmrGame.Stop();
                lblTime.Text = "Out of time!";

                for (int i = 0; i < btn.GetLength(0); i++)
                {
                    for (int j = 0; j < btn.GetLength(1); j++)
                    {
                        btn[i, j].Enabled = false;
                    }
                }

                MessageBox.Show("You scored " + playerScore + " points. Well done, " + playerName + "!");
            }
        }

        private void howToPlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("Eliminate as many linked blocks of the same colour as you can within the time limit.\nYou will earn more points for linking large groups of blocks together.", "How To Play", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}