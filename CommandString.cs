
using System;
using Server;
using курсач3сервер;

namespace COMMAND
{
    public static class UserCommand
    {
        public static string findLoginCOMMAND(string login)
        {
            return "SELECT COUNT(*) FROM Users WHERE login = '" + login + "'";
        }
        public static string getCountCOMMAND()
        {
            return "SELECT COUNT(*) FROM Users";
        }

        public static string findUserCOMMAND(string login, string password)
        {
            return $"SELECT UserID FROM Users WHERE login = '{login}' AND password = '{password}'";
        }
        public static string findUserLoginCOMMAND(string login)
        {
            return "SELECT UserID FROM Users WHERE login = '" + login + "'";
        }
        public static string updateNameCOMMAND(int UserID, string surname, string name, string patronymic)
        {
            if (patronymic != null) return "UPDATE Users set surname = '" + surname + "', name = '" + name + "', patronymic = '" + patronymic + "' where UserID = " + UserID;
            return "UPDATE Users set surname = '" + surname + "', name = '" + name + "', patronymic = null where UserID = " + UserID;
        }
        public static string updateDataCOMMAND(int UserID, string login, string password)
        {
            return $"UPDATE Users set login = '{login}', password = '{password}' where UserID = {UserID}";

        }
        public static string updateBirthdayCOMMAN(DateTime date, int UserID)
        {
            return "SET DATEFORMAT dmy UPDATE Users set birthday = '" + date.Day + "." + date.Month + "." + date.Year + "' where UserID = " + UserID;
        }
        public static string COMMANDgetMyData(int userID)
        {
            return "SELECT * FROM Users WHERE UserID = " + userID;
        }
    }

    public static class AdminCommand
    {
        public static string COMMANDInsertNewAdmin(string position, int ID)
        {
            return "INSERT INTO Admins (position, user_UserID) VALUES('" + position + "', " + ID + ")";
        }
        public static string COMMANDgetMyData(int AdminID)
        {
            return "SELECT * FROM Admins inner join Users on Admins.user_UserID = Users.UserID WHERE AdminID = " + AdminID;
        }
        public static string COMMANDgetMyDataAuth(int userID)
        {
            return "SELECT * FROM Admins inner join Users on Admins.user_UserID = Users.UserID WHERE user_UserID = " + userID;
        }
        public static string COMMANDcheckAdminUserID(int UserID)
        {
            return "SELECT COUNT(*) FROM Admins WHERE user_UserID = " + UserID;
        }

       
        public static string getCountCOMMAND()
        {
            return "SELECT COUNT(*) FROM Admins";
        }
    }

    public static class CustomsControlPointCommand
    {
        public static string InsertNewPointsCOMMAND(CustomsControlPoint point)
        {
             return "INSERT INTO CustomsControlPoints VALUES('" + point.name + "', '" + point.TimeStart.Hours + ":" + point.TimeStart.Minutes + "', '" + point.TimeEnd.Hours + ":" + point.TimeEnd.Minutes + "', " + point.location.LocationID + ")";
            
        }
        public static string checkNewPointsCOMMAND(Location location)
        {
            return "SELECT CustomsControlPointID FROM CustomsControlPoints INNER JOIN Locations ON CustomsControlPoints.location_LocationID = Locations.LocationID WHERE Locations.Country = '" + location.Country + "' AND Locations.Region = '" + location.Region + "' AND Locations.District = '" + location.District + "'";
        }
        public static string findCOMMAND()
        {
            return "SELECT* FROM CustomsControlPoints INNER JOIN Locations ON CustomsControlPoints.location_LocationID = Locations.LocationID ORDER BY Locations.LocationID";
        }
        public static string updateNameCOMMAND(string name, int id)
        {
            return $"UPDATE CustomsControlPoints SET name = '{name}' WHERE CustomsControlPointID = {id}";
        }
        public static string updateTimeCOMMAND(TimeSpan[] time, int id)
        {
            return $"UPDATE CustomsControlPoints SET TimeStart = '{time[0].Hours}:{time[0].Minutes}', TimeEnd = '{time[1].Hours}:{time[1].Minutes}' WHERE CustomsControlPointID = {id}";
        }
        public static string deleteCOMMAND(int ID)
        {
            return $"DELETE FROM CustomsControlPoints WHERE CustomsControlPointID = {ID}";
        }
        public static string getFullData(int ID)
        {
            return $"select * from CustomsControlPoints inner join Locations on CustomsControlPoints.location_LocationID = Locations.LocationID where CustomsControlPointID = {ID}";
        }
    }

    public static class LocationCommand
    {
        public static string findCOMMAND(Location location)
        {
            return $"SELECT LocationID FROM Locations WHERE Country = '{location.Country}' AND Region = '{location.Region}' AND District = '{location.District}'";
        }
    }

    public static class VoteCommand
    {
        public static string insertCOMMAND(Vote vote)
        {
            return $"INSERT INTO Votes VALUES({vote.user.UserID}, {vote.position}, {vote.point.CustomsControlPointID})";

        }
        public static string countCOMMAND(int ID)
        {
            return $"SELECT COUNT(*) FROM Votes WHERE user_UserID = {ID}";

        }
        public static string getDataCOMMAND()
        {
            return "select Votes.*, CustomsControlPoints.name from Votes inner join CustomsControlPoints on Votes.point_CustomsControlPointID = CustomsControlPoints.CustomsControlPointID";

        }
        public static string countPointCOMMAND(int ID)
        {
            return $"SELECT COUNT(*) FROM Votes WHERE point_CustomsControlPointID = {ID}";

        }
        public static string getCOMMAND()
        {
            return " select point_CustomsControlPointID, position from Votes order by point_CustomsControlPointID";
        }
        public static string deleteCOMMAND()
        {
            return "delete from Votes";
        }
      
    }


    public static class CustomControlPointNewTimeCommand
    {
        public static string removeCOMMAND()
        {
            return "delete from CustomControlPointNewTimes";
        }
        public static string insertCOMMAND(CustomControlPointNewTime point)
        {

            string[] data = Math.Round(point.percent,2).ToString().Split(',');
            string data1 = data[0] == null ? "0" : data[0];
            string data2 = data.Length == 1 ? "0" : data[1];
            return $"insert into CustomControlPointNewTimes  values('{point.newStart.Hours}:{point.newStart.Minutes}', '{point.newEnd.Hours}:{point.newEnd.Minutes}', {point.point.CustomsControlPointID}, {data1}.{data2}, {point.position})";

        }
        public static string finedCOMMAND()
        {

            return "select * from CustomControlPointNewTimes inner join CustomsControlPoints on CustomControlPointNewTimes.point_CustomsControlPointID = CustomsControlPoints.CustomsControlPointID inner join Locations on CustomsControlPoints.location_LocationID = Locations.LocationID";
        }
        public static string deleteCOMMAND()
        {
            return "delete from CustomControlPointNewTimes";
        }
    }
}