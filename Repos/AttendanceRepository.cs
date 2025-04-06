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

        public string SaveAppliedAttendance(List<FAttendanceTbl> data)
        {
            string insertFAttendanceData = @"
INSERT INTO tbl_FAttendance (
    RosterId, 
    AttendanceDate, 
    StartTime, 
    EndTime, 
    TotalHours, 
    BreakTime, 
    BlockId, 
    Pay, 
    AttendanceType, 
    JobId, 
    PaidBreak, 
    AttendanceSignPic, 
    LineId, 
    JobPaid, 
    Remarks, 
    AppliedBy, 
    ApprovedStatus, 
    ApprovedBy, 
    ApprovedDt
)
VALUES (
    @RosterId, 
    @AttendanceDate, 
    @StartTime, 
    @EndTime, 
    @TotalHours, 
    @BreakTime, 
    @BlockId, 
    @Pay, 
    @AttendanceType, 
    @JobId, 
    @PaidBreak, 
    @AttendanceSignPic, 
    @LineId, 
    @JobPaid, 
    @Remarks, 
    @AppliedBy, 
    @ApprovedStatus, 
    @ApprovedBy, 
    @ApprovedDt
);";

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
                            foreach (var record in data)  // Assuming 'data' is your list of FAttendanceTbl objects
                            {
                                using (var command = new SqlCommand(insertFAttendanceData, connection, transaction))
                                {
                                    // Add parameters to the command to prevent SQL injection
                                    command.Parameters.AddWithValue("@RosterId", record.RosterId);
                                    command.Parameters.AddWithValue("@AttendanceDate", record.AttendanceDate);
                                    command.Parameters.AddWithValue("@StartTime", record.StartTime);
                                    command.Parameters.AddWithValue("@EndTime", record.EndTime);
                                    command.Parameters.AddWithValue("@TotalHours", record.TotalHours.HasValue ? (object)record.TotalHours.Value : DBNull.Value);
                                    command.Parameters.AddWithValue("@BreakTime", record.BreakTime.HasValue ? (object)record.BreakTime.Value : DBNull.Value);
                                    command.Parameters.AddWithValue("@BlockId", record.BlockId.HasValue ? (object)record.BlockId.Value : DBNull.Value);
                                    command.Parameters.AddWithValue("@Pay", record.Pay.HasValue ? (object)record.Pay.Value : DBNull.Value);
                                    command.Parameters.AddWithValue("@AttendanceType", string.IsNullOrEmpty(record.AttendanceType) ? DBNull.Value : (object)record.AttendanceType);
                                    command.Parameters.AddWithValue("@JobId", record.JobId.HasValue ? (object)record.JobId.Value : DBNull.Value);
                                    command.Parameters.AddWithValue("@PaidBreak", record.PaidBreak.HasValue ? (object)record.PaidBreak.Value : DBNull.Value);
                                    command.Parameters.AddWithValue("@AttendanceSignPic", string.IsNullOrEmpty(record.AttendanceSignPic) ? DBNull.Value : (object)record.AttendanceSignPic);
                                    command.Parameters.AddWithValue("@LineId", record.LineId.HasValue ? (object)record.LineId.Value : DBNull.Value);
                                    command.Parameters.AddWithValue("@JobPaid", record.JobPaid);
                                    command.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(record.Remarks) ? DBNull.Value : (object)record.Remarks);
                                    command.Parameters.AddWithValue("@AppliedBy", string.IsNullOrEmpty(record.AppliedBy) ? DBNull.Value : (object)record.AppliedBy);
                                    command.Parameters.AddWithValue("@ApprovedStatus", record.ApprovedStatus);
                                    command.Parameters.AddWithValue("@ApprovedBy", string.IsNullOrEmpty(record.ApprovedBy) ? DBNull.Value : (object)record.ApprovedBy);
                                    command.Parameters.AddWithValue("@ApprovedDt", record.ApprovedDt.HasValue ? (object)record.ApprovedDt.Value : DBNull.Value);

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
