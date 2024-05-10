using LXP.Core.IServices;
using LXP.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LXP.Common.ViewModels;
using LXP.Common.Entities;
using Microsoft.Extensions.Logging;

namespace LXP.Core.Services
{
    public class CourseTopicServices:ICourseTopicServices
    {
        private readonly ICourseTopicRepository _courseTopicRepository;
        private readonly ICourseRepository _courseRepository;
        public CourseTopicServices(ICourseTopicRepository courseTopicRepository,ICourseRepository courseRepository)
        {
            _courseTopicRepository = courseTopicRepository;
            _courseRepository = courseRepository;
        }
        public object GetTopicDetails(string courseId)
        {
            return _courseTopicRepository.GetTopicDetailsByCourseId(courseId);
        }

        public async Task<bool> AddCourseTopic(CourseTopicViewModel courseTopic)
        {
            bool isTopicExists =  _courseTopicRepository.AnyTopicByTopicName(courseTopic.Name);
            Guid CourseId = Guid.Parse(courseTopic.CourseId);
            Course course = _courseRepository.GetCourseDetailsByCourseId(CourseId);
            if (!isTopicExists)
            {
                Topic topic = new Topic()
                {
                    TopicId = Guid.NewGuid(),
                    Name = courseTopic.Name,
                    Description = courseTopic.Description,
                    Course = course,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    CreatedBy = courseTopic.CreatedBy,
                    ModifiedAt = null,
                    ModifiedBy = null
                };
                await _courseTopicRepository.AddCourseTopic(topic);
                return true;
            }
            else
            {
                return false;
            }
            


        }
        public async Task<bool> SoftDeleteTopic(string topicId)
        {

            Topic topic = await _courseTopicRepository.GetTopicByTopicId(Guid.Parse(topicId));
            topic.IsActive = false;
            int rowAffected=await _courseTopicRepository.UpdateCourseTopic(topic);
            if (rowAffected > 0)
            {
                return true;
            }
            return false;
            
        }




    }
}
