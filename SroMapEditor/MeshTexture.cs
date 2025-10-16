namespace SroMapEditor
{
    using System;

    public class MeshTexture
    {
        public int ID;
        public string name;

        public MeshTexture(string tName, int tID)
        {
            this.name = tName;
            this.ID = tID;
        }
    }
}

