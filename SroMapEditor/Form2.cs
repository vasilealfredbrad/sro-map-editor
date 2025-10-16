namespace SroMapEditor
{
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    public class Form2 : Form
    {
        private Button button1;
        private Button button2;
        private Button button3;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private CheckBox checkBox3;
        private CheckBox checkBox4;
        private IContainer components;
        public GLControl glControl1;
        private GroupBox groupBox1;
        private Label label1;
        private ListBox listBox1;
        private ObjectNames ONames;
        private float rotation;
        private int selectedI;
        public int SelectedItem = -1;
        private List<int> texIDs = new List<int>();
        private List<string> texNames = new List<string>();
        private TextBox textBox1;
        private TrackBar trackBar1;
        private TreeView treeView1;
        private MapObject viewObj;

        public Form2(ObjectNames objNames, int selectedIndex)
        {
            this.InitializeComponent();
            this.ONames = objNames;
            this.selectedI = selectedIndex;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.treeView1.Visible)
            {
                this.SelectedItem = (int) this.treeView1.SelectedNode.Tag;
            }
            else
            {
                this.SelectedItem = int.Parse(this.listBox1.SelectedItem.ToString().Split(new char[] { ' ' })[0]);
            }
            base.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.SelectedItem = -1;
            base.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";
            this.setSearchResults();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.setSearchResults();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            this.setSearchResults();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            this.setSearchResults();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            this.setSearchResults();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.treeView1.Nodes.Add(new TreeNode("Buildings"));
            this.treeView1.Nodes.Add(new TreeNode("Artifacts"));
            this.treeView1.Nodes.Add(new TreeNode("Nature"));
            this.treeView1.Nodes.Add(new TreeNode("Other"));
            TreeNode node = new TreeNode();
            for (int i = 0; i < this.ONames.names.Length; i++)
            {
                string str = this.ONames.names[i].Substring(this.ONames.names[i].IndexOf('\\') + 1);
                TreeNode node2 = new TreeNode(str.Substring(str.IndexOf('\\') + 1)) {
                    Tag = i
                };
                if (this.ONames.names[i].Contains(@"\bldg\"))
                {
                    this.treeView1.Nodes[0].Nodes.Add(node2);
                }
                else if (this.ONames.names[i].Contains(@"\artifact\"))
                {
                    this.treeView1.Nodes[1].Nodes.Add(node2);
                }
                else if (this.ONames.names[i].Contains(@"\nature\"))
                {
                    this.treeView1.Nodes[2].Nodes.Add(node2);
                }
                else
                {
                    this.treeView1.Nodes[3].Nodes.Add(node2);
                }
                if (i == this.selectedI)
                {
                    node = node2;
                }
            }
            this.treeView1.HideSelection = false;
            this.treeView1.SelectedNode = node;
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.Black);
            int width = this.glControl1.Width;
            int height = this.glControl1.Height;
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-100.0, 100.0, -100.0, 100.0, -1000.0, 1000.0);
            GL.Viewport(0, 0, width, height);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
        }

        private void InitializeComponent()
        {
            this.button1 = new Button();
            this.button2 = new Button();
            this.treeView1 = new TreeView();
//            this.glControl1 = new GLControl();
            this.trackBar1 = new TrackBar();
            this.label1 = new Label();
            this.groupBox1 = new GroupBox();
            this.checkBox4 = new CheckBox();
            this.checkBox3 = new CheckBox();
            this.checkBox2 = new CheckBox();
            this.checkBox1 = new CheckBox();
            this.textBox1 = new TextBox();
            this.listBox1 = new ListBox();
            this.button3 = new Button();
            this.trackBar1.BeginInit();
            this.groupBox1.SuspendLayout();
            base.SuspendLayout();
            this.button1.Location = new Point(0x211, 0x1dc);
            this.button1.Name = "button1";
            this.button1.Size = new Size(0x71, 40);
            this.button1.TabIndex = 2;
            this.button1.Text = "Select";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            this.button2.Location = new Point(0x292, 0x1dc);
            this.button2.Name = "button2";
            this.button2.Size = new Size(0x71, 40);
            this.button2.TabIndex = 3;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new EventHandler(this.button2_Click);
            this.treeView1.Location = new Point(12, 12);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new Size(0x1c1, 0x1f8);
            this.treeView1.TabIndex = 4;
            this.treeView1.AfterSelect += new TreeViewEventHandler(this.treeView1_AfterSelect);
            this.glControl1.BackColor = Color.Black;
            this.glControl1.Location = new Point(470, 130);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new Size(0x13c, 0x121);
            this.glControl1.TabIndex = 5;
            this.glControl1.VSync = false;
            this.glControl1.Load += new EventHandler(this.glControl1_Load);
            this.trackBar1.Location = new Point(470, 0x1a9);
            this.trackBar1.Maximum = 0x274;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new Size(0x13c, 0x2d);
            this.trackBar1.TabIndex = 6;
            this.trackBar1.Scroll += new EventHandler(this.trackBar1_Scroll);
            this.label1.AutoSize = true;
            this.label1.Location = new Point(0x1d3, 0x72);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x21, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "None";
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.checkBox4);
            this.groupBox1.Controls.Add(this.checkBox3);
            this.groupBox1.Controls.Add(this.checkBox2);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Location = new Point(470, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(0x13c, 100);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search";
            this.checkBox4.AutoSize = true;
            this.checkBox4.Checked = true;
            this.checkBox4.CheckState = CheckState.Checked;
            this.checkBox4.Location = new Point(0xe4, 0x2e);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new Size(0x34, 0x11);
            this.checkBox4.TabIndex = 4;
            this.checkBox4.Text = "Other";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new EventHandler(this.checkBox4_CheckedChanged);
            this.checkBox3.AutoSize = true;
            this.checkBox3.Checked = true;
            this.checkBox3.CheckState = CheckState.Checked;
            this.checkBox3.Location = new Point(0xa3, 0x2e);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new Size(0x3a, 0x11);
            this.checkBox3.TabIndex = 3;
            this.checkBox3.Text = "Nature";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new EventHandler(this.checkBox3_CheckedChanged);
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = CheckState.Checked;
            this.checkBox2.Location = new Point(0x56, 0x2d);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new Size(0x40, 0x11);
            this.checkBox2.TabIndex = 2;
            this.checkBox2.Text = "Artifacts";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new EventHandler(this.checkBox2_CheckedChanged);
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = CheckState.Checked;
            this.checkBox1.Location = new Point(6, 0x2d);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new Size(0x44, 0x11);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "Buildings";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new EventHandler(this.checkBox1_CheckedChanged);
            this.textBox1.Location = new Point(6, 0x13);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Size(0x127, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new EventHandler(this.textBox1_TextChanged);
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new Size(0x1c1, 0x1f2);
            this.listBox1.TabIndex = 10;
            this.listBox1.Visible = false;
            this.listBox1.SelectedIndexChanged += new EventHandler(this.listBox1_SelectedIndexChanged);
            this.button3.Location = new Point(0xe4, 0x45);
            this.button3.Name = "button3";
            this.button3.Size = new Size(0x4b, 0x17);
            this.button3.TabIndex = 5;
            this.button3.Text = "Clear";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new EventHandler(this.button3_Click);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
//            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x31e, 0x210);
            base.Controls.Add(this.listBox1);
            base.Controls.Add(this.groupBox1);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.trackBar1);
            base.Controls.Add(this.glControl1);
            base.Controls.Add(this.treeView1);
            base.Controls.Add(this.button2);
            base.Controls.Add(this.button1);
            base.Name = "Form2";
            this.Text = "Choose an item";
            base.Load += new EventHandler(this.Form2_Load);
            this.trackBar1.EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.viewObj = new MapObject();
            List<MeshTexture[]> list = new List<MeshTexture[]>();
            this.viewObj.LoadFiles(@"Data\", this.listBox1.SelectedItem.ToString().Split(new char[] { ' ' })[2]);
            list.Add(this.viewObj.readMaterial());
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
            this.viewObj.FindTex(this.texNames, this.texIDs);
            this.label1.Text = this.listBox1.SelectedItem.ToString().Split(new char[] { ' ' })[2];
            GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(this.viewObj.boundingBoxp1[0] * 1.2, this.viewObj.boundingBoxp2[0] * 1.2, (this.viewObj.boundingBoxp1[1] + 10f) * 1.2, this.viewObj.boundingBoxp2[1] * 1.2, -300.0, 300.0);
        }

        public void RenderGL()
        {
            if (base.Visible)
            {
                GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Modelview);
                GL.LoadIdentity();
                if (!this.glControl1.Context.IsCurrent)
                {
                    this.glControl1.MakeCurrent();
                }
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.PushMatrix();
                GL.Rotate((float) -90f, (float) 1f, (float) 0f, (float) 0f);
                GL.Rotate((double) (this.rotation * 57.295779513082323), (double) 0.0, (double) 0.0, (double) 1.0);
                if (this.viewObj != null)
                {
                    this.viewObj.Draw();
                }
                GL.End();
                GL.PopMatrix();
                this.glControl1.SwapBuffers();
            }
        }

        private void setSearchResults()
        {
            if (this.textBox1.Text != "")
            {
                this.listBox1.Visible = true;
                this.treeView1.Visible = false;
                List<int> source = new List<int>();
                for (int i = 0; i < this.ONames.names.Count<string>(); i++)
                {
                    if (this.ONames.names[i].Contains(this.textBox1.Text))
                    {
                        if (this.ONames.names[i].Contains(@"\bldg\") && this.checkBox1.Checked)
                        {
                            source.Add(i);
                        }
                        else if (this.ONames.names[i].Contains(@"\artifact\") && this.checkBox2.Checked)
                        {
                            source.Add(i);
                        }
                        else if (this.ONames.names[i].Contains(@"\nature\") && this.checkBox3.Checked)
                        {
                            source.Add(i);
                        }
                        else if ((!this.ONames.names[i].Contains(@"\bldg\") && !this.ONames.names[i].Contains(@"\artifact\")) && (!this.ONames.names[i].Contains(@"\nature\") && this.checkBox4.Checked))
                        {
                            source.Add(i);
                        }
                    }
                }
                this.listBox1.Items.Clear();
                if (source.Count > 0)
                {
                    for (int j = 0; j < source.Count<int>(); j++)
                    {
                        this.listBox1.Items.Add(source[j] + " - " + this.ONames.names[source[j]]);
                    }
                }
                else
                {
                    this.listBox1.Items.Add("No objects matching your search were found.");
                }
            }
            else
            {
                this.listBox1.Visible = false;
                this.treeView1.Visible = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.setSearchResults();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            this.rotation = ((float) this.trackBar1.Value) / 100f;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Level == 1)
            {
                this.viewObj = new MapObject();
                List<MeshTexture[]> list = new List<MeshTexture[]>();
                this.viewObj.LoadFiles(@"Data\", this.ONames.names[(int) e.Node.Tag]);
                list.Add(this.viewObj.readMaterial());
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
                this.viewObj.FindTex(this.texNames, this.texIDs);
                this.label1.Text = this.ONames.names[(int) e.Node.Tag];
                GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(this.viewObj.boundingBoxp1[0] * 1.2, this.viewObj.boundingBoxp2[0] * 1.2, (this.viewObj.boundingBoxp1[1] + 10f) * 1.2, this.viewObj.boundingBoxp2[1] * 1.2, -300.0, 300.0);
            }
        }
    }
}

