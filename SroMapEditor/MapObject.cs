namespace SroMapEditor
{
    using OpenTK.Graphics.OpenGL;
    using SroMapEditor.DDSReader;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;

    internal class MapObject
    {
        public float[] boundingBoxp1;
        public float[] boundingBoxp2;
        public bool DistFade;
        private int[][,] Faces;
        public List<int> groups = new List<int>();
        public int ID;
        public string materialPath;
        public int nameI;
        private string pathPrefix;
        private int readingMesh;
        public float[] rotBoundingBoxp1;
        public float[] rotBoundingBoxp2;
        private string[] Tex;
        private int[] TexIDs;
        public float Theta;
        public string Unknown1;
        public string Unknown2;
        private float[][,] UV;
        private float[][,] Verts;
        public float X;
        public float Y;
        public float Z;

        public void calcGroups()
        {
            List<int> list2;
            if (this.boundingBoxp1 == null)
            {
                int num9 = (int) Math.Floor((double) (this.X / 80f));
                while (((num9 - 2) % 4) != 0)
                {
                    num9++;
                    if (num9 < 0x17)
                    {
                        num9 = 0x17;
                        break;
                    }
                }
                int num10 = (int) Math.Floor((double) (this.Z / 320f));
                this.groups.Add(num9 + (0x18 * num10));
                return;
            }
            int item = (int) Math.Floor((double) ((this.rotBoundingBoxp1[0] + this.X) / 80f));
            int num2 = (int) Math.Floor((double) ((this.rotBoundingBoxp1[2] + this.Z) / 320f));
            int num3 = (int) Math.Floor((double) ((this.rotBoundingBoxp2[0] + this.X) / 80f));
            int num4 = (int) Math.Floor((double) ((this.rotBoundingBoxp2[2] + this.Z) / 320f));
            List<int> list = new List<int>();
            if (item < 2)
            {
                item = 2;
            }
            if (num3 < 2)
            {
                num3 = 2;
            }
            if (item > 0x17)
            {
                item = 0x17;
            }
            if (num3 > 0x17)
            {
                num3 = 0x17;
            }
            if (item == num3)
            {
                if (((item - 2) % 4) != 0)
                {
                    while (((item - 2) % 4) != 0)
                    {
                        item++;
                        if (item < 0x17)
                        {
                            item = 0x17;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (((item - 2) % 4) != 0)
                {
                    while (((item - 2) % 4) != 0)
                    {
                        item--;
                        if (item < 0)
                        {
                            item = 2;
                            break;
                        }
                    }
                }
                if (((num3 - 2) % 4) != 0)
                {
                    while (((num3 - 2) % 4) != 0)
                    {
                        num3++;
                        if (num3 > 0x17)
                        {
                            num3 = 0x17;
                            break;
                        }
                    }
                }
                for (int j = 0; j < (((num3 - item) / 4) + 1); j++)
                {
                    list.Add(item + (4 * j));
                }
                goto Label_012B;
            }
            list.Add(item);
        Label_012B:
            list2 = new List<int>();
            if (num2 < 0)
            {
                num2 = 0;
            }
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num2 > 5)
            {
                num2 = 5;
            }
            if (num4 > 5)
            {
                num4 = 5;
            }
            if (num2 != num4)
            {
                for (int k = 0; k < ((num4 - num2) + 1); k++)
                {
                    list2.Add(num2 + k);
                }
            }
            else
            {
                list2.Add(num2);
            }
            for (int i = 0; i < list2.Count; i++)
            {
                for (int m = 0; m < list.Count; m++)
                {
                    this.groups.Add((list2[i] * 0x18) + list[m]);
                }
            }
        }

        public void Draw()
        {
            if (this.Faces != null)
            {
                for (int i = 0; i < this.Faces.Length; i++)
                {
                    GL.Enable(EnableCap.Texture2D);
                    if (this.TexIDs != null)
                    {
                        GL.BindTexture(TextureTarget.Texture2D, this.TexIDs[i]);
                    }
                    GL.Begin(BeginMode.Triangles);
                    if (this.Faces[i] != null)
                    {
                        for (int j = 0; j < (this.Faces[i].Length / 3); j++)
                        {
                            GL.TexCoord2(this.UV[i][this.Faces[i][j, 0], 0], this.UV[i][this.Faces[i][j, 0], 1]);
                            GL.Vertex3(this.Verts[i][this.Faces[i][j, 0], 0], this.Verts[i][this.Faces[i][j, 0], 2], this.Verts[i][this.Faces[i][j, 0], 1]);
                            GL.TexCoord2(this.UV[i][this.Faces[i][j, 1], 0], this.UV[i][this.Faces[i][j, 1], 1]);
                            GL.Vertex3(this.Verts[i][this.Faces[i][j, 1], 0], this.Verts[i][this.Faces[i][j, 1], 2], this.Verts[i][this.Faces[i][j, 1], 1]);
                            GL.TexCoord2(this.UV[i][this.Faces[i][j, 2], 0], this.UV[i][this.Faces[i][j, 2], 1]);
                            GL.Vertex3(this.Verts[i][this.Faces[i][j, 2], 0], this.Verts[i][this.Faces[i][j, 2], 2], this.Verts[i][this.Faces[i][j, 2], 1]);
                        }
                    }
                    GL.End();
                    GL.Disable(EnableCap.Texture2D);
                }
            }
        }

        public void drawBoundingBox()
        {
            if (this.boundingBoxp1 != null)
            {
                GL.Color3((byte) 0xff, (byte) 0, (byte) 0);
                GL.Begin(BeginMode.LineStrip);
                GL.Vertex3(this.boundingBoxp1[0], this.boundingBoxp1[2], this.boundingBoxp1[1]);
                GL.Vertex3(this.boundingBoxp2[0], this.boundingBoxp1[2], this.boundingBoxp1[1]);
                GL.Vertex3(this.boundingBoxp2[0], this.boundingBoxp1[2], this.boundingBoxp2[1]);
                GL.Vertex3(this.boundingBoxp1[0], this.boundingBoxp1[2], this.boundingBoxp2[1]);
                GL.Vertex3(this.boundingBoxp1[0], this.boundingBoxp2[2], this.boundingBoxp2[1]);
                GL.Vertex3(this.boundingBoxp2[0], this.boundingBoxp2[2], this.boundingBoxp2[1]);
                GL.Vertex3(this.boundingBoxp2[0], this.boundingBoxp2[2], this.boundingBoxp1[1]);
                GL.Vertex3(this.boundingBoxp1[0], this.boundingBoxp2[2], this.boundingBoxp1[1]);
                GL.Vertex3(this.boundingBoxp1[0], this.boundingBoxp1[2], this.boundingBoxp1[1]);
                GL.End();
                GL.Begin(BeginMode.Lines);
                GL.Vertex3(this.boundingBoxp1[0], this.boundingBoxp2[2], this.boundingBoxp1[1]);
                GL.Vertex3(this.boundingBoxp1[0], this.boundingBoxp2[2], this.boundingBoxp2[1]);
                GL.Vertex3(this.boundingBoxp1[0], this.boundingBoxp1[2], this.boundingBoxp1[1]);
                GL.Vertex3(this.boundingBoxp1[0], this.boundingBoxp1[2], this.boundingBoxp2[1]);
                GL.Vertex3(this.boundingBoxp2[0], this.boundingBoxp1[2], this.boundingBoxp1[1]);
                GL.Vertex3(this.boundingBoxp2[0], this.boundingBoxp2[2], this.boundingBoxp1[1]);
                GL.Vertex3(this.boundingBoxp2[0], this.boundingBoxp1[2], this.boundingBoxp2[1]);
                GL.Vertex3(this.boundingBoxp2[0], this.boundingBoxp2[2], this.boundingBoxp2[1]);
                GL.End();
                GL.Color3((byte) 0xff, (byte) 0xff, (byte) 0xff);
            }
        }

        public void drawGroups()
        {
            GL.Color4((byte) 0xff, (byte) 0xff, (byte) 0, (byte) 100);
            for (int i = 0; i < this.groups.Count; i++)
            {
                GL.Begin(BeginMode.TriangleStrip);
                GL.Vertex3((double) ((this.groups[i] % 0x18) * 80), Math.Floor((double) (((double) this.groups[i]) / 24.0)) * 320.0, 500.0);
                GL.Vertex3((double) (((this.groups[i] % 0x18) + 1) * 80), Math.Floor((double) (((double) this.groups[i]) / 24.0)) * 320.0, 500.0);
                GL.Vertex3((double) ((this.groups[i] % 0x18) * 80), (Math.Floor((double) (((double) this.groups[i]) / 24.0)) + 1.0) * 320.0, 500.0);
                GL.Vertex3((double) (((this.groups[i] % 0x18) + 1) * 80), (Math.Floor((double) (((double) this.groups[i]) / 24.0)) + 1.0) * 320.0, 500.0);
                GL.End();
            }
            GL.Color4((byte) 0xff, (byte) 0xff, (byte) 0xff, (byte) 0xff);
        }

        public void FindTex(List<string> texNames, List<int> texIDs)
        {
            if (this.Tex != null)
            {
                this.TexIDs = new int[this.Tex.Length];
                for (int i = 0; i < this.Tex.Length; i++)
                {
                    if (texNames.IndexOf(this.Tex[i]) != -1)
                    {
                        this.TexIDs[i] = texIDs[texNames.IndexOf(this.Tex[i])];
                    }
                }
            }
        }

        public void LoadFiles(string pathPref, string path)
        {
            this.readingMesh = 0;
            this.pathPrefix = pathPref;
            BinaryReader reader = new BinaryReader(File.Open(pathPref + path, FileMode.Open)) {
                BaseStream = { Position = 12L }
            };
            int num = reader.ReadInt32();
            int num2 = reader.ReadInt32();
            int num3 = reader.ReadInt32();
            reader.BaseStream.Position = num + 8;
            int count = reader.ReadInt32();
            string str = new string(reader.ReadChars(count));
            reader.BaseStream.Position = num2;
            int length = reader.ReadInt32();
            string[] strArray = new string[length];
            for (int i = 0; i < length; i++)
            {
                int num7 = reader.ReadInt32();
                if (num7 < 20)
                {
                    num7 = reader.ReadInt32();
                }
                strArray[i] = new string(reader.ReadChars(num7));
            }
            reader.BaseStream.Position = num3;
            if (reader.ReadInt32() == 0)
            {
                this.DistFade = false;
            }
            else
            {
                this.DistFade = true;
            }
            reader.Close();
            this.materialPath = pathPref + str;
            reader.Close();
            length = strArray.Length;
            this.Verts = new float[length][,];
            this.UV = new float[length][,];
            this.Faces = new int[length][,];
            this.Tex = new string[length];
            foreach (string str2 in strArray)
            {
                this.readModel(pathPref + str2);
            }
            float num8 = this.Verts[0][0, 0];
            float num9 = this.Verts[0][0, 1];
            float num10 = this.Verts[0][0, 2];
            float num11 = this.Verts[0][0, 0];
            float num12 = this.Verts[0][0, 1];
            float num13 = this.Verts[0][0, 2];
            for (int j = 0; j < this.Verts.Length; j++)
            {
                for (int k = 0; k < (this.Verts[j].Length / 3); k++)
                {
                    if (this.Verts[j][k, 0] < num8)
                    {
                        num8 = this.Verts[j][k, 0];
                    }
                    if (this.Verts[j][k, 1] < num9)
                    {
                        num9 = this.Verts[j][k, 1];
                    }
                    if (this.Verts[j][k, 2] < num10)
                    {
                        num10 = this.Verts[j][k, 2];
                    }
                    if (this.Verts[j][k, 0] > num11)
                    {
                        num11 = this.Verts[j][k, 0];
                    }
                    if (this.Verts[j][k, 1] > num12)
                    {
                        num12 = this.Verts[j][k, 1];
                    }
                    if (this.Verts[j][k, 2] > num13)
                    {
                        num13 = this.Verts[j][k, 2];
                    }
                }
            }
            this.boundingBoxp1 = new float[] { num8, num9, num10 };
            this.boundingBoxp2 = new float[] { num11, num12, num13 };
            this.rotBoundingBoxp1 = new float[] { num8, num9, num10 };
            this.rotBoundingBoxp2 = new float[] { num11, num12, num13 };
            this.setRotation(this.Theta);
        }

        public MeshTexture[] readMaterial()
        {
            BinaryReader reader = new BinaryReader(File.Open(this.materialPath, FileMode.Open)) {
                BaseStream = { Position = 12L }
            };
            int num = reader.ReadInt32();
            MeshTexture[] textureArray = new MeshTexture[num];
            for (int i = 0; i < num; i++)
            {
                int count = reader.ReadInt32();
                string tName = new string(reader.ReadChars(count));
                Stream baseStream = reader.BaseStream;
                baseStream.Position += 0x48L;
                int num4 = reader.ReadInt32();
                string source = new string(reader.ReadChars(num4));
                Stream stream2 = reader.BaseStream;
                stream2.Position += 7L;
                if (source != "")
                {
                    int texture = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, texture);
                    System.Drawing.Bitmap bitmapImage = null;
                    string path = "";
                    if (source.Contains<char>('\\'))
                    {
                        path = this.pathPrefix + source;
                    }
                    else
                    {
                        path = this.materialPath.Substring(0, this.materialPath.LastIndexOf('\\') + 1) + source;
                    }
                    byte[] ddsimage = new byte[0];
                    if (!File.Exists(path))
                    {
                        ddsimage = File.ReadAllBytes(path.Replace(".ddj", ".dds"));
                    }
                    else
                    {
                        ddsimage = File.ReadAllBytes(path).Skip<byte>(20).ToArray<byte>();
                    }
                    DDSImage image = new DDSImage(ddsimage);
                    bitmapImage = image.BitmapImage;
                    image = null;
                    BitmapData bitmapdata = bitmapImage.LockBits(new Rectangle(0, 0, bitmapImage.Width, bitmapImage.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmapdata.Width, bitmapdata.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapdata.Scan0);
                    bitmapImage.UnlockBits(bitmapdata);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 0x2601);
                    GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, 0x2100);
                    textureArray[i] = new MeshTexture(tName, texture);
                }
            }
            reader.Close();
            reader.Dispose();
            return textureArray;
        }

        private void readModel(string path)
        {
            BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)) {
                BaseStream = { Position = 12L }
            };
            int num = reader.ReadInt32();
            Stream baseStream = reader.BaseStream;
            baseStream.Position += 4L;
            int num2 = reader.ReadInt32();
            reader.BaseStream.Position = 0x48L;
            int num3 = reader.ReadInt32();
            Stream stream2 = reader.BaseStream;
            stream2.Position += num3;
            int count = reader.ReadInt32();
            this.Tex[this.readingMesh] = new string(reader.ReadChars(count));
            reader.BaseStream.Position = num;
            int num5 = reader.ReadInt32();
            this.Verts[this.readingMesh] = new float[num5, 3];
            this.UV[this.readingMesh] = new float[num5, 2];
            for (int i = 0; i < num5; i++)
            {
                this.Verts[this.readingMesh][i, 0] = reader.ReadSingle();
                this.Verts[this.readingMesh][i, 1] = reader.ReadSingle();
                this.Verts[this.readingMesh][i, 2] = reader.ReadSingle();
                Stream stream3 = reader.BaseStream;
                stream3.Position += 12L;
                this.UV[this.readingMesh][i, 0] = reader.ReadSingle();
                this.UV[this.readingMesh][i, 1] = reader.ReadSingle();
                Stream stream4 = reader.BaseStream;
                stream4.Position += 0x10L;
                int num7 = Math.Abs(reader.ReadInt32());
                if ((num7 < 0) || (num7 > 10))
                {
                    Stream stream5 = reader.BaseStream;
                    stream5.Position -= 8L;
                }
            }
            reader.BaseStream.Position = num2;
            int num8 = reader.ReadInt32();
            this.Faces[this.readingMesh] = new int[num8, 3];
            for (int j = 0; j < num8; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    this.Faces[this.readingMesh][j, k] = reader.ReadInt16();
                }
            }
            reader.Close();
            this.readingMesh++;
        }

        public void setRotation(float rot)
        {
            this.Theta = rot;
            this.rotBoundingBoxp2[0] = (this.boundingBoxp1[0] * ((float) Math.Cos((double) this.Theta))) - (this.boundingBoxp1[2] * ((float) Math.Sin((double) this.Theta)));
            this.rotBoundingBoxp2[2] = (this.boundingBoxp1[0] * ((float) Math.Sin((double) this.Theta))) + (this.boundingBoxp1[2] * ((float) Math.Cos((double) this.Theta)));
            this.rotBoundingBoxp1[0] = (this.boundingBoxp2[0] * ((float) Math.Cos((double) this.Theta))) - (this.boundingBoxp2[2] * ((float) Math.Sin((double) this.Theta)));
            this.rotBoundingBoxp1[2] = (this.boundingBoxp2[0] * ((float) Math.Sin((double) this.Theta))) + (this.boundingBoxp2[2] * ((float) Math.Cos((double) this.Theta)));
            if (this.rotBoundingBoxp1[2] > this.rotBoundingBoxp2[2])
            {
                float num = this.rotBoundingBoxp1[2];
                this.rotBoundingBoxp1[2] = this.rotBoundingBoxp2[2];
                this.rotBoundingBoxp2[2] = num;
            }
            if (this.rotBoundingBoxp1[0] > this.rotBoundingBoxp2[0])
            {
                float num2 = this.rotBoundingBoxp1[0];
                this.rotBoundingBoxp1[0] = this.rotBoundingBoxp2[0];
                this.rotBoundingBoxp2[0] = num2;
            }
        }
    }
}

