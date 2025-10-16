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
    using System.Windows.Forms;

    internal class Terrain
    {
        private List<int> FileTexIDs = new List<int>();
        private float[][][] Heights = new float[0x24][][];
        private string pathPrefix = @"Map\tile2d\";
        private int[][][] Tex = new int[0x24][][];
        private int[] TexIDs;
        private string[] TexPaths;

        public Terrain(string path)
        {
            this.getTexPaths();
            BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)) {
                BaseStream = { Position = 12L }
            };
            for (int i = 0; i < 0x24; i++)
            {
                this.Heights[i] = new float[0x11][];
                this.Tex[i] = new int[0x11][];
                Stream baseStream = reader.BaseStream;
                baseStream.Position += 6L;
                for (int j = 0; j < 0x11; j++)
                {
                    this.Heights[i][j] = new float[0x11];
                    this.Tex[i][j] = new int[0x11];
                }
                for (int k = 0; k < 0x11; k++)
                {
                    for (int m = 0; m < 0x11; m++)
                    {
                        this.Heights[i][m][k] = reader.ReadSingle();
                        this.Tex[i][m][k] = reader.ReadByte();
                        Stream stream2 = reader.BaseStream;
                        stream2.Position += 2L;
                    }
                }
                Stream stream3 = reader.BaseStream;
                stream3.Position += 0x222L;
            }
            reader.Close();
            reader.Dispose();
            this.getTex();
        }

        public void Draw()
        {
            GL.Color3((byte) 0xff, (byte) 0xff, (byte) 0xff);
            for (int i = 0; i < 0x24; i++)
            {
                for (int j = 0; j < 0x10; j++)
                {
                    for (int k = 0; k < 0x10; k++)
                    {
                        GL.Enable(EnableCap.Texture2D);
                        GL.BindTexture(TextureTarget.Texture2D, this.TexIDs[this.FileTexIDs.IndexOf(this.Tex[i][j][k])]);
                        GL.Begin(BeginMode.TriangleStrip);
                        GL.TexCoord2((float) (((float) j) / 2f), (float) (((float) k) / 2f));
                        GL.Vertex3((double) ((j * 20) + ((i % 6) * 320)), (k * 20) + (Math.Floor((double) (((double) i) / 6.0)) * 320.0), (double) this.Heights[i][j][k]);
                        GL.TexCoord2((float) (((float) (j + 1)) / 2f), (float) (((float) k) / 2f));
                        GL.Vertex3((double) (((j + 1) * 20) + ((i % 6) * 320)), (k * 20) + (Math.Floor((double) (((double) i) / 6.0)) * 320.0), (double) this.Heights[i][j + 1][k]);
                        GL.TexCoord2((float) (((float) j) / 2f), (float) (((float) (k + 1)) / 2f));
                        GL.Vertex3((double) ((j * 20) + ((i % 6) * 320)), ((k + 1) * 20) + (Math.Floor((double) (((double) i) / 6.0)) * 320.0), (double) this.Heights[i][j][k + 1]);
                        GL.TexCoord2((float) (((float) (j + 1)) / 2f), (float) (((float) (k + 1)) / 2f));
                        GL.Vertex3((double) (((j + 1) * 20) + ((i % 6) * 320)), ((k + 1) * 20) + (Math.Floor((double) (((double) i) / 6.0)) * 320.0), (double) this.Heights[i][j + 1][k + 1]);
                        GL.End();
                        GL.Disable(EnableCap.Texture2D);
                    }
                }
            }
        }

        private void getTex()
        {
            List<int> list = new List<int>();
            List<int> list2 = new List<int>();
            for (int i = 0; i < 0x24; i++)
            {
                for (int j = 0; j < 0x11; j++)
                {
                    for (int k = 0; k < 0x11; k++)
                    {
                        if (!list.Contains(this.Tex[i][j][k]))
                        {
                            list.Add(this.Tex[i][j][k]);
                            int texture = GL.GenTexture();
                            GL.BindTexture(TextureTarget.Texture2D, texture);
                            System.Drawing.Bitmap bitmapImage = null;
                            string path = this.pathPrefix + this.TexPaths[this.Tex[i][j][k]];
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
                            this.FileTexIDs.Add(this.Tex[i][j][k]);
                            list2.Add(texture);
                        }
                    }
                }
            }
            this.TexIDs = list2.ToArray();
            list2.Clear();
        }

        private void getTexPaths()
        {
            if (!File.Exists(@"Map\tile2d.ifo"))
            {
                MessageBox.Show("Could not find tile2d.ifo. Terminating application.");
                Environment.Exit(1);
            }
            string[] strArray = File.ReadAllLines(@"Map\tile2d.ifo");
            this.TexPaths = new string[strArray.Length - 2];
            for (int i = 0; i < this.TexPaths.Length; i++)
            {
                string str = strArray[i + 2].Split(new char[] { ' ' })[3];
                this.TexPaths[i] = str.Substring(1, str.Length - 2);
            }
        }
    }
}

