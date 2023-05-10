using System.Data;
using MySql.Data.MySqlClient;
using System.Runtime.InteropServices;
using RJCodeAdvance.RJControls;

namespace Attendance_Management_System
{
    public partial class Form1 : Form
    {
        // Making class variables to be used across
        private int borderSize = 2;
        private Size formSize;
        public int t_id;
        public bool isAdmin = false;

        public Form1(int t_id, bool isAdmin)
        {
            this.t_id = t_id;
            this.isAdmin = isAdmin;

            InitializeComponent();
            CollapseMenu();
            CreateCourseMenu();
            UpdateTeacherLabel();

            if (isAdmin)
            {
                MakeAdminOptions();
            }

            this.Padding = new Padding(borderSize);
            this.BackColor = Color.FromArgb(98, 102, 244);
        }

        // Method to enable buttons only admin can see
        private void MakeAdminOptions()
        {
            addButton.Visible = true;
        }

        // Method to update the value of the label with the teachers thats logged in
        private void UpdateTeacherLabel()
        {
            string connectionString = "server=127.0.0.1;uid=root;pwd=root;database=attendance";
            string query = "SELECT name FROM teachers WHERE id=@t_id";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@t_Id", t_id);

                string name = cmd.ExecuteScalar().ToString()!;

                teacherLabel.Text = isAdmin ? $"Welcome {name} (Admin)" : $"Welcome {name}";
            }
        }

        // Method to fetch courses for the teacher and add it to the dropdown in sidebar
        private void CreateCourseMenu()
        {
            string connectionString = "server=127.0.0.1;uid=root;pwd=root;database=attendance";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT code, name FROM courses WHERE t_id=@t_id;", conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@t_id", t_id);

                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string code = reader["code"].ToString()!;
                        string name = reader["name"].ToString()!;

                        ToolStripMenuItem toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                        coursesToolStripMenuItem.DropDownItems.Add(toolStripMenuItem);

