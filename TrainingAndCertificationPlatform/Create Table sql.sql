CREATE DATABASE TrainingAndCertificationPlatform;
GO
USE TrainingAndCertificationPlatform;
GO


CREATE TABLE Users (
    UserId       INT PRIMARY KEY IDENTITY,
    FullName     NVARCHAR(100) NOT NULL,
    Email        NVARCHAR(100) NOT NULL UNIQUE,
    Role         NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL
);


CREATE TABLE Subjects (
    SubjectId INT PRIMARY KEY IDENTITY,
    Name      NVARCHAR(100) NOT NULL
);


CREATE TABLE Courses (
    CourseId             INT PRIMARY KEY IDENTITY,
    Title                NVARCHAR(100) NOT NULL,
    Description          NVARCHAR(100),
    DurationHours        INT           NOT NULL,
    Capacity             INT           NOT NULL,
    Fee                  DECIMAL(10,2) NOT NULL,
    SubjectId            INT           NOT NULL,
    PrerequisiteCourseId INT           NULL,
    FOREIGN KEY (SubjectId)            REFERENCES Subjects(SubjectId),
    FOREIGN KEY (PrerequisiteCourseId) REFERENCES Courses(CourseId)
);


CREATE TABLE InstructorSubjects (
    Id           INT PRIMARY KEY IDENTITY,
    InstructorId INT NOT NULL,
    SubjectId    INT NOT NULL,
    FOREIGN KEY (InstructorId) REFERENCES Users(UserId),
    FOREIGN KEY (SubjectId)    REFERENCES Subjects(SubjectId)
);


CREATE TABLE InstructorAvailability (
    AvailabilityId INT PRIMARY KEY IDENTITY,
    InstructorId   INT  NOT NULL,
    AvailableDate  DATE NOT NULL,
    StartTime      TIME NOT NULL,
    EndTime        TIME NOT NULL,
    FOREIGN KEY (InstructorId) REFERENCES Users(UserId)
);


CREATE TABLE Rooms (
    RoomId   INT PRIMARY KEY IDENTITY,
    RoomName NVARCHAR(100) NOT NULL,
    Capacity INT           NOT NULL
);


CREATE TABLE RoomEquipment (
    EquipmentId   INT PRIMARY KEY IDENTITY,
    RoomId        INT           NOT NULL,
    EquipmentName NVARCHAR(100) NOT NULL,
    FOREIGN KEY (RoomId) REFERENCES Rooms(RoomId)
);


CREATE TABLE CourseSessions (
    SessionId    INT PRIMARY KEY IDENTITY,
    CourseId     INT          NOT NULL,
    InstructorId INT          NOT NULL,
    RoomId       INT          NOT NULL,
    SessionDate  DATE         NOT NULL,
    StartTime    TIME         NOT NULL,
    EndTime      TIME         NOT NULL,
    MaxCapacity  INT          NOT NULL,
    Status       NVARCHAR(100) NOT NULL,
    FOREIGN KEY (CourseId)     REFERENCES Courses(CourseId),
    FOREIGN KEY (InstructorId) REFERENCES Users(UserId),
    FOREIGN KEY (RoomId)       REFERENCES Rooms(RoomId)
);


CREATE TABLE Enrollments (
    EnrollmentId INT PRIMARY KEY IDENTITY,
    TraineeId    INT           NOT NULL,
    SessionId    INT           NOT NULL,
    Status       NVARCHAR(100) NOT NULL,
    EnrolledAt   DATETIME,
    FOREIGN KEY (TraineeId) REFERENCES Users(UserId),
    FOREIGN KEY (SessionId) REFERENCES CourseSessions(SessionId)
);


CREATE TABLE Assessments (
    AssessmentId INT PRIMARY KEY IDENTITY,
    EnrollmentId INT           NOT NULL,
    Result       NVARCHAR(100) NOT NULL,
    Notes        NVARCHAR(100),
    RecordedAt   DATETIME,
    FOREIGN KEY (EnrollmentId) REFERENCES Enrollments(EnrollmentId)
);


CREATE TABLE CertificationTracks (
    TrackId     INT PRIMARY KEY IDENTITY,
    Name        NVARCHAR(100) NOT NULL,
    Description NVARCHAR(100)
);


CREATE TABLE TrackCourses (
    Id       INT PRIMARY KEY IDENTITY,
    TrackId  INT NOT NULL,
    CourseId INT NOT NULL,
    FOREIGN KEY (TrackId)  REFERENCES CertificationTracks(TrackId),
    FOREIGN KEY (CourseId) REFERENCES Courses(CourseId)
);


CREATE TABLE TraineeCertifications (
    CertificationId INT PRIMARY KEY IDENTITY,
    TraineeId       INT           NOT NULL,
    TrackId         INT           NOT NULL,
    Status          NVARCHAR(100) NOT NULL,
    CertificateRef  NVARCHAR(100),
    IssuedAt        DATETIME      NULL,
    FOREIGN KEY (TraineeId) REFERENCES Users(UserId),
    FOREIGN KEY (TrackId)   REFERENCES CertificationTracks(TrackId)
);


CREATE TABLE Payments (
    PaymentId    INT PRIMARY KEY IDENTITY,
    EnrollmentId INT           NOT NULL,
    Amount       DECIMAL(10,2) NOT NULL,
    PaidAt       DATETIME,
    DueDate      DATE          NOT NULL,
    IsOverdue    BIT           NOT NULL DEFAULT 0,
    FOREIGN KEY (EnrollmentId) REFERENCES Enrollments(EnrollmentId)
);


CREATE TABLE Notifications (
    NotificationId INT PRIMARY KEY IDENTITY,
    UserId         INT           NOT NULL,
    Message        NVARCHAR(100) NOT NULL,
    IsRead         BIT           NOT NULL DEFAULT 0,
    CreatedAt      DATETIME,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);