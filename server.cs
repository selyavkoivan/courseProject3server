using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using COMMAND;
using курсач3сервер;

namespace Server
{
   
    class ClientObject
    {

        public Socket socket;
        public ClientObject(Socket socket)
        {
            this.socket = socket;
        }

        private void add()
        {

            User user = new User(socket);

            if (User.getCount() != 0)
            {

                int result = user.CheckDBToReg();
                if (result != 0)
                {
                    socket.Send(BitConverter.GetBytes(false));
  
                    return;
                }
            }
            socket.Send(BitConverter.GetBytes(true));

            byte[] bytes = new byte[64];
            int size = socket.Receive(bytes);
            if (!BitConverter.ToBoolean(bytes, 0))
            {
             
                return;
            }
            user.receive(socket);
        
            if (Admin.getCount() == 0)
            {

                Admin admin = new Admin(user, "Главный администратор");
                using (DBContext context = new DBContext())
                {
                    context.Admins.Add(admin);
                    context.SaveChanges();
                }
              
                return;
            }
            using (DBContext context = new DBContext())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
        


        }
        private void send()
        {
            int size = User.getCount() - Admin.getCount();
       
            
            socket.Send(BitConverter.GetBytes(size));
            for (int ID = 1, i = 0; i < size; ID++)
            {
                if (Admin.checkAdminUserID(ID) != 0) continue;
                User user = User.getUser(ID);
                if (user != null)
                {
                    user.send(socket);
                    i++;



                }



            }


        }

        private bool auth()
        {
            User user = new User(socket);
            
        
            int result = user.CheckDBToAuth();
            socket.Send(BitConverter.GetBytes(result));
            if (result != 0)
            {
                int adminID = Admin.checkAdmin(user);

              
                socket.Send(BitConverter.GetBytes(adminID));
                return true;
            }

            return false;
        }
        private void sendMyAdminData()
        {
            byte[] bytes = new byte[64];
            socket.Receive(bytes);
            Admin admin = new Admin();
            admin.getMyData(BitConverter.ToInt32(bytes, 0));
            admin.sendMyData(socket);
        }
        private void sendMyUserData()
        {
            byte[] bytes = new byte[64];
            socket.Receive(bytes);
            User user = new User();
            user.getMyData(BitConverter.ToInt32(bytes, 0));
            user.sendMyData(socket);
        }
        private void getNewName()
        {
            User user = new User();
            user.receiveName(socket);
            user.updateName();
        }

        private void getNewLogin()
        {
            User user = new User();
            user.receiveLogin(socket);
            user.updateLogin();
        }
        private void getNewBirthday()
        {
            User user = new User();
            user.receiveBirhtday(socket);
            user.updateBirthday();
        }
        private void giveRights()
        {
            byte[] bytes = new byte[256];
            int size = socket.Receive(bytes);
            string[] strings = Encoding.Unicode.GetString(bytes, 0, size).Split('\t');
            Admin.insertNewAdmin(strings[1], User.findUserLogin(strings[0]));


        }
        private void sendPoints()
        {


            List<CustomsControlPoint> points = new List<CustomsControlPoint>();
            using (SqlConnection connection = new SqlConnection(Program.ConnString()))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(CustomsControlPointCommand.findCOMMAND(), connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CustomsControlPoint point = new CustomsControlPoint();
                    point.Read(reader);
                    points.Add(point);

                }
            }
       
            string str = points.Count + "\n";
            for (int i = 0; i < points.Count; i++)
            {
                str += points[i].Send() + "\n";
            }
       
            socket.Send(Encoding.Unicode.GetBytes(str));
        




         

        }
        private void sendLocations()
        {

            using (DBContext context = new DBContext())
            {
                int size = context.Locations.Count();
                socket.Send(BitConverter.GetBytes(size));
                for (int ID = 1, i = 0; i < size; ID++)
                {
                    Location location = context.Locations.Find(ID);
                    if (location != null)
                    {
                        location.Send(socket);
                        byte[] bytes = new byte[12];
                        socket.Receive(bytes);
                        i++;

                    }

                }

            }



        }
        private void getPoints()
        {

            CustomsControlPoint point = new CustomsControlPoint(socket);
        
            int ID = point.location.CheckPoint();
            socket.Send(BitConverter.GetBytes(ID));
            if (ID == 0)
            {
                point.location.FindId();

                if (point.location.LocationID != 0)
                {
                    point.InsertPoint();

                    sendPoints();
                }

                return;
            }
        }
        private void setNewPointName()
        {
            byte[] bytes = new byte[128];
            int size = socket.Receive(bytes);
            string[] data = Encoding.Unicode.GetString(bytes, 0, size).Split('\t');
            CustomsControlPoint.UpdateName(data[1], Convert.ToInt32(data[0]));
        }
        private void setNewPointTime()
        {
            byte[] bytes = new byte[128];
            int size = socket.Receive(bytes);
            string[] data = Encoding.Unicode.GetString(bytes, 0, size).Split('\t');
            TimeSpan[] time = new TimeSpan[2];
            time[0] = new TimeSpan(Convert.ToInt32(data[1]), Convert.ToInt32(data[2]), 0);
            time[1] = new TimeSpan(Convert.ToInt32(data[3]), Convert.ToInt32(data[4]), 0);
            CustomsControlPoint.UpdateTime(time, Convert.ToInt32(data[0]));
        }
        private void delPoint()
        {
            byte[] bytes = new byte[128];
            int size = socket.Receive(bytes);
            int ID = BitConverter.ToInt32(bytes, 0);
            CustomsControlPoint.Delete(ID);
        }

