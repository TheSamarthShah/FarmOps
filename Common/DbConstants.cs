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
    }
}
