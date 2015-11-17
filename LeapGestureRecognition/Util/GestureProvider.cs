using Leap;
using LeapGestureRecognition.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace LeapGestureRecognition.Util
{
	public class GestureProvider
	{
		private SQLiteConnection _conn;
		private string _connString;

		public GestureProvider(string fileName)
		{
			_connString = String.Format("Data Source={0};Version=3;Pooling=True;Max Pool Size=100;", fileName);
			initDB(fileName);
		}

		#region Public Properties
		public ObservableCollection<SingleHandGestureStatic> StaticSingleHandGestures { get; set; }
		#endregion

		#region Private Methods
		private void initDB(string fileName)
		{
			if (File.Exists(fileName)) return;
			SQLiteConnection.CreateFile(fileName);
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				string sql = "CREATE TABLE Gestures (name TEXT(50), json TEXT)";
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					command.ExecuteNonQuery();
				}
			}
		}
		#endregion

		#region Public Methods
		public void SaveGesture(SingleHandGestureStatic gesture) // Might want to create separate methods for create and update.
		{
			string json = JsonConvert.SerializeObject(gesture);
			string sql;
			if (!GestureExists(gesture.Name))
			{
				sql = String.Format("INSERT INTO Gestures (name, json) VALUES ('{0}', '{1}')", gesture.Name, json);
			}
			else
			{
				sql = String.Format("UPDATE Gestures SET json='{0}' WHERE name='{1}'", json, gesture.Name);
			}
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					command.ExecuteNonQuery();
				}
			}
		}

		public SingleHandGestureStatic LoadGesture(string name)
		{
			string json;
			string sql = String.Format("SELECT json FROM Gestures WHERE name='{0}' LIMIT 1", name);
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					using (SQLiteDataReader reader = command.ExecuteReader())
					{
						if (!reader.Read()) throw new Exception(String.Format("Gesture '{0}' does not exist.", name));
						json = reader.GetString(0);
					}
				}
			}
			return JsonConvert.DeserializeObject<SingleHandGestureStatic>(json);
		}

		public void DeleteGesture(string name) // Might want to return a bool to indicate success or failure
		{
			string sql = String.Format("DELETE FROM Gestures WHERE name='{0}'", name);
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					command.ExecuteNonQuery();
				}
			}
		}

		public bool GestureExists(string name)
		{
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				string sql = String.Format("SELECT COUNT(1) FROM Gestures WHERE name='{0}'", name);
				SQLiteCommand command = new SQLiteCommand(sql, connection);
				return (int)(long)command.ExecuteScalar() > 0;
			}
		}

		public ObservableCollection<SingleHandGestureStatic> LoadAllGestures()
		{
			ObservableCollection<SingleHandGestureStatic> gestures = new ObservableCollection<SingleHandGestureStatic>();
			string sql = "SELECT json FROM Gestures";
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					using (SQLiteDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							string json = reader.GetString(0);
							gestures.Add(JsonConvert.DeserializeObject<SingleHandGestureStatic>(json));
						}
					}
				}
			}
			return gestures;
		}
		#endregion

	}
}
