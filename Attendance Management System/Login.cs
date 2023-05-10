using MySql.Data.MySqlClient;


namespace Attendance_Management_System
{
    public partial class Login : Form
    {
        // Make class variables
        public bool isLoggedIn;

        // Make constructor
        public Login()
        {
            InitializeComponent();
            isLoggedIn = false; // Initially not logged in
        }

        // Method to authenticate the user
        private void loginBtn_Click(object sender, EventArgs e)
        {
            string connectionString = "server=127.0.0.1;uid=root;pwd=root;database=attendance";
            
            string username = usernameBox.Text;
            string password = passwordBox.Text;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT id, is_admin FROM users WHERE username=@username AND password=@password;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue ("@password", password);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var t_id = reader["id"];
                    if (t_id != null)
                    {
                        bool isAdmin = (bool) reader["is_admin"];

                        Application.ExitThread();
                        Thread newThread = new Thread((ThreadStart)delegate
                        {
                            Application.Run(new Form1((int)t_id, isAdmin));
                        });
                        //newThread.TrySetApartmentState(ApartmentState.STA)
                        newThread.Start();
                        isLoggedIn = true;
                    }

                    else
                    {
                        bunifuSnackbar1.Show(this, "Incorrect credentials", Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Error);

                    }
                }
                bunifuSnackbar1.Show(this, "Incorrect credentials", Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Error);

            }
        }
    }
}
