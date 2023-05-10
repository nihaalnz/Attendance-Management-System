namespace Attendance_Management_System
{
    public class User
    {
        // Variables
        private int id;
        private Teacher teacher;
        private string username;
        private string password;
        private bool isAdmin;

        // Properties
        public int Id { get { return id; } set { id = value; } }
        public Teacher Teacher { get { return teacher; } set { teacher = value; } }
        public string Username { get { return username; } set { username = value; } }
        public string Password { get { return password; } set { password = value; } }
        public bool IsAdmin { get { return isAdmin; } set { isAdmin = value; } }

        // Method to verify log-in
        public bool LogIn(string username, string password){
            if (username == this.username && password == this.password)
            {
                return true;
            }
            return false;
        }

        // Method to log-out
        public bool LogOut(){
            return true; // Successfully logged out
        }

    }
}
