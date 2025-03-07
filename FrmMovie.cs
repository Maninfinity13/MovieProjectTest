using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieProjectTest
{
    public partial class FrmMovie : Form
    {
        public FrmMovie()
        {
            InitializeComponent();
        }






        //ปุ่ม Exit
        private void btExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("คุณต้องการออกจากโปรแกรมหรือไม่?...","ยืนยันการออก",MessageBoxButtons.YesNo,MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
