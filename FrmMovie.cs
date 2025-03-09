using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
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

        private void ResetUI()
        {
            // ล้างค่าใน GroupBox "ข้อมูลภาพยนตร์"
            lbMovieId.Text = "";
            tbMovieName.Text = "";
            tbMovieDetail.Text = "";
            dtpMovieDateSale.Value = DateTime.Now;
            nudMovieHour.Value = 0;
            nudMovieMinute.Value = 0;
            cbbMovieType.SelectedIndex = -1;
            tbMovieDVDTotal.Text = "";
            tbMovieDVDPrice.Text = "";
            pcbMovieImg.Image = null;
            pcbDirMovie.Image = null;

            // ปิดการใช้งานช่องป้อนข้อมูล
            tbMovieName.Enabled = false;
            tbMovieDetail.Enabled = false;
            dtpMovieDateSale.Enabled = false;
            nudMovieHour.Enabled = false;
            nudMovieMinute.Enabled = false;
            cbbMovieType.Enabled = false;
            tbMovieDVDTotal.Enabled = false;
            tbMovieDVDPrice.Enabled = false;
            btSelectImg1.Enabled = false;
            btSelectImg2.Enabled = false;

            // ปิดปุ่ม Edit และ Delete
            btEdit.Enabled = false;
            btDel.Enabled = false;

            // เปิดปุ่ม Add
            btAdd.Enabled = true;
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
            string strSql = "SELECT movieId, movieName, movieDetail, movieDateSale, movieTypeId FROM movie_tb " ;

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
        private void LoadMovieTypes()
        {
            // สร้างการเชื่อมต่อกับฐานข้อมูล
            string connectionString = "Server=DESKTOP-ILU10GQ\\SQLEXPRESS;Database=movie_record_db;Trusted_connection=True"; 
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    // เปิดการเชื่อมต่อ
                    conn.Open();

                    // สร้างคำสั่ง SQL เพื่อดึงข้อมูลหมวดหมู่
                    string query = "SELECT movieTypeName FROM movie_type_tb"; 
                    SqlCommand cmd = new SqlCommand(query, conn);

                    // ดึงข้อมูลจากฐานข้อมูล
                    SqlDataReader reader = cmd.ExecuteReader();

                    // ล้างข้อมูลเก่าที่มีอยู่ใน ComboBox ก่อน
                    cbbMovieType.Items.Clear();

                    // เติมข้อมูลหมวดหมู่ที่ได้จากฐานข้อมูลลงใน ComboBox
                    while (reader.Read())
                    {
                        cbbMovieType.Items.Add(reader["movieTypeName"].ToString()); // เพิ่มข้อมูลจากฐานข้อมูลลงใน ComboBox
                    }

                    // ปิดการเชื่อมต่อ
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
                }
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
            // รีเซ็ต UI ทุกอย่างเมื่อเริ่มต้น
            ResetUI();

            getMovieFromDBToDGV();
            LoadMovieTypes();  // โหลดข้อมูลหมวดหมู่ภาพยนตร์เมื่อฟอร์มโหลด
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

        private void btSelectImg1_Click(object sender, EventArgs e)
        {
            // สร้าง OpenFileDialog เพื่อให้ผู้ใช้เลือกไฟล์รูป
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ไฟล์รูปภาพ |*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            openFileDialog.Title = "เลือกภาพตัวอย่างภาพยนตร์";

            // ถ้าผู้ใช้เลือกไฟล์และกด OK
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // นำรูปไปแสดงใน PictureBox
                pcbMovieImg.Image = new Bitmap(openFileDialog.FileName);

                // บันทึกพาธของไฟล์รูปที่เลือก (ถ้าต้องการเก็บพาธเพื่อบันทึกลงฐานข้อมูล)
                pcbMovieImg.Tag = openFileDialog.FileName;
            }
        }

        private void btSelectImg2_Click(object sender, EventArgs e)
        {
            // สร้าง OpenFileDialog เพื่อให้ผู้ใช้เลือกไฟล์รูป
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ไฟล์รูปภาพ |*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            openFileDialog.Title = "เลือกภาพตัวอย่างภาพยนตร์";

            // ถ้าผู้ใช้เลือกไฟล์และกด OK
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // นำรูปไปแสดงใน PictureBox
                pcbDirMovie.Image = new Bitmap(openFileDialog.FileName);

                // บันทึกพาธของไฟล์รูปที่เลือก (ถ้าต้องการเก็บพาธเพื่อบันทึกลงฐานข้อมูล)
                pcbDirMovie.Tag = openFileDialog.FileName;
            }
        }

        private void tbMovieDVDTotal_TextChanged(object sender, EventArgs e)
        {

        }

        private void lsMovieShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ตรวจสอบว่ามีการเลือกข้อมูลใน lsMovieShow หรือไม่
            if (lsMovieShow.SelectedItems.Count == 0)
                return;

            // ดึงค่าจากรายการที่ถูกเลือก
            string selectedMovieId = lsMovieShow.SelectedItems[0].SubItems[0].Text; // ดึงรหัสภาพยนตร์

            // ติดต่อฐานข้อมูลเพื่อดึงข้อมูลของภาพยนตร์ที่เลือก
            string query = "SELECT * FROM movie_tb WHERE movieId = @movieId";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@movieId", selectedMovieId);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read()) // ถ้าพบข้อมูล
                {
                    lbMovieId.Text = reader["movieId"].ToString(); // รหัสภาพยนตร์
                    tbMovieName.Text = reader["movieName"].ToString(); // ชื่อภาพยนตร์
                    tbMovieDetail.Text = reader["movieDetail"].ToString(); // รายละเอียด
                    dtpMovieDateSale.Value = Convert.ToDateTime(reader["movieDateSale"]); // วันที่วางขาย
                    nudMovieHour.Value = Convert.ToInt32(reader["movieHour"]); // ความยาว (ชั่วโมง)
                    nudMovieMinute.Value = Convert.ToInt32(reader["movieMinute"]); // ความยาว (นาที)
                    cbbMovieType.SelectedValue = reader["movieTypeId"]; // หมวดหมู่
                    tbMovieDVDTotal.Text = reader["movieDVDTotal"].ToString(); // จำนวน DVD
                    tbMovieDVDPrice.Text = reader["movieDVDPrice"].ToString(); // ราคา DVD

                    // โหลดรูปภาพจากฐานข้อมูล (ถ้ามี)
                    if (reader["movieImage"] != DBNull.Value)
                    {
                        byte[] imgBytes = (byte[])reader["movieImage"];
                        using (MemoryStream ms = new MemoryStream(imgBytes))
                        {
                            pcbMovieImg.Image = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        pcbMovieImg.Image = null;
                    }

                    if (reader["directorImage"] != DBNull.Value)
                    {
                        byte[] dirImgBytes = (byte[])reader["directorImage"];
                        using (MemoryStream ms = new MemoryStream(dirImgBytes))
                        {
                            pcbDirMovie.Image = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        pcbDirMovie.Image = null;
                    }
                }
                reader.Close();
            }

            // ปรับสถานะปุ่ม
            btAdd.Enabled = false;   // ปิดปุ่ม "เพิ่ม"
            btEdit.Enabled = true;   // เปิดปุ่ม "แก้ไข"
            btDel.Enabled = true;    // เปิดปุ่ม "ลบ"
        }

        private void btDel_Click(object sender, EventArgs e)
        {
            // ตรวจสอบว่ามีรหัสภาพยนตร์ที่ต้องการลบหรือไม่
            if (string.IsNullOrEmpty(lbMovieId.Text))
            {
                MessageBox.Show("กรุณาเลือกภาพยนตร์ที่ต้องการลบ!", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // แสดงกล่องยืนยัน
            DialogResult result = MessageBox.Show("คุณต้องการลบภาพยนตร์นี้หรือไม่?", "ยืนยันการลบ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                return; // ถ้ากด No ให้หยุดทำงานตรงนี้
            }

            // ดำเนินการลบข้อมูล
            string movieIdToDelete = lbMovieId.Text;
            string query = "DELETE FROM movie_tb WHERE movieId = @movieId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@movieId", movieIdToDelete);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("ลบข้อมูลภาพยนตร์สำเร็จ!", "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("ไม่พบข้อมูลที่ต้องการลบ!", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // รีเฟรช DataGridView แสดงข้อมูลล่าสุด
            getMovieFromDBToDGV();

            // รีเซ็ต UI ให้เหมือนตอนเปิดหน้าจอ
            ResetUI();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            // เรียกใช้ฟังก์ชัน ResetUI() เพื่อล้างค่าทั้งหมด
            ResetUI();
        }

        private void btEdit_Click(object sender, EventArgs e)
        {
            // ปลดล็อกช่องป้อนข้อมูลทั้งหมด
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

            // ปิดปุ่มแก้ไข
            btEdit.Enabled = false;

            // เปิดปุ่มบันทึกและยกเลิก
            btSaveAddEdit.Enabled = true;
            btCancel.Enabled = true;
        }

        private void btMovieSearch_Click(object sender, EventArgs e)
        {
            // ตรวจสอบว่า ผู้ใช้ป้อนข้อมูลอะไรหรือไม่
            string searchKeyword = tbMovieSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchKeyword))
            {
                MessageBox.Show("กรุณาป้อนรหัสหรือชื่อภาพยนตร์เพื่อค้นหา", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // หยุดการทำงานหากไม่ได้ป้อนข้อมูล
            }

            // สร้างการเชื่อมต่อกับฐานข้อมูล
            string connectionString = "Server=DESKTOP-ILU10GQ\\SQLEXPRESS;Database=movie_record_db;Trusted_connection=True"; 
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "";
                    if (rdMovieId.Checked) // หากค้นหาตามรหัส
                    {
                        query = "SELECT * FROM movie_tb WHERE movieId = @movieId";
                    }
                    else if (rdMovieName.Checked) // หากค้นหาตามชื่อ
                    {
                        query = "SELECT * FROM movie_tb WHERE movieName LIKE '%' + @movieName + '%'";
                    }

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@movieId", searchKeyword);
                    cmd.Parameters.AddWithValue("@movieName", searchKeyword);

                    SqlDataReader reader = cmd.ExecuteReader();

                    // ล้างข้อมูลเก่าใน ListBox ก่อน
                    lsMovieShow.Items.Clear();

                    int index = 1; // ใช้สำหรับการแสดงลำดับ
                    while (reader.Read())
                    {
                        // สร้างรายการใหม่ใน ListBox โดยมีหมายเลขลำดับกำกับ
                        string movieInfo = $"{index}. {reader["movieName"].ToString()}"; // หรือข้อมูลอื่นๆ ที่ต้องการแสดง
                        lsMovieShow.Items.Add(movieInfo);
                        index++; // เพิ่มลำดับ
                    }

                    // ปิดการเชื่อมต่อ
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
                }
            }
        }

        private void dgvMovieShowAll_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
