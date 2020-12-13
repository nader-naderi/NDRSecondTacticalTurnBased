using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;
using System.Collections.Generic;

namespace NDR2ndTTB
{
    public class Serialization
    {

        public static void SaveLevel(string saveName, GridBase gridBase)
        {
            if (string.IsNullOrEmpty(saveName))
                saveName = "level1";

            SaveLevelFile saveFile = new SaveLevelFile();
            
            saveFile.sizeX = gridBase.sizeX;
            saveFile.sizeY = gridBase.sizeY;
            saveFile.sizeZ = gridBase.sizeZ;

            saveFile.scaleXZ = gridBase.scaleXZ;
            saveFile.scaleY = gridBase.scaleY;
            saveFile.scaleXZ = gridBase.scaleXZ;
            saveFile.savedNodes = NodeToSaveable(gridBase);

            string saveLocation = SaveLocation();
            saveLocation += saveName;

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(saveLocation, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, saveFile);
            stream.Close();
        }

        public static SaveLevelFile LoadLevel(string loadName)
        {
            SaveLevelFile saveFile = null;

            string targetName = SaveLocation();
            targetName += loadName;

            if(!File.Exists(targetName))
            {
                Debug.Log("Can't find level " + targetName);
            }
            else
            {
                IFormatter formatter = new BinaryFormatter();
                FileStream fileStream = new FileStream(targetName, FileMode.Open);
                SaveLevelFile save = (SaveLevelFile)formatter.Deserialize(fileStream);

                saveFile = save;
                fileStream.Close();

            }

            return saveFile;
        }
        
        public static List<SaveableNode> NodeToSaveable(GridBase grid)
        {
            List<SaveableNode> returnValue = new List<SaveableNode>();

            for (int x = 0; x < grid.sizeX; x++)
            {
                for (int y = 0; y < grid.sizeY; y++)
                {
                    for (int z = 0; z < grid.sizeZ; z++)
                    {
                        SaveableNode sn = new SaveableNode();
                        Node n = grid.grid[x, y, z];
                        sn.x = n.X;
                        sn.y = n.Y;
                        sn.z = n.Z;
                        sn.isWalkable = n.IsWalkable;
                        returnValue.Add(sn);
                    }
                }
            }

            return returnValue;
        }

        static string SaveLocation()
        {
            string saveLoc = Application.streamingAssetsPath + "/Levels/";
            if (!Directory.Exists(saveLoc))
            {
                Directory.CreateDirectory(saveLoc);
            }

            return saveLoc;
        }

    }

    [Serializable]
    public class SaveLevelFile
    {
        public int sizeX;
        public int sizeY;
        public int sizeZ;

        public float scaleXZ;
        public float scaleY;

        public List<SaveableNode> savedNodes;

    }

    [Serializable]
    public class SaveableNode
    {
        public int x, y, z;
        public bool isWalkable;
    }

}