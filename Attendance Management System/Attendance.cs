using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance_Management_System
{
    public class Attendance
    {
        private int id;
        private Student student;
        private Course course;
        private string date;

        public int Id { get { return id; } set { id = value; } }
        public Student Student { get { return student; } set { student = value; } }
        public Course Course { get { return course; } set { course = value; } }
        public string Date { get { return date; } set { date = value; } }

        public void MarkAttendance()
        {

        }
        public void UpdateAttendance()
        {

        }
    }
}
