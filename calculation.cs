using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using COMMAND;
using Server;

namespace курсач3сервер
{
    public class VoteData
    {
        public readonly List<int> Positions;
        public int Point;

        public VoteData()
        {
            Positions = new List<int>();
        }
    }

    public class TotalData
    {
        public readonly int Position;
        public readonly CustomsControlPoint Point;
        public int Number;
        public double Percent;
        public TimeSpan Start;
        public TimeSpan End;
        public TotalData(int point, int position)
        {
            var context = new DBContext();
            Point = new CustomsControlPoint();
            Point = context.CustomsControlPoints.Find(point);
            Position = position;
           


        }


    }


    public class VoteCalculation
    {
        private static readonly double[] PointsLoad = {26, 26, 27, 28, 31, 36, 38, 45, 51, 60, 73, 81, 89, 99, 100, 96, 86, 73, 60, 48, 40, 30, 27, 26};
        private readonly List<VoteData> _list;
        private List<TotalData> _data;
        private int[,] _counter;
        private int[] _mas;
        public VoteCalculation()
        {
            _list = new List<VoteData>();
            var connection = new SqlConnection(Program.ConnString());
            using (new DBContext())
            {
                connection.Open();
                var command = new SqlCommand(VoteCommand.getCOMMAND(), connection);

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (_list.Count != 0)
                    {
                        if (_list.Last().Point == Convert.ToInt32(reader[0]))
                        {
                            _list.Last().Positions.Add(Convert.ToInt32(reader[1]));
                        }
                        else
                        {
                            var vote = new VoteData {Point = Convert.ToInt32(reader[0])};
                            vote.Positions.Add(Convert.ToInt32(reader[1]));
                            _list.Add(vote);
                        }
                    }
                    else
                    {
                        var vote = new VoteData {Point = Convert.ToInt32(reader[0])};
                        vote.Positions.Add(Convert.ToInt32(reader[1]));
                        _list.Add(vote);
                    }
                }
            }
        }



        public void Matrix()
        {
            _counter = new int[_list.Count, _list.Count];

            for (var i = 0; i < _list.Count; i++)
            {

                for (var j = 0; j < _list.Count; j++)
                {
                    var p2 = _list[j];
                    var p1 = _list[i];

                    _counter[i, j] = 0;
                    if (p1.Point == p2.Point)
                    {
                        _counter[i, j] = -1;
                    }
                    else
                    {
                        for (var k = 0; k < _list[k].Positions.Count; k++)
                        {
                            if (p1.Positions[k] > p2.Positions[k]) _counter[i, j]++;
                        }
                    }
                }


            }

        }


        public void MatrixToList()
        {
            _mas = new int[_list.Count];
            for (var i = 0; i < _list.Count; i++)
            {
                _mas[i] = 0;
                for (var  j = 0; j < _list.Count; j++)
                {
                    if (i == j) continue;
                    if (_counter[j, i] > _counter[i, j]) _mas[i]++;
                }
                
            }

        }


        public void TotalList()
        {
            _data = new List<TotalData>();
            for (var i = 0; i < _list.Count; i++)
            {
                var newData = new TotalData(_list[i].Point, _mas[i]);
                _data.Add(newData);
            }
            _data.Sort((first, next) => next.Position.CompareTo(first.Position));
  
            _data[0].Number = 1;
            _data[0].Percent = 0;
            for (int i = 1, j = 1; i < _list.Count; i++, j++)
            {
                _data[i].Number = _data[i - 1].Position == _data[i].Position ? _data[i - 1].Number : j;
            }
            var percent = 100.0 / (_data.Last().Number);
            for (var i = 1; i < _list.Count; i++)
            {
                _data[i].Percent = percent * (_data[i].Number == 0 ? _data[i].Number : _data[i].Number - 1);
            }



        }

