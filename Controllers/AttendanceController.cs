using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FarmOps.Models.AttendanceModels;
using FarmOps.Services;
using Newtonsoft.Json;
using FarmOps.Common;
using System.Data;
namespace FarmOps.Controllers
{
    [Authorize]
    public class AttendanceController : BaseController
    {
        private readonly AttendanceService _attendanceService;
        public AttendanceController(AttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
            var viewModel = new AttendaceInsertModel
            {
                // Initialize the AttendanceRecords list as an empty list to avoid NullReferenceException
                AttendanceRecords = new List<AttendanceViewListModel>()
            };
        }
        public IActionResult Index()
        {
            List<string> workers = [];
            string selectedWorker = null;
            List<ModelAttendanceViewRecord> allAttendanceData = new List<ModelAttendanceViewRecord>();
            try
            {
                var userType = GetUserType();
                var userId = GetUserId();

                //workers = _attendanceService.GetRelatedWorkers(userType, userId);

                //allAttendanceData = _attendanceService.GetAllAttendanceData(userType, userId);
            }
            catch (Exception ex)
            {
                // Handle any errors that might occur during logout (e.g., log them)
                ViewBag.Message = "Unable to load data!" + ex.Message;
            }

            // Passing the list of workers and the selected worker to the view
            ViewBag.Workers = workers;
            ViewBag.SelectedWorker = selectedWorker;
            
            return View(allAttendanceData);
        }

        [HttpPost]
        public IActionResult SaveTimeAttendance([FromBody] List<TimeAttendanceInsert> attendanceFormData)
        {
            try
            {
                if (attendanceFormData == null || !attendanceFormData.Any())
                {
                    return BadRequest(new { Success = false, Message = "No data provided." });
                }

                // If there are any validation issues with the received data, you can handle them here
                var validationErrors = attendanceFormData
                    .SelectMany((d, i) =>
                        !ModelState.IsValid
                        ? ModelState.Values.SelectMany(v => v.Errors)
                            .Select(e => $"Row {i + 1}: {e.ErrorMessage}")
                        : Array.Empty<string>())
                    .ToList();

                if (validationErrors.Any())
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = validationErrors
                    });
                }

                // Call the service to save the attendance data
                _attendanceService.SaveTimeAttendance(attendanceFormData);

                return Ok(new { Success = true, Message = $"Saved {attendanceFormData.Count} records successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An error occurred while saving attendance data: " + ex.Message });
            }
        }

        public string GetDutyDataJson()
        {
            string ret = "";
            if (GlobalVariables.DutyTable != null)
            {
                ret = JsonConvert.SerializeObject(GlobalVariables.DutyTable);
            }
            return ret;
        }
    }
}
