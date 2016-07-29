using Leap;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace LeapGestureRecognition
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
			if (!File.Exists(fileName))
			{
				SQLiteConnection.CreateFile(fileName);

				// Create tables
				var createStatements = new List<string>() 
				{
					"CREATE TABLE BoneColors (bone TEXT(50), color TEXT(10))",
					"CREATE TABLE BoolOptions (name TEXT(50), value INTEGER(1))",
					"CREATE TABLE DynamicGestureClasses (`id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, `name` TEXT, `gesture_json` TEXT, `sample_instance_json` TEXT )",
					"CREATE TABLE DynamicGestureInstances (`id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, `class_id` INTEGER NOT NULL, `json` TEXT NOT NULL)",
					"CREATE TABLE StaticGestureClasses (`id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, `name` TEXT, `gesture_json` TEXT, `sample_instance_json` TEXT)",
					"CREATE TABLE StaticGestureInstances (`id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, `class_id` INTEGER NOT NULL, `json` TEXT NOT NULL)"
				};
				executeBatchNonQuery(createStatements);

				// Insert default values into tables
				foreach (var kvp in Constants.DefaultBoneColors)
				{
					AddBoneColor(kvp.Key, kvp.Value);
				}
				foreach (var kvp in Constants.DefaultBoolOptions)
				{
					AddBoolOption(kvp.Key, kvp.Value);
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

		public void AddBoneColor(string name, Color color)
		{
			string sql = String.Format("INSERT INTO BoneColors (color, bone) VALUES ('{0}', '{1}')", color.ToString(), name);
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

		public void AddBoolOption(string name, bool value)
		{
			string sql = String.Format("INSERT INTO BoolOptions (name, value) VALUES ('{0}', '{1}')", name, value ? 1 : 0);
			executeNonQuery(sql);
		}
		#endregion

		#region StaticGestureInstances
		public ObservableCollection<SGInstanceWrapper> GetStaticGestureInstances(int classId)
		{
			var gestureInstances = new ObservableCollection<SGInstanceWrapper>();
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
							var gesture = JsonConvert.DeserializeObject<SGInstance>(reader.GetString(2));
							gesture.UpdateFeatureVector();
							SGInstanceWrapper instance = new SGInstanceWrapper()
							{
								Id = reader.GetInt32(0),
								ClassId = reader.GetInt32(1),
								Gesture = gesture,
							};
							instance.InstanceName = String.Format("class {0} inst {1}", instance.ClassId, instance.Id);
							gestureInstances.Add(instance);
						}
					}
				}
			}
			return gestureInstances;
		}

		public void SaveNewStaticGestureInstance(int classId, SGInstance instance) // Whenever this is called it will be a new instance.
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

		#region StaticGestureClass
		public int SaveNewStaticGestureClass(string name, SGClass gesture) // Returns id of gesture
		{
			string sql;
			string gestureJson = JsonConvert.SerializeObject(gesture);
			//string sampleInstance = JsonConvert.SerializeObject(gesture.SampleInstance);
			sql = String.Format("INSERT INTO StaticGestureClasses (name, sample_instance_json) VALUES ('{0}', '{1}')", name, gestureJson);
			executeNonQuery(sql);
			return singleIntValueQuery("SELECT last_insert_rowid()"); // Get the (auto-incremented) key
		}

		public void SaveStaticGestureClass(SGClassWrapper gestureWrapper)
		{
			string sql;
			string gestureJson = JsonConvert.SerializeObject(gestureWrapper.Gesture);
			string sampleInstanceJson = JsonConvert.SerializeObject(gestureWrapper.SampleInstance);
			sql = String.Format("UPDATE StaticGestureClasses SET name='{0}', gesture_json='{1}', sample_instance_json='{2}' WHERE id='{3}'", gestureWrapper.Name, gestureJson, sampleInstanceJson, gestureWrapper.Id);
			executeNonQuery(sql);
		}

		public string GetStaticGestureClassName(int id)
		{
			string sql = String.Format("SELECT name FROM StaticGestureClasses WHERE id='{0}'", id);
			return singleStringValueQuery(sql);
		}

		public void DeleteStaticGestureClass(int id)
		{
			string sql = String.Format("DELETE FROM StaticGestureClasses WHERE id='{0}'", id);
			executeNonQuery(sql);
			// Also delete all instances in StaticGestureInstances table
			sql = String.Format("DELETE FROM StaticGestureInstances WHERE class_id='{0}'", id);
			executeNonQuery(sql);
		}

		public SGClassWrapper GetStaticGestureClass(int id)
		{
			string sql = "SELECT id, name, gesture_json, sample_instance_json FROM StaticGestureClasses WHERE id=" + id;
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					using (SQLiteDataReader reader = command.ExecuteReader())
					{
						reader.Read();
						return new SGClassWrapper()
						{
							Id = reader.GetInt32(0),
							Name = reader.GetString(1),
							Gesture = JsonConvert.DeserializeObject<SGClass>(reader.GetString(2)),
							SampleInstance = JsonConvert.DeserializeObject<SGInstance>(reader.GetString(3)) // Don't care about this
						};
					}
				}
			}
		}

		public ObservableCollection<SGClassWrapper> GetAllStaticGestureClasses()
		{
			var gestures = new ObservableCollection<SGClassWrapper>();
			string sql = "SELECT id, name, gesture_json, sample_instance_json FROM StaticGestureClasses";
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					using (SQLiteDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							//gestures.Add(new StaticGestureClassWrapper() 
							//{
							//	Id = reader.GetInt32(0),
							//	Name = reader.GetString(1),
							//	Gesture = JsonConvert.DeserializeObject<StaticGestureClass>(reader.GetString(2)),
							//	SampleInstance = JsonConvert.DeserializeObject<StaticGestureInstance>(reader.GetString(3))
							//});

							var currGest = new SGClassWrapper()
							{
								Id = reader.GetInt32(0),
								Name = reader.GetString(1),
								Gesture = JsonConvert.DeserializeObject<SGClass>(reader.GetString(2)),
								SampleInstance = JsonConvert.DeserializeObject<SGInstance>(reader.GetString(3))
							};
							gestures.Add(currGest);
						}
					}
				}
			}
			return gestures;
		}
		#endregion


		#region DynamicGestureInstances
		public ObservableCollection<DGInstanceWrapper> GetDynamicGestureInstances(int classId)
		{
			var gestureInstances = new ObservableCollection<DGInstanceWrapper>();
			string sql = String.Format("SELECT id, class_id, json FROM DynamicGestureInstances WHERE class_id='{0}'", classId);
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					using (SQLiteDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							DGInstanceWrapper instance = new DGInstanceWrapper()
							{
								Id = reader.GetInt32(0),
								ClassId = reader.GetInt32(1),
								Instance = JsonConvert.DeserializeObject<DGInstance>(reader.GetString(2)),
							};
							instance.InstanceName = String.Format("class {0} inst {1}", instance.ClassId, instance.Id);
							gestureInstances.Add(instance);
						}
					}
				}
			}
			return gestureInstances;
		}

		public void SaveNewDynamicGestureInstance(int classId, DGInstance instance) // Whenever this is called it will be a new instance.
		{
			string serializedInstance = JsonConvert.SerializeObject(instance);
			string sql = String.Format("INSERT INTO DynamicGestureInstances (class_id, json) VALUES ('{0}', '{1}')", classId, serializedInstance);
			executeNonQuery(sql);
		}

		public void DeleteDynamicGestureInstance(int id)
		{
			string sql = String.Format("DELETE FROM DynamicGestureInstances WHERE id='{0}'", id);
			executeNonQuery(sql);
		}

		// Replaces all existing DynamicGestureInstances with these new ones
		#endregion

		#region DynamicGestureClass
		public int SaveNewDynamicGestureClass(string name, DGClass gesture) // Returns id of gesture
		{
			string sql;
			string gestureJson = JsonConvert.SerializeObject(gesture);
			//string sampleInstance = JsonConvert.SerializeObject(gesture.SampleInstance);
			sql = String.Format("INSERT INTO DynamicGestureClasses (name, sample_instance_json) VALUES ('{0}', '{1}')", name, gestureJson);
			executeNonQuery(sql);
			return singleIntValueQuery("SELECT last_insert_rowid()"); // Get the (auto-incremented) key
		}

		public void SaveDynamicGestureClass(DGClassWrapper gestureWrapper)
		{
			string sql;
			string gestureJson = JsonConvert.SerializeObject(gestureWrapper.Gesture);
			string sampleInstanceJson = JsonConvert.SerializeObject(gestureWrapper.SampleInstance);
			sql = String.Format("UPDATE DynamicGestureClasses SET name='{0}', gesture_json='{1}', sample_instance_json='{2}' WHERE id='{3}'", gestureWrapper.Name, gestureJson, sampleInstanceJson, gestureWrapper.Id);
			executeNonQuery(sql);
		}

		public string GetDynamicGestureClassName(int id)
		{
			string sql = String.Format("SELECT name FROM DynamicGestureClasses WHERE id='{0}'", id);
			return singleStringValueQuery(sql);
		}

		public void DeleteDynamicGestureClass(int id)
		{
			string sql = String.Format("DELETE FROM DynamicGestureClasses WHERE id='{0}'", id);
			executeNonQuery(sql);
			// Also delete all instances in DynamicGestureInstances table
			sql = String.Format("DELETE FROM DynamicGestureInstances WHERE class_id='{0}'", id);
			executeNonQuery(sql);
		}

		public ObservableCollection<DGClassWrapper> GetAllDynamicGestureClasses()
		{
			var gestures = new ObservableCollection<DGClassWrapper>();
			string sql = "SELECT id, name, gesture_json, sample_instance_json FROM DynamicGestureClasses";
			using (var connection = new SQLiteConnection(_connString))
			{
				connection.Open();
				using (SQLiteCommand command = new SQLiteCommand(sql, connection))
				{
					using (SQLiteDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							gestures.Add(new DGClassWrapper()
							{
								Id = reader.GetInt32(0),
								Name = reader.GetString(1),
								Gesture = JsonConvert.DeserializeObject<DGClass>(reader.GetString(2)),
								SampleInstance = JsonConvert.DeserializeObject<DGInstance>(reader.GetString(3))
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
