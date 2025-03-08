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
        private string connectionString = "Server=DESKTOP-ILU10GQ\\SQLEXPRESS;Database=movie_record_db;Trusted_connection=True";

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

        }
        private void getMovieFromDBToDGV()
        {
            //ติดต่อ DB
            SqlConnection conn = new SqlConnection(connectionString);
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();

            //คำสั่ง SQL
            string strSql = "SELECT movieId, movieName, movieDetail, movieDateSale, movieTypeId FROM movie_tb " +
                            "WHERE movieId = @movieId";

            //สร้าง SQL Transaction และ SQL Command เพื่อทำงานกับคำสั่ง SQL
            SqlTransaction sqlTransaction = conn.BeginTransaction();
            SqlCommand sqlCommand = new SqlCommand();
            sqlCommand.Connection = conn;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.CommandText = strSql;
            sqlCommand.Transaction = sqlTransaction;

            //Bind param เพื่อกำหนดข้อมูลให้กับ SQL Paramiter
            //sqlCommand.Parameters.AddWithValue("@travellerId", SharedInfo.travellerId);*******************

            //สั่งให้ SQL
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            //เอาข้อมูลใน DataTable ไปแสดงใน DGV
            if (dt.Rows.Count > 0)
            {
                //ปรบความสูงของแถว DGV
                dgvMovieShowAll.RowTemplate.Height = 50;
                //กรณีมี จะนำข้อมูลมาแสดง
                dgvMovieShowAll.DataSource = dt;
               
        

                //ปรับรูปให้พอดีกับความสูง
                DataGridViewImageColumn imgCol = (DataGridViewImageColumn)dgvMovieShowAll.Columns[2];
                imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            }
            else
            {
                //กรณีไม่มีให้แสดงแค่หัวคอลัมน์ของตารางใน DGV
            }

        }



        private void tbMovieSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private string GenerateNewMovieId()
        {
            string newMovieId = "mv001"; // ค่าพื้นฐานกรณีไม่มีข้อมูลในฐานข้อมูล
            string query = "SELECT TOP 1 movieId FROM movie_tb ORDER BY movieId DESC";

            using (SqlConnection conn = new SqlConnection( connectionString ))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        string lastId = result.ToString(); // ได้ค่าเช่น "mv028"
                        int number = int.Parse(lastId.Substring(2)) + 1; // ตัด "mv" ออก แล้วแปลงเป็น int +1
                        newMovieId = $"mv{number:D3}"; // แปลงกลับเป็นรูปแบบ mvNNN
                    }
                }
            }

            return newMovieId;
        }



        private void btAdd_Click(object sender, EventArgs e)
        {
            // ดึงรหัสภาพยนตร์ใหม่และแสดงที่ lbMovieId
            lbMovieId.Text = GenerateNewMovieId();

            // ปลดล็อกการป้อนข้อมูล
            tbMovieName.Enabled = true;
            tbMovieDetail.Enabled = true;
            dtpMovieDateSale.Enabled = true;
            nudMovieHour.Enabled = true;
            nudMovieMinute.Enabled = true;
            cbbMovieType.Enabled = true;
            tbMovieDVDTotal.Enabled = true;
            tbMovieDVDPrice.Enabled = true;
            btSelectImg1.Enabled = true;
            btSelectImg2.Enabled = true;

            // ปิดปุ่มเพิ่ม และเปิดปุ่มบันทึก
            btAdd.Enabled = false;
            btSaveAddEdit.Enabled = true;
        }

        private void nudMovieMinute_ValueChanged(object sender, EventArgs e)
        {

        }

        private void FrmMovie_Load(object sender, EventArgs e)
        {

        }

        private void btSaveAddEdit_Click(object sender, EventArgs e)
        {
            //Validate Data
            if (tbMovieName.Text.Trim().Length == 0)
            {
                //SharedInfo.showWarningMSG("ป้อนสถานที่ไปด้วย")
                MessageBox.Show("ป้อนชื่อภาพยนต์ด้วย", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (tbMovieDetail.Text.Trim().Length == 0)
            {
                MessageBox.Show("ป้อนรายละเอียดภาพยนต์ด้วย", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (dtpMovieDateSale.Value.Date < DateTime.Now.Date)
            {
                MessageBox.Show("วันที่วางขายควรจะน้อยกว่าวันปัจจุบัน", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (pcbMovieImg == null)
            {
                MessageBox.Show("ใส่รูปภาพด้วย", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (pcbDirMovie == null)
            {
                MessageBox.Show("ใส่รูปภาพกำกับภาพยนต์ด้วย", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (nudMovieMinute.Value < 30)
            {
                MessageBox.Show("ภาพยนต์ไม่ควรมีระยะเวลาต่ำกว่า 30 นาที", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (cbbMovieType.SelectedIndex == -1)
            {
                MessageBox.Show("กรุณาเลือกหมวดหมู่ภาพยนตร์", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (string.IsNullOrEmpty(tbMovieDVDTotal.Text) || Convert.ToInt32(tbMovieDVDTotal.Text) == 0)
            {
                MessageBox.Show("กรุณาใส่จำนวนของ DVD ด้วย", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (string.IsNullOrEmpty(tbMovieDVDPrice.Text) || Convert.ToDecimal(tbMovieDVDPrice.Text) == 0)
            {
                MessageBox.Show("กรุณาใส่ราคาของ DVD fh;p", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                //ส่งข้อมูลไปที่ DB
                //ติดต่อ DB

                SqlConnection conn = new SqlConnection(connectionString);
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Open();

                //คำสั่ง SQL
                string strSql = "INSERT INTO movie_tb " +
                                "(movieName, movieDetail, movieDateSale, movieLenghtHour, movieLenghtMinute, movieTypeId,movieDVDTotal,movieDVDPrice) " +
                                "VALUES " +
                                "(@movieName, @movieDetail, @movieDateSale, @movieLenghtHour, @movieLenghtMinute, @movieTypeId,@movieDVDTotal,@movieDVDPrice) ";

                //สร้าง SQL Transaction และ SQL Command เพื่อทำงานกับคำสั่ง SQL
                SqlTransaction sqlTransaction = conn.BeginTransaction();
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = conn;
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = strSql;
                sqlCommand.Transaction = sqlTransaction;

                //Bind param เพื่อกำหนดข้อมูลให้กับ SQL Paramiter
                sqlCommand.Parameters.AddWithValue("@movieName", tbMovieName.Text.Trim());
                sqlCommand.Parameters.AddWithValue("@movieDetail", tbMovieDetail.Text.Trim());
                sqlCommand.Parameters.AddWithValue("@movieDateSale", dtpMovieDateSale.Value.Date);
                sqlCommand.Parameters.AddWithValue("@movieDateSale", dtpMovieDateSale.Value.Date);
                sqlCommand.Parameters.AddWithValue("@movieLengthHour", (int)nudMovieHour.Value);
                sqlCommand.Parameters.AddWithValue("@movieLengthMinute", (int)nudMovieMinute.Value);
                sqlCommand.Parameters.AddWithValue("@movieType", cbbMovieType.SelectedItem.ToString());
                sqlCommand.Parameters.AddWithValue("@movieDVDTotal", float.Parse(tbMovieDVDTotal.Text.Trim()));
                sqlCommand.Parameters.AddWithValue("@movieDVDPrice", float.Parse(tbMovieDVDPrice.Text.Trim()));
                sqlCommand.Parameters.AddWithValue("@movieImg", pcbMovieImg);
                sqlCommand.Parameters.AddWithValue("@movieDirImg", pcbDirMovie);
                //sqlCommand.Parameters.AddWithValue("@travellerId", ShareInfo.travellerId); **************

                //สั่งให้ SQL ทำงาน
                try
                {
                    sqlCommand.ExecuteNonQuery();
                    sqlTransaction.Commit();
                    conn.Close();

                    MessageBox.Show("บันทึกจ้อมูลภาพยนต์สำเร็จ", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Dispose();//ปิด dialog เพราะเปิดแบบ showDialog
                    //FrmLogin frmLogin = new FrmLogin();
                    //frmLogin.Show();
                    //Hide();
                }
                catch (Exception ex)
                {
                    sqlTransaction.Rollback();
                    conn.Close();

                    MessageBox.Show("มีข้อผิดพลาดเกิดขึ้น กรุณาลองใหม่...", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void cbbMovieType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
