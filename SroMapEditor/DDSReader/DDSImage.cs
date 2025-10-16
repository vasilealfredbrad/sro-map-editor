namespace SroMapEditor.DDSReader
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;


    public class DDSImage
    {
        private uint alphabitdepth;
        private uint alphabitmask;
        private uint bbitmask;
        private uint blocksize;
        private uint bpp;
        private uint bps;
        private BinaryReader br;
        private byte[] compdata;
        private SroMapEditor.DDSReader.PixelFormat CompFormat;
        private uint compsize;
        private const uint DDS_ALPHAPIXELS = 1;
        private const uint DDS_FOURCC = 4;
        private static byte[] DDS_HEADER = Convert.FromBase64String("RERTIA==");
        private const uint DDS_LINEARSIZE = 0x80000;
        private const uint DDS_LUMINANCE = 0x20000;
        private const uint DDS_PITCH = 8;
        private uint ddscaps1;
        private uint ddscaps2;
        private uint ddscaps3;
        private uint ddscaps4;
        private uint depth;
        private uint flags1;
        private uint flags2;
        private uint fourcc;
        private const uint FOURCC_ATI1 = 0x31495441;
        private const uint FOURCC_ATI2 = 0x32495441;
        private const uint FOURCC_DOLLARNULL = 0x24;
        private const uint FOURCC_DXT1 = 0x31545844;
        private const uint FOURCC_DXT2 = 0x32545844;
        private const uint FOURCC_DXT3 = 0x33545844;
        private const uint FOURCC_DXT4 = 0x34545844;
        private const uint FOURCC_DXT5 = 0x35545844;
        private const uint FOURCC_oNULL = 0x6f;
        private const uint FOURCC_pNULL = 0x70;
        private const uint FOURCC_qNULL = 0x71;
        private const uint FOURCC_rNULL = 0x72;
        private const uint FOURCC_RXGB = 0x42475852;
        private const uint FOURCC_sNULL = 0x73;
        private const uint FOURCC_tNULL = 0x74;
        private uint gbitmask;
        private uint height;
        private Bitmap img;
        private uint linearsize;
        private uint mipmapcount;
        private byte[] rawidata;
        private uint rbitmask;
        private uint rgbbitcount;
        private byte[] signature;
        private uint size1;
        private uint size2;
        private uint sizeofplane;
        public const string SUPPORTED_ENCODERS = "DXT1 DXT3";
        private uint texturestage;
        private uint width;

        public unsafe DDSImage(byte[] ddsimage)
        {
            MemoryStream input = new MemoryStream(ddsimage.Length);
            input.Write(ddsimage, 0, ddsimage.Length);
            input.Seek(0L, SeekOrigin.Begin);
            this.br = new BinaryReader(input);
            this.signature = this.br.ReadBytes(4);
            if (!IsByteArrayEqual(this.signature, DDS_HEADER))
            {
                Console.WriteLine("Got header of '" + Encoding.ASCII.GetString(this.signature, 0, this.signature.Length) + "'.");
            }
            this.size1 = this.br.ReadUInt32();
            this.flags1 = this.br.ReadUInt32();
            this.height = this.br.ReadUInt32();
            this.width = this.br.ReadUInt32();
            this.linearsize = this.br.ReadUInt32();
            this.depth = this.br.ReadUInt32();
            this.mipmapcount = this.br.ReadUInt32();
            this.alphabitdepth = this.br.ReadUInt32();
            for (int i = 0; i < 10; i++)
            {
                this.br.ReadUInt32();
            }
            this.size2 = this.br.ReadUInt32();
            this.flags2 = this.br.ReadUInt32();
            this.fourcc = this.br.ReadUInt32();
            this.rgbbitcount = this.br.ReadUInt32();
            this.rbitmask = this.br.ReadUInt32();
            this.gbitmask = this.br.ReadUInt32();
            this.bbitmask = this.br.ReadUInt32();
            this.alphabitmask = this.br.ReadUInt32();
            this.ddscaps1 = this.br.ReadUInt32();
            this.ddscaps2 = this.br.ReadUInt32();
            this.ddscaps3 = this.br.ReadUInt32();
            this.ddscaps4 = this.br.ReadUInt32();
            this.texturestage = this.br.ReadUInt32();
            if (this.depth == 0)
            {
                this.depth = 1;
            }
            if ((this.flags2 & 4) > 0)
            {
                this.blocksize = (((this.width + 3) / 4) * ((this.height + 3) / 4)) * this.depth;
                switch (this.fourcc)
                {
                    case 0x31495441:
                        this.CompFormat = SroMapEditor.DDSReader.PixelFormat.ATI1N;
                        this.blocksize *= 8;
                        goto Label_056A;

                    case 0x31545844:
                        this.CompFormat = SroMapEditor.DDSReader.PixelFormat.DXT1;
                        this.blocksize *= 8;
                        goto Label_056A;

                    case 0x32495441:
                        this.CompFormat = SroMapEditor.DDSReader.PixelFormat.THREEDC;
                        this.blocksize *= 0x10;
                        goto Label_056A;

                    case 0x6f:
                        this.CompFormat = SroMapEditor.DDSReader.PixelFormat.R16F;
                        this.blocksize = ((this.width * this.height) * this.depth) * 2;
                        goto Label_056A;

                    case 0x70:
                        this.CompFormat = SroMapEditor.DDSReader.PixelFormat.G16R16F;
                        this.blocksize = ((this.width * this.height) * this.depth) * 4;
                        goto Label_056A;

                    case 0x71:
                        this.CompFormat = SroMapEditor.DDSReader.PixelFormat.A16B16G16R16F;
                        this.blocksize = ((this.width * this.height) * this.depth) * 8;
                        goto Label_056A;

                    case 0x72:
                        this.CompFormat = SroMapEditor.DDSReader.PixelFormat.R32F;
                        this.blocksize = ((this.width * this.height) * this.depth) * 4;
                        goto Label_056A;

                    case 0x73:
                        this.CompFormat = SroMapEditor.DDSReader.PixelFormat.G32R32F;
                        this.blocksize = ((this.width * this.height) * this.depth) * 8;
                        goto Label_056A;

                    case 0x74:
                        this.CompFormat = SroMapEditor.DDSReader.PixelFormat.A32B32G32R32F;
                        this.blocksize = ((this.width * this.height) * this.depth) * 0x10;
                        goto Label_056A;

                    case 0x24:
                        this.CompFormat = SroMapEditor.DDSReader.PixelFormat.A16B16G16R16;
                        this.blocksize = ((this.width * this.height) * this.depth) * 8;
                        goto Label_056A;

                    case 0x32545844:
                        this.CompFormat = SroMapEditor.DDSReader.PixelFormat.DXT2;
                        this.blocksize *= 0x10;
                        goto Label_056A;

                    case 0x33545844:
                        this.CompFormat = SroMapEditor.DDSReader.PixelFormat.DXT3;
                        this.blocksize *= 0x10;
                        goto Label_056A;

                    case 0x34545844:
                        this.CompFormat = SroMapEditor.DDSReader.PixelFormat.DXT4;
                        this.blocksize *= 0x10;
                        goto Label_056A;

                    case 0x35545844:
                        this.CompFormat = SroMapEditor.DDSReader.PixelFormat.DTX5;
                        this.blocksize *= 0x10;
                        goto Label_056A;

                    case 0x42475852:
                        this.CompFormat = SroMapEditor.DDSReader.PixelFormat.RXGB;
                        this.blocksize *= 0x10;
                        goto Label_056A;
                }
                this.CompFormat = SroMapEditor.DDSReader.PixelFormat.UNKNOWN;
                this.blocksize *= 0x10;
            }
            else
            {
                if ((this.flags2 & 0x20000) > 0)
                {
                    if ((this.flags2 & 1) > 0)
                    {
                        this.CompFormat = SroMapEditor.DDSReader.PixelFormat.LUMINANCE_ALPHA;
                    }
                    else
                    {
                        this.CompFormat = SroMapEditor.DDSReader.PixelFormat.LUMINANCE;
                    }
                }
                else if ((this.flags2 & 1) > 0)
                {
                    this.CompFormat = SroMapEditor.DDSReader.PixelFormat.ARGB;
                }
                else
                {
                    this.CompFormat = SroMapEditor.DDSReader.PixelFormat.RGB;
                }
                this.blocksize = ((this.width * this.height) * this.depth) * (this.rgbbitcount >> 3);
            }
        Label_056A:
            SroMapEditor.DDSReader.PixelFormat format1 = this.CompFormat;
            if (((this.flags1 & 0x80008) == 0) || (this.linearsize == 0))
            {
                this.flags1 |= 0x80000;
                this.linearsize = this.blocksize;
            }
            this.ReadData();
            this.bpp = this.PixelFormatToBpp(this.CompFormat);
            this.bps = (this.width * this.bpp) * this.PixelFormatToBpc(this.CompFormat);
            this.sizeofplane = this.bps * this.height;
            this.rawidata = new byte[((this.depth * this.sizeofplane) + (this.height * this.bps)) + (this.width * this.bpp)];
            switch (this.CompFormat)
            {
                case SroMapEditor.DDSReader.PixelFormat.ARGB:
                case SroMapEditor.DDSReader.PixelFormat.RGB:
                case SroMapEditor.DDSReader.PixelFormat.LUMINANCE:
                case SroMapEditor.DDSReader.PixelFormat.LUMINANCE_ALPHA:
                    this.DecompressARGB();
                    break;

                case SroMapEditor.DDSReader.PixelFormat.DXT1:
                    this.DecompressDXT1();
                    break;

                case SroMapEditor.DDSReader.PixelFormat.DXT3:
                    this.DecompressDXT3();
                    break;
            }
            this.img = new Bitmap((int) this.width, (int) this.height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData bitmapdata = this.img.LockBits(new Rectangle(0, 0, this.img.Width, this.img.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            IntPtr ptr = bitmapdata.Scan0;
            int num2 = (this.img.Width * this.img.Height) * 4;
            byte* numPtr = (byte*) ptr;
            for (int j = 0; j < num2; j += 4)
            {
                numPtr[j] = this.rawidata[j + 2];
                numPtr[j + 1] = this.rawidata[j + 1];
                numPtr[j + 2] = this.rawidata[j];
                numPtr[j + 3] = this.rawidata[j + 3];
            }
            this.img.UnlockBits(bitmapdata);
            this.rawidata = null;
            this.compdata = null;
        }

        private void DecompressARGB()
        {
        }

        private void DecompressDXT1()
        {
            Colour8888[] colourArray = new Colour8888[4];
            MemoryStream input = new MemoryStream(this.compdata.Length);
            input.Write(this.compdata, 0, this.compdata.Length);
            input.Seek(0L, SeekOrigin.Begin);
            BinaryReader reader = new BinaryReader(input);
            colourArray[0].a = 0xff;
            colourArray[1].a = 0xff;
            colourArray[2].a = 0xff;
            for (int i = 0; i < this.depth; i++)
            {
                for (int j = 0; j < this.height; j += 4)
                {
                    for (int k = 0; k < this.width; k += 4)
                    {
                        ushort data = reader.ReadUInt16();
                        ushort num2 = reader.ReadUInt16();
                        this.ReadColour(data, ref colourArray[0]);
                        this.ReadColour(num2, ref colourArray[1]);
                        uint num3 = reader.ReadUInt32();
                        if (data > num2)
                        {
                            colourArray[2].b = (byte) ((((2 * colourArray[0].b) + colourArray[1].b) + 1) / 3);
                            colourArray[2].g = (byte) ((((2 * colourArray[0].g) + colourArray[1].g) + 1) / 3);
                            colourArray[2].r = (byte) ((((2 * colourArray[0].r) + colourArray[1].r) + 1) / 3);
                            colourArray[3].b = (byte) (((colourArray[0].b + (2 * colourArray[1].b)) + 1) / 3);
                            colourArray[3].g = (byte) (((colourArray[0].g + (2 * colourArray[1].g)) + 1) / 3);
                            colourArray[3].r = (byte) (((colourArray[0].r + (2 * colourArray[1].r)) + 1) / 3);
                            colourArray[3].a = 0xff;
                        }
                        else
                        {
                            colourArray[2].b = (byte) ((colourArray[0].b + colourArray[1].b) / 2);
                            colourArray[2].g = (byte) ((colourArray[0].g + colourArray[1].g) / 2);
                            colourArray[2].r = (byte) ((colourArray[0].r + colourArray[1].r) / 2);
                            colourArray[3].b = 0;
                            colourArray[3].g = 0;
                            colourArray[3].r = 0;
                            colourArray[3].a = 0;
                        }
                        int num6 = 0;
                        int num7 = 0;
                        while (num6 < 4)
                        {
                            int num5 = 0;
                            while (num5 < 4)
                            {
                                int index = (int) ((num3 & (3 << ((num7 * 2) & 0x1f))) >> (num7 * 2));
                                if (((k + num5) < this.width) && ((j + num6) < this.height))
                                {
                                    uint num4 = (uint) (((i * this.sizeofplane) + ((j + num6) * this.bps)) + ((k + num5) * this.bpp));
                                    this.rawidata[num4] = colourArray[index].r;
                                    this.rawidata[(int) ((IntPtr) (num4 + 1))] = colourArray[index].g;
                                    this.rawidata[(int) ((IntPtr) (num4 + 2))] = colourArray[index].b;
                                    this.rawidata[(int) ((IntPtr) (num4 + 3))] = colourArray[index].a;
                                }
                                num5++;
                                num7++;
                            }
                            num6++;
                        }
                    }
                }
            }
        }

        private void DecompressDXT3()
        {
            Colour8888[] colourArray = new Colour8888[4];
            MemoryStream input = new MemoryStream(this.compdata.Length);
            input.Write(this.compdata, 0, this.compdata.Length);
            input.Seek(0L, SeekOrigin.Begin);
            BinaryReader reader = new BinaryReader(input);
            for (int i = 0; i < this.depth; i++)
            {
                for (int j = 0; j < this.height; j += 4)
                {
                    for (int k = 0; k < this.width; k += 4)
                    {
                        uint num2;
                        int num3;
                        byte[] buffer = reader.ReadBytes(8);
                        ushort data = reader.ReadUInt16();
                        ushort num12 = reader.ReadUInt16();
                        this.ReadColour(data, ref colourArray[0]);
                        this.ReadColour(num12, ref colourArray[1]);
                        uint num = reader.ReadUInt32();
                        colourArray[2].b = (byte) ((((2 * colourArray[0].b) + colourArray[1].b) + 1) / 3);
                        colourArray[2].g = (byte) ((((2 * colourArray[0].g) + colourArray[1].g) + 1) / 3);
                        colourArray[2].r = (byte) ((((2 * colourArray[0].r) + colourArray[1].r) + 1) / 3);
                        colourArray[3].b = (byte) (((colourArray[0].b + (2 * colourArray[1].b)) + 1) / 3);
                        colourArray[3].g = (byte) (((colourArray[0].g + (2 * colourArray[1].g)) + 1) / 3);
                        colourArray[3].r = (byte) (((colourArray[0].r + (2 * colourArray[1].r)) + 1) / 3);
                        int num4 = 0;
                        int num5 = 0;
                        while (num4 < 4)
                        {
                            num3 = 0;
                            while (num3 < 4)
                            {
                                int index = (int) ((num & (3 << ((num5 * 2) & 0x1f))) >> (num5 * 2));
                                if (((k + num3) < this.width) && ((j + num4) < this.height))
                                {
                                    num2 = (uint) (((i * this.sizeofplane) + ((j + num4) * this.bps)) + ((k + num3) * this.bpp));
                                    this.rawidata[num2] = colourArray[index].r;
                                    this.rawidata[(int) ((IntPtr) (num2 + 1))] = colourArray[index].g;
                                    this.rawidata[(int) ((IntPtr) (num2 + 2))] = colourArray[index].b;
                                }
                                num5++;
                                num3++;
                            }
                            num4++;
                        }
                        for (num4 = 0; num4 < 4; num4++)
                        {
                            ushort num10 = (ushort) (buffer[2 * num4] + (0x100 * buffer[(2 * num4) + 1]));
                            for (num3 = 0; num3 < 4; num3++)
                            {
                                if (((k + num3) < this.width) && ((j + num4) < this.height))
                                {
                                    num2 = (uint) ((((i * this.sizeofplane) + ((j + num4) * this.bps)) + ((k + num3) * this.bpp)) + ((long) 3L));
                                    this.rawidata[num2] = (byte) (num10 & 15);
                                    this.rawidata[num2] = (byte) (this.rawidata[num2] | (this.rawidata[num2] << 4));
                                }
                                num10 = (ushort) (num10 >> 4);
                            }
                        }
                    }
                }
            }
        }

        private static bool IsByteArrayEqual(byte[] arg0, byte[] arg1)
        {
            if (arg0.Length != arg1.Length)
            {
                return false;
            }
            for (int i = 0; i < arg0.Length; i++)
            {
                if (arg0[i] != arg1[i])
                {
                    return false;
                }
            }
            return true;
        }

        private uint PixelFormatToBpc(SroMapEditor.DDSReader.PixelFormat pf)
        {
            switch (pf)
            {
                case SroMapEditor.DDSReader.PixelFormat.A16B16G16R16:
                    return 2;

                case SroMapEditor.DDSReader.PixelFormat.R16F:
                case SroMapEditor.DDSReader.PixelFormat.G16R16F:
                case SroMapEditor.DDSReader.PixelFormat.A16B16G16R16F:
                    return 4;

                case SroMapEditor.DDSReader.PixelFormat.R32F:
                case SroMapEditor.DDSReader.PixelFormat.G32R32F:
                case SroMapEditor.DDSReader.PixelFormat.A32B32G32R32F:
                    return 4;
            }
            return 1;
        }

        private uint PixelFormatToBpp(SroMapEditor.DDSReader.PixelFormat pf)
        {
            switch (pf)
            {
                case SroMapEditor.DDSReader.PixelFormat.ARGB:
                case SroMapEditor.DDSReader.PixelFormat.LUMINANCE:
                case SroMapEditor.DDSReader.PixelFormat.LUMINANCE_ALPHA:
                    return (this.rgbbitcount / 8);

                case SroMapEditor.DDSReader.PixelFormat.RGB:
                case SroMapEditor.DDSReader.PixelFormat.THREEDC:
                case SroMapEditor.DDSReader.PixelFormat.RXGB:
                    return 3;

                case SroMapEditor.DDSReader.PixelFormat.ATI1N:
                    return 1;

                case SroMapEditor.DDSReader.PixelFormat.A16B16G16R16:
                case SroMapEditor.DDSReader.PixelFormat.A16B16G16R16F:
                case SroMapEditor.DDSReader.PixelFormat.G32R32F:
                    return 8;

                case SroMapEditor.DDSReader.PixelFormat.R16F:
                    return 2;

                case SroMapEditor.DDSReader.PixelFormat.A32B32G32R32F:
                    return 0x10;
            }
            return 4;
        }

        private uint PixelFormatToChannelCount(SroMapEditor.DDSReader.PixelFormat pf)
        {
            switch (pf)
            {
                case SroMapEditor.DDSReader.PixelFormat.RGB:
                case SroMapEditor.DDSReader.PixelFormat.THREEDC:
                case SroMapEditor.DDSReader.PixelFormat.RXGB:
                    return 3;

                case SroMapEditor.DDSReader.PixelFormat.ATI1N:
                case SroMapEditor.DDSReader.PixelFormat.LUMINANCE:
                case SroMapEditor.DDSReader.PixelFormat.R16F:
                case SroMapEditor.DDSReader.PixelFormat.R32F:
                    return 1;

                case SroMapEditor.DDSReader.PixelFormat.LUMINANCE_ALPHA:
                case SroMapEditor.DDSReader.PixelFormat.G16R16F:
                case SroMapEditor.DDSReader.PixelFormat.G32R32F:
                    return 2;
            }
            return 4;
        }

        private void ReadColour(ushort Data, ref Colour8888 op)
        {
            byte num3 = (byte) (Data & 0x1f);
            byte num2 = (byte) ((Data & 0x7e0) >> 5);
            byte num = (byte) ((Data & 0xf800) >> 11);
            op.r = (byte) ((num * 0xff) / 0x1f);
            op.g = (byte) ((num2 * 0xff) / 0x3f);
            op.b = (byte) ((num3 * 0xff) / 0x1f);
        }

        private void ReadData()
        {
            this.compdata = null;
            if ((this.flags1 & 0x80000) > 1)
            {
                this.compdata = this.br.ReadBytes((int) this.linearsize);
                this.compsize = (uint) this.compdata.Length;
            }
            else
            {
                uint num = (this.width * this.rgbbitcount) / 8;
                this.compsize = (num * this.height) * this.depth;
                this.compdata = new byte[this.compsize];
                MemoryStream stream = new MemoryStream((int) this.compsize);
                for (int i = 0; i < this.depth; i++)
                {
                    for (int j = 0; j < this.height; j++)
                    {
                        byte[] buffer = this.br.ReadBytes((int) this.bps);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
                stream.Seek(0L, SeekOrigin.Begin);
                stream.Read(this.compdata, 0, this.compdata.Length);
                stream.Close();
            }
        }

        public Bitmap BitmapImage
        {
            get
            {
                return this.img;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Colour8888
        {
            public byte r;
            public byte g;
            public byte b;
            public byte a;
        }
    }
}

