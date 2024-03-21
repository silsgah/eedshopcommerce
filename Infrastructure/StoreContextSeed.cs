using Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure
{
    public class StoreContextSeed
    {
        private const string Path = "../Infrastructure/SeedData/categories.json";
        private const string coursePath = "../Infrastructure/SeedData/courses.json";
        private const string learningPath = "../Infrastructure/SeedData/learnings.json";
        private const string requirmentPath = "../Infrastructure/SeedData/requirements.json";
        public static async Task SeedAsync(StoreContext context, ILogger logger, UserManager<User> userManager)
        {
            try
            {
                //var instructor = new User
                //{
                //    UserName = "instructor",
                //    Email = "instructor@test.com"
                //};

                //await userManager.CreateAsync(instructor, "Password@123");
                //await userManager.AddToRolesAsync(instructor, new[] { "Student", "Instructor" });
                if (!userManager.Users.Any())
                {
                    var student = new User
                    {
                        UserName = "student",
                        Email = "student@test.com"
                    };

                    await userManager.CreateAsync(student, "Password@123");
                    await userManager.AddToRoleAsync(student, "Student");

                    var instructor = new User
                    {
                        UserName = "instructor",
                        Email = "instructor@test.com"
                    };

                    await userManager.CreateAsync(instructor, "Password@123");
                    await userManager.AddToRolesAsync(instructor, new[] { "Student", "Instructor" });
                }
                if (!context.Categories.Any())
                {
                    var CategoryData = File.ReadAllText(Path);
                    var catgeory = JsonSerializer.Deserialize<List<Category>>(CategoryData);

                    foreach (var item in catgeory)
                    {
                        context.Categories.Add(item);
                    }

                    await context.SaveChangesAsync();

                }

                if (!context.Courses.Any())
                {
                    var CourseData = File.ReadAllText(coursePath);
                    var course = JsonSerializer.Deserialize<List<Course>>(CourseData);

                    foreach (var item in course)
                    {
                        context.Courses.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if (!context.Learnings.Any())
                {
                    var LearningData = File.ReadAllText(learningPath);
                    var learning = JsonSerializer.Deserialize<List<Learning>>(LearningData);

                    foreach (var item in learning)
                    {
                        context.Learnings.Add(item);
                    }
                    await context.SaveChangesAsync();
                }

                if (!context.Requirements.Any())
                {
                    var RequirementData = File.ReadAllText(requirmentPath);
                    var requirement = JsonSerializer.Deserialize<List<Requirement>>(RequirementData);

                    foreach (var item in requirement)
                    {
                        context.Requirements.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
                if (!context.Sections.Any())
                {
                    var sectionsData = File.ReadAllText("../Infrastructure/SeedData/sections.json");
                    var sections = JsonSerializer.Deserialize<List<Section>>(sectionsData);

                    foreach (var item in sections)
                    {
                        var course = await context.Courses.FindAsync(item.CourseId);

                        var section = new Section
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Course = course
                        };
                        context.Sections.Add(item);
                    }
                    await context.SaveChangesAsync();
                }
                if (!context.Lectures.Any())
                {
                    var lecturesData = File.ReadAllText("../Infrastructure/SeedData/lectures.json");
                    var lectures = JsonSerializer.Deserialize<List<Lecture>>(lecturesData);

                    foreach (var item in lectures)
                    {
                        var section = await context.Sections.FindAsync(item.SectionId);

                        var lecture = new Lecture
                        {
                            Id = item.Id,
                            Title = item.Title,
                            Url = item.Url,
                            Section = section
                        };

                        context.Lectures.Add(item);

                    }
                    await context.SaveChangesAsync();
                }
                //var student2 = new User
                //{
                //    UserName = "student2",
                //    Email = "student1@test.com"
                //};
                //await userManager.CreateAsync(student2, "Password@123");
                //await userManager.AddToRoleAsync(student2, "Student");
            }
            
            catch (Exception ex)

            {
                logger.LogError(ex.Message);
            }
        }
    }
}
