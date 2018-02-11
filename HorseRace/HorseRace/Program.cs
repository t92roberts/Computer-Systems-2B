using System;
using System.Threading;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HorseRace
{
    static class Program
    {
        /// <summary> 
        /// Uses threads to run an animation independently of controls 
        /// </summary> 

        public class Animation : Form
        {
            private Panel controlPanel, drawingPanel;
            private Button pauseButton, clearButton, quitButton;
            private Thread DesertOrchid_t, RedRum_t;
            private Color backColor = Color.White;

            //variables 
            private int topX = 0, bottomX = 0;
            int won = -1;

            public Animation()
            {
                ClientSize = new Size(500, 400);
                SetupControlPanel();
                drawingPanel = new Panel();
                drawingPanel.Bounds = new Rectangle(controlPanel.Width, 0, ClientSize.Width - controlPanel.Width, ClientSize.Height);
                drawingPanel.BackColor = backColor;
                Controls.Add(drawingPanel);
                StartPosition = FormStartPosition.CenterScreen;

                ThreadStart drawStart_top = new ThreadStart(DesertOrchid);
                DesertOrchid_t = new Thread(drawStart_top);
                ThreadStart drawStart_bottom = new ThreadStart(RedRum);
                RedRum_t = new Thread(drawStart_bottom);
            }

            /// <summary> 
            /// Code for first horse 
            /// </summary> 
            private void RedRum()
            {
                Random random = new Random();
                Graphics g = drawingPanel.CreateGraphics();
                Pen pen = new Pen(Color.Black, 6);

                // Draw finish line
                g.DrawLine(pen, ClientSize.Width - 200, ClientSize.Height / 2, ClientSize.Width - 200, ClientSize.Height);

                while (true)
                {
                    pen = new Pen(Color.FromArgb(random.Next(100) + 155, 100, 100), 4);

                    if (this.topX > ClientSize.Width - 200 && this.won == -1)
                    {
                        this.won = 1;
                        Console.WriteLine("RedRum won");
                        clearButton.BackColor = Color.FromArgb(random.Next(100) + 155, 100, 100);
                    }

                    g.DrawLine(pen, this.bottomX, ClientSize.Height / 2, this.bottomX += 4, ClientSize.Height);

                    Thread.Sleep(random.Next(100) + 40);
                }
            }

            /// <summary> 
            /// Code for second horse 
            /// </summary> 
            private void DesertOrchid()
            {
                Random random = new Random();
                Graphics g = drawingPanel.CreateGraphics();
                Pen pen = new Pen(Color.Black, 6);
                // Draw finish line 
                g.DrawLine(pen, ClientSize.Width - 200, ClientSize.Height / 2, ClientSize.Width -
               200, ClientSize.Height);

                while (true)
                {
                    pen = new Pen(Color.FromArgb(100, random.Next(100) + 155, 100), 4);
                    if (this.topX > ClientSize.Width - 200 && this.won == -1)
                    {
                        this.won = 1;
                        Console.WriteLine("Desert Orchid won");
                        clearButton.BackColor = Color.FromArgb(100, random.Next(100) + 155, 100);
                    }
                    g.DrawLine(pen, this.topX, 0, this.topX += 4, ClientSize.Height / 2);

                    Thread.Sleep(random.Next(100) + 40);
                }
            }

            private void SetupControlPanel()
            {
                controlPanel = new Panel();
                controlPanel.Bounds = new Rectangle(0, 0, ClientSize.Width / 5, ClientSize.Height);
                controlPanel.BackColor = Color.White;
                this.Controls.Add(controlPanel);

                // Buttons 
                pauseButton = new Button();
                pauseButton.Text = "Start";
                pauseButton.Bounds = new Rectangle(8, 8, 85, 25);
                pauseButton.Click += new EventHandler(pauseButton_Click);
                controlPanel.Controls.Add(pauseButton);

                clearButton = new Button();
                clearButton.Text = "Clear";
                clearButton.Bounds = new Rectangle(8, pauseButton.Bounds.Y + pauseButton.Height + 5, 85, 25);
                clearButton.Click += new EventHandler(clearButton_Click);
                controlPanel.Controls.Add(clearButton);

                quitButton = new Button();
                quitButton.Text = "Quit";
                quitButton.Bounds = new Rectangle(8, clearButton.Bounds.Y + clearButton.Height + 5, 85, 25);
                quitButton.Click += new EventHandler(quitButton_Click);
                controlPanel.Controls.Add(quitButton);
            }

            private void clearButton_Click(object sender, EventArgs args)
            {
                Graphics g = drawingPanel.CreateGraphics();
                Brush brush = new SolidBrush(backColor);
                g.FillRectangle(brush, 0, 0, drawingPanel.Width, drawingPanel.Height);
                this.bottomX = 0;
                this.topX = 0;
                this.won = -1;
                clearButton.BackColor = Color.White;
                Pen pen = new Pen(Color.Black, 6);
                g.DrawLine(pen, ClientSize.Width - 200, 1, ClientSize.Width - 200, ClientSize.Height);
            }

            /// <summary> 
            /// Kills the thread, then exits the application. 
            /// </summary> 
            /// <param name="sender"></param> 
            /// <param name="args"></param> 
            private void quitButton_Click(object sender, EventArgs args)
            {
                if ((DesertOrchid_t.ThreadState & ThreadState.Suspended) != 0)
                    DesertOrchid_t.Resume();
                DesertOrchid_t.Abort();
                Application.Exit();
            }

            /// <summary> 
            /// Message handler for pauseButton. Starts the animation 
            /// the first time it is pressed. After that, it will toggle 
            /// between pausing and resuming the animation.
            /// </summary> 
            /// <param name="sender"></param> 
            /// <param name="args"></param> 
            private void pauseButton_Click(object sender, EventArgs args)
            {
                if (sender == pauseButton)
                {
                    if ((DesertOrchid_t.ThreadState & ThreadState.Suspended) != 0)
                    {
                        DesertOrchid_t.Resume();
                        pauseButton.Text = "Pause";
                    }
                    else if ((DesertOrchid_t.ThreadState & (ThreadState.Running |
                    ThreadState.WaitSleepJoin)) != 0)
                    {
                        DesertOrchid_t.Suspend();
                        pauseButton.Text = "Resume";
                    }
                    else if (DesertOrchid_t.ThreadState == ThreadState.Unstarted)
                    {
                        DesertOrchid_t.Start();
                        pauseButton.Text = "Pause";
                    }
                }

                if ((RedRum_t.ThreadState & ThreadState.Suspended) != 0)
                {
                    RedRum_t.Resume();
                    pauseButton.Text = "Pause";
                }
                else if ((RedRum_t.ThreadState & (ThreadState.Running |
                ThreadState.WaitSleepJoin)) != 0)
                {
                    RedRum_t.Suspend();
                    pauseButton.Text = "Resume";
                }
                else if (RedRum_t.ThreadState == ThreadState.Unstarted)
                {
                    RedRum_t.Start();
                    pauseButton.Text = "Pause";
                }

            }


            // Main - this code always at the end 
            public static void Main()
            {
                Application.Run(new Animation());
            }
        }
    }
}
