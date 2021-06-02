using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using COMMAND;
using курсач3сервер;

namespace Server
{
    public class User
    {
        [Key]
        public int UserID {get; set; }

        [MaxLength(30)]
        [Required]
        public string login { get; set; }

        [MaxLength(250)]
        [Required]
        public string password { get; set; }

        [MaxLength(20)]
        [Required]
        public string surname { get; set; }

        [MaxLength(15)]
        [Required]
        public string name { get; set; }
      
        [MaxLength(15)]
        public string patronymic { get; set; }
        [Required]
        public DateTime birthday { get; set; }





        /*********************************************************************************************************/



        public User() { }
        public User(Socket socket) 
        {
            int bytes;
            byte[] data = new byte[512];
            bytes = socket.Receive(data);
            string[] userData = Encoding.Unicode.GetString(data, 0, bytes).Split('\t');
            login = userData[0];

            password = userData[1].Encrypt();
           

        }
        public void receive(Socket socket)
        {
            int size;
            byte[] bytes = new byte[256];

            size = socket.Receive(bytes);
            string FULLName = Encoding.Unicode.GetString(bytes, 0, size);

            string[] strings = FULLName.Split('\t');
            surname = strings[0];
            name = strings[1];
            if (strings.Length > 2) patronymic = strings[2];
            else patronymic = null;
            socket.Send(BitConverter.GetBytes(0));
            birthday = MyConvert.getDate(socket);



        }
        public void receiveName(Socket socket)
        {

            int bytes;
            byte[] data = new byte[256];
            bytes = socket.Receive(data);
            string FULLName = Encoding.Unicode.GetString(data, 0, bytes);
            string[] strings = FULLName.Split('\t');
            UserID = Convert.ToInt32(strings[0]);
            surname = strings[1];
            name = strings[2];
            if (strings.Length > 3) patronymic = strings[3];
            else patronymic = null;

        }
        public void receiveLogin(Socket socket)
        {


            int bytes;
            byte[] data = new byte[256];
            bytes = socket.Receive(data);
            string mydata = Encoding.Unicode.GetString(data, 0, bytes);
            string[] strings = mydata.Split('\t');
            UserID = Convert.ToInt32(strings[0]);
            login = strings[1];
            password = strings[2].Encrypt();

        }
        public void receiveBirhtday(Socket socket)
        {
            byte[] bytes = new byte[128];
            int size = socket.Receive(bytes);
            string receiveStr = Encoding.Unicode.GetString(bytes, 0, size);
            string[] strings = receiveStr.Split('\t');
            UserID = Convert.ToInt32(strings[0]);
            birthday = new DateTime(Convert.ToInt32(strings[1]), Convert.ToInt32(strings[2]), Convert.ToInt32(strings[3]));
        }
        public void send(Socket socket)
        {
            string str = UserID + "\t" + login + "\t" + password.Decrypt() + "\t" + surname + "\t" + name;
            if (patronymic != null)
            {
                str = str + "\t" + patronymic;
            }
            byte[] bytes = Encoding.Unicode.GetBytes(str);
            socket.Send(bytes);
            socket.Receive(bytes);
            MyConvert.sendDate(birthday, socket);
            socket.Receive(bytes);

        }
        public static int findUserLogin(string login)
        {
            using (SqlConnection connection = new SqlConnection(Program.ConnString()))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(UserCommand.findUserLoginCOMMAND(login), connection);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
        public void updateName()
        {
            using (SqlConnection connection = new SqlConnection(Program.ConnString()))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(UserCommand.updateNameCOMMAND(UserID, surname, name, patronymic), connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        public void updateLogin()
        {
            SqlConnection connection = new SqlConnection(Program.ConnString());
            connection.Open();
            SqlCommand command = new SqlCommand(UserCommand.updateDataCOMMAND(UserID, login, password), connection);
            command.ExecuteNonQuery();
            connection.Close();
        }
        public void updateBirthday()
        {
            SqlConnection connection = new SqlConnection(Program.ConnString());
            connection.Open();
            SqlCommand command = new SqlCommand(UserCommand.updateBirthdayCOMMAN(birthday, UserID), connection);
            command.ExecuteNonQuery();
            connection.Close();
        }
        public int CheckDBToReg()
        {
            SqlConnection connection = new SqlConnection(Program.ConnString());
            connection.Open();
            SqlCommand command = new SqlCommand(UserCommand.findLoginCOMMAND(login), connection);
            return Convert.ToInt32(command.ExecuteScalar());
            
        }

        public int CheckDBToAuth()
        {
            using (DBContext context = new DBContext())
            {
                for (int id = 1, i = 0; i < context.Users.Count(); id++)
                {
                    User user = context.Users.Find(id);
                    if (user != null)
                    {
                        
                        if (user.password.Decrypt() == password.Decrypt() && user.login == login)
                        {
                                                   
                            UserID = user.UserID;
                            return user.UserID;
                        }
                        i++;
                    }
                }
            }
            return 0;

        }
        public static int getCount()
        {
            SqlConnection connection = new SqlConnection(Program.ConnString());
            connection.Open();
            SqlCommand command = new SqlCommand(UserCommand.getCountCOMMAND(), connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }
        static public User getUser(int i)
        {
            DBContext context = new DBContext();
            return context.Users.Find(i);
        }
        public void getMyData(int UserID)
        {
            SqlConnection connection = new SqlConnection(Program.ConnString());
            connection.Open();
      
            SqlCommand command = new SqlCommand(UserCommand.COMMANDgetMyData(UserID), connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                readMyData(ref reader);
            }
       
        }
        private void readMyData(ref SqlDataReader reader)
        {
            UserID = reader.GetInt32(0);
            login = reader.GetString(1);
            password = reader.GetString(2).Decrypt();
            surname = reader.GetString(3);
            name = reader.GetString(4);
            if (reader[5] != DBNull.Value) patronymic = reader.GetString(5);
            else patronymic = null;
            birthday = reader.GetDateTime(6);



        }
        public void sendMyData(Socket socket)
        {
            string str = login + "\t" +password + "\t" + surname + "\t" + name;
            if (patronymic != null)
            {
                str = str + "\t" + patronymic;
            }
         
            byte[] bytes = Encoding.Unicode.GetBytes(str);
            socket.Send(bytes);
            MyConvert.sendDate(birthday, socket);
        }
    }

    public class Admin
    {
        [Key]
        public int AdminID { get; set; }

        [MaxLength(30)]
        [Required]
        public string position { get; set; }

        [Required]
        public User user { get; set; }


        /*********************************************************************************************************/

        public Admin(User user, string position)
        {
            this.user = user;
            this.position = position;
        }
        public Admin()
        {
            user = new User();
        }
       
        public static int getCount()
        {
            SqlConnection connection = new SqlConnection(Program.ConnString());
            connection.Open();
            SqlCommand command = new SqlCommand(AdminCommand.getCountCOMMAND(), connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public static int checkAdmin(User user)
        {
            SqlConnection connection = new SqlConnection(Program.ConnString());
            connection.Open();
            SqlCommand command = new SqlCommand(AdminCommand.COMMANDgetMyDataAuth(user.UserID), connection);
            return Convert.ToInt32(command.ExecuteScalar());
            
        }
        private void readMyData(ref SqlDataReader reader)
        {
            AdminID = reader.GetInt32(0);
            position = reader.GetString(1);
            user.UserID = reader.GetInt32(3);
            user.login = reader.GetString(4);
            user.password = reader.GetString(5).Decrypt();
            user.surname = reader.GetString(6);
            user.name = reader.GetString(7);
            if (reader[8] != DBNull.Value) user.patronymic = reader.GetString(8);
            else user.patronymic = null;
            user.birthday = reader.GetDateTime(9);
  


        }
        public void getMyData(int AdminID)
        {
            SqlConnection connection = new SqlConnection(Program.ConnString());
            connection.Open();
     
            SqlCommand command = new SqlCommand(AdminCommand.COMMANDgetMyData(AdminID), connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                readMyData(ref reader);
            }
           
        }
        public void sendMyData(Socket socket)
        {
            string str = position + "\t" + user.UserID + "\t" + user.login + "\t" + user.password + "\t" + user.surname + "\t" + user.name;
            if(user.patronymic != null)
            {
                str = str + "\t" + user.patronymic;
            }
          
            byte[] bytes = Encoding.Unicode.GetBytes(str);
            socket.Send(bytes);
            MyConvert.sendDate(user.birthday, socket);
        }
        public static int checkAdminUserID(int UserID)
        {
            SqlConnection connection = new SqlConnection(Program.ConnString());
            connection.Open();
            SqlCommand command = new SqlCommand(AdminCommand.COMMANDcheckAdminUserID(UserID), connection);
            return Convert.ToInt32(command.ExecuteScalar());
        }
        public static void insertNewAdmin(string position, int ID)
        {
            SqlConnection connection = new SqlConnection(Program.ConnString());
            connection.Open();
            SqlCommand command = new SqlCommand(AdminCommand.COMMANDInsertNewAdmin(position, ID), connection);
            command.ExecuteNonQuery();
        }
    }
}