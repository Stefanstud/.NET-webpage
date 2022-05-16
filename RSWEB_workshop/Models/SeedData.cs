using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RSWEB_workshop.Areas.Identity.Data;
using RSWEB_workshop.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RSWEB_workshop.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<RSWEB_workshopUser>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            RSWEB_workshopUser user = await UserManager.FindByEmailAsync("admin@ekursevi.com");
            if (user == null)
            {
                var User = new RSWEB_workshopUser();
                User.Email = "admin@ekursevi.com";
                User.UserName = "admin@ekursevi.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
            //Add Teacher Role
            roleCheck = await RoleManager.RoleExistsAsync("Teacher");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Teacher")); }
            user = await UserManager.FindByEmailAsync("teacher@ekursevi.com");
            if (user == null)
            {
                var User = new RSWEB_workshopUser();
                User.Email = "teacher@ekursevi.com";
                User.UserName = "teacher@ekursevi.com";
                string userPWD = "Teacher123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Teacher
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Teacher"); }
            }
            //Add Student Role
            roleCheck = await RoleManager.RoleExistsAsync("Student");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Student")); }
            user = await UserManager.FindByEmailAsync("student@ekursevi.com");
            if (user == null)
            {
                var User = new RSWEB_workshopUser();
                User.Email = "student@ekursevi.com";
                User.UserName = "student@ekursevi.com";
                string userPWD = "Student123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Student
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Student"); }
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new RSWEB_workshopContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<RSWEB_workshopContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();

                if (context.Teacher.Any())
                {
                    return;   // DB has been seeded
                }
                if (context.Student.Any())
                {
                    return;   // DB has been seeded
                }
                if (context.Course.Any())
                {
                    return;   // DB has been seeded
                }
                if (context.Enrollment.Any())
                {
                    return;   // DB has been seeded
                }

                context.Teacher.AddRange(
                    new Teacher
                    {
                        //Id = 1,
                        FirstName = "Stojan",
                        LastName = "Stojanov",
                        Degree = "CS",
                        AcademicRank = "PhD",
                        OfficeNumber = "23",
                        HireDate = DateTime.Parse("2010-7-2")
                    },

                    new Teacher
                    {
                        //Id = 2,
                        FirstName = "Flora",
                        LastName = "Kocoska",
                        Degree = "Electronics",
                        AcademicRank = "Master",
                        OfficeNumber = "22",
                        HireDate = DateTime.Parse("2020-6-7"),
                    },
                    new Teacher
                    {
                        //Id = 3,
                        FirstName = "Bube",
                        LastName = "Bube",
                        Degree = "Robotics",
                        AcademicRank = "PhD",
                        OfficeNumber = "24",
                        HireDate = DateTime.Parse("2008-6-6")
                    }
                );
                    context.SaveChanges();
                
                context.Course.AddRange(
                    new Course
                    {
                        //Id=1
                        Title = "RSWEB",
                        Credits = 6,
                        Semester = "sixth",
                        Programme = "KTI",
                        EducationLevel = "high",
                        FirstTeacherId = context.Teacher.First(x => x.FirstName.Equals("Stojan") && x.LastName.Equals("Stojanov")).Id,
                        SecondTeacherId = context.Teacher.First(x => x.FirstName.Equals("Bube") && x.LastName.Equals("Bube")).Id

                    },

                    new Course
                    {
                        //Id = 2,
                        Title = "OEK",
                        Credits = 6,
                        Semester = "second",
                        Programme = "KHIE",
                        EducationLevel = "medium",
                        FirstTeacherId = context.Teacher.First(x => x.FirstName.Equals("Flora") && x.LastName.Equals("Kocoska")).Id,
                        SecondTeacherId = context.Teacher.First(x => x.FirstName.Equals("Stojan") && x.LastName.Equals("Stojanov")).Id
                    },

                    new Course
                    {
                        //Id = 3,
                        Title = "Robotika",
                        Credits = 8,
                        Semester = "fourth",
                        Programme = "KSIAR",
                        EducationLevel = "high",
                        FirstTeacherId = context.Teacher.First(x => x.FirstName.Equals("Bube") && x.LastName.Equals("Bube")).Id,
                        SecondTeacherId = context.Teacher.First(x => x.FirstName.Equals("Flora") && x.LastName.Equals("Kocoska")).Id
                    }

                );
                    context.SaveChanges();
                
                context.Student.AddRange(
                    new Student
                    {
                        //Id = 1,
                        StudentId = 50,
                        FirstName = "Stefan",
                        LastName = "Krsteski",
                        AcquiredCredits = 150,
                        CurrentSemester = 6,
                        EducationLevel = "high",
                        EnrollmentDate = DateTime.Parse("2019-10-10")

                    },

                    new Student
                    {
                        //Id = 2,
                        StudentId = 8,
                        FirstName = "Matea",
                        LastName = "Tashkovska",
                        AcquiredCredits = 240,
                        CurrentSemester = 10,
                        EducationLevel = "high",
                        EnrollmentDate = DateTime.Parse("2019-09-01")
                    },

                    new Student
                    {
                        //Id = 3,
                        StudentId = 7,
                        FirstName = "Hristijan",
                        LastName = "Slavkoski",
                        AcquiredCredits = 120,
                        CurrentSemester = 5,
                        EducationLevel = "high",
                        EnrollmentDate = DateTime.Parse("2019-10-10")
                    }
                    );
                context.SaveChanges();
                context.Enrollment.AddRange(
                    new Enrollment
                    {
                        //Id=1 ,
                        CourseId = context.Course.First(x => x.Title == "RSWEB").Id,
                        StudentId = context.Student.First(x => x.FirstName == "Stefan" && x.LastName == "Krsteski").Id,
                        Semester = "fifth",
                        Year = 2019,
                        Grade = 10,
                        SeminalUrl = "link1",
                        ProjectUrl = "link2",
                        ExamPoint = 100,
                        SeminalPoints = 100,
                        ProjectPoints = 100,
                        AdditionalPoints = 0,
                        FinishDate = DateTime.Parse("2022-10-5")

                    },

                    new Enrollment
                    {
                        //Id = 2,
                        CourseId = context.Course.First(x => x.Title == "Robotika").Id,
                        StudentId = context.Student.First(x => x.FirstName == "Hristijan" && x.LastName == "Slavkoski").Id,
                        Semester = "third",
                        Year = 2020,
                        Grade = 10,
                        SeminalUrl = "link3",
                        ProjectUrl = "link4",
                        ExamPoint = 101,
                        SeminalPoints = 99,
                        ProjectPoints = 20,
                        AdditionalPoints = 3,
                        FinishDate = DateTime.Parse("2023-11-5")
                    },

                    new Enrollment
                    {
                        //Id = 3,
                        CourseId = context.Course.First(x => x.Title == "OEK").Id,
                        StudentId = context.Student.First(x => x.FirstName == "Matea" && x.LastName == "Tashkovska").Id,
                        Semester = "second",
                        Year = 2020,
                        Grade = 8,
                        SeminalUrl = "link5",
                        ProjectUrl = "link6",
                        ExamPoint = 91,
                        SeminalPoints = 30,
                        ProjectPoints = 20,
                        AdditionalPoints = 10,
                        FinishDate = DateTime.Parse("2021-5-2")
                    }
                    );
                context.SaveChanges();
                
            }
        }
    }
}