        private void recievePriority()
        {
            using (DBContext context = new DBContext())
            {
                byte[] bytes = new byte[8196];
                int size = socket.Receive(bytes);
            
                string[] data = Encoding.Unicode.GetString(bytes, 0, size).Split('\n');
                int count = Convert.ToInt32(data[0]);
                int userID = Convert.ToInt32(data[1]);
                for (int i = 2; i <= count + 1; i++)
                {
                    Vote vote = new Vote();
                    vote.GetData(data[i], userID);
                    vote.InsertToDatabase();
                }
            }

        }
        private void checkMe()
        {
            byte[] bytes = new byte[16];
            socket.Receive(bytes);
            socket.Send(BitConverter.GetBytes(Vote.CheckMe(BitConverter.ToInt32(bytes, 0))));
        }
        private void sendVotes()
        {
            using(DBContext context = new DBContext())
            {
                SqlConnection connection = new SqlConnection(Program.ConnString());
                connection.Open();
                SqlCommand command = new SqlCommand(VoteCommand.getDataCOMMAND(), connection);

                SqlDataReader reader = command.ExecuteReader();
                string data = context.votes.Count() + "\n";
                while(reader.Read())
                {
                    Vote votes = new Vote();
                    votes.Read(reader);
                    data += votes.Send();
                }
                socket.Send(Encoding.Unicode.GetBytes(data));
                
            }
        }

        private void findTheBest()
        {
            VoteCalculation calculation = new VoteCalculation();
            calculation.Matrix();
            calculation.MatrixToList();
            calculation.TotalList();
            calculation.ItsNewTime();
            calculation.NewTimePointToDb();
            sentNewTimePoint();
        }

        private void sendPointsVote()

        {

            DBContext context = new DBContext();
            List<CustomsControlPoint> points = new List<CustomsControlPoint>();
            using (SqlConnection connection = new SqlConnection(Program.ConnString()))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(CustomsControlPointCommand.findCOMMAND(), connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CustomsControlPoint point = new CustomsControlPoint();
                    point.Read(reader);
                    if(context.votes.Count() == 0) points.Add(point);
                    else if (Vote.VoteCheck(point.CustomsControlPointID)) points.Add(point);

                }
            }

            string str = points.Count + "\n";
            for (int i = 0; i < points.Count; i++)
            {
                str += points[i].Send() + "\n";
            }
          
