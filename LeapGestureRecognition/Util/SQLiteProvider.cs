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

				// Create StringOptions table
				sql = "CREATE TABLE StringOptions (name TEXT, value TEXT, pinky REAL, ring REAL, middle REAL, index REAL, thumb REAL)";
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					command.ExecuteNonQuery();
				}
				// Fill StringOptions table with default values
				foreach (var stringOption in Constants.DefaultStringOptions)
				{
					sql = String.Format("INSERT INTO StringOptions (name, value) VALUES ('{0}', '{1}')", stringOption.Key, stringOption.Value);
					using (SQLiteCommand command = new SQLiteCommand(sql, connection))
					{
						command.ExecuteNonQuery();
					}
				}

				// Create Users table
				sql = "CREATE TABLE Users (name TEXT(50), is_active INTEGER(1), pinky REAL, ring REAL, middle REAL, index REAL, thumb REAL)";
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					command.ExecuteNonQuery();
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

		private void executeBatchNonQuery(List<string> queries)
		{
			foreach (string sql in queries)
			{
				executeNonQuery(sql);
			}
		}

		private string singleStringValueQuery(string sql)
		{
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					using (SQLiteDataReader reader = command.ExecuteReader())
					{
						reader.Read();
						return reader.GetString(0);
					}
				}
			}
		}

		private int singleIntValueQuery(string sql)
		{
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					using (SQLiteDataReader reader = command.ExecuteReader())
					{
						reader.Read();
						return reader.GetInt32(0);
					}
				}
			}
		}
		#endregion
		
		#endregion

		#region Public Methods



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

		#region Users
		public LGR_User GetActiveUser()
		{
			//string sql = String.Format("SELECT value FROM StringOptions WHERE name='{0}'", Constants.StringOptionsNames.ActiveUser);
			//string activeUserName = singleValueQuery(sql);
			string sql = "SELECT name, pinky_length, ring_length, middle_length, index_length, thumb_length, id FROM Users WHERE is_active=1";
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					using (SQLiteDataReader reader = command.ExecuteReader())
					{
						reader.Read();
						return new LGR_User()
						{
							Name = reader.GetString(0),
							IsActive = true,
							PinkyLength = reader.GetFloat(1),
							RingLength = reader.GetFloat(2),
							MiddleLength = reader.GetFloat(3),
							IndexLength = reader.GetFloat(4),
							ThumbLength = reader.GetFloat(5),
							Id = reader.GetInt32(6)
						};
					}
				}
			}
		}

		public void SetActiveUser(int id)
		{
			List<string> queries = new List<string>()
			{
				"UPDATE Users SET is_active=0",
				String.Format("UPDATE Users SET is_active=1 WHERE id='{0}'", id)
			};
			executeBatchNonQuery(queries);
		}

		public void UpdateUser(LGR_User user)
		{
			string sql = String.Format("UPDATE Users SET name='{0}', is_active='{1}', pinky_length='{2}', " + 
				"ring_length='{3}', middle_length='{4}', index_length='{5}', thumb_length='{6}' WHERE id='{7}'",
				user.Name, user.IsActive ? 1 : 0, user.PinkyLength, user.RingLength, user.MiddleLength,
				user.IndexLength, user.ThumbLength, user.Id);

			executeNonQuery(sql);
		}

		public void SaveNewUser(LGR_User user) // Return the id of the user
		{
			string sql = String.Format("INSERT INTO Users (name, is_active, pinky_length, ring_length, middle_length, index_length, thumb_length) " +
				"VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}')", user.Name, user.IsActive ? 1 : 0,
				user.PinkyLength, user.RingLength, user.MiddleLength, user.IndexLength, user.ThumbLength);

			executeNonQuery(sql);
			// Set the id of user:
		}

		public List<LGR_User> GetAllUsers()
		{
			List<LGR_User> users = new List<LGR_User>();
			string sql = "SELECT name, is_active, pinky_length, ring_length, middle_length, index_length, thumb_length, id FROM Users";
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					using (SQLiteDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							users.Add(new LGR_User()
							{
								Name = reader.GetString(0),
								IsActive = reader.GetInt32(1) == 1,
								PinkyLength = reader.GetFloat(2),
								RingLength = reader.GetFloat(3),
								MiddleLength = reader.GetFloat(4),
								IndexLength = reader.GetFloat(5),
								ThumbLength = reader.GetFloat(6),
								Id = reader.GetInt32(7)
							});
						}
					}
				}
			}
			return users;
		}

		public void DeleteUser(int id) // Might want to return a bool to indicate success or failure
		{
			string sql = String.Format("DELETE FROM Users WHERE id='{0}'", id);
			executeNonQuery(sql);
		}
		#endregion

		#region Gestures
		public int SaveGesture(LGR_StaticGesture gesture) // Returns id of gesture
		{
			string sql;
			if (!GestureExists(gesture.Id))
			{
				sql = String.Format("INSERT INTO Gestures (name, json) VALUES ('{0}', '{1}')", gesture.Name, "");
				executeNonQuery(sql); // Need to set (auto-incremented) id before serializing.
				gesture.Id = singleIntValueQuery("SELECT last_insert_rowid()");
			}
			string json = JsonConvert.SerializeObject(gesture);
			sql = String.Format("UPDATE Gestures SET json='{0}', name='{1}' WHERE id='{2}'", json, gesture.Name, gesture.Id);
			executeNonQuery(sql);
			return gesture.Id;
		}

		public void SaveGesture(string name, LGR_StaticGesture gesture)
		{
			gesture.Name = name;
			SaveGesture(gesture);
		}

		public LGR_StaticGesture GetGesture(string name)
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
			return JsonConvert.DeserializeObject<LGR_StaticGesture>(json);
		}

		public LGR_StaticGesture GetGesture(int id)
		{
			string json;
			string sql = String.Format("SELECT json FROM Gestures WHERE id='{0}' LIMIT 1", id);
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					using (SQLiteDataReader reader = command.ExecuteReader())
					{
						if (!reader.Read()) throw new Exception(String.Format("Gesture id '{0}' does not exist.", id));
						json = reader.GetString(0);
					}
				}
			}
			return JsonConvert.DeserializeObject<LGR_StaticGesture>(json);
		}

		public void DeleteGesture(string name)
		{
			string sql = String.Format("DELETE FROM Gestures WHERE name='{0}'", name);
			executeNonQuery(sql);
		}

		public void DeleteGesture(int id)
		{
			List<string> queries = new List<string>()
			{
				String.Format("DELETE FROM Gestures WHERE id='{0}'", id),
				String.Format("DELETE FROM GestureInstances WHERE class_id='{0}'", id)
			};
			executeBatchNonQuery(queries);
		}

		// Does not check to see if newName already exists in DB.
		public void RenameGesture(string oldName, string newName) // Might want to return a bool to indicate success or failure
		{
			// Need to actually load the gesture and rename the object, and then store again with updated JSON. 
			LGR_StaticGesture gesture = GetGesture(oldName);
			gesture.Name = newName;
			string newJson = JsonConvert.SerializeObject(gesture);
			string sql = String.Format("UPDATE Gestures SET name='{0}', json='{1}' WHERE name='{2}'", newName, newJson, oldName);
			executeNonQuery(sql);
		}

		public void RenameGesture(int id, string newName) // Might want to return a bool to indicate success or failure
		{
			// Need to actually load the gesture and rename the object, and then store again with updated JSON. 
			LGR_StaticGesture gesture = GetGesture(id);
			gesture.Name = newName;
			string newJson = JsonConvert.SerializeObject(gesture);
			string sql = String.Format("UPDATE Gestures SET name='{0}', json='{1}' WHERE id='{2}'", newName, newJson, id);
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

		public bool GestureExists(int id)
		{
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				string sql = String.Format("SELECT COUNT(1) FROM Gestures WHERE id='{0}'", id);
				SQLiteCommand command = new SQLiteCommand(sql, connection);
				return (int)(long)command.ExecuteScalar() > 0;
			}
		}

		public ObservableCollection<LGR_StaticGesture> GetAllGestures()
		{
			ObservableCollection<LGR_StaticGesture> gestures = new ObservableCollection<LGR_StaticGesture>();
			string sql = "SELECT id,name,json FROM Gestures";
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					using (SQLiteDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							int id = reader.GetInt32(0);
							string name = reader.GetString(1);
							string json = reader.GetString(2);
							var gest = JsonConvert.DeserializeObject<LGR_StaticGesture>(json) ?? new LGR_StaticGesture();
							gest.Id = id;
							gest.Name = name;
							// This code above can be simplified later. Adding this logic for when manually adding entries in DB with empty json.
							gestures.Add(gest);
						}
					}
				}
			}
			return gestures;
		}

		public string GetGestureName(int id)
		{
			string sql = String.Format("SELECT name FROM Gestures WHERE id='{0}'", id);
			return singleStringValueQuery(sql);
		}
		#endregion

		#region GestureInstances
		public ObservableCollection<LGR_StaticGesture> GetGestureInstances(int classId)
		{
			ObservableCollection<LGR_StaticGesture> gestureInstances = new ObservableCollection<LGR_StaticGesture>();
			string sql = String.Format("SELECT id, class_id, json FROM GestureInstances WHERE class_id='{0}'", classId);
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					using (SQLiteDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							LGR_StaticGesture instance = JsonConvert.DeserializeObject<LGR_StaticGesture>(reader.GetString(2));
							instance.Id = reader.GetInt32(0);
							instance.ClassId = reader.GetInt32(1);
							//instance.InstanceName = (instance.Id == -1 || instance.ClassId == -1) ? "new instance" : String.Format("class {0} inst {1}", instance.ClassId, instance.Id);
							instance.InstanceName = String.Format("class {0} inst {1}", instance.ClassId, instance.Id);
							gestureInstances.Add(instance);
						}
					}
				}
			}
			return gestureInstances;
		}

		public void SaveNewGestureInstance(LGR_StaticGesture gestureInstance) // Whenever this is called it will be a new instance.
		{
			string serializedGestureInstance = JsonConvert.SerializeObject(gestureInstance);
			string sql = String.Format("INSERT INTO GestureInstances (class_id, json) VALUES ('{0}', '{1}')", gestureInstance.ClassId, serializedGestureInstance);
			executeNonQuery(sql);
		}

		public void DeleteGestureInstance(int id)
		{
			string sql = String.Format("DELETE FROM GestureInstances WHERE id='{0}'", id);
			executeNonQuery(sql);
		}
		#endregion

		#endregion

	}
}
