﻿using LXP.Common.ViewModels;
using LXP.Common.Entities;
using LXP.Core.IServices;

using LXP.Data.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using LXP.Data.Repository;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LXP.Core.Services
{
    public class CourseServices : ICourseServices
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICourseLevelRepository _courseLevelRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _contextAccessor;
        

        public CourseServices(ICourseRepository courseRepository,ICategoryRepository categoryRepository,ICourseLevelRepository courseLevelRepository,IWebHostEnvironment environment,IHttpContextAccessor httpContextAccessor) 
        {
            _courseRepository = courseRepository;
            _courseLevelRepository = courseLevelRepository;
            _categoryRepository = categoryRepository;
            _environment = environment;
            _contextAccessor = httpContextAccessor;

        }
        public  bool AddCourse(CourseViewModel course)
        {
            bool isCourseExists = _courseRepository.AnyCourseByCourseTitle(course.Title);

            if (!isCourseExists)
            {
                

                Guid levelId= Guid.Parse(course.Level);
                CourseLevel level = _courseLevelRepository.GetCourseLevelByCourseLevelId(levelId);
                Guid categoryId = Guid.Parse(course.Catagory);
                CourseCategory category = _categoryRepository.GetCategoryByCategoryId(categoryId);

                // Generate a unique file name
                var uniqueFileName = $"{Guid.NewGuid()}_{course.Thumbnailimage.FileName}";

                // Save the image to a designated folder (e.g., wwwroot/images)
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "CourseThumbnailImages"); // Use WebRootPath
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    course.Thumbnailimage.CopyTo(stream); // Use await
                }

                Course newcourse = new Course
                {
                    CourseId = Guid.NewGuid(),
                    Catagory = category,
                    Level = level,
                    Title = course.Title,
                    Description = course.Description,
                    Duration = course.Duration,
                    Thumbnail = uniqueFileName,
                    CreatedBy = "Admin",
                    CreatedAt = new DateTime(),
                    IsActive = true,
                    IsAvailable = true,
                    ModifiedAt = new DateTime(),
                    ModifiedBy = "Admin"


                };
                _courseRepository.AddCourse(newcourse);

                return true;
            }
            else
            {
                return false;
            }
        }

        public Course GetCourseByCourseId(string courseId)
        {
            Guid CourseId = Guid.Parse(courseId);
            var course = _courseRepository.GetCourseDetailsByCourseId(CourseId);
            var courseView = new Course
            {
               Title = course.Title,
               Description = course.Description,
               Catagory=course.Catagory,
               Level=course.Level,
               Duration = course.Duration,
               Thumbnail= String.Format("{0}://{1}{2}/wwwroot/CourseThumbnailImages/{3}",
                                             _contextAccessor.HttpContext.Request.Scheme,
                                             _contextAccessor.HttpContext.Request.Host,
                                             _contextAccessor.HttpContext.Request.PathBase,
                                             course.Thumbnail)
           
            };
            return courseView;
            


        }
    }
}
