using SopraProjekt.Entities;
using SopraProjekt.GameState;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SopraProjekt.Helper
{
    /// <summary>
    /// A storage handling class
    /// </summary>
    internal static class StorageHandler
    {
        /// <summary>
        /// Saves the map of the game
        /// </summary>
        /// <param name="map"></param>
        /// <param name="path"></param>
        public static void SaveMap(Map map, string path)
        {
            //map.PrepareMapStorage();
            //var textureMap = map.GetVisibleTextureMap(new Rectangle(0,0,map.MapSize.X,map.MapSize.Y));

            /*foreach (var entity in map.mEntities.GetElements(new Rectangle(new Point(0, 0), new Point(Map.MapSizeMaxX, Map.MapSizeMaxY))))
            {
                Debug.Assert(entity != null, "Could not initialize entity" + entity);
                entity.PrepareEntityStorage();
            }*/
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            FileStream fs = null;
            try
            {
                fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                var bf = new BinaryFormatter();
                bf.Serialize(fs, map);
                //Debug.WriteLine("Serialized");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                // TODO: Exception here
            }
            finally
            {
                fs?.Close();
            }
        }

        /// <summary>
        /// Loads the map to the game
        /// </summary>
        /// <returns></returns>
        public static Map LoadMap(string path)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(path, FileMode.Open);
                var bf = new BinaryFormatter();
                var map = (Map)bf.Deserialize(fs);
                //map.CheckOutMapStorage();
                map.LoadFromFile();
                /*foreach (var entity in map.mEntities.GetElements(new Rectangle(new Point(0, 0), new Point(Map.MapSizeMaxX, Map.MapSizeMaxY))))
                {
                    Debug.Assert(entity != null);
                    entity.CheckOutEntityStorage();
                }*/
                //Debug.WriteLine("Deseralized");
                return map;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                // TODO: Exception Here
                return null;
            }
            finally
            {
                fs?.Close();
            }
        }

        /// <summary>
        /// Saves the Statistics to a file
        /// </summary>
        public static void SaveStatistics()
        {
            string path = "StatisticsStorage.dat";
            SerializableStatisticState state = new SerializableStatisticState();
            state.PrepareSerialization();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            FileStream fs = null;
            try
            {
                fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                var bf = new BinaryFormatter();
                bf.Serialize(fs, state);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                // TODO: Exception here
            }
            finally
            {
                fs?.Close();
            }
        }

        /// <summary>
        /// Loads the Statistics from a file
        /// </summary>
        public static void LoadStatistics()
        {
            string path = "StatisticsStorage.dat";
            FileStream fs = null;
            if (File.Exists(path))
            {
                try
                {
                    fs = new FileStream(path, FileMode.Open);
                    var bf = new BinaryFormatter();
                    var state = (SerializableStatisticState)bf.Deserialize(fs);
                    state.CheckOutSerialization();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                finally
                {
                    fs?.Close();
                }
            }
        }

        /// <summary>
        /// Saves the Achievements to a file
        /// </summary>
        public static void SaveAchievements()
        {
            string path = "AchievementStorage.dat";
            SerializableAchievementState state = new SerializableAchievementState();
            state.PrepareSerialization();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            FileStream fs = null;
            try
            {
                fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                var bf = new BinaryFormatter();
                bf.Serialize(fs, state);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                // TODO: Exception here
            }
            finally
            {
                fs?.Close();
            }
        }

        /// <summary>
        /// Loads the Achievements from a file
        /// </summary>
        public static void LoadAchievements()
        {
            string path = "AchievementStorage.dat";
            FileStream fs = null;
            if (File.Exists(path))
            {
                try
                {
                    fs = new FileStream(path, FileMode.Open);
                    var bf = new BinaryFormatter();
                    var state = (SerializableAchievementState)bf.Deserialize(fs);
                    state.CheckOutSerialization();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                finally
                {
                    fs?.Close();
                }
            }
        }
    }
}