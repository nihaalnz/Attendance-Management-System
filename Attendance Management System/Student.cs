namespace Attendance_Management_System
{
    public class Student
    {
        private int id;
        private string name;
        private string dob;
        private Course course;
        private int? currentCourseYear;

        public int Id { get { return id; } set { id = value; } }
        public string Name { get { return name; } set { name = value; } }
        public string Dob { get { return dob; } set { dob = value; } }

        public Course Course { get { return course; } set { course = value; } }
        public int? CurrentCourseYear { get { return currentCourseYear; } set { currentCourseYear = value; } }

        public bool viewReport()
        {
            if (Course != null && CurrentCourseYear.HasValue)
            {
                // Generate and display the report for the student's course and current course year
                string report = GenerateReport();
                Console.WriteLine(report);
                
                return true;
            }

            Console.WriteLine("Cannot view report. Course information is missing.");
            
            return false;

        }

        private string GenerateReport()
        {
            // Generate the report for the student's course and current course year
            // Example: Concatenating the student's name, course name, and current course year
            string report = $"Student: {Name}\nCourse: {Course?.Name}\nCurrent Year: {CurrentCourseYear}";
            return report;
        }
    }

}
