﻿using AttendanceControlSystem.Entity;
using AttendanceControlSystem.Interfaces;
using AttendanceControlSystem.Models.StudentModels;
using AttendanceControlSystem.Utility;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceControlSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RoleConstants.Admin)]
    public class StudentController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IStudentService _studentService;
        public StudentController(IStudentService studentService, IMapper mapper)
        {
            _studentService = studentService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<List<Student>> GetAllAsync() =>
            await _studentService.GetAllStudentAsync();

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync([FromRoute] string id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
                return NotFound($"There is no student with such id '{id}'");

            return Ok(student);
        }

        [HttpGet("GetStundetByParameters")]
        public async Task<IActionResult> FindStudentByParametesAsync([FromQuery] SearchStudentModel searchStudentModel)
        {
            var student = await _studentService.GetStudentByParametetsAsync(i => i.FirstName == searchStudentModel.FirstName && i.LastName == searchStudentModel.LastName && i.MiddleName == searchStudentModel.MiddleName && i.Group == searchStudentModel.Group && i.Course == searchStudentModel.Course);
            if (student == null)
                throw new Exception("There is no student with such parameters");

            return Ok(student);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateStudentModel createStudentModel)
        {
            var student = _mapper.Map<Student>(createStudentModel);

            await _studentService.CreateAsync(student);
            return Ok(student);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] StudentModel studentModel)
        {
            var student = _mapper.Map<Student>(studentModel);
            await _studentService.UpdateAsync(studentModel.Id, student);
            return Ok(student);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
                return NotFound($"There is no student with such id '{id}'");

            await _studentService.RemoveAsync(id);

            if (System.IO.File.Exists(student.ImagePath))
            {
                try
                {
                    System.IO.File.Delete(student.ImagePath);
                }
                catch
                {
                    throw new Exception("Removing photo failed");
                }
            }
            else 
            {
                throw new Exception("Current photo does not exist");
            }

            return NoContent();
        }
    }
}
