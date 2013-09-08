using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dataPump2.det
{
    public partial class Form1 : Form
    {
        int i = 1;
        public void set_tab()
        {
            switch (i)
            {
                case 1:
                    this.panel1.Visible = true;
                    this.panel2.Visible = false;
                    this.panel3.Visible = false;
                    this.panel4.Visible = false;
                    break;
                case 2:
                    this.panel1.Visible = false;
                    this.panel2.Visible = true;
                    this.panel3.Visible = false;
                    this.panel4.Visible = false;
                    break;
                case 3:
                    this.panel1.Visible = false;
                    this.panel2.Visible = false;
                    this.panel3.Visible = true;
                    this.panel4.Visible = false;
                    break;
                case 4:
                    this.panel1.Visible = false;
                    this.panel2.Visible = false;
                    this.panel3.Visible = false;
                    this.panel4.Visible = true;
                    break;
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            i++;
            set_tab();
            this.timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (i)
            {
                case 1:
                    this.tableLayoutPanel1.ColumnStyles[0].Width+=10;
                    if (this.tableLayoutPanel1.ColumnStyles[0].Width >= 90)
                    {
                        this.tableLayoutPanel1.ColumnStyles[0].Width = 100;
                        this.tableLayoutPanel1.ColumnStyles[1].Width = 0;
                        this.timer1.Enabled = false;
                    }
                    break;
                case 2:
                    this.tableLayoutPanel1.ColumnStyles[1].Width += 10;
                    if (this.tableLayoutPanel1.ColumnStyles[1].Width >= 90)
                    {
                        this.tableLayoutPanel1.ColumnStyles[1].Width = 100;
                        this.tableLayoutPanel1.ColumnStyles[0].Width = 0;
                        this.tableLayoutPanel1.ColumnStyles[2].Width = 0;
                        this.timer1.Enabled = false;
                    }
                    break;
                case 3:
                    this.tableLayoutPanel1.ColumnStyles[2].Width += 10;
                    if (this.tableLayoutPanel1.ColumnStyles[2].Width >= 90)
                    {
                        this.tableLayoutPanel1.ColumnStyles[2].Width = 100;
                        this.tableLayoutPanel1.ColumnStyles[1].Width = 0;
                        this.tableLayoutPanel1.ColumnStyles[3].Width = 0;
                        this.timer1.Enabled = false;
                    }
                    break;
                case 4:
                    this.tableLayoutPanel1.ColumnStyles[3].Width += 10;
                    if (this.tableLayoutPanel1.ColumnStyles[3].Width >= 90)
                    {
                        this.tableLayoutPanel1.ColumnStyles[3].Width = 100;
                        this.tableLayoutPanel1.ColumnStyles[2].Width = 0;
                        this.timer1.Enabled = false;
                    }
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.tableLayoutPanel1.ColumnStyles[0].Width = 100;
            this.tableLayoutPanel1.ColumnStyles[1].Width = 0;
            this.tableLayoutPanel1.ColumnStyles[2].Width = 0;
            this.tableLayoutPanel1.ColumnStyles[3].Width = 0;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            i--;
            set_tab();
            this.timer1.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }       

    }
}
