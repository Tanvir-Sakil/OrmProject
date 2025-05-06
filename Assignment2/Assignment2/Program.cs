using Assignment2;
using System;
using System.Collections.Generic;

class Program
{
    public static void Main()
    {
        var connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=OrmPractice;User ID=ormpractice; Password=123456;TrustServerCertificate=True;MultipleActiveResultSets=True;";
        var orm = new MyORM<Guid, Course>(connectionString);
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = "Training on Advanced Comptetitive programming -batch 4",
            Fees = 8000.99,
            Tests = new List<AdmissionTest>
            {
                new AdmissionTest
                {
                    Id = Guid.NewGuid(),
                    StartDateTime = DateTime.Parse("2025-05-16 14:30:00"),
                    EndDateTime = DateTime.Parse("2025-05-16 15:30:00"),
                    TestFees =200
                },

                 new AdmissionTest
                {
                    Id = Guid.NewGuid(),
                    StartDateTime = DateTime.Parse("2025-06-22 14:30:00"),
                    EndDateTime = DateTime.Parse("2025-06-22 15:30:00"),
                    TestFees =200
                }
            },
            Topics = new List<Topic>
            {
                new Topic
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction to Advanced Competitive Programming",
                    Sessions = new List<Session>
                    {
                        new Session
                        {
                            Id = Guid.NewGuid(),
                            DurationInHour = 2,
                            LearningObjective = "Advanced use of STL"
                        },
                        new Session
                        {
                            Id = Guid.NewGuid(),
                            DurationInHour = 1,
                            LearningObjective = "C++ memory management"
                        }
                    }
                },
                new Topic
                {
                    Id = Guid.NewGuid(),
                    Title = "Introduction to Dynamic Programming",
                    Sessions = new List<Session>
                    {
                        new Session
                        {
                            Id = Guid.NewGuid(),
                            DurationInHour = 2,
                            LearningObjective = "Classical Dynamic Programming Problems"
                        }
                    }
                }
            },
            Teacher = new Instructor
            {
                Id = Guid.NewGuid(),
                Name = "Tanvir",
                Email = "MdnvirSakil116991159u@example.com",

                PermanentAddress = new Address
                {
                    Id = Guid.NewGuid(),
                    City = "Chittagong",
                    Street = "Padua",
                    Country ="Bangladesh"

                },
                PresentAddress = new Address
                {
                    Id = Guid.NewGuid(),
                    City = "Dhaka",
                    Street = "Lalbag",
                    Country = "Bangladesh"

                },
                PhoneNumbers = new List<Phone>
                {
                    new Phone
                    {
                        Id = Guid.NewGuid(),
                        Number = "162436404",
                        Extension = "124",
                        CountryCode = "+880"

                    },
                        new Phone
                    {
                        Id = Guid.NewGuid(),
                        Number = "162436403",
                        Extension = "123",
                        CountryCode = "+880"
                    }
                }
            }
        };
        orm.Insert(course);
        Console.WriteLine("Inserted Successfully");

        var courseId = course.Id;
        var retrievedCourse = orm.GetById(courseId);
        retrievedCourse.Fees = 6699.88;
        retrievedCourse.Teacher.Name = "Sakil";

        retrievedCourse.Topics.Add(new Topic
        {
            Id = Guid.NewGuid(),
            Title = "Introduction to Advanced Searching",
            Description = "Searching is most importing topic for solving problem efficiently",
            Sessions = new List<Session>
                    {
                        new Session
                        {
                            Id = Guid.NewGuid(),
                            DurationInHour = 2,
                            LearningObjective = "Practice Searching Problems"
                        }
                    }
        });
        orm.Update(retrievedCourse);
        Console.WriteLine("Updated Successfully");

        var list = orm.GetAll();

        foreach (var item in list)
        {
           Console.WriteLine($"Final info for course with ID: {item.Id}");

            foreach (var property in item.GetType().GetProperties())
            {
                var value = property.GetValue(item);

                if (value is IEnumerable<object> collection && !(value is string))
                {
                    Console.WriteLine($"{property.Name}:  ");
                    int Propcont = 0;
                    foreach (var collectionItem in collection)
                    { 
                        Console.WriteLine($"     { property.Name.Substring(0, property.Name.Length - 1)}-{++Propcont}");
                        Console.WriteLine($"\t{collectionItem}");
                    foreach (var subProp in collectionItem.GetType().GetProperties())
                        {
                            var subValue = subProp.GetValue(collectionItem);
                            if (subValue is IEnumerable<object> subCollection && !(subValue is string))
                            {
                                Console.WriteLine($"{subProp.Name}:");
                                int subPropCont = 0;
                                

                                foreach (var subItem in subCollection)
                                {
                                    Console.WriteLine($"       {subProp.Name.Substring(0, subProp.Name.Length - 1)}-{++subPropCont}");
                                    Console.WriteLine($"\t{subItem}");
                                }
                            }
                            /*else
                            {
                                Console.WriteLine($"4:   {subProp.Name}: {subValue}");
                            }*/
                        }
                       
                    }
                }
                else
                {
                    Console.WriteLine($"{property.Name}:\n\t{value}");
                }
            }
        }

        var GettingCourse = orm.GetById(course.Id);

        Console.WriteLine($"Course Title: {GettingCourse.Title}");


        orm.Delete(course);

        Console.WriteLine("Delete successfully");

        var AfterDeletionlist = orm.GetAll();

        foreach (var item in AfterDeletionlist)
        {
            Console.WriteLine($"Final info for course with ID: {item.Id}");

            foreach (var property in item.GetType().GetProperties())
            {
                var value = property.GetValue(item);

                if (value is IEnumerable<object> collection && !(value is string))
                {
                    Console.WriteLine($"{property.Name}:  ");
                    int Propcont = 0;
                    foreach (var collectionItem in collection)
                    {
                        Console.WriteLine($"     {property.Name.Substring(0, property.Name.Length - 1)}-{++Propcont}");
                        Console.WriteLine($"\t{collectionItem}");
                        foreach (var subProp in collectionItem.GetType().GetProperties())
                        {
                            var subValue = subProp.GetValue(collectionItem);
                            if (subValue is IEnumerable<object> subCollection && !(subValue is string))
                            {
                                Console.WriteLine($"{subProp.Name}:");
                                int subPropCont = 0;


                                foreach (var subItem in subCollection)
                                {
                                    Console.WriteLine($"       {subProp.Name.Substring(0, subProp.Name.Length - 1)}-{++subPropCont}");
                                    Console.WriteLine($"\t{subItem}");
                                }
                            }
                            /*else
                                    {
                                Console.WriteLine($"4:   {subProp.Name}: {subValue}");
                            }
                            */
                                }

                    }
                }
                else
                {
                    Console.WriteLine($"{property.Name}:\n\t{value}");
                }
            }
        }


    }
}