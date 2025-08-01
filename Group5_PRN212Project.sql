USE [master]
GO
/****** Object:  Database [PRN212SkillsHoannn6]    Script Date: 23/07/2025 21:18:53 ******/
CREATE DATABASE [PRN212SkillsHoannn6]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'PRN212SkillsHoannn6', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\PRN212SkillsHoannn6.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'PRN212SkillsHoannn6_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\PRN212SkillsHoannn6_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [PRN212SkillsHoannn6].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET ARITHABORT OFF 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET  ENABLE_BROKER 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET  MULTI_USER 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET DB_CHAINING OFF 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET QUERY_STORE = ON
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [PRN212SkillsHoannn6]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 23/07/2025 21:18:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ActivityLogs]    Script Date: 23/07/2025 21:18:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActivityLogs](
	[LogId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[Action] [nvarchar](max) NOT NULL,
	[Timestamp] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_ActivityLogs] PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AssessmentResults]    Script Date: 23/07/2025 21:18:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssessmentResults](
	[ResultId] [int] IDENTITY(1,1) NOT NULL,
	[AssessmentId] [int] NOT NULL,
	[StudentId] [int] NOT NULL,
	[Score] [decimal](4, 1) NULL,
	[SubmissionDate] [datetime2](7) NULL,
	[SubmissionFilePath] [nvarchar](max) NULL,
 CONSTRAINT [PK_AssessmentResults] PRIMARY KEY CLUSTERED 
(
	[ResultId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Assessments]    Script Date: 23/07/2025 21:18:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Assessments](
	[AssessmentId] [int] IDENTITY(1,1) NOT NULL,
	[CourseId] [int] NOT NULL,
	[AssessmentName] [nvarchar](max) NOT NULL,
	[MaxScore] [int] NOT NULL,
	[DueDate] [datetime2](7) NOT NULL,
	[AssessmentType] [nvarchar](50) NULL,
	[Instructions] [nvarchar](max) NULL,
 CONSTRAINT [PK_Assessments] PRIMARY KEY CLUSTERED 
(
	[AssessmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Certificates]    Script Date: 23/07/2025 21:18:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Certificates](
	[CertificateId] [int] IDENTITY(1,1) NOT NULL,
	[StudentId] [int] NOT NULL,
	[CourseId] [int] NOT NULL,
	[IssueDate] [datetime2](7) NOT NULL,
	[CertificateCode] [nvarchar](50) NOT NULL,
	[FilePath] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Certificates] PRIMARY KEY CLUSTERED 
(
	[CertificateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UK_Certificates_CertificateCode] UNIQUE NONCLUSTERED 
(
	[CertificateCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CourseMaterials]    Script Date: 23/07/2025 21:18:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CourseMaterials](
	[MaterialId] [int] IDENTITY(1,1) NOT NULL,
	[CourseId] [int] NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[FilePath] [nvarchar](max) NOT NULL,
	[UploadDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_CourseMaterials] PRIMARY KEY CLUSTERED 
(
	[MaterialId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CourseSchedules]    Script Date: 23/07/2025 21:18:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CourseSchedules](
	[ScheduleId] [int] IDENTITY(1,1) NOT NULL,
	[CourseId] [int] NOT NULL,
	[SessionDate] [datetime2](7) NOT NULL,
	[StartTime] [time](7) NOT NULL,
	[EndTime] [time](7) NOT NULL,
	[Room] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_CourseSchedules] PRIMARY KEY CLUSTERED 
(
	[ScheduleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Enrollments]    Script Date: 23/07/2025 21:18:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Enrollments](
	[EnrollmentId] [int] IDENTITY(1,1) NOT NULL,
	[StudentId] [int] NOT NULL,
	[CourseId] [int] NOT NULL,
	[CompletionStatus] [bit] NOT NULL,
	[CompletionDate] [datetime2](7) NULL,
 CONSTRAINT [PK_Enrollments] PRIMARY KEY CLUSTERED 
(
	[EnrollmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feedbacks]    Script Date: 23/07/2025 21:18:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedbacks](
	[FeedbackId] [int] IDENTITY(1,1) NOT NULL,
	[StudentId] [int] NOT NULL,
	[CourseId] [int] NOT NULL,
	[Rating] [int] NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[FeedbackDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Feedbacks] PRIMARY KEY CLUSTERED 
(
	[FeedbackId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Instructors]    Script Date: 23/07/2025 21:18:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Instructors](
	[InstructorId] [int] IDENTITY(1,1) NOT NULL,
	[InstructorName] [nvarchar](max) NOT NULL,
	[Experience] [int] NOT NULL,
	[Email] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](20) NULL,
 CONSTRAINT [PK_Instructors] PRIMARY KEY CLUSTERED 
(
	[InstructorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LifeSkillCourses]    Script Date: 23/07/2025 21:18:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LifeSkillCourses](
	[CourseId] [int] IDENTITY(1,1) NOT NULL,
	[CourseName] [nvarchar](max) NOT NULL,
	[InstructorId] [int] NOT NULL,
	[StartDate] [datetime2](7) NULL,
	[EndDate] [datetime2](7) NULL,
	[Description] [nvarchar](max) NULL,
	[MaxStudents] [int] NULL,
	[Price] [decimal](18, 2) NULL,
	[Status] [nvarchar](50) NULL,
 CONSTRAINT [PK_LifeSkillCourses] PRIMARY KEY CLUSTERED 
(
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notifications]    Script Date: 23/07/2025 21:18:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notifications](
	[NotificationId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[StudentId] [int] NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Notifications] PRIMARY KEY CLUSTERED 
(
	[NotificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payments]    Script Date: 23/07/2025 21:18:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payments](
	[PaymentId] [int] IDENTITY(1,1) NOT NULL,
	[StudentId] [int] NOT NULL,
	[CourseId] [int] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[PaymentDate] [datetime2](7) NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED 
(
	[PaymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Students]    Script Date: 23/07/2025 21:18:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Students](
	[StudentId] [int] IDENTITY(1,1) NOT NULL,
	[StudentCode] [nvarchar](max) NOT NULL,
	[StudentName] [nvarchar](max) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](max) NULL,
	[Status] [nvarchar](50) NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[DateOfBirth] [datetime2](7) NULL,
	[AvatarPath] [nvarchar](max) NULL,
	[LastLogin] [datetime2](7) NULL,
 CONSTRAINT [PK_Students] PRIMARY KEY CLUSTERED 
(
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[ActivityLogs]  WITH CHECK ADD  CONSTRAINT [FK_ActivityLogs_Students_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Students] ([StudentId])
GO
ALTER TABLE [dbo].[ActivityLogs] CHECK CONSTRAINT [FK_ActivityLogs_Students_UserId]
GO
ALTER TABLE [dbo].[AssessmentResults]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentResults_Assessments_AssessmentId] FOREIGN KEY([AssessmentId])
REFERENCES [dbo].[Assessments] ([AssessmentId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AssessmentResults] CHECK CONSTRAINT [FK_AssessmentResults_Assessments_AssessmentId]
GO
ALTER TABLE [dbo].[AssessmentResults]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentResults_Students_StudentId] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([StudentId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AssessmentResults] CHECK CONSTRAINT [FK_AssessmentResults_Students_StudentId]
GO
ALTER TABLE [dbo].[Assessments]  WITH CHECK ADD  CONSTRAINT [FK_Assessments_LifeSkillCourses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[LifeSkillCourses] ([CourseId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Assessments] CHECK CONSTRAINT [FK_Assessments_LifeSkillCourses_CourseId]
GO
ALTER TABLE [dbo].[Certificates]  WITH CHECK ADD  CONSTRAINT [FK_Certificates_LifeSkillCourses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[LifeSkillCourses] ([CourseId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Certificates] CHECK CONSTRAINT [FK_Certificates_LifeSkillCourses_CourseId]
GO
ALTER TABLE [dbo].[Certificates]  WITH CHECK ADD  CONSTRAINT [FK_Certificates_Students_StudentId] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([StudentId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Certificates] CHECK CONSTRAINT [FK_Certificates_Students_StudentId]
GO
ALTER TABLE [dbo].[CourseMaterials]  WITH CHECK ADD  CONSTRAINT [FK_CourseMaterials_LifeSkillCourses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[LifeSkillCourses] ([CourseId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CourseMaterials] CHECK CONSTRAINT [FK_CourseMaterials_LifeSkillCourses_CourseId]
GO
ALTER TABLE [dbo].[CourseSchedules]  WITH CHECK ADD  CONSTRAINT [FK_CourseSchedules_LifeSkillCourses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[LifeSkillCourses] ([CourseId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CourseSchedules] CHECK CONSTRAINT [FK_CourseSchedules_LifeSkillCourses_CourseId]
GO
ALTER TABLE [dbo].[Enrollments]  WITH CHECK ADD  CONSTRAINT [FK_Enrollments_LifeSkillCourses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[LifeSkillCourses] ([CourseId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Enrollments] CHECK CONSTRAINT [FK_Enrollments_LifeSkillCourses_CourseId]
GO
ALTER TABLE [dbo].[Enrollments]  WITH CHECK ADD  CONSTRAINT [FK_Enrollments_Students_StudentId] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([StudentId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Enrollments] CHECK CONSTRAINT [FK_Enrollments_Students_StudentId]
GO
ALTER TABLE [dbo].[Feedbacks]  WITH CHECK ADD  CONSTRAINT [FK_Feedbacks_LifeSkillCourses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[LifeSkillCourses] ([CourseId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Feedbacks] CHECK CONSTRAINT [FK_Feedbacks_LifeSkillCourses_CourseId]
GO
ALTER TABLE [dbo].[Feedbacks]  WITH CHECK ADD  CONSTRAINT [FK_Feedbacks_Students_StudentId] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([StudentId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Feedbacks] CHECK CONSTRAINT [FK_Feedbacks_Students_StudentId]
GO
ALTER TABLE [dbo].[LifeSkillCourses]  WITH CHECK ADD  CONSTRAINT [FK_LifeSkillCourses_Instructors_InstructorId] FOREIGN KEY([InstructorId])
REFERENCES [dbo].[Instructors] ([InstructorId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LifeSkillCourses] CHECK CONSTRAINT [FK_LifeSkillCourses_Instructors_InstructorId]
GO
ALTER TABLE [dbo].[Notifications]  WITH CHECK ADD  CONSTRAINT [FK_Notifications_Students_StudentId] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([StudentId])
GO
ALTER TABLE [dbo].[Notifications] CHECK CONSTRAINT [FK_Notifications_Students_StudentId]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_LifeSkillCourses_CourseId] FOREIGN KEY([CourseId])
REFERENCES [dbo].[LifeSkillCourses] ([CourseId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_LifeSkillCourses_CourseId]
GO
ALTER TABLE [dbo].[Payments]  WITH CHECK ADD  CONSTRAINT [FK_Payments_Students_StudentId] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([StudentId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Payments] CHECK CONSTRAINT [FK_Payments_Students_StudentId]
GO
USE [master]
GO
ALTER DATABASE [PRN212SkillsHoannn6] SET  READ_WRITE 
GO
