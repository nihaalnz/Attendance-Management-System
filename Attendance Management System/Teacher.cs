namespace Attendance_Management_System
{
    public class Teacher
    {
        private int id;
        private string name;
        private string dob;

        public int Id { get { return id; } set { id = value; } }
        public string Name { get { return name; } set { name = value; } }
        public string? Dob { get { return dob; } set { dob = value!; } }

        public void GenerateReport(){
            
        }
        public void UpdateAttendance(){

        }
    }

}
