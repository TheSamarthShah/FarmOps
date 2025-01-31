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
        private readonly IAttendanceService _attendanceService;
        public AttendanceController(IAttendanceService attendanceService)
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

                workers = _attendanceService.GetRelatedWorkers(userType, userId);

                allAttendanceData = _attendanceService.GetAllAttendanceData(userType, userId);
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
        
        public IActionResult SaveAttendance(string data)
        {
            try
            {
                var attendanceInsertModel = JsonConvert.DeserializeObject<AttendaceInsertModel>(data);
                _attendanceService.SaveAttendance(attendanceInsertModel);
            }
            catch (Exception ex) {
                ViewBag.Message = "[Faliure] - Opps! Something went wrong. Please try again later.";
            }

            var response = new
            {
                Success = true,
                Message = "Attendance saved successfully!"
            };

            // Return success response
            return Ok(response);
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
