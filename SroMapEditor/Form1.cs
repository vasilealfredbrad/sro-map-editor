namespace SroMapEditor
{
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    public class Form1 : Form
    {
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private Button button7;
        private Button button8;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private CheckBox checkBox3;
        private CheckBox checkBox4;
        private CheckBox checkBox5;
        private CheckBox checkBox6;
        private CheckBox checkBox7;
        private IContainer components;
        private bool dragging;
        private Point dragStart;
        private FolderBrowserDialog folderBrowserDialog1;
        private Form2 form2;
        private GLControl glControl1;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private Label label1;
        private Label label10;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private ListBox listBox1;
        private ListBox listBox2;
        private ListBox listBox3;
        private bool mapLoaded = false;
        private NumericUpDown numericUpDown1;
        private NumericUpDown numericUpDown2;
        private NumericUpDown numericUpDown3;
        private NVM nvmFile;
        private List<MapObject> Objects;
        private ObjectNames ObjNames = new ObjectNames();
        private OpenFileDialog openFileDialog1;
        private int OX;
        private int OY;
        private string pathPref = @"Data\";
        private int rotHor;
        private int rotVert;
        private SaveFileDialog saveFileDialog1;
        private SaveFileDialog saveFileDialog2;
        private int selectedObj = -1;
        private Terrain terrainmap;
        private List<int> texIDs = new List<int>();
        private List<string> texNames = new List<string>();
        private TextBox textBox1;
        private TextBox textBox2;
        private TrackBar trackBar1;
        private double zoom = 1.0;

        public Form1()
        {
            base.MouseWheel += new MouseEventHandler(this.onMouseWheel);
            this.InitializeComponent();
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            while (this.glControl1.IsIdle)
            {
                if (this.form2 == null)
                {
                    this.Render();
                }
                else
                {
                    this.form2.RenderGL();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = this.openFileDialog1.FileName.Substring(0, this.openFileDialog1.FileName.LastIndexOf('.')) + ".m";
                if (File.Exists(path))
                {
                    this.terrainmap = new Terrain(path);
                }
                else
                {
                    MessageBox.Show("Terrain not found.");
                }
                OFile file = new OFile(this.openFileDialog1.FileName);
                this.OX = file.OX;
                this.OY = file.OY;
                this.Text = string.Concat(new object[] { "SRO Map Viewer - X: ", this.OX, " Y:", this.OY });
                this.Objects = file.objects;
                List<MeshTexture[]> list = new List<MeshTexture[]>();
                foreach (MapObject obj2 in this.Objects)
                {
                    if (this.ObjNames.names[obj2.nameI].Contains(".bsr"))
                    {
                        obj2.LoadFiles(this.pathPref, this.ObjNames.names[obj2.nameI]);
                        list.Add(obj2.readMaterial());
                    }
                }
                foreach (MeshTexture[] textureArray in list)
                {
                    foreach (MeshTexture texture in textureArray)
                    {
                        if ((texture != null) && !this.texNames.Contains(texture.name))
                        {
                            this.texNames.Add(texture.name);
                            this.texIDs.Add(texture.ID);
                        }
                    }
                }
                foreach (MapObject obj3 in this.Objects)
                {
                    obj3.FindTex(this.texNames, this.texIDs);
                }
                this.mapLoaded = true;
                this.groupBox3.Enabled = true;
                this.button2.Enabled = true;
                this.fillLists();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.saveMap(this.saveFileDialog1.FileName);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.form2 = new Form2(this.ObjNames, this.Objects[this.selectedObj].nameI);
            this.form2.ShowDialog();
            int selectedItem = this.form2.SelectedItem;
            this.form2 = null;
            if (!this.glControl1.Context.IsCurrent)
            {
                this.glControl1.MakeCurrent();
            }
            if (selectedItem != -1)
            {
                this.Objects[this.selectedObj].nameI = selectedItem;
                this.Objects[this.selectedObj].LoadFiles(this.pathPref, this.ObjNames.names[selectedItem]);
                foreach (MeshTexture[] textureArray in new List<MeshTexture[]> { this.Objects[this.selectedObj].readMaterial() })
                {
                    foreach (MeshTexture texture in textureArray)
                    {
                        if ((texture != null) && !this.texNames.Contains(texture.name))
                        {
                            this.texNames.Add(texture.name);
                            this.texIDs.Add(texture.ID);
                        }
                    }
                }
                this.Objects[this.selectedObj].FindTex(this.texNames, this.texIDs);
                this.fillLists();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.form2 = new Form2(this.ObjNames, -1);
            this.form2.ShowDialog();
            int selectedItem = this.form2.SelectedItem;
            this.form2 = null;
            if (!this.glControl1.Context.IsCurrent)
            {
                this.glControl1.MakeCurrent();
            }
            if (selectedItem != -1)
            {
                MapObject item = new MapObject {
                    nameI = selectedItem
                };
                item.LoadFiles(this.pathPref, this.ObjNames.names[selectedItem]);
                foreach (MeshTexture[] textureArray in new List<MeshTexture[]> { item.readMaterial() })
                {
                    foreach (MeshTexture texture in textureArray)
                    {
                        if ((texture != null) && !this.texNames.Contains(texture.name))
                        {
                            this.texNames.Add(texture.name);
                            this.texIDs.Add(texture.ID);
                        }
                    }
                }
                item.FindTex(this.texNames, this.texIDs);
                item.X = 0f;
                item.Y = 0f;
                item.Z = 0f;
                item.Theta = 0f;
                item.ID = this.genUniqueID();
                item.DistFade = true;
                this.Objects.Add(item);
                this.selectedObj = this.Objects.Count - 1;
                this.fillLists();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MapObject item = new MapObject {
                nameI = this.Objects[this.selectedObj].nameI
            };
            item.LoadFiles(this.pathPref, this.ObjNames.names[item.nameI]);
            foreach (MeshTexture[] textureArray in new List<MeshTexture[]> { item.readMaterial() })
            {
                foreach (MeshTexture texture in textureArray)
                {
                    if ((texture != null) && !this.texNames.Contains(texture.name))
                    {
                        this.texNames.Add(texture.name);
                        this.texIDs.Add(texture.ID);
                    }
                }
            }
            item.FindTex(this.texNames, this.texIDs);
            item.X = this.Objects[this.selectedObj].X;
            item.Y = this.Objects[this.selectedObj].Y;
            item.Z = this.Objects[this.selectedObj].Z;
            item.Theta = this.Objects[this.selectedObj].Theta;
            item.ID = this.genUniqueID();
            this.Objects.Add(item);
            this.selectedObj = this.Objects.Count - 1;
            this.fillLists();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (this.selectedObj != -1)
            {
                this.Objects.RemoveAt(this.selectedObj);
                this.selectedObj = -1;
                this.fillLists();
            }
            else
            {
                MessageBox.Show("Select an object first!");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.nvmFile = new NVM(this.folderBrowserDialog1.SelectedPath + @"\nv_" + this.OY.ToString("X").ToLower() + this.OX.ToString("X").ToLower() + ".nvm");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.saveFileDialog2.FileName = this.nvmFile.filepath;
            this.nvmFile.setEntities(this.Objects, this.pathPref, this.ObjNames.names);
            if (this.saveFileDialog2.ShowDialog() == DialogResult.OK)
            {
                this.nvmFile.saveNVM(this.saveFileDialog2.FileName, this.OX, this.OY);
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            this.Objects[this.selectedObj].DistFade = this.checkBox6.Checked;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void fillLists()
        {
            this.listBox1.Items.Clear();
            this.listBox2.Items.Clear();
            this.listBox3.Items.Clear();
            for (int i = 0; i < this.Objects.Count; i++)
            {
                MapObject obj2 = this.Objects[i];
                string str = this.ObjNames.names[obj2.nameI];
                if (str.Contains(@"\bldg\"))
                {
                    this.listBox1.Items.Add(i + " - " + str.Substring(str.LastIndexOf('\\') + 1));
                    if (i == this.selectedObj)
                    {
                        this.listBox1.SelectedIndex = this.listBox1.Items.Count - 1;
                    }
                }
                else if (str.Contains(@"\nature\"))
                {
                    this.listBox2.Items.Add(i + " - " + str.Substring(str.LastIndexOf('\\') + 1));
                    if (i == this.selectedObj)
                    {
                        this.listBox2.SelectedIndex = this.listBox2.Items.Count - 1;
                    }
                }
                else
                {
                    this.listBox3.Items.Add(i + " - " + str.Substring(str.LastIndexOf('\\') + 1));
                    if (i == this.selectedObj)
                    {
                        this.listBox3.SelectedIndex = this.listBox3.Items.Count - 1;
                    }
                }
            }
        }

        private int genUniqueID()
        {
            int item = 0;
            List<int> list = new List<int>();
            foreach (MapObject obj2 in this.Objects)
            {
                list.Add(obj2.ID);
            }
            while (list.Contains(item))
            {
                item++;
            }
            return item;
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.Black);
            int width = this.glControl1.Width;
            int height = this.glControl1.Height;
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0.0, 1920.0, 0.0, 1920.0, -5000.0, 5000.0);
            GL.Viewport(0, 0, width, height);
            GL.PointSize(3f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
            Application.Idle += new EventHandler(this.Application_Idle);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(Form1));
            this.glControl1 = new GLControl();
            this.button1 = new Button();
            this.button2 = new Button();
            this.openFileDialog1 = new OpenFileDialog();
            this.label1 = new Label();
            this.listBox1 = new ListBox();
            this.listBox2 = new ListBox();
            this.label2 = new Label();
            this.listBox3 = new ListBox();
            this.label3 = new Label();
            this.groupBox1 = new GroupBox();
            this.checkBox7 = new CheckBox();
            this.checkBox5 = new CheckBox();
            this.checkBox4 = new CheckBox();
            this.checkBox3 = new CheckBox();
            this.checkBox2 = new CheckBox();
            this.checkBox1 = new CheckBox();
            this.groupBox2 = new GroupBox();
            this.checkBox6 = new CheckBox();
            this.button3 = new Button();
            this.label10 = new Label();
            this.label9 = new Label();
            this.label8 = new Label();
            this.textBox2 = new TextBox();
            this.textBox1 = new TextBox();
            this.label7 = new Label();
            this.label6 = new Label();
            this.label5 = new Label();
            this.label4 = new Label();
            this.numericUpDown3 = new NumericUpDown();
            this.numericUpDown2 = new NumericUpDown();
            this.numericUpDown1 = new NumericUpDown();
            this.trackBar1 = new TrackBar();
            this.groupBox3 = new GroupBox();
            this.button6 = new Button();
            this.button5 = new Button();
            this.button4 = new Button();
            this.saveFileDialog1 = new SaveFileDialog();
            this.button7 = new Button();
            this.button8 = new Button();
            this.folderBrowserDialog1 = new FolderBrowserDialog();
            this.saveFileDialog2 = new SaveFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.numericUpDown3.BeginInit();
            this.numericUpDown2.BeginInit();
            this.numericUpDown1.BeginInit();
            this.trackBar1.BeginInit();
            this.groupBox3.SuspendLayout();
            base.SuspendLayout();
            this.glControl1.BackColor = Color.Black;
            this.glControl1.Location = new Point(0, 0);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new Size(0x2e9, 630);
            this.glControl1.TabIndex = 0;
            this.glControl1.VSync = false;
            this.glControl1.Load += new EventHandler(this.glControl1_Load);
            this.glControl1.MouseDown += new MouseEventHandler(this.onGLMouseDown);
            this.glControl1.MouseMove += new MouseEventHandler(this.onGLMouseMove);
            this.glControl1.MouseUp += new MouseEventHandler(this.onGLMouseUp);
            this.button1.Location = new Point(0x2f2, 3);
            this.button1.Name = "button1";
            this.button1.Size = new Size(0x94, 0x16);
            this.button1.TabIndex = 1;
            this.button1.Text = "Open Map";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            this.button2.Enabled = false;
            this.button2.Location = new Point(0x2f2, 0x18);
            this.button2.Name = "button2";
            this.button2.Size = new Size(0x94, 0x16);
            this.button2.TabIndex = 2;
            this.button2.Text = "Save Map";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new EventHandler(this.button2_Click);
            this.openFileDialog1.Filter = "Object Maps|*.o2";
            this.label1.AutoSize = true;
            this.label1.Location = new Point(0x38c, 10);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x31, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Buildings";
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new Point(0x38f, 0x1a);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new Size(0x91, 0xe1);
            this.listBox1.TabIndex = 4;
            this.listBox1.SelectedIndexChanged += new EventHandler(this.listBox1_SelectedIndexChanged);
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new Point(0x38f, 0x111);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new Size(0x91, 0xad);
            this.listBox2.TabIndex = 6;
            this.listBox2.SelectedIndexChanged += new EventHandler(this.listBox2_SelectedIndexChanged);
            this.label2.Location = new Point(0x38c, 0x101);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x31, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Nature";
            this.listBox3.FormattingEnabled = true;
            this.listBox3.Location = new Point(0x38f, 0x1d3);
            this.listBox3.Name = "listBox3";
            this.listBox3.Size = new Size(0x91, 0x93);
            this.listBox3.TabIndex = 8;
            this.listBox3.SelectedIndexChanged += new EventHandler(this.listBox3_SelectedIndexChanged);
            this.label3.Location = new Point(0x38c, 0x1c3);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x31, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Other";
            this.groupBox1.Controls.Add(this.checkBox7);
            this.groupBox1.Controls.Add(this.checkBox5);
            this.groupBox1.Controls.Add(this.checkBox4);
            this.groupBox1.Controls.Add(this.checkBox3);
            this.groupBox1.Controls.Add(this.checkBox2);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Location = new Point(0x2ef, 0x56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(0x9a, 0x9a);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            this.checkBox7.AutoSize = true;
            this.checkBox7.Location = new Point(12, 0x83);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new Size(80, 0x11);
            this.checkBox7.TabIndex = 5;
            this.checkBox7.Text = "Show NVM";
            this.checkBox7.UseVisualStyleBackColor = true;
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new Point(12, 110);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new Size(120, 0x11);
            this.checkBox5.TabIndex = 4;
            this.checkBox5.Text = "Highlight unknown2";
            this.checkBox5.UseVisualStyleBackColor = true;
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new Point(12, 0x57);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new Size(0x66, 0x11);
            this.checkBox4.TabIndex = 3;
            this.checkBox4.Text = "Highlight Fading";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new Point(12, 0x3f);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new Size(0x4b, 0x11);
            this.checkBox3.TabIndex = 2;
            this.checkBox3.Text = "Show Grid";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = CheckState.Checked;
            this.checkBox2.Location = new Point(12, 40);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new Size(0x5c, 0x11);
            this.checkBox2.TabIndex = 1;
            this.checkBox2.Text = "Show Objects";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = CheckState.Checked;
            this.checkBox1.Location = new Point(12, 0x13);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new Size(0x59, 0x11);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Show Terrain";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.groupBox2.Controls.Add(this.checkBox6);
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.textBox2);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.numericUpDown3);
            this.groupBox2.Controls.Add(this.numericUpDown2);
            this.groupBox2.Controls.Add(this.numericUpDown1);
            this.groupBox2.Controls.Add(this.trackBar1);
            this.groupBox2.Enabled = false;
            this.groupBox2.Location = new Point(0x2f2, 0x16a);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new Size(0x94, 0x100);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Selected Object";
            this.checkBox6.AutoSize = true;
            this.checkBox6.Location = new Point(9, 0x53);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new Size(0x5f, 0x11);
            this.checkBox6.TabIndex = 14;
            this.checkBox6.Text = "Distance Fade";
            this.checkBox6.UseVisualStyleBackColor = true;
            this.checkBox6.CheckedChanged += new EventHandler(this.checkBox6_CheckedChanged);
            this.button3.Location = new Point(20, 0x38);
            this.button3.Name = "button3";
            this.button3.Size = new Size(0x69, 0x17);
            this.button3.TabIndex = 13;
            this.button3.Text = "Change Type";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new EventHandler(this.button3_Click);
            this.label10.AutoSize = true;
            this.label10.Location = new Point(0x2e, 0x29);
            this.label10.Name = "label10";
            this.label10.Size = new Size(0x21, 13);
            this.label10.TabIndex = 12;
            this.label10.Text = "None";
            this.label9.AutoSize = true;
            this.label9.Location = new Point(6, 0x29);
            this.label9.Name = "label9";
            this.label9.Size = new Size(0x22, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "Type:";
            this.label8.AutoSize = true;
            this.label8.Location = new Point(6, 0x16);
            this.label8.Name = "label8";
            this.label8.Size = new Size(0x3a, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "Unique ID:";
            this.textBox2.Location = new Point(0x3e, 0x13);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Size(80, 20);
            this.textBox2.TabIndex = 9;
            this.textBox1.Location = new Point(0x3e, 0xba);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new Size(80, 20);
            this.textBox1.TabIndex = 8;
            this.label7.AutoSize = true;
            this.label7.Location = new Point(6, 0xbc);
            this.label7.Name = "label7";
            this.label7.Size = new Size(50, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Rotation:";
            this.label6.AutoSize = true;
            this.label6.Location = new Point(6, 0x9a);
            this.label6.Name = "label6";
            this.label6.Size = new Size(0x11, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Z:";
            this.label5.AutoSize = true;
            this.label5.Location = new Point(6, 130);
            this.label5.Name = "label5";
            this.label5.Size = new Size(0x11, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Y:";
            this.label4.AutoSize = true;
            this.label4.Location = new Point(6, 0x6a);
            this.label4.Name = "label4";
            this.label4.Size = new Size(0x11, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "X:";
            this.numericUpDown3.Location = new Point(0x25, 0x68);
            int[] bits = new int[4];
            bits[0] = 0xf4240;
            this.numericUpDown3.Maximum = new decimal(bits);
            int[] numArray2 = new int[4];
            numArray2[0] = 0x989680;
            numArray2[3] = -2147483648;
            this.numericUpDown3.Minimum = new decimal(numArray2);
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new Size(0x69, 20);
            this.numericUpDown3.TabIndex = 3;
            this.numericUpDown3.ValueChanged += new EventHandler(this.numericUpDown3_ValueChanged);
            this.numericUpDown2.Location = new Point(0x25, 0x80);
            int[] numArray3 = new int[4];
            numArray3[0] = 0x989680;
            this.numericUpDown2.Maximum = new decimal(numArray3);
            int[] numArray4 = new int[4];
            numArray4[0] = 0x989680;
            numArray4[3] = -2147483648;
            this.numericUpDown2.Minimum = new decimal(numArray4);
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new Size(0x69, 20);
            this.numericUpDown2.TabIndex = 2;
            this.numericUpDown2.ValueChanged += new EventHandler(this.numericUpDown2_ValueChanged);
            this.numericUpDown1.Location = new Point(0x25, 0x98);
            int[] numArray5 = new int[4];
            numArray5[0] = 0xf4240;
            this.numericUpDown1.Maximum = new decimal(numArray5);
            int[] numArray6 = new int[4];
            numArray6[0] = 0xf4240;
            numArray6[3] = -2147483648;
            this.numericUpDown1.Minimum = new decimal(numArray6);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new Size(0x69, 20);
            this.numericUpDown1.TabIndex = 1;
            this.numericUpDown1.ValueChanged += new EventHandler(this.numericUpDown1_ValueChanged);
            this.trackBar1.Location = new Point(20, 0xd1);
            this.trackBar1.Maximum = 0x274;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new Size(0x68, 0x2d);
            this.trackBar1.TabIndex = 0;
            this.trackBar1.Scroll += new EventHandler(this.trackBar1_Scroll);
            this.groupBox3.Controls.Add(this.button6);
            this.groupBox3.Controls.Add(this.button5);
            this.groupBox3.Controls.Add(this.button4);
            this.groupBox3.Enabled = false;
            this.groupBox3.Location = new Point(0x2ef, 0xf6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new Size(0x97, 110);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Modify";
            this.button6.Location = new Point(7, 0x4d);
            this.button6.Name = "button6";
            this.button6.Size = new Size(0x8a, 0x17);
            this.button6.TabIndex = 2;
            this.button6.Text = "Delete Current";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new EventHandler(this.button6_Click);
            this.button5.Location = new Point(7, 0x30);
            this.button5.Name = "button5";
            this.button5.Size = new Size(0x8a, 0x17);
            this.button5.TabIndex = 1;
            this.button5.Text = "Clone Current";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new EventHandler(this.button5_Click);
            this.button4.Location = new Point(7, 0x13);
            this.button4.Name = "button4";
            this.button4.Size = new Size(0x8a, 0x17);
            this.button4.TabIndex = 0;
            this.button4.Text = "Add Object";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new EventHandler(this.button4_Click);
            this.saveFileDialog1.Filter = "Object Map|*.o2";
            this.button7.Location = new Point(0x2f2, 0x2d);
            this.button7.Name = "button7";
            this.button7.Size = new Size(0x94, 0x16);
            this.button7.TabIndex = 12;
            this.button7.Text = "Open NVM";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new EventHandler(this.button7_Click);
            this.button8.Location = new Point(0x2f2, 0x42);
            this.button8.Name = "button8";
            this.button8.Size = new Size(0x94, 0x16);
            this.button8.TabIndex = 13;
            this.button8.Text = "Save NVM";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new EventHandler(this.button8_Click);
            this.saveFileDialog2.Filter = "Collision map|*.nvm";
            this.saveFileDialog2.Title = "Save collisionmap";
            base.AutoScaleDimensions = new SizeF(6f, 13f);
//            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x42c, 630);
            base.Controls.Add(this.button8);
            base.Controls.Add(this.button7);
            base.Controls.Add(this.groupBox3);
            base.Controls.Add(this.groupBox2);
            base.Controls.Add(this.groupBox1);
            base.Controls.Add(this.listBox3);
            base.Controls.Add(this.label3);
            base.Controls.Add(this.listBox2);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.listBox1);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.button2);
            base.Controls.Add(this.button1);
            base.Controls.Add(this.glControl1);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "Form1";
            this.Text = "SRO Map Editor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.numericUpDown3.EndInit();
            this.numericUpDown2.EndInit();
            this.numericUpDown1.EndInit();
            this.trackBar1.EndInit();
            this.groupBox3.ResumeLayout(false);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex != -1)
            {
                this.selectedObj = int.Parse(((string) this.listBox1.SelectedItem).Split(new char[] { ' ' })[0]);
                this.listBox2.SelectedIndex = -1;
                this.listBox3.SelectedIndex = -1;
                this.groupBox2.Enabled = true;
                this.textBox2.Text = this.Objects[this.selectedObj].ID.ToString();
                this.label10.Text = ((string) this.listBox1.SelectedItem).Split(new char[] { ' ' })[2];
                this.textBox1.Text = this.Objects[this.selectedObj].Theta.ToString();
                this.numericUpDown1.Value = (int) this.Objects[this.selectedObj].Z;
                this.numericUpDown2.Value = (int) this.Objects[this.selectedObj].Y;
                this.numericUpDown3.Value = (int) this.Objects[this.selectedObj].X;
                this.checkBox6.Checked = this.Objects[this.selectedObj].DistFade;
                if (this.Objects[this.selectedObj].Theta < 0f)
                {
                    MapObject local1 = this.Objects[this.selectedObj];
                    local1.Theta += 6.283185f;
                }
                this.trackBar1.Value = (int) (this.Objects[this.selectedObj].Theta * 100f);
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox2.SelectedIndex != -1)
            {
                this.selectedObj = int.Parse(((string) this.listBox2.SelectedItem).Split(new char[] { ' ' })[0]);
                this.listBox1.SelectedIndex = -1;
                this.listBox3.SelectedIndex = -1;
                this.groupBox2.Enabled = true;
                this.textBox2.Text = this.Objects[this.selectedObj].ID.ToString();
                this.label10.Text = ((string) this.listBox2.SelectedItem).Split(new char[] { ' ' })[2];
                this.textBox1.Text = this.Objects[this.selectedObj].Theta.ToString();
                this.numericUpDown1.Value = (decimal) this.Objects[this.selectedObj].Z;
                this.numericUpDown2.Value = (decimal) this.Objects[this.selectedObj].Y;
                this.numericUpDown3.Value = (decimal) this.Objects[this.selectedObj].X;
                this.checkBox6.Checked = this.Objects[this.selectedObj].DistFade;
                if (this.Objects[this.selectedObj].Theta < 0f)
                {
                    MapObject local1 = this.Objects[this.selectedObj];
                    local1.Theta += 6.283185f;
                }
                this.trackBar1.Value = (int) (this.Objects[this.selectedObj].Theta * 100f);
            }
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox3.SelectedIndex != -1)
            {
                this.selectedObj = int.Parse(((string) this.listBox3.SelectedItem).Split(new char[] { ' ' })[0]);
                this.listBox2.SelectedIndex = -1;
                this.listBox1.SelectedIndex = -1;
                this.groupBox2.Enabled = true;
                this.textBox2.Text = this.Objects[this.selectedObj].ID.ToString();
                this.label10.Text = ((string) this.listBox3.SelectedItem).Split(new char[] { ' ' })[2];
                this.textBox1.Text = this.Objects[this.selectedObj].Theta.ToString();
                this.numericUpDown1.Value = (decimal) this.Objects[this.selectedObj].Z;
                this.numericUpDown2.Value = (decimal) this.Objects[this.selectedObj].Y;
                this.numericUpDown3.Value = (decimal) this.Objects[this.selectedObj].X;
                this.checkBox6.Checked = this.Objects[this.selectedObj].DistFade;
                if (this.Objects[this.selectedObj].Theta < 0f)
                {
                    MapObject local1 = this.Objects[this.selectedObj];
                    local1.Theta += 6.283185f;
                }
                this.trackBar1.Value = (int) (this.Objects[this.selectedObj].Theta * 100f);
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.Objects[this.selectedObj].Z = (float) this.numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            this.Objects[this.selectedObj].Y = (float) this.numericUpDown2.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            this.Objects[this.selectedObj].X = (float) this.numericUpDown3.Value;
        }

        private void onGLMouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Right) && !this.dragging)
            {
                this.dragging = true;
                this.dragStart = e.Location;
            }
        }

        private void onGLMouseMove(object sender, MouseEventArgs e)
        {
            if (this.dragging)
            {
                this.rotHor += e.Location.X - this.dragStart.X;
                this.rotVert += e.Location.Y - this.dragStart.Y;
            }
        }

        private void onGLMouseUp(object sender, MouseEventArgs e)
        {
            this.dragging = false;
        }

        private void onMouseWheel(object sender, MouseEventArgs e)
        {
            if ((this.mapLoaded && (base.PointToClient(e.Location).X < this.glControl1.Width)) && (base.PointToClient(e.Location).Y < this.glControl1.Height))
            {
                this.zoom += ((double) e.Delta) / 300.0;
                if (this.zoom < 0.1)
                {
                    this.zoom = 0.1;
                }
            }
        }

        private void Render()
        {
            if (!this.glControl1.Context.IsCurrent)
            {
                this.glControl1.MakeCurrent();
            }
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Modelview);
            GL.LoadIdentity();
            new List<int>();
            if (this.mapLoaded)
            {
                GL.PushMatrix();
                GL.Translate((float) 960f, (float) 960f, (float) 0f);
                GL.Scale(this.zoom, this.zoom, this.zoom);
                GL.Rotate((double) (((double) this.rotHor) / 100.0), (double) 0.0, (double) 1.0, (double) 0.0);
                GL.Rotate((double) (((double) this.rotVert) / 100.0), (double) 1.0, (double) 0.0, (double) 0.0);
                GL.Translate((float) -960f, (float) -960f, (float) 0f);
                if ((this.terrainmap != null) && this.checkBox1.Checked)
                {
                    this.terrainmap.Draw();
                }
                if (this.checkBox2.Checked)
                {
                    for (int i = 0; i < this.Objects.Count; i++)
                    {
                        GL.PushMatrix();
                        GL.Translate(this.Objects[i].X, this.Objects[i].Z, this.Objects[i].Y);
                        GL.Rotate((double) (this.Objects[i].Theta * 57.295779513082323), (double) 0.0, (double) 0.0, (double) 1.0);
                        if (this.checkBox4.Checked && this.Objects[i].DistFade)
                        {
                            GL.Color3((byte) 50, (byte) 0xff, (byte) 50);
                        }
                        if (this.checkBox5.Checked && (this.Objects[i].Unknown2 == "0100"))
                        {
                            GL.Color3((byte) 50, (byte) 50, (byte) 0xff);
                        }
                        if (i == this.selectedObj)
                        {
                            GL.Color3((byte) 0xff, (byte) 100, (byte) 100);
                        }
                        this.Objects[i].Draw();
                        if (i == this.selectedObj)
                        {
                            GL.Color3((byte) 0xff, (byte) 0xff, (byte) 0xff);
                            this.Objects[i].drawBoundingBox();
                        }
                        GL.PopMatrix();
                        if (this.checkBox3.Checked && (i == this.selectedObj))
                        {
                            this.Objects[i].drawGroups();
                        }
                        GL.Color3((byte) 0xff, (byte) 0xff, (byte) 0xff);
                    }
                }
                if (this.checkBox3.Checked)
                {
                    if (this.rotHor != 0)
                    {
                        this.rotHor = 0;
                    }
                    if (this.rotVert != 0)
                    {
                        this.rotVert = 0;
                    }
                    GL.Color3((byte) 0xff, (byte) 0, (byte) 0xff);
                    int num2 = 6;
                    int num3 = 0x18;
                    for (int j = 0; j < num3; j++)
                    {
                        for (int k = 0; k < num2; k++)
                        {
                            GL.Begin(BeginMode.LineStrip);
                            GL.Vertex3(j * (0x780 / num3), k * (0x780 / num2), 500);
                            GL.Vertex3((j + 1) * (0x780 / num3), k * (0x780 / num2), 500);
                            GL.Vertex3((j + 1) * (0x780 / num3), (k + 1) * (0x780 / num2), 500);
                            GL.Vertex3(j * (0x780 / num3), (k + 1) * (0x780 / num2), 500);
                            GL.End();
                        }
                    }
                    GL.Color3((byte) 0xff, (byte) 0xff, (byte) 0xff);
                }
                GL.PopMatrix();
            }
            this.glControl1.SwapBuffers();
        }

        private void saveMap(string path)
        {
            List<int>[] listArray = new List<int>[0x90];
            for (int i = 0; i < this.Objects.Count; i++)
            {
                if (this.Objects[i].groups == null)
                {
                    this.Objects[i].calcGroups();
                }
                else if (this.Objects[i].groups.Count == 0)
                {
                    this.Objects[i].calcGroups();
                }
                for (int k = 0; k < this.Objects[i].groups.Count; k++)
                {
                    if (listArray[this.Objects[i].groups[k]] == null)
                    {
                        listArray[this.Objects[i].groups[k]] = new List<int>();
                    }
                    listArray[this.Objects[i].groups[k]].Add(i);
                }
            }
            BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create));
            writer.Write("JMXVMAPO1001".ToCharArray());
            for (int j = 0; j < 0x90; j++)
            {
                if (listArray[j] == null)
                {
                    writer.Write((short) 0);
                }
                else
                {
                    writer.Write((short) listArray[j].Count);
                    for (int m = 0; m < listArray[j].Count; m++)
                    {
                        MapObject obj2 = this.Objects[listArray[j][m]];
                        writer.Write(obj2.nameI);
                        float x = obj2.X;
                        if (obj2.X < 0f)
                        {
                            x += 1920f;
                        }
                        if (obj2.X > 1920f)
                        {
                            x -= 1920f;
                        }
                        writer.Write(x);
                        writer.Write(obj2.Y);
                        float z = obj2.Z;
                        if (obj2.Z < 0f)
                        {
                            z += 1920f;
                        }
                        if (obj2.Z > 1920f)
                        {
                            z -= 1920f;
                        }
                        writer.Write(z);
                        if (!obj2.DistFade)
                        {
                            writer.Write(new byte[] { 0xff, 0xff });
                        }
                        else
                        {
                            writer.Write((short) 0);
                        }
                        writer.Write(obj2.Theta);
                        writer.Write(obj2.ID);
                        if (obj2.Unknown1 == "0100")
                        {
                            writer.Write((short) 1);
                        }
                        else
                        {
                            writer.Write((short) 1);
                        }
                        writer.Write((byte) (this.OX + Math.Floor((double) (obj2.X / 1920f))));
                        writer.Write((byte) (this.OY + Math.Floor((double) (obj2.Z / 1920f))));
                    }
                }
            }
            writer.Close();
            MessageBox.Show("File saved");
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            this.Objects[this.selectedObj].setRotation(((float) this.trackBar1.Value) / 100f);
            this.textBox1.Text = (((float) this.trackBar1.Value) / 100f).ToString();
        }
    }
}

