namespace SroMapEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal class NVM
    {
        private NVMEntity[] entities;
        public int entryNum;
        public string filepath;
        private byte[] otherData;

        public NVM(string path)
        {
            this.filepath = path;
            BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)) {
                BaseStream = { Position = 12L }
            };
            this.entryNum = reader.ReadInt16();
            this.entities = new NVMEntity[this.entryNum];
            for (int i = 0; i < this.entryNum; i++)
            {
                NVMEntity entity = new NVMEntity {
                    id = reader.ReadInt32(),
                    X = reader.ReadSingle(),
                    Y = reader.ReadSingle(),
                    Z = reader.ReadSingle(),
                    UK1 = reader.ReadInt16(),
                    UK2 = reader.ReadSingle(),
                    UK3 = reader.ReadInt16(),
                    UK4 = reader.ReadInt16(),
                    UK5 = reader.ReadInt16(),
                    Grid = reader.ReadInt16()
                };
                int num2 = reader.ReadInt16();
                entity.xtra = reader.ReadBytes(num2 * 6);
                this.entities[i] = entity;
            }
            this.otherData = reader.ReadBytes(((int) reader.BaseStream.Length) - ((int) reader.BaseStream.Position));
            reader.Close();
        }

        public void saveNVM(string path, int OX, int OY)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create));
            writer.Write("JMXVNVM 1000".ToCharArray());
            writer.Write((short) this.entities.Length);
            for (int i = 0; i < this.entities.Length; i++)
            {
                writer.Write(this.entities[i].id);
                float x = this.entities[i].X;
                if (this.entities[i].X > 1920f)
                {
                    x -= 1920f;
                }
                else if (this.entities[i].X < 0f)
                {
                    x += 1920f;
                }
                writer.Write(x);
                writer.Write(this.entities[i].Y);
                float z = this.entities[i].Z;
                if (this.entities[i].Z > 1920f)
                {
                    z -= 1920f;
                }
                else if (this.entities[i].Z < 0f)
                {
                    z += 1920f;
                }
                writer.Write(z);
                writer.Write(this.entities[i].UK1);
                writer.Write(this.entities[i].UK2);
                writer.Write(this.entities[i].UK3);
                writer.Write(this.entities[i].UK4);
                writer.Write(this.entities[i].UK5);
                writer.Write((byte) (OX + Math.Floor((double) (this.entities[i].X / 1920f))));
                writer.Write((byte) (OY + Math.Floor((double) (this.entities[i].Z / 1920f))));
                writer.Write((short) 0);
            }
            writer.Write(this.otherData);
            writer.Close();
        }

        public void setEntities(List<MapObject> objects, string pathPref, string[] bsrPaths)
        {
            List<NVMEntity> list = new List<NVMEntity>();
            for (int i = 0; i < objects.Count; i++)
            {
                if (bsrPaths[objects[i].nameI].Contains(".bsr"))
                {
                    BinaryReader reader = new BinaryReader(File.Open(pathPref + bsrPaths[objects[i].nameI], FileMode.Open)) {
                        BaseStream = { Position = 0x44L }
                    };
                    int num2 = reader.ReadInt32();
                    Stream baseStream = reader.BaseStream;
                    baseStream.Position += 0x30 + num2;
                    int num3 = reader.ReadInt32();
                    reader.Close();
                    if (num3 != 0)
                    {
                        NVMEntity item = new NVMEntity {
                            id = objects[i].nameI,
                            X = objects[i].X,
                            Y = objects[i].Y,
                            Z = objects[i].Z
                        };
                        if (objects[i].Unknown1 == "FFFF")
                        {
                            item.UK1 = -1;
                        }
                        else
                        {
                            item.UK1 = 0;
                        }
                        item.UK2 = objects[i].Theta;
                        item.UK3 = (short) objects[i].ID;
                        item.UK4 = 0;
                        if (objects[i].Unknown2 != "0100")
                        {
                            item.UK5 = 1;
                        }
                        else
                        {
                            item.UK5 = 0;
                        }
                        list.Add(item);
                    }
                }
            }
            this.entities = list.ToArray();
        }
    }
}

