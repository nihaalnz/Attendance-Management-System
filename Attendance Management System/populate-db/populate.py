from faker import Faker
from random import randint
import mysql.connector as mysql

tables = """
CREATE TABLE IF NOT EXISTS teachers(
    id INT(2) PRIMARY KEY AUTO_INCREMENT,
    `name` VARCHAR(30)
);
ALTER TABLE teachers AUTO_INCREMENT=1;
CREATE TABLE IF NOT EXISTS courses(
    id INT(2) PRIMARY KEY AUTO_INCREMENT,
    code VARCHAR(10),
    `name` VARCHAR(100),
    t_id INT(2),
    c_year YEAR,
    FOREIGN KEY (t_id) REFERENCES teachers(id)
);
ALTER TABLE courses AUTO_INCREMENT=1;

CREATE TABLE IF NOT EXISTS students(
    id INT(2) PRIMARY KEY AUTO_INCREMENT,
    `name` VARCHAR(50),
    dob DATE,
    c_id INT(2),
    FOREIGN KEY (c_id) REFERENCES courses(`id`)
);
ALTER TABLE students AUTO_INCREMENT=1;

CREATE TABLE IF NOT EXISTS attendance(
    id INT(2) PRIMARY KEY AUTO_INCREMENT,
    `date` DATE,
    s_id INT(2),
    c_id INT(2),
    FOREIGN KEY (s_id) REFERENCES students(id),
    FOREIGN KEY (c_id) REFERENCES courses(id)
);
ALTER TABLE attendance AUTO_INCREMENT=1;

CREATE TABLE users(
    ID INT PRIMARY KEY AUTO_INCREMENT,
    t_id INT NOT NULL,
    username VARCHAR(15) NOT NULL,
    `password` VARCHAR(60) NOT NULL,
    is_admin BOOLEAN NOT NULL,
    FOREIGN KEY (t_id) REFERENCES teachers (id)
);
ALTER TABLE users AUTO_INCREMENT=1;

-- UPDATE users SET `password`='admin', is_admin=1 WHERE id=4;
-- UPDATE users SET `password`='easypass' WHERE id=1;
"""

con = mysql.connect(user='root', password='root', host='localhost', database='attendance')
cur = con.cursor()

fake = Faker()

course_names = {
    'SWE4202': 'Computing Infrastructure',
    'SWE4203': 'Databases',
    'SWE4204': 'Fundamentals of Softvare Engineering',
    'SWE4205': 'Technology in Practice',
    'SWE4206': 'Networks and Security',
    'SWE4207': 'Computer Science Fundamentals',
}
TOTAL_TEACHERS = len(course_names)

# Teacher data -> Users data
for i in range(TOTAL_TEACHERS):
    t_name = fake.name()
    cur.execute("INSERT INTO teachers(name) VALUES (%s)", (t_name,))
    
    t_id = cur.lastrowid
    username = t_name.lower().replace(" ", "")[:15]
    password = fake.password(length=12)
    cur.execute("INSERT INTO users(t_id, username, `password`, is_admin) VALUES (%s, %s, %s, %s);", (t_id, username, password, False))

# Course data
for idx, (code, name) in enumerate(course_names.items(), start=1):
    t_id = randint(1, TOTAL_TEACHERS)
    c_year = '2023'
    cur.execute("INSERT INTO courses(code, name, t_id, c_year) VALUES (%s, %s, %s, %s)", (code, name, t_id, c_year))

    # Student data
    for i in range(20):
        s_name = fake.name()
        s_dob = fake.date_of_birth(maximum_age=25, minimum_age=15)
        
        c_id = idx
        
        cur.execute("INSERT INTO students(name, dob, c_id) VALUES (%s, %s, %s)", (s_name, s_dob, c_id))


# Save the transaction
con.commit()
print("Done!")