            socket.Send(Encoding.Unicode.GetBytes(str));

            
        }


        private void sentNewTimePoint()
        {
            List<CustomControlPointNewTime> list = new List<CustomControlPointNewTime>();
          
                    SqlConnection connection = new SqlConnection(Program.ConnString());
                    connection.Open();
                    SqlCommand command = new SqlCommand(CustomControlPointNewTimeCommand.finedCOMMAND(), connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        CustomControlPointNewTime point = new CustomControlPointNewTime(reader);
                        list.Add(point);
                    }
                            


            string data = list.Count() + "\n";
            foreach (var it in list)
            {
                data += it.Add();
               
            }
            
            socket.Send(Encoding.Unicode.GetBytes(data));
        
        }
        private void setNewTime()
        {
            byte[] bytes = new byte[65536];
            int size = socket.Receive(bytes);
            string[] data = Encoding.Unicode.GetString(bytes, 0, size).Split('\n');
            for(int i = 1; i <= Convert.ToInt32(data[0]); i++)
            {
                string[] newData = data[i].Split('\t');
                TimeSpan[] time = new TimeSpan[2];
                time[0] = new TimeSpan(Convert.ToInt32(newData[1]), Convert.ToInt32(newData[2]), 0);
                time[1] = new TimeSpan(Convert.ToInt32(newData[3]), Convert.ToInt32(newData[4]), 0);
                CustomsControlPoint.UpdateTime(time, Convert.ToInt32(newData[0]));
            }
            Vote.Delete();
            CustomControlPointNewTime.Delete();
        }
        private bool adminCommandMenu()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Ожидание команды");
                    byte[] bytes = new byte[128];
                    int size = socket.Receive(bytes);
                    string choice = Encoding.Unicode.GetString(bytes, 0, size);
                    Console.Write("Полученная команда : ");
                    switch (choice)
                    {
                        case "exit":
                            Console.WriteLine(choice);
                            Console.WriteLine("Клиент отключился");

                            return false;
                        case "":
                            Console.WriteLine("exit");
                            Console.WriteLine("Клиент отключился");

                            return false;
                        case "loadUserTable":
                            Console.WriteLine(choice);
                            send();
                            break;
                        case "loadMyData":
                            Console.WriteLine(choice);
                            sendMyAdminData();
                            break;
                        case "sendNewName":
                            Console.WriteLine(choice);
                            getNewName();
                            socket.Send(BitConverter.GetBytes(0));
                            sendMyAdminData();
                            break;
                        case "sendNewLogin":
                            Console.WriteLine(choice);
                            getNewLogin();
                            socket.Send(BitConverter.GetBytes(0));
                            sendMyAdminData();
                            break;
                        case "sendNewBirthday":
                            Console.WriteLine(choice);
                            getNewBirthday();
                            socket.Send(BitConverter.GetBytes(0));
                            sendMyAdminData();
                            break;
                        case "giveRights":
                            Console.WriteLine(choice);
                            giveRights();
                            socket.Send(BitConverter.GetBytes(0));
                            send();
                            break;
                        case "getControlPoints":
                            Console.WriteLine(choice);
    
                            sendPoints();
                            break;
                        case "getLocations":
                            Console.WriteLine(choice);
                            sendLocations();
                            break;
                        case "sendControlPoints":
                            Console.WriteLine(choice);
                            getPoints();
                            break;
                        case "sendNewPointName":
                            Console.WriteLine(choice);
                            setNewPointName();
                            sendPoints();
                            break;
                        case "sendNewPointTime":
                            Console.WriteLine(choice);
                            setNewPointTime();
                            sendPoints();
                            break;
                        case "deletePoint":
                            Console.WriteLine(choice);
                           delPoint();
                           sendPoints();
                            break;
                        case "sendPriority":
                            Console.WriteLine(choice);
                            recievePriority();
                            break;
                        case "checkMeInVotes":
                            Console.WriteLine(choice);
                            checkMe();
                            break;
                        case "getControlPointsVotes":
                            Console.WriteLine(choice);
                            sendPointsVote();
                            break;
                        case "getVotes":
                            Console.WriteLine(choice);
                            sendVotes();
                            break;
                        case "findTheBest":
                            Console.WriteLine(choice);
                            findTheBest();
                            sendMyAdminData();
                            break;
                        case "setNewTimePoints":
                            Console.WriteLine(choice);
                            setNewTime();
                            break;
                        case "goBack":
                            Console.WriteLine(choice);
                            return true;
                        case "loadMyUserData":
                            Console.WriteLine(choice);
                            sendMyUserData();
                            break;
                        case "sendNewUserName":
                            Console.WriteLine(choice);
                            getNewName();
                            socket.Send(BitConverter.GetBytes(0));
                            sendMyUserData();
                            break;
                        case "sendNewUserLogin":
                            Console.WriteLine(choice);
                            getNewLogin();
                            socket.Send(BitConverter.GetBytes(0));
                            sendMyUserData();
                            break;
                        case "sendNewUserBirthday":
                            Console.WriteLine(choice);
                            getNewBirthday();
                            socket.Send(BitConverter.GetBytes(0));
                            sendMyUserData();
                            break;
                    }


                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                return false;
            }
        }
        private bool userCommandMenu() { return false; }
        public void Process()
        {

            
            Console.WriteLine("Клиент подключился");
            try
            {
                while (true)
                {
                    Console.WriteLine("Ожидание команды");
                    byte[] bytes = new byte[128];
                    int size = socket.Receive(bytes);
                    string choice = Encoding.Unicode.GetString(bytes, 0, size);
                    Console.Write("Полученная команда : ");
                    switch (choice)
                    {
                        case "reg" :
                            Console.WriteLine(choice);
                            add();
                            break;
                        case "auth":
                            Console.WriteLine(choice);
                            
                            if (auth())
                            {
                                if (!adminCommandMenu())
                                {
                                    socket.Close();
                                    return;
                                }
                            }
                           
                            break;
                        case "exit":
                            Console.WriteLine(choice);
                            Console.WriteLine("Клиент отключился");
                            socket.Close();
                            return;
                        case "":
                            Console.WriteLine("exit");
                            Console.WriteLine("Клиент отключился");
                            socket.Close();
                            return;
                       
                    }


                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
    }



    

}