namespace FarmOps.Common
{
    public static class DbConstants
    {
        public static string selectBasicUserDetail = @"
        SELECT  TOP 1
                userTbl.[IDCOLUMNNAME] as Id,
                userTbl.FirstName,
                userTbl.LastName,
                userTbl.MiddleName,
                userTbl.Phone as PhoneNo,
                userTbl.DOB,
                userTbl.Picture as PhotoUrl,
                userTbl.[Passport Number] PassportNo
        FROM    [TABLENAME] userTbl
        WHERE   userTbl.[IDCOLUMNNAME] = @p0";

        public static string selectTblDuty = @"
        SELECT  duty.RosterID, 
                duty.ShiftID, 
                duty.Day, 
                duty.WorkerID, 
                duty.GrowerID, 
                duty.supervisorId, 
                duty.MonitorID, 
                duty.cat, 
                duty.team, 
                duty.mcatid 
        FROM    dbo.tbl_Duty AS duty
        WHERE   duty.[IDCOLUMNNAME] = @p0";

        public static string selectAllAttendanceData = @"
        SELECT a.*, ad.*
        FROM [dbo].[tbl_AppliedAttendance] a
        INNER JOIN [dbo].[tbl_Duty] d
            ON a.RosterID = d.RosterID
        LEFT JOIN [dbo].[tbl_AttendanceDetail] ad
            ON a.attendanceId = ad.attendanceId
        WHERE d.[IDCOLUMNNAME] = @p0;";

        public static string insertAttendanceData = @"
            INSERT INTO Attendance (
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
            VALUES (@RosterID, @AttendanceDate, @SigninTime, @SignoutTime, @TotalWorkHours, 
                    @TotalBreakHours, @BreakIds, @Pay, @JobCat, @OnPaidLeave, @LineId, 
                    @JobNotPaid, @InsertId, @InsertDt, @UpdtId, @UpdtDt);";

        public static string checkLogin = @"SELECT userId 
                                            FROM tbl_FLogin 
                                            WHERE emailAddress = @p0 
                                            AND userPassword = @p1
                                            AND userType = @p2;
                                            ";
    }
}
