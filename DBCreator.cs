
using System;
using System.Data.SqlClient;
using System.Linq;

namespace курсач3сервер
{
    internal static class DBCreator
    {


        private static void FillLocation(DBContext context)
        {
            string[] str = { "INSERT INTO Locations", 
            "VALUES('Латвия', 'Витебская', 'Верхнедвинский')",
            "INSERT INTO Locations",
            "VALUES('Латвия', 'Витебская', 'Миорский')",
            "INSERT INTO Locations",
            "VALUES('Латвия', 'Витебская', 'Браславский')",
            "INSERT INTO Locations",
            "VALUES('Литва', 'Витебская', 'Браславский')",
            "INSERT INTO Locations",
            "VALUES('Литва', 'Витебская', 'Поставский')",
            "INSERT INTO Locations",
            "VALUES('Литва', 'Гродненская', 'Островецкий')",
            "INSERT INTO Locations",
            "VALUES('Литва', 'Гродненская', 'Ошмянский')",
            "INSERT INTO Locations",
            "VALUES('Литва', 'Гродненская', 'Ивьевский')",
            "INSERT INTO Locations",
            "VALUES('Литва', 'Гродненская', 'Вороновский')",
            "INSERT INTO Locations",
            "VALUES('Литва', 'Гродненская', 'Щучинский')",
            "INSERT INTO Locations",
            "VALUES('Литва', 'Гродненская', 'Гродненский')",
            "INSERT INTO Locations",
            "VALUES('Польша', 'Гродненская', 'Гродненский')",
            "INSERT INTO Locations",
            "VALUES('Польша', 'Гродненская', 'Берестовицкий')",
            "INSERT INTO Locations",
            "VALUES('Польша', 'Гродненская', 'Свислочский')",
            "INSERT INTO Locations",
            "VALUES('Польша', 'Брестская', 'Пружанский')",
            "INSERT INTO Locations",
            "VALUES('Польша', 'Брестская', 'Пружанский')",
            "INSERT INTO Locations",
            "VALUES('Польша', 'Брестская', 'Каменицкий')",
            "INSERT INTO Locations",
            "VALUES('Польша', 'Брестская', 'Брестский')",
            "INSERT INTO Locations",
            "VALUES('Украина', 'Брестская', 'Брестский')",
            "INSERT INTO Locations",
            "VALUES('Украина', 'Брестская', 'Малоритский')",
            "INSERT INTO Locations",
            "VALUES('Украина', 'Брестская', 'Кобринский')",
            "INSERT INTO Locations",
            "VALUES('Украина', 'Брестская', 'Дрогичинский')",
            "INSERT INTO Locations",
            "VALUES('Украина', 'Брестская', 'Ивановский')",
            "INSERT INTO Locations",
            "VALUES('Украина', 'Брестская', 'Пинский')",
            "INSERT INTO Locations",
            "VALUES('Украина', 'Брестская', 'Столинский')",
            "INSERT INTO Locations",
            "VALUES('Украина', 'Гомельская', 'Лельчицкий')",
            "INSERT INTO Locations",
            "VALUES('Украина', 'Гомельская', 'Ельский')",
            "INSERT INTO Locations",
            "VALUES('Украина', 'Гомельская', 'Наровлянский')",
            "INSERT INTO Locations",
            "VALUES('Украина', 'Гомельская', 'Брагинский')",
            "INSERT INTO Locations",
            "VALUES('Украина', 'Гомельская', 'Лоевский')",
            "INSERT INTO Locations",
            "VALUES('Украина', 'Гомельская', 'Гомельский')",
            "INSERT INTO Locations",
            "VALUES('Украина', 'Гомельская', 'Добрушский')"};

            string CommandString = string.Join("\n", str);
            SqlConnection connection = new SqlConnection(Program.ConnString());
            connection.Open();
            SqlCommand command = new SqlCommand(CommandString, connection);
            command.ExecuteNonQuery();

        }

        public static void check()
        {
            using (DBContext context = new DBContext())
            {
                if (!context.Database.Exists())
                {
                    Console.WriteLine("База данных отсутствует");
                    context.Database.Create();
                    Console.WriteLine("База данных создана");
                }
                
                if (context.Locations.Count() == 0)
                {
                    FillLocation(context);
                }

     
            }



        }
    }
}