// Paste this code to replace everything following the line beginning "namespace"
{
    public partial class Form1 : Form
    {
        Button[,] btn = new Button[5, 5];

        Random r = new Random();
        
        public Form1()
        {
            InitializeComponent();
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    btn[x, y] = new Button();
                    btn[x, y].SetBounds(55 * x, 55 * y, 45, 45);
                    btn[x, y].BackColor = Color.PowderBlue; 
                    btn[x, y].Click += new EventHandler(this.btnEvent_Click);
                    Controls.Add(btn[x, y]);
                }
            }
            btn[r.Next(5), r.Next(5)].BackColor = Color.Blue;
        }

        void btnEvent_Click(object sender, EventArgs e)
        {
            if (((Button)sender).BackColor == Color.Blue)
            {
                ((Button)sender).BackColor = Color.PowderBlue;
                btn[r.Next(5), r.Next(5)].BackColor = Color.Blue;
                Console.WriteLine("WHACKED!");
            }
            else
            Console.WriteLine("Missed!");
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}