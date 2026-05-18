USE TrainingAndCertificationPlatform;
GO


INSERT INTO Users (FullName, Email, Role, PasswordHash) VALUES
('Fatima Nasser',   'fatima.nasser@gmail.com',  'TrainingCoordinator', 'Admin123'),
('Hassan Ali',      'hassan.ali@gmail.com',      'Instructor',          'Instructor123'),
('Husain Mohammed', 'husain.mohammed@gmail.com', 'Instructor',          'Instructor123'),
('Sara Ahmed',      'sara.ahmed@gmail.com',      'Trainee',             'Trainee123'),
('Ali Jassim',      'ali.jassim@gmail.com',      'Trainee',             'Trainee123'),
('Maryam Khalil',   'maryam.khalil@gmail.com',   'Trainee',             'Trainee123'),
('Omar Salman',     'omar.salman@gmail.com',      'Trainee',             'Trainee123'),
('Zainab Yousuf',   'zainab.yousuf@gmail.com',   'Trainee',             'Trainee123');


INSERT INTO Subjects (Name) VALUES
('Information Technology'),
('Business Management'),
('Health and Safety'),
('Project Management'),
('Digital Marketing');


INSERT INTO Courses (Title, Description, DurationHours, Capacity, Fee, SubjectId, PrerequisiteCourseId) VALUES
('Introduction to Networking',  'Basics of networking',          20, 20, 120.00, 1, NULL),
('Advanced Cybersecurity',      'Cybersecurity fundamentals',    30, 15, 250.00, 1, 1),
('Business Communication',      'Professional writing skills',   16, 25, 95.00,  2, NULL),
('PMP Exam Preparation',        'Project management essentials', 40, 20, 300.00, 4, NULL),
('Social Media Marketing',      'Digital marketing strategies',  24, 30, 150.00, 5, NULL);


INSERT INTO InstructorSubjects (InstructorId, SubjectId) VALUES
(2, 1),
(2, 4),
(3, 2),
(3, 5),
(3, 3);


INSERT INTO InstructorAvailability (InstructorId, AvailableDate, StartTime, EndTime) VALUES
(2, '2025-06-01', '08:00', '14:00'),
(2, '2025-06-02', '08:00', '14:00'),
(2, '2025-06-03', '08:00', '14:00'),
(3, '2025-06-01', '09:00', '15:00'),
(3, '2025-06-04', '09:00', '15:00');


INSERT INTO Rooms (RoomName, Capacity) VALUES
('Manama Hall A',   25),
('Manama Hall B',   20),
('Riffa Lab 1',     15),
('Muharraq Room 1', 30),
('Seef Room 2',     20);


INSERT INTO RoomEquipment (RoomId, EquipmentName) VALUES
(1, 'Projector'),
(2, 'Whiteboard'),
(3, 'Lab Computers'),
(3, 'Projector'),
(4, 'Whiteboard');


INSERT INTO CourseSessions (CourseId, InstructorId, RoomId, SessionDate, StartTime, EndTime, MaxCapacity, Status) VALUES
(1, 2, 3, '2025-06-01', '08:00', '13:00', 15, 'Completed'),
(2, 2, 3, '2025-06-02', '08:00', '13:00', 15, 'Completed'),
(3, 3, 1, '2025-06-01', '09:00', '13:00', 25, 'Scheduled'),
(4, 2, 2, '2025-06-03', '08:00', '14:00', 20, 'Scheduled'),
(5, 3, 4, '2025-06-04', '09:00', '14:00', 30, 'Ongoing');

INSERT INTO Enrollments (TraineeId, SessionId, Status, EnrolledAt) VALUES
(4, 1, 'Completed', '2025-05-01'),
(5, 1, 'Completed', '2025-05-01'),
(6, 3, 'Confirmed', '2025-05-03'),
(7, 4, 'Enrolled',  '2025-05-05'),
(8, 5, 'Attending', '2025-05-06');


INSERT INTO Assessments (EnrollmentId, Result, Notes, RecordedAt) VALUES
(1, 'Pass', 'Excellent performance',             '2025-06-02'),
(2, 'Pass', 'Good understanding of concepts',    '2025-06-02'),
(3, 'Fail', 'Did not meet passing criteria',     '2025-06-02'),
(4, 'Pass', 'Strong project management skills',  '2025-06-04'),
(5, 'Pass', 'Great digital marketing knowledge', '2025-06-05');


INSERT INTO CertificationTracks (Name, Description) VALUES
('IT Professional Certificate',        'Core IT skills track'),
('Business Leadership Certificate',    'Business and project management track'),
('Digital Transformation Certificate', 'Marketing and IT combined track'),
('Workplace Safety Certificate',       'Health and safety compliance track'),
('GCC Management Certificate',         'Management essentials for Gulf region');


INSERT INTO TrackCourses (TrackId, CourseId) VALUES
(1, 1),
(1, 2),
(2, 3),
(2, 4),
(3, 5);


INSERT INTO TraineeCertifications (TraineeId, TrackId, Status, CertificateRef, IssuedAt) VALUES
(4, 1, 'Issued',     'CERT-BH-2025-0001', '2025-06-10'),
(5, 1, 'Eligible',   'CERT-BH-2025-0002', NULL),
(6, 2, 'InProgress', NULL,                NULL),
(7, 2, 'InProgress', NULL,                NULL),
(8, 3, 'InProgress', NULL,                NULL);


INSERT INTO Payments (EnrollmentId, Amount, PaidAt, DueDate, IsOverdue) VALUES
(1, 120.00, '2025-05-01', '2025-05-10', 0),
(2, 120.00, '2025-05-01', '2025-05-10', 0),
(3, 50.00,  '2025-05-20', '2025-05-10', 1),
(4, 150.00, '2025-05-05', '2025-05-15', 0),
(5, 75.00,  NULL,         '2025-05-15', 1);


INSERT INTO Notifications (UserId, Message, IsRead, CreatedAt) VALUES
(4, 'Your enrollment for Introduction to Networking is confirmed.', 1, '2025-05-01'),
(5, 'Your enrollment for Introduction to Networking is confirmed.', 1, '2025-05-01'),
(6, 'Your session for Business Communication starts on June 1st.',  0, '2025-05-20'),
(2, 'You have been assigned a new session on June 3rd.',            1, '2025-05-10'),
(3, 'New trainee enrolled in your Social Media Marketing session.', 0, '2025-05-06');