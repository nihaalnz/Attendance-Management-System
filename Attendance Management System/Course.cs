namespace Attendance_Management_System
{
    public class Course
    {
        private int id;
        private string name;
        private string code;
        private string tutor;

        public int Id { get { return id; } set { id = value; } }
        public string Name { get { return name; } set { name = value; } }
        public string Code { get { return code; } set { code = value; } }
        public string Tutor { get { return tutor; } set { tutor = value; } }

        public void ViewReport(){
            
        }
    }
}
