namespace SroMapEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class OFile
    {
        private List<MapObject> AllObjects = new List<MapObject>();
        public List<MapObject> objects = new List<MapObject>();
        public int OX;
        public int OY;

        public OFile(string path)
        {
            BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));
            string[] strArray = path.Split(new char[] { '\\' });
            this.OX = int.Parse(strArray[strArray.Length - 1].Remove(strArray[strArray.Length - 1].IndexOf(".o2")));
            this.OY = int.Parse(strArray[strArray.Length - 2]);
            List<int> list = new List<int>();
            reader.BaseStream.Position = 12L;
            for (int i = 0; i < 0x90; i++)
            {
                short num2 = reader.ReadInt16();
                for (int j = 0; j < num2; j++)
                {
                    MapObject item = new MapObject();
                    item.groups.Add(i);
                    item.nameI = reader.ReadInt32();
                    item.X = reader.ReadSingle();
                    item.Y = reader.ReadSingle();
                    item.Z = reader.ReadSingle();
                    string str = reader.ReadByte().ToString("X2");
                    item.Unknown1 = str + reader.ReadByte().ToString("X2");
                    float num4 = reader.ReadSingle();
                    while (num4 < 0f)
                    {
                        num4 += 6.283185f;
                    }
                    while (num4 > 6.283185f)
                    {
                        num4 -= 6.283185f;
                    }
                    item.Theta = num4;
                    item.ID = reader.ReadInt32();
                    string str2 = reader.ReadByte().ToString("X2");
                    item.Unknown2 = str2 + reader.ReadByte().ToString("X2");
                    int num5 = reader.ReadByte();
                    int num6 = reader.ReadByte();
                    item.X += (num5 - this.OX) * 0x780;
                    item.Z += (num6 - this.OY) * 0x780;
                    this.AllObjects.Add(item);
                    if (!list.Contains(item.ID))
                    {
                        this.objects.Add(item);
                        list.Add(item.ID);
                    }
                    else
                    {
                        this.objects[list.IndexOf(item.ID)].groups.Add(i);
                    }
                }
            }
            this.objects.Reverse();
            reader.Close();
        }
    }
}

