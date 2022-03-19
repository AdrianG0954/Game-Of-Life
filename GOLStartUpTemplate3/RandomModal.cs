using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOLStartUpTemplate3
{
    public partial class RandomModal : Form
    {
        public RandomModal()
        {
            InitializeComponent();
        }

        public int RandomSeed
        {
            get
            {
                return (int)numericUpDown1.Value;
            }

            set
            {
                numericUpDown1.Value = value;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Random rand = new Random();

            numericUpDown1.Value = rand.Next(-999999,999999);
        }
    }
}
