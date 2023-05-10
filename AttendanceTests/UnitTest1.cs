
using System.Data;
using Attendance_Management_System;

namespace AttendanceSystemTests
{
    [TestClass]
    public class AttendanceTests
    {
        [TestMethod]
        public void LogInCredentialCheck()
        {
            // Arrange
            User user = new User();
            user.Username = "robertclark";
            user.Password = "helloworld";

            // Act
            bool result = user.LogIn("robertclark", "helloworld");

            // Assert
            Assert.IsTrue(result, "Invalid credentials");
        }

        [TestMethod]
        public void LogOutSuccessCheck()
        {
            // Arrange
            User user = new User();

            // Act
            bool logOutSuccess = user.LogOut();

            // Assert
            Assert.IsTrue(logOutSuccess);
        }


        [TestMethod]
        public void GetCoursesCheck()
        {
            // Arrange
            var form = new Form1(1, false);

            // Act
            DataTable courses = form.GetCourses();

            // Assert
            Assert.IsNotNull(courses, "Empty result");
        }

        [TestMethod]
        public void CheckStudentReport()
        {
            // Arrange
            Student student = new Student();
            student.Id = 1;
            student.Name = "Dummy";
            student.CurrentCourseYear = 1;
            student.Course = new Course();

            // Act
            bool viewReport = student.viewReport();


            // Assert
            Assert.IsTrue(viewReport, "Unable to view report");
        }

        [TestMethod]
        public void GetAttendanceTableCheck()
        {
            // Arrange
            Form1 form = new Form1(4, true);

            // Act
            DataTable attendanceTable = form.MakeAttendanceTable(3);

            // Assert
            Assert.IsNotNull(attendanceTable, "Empty result");
        }

        [TestMethod]
        public void SetterGetterCheck()
        {
            // Arrange
            Attendance attendance = new Attendance();
            int id = 1;
            Student student = new Student();
            Course course = new Course();
            string date = "2023-05-10";

            // Act
            attendance.Id = id;
            attendance.Student = student;
            attendance.Course = course;
            attendance.Date = date;

            // Assert
            Assert.AreEqual(id, attendance.Id);
            Assert.AreEqual(student, attendance.Student);
            Assert.AreEqual(course, attendance.Course);
            Assert.AreEqual(date, attendance.Date);
        }

    }
}