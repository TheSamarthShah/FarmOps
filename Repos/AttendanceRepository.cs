using FarmOps.Common;
using FarmOps.Models;
using FarmOps.Models.AttendanceModels;
using FarmOps.Models.LoginModels;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Data;

namespace FarmOps.Repos
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly DBContext _dbContext;
        private readonly string _connectionString;
        public AttendanceRepository(DBContext dBContext, string connectionString)
        {
            _dbContext = dBContext;
            _connectionString = connectionString;
        }
        public long GetNextAttendanceId()
        {
            long nextAttendanceId = 0;

            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    // Query the highest AttendanceId from the table, if no records exist, return 0.
                    var command = new SqlCommand("SELECT ISNULL(MAX(attendanceId), 0) FROM tbl_AppliedAttendance", connection);
                    nextAttendanceId = (long)command.ExecuteScalar();

                    // Increment the AttendanceId by 1 for the next record
                    nextAttendanceId += 1;
                }
                catch (Exception ex)
                {
                    // Log and rethrow the exception
                    Console.WriteLine($"Error while fetching next AttendanceId: {ex.Message}");
                    throw new ApplicationException("Error while fetching next AttendanceId: " + ex.Message);
                }
            }

            return nextAttendanceId;
        }

        public string SaveAppliedAttendance(List<AppliedAttendanceTblModel> data)
        {
            string insertAttendanceData = @"
        INSERT INTO tbl_AppliedAttendance (
AttendanceId,
            RosterID, 
            attendDate, 
            signinTime, 
            signoutTime, 
            totalWorkHours, 
            totalBreakHours, 
            breakIds, 
            pay, 
            job_cat, 
            on_paid_leave, 
            lineid, 
            jobnotpaid, 
            InsertId, 
            InsertDt, 
            UpdtId, 
            UpdtDt
        )
        VALUES (@attendanceId,@RosterID, @AttendanceDate, @SigninTime, @SignoutTime, @TotalWorkHours, 
                @TotalBreakHours, @BreakIds, @Pay, @JobCat, @OnPaidLeave, @LineId, 
                @JobNotPaid, @InsertId, @InsertDt, @UpdtId, @UpdtDt);";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Begin a transaction to ensure data consistency
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (var record in data)
                            {
                                using (var command = new SqlCommand(insertAttendanceData, connection, transaction))
                                {
                                    // Add parameters to the command to prevent SQL injection
                                    command.Parameters.AddWithValue("@attendanceId", record.AttendanceId);
                                    command.Parameters.AddWithValue("@RosterID", record.RosterID);
                                    command.Parameters.AddWithValue("@AttendanceDate", record.AttendanceDate);
                                    command.Parameters.AddWithValue("@SigninTime", record.SigninTime.HasValue ? (object)record.SigninTime.Value : DBNull.Value);
                                    command.Parameters.AddWithValue("@SignoutTime", record.SignoutTime.HasValue ? (object)record.SignoutTime.Value : DBNull.Value);
                                    command.Parameters.AddWithValue("@TotalWorkHours", record.TotalWorkHours.HasValue ? (object)record.TotalWorkHours.Value : DBNull.Value);
                                    command.Parameters.AddWithValue("@TotalBreakHours", record.TotalBreakHours.HasValue ? (object)record.TotalBreakHours.Value : DBNull.Value);
                                    command.Parameters.AddWithValue("@BreakIds", string.IsNullOrEmpty(record.BreakIds) ? DBNull.Value : (object)record.BreakIds);
                                    command.Parameters.AddWithValue("@Pay", record.Pay);
                                    command.Parameters.AddWithValue("@JobCat", record.JobCat);
                                    command.Parameters.AddWithValue("@OnPaidLeave", record.OnPaidLeave);
                                    command.Parameters.AddWithValue("@LineId", record.LineId);
                                    command.Parameters.AddWithValue("@JobNotPaid", record.JobNotPaid);
                                    command.Parameters.AddWithValue("@InsertId", "self");
                                    command.Parameters.AddWithValue("@InsertDt", record.InsertDt);
                                    command.Parameters.AddWithValue("@UpdtId", string.IsNullOrEmpty(record.UpdtId) ? DBNull.Value : (object)record.UpdtId);
                                    command.Parameters.AddWithValue("@UpdtDt", record.UpdtDt.HasValue ? (object)record.UpdtDt.Value : DBNull.Value);

                                    // Execute the insert command
                                    command.ExecuteNonQuery();
                                }
                            }

                            // Commit the transaction
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            // If something fails, roll back the transaction
                            transaction.Rollback();
                            Console.WriteLine($"Error during insert: {ex.Message}");
                            throw new ApplicationException($"An error occurred while inserting attendance data: {ex.Message}");
                        }
                    }
                }

                return "Attendance data saved successfully.";
            }
            catch (Exception ex)
            {
                // Log the error (could be logged into a file or logging system)
                Console.WriteLine($"Error during DB connection or transaction: {ex.Message}");
                throw new ApplicationException($"An error occurred during the database operation: {ex.Message}");
            }
        }


        public string SaveAttendanceDetails(AttendanceDetailTblModel data)
        {
            return "success";
        }

        public DataTable GetAssignedWorkers(string ClmName, string UserId)
        {
            try
            {
                DataTable DutyTbl = new DataTable();

                using (var connection = new SqlConnection(_connectionString))
                    {
                        try
                        {
                            connection.Open();
                            string query = DbConstants.selectTblDuty;

                            query = query.Replace("[IDCOLUMNNAME]", ClmName);

                            var command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@p0", UserId);
                            SqlDataAdapter sa = new SqlDataAdapter(command);
                            sa.Fill(DutyTbl);
                            sa.Dispose();

                        }
                        catch (Exception ex)
                        {
                            // Handle exceptions (logging, rethrow, etc.)
                            Console.WriteLine(ex.Message);
                        throw new ApplicationException(ex.Message);
                    }
                }
                return DutyTbl;
            }
            catch (Exception ex)
            {
                // For general exceptions, return a generic error status code (e.g., 500)
                throw new ApplicationException(ex.Message);
            }
        }

        public DataTable GetAllAttendanceData(string ClmName, string UserId)
        {
            try
            {
                DataTable allAttendanceData = new DataTable();

                using (var connection = new SqlConnection(_connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = DbConstants.selectAllAttendanceData;

                        query = query.Replace("[IDCOLUMNNAME]", ClmName);

                        var command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@p0", UserId);
                        SqlDataAdapter sa = new SqlDataAdapter(command);
                        sa.Fill(allAttendanceData);
                        sa.Dispose();

                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions (logging, rethrow, etc.)
                        Console.WriteLine(ex.Message);
                        throw new ApplicationException(ex.Message);
                    }
                }
                return allAttendanceData;
            }
            catch (Exception ex)
            {
                // For general exceptions, return a generic error status code (e.g., 500)
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