        public void ItsNewTime()
        {
         
            for (var i = 0; i < _data.Count; i++)
            {
                for (var j = 0; j < PointsLoad.Length; j++)
                {
                
                    if (j <= 14 && _data[i].Percent > PointsLoad[j] && _data[i].Percent < PointsLoad[j + 1])
                    {
                        _data[i].Start = _data[i].Percent - PointsLoad[j] <= PointsLoad[j+ 1] - _data[i].Percent ? new TimeSpan(j + 4, 30, 0) : new TimeSpan(j + 4, 0, 0);
                    }
                    if(j >= 14 && _data[i].Start == null)
                    {
                        _data[i].Start = _data[i].End = new TimeSpan(0, 0, 0);
                        break;
                    }

                    if (j < 14 || !(_data[i].Percent > PointsLoad[j]) ||
                        !(_data[i].Percent < PointsLoad[j - 1])) continue;
                    if (i <= 19)
                    {
                        _data[i].End = PointsLoad[j -1] -_data[i].Percent >= _data[i].Percent - PointsLoad[j] ? new TimeSpan(j + 4, 30, 0) : new TimeSpan(j + 4, 0, 0);
                    }
                    else
                    {
                        _data[i].End = PointsLoad[j - 1] - _data[i].Percent >= _data[i].Percent - PointsLoad[j] ? new TimeSpan(j - 20, 30, 0) : new TimeSpan(j - 20, 0, 0);
                    }
                    break;
                }
            } 
        }

        public void NewTimePointToDb()
        {

            if (_data.Count != 0)
            {
                CustomControlPointNewTime.Remove();
            }

            foreach (var newTime in _data.Select(it => new CustomControlPointNewTime(it)))
            {
                newTime.Insert();
            }
        }
    }



    public class CustomControlPointNewTime
    {
        [Key]
        public int CustomControlPointNewTimeID { get; set; }
      
        [Required]
        public CustomsControlPoint point { get; set; }
        [Required]
        public TimeSpan newStart { get; set; }
        [Required]
        public TimeSpan newEnd { get; set; }

        [Required]
        public double percent { get; set; }
        [Required]
        public int position { get; set; }
        public CustomControlPointNewTime()
        {
            point = new CustomsControlPoint();
        }
        public static void Delete()
        {
            var connection = new SqlConnection(Program.ConnString());
            connection.Open();
            var command = new SqlCommand(CustomControlPointNewTimeCommand.deleteCOMMAND(), connection);
            command.ExecuteNonQuery();
        }
        public CustomControlPointNewTime(SqlDataReader reader)
        {
            point = new CustomsControlPoint();
            CustomControlPointNewTimeID = reader.GetInt32(0);
            newStart = reader.GetTimeSpan(1);
            newEnd = reader.GetTimeSpan(2);
            percent = reader.GetDouble(4);
            position = reader.GetInt32(5);
            point.CustomsControlPointID = reader.GetInt32(6);
            point.name = reader.GetString(7);
            point.TimeStart = reader.GetTimeSpan(8);
            point.TimeEnd = reader.GetTimeSpan(9);
            point.location.LocationID = reader.GetInt32(11);
            point.location.Country = reader.GetString(12);
            point.location.Region = reader.GetString(13);
            point.location.District = reader.GetString(14);
        }

        
        public CustomControlPointNewTime(TotalData data)
        {
            point = data.Point;
            newStart = data.Start;
            newEnd = data.End;
            position = data.Position;
            percent = data.Percent;
           
        }

        public static void Remove()
        {
            var connection = new SqlConnection(Program.ConnString());
            connection.Open();
            var command = new SqlCommand(CustomControlPointNewTimeCommand.removeCOMMAND(), connection);
            command.ExecuteNonQuery();
            connection.Close();
        }

        public void Insert()
        {
            var connection = new SqlConnection(Program.ConnString());
            connection.Open();
            var command = new SqlCommand(CustomControlPointNewTimeCommand.insertCOMMAND(this), connection);
            command.ExecuteNonQuery();
            connection.Close();
        }

        public string Add()
        {
            var data = $"{CustomControlPointNewTimeID}\t{newStart.Hours}\t{newStart.Minutes}\t{newEnd.Hours}\t{newEnd.Minutes}\t";
            data += point.Send();
            data += $"\t{percent}\t{position}";
            data += "\n";
            return data;
            
        }
    }
}