                        toolStripMenuItem.Name = code;
                        toolStripMenuItem.Size = new System.Drawing.Size(224, 26);
                        toolStripMenuItem.Text = $"{code} - {name}";
                        toolStripMenuItem.Tag = code;
                        toolStripMenuItem.Click += new System.EventHandler((object? sender, EventArgs e) =>
                        {
                            MakeAttendanceTableMenu(sender!, e, code);
                        }
                        );

                    }

                }
            }

        }

        // Intercept the windows proc to enable dragging of window without titlebar
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        // Method to fetch attendance data and return a DataTable filled with it
        public DataTable MakeAttendanceTable(int c_id)
        {

            DataTable dt = new DataTable();
            string connectionString = "server=127.0.0.1;uid=root;pwd=root;database=attendance";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("SELECT students.id, students.name FROM courses INNER JOIN students ON courses.id=students.c_id WHERE courses.id=@c_id;", conn))
                {
                    cmd.Parameters.AddWithValue("@c_id", c_id);
                    conn.Open();

                    MySqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                }
            }
            var p = new DataColumn();
            p.DataType = System.Type.GetType("System.Boolean");
            p.ColumnName = "Attendance";
            p.ReadOnly = false;
            p.DefaultValue = false;

            dt.Columns.Add(p);

            Console.WriteLine(dt);


            return dt;
        }

        // Method to insert the marked attendance onto the database
        private void AddAttendance(DataGridView dataGridView, int c_id)
        {
            var todayDate = DateTime.Today.ToString("yyyy-MM-dd");
            int totalStudents = dataGridView.Rows.Count;

            string connectionString = "server=127.0.0.1;uid=root;pwd=root;database=attendance";
            string query = "INSERT INTO attendance (s_id, `date`, c_id) VALUES (@s_id, @date, @c_id)";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                for (int i = 1; i <= totalStudents; i++)
                {
                    bool isPresent = Convert.ToBoolean(dataGridView.Rows[i - 1].Cells["Attendance"].Value);

                    if (isPresent)
                    {
                        string name = dataGridView.Rows[i - 1].Cells["name"].Value.ToString()!;
                        string search_query = "SELECT id FROM students WHERE name=@name";
                        var search_cmd = new MySqlCommand(search_query, conn);
                        search_cmd.Parameters.AddWithValue("@name", name);

                        int s_id = (int)search_cmd.ExecuteScalar();

                        var cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@s_id", s_id);
                        cmd.Parameters.AddWithValue("@date", todayDate);
                        cmd.Parameters.AddWithValue("@c_id", c_id);

                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }

            }

        }

        // Method to open the dropdown menu
        private void iconButton3_Click(object sender, EventArgs e)
        {
            Open_DropdownMenu(rjDropdownMenu1, sender);
        }

        // Method to handle minimize event of the window
        private void iconButton7_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                btnMaximize.IconChar = FontAwesome.Sharp.IconChar.WindowRestore;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                btnMaximize.IconChar = FontAwesome.Sharp.IconChar.WindowMaximize;
            }
        }

        // Method to handle drag of window
        private void panelTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        // Intercept the proc message
        // protected override void WndProc(ref Message m){
        //     const int WM_NCCALCSIZE = 0x0083;
        //     if (m.Msg == WM_NCCALCSIZE && m.WParam.ToInt32() == 1){
        //         return;
        //     }
        //     base.WndProc(ref m);
        // }

        // Method to resize the form correctly
        private void Form1_Resize(object sender, EventArgs e)
        {
            AdjustForm(); // Refactoring
        }

        // Method to manage the window padding by removing and adding extra padding based on window state
        private void AdjustForm()
        {
            switch (this.WindowState)
            {
                case FormWindowState.Maximized:
                    this.Padding = new Padding(8, 8, 8, 0);

                    break;

                case FormWindowState.Normal:
                    if (this.Padding.Top != borderSize)
                    {
                        this.Padding = new Padding(borderSize);
                    }

                    break;
            }
        }

        // Method to handle window minimize
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        // Method to handle window close
        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Method to hide the sidebar
        private void btnMenu_Click(object sender, EventArgs e)
        {
            CollapseMenu(); // Refactoring
        }

        // Method to reduce the width of the sidebar and make it closed
        private void CollapseMenu()
        {
            if (this.panelMenu.Width > 200)
            {
                panelMenu.Width = 100;
                pictureBox1.Visible = false;
                btnMenu.Dock = DockStyle.Top;

                foreach (Button menuButton in this.panelMenu.Controls.OfType<Button>())
                {
                    menuButton.Text = "";
                    menuButton.ImageAlign = ContentAlignment.MiddleCenter;
                    menuButton.Padding = new Padding(0);
                }
            }
            else
            {
                panelMenu.Width = 230;
                pictureBox1.Visible = true;
                btnMenu.Dock = DockStyle.None;

                foreach (Button menuButton in this.panelMenu.Controls.OfType<Button>())
                {
                    menuButton.Text = "    " + menuButton.Tag.ToString();
                    menuButton.ImageAlign = ContentAlignment.MiddleLeft;
                    menuButton.Padding = new Padding(10, 0, 0, 0);
                }

            }
        }

        // Method to open the dropdown
        private void Open_DropdownMenu(RJDropdownMenu dropDownMenu, object sender)
        {
            Control control = (Control)sender;
            dropDownMenu.VisibleChanged += new EventHandler((sender2, ev) => DropdownMenu_VisibleChanged(sender2!, ev, control)
            );
            dropDownMenu.Show(control, control.Width, 0);
        }

        // Method to change the dropdown background
        private void DropdownMenu_VisibleChanged(object sender, EventArgs e, Control control)
        {
            RJDropdownMenu dropdownMenu = (RJDropdownMenu)sender;
            if (!DesignMode)
            {
                if (dropdownMenu.Visible)
                {
                    control.BackColor = Color.FromArgb(159, 161, 224);
                }
                else
                {
                    control.BackColor = Color.FromArgb(18, 23, 64);
                }
            }
        }

        // Overridden methods to remove the window titlebar and give snap abilities back
        protected override void WndProc(ref Message m)
        {
            const int WM_NCCALCSIZE = 0x0083;//Standar Title Bar - Snap Window
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MINIMIZE = 0xF020; //Minimize form (Before)
            const int SC_RESTORE = 0xF120; //Restore form (Before)
            const int WM_NCHITTEST = 0x0084;//Win32, Mouse Input Notification: Determine what part of the window corresponds to a point, allows to resize the form.
            const int resizeAreaSize = 10;

            #region Form Resize
            // Resize/WM_NCHITTEST values
            const int HTCLIENT = 1; //Represents the client area of the window
            const int HTLEFT = 10;  //Left border of a window, allows resize horizontally to the left
            const int HTRIGHT = 11; //Right border of a window, allows resize horizontally to the right
            const int HTTOP = 12;   //Upper-horizontal border of a window, allows resize vertically up
            const int HTTOPLEFT = 13;//Upper-left corner of a window border, allows resize diagonally to the left
            const int HTTOPRIGHT = 14;//Upper-right corner of a window border, allows resize diagonally to the right
            const int HTBOTTOM = 15; //Lower-horizontal border of a window, allows resize vertically down
            const int HTBOTTOMLEFT = 16;//Lower-left corner of a window border, allows resize diagonally to the left
            const int HTBOTTOMRIGHT = 17;//Lower-right corner of a window border, allows resize diagonally to the right

            ///<Doc> More Information: https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-nchittest </Doc>

            if (m.Msg == WM_NCHITTEST)
            { //If the windows m is WM_NCHITTEST
                base.WndProc(ref m);
                if (this.WindowState == FormWindowState.Normal)//Resize the form if it is in normal state
                {
                    if ((int)m.Result == HTCLIENT)//If the result of the m (mouse pointer) is in the client area of the window
                    {
                        Point screenPoint = new Point(m.LParam.ToInt32()); //Gets screen point coordinates(X and Y coordinate of the pointer)                           
                        Point clientPoint = this.PointToClient(screenPoint); //Computes the location of the screen point into client coordinates                          

                        if (clientPoint.Y <= resizeAreaSize)//If the pointer is at the top of the form (within the resize area- X coordinate)
                        {
                            if (clientPoint.X <= resizeAreaSize) //If the pointer is at the coordinate X=0 or less than the resizing area(X=10) in 
                                m.Result = (IntPtr)HTTOPLEFT; //Resize diagonally to the left
                            else if (clientPoint.X < (this.Size.Width - resizeAreaSize))//If the pointer is at the coordinate X=11 or less than the width of the form(X=Form.Width-resizeArea)
                                m.Result = (IntPtr)HTTOP; //Resize vertically up
                            else //Resize diagonally to the right
                                m.Result = (IntPtr)HTTOPRIGHT;
                        }
                        else if (clientPoint.Y <= (this.Size.Height - resizeAreaSize)) //If the pointer is inside the form at the Y coordinate(discounting the resize area size)
                        {
                            if (clientPoint.X <= resizeAreaSize)//Resize horizontally to the left
                                m.Result = (IntPtr)HTLEFT;
                            else if (clientPoint.X > (this.Width - resizeAreaSize))//Resize horizontally to the right
                                m.Result = (IntPtr)HTRIGHT;
                        }
                        else
                        {
                            if (clientPoint.X <= resizeAreaSize)//Resize diagonally to the left
                                m.Result = (IntPtr)HTBOTTOMLEFT;
                            else if (clientPoint.X < (this.Size.Width - resizeAreaSize)) //Resize vertically down
                                m.Result = (IntPtr)HTBOTTOM;
                            else //Resize diagonally to the right
                                m.Result = (IntPtr)HTBOTTOMRIGHT;
                        }
                    }
                }
                return;
            }
            #endregion

            //Remove border and keep snap window
            if (m.Msg == WM_NCCALCSIZE && m.WParam.ToInt32() == 1)
            {
                return;
            }

            //Keep form size when it is minimized and restored. Since the form is resized because it takes into account the size of the title bar and borders.
            if (m.Msg == WM_SYSCOMMAND)
            {
                /// <see cref="https://docs.microsoft.com/en-us/windows/win32/menurc/wm-syscommand"/>
                /// Quote:
                /// In WM_SYSCOMMAND messages, the four low - order bits of the wParam parameter 
                /// are used internally by the system.To obtain the correct result when testing 
                /// the value of wParam, an application must combine the value 0xFFF0 with the 
                /// wParam value by using the bitwise AND operator.
                int wParam = (m.WParam.ToInt32() & 0xFFF0);

                if (wParam == SC_MINIMIZE)  //Before
                    formSize = this.ClientSize;
                if (wParam == SC_RESTORE)// Restored form(Before)
                    this.Size = formSize;
            }
            base.WndProc(ref m);
        }


        // Method to fill coursetable with data based on button click
        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            FillCourseTable(sender, e); // Refactored

        }

        // Method to fill the coursetable with data from database
        private void FillCourseTable(object sender, EventArgs e)
        {
            panel2.BringToFront();
            DataTable courses = GetCourses(); // Refactored

            courseTable.DataSource = courses;
        }

        // Method to get the course details from the database and fill the datatable with it
        public DataTable GetCourses()
        {

            DataTable dt = new DataTable();
            string connectionString = "server=127.0.0.1;uid=root;pwd=root;database=attendance";
            string query = isAdmin ? "SELECT code, name FROM courses;" : "SELECT code, name FROM courses WHERE t_id=@t_id;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@t_id", t_id);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    // Console.WriteLine(reader);

                    dt.Load(reader);
                }
            }

            return dt;
        }

        // Method to make the attendance table
        private void courseTable_CellDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        {
            MakeAttendanceTable(e);

        }

        // Method to fill the attendance table fill with data from database
        private void MakeAttendanceTable(DataGridViewCellEventArgs e)
        {
            string connectionString = "server=127.0.0.1;uid=root;pwd=root;database=attendance";

            string name = courseTable.Rows[e.RowIndex].Cells[1].Value.ToString()!;
            string code = courseTable.Rows[e.RowIndex].Cells[0].Value.ToString()!;
            courseLabel.Text = code;

            using (MySqlConnection conn = new(connectionString))
            {
                conn.Open();

                string query = "SELECT id FROM courses WHERE name=@name";
                MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@name", name);

                int c_id = (int)cmd.ExecuteScalar();

                panel3.BringToFront();

                DataTable attendanceDataTable = MakeAttendanceTable(c_id);

                attendanceTable.DataSource = attendanceDataTable;
                attendanceTable.Tag = c_id;
            }
        }

        // Method to make attendance table based on data from the dropdown
        private void MakeAttendanceTableMenu(object sender, EventArgs e, string code)
        {
            string connectionString = "server=127.0.0.1;uid=root;pwd=root;database=attendance";

            courseLabel.Text = code;

            using (MySqlConnection conn = new(connectionString))
            {
                conn.Open();

                string query = "SELECT id FROM courses WHERE code=@code";
                MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@code", code);

                int c_id = (int)cmd.ExecuteScalar();

                panel3.BringToFront();

                DataTable attendanceDataTable = MakeAttendanceTable(c_id);

                attendanceTable.DataSource = attendanceDataTable;
                attendanceTable.Tag = c_id;
            }
        }

        // Method to save attendance to the database and show messagebox
        private void bunifuButton3_Click(object sender, EventArgs e)
        {
            try
            {
                AddAttendance(attendanceTable, (int)attendanceTable.Tag);

                // MessageBox.Show("Attendance successfully saved!", "Success");
                bunifuSnackbar1.Show(this, "Attendance successfully saved!", Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success);
                panel2.BringToFront();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        // Method to bring panel to from
        private void bunifuButton4_Click(object sender, EventArgs e)
        {
            panel2.BringToFront();
        }

        // Method to bring content panel to front
        private void iconButton2_Click(object sender, EventArgs e)
        {
            panelContent.BringToFront();
        }

        // Method to exit
        private void iconButton4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Method to make the logout form
        private void iconButton5_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
            Thread newThread = new Thread((ThreadStart)delegate
            {
                Application.Run(new Login());
            });
            //newThread.TrySetApartmentState(ApartmentState.STA)
            newThread.Start();
        }

        // Method to bring the insert teacher panel to front
        private void addTeacherBtn_Click(object sender, EventArgs e)
        {
            panelAddTeacher.BringToFront();
        }

        // Method to save teacher data in the database
        private void teacherSaveBtn_Click(object sender, EventArgs e)
        {
            string connectionString = "server=127.0.0.1;uid=root;pwd=root;database=attendance";
            string t_query = "INSERT INTO teachers (`name`) VALUES (@name)";
            string u_query = "INSERT INTO users (t_id, username, password, is_admin) VALUES (@t_id, @username, @password, @isAdmin)";

            string name = teacherNameBox.Text;
            string username = teacherNameBox.Text.Replace(" ", "").ToLower();
            string password = passwordBox.Text;
            bool isAdmin = isAdminCheck.Checked;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(t_query, conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();

                MySqlCommand u_cmd = new MySqlCommand(u_query, conn);
                int t_id = (int)cmd.LastInsertedId;

                u_cmd.Parameters.AddWithValue("@t_id", t_id);
                u_cmd.Parameters.AddWithValue("@username", username);
                u_cmd.Parameters.AddWithValue("@password", password);
                u_cmd.Parameters.AddWithValue("@isAdmin", isAdmin);
                u_cmd.ExecuteNonQuery();

                bunifuSnackbar1.Show(this, "Succesfully added teacher!", Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success);

                panelAdd.BringToFront();
            }
        }

        // Method to save student data into database
        private void saveStudentBtn_Click(object sender, EventArgs e)
        {
            string c_id = coursesDropDown.SelectedValue.ToString()!;
            string connectionString = "server=127.0.0.1;uid=root;pwd=root;database=attendance";
            string dob = dateOfBirthBox.Value.ToString("yyyy-MM-dd");
            string name = studentNameBox.Text;

            string query = "INSERT INTO students (`name`, dob, c_id) VALUES (@name, @dob, @c_id)";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@dob", dob);
                cmd.Parameters.AddWithValue("@c_id", c_id);

                cmd.ExecuteNonQuery();

                bunifuSnackbar1.Show(this, "Succesfully added student!", Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success);

                panelAdd.BringToFront();

            }

        }

        // Method to show the add student panel and fill dropdown with courses
        private void addStudentBtn_Click(object sender, EventArgs e)
        {
            DataTable courses = new DataTable();
            string connectionString = "server=127.0.0.1;uid=root;pwd=root;database=attendance";
            string query = "SELECT id, code, name FROM courses;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                conn.Open();
                MySqlDataReader reader = cmd.ExecuteReader();

                courses.Load(reader);
            }

            coursesDropDown.DataSource = courses;
            coursesDropDown.DisplayMember = "code";
            coursesDropDown.ValueMember = "id";

            panelAddStudent.BringToFront();
        }

        // Method to add teachers to the dropdown and show the add teacher panel
        private void addCourseBtn_Click(object sender, EventArgs e)
        {
            DataTable teachers = new DataTable();
            string connectionString = "server=127.0.0.1;uid=root;pwd=root;database=attendance";
            string query = "SELECT id, name FROM teachers;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                conn.Open();
                MySqlDataReader reader = cmd.ExecuteReader();

                teachers.Load(reader);
            }

            teacherDropDown.DataSource = teachers;
            teacherDropDown.DisplayMember = "name";
            teacherDropDown.ValueMember = "id";

            panelAddCourse.BringToFront();

        }

        // Method to save course to the database
        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            string code = codeTextBox.Text;
            string name = courseNameBox.Text;
            string year = courseYearBox.Value.ToString("yyyy");
            string t_id = teacherDropDown.SelectedValue.ToString()!;


            string connectionString = "server=127.0.0.1;uid=root;pwd=root;database=attendance";
            string query = "INSERT INTO courses (code, `name`, c_year, t_id) VALUES (@code, @name, @c_year, @t_id)";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@code", code);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@c_year", year);
                cmd.Parameters.AddWithValue("@t_id", t_id);

                cmd.ExecuteNonQuery();

                bunifuSnackbar1.Show(this, "Succesfully added course!", Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success);

                panelAdd.BringToFront();

            }
        }

        // Method to bring the add data panel to front
        private void addButton_Click(object sender, EventArgs e)
        {
            panelAdd.BringToFront();
        }

        // Method to fill the courses table with data from database
        private void editBtn_Click(object sender, EventArgs e)
        {
            editPanel.BringToFront();

            DataTable courses = GetCourses();
            editCourseTable.DataSource = courses;
        }

        // Method to fill the table with attendance data from the database
        private void editAttendanceBtn_Click(object sender, EventArgs e)
        {
            DataTable attendanceDataTable = new DataTable();

            string code = editCourseTable.SelectedRows[0].Cells[0].Value.ToString()!;
            string date = editDateBox.Value.ToString("yyyy-MM-dd");
            label10.Text = $"Edit Attendance ({date} - {code})";

            string connectionString = "server=127.0.0.1;uid=root;pwd=root;database=attendance";
            string query = "SELECT students.id AS s_id, students.name, courses.code, IF((SELECT id FROM attendance WHERE s_id=students.id AND c_id=courses.id AND date = @date), True, False) AS attendance FROM courses INNER JOIN students ON courses.id = students.c_id WHERE code=@code;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@code", code);

                MySqlDataReader reader = cmd.ExecuteReader();
                attendanceDataTable.Load(reader);
            }
            var p = new DataColumn();
            p.DataType = System.Type.GetType("System.Boolean");
            p.ColumnName = "Attendance";
            p.ReadOnly = false;
            p.DefaultValue = false;

            attendanceDataTable.Columns.Add(p);
            var attendanceColumn = attendanceDataTable.Columns["attendance"];

            foreach (DataRow row in attendanceDataTable.Rows)
            {
                long val = Convert.ToInt64(row["attendance"]);
                row["Attendance"] = val == 1 ? true : false;
            }

            attendanceDataTable.Columns.Remove("attendance");
            editAttendanceTable.DataSource = attendanceDataTable;

            editAttendancePanel.BringToFront();
        }

        // Method to edit the attendance data from the database
        private void saveBtn_Click(object sender, EventArgs e)
        {
            string connectionString = "server=127.0.0.1;uid=root;pwd=root;database=attendance";
            string a_query = "SELECT id FROM attendance WHERE date=@date AND s_id=@s_id AND c_id=(SELECT id FROM courses WHERE courses.code=@code);";
            string add_query = "INSERT INTO attendance (s_id, `date`, c_id) VALUES (@s_id, @date, ((SELECT id FROM courses WHERE courses.code=@code)));";
            string del_query = "DELETE FROM attendance WHERE date=@date AND s_id=@s_id AND c_id=(SELECT id FROM courses WHERE courses.code=@code);";

            int totalStudents = editAttendanceTable.Rows.Count;
            string date = editDateBox.Value.ToString("yyyy-MM-dd");

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                for (int i = 1; i <= totalStudents; i++)
                {
                    DataGridViewRow curRow = editAttendanceTable.Rows[i - 1];
                    string s_id = curRow.Cells["s_id"].Value.ToString()!;
                    string name = curRow.Cells["name"].Value.ToString()!;
                    string code = curRow.Cells["code"].Value.ToString()!;
                    bool isPresent = Convert.ToBoolean(curRow.Cells["Attendance"].Value);

                    MySqlCommand a_cmd = new MySqlCommand(a_query, conn);
                    a_cmd.Parameters.AddWithValue("@date", date);
                    a_cmd.Parameters.AddWithValue("@s_id", s_id);
                    a_cmd.Parameters.AddWithValue("@code", code);

                    var exists = a_cmd.ExecuteScalar();
                    if (exists == null)
                    {
                        if (isPresent)
                        {
                            MySqlCommand add_cmd = new MySqlCommand(add_query, conn);
                            add_cmd.Parameters.AddWithValue("@date", date);
                            add_cmd.Parameters.AddWithValue("@s_id", s_id);
                            add_cmd.Parameters.AddWithValue("@code", code);

                            add_cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        if (!isPresent)
                        {
                            MySqlCommand del_cmd = new MySqlCommand(del_query, conn);
                            del_cmd.Parameters.AddWithValue("@date", date);
                            del_cmd.Parameters.AddWithValue("@s_id", s_id);
                            del_cmd.Parameters.AddWithValue("@code", code);

                            del_cmd.ExecuteNonQuery();

                        }
                    }
                }

                bunifuSnackbar1.Show(this, $"Succesfully updated attendance for {date}!", Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success);

                editPanel.BringToFront();

            }

        }

        // Method to show the edit panel
        private void mainEditBtn_Click(object sender, EventArgs e)
        {
            editBtn_Click(sender, e);
        }

        // Method to fill the table with courses from database
        private void iconButton1_Click(object sender, EventArgs e)
        {
            panelReport.BringToFront();

            DataTable courses = GetCourses();
            courseDGV.DataSource = courses;
        }

        // Method to show the students in a course from the database
        private void courseDGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string code = courseDGV.Rows[e.RowIndex].Cells["code"].Value.ToString()!;
            Console.WriteLine(code);
            string connectionString = "server=127.0.0.1;uid=root;pwd=root;database=attendance";
            string query = "SELECT id, name FROM students WHERE c_id=(SELECT id FROM courses WHERE code=@code);";

            panelStudents.BringToFront();

            DataTable students = new DataTable();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@code", code);
                MySqlDataReader reader = cmd.ExecuteReader();

                students.Load(reader);
            }
            studentsTable.DataSource = students;
        }

        // Method to bring the report panel to the front
        private void studentsTable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            panelStudentReport.BringToFront();

            string name = studentsTable.Rows[e.RowIndex].Cells[1].Value.ToString()!;
            label13.Text = $"Report for {name}";
        }
    }
}