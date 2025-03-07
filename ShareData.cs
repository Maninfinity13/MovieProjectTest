using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieProjectTest
{
    internal class ShareData
    {
        
        public static void showWarningMSG(string msg)
        {
            MessageBox.Show(msg, "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        //สร้างตัวแปรเก็บ Connection String ที่ใช้ติดต่อกับ DB
        
    }
}
