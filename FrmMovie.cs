using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieProjectTest
{
    public partial class FrmMovie : Form
    {
        //เชื่อมต่อกับฐานข้อมูล
        private string connectionString = "your_connection_string_here";

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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = tbMovieSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("กรุณาป้อนรหัสหรือชื่อภาพยนตร์", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SearchMovies(searchText);
        }

        private void SearchMovies(string searchText)
        {
            //lsMovieShow.Items.Clear();// เอาไว้เคลียร์ตอนจะกดค้นหาใหม่
            using (SqlConnection conn = new SqlConnection(connectionString))//
            {
                conn.Open();

                //คำสั่ง SQL สำหรับค้นหาภาพยนตร์โดยใช้รหัสตรงกัน หรือชื่อที่มีส่วนใดส่วนหนึ่งตรงกับคำค้นหา
                string query = "SELECT movie_id, movie_name FROM movie_tb WHERE movie_id = @searchText OR movie_name LIKE @searchPattern";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@searchText", searchText);
                    cmd.Parameters.AddWithValue("@searchPattern", "%" + searchText + "%");
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int index = 1;
                        while (reader.Read())
                        {
                            string movieName = reader["movie_name"].ToString();
                            lsMovieShow.Items.Add($"{index}. {movieName}");
                            index++;
                        }
                        if (index == 1)
                        {
                            MessageBox.Show("ไม่พบข้อมูลภาพยนตร์ที่ค้นหา", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

        private void tbMovieSearch_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
