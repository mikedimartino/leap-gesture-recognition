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
using System.Windows.Media;

namespace LeapGestureRecognition.Util
{
	public class SQLiteProvider
	{
		private string _connString;

		public SQLiteProvider(string fileName)
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
				// Create Gestures table
				string sql = "CREATE TABLE Gestures (name TEXT(50), json TEXT)";
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					command.ExecuteNonQuery();
				}

				// Create BoneColors table
				sql = "CREATE TABLE BoneColors (bone TEXT(50), color TEXT(10))";
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					command.ExecuteNonQuery();
				}
				// Fill BoneColors table with default values
				foreach (var boneColor in Constants.DefaultBoneColors)
				{
					sql = String.Format("INSERT INTO BoneColors (bone, color) VALUES ('{0}', '{1}')", boneColor.Key, boneColor.Value.ToString());
					using (SQLiteCommand command = new SQLiteCommand(sql, connection))
					{
						command.ExecuteNonQuery();
					}
				}

				// Create BoolOptions table
				sql = "CREATE TABLE BoolOptions (name TEXT(50), value INTEGER(1))";
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					command.ExecuteNonQuery();
				}
				// Fill BoolOptions table with default values
				foreach (var boolOption in Constants.DefaultBoolOptions)
				{
					sql = String.Format("INSERT INTO BoolOptions (name, value) VALUES ('{0}', '{1}')", boolOption.Key, boolOption.Value ? 1 : 0);
					using (SQLiteCommand command = new SQLiteCommand(sql, connection))
					{
						command.ExecuteNonQuery();
					}
				}
			}
		}


		#region SQLite Wrapper Methods
		private int executeNonQuery(string sql)
		{
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					return command.ExecuteNonQuery();
				}
			}
		}
		#endregion
		
		#endregion

		#region Public Methods

		#region Gestures
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
			executeNonQuery(sql);
		}

		public void SaveGesture(string name, SingleHandGestureStatic gesture)
		{
			gesture.Name = name;
			SaveGesture(gesture);
		}

		public SingleHandGestureStatic GetGesture(string name)
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
			executeNonQuery(sql);
		}

		// Does not check to see if newName already exists in DB.
		public void RenameGesture(string oldName, string newName) // Might want to return a bool to indicate success or failure
		{
			// Need to actually load the gesture and rename the object, and then store again with updated JSON. 
			SingleHandGestureStatic gesture = GetGesture(oldName);
			gesture.Name = newName;
			string newJson = JsonConvert.SerializeObject(gesture);
			string sql = String.Format("UPDATE Gestures SET name='{0}', json='{1}' WHERE name='{2}'", newName, newJson, oldName);
			executeNonQuery(sql);
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

		public ObservableCollection<SingleHandGestureStatic> GetAllGestures()
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

		#region Bone Colors
		public Dictionary<string, Color> GetBoneColors()
		{
			Dictionary<string, Color> boneColors = new Dictionary<string, Color>();
			string sql = "SELECT bone, color FROM BoneColors";
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					using (SQLiteDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							string bone = reader.GetString(0);
							Color color = (Color) ColorConverter.ConvertFromString(reader.GetString(1));
							boneColors.Add(bone, color);
						}
					}
				}
			}
			return boneColors;
		}

		public void UpdateBoneColor(string name, Color color)
		{
			string sql = String.Format("UPDATE BoneColors SET color='{0}' WHERE bone='{1}'", color.ToString(), name);
			executeNonQuery(sql);
		}
		#endregion

		#region BoolOptions
		public Dictionary<string, bool> GetBoolOptions()
		{
			Dictionary<string, bool> boolOptions = new Dictionary<string, bool>();
			string sql = "SELECT name, value FROM BoolOptions";
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					using (SQLiteDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							string name = reader.GetString(0);
							bool value = reader.GetInt32(1) == 1;
							boolOptions.Add(name, value);
						}
					}
				}
			}
			return boolOptions;
		}

		public void UpdateBoolOption(string name, bool value)
		{
			string sql = String.Format("UPDATE BoolOptions SET value='{0}' WHERE name='{1}'", value ? 1 : 0, name);
			executeNonQuery(sql);
		}
		#endregion

		#endregion

	}
}
