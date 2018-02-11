// Paste this code to replace everything following the line beginning "namespace"
{
    public partial class Form1 : Form
    {
        Button[,] btn = new Button[3,3];
        
        public Form1()
        {
            InitializeComponent();
            int digit = 1;
            for (int x = 0; x < btn.GetLength(0); x++)
            {
                for (int y = 0; y < btn.GetLength(1); y++)
                {
                    btn[x, y] = new Button();
                    btn[x, y].SetBounds(55 * x, 55* y, 45, 45);
                    btn[x, y].BackColor = Color.PowderBlue;
                    btn[x, y].Text = Convert.ToString(digit);
                    btn[x, y].Click += new EventHandler(this.btnEvent_Click);
                    Controls.Add(btn[x,y]);
                    digit++;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        void btnEvent_Click(object sender, EventArgs e)
        {
            Console.WriteLine(((Button)sender).Text);
        }
    }
}
