﻿using API.DTO;
using API.ErrorResponse;
using AutoMapper;
using Entity;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class LecturesController : BaseController
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        public LecturesController(StoreContext context, IMapper mapper, UserManager<User> userManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _context = context;

        }

        [Authorize]

        [HttpGet("{courseId}")]

        public async Task<ActionResult<UserLectureDto>> GetLectures(Guid courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var sections = await _context.Sections.Where(x => x.CourseId == course.Id).Include(c => c.Lectures).ToListAsync();

            var userCourse = _context.UserCourses.Where(x => x.User == user).Where(x => x.Course == course).First();

            return new UserLectureDto
            {
                id = courseId,
                CourseName = course.Title,
                Sections = _mapper.Map<List<Section>, List<SectionDto>>(sections),
                CurrentLecture = userCourse.CurrentLecture
            };

        }

        [Authorize]
        [HttpPut("setCurrentLecture")]

        public async Task<ActionResult> UpdateCurrentLecture([FromBody] UpdateLectureDto updateLecture)
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var userCourse = _context.UserCourses.Where(x => x.User == user).Where(x => x.CourseId == updateLecture.CourseId).First();

            userCourse.CurrentLecture = updateLecture.LectureId;

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest(new ApiResponse(400, "Problem updating current lecture"));

        }

        ///
        [Authorize(Roles = "Instructor")]
        [HttpPost]
        public async Task<ActionResult<string>> AddSection(AddSectionDto sectionDto)
        {
            var course = await _context.Courses.FindAsync(sectionDto.CourseId);

            var section = new Section
            {
                Name = sectionDto.SectionName,
                Course = course
            };

            _context.Sections.Add(section);

            foreach (var item in sectionDto.Lectures)
            {
                var lecture = new Lecture
                {
                    Title = item.Title,
                    Url = item.Url,
                    Section = section
                };

                _context.Lectures.Add(lecture);
            }

            var response = await _context.SaveChangesAsync() > 0;

            if (response) return "Section Added Successfully";

            return BadRequest(new ApiResponse(400, "Problem creating Section"));
        }
    }
}
