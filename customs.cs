using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Text;
using COMMAND;
using курсач3сервер;

namespace Server
{

    public class Location
    {
        public int LocationID { get; set; }
        [MaxLength(7)]
        [Required]
        public string Country { get; set; }
        [MaxLength(11)]
        [Required]
        public string Region { get; set; }
        [MaxLength(20)]
        [Required]
        public string District { get; set; }

        public void Send(Socket socket)
        {
            socket.Send(Encoding.Unicode.GetBytes(LocationID + "\t" + Country + "\t" + Region + "\t" + District));
        }


        public int CheckPoint()
        {
            var connection = new SqlConnection(Program.ConnString());
            connection.Open();
            var command = new SqlCommand(CustomsControlPointCommand.checkNewPointsCOMMAND(this), connection);
         
            return Convert.ToInt32(command.ExecuteScalar());

        }
        public void FindId()
        {
            var connection = new SqlConnection(Program.ConnString());
            connection.Open();
            var command = new SqlCommand(LocationCommand.findCOMMAND(this), connection);
           
            LocationID = Convert.ToInt32(command.ExecuteScalar());
           
        }


    }
    public class CustomsControlPoint
    {
        [Key]
        public int CustomsControlPointID { get; set; }


        [MaxLength(30)]
        [Required]
        public string name { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        [Required]
        
        public Location location { get; set; }
        

        public CustomsControlPoint()
        {
            location = new Location();
        }

        public CustomsControlPoint(Socket socket)
        {
            location = new Location();
            var bytes = new byte[256];
            var size = socket.Receive(bytes);
            var data = Encoding.Unicode.GetString(bytes, 0, size).Split('\t');
            name = data[0];
            location.Country = data[1];
            location.Region = data[2];
            location.District = data[3];
            TimeStart = new TimeSpan(Convert.ToInt32(data[4]), Convert.ToInt32(data[5]), 0);
            TimeEnd = new TimeSpan(Convert.ToInt32(data[6]), Convert.ToInt32(data[7]), 0);
        }

       
        public void InsertPoint()
        {
            var connection = new SqlConnection(Program.ConnString());
            connection.Open();
            var command = new SqlCommand(CustomsControlPointCommand.InsertNewPointsCOMMAND(this), connection);
            command.ExecuteScalar();

        }
        public string Send()
        {
            var str = CustomsControlPointID + "\t" + name + "\t" + location.LocationID + "\t" + location.Country + "\t" + location.Region + "\t" + location.District;
            str += "\t" + TimeStart.Hours + "\t" + TimeStart.Minutes + "\t" + TimeEnd.Hours + "\t" + TimeEnd.Minutes;
            return str;
        }

        public void Read(SqlDataReader reader)
        {

            location = new Location();
            CustomsControlPointID = reader.GetInt32(0);
            name = reader.GetString(1);
            TimeStart = reader.GetTimeSpan(2);
            TimeEnd = reader.GetTimeSpan(3);
            location.LocationID = reader.GetInt32(5);
            location.Country = reader.GetString(6);
            location.Region = reader.GetString(7);
            location.District = reader.GetString(8);

        }

        public static void UpdateName(string name, int ID)
        {
            var connection = new SqlConnection(Program.ConnString());
            connection.Open();
            var command = new SqlCommand(CustomsControlPointCommand.updateNameCOMMAND(name, ID), connection);
            command.ExecuteNonQuery();
        }
        public static void UpdateTime(TimeSpan[] time, int ID)
        {
            var connection = new SqlConnection(Program.ConnString());
            connection.Open();
            var command = new SqlCommand(CustomsControlPointCommand.updateTimeCOMMAND(time, ID), connection);
            command.ExecuteNonQuery();
        }
        public static void Delete(int id)
        {
            var connection = new SqlConnection(Program.ConnString());
            connection.Open();
            var command = new SqlCommand(CustomsControlPointCommand.deleteCOMMAND(id), connection);
            command.ExecuteNonQuery();
        }

    }


    public class Vote
    {
        [Key]
        public int VoteID { get; set; }
        
        [Required]
        public int position { get; set; }
        [Required]
        public User user { get; set; }
        [Required]
        public CustomsControlPoint point {get;set;}

        public Vote()
        {
            user = new User();
            point = new CustomsControlPoint();
        }



        public void GetData(string data, int userId)
        {
            var myData = data.Split('\t');
            point.CustomsControlPointID = Convert.ToInt32(myData[0]);
            position = Convert.ToInt32(myData[1]);
            user.UserID = userId;
        }
        public void InsertToDatabase()
        {
            var connection = new SqlConnection(Program.ConnString());
            connection.Open();
            var command = new SqlCommand(VoteCommand.insertCOMMAND(this), connection);
            command.ExecuteNonQuery();
        }
        public static bool CheckMe(int id)
        {
            var connection = new SqlConnection(Program.ConnString());
            connection.Open();
            var command = new SqlCommand(VoteCommand.countCOMMAND(id), connection);
            return Convert.ToInt32(command.ExecuteScalar()) != 0;
        }
        public void Read(SqlDataReader reader)
        {
            VoteID = Convert.ToInt32(reader[0]);
            user.UserID = Convert.ToInt32(reader[1]);
            position= Convert.ToInt32(reader[2]);
            point.CustomsControlPointID= Convert.ToInt32(reader[3]);
            point.name = reader[4].ToString();
        }
        public string Send()
        {
            var data = VoteID + "\t";
            data += user.UserID + "\t";
            data += position + "\t";
            data += point.CustomsControlPointID + "\t";
            data += point.name + "\n";
            return data;

        }

        public static bool VoteCheck(int id)
        {
            var connection = new SqlConnection(Program.ConnString());
            connection.Open();
            var command = new SqlCommand(VoteCommand.countPointCOMMAND(id), connection);
            return Convert.ToInt32(command.ExecuteScalar()) != 0;
        }
        public static void Delete()
        {
            var connection = new SqlConnection(Program.ConnString());
            connection.Open();
            var command = new SqlCommand(VoteCommand.deleteCOMMAND(), connection);
            command.ExecuteNonQuery();
        }
    }
}