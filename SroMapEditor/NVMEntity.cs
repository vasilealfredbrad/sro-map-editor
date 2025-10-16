namespace SroMapEditor
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct NVMEntity
    {
        public int id;
        public float X;
        public float Y;
        public float Z;
        public short UK1;
        public float UK2;
        public short UK3;
        public short UK4;
        public short UK5;
        public short Grid;
        public byte[] xtra;
    }
}

