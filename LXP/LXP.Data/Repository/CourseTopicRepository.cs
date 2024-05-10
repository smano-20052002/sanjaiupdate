using LXP.Common.Entities;
using LXP.Data.DBContexts;
using LXP.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXP.Data.Repository
{
    public class CourseTopicRepository:ICourseTopicRepository
    {
        private readonly LXPDbContext _lXPDbContext;
        public CourseTopicRepository(LXPDbContext lXPDbContext) 
        {
            this._lXPDbContext = lXPDbContext;
        }
        public async Task AddCourseTopic(Topic topic)
        {
            await _lXPDbContext.Topics.AddAsync(topic);
            await _lXPDbContext.SaveChangesAsync();
        }
        public async Task<int> UpdateCourseTopic(Topic topic)
        {
            _lXPDbContext.Topics.Update(topic);
            return await _lXPDbContext.SaveChangesAsync();
            
        }
        public async Task<Topic> GetTopicByTopicId(Guid topicId)
        {
            return await _lXPDbContext.Topics.FindAsync(topicId);
        }
        public bool AnyTopicByTopicId(Guid topicId)
        {
            return _lXPDbContext.Topics.Any(topic => topic.TopicId == topicId);
        }
        public object GetTopicDetailsByCourseId(string courseId)
        {
            var result = from course in _lXPDbContext.Courses
                                    where course.CourseId == Guid.Parse(courseId)
                                    select new
                                    {
                                        CourseId = course.CourseId,
                                        CourseTitle = course.Title,
                                        CourseIsActive = course.IsActive,
                                        Topics = (from topic in _lXPDbContext.Topics
                                                  where topic.CourseId == course.CourseId && topic.IsActive==true
                                                  select new
                                                  {
                                                      TopicId = topic.TopicId,
                                                      TopicName = topic.Name,
                                                      TopicDescription = topic.Description,
                                                      TopicIsActive = topic.IsActive,
                                                      Materials = (from material in _lXPDbContext.Materials
                                                                   join materialType in _lXPDbContext.MaterialTypes on material.MaterialTypeId equals materialType.MaterialTypeId
                                                                   where material.TopicId == topic.TopicId && material.IsActive==true
                                                                   select new
                                                                   {
                                                                       MaterialId = material.MaterialId,
                                                                       MaterialName = material.Name,
                                                                       MaterialType = materialType.Type,
                                                                       MaterialFilePath=material.FilePath,
                                                                       MaterialDuration = material.Duration,
                                                                       MaterialIsActive= material.IsActive,
                                                                       MaterialIsAvailable= material.IsAvailable
                                                                   }).ToList()
                                                  }).ToList()
                                    };


            
            return result;
        }
        public bool AnyTopicByTopicName(string topicName)
        {
            return _lXPDbContext.Topics.Any(topic=>topic.Name==topicName);
        }
    }
}
