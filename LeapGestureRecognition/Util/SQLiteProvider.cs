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

		#region StaticGestureInstances
		public ObservableCollection<StaticGestureInstanceWrapper> GetStaticGestureInstances(int classId)
		{
			var gestureInstances = new ObservableCollection<StaticGestureInstanceWrapper>();
			string sql = String.Format("SELECT id, class_id, json FROM StaticGestureInstances WHERE class_id='{0}'", classId);
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					using (SQLiteDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							StaticGestureInstanceWrapper instance = new StaticGestureInstanceWrapper()
							{
								Id = reader.GetInt32(0),
								ClassId = reader.GetInt32(1),
								Gesture = JsonConvert.DeserializeObject<LGR_StaticGesture>(reader.GetString(2)),
							};
							instance.InstanceName = String.Format("class {0} inst {1}", instance.ClassId, instance.Id);
							gestureInstances.Add(instance);
						}
					}
				}
			}
			return gestureInstances;
		}

		public void SaveNewStaticGestureInstance(int classId, LGR_StaticGesture instance) // Whenever this is called it will be a new instance.
		{
			string serializedInstance = JsonConvert.SerializeObject(instance);
			string sql = String.Format("INSERT INTO StaticGestureInstances (class_id, json) VALUES ('{0}', '{1}')", classId, serializedInstance);
			executeNonQuery(sql);
		}

		public void DeleteStaticGestureInstance(int id)
		{
			string sql = String.Format("DELETE FROM StaticGestureInstances WHERE id='{0}'", id);
			executeNonQuery(sql);
		}

		// Replaces all existing StaticGestureInstances with these new ones
		#endregion

		#region BayesStaticGestures
		public int SaveNewBayesStaticGesture(string name, BayesStaticGesture gesture) // Returns id of gesture
		{
			string sql;
			string gestureJson = JsonConvert.SerializeObject(gesture);
			//string sampleInstance = JsonConvert.SerializeObject(gesture.SampleInstance);
			sql = String.Format("INSERT INTO BayesStaticGestures (name, sample_instance_json) VALUES ('{0}', '{1}')", name, gestureJson);
			executeNonQuery(sql);
			return singleIntValueQuery("SELECT last_insert_rowid()"); // Get the (auto-incremented) key
		}

		public void SaveBayesStaticGesture(BayesStaticGestureWrapper gestureWrapper)
		{
			string sql;
			string gestureJson = JsonConvert.SerializeObject(gestureWrapper.Gesture);
			string sampleInstanceJson = JsonConvert.SerializeObject(gestureWrapper.SampleInstance);
			sql = String.Format("UPDATE BayesStaticGestures SET name='{0}', gesture_json='{1}', sample_instance_json='{2}' WHERE id='{3}'", gestureWrapper.Name, gestureJson, sampleInstanceJson, gestureWrapper.Id);
			executeNonQuery(sql);
		}

		public string GetBayesStaticGestureName(int id)
		{
			string sql = String.Format("SELECT name FROM BayesStaticGestures WHERE id='{0}'", id);
			return singleStringValueQuery(sql);
		}

		public void DeleteBayesStaticGesture(int id)
		{
			string sql = String.Format("DELETE FROM BayesStaticGestures WHERE id='{0}'", id);
			executeNonQuery(sql);
			// Also delete all instances in StaticGestureInstances table
			sql = String.Format("DELETE FROM StaticGestureInstances WHERE class_id='{0}'", id);
			executeNonQuery(sql);
		}

		public ObservableCollection<BayesStaticGestureWrapper> GetAllBayesStaticGestures()
		{
			var gestures = new ObservableCollection<BayesStaticGestureWrapper>();
			string sql = "SELECT id, name, gesture_json, sample_instance_json FROM BayesStaticGestures";
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					using (SQLiteDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							gestures.Add(new BayesStaticGestureWrapper() 
							{
								Id = reader.GetInt32(0),
								Name = reader.GetString(1),
								Gesture = JsonConvert.DeserializeObject<BayesStaticGesture>(reader.GetString(2)),
								SampleInstance = JsonConvert.DeserializeObject<LGR_StaticGesture>(reader.GetString(3))
							});
						}
					}
				}
			}
			return gestures;
		}
		#endregion

		#endregion

	}
}
