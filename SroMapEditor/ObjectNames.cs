namespace SroMapEditor
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    public class ObjectNames
    {
        public string[] names;

        public ObjectNames()
        {
            if (!File.Exists(@"Map\object.ifo"))
            {
                MessageBox.Show("Could not find object.ifo. Terminating application.");
                Environment.Exit(1);
            }
            string[] strArray = File.ReadAllLines(@"Map\object.ifo");
            this.names = new string[strArray.Length - 2];
            for (int i = 0; i < (strArray.Length - 2); i++)
            {
                string str = strArray[i + 2].Substring(strArray[i + 2].IndexOf(' ') + 1);
                str = str.Substring(str.IndexOf(' ') + 1);
                this.names[i] = str.Remove(str.Length - 1).Substring(1);
            }
        }
    }
}

