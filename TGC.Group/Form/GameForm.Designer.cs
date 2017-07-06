namespace TGC.Group.Form
{
    partial class GameForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameForm));
            this.panel3D = new System.Windows.Forms.Panel();
            this.picture3 = new System.Windows.Forms.PictureBox();
            this.picture2 = new System.Windows.Forms.PictureBox();
            this.BtnSalir = new System.Windows.Forms.Button();
            this.BtnPersonaje = new System.Windows.Forms.Button();
            this.BtnComenzar = new System.Windows.Forms.Button();
            this.picture1 = new System.Windows.Forms.PictureBox();
            this.panel3D.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picture3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel3D
            // 
            this.panel3D.AllowDrop = true;
            this.panel3D.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel3D.BackgroundImage")));
            this.panel3D.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel3D.Controls.Add(this.picture3);
            this.panel3D.Controls.Add(this.picture2);
            this.panel3D.Controls.Add(this.BtnSalir);
            this.panel3D.Controls.Add(this.BtnPersonaje);
            this.panel3D.Controls.Add(this.BtnComenzar);
            this.panel3D.Controls.Add(this.picture1);
            this.panel3D.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3D.Location = new System.Drawing.Point(0, 0);
            this.panel3D.Name = "panel3D";
            this.panel3D.Size = new System.Drawing.Size(784, 561);
            this.panel3D.TabIndex = 0;
            // 
            // picture3
            // 
            this.picture3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picture3.BackgroundImage")));
            this.picture3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picture3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picture3.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.picture3.Location = new System.Drawing.Point(893, 188);
            this.picture3.Name = "picture3";
            this.picture3.Size = new System.Drawing.Size(250, 250);
            this.picture3.TabIndex = 5;
            this.picture3.TabStop = false;
            this.picture3.Visible = false;
            this.picture3.Click += new System.EventHandler(this.picture3_Click);
            // 
            // picture2
            // 
            this.picture2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picture2.BackgroundImage")));
            this.picture2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picture2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picture2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.picture2.Location = new System.Drawing.Point(512, 188);
            this.picture2.Name = "picture2";
            this.picture2.Size = new System.Drawing.Size(250, 250);
            this.picture2.TabIndex = 4;
            this.picture2.TabStop = false;
            this.picture2.Visible = false;
            this.picture2.Click += new System.EventHandler(this.picture2_Click);
            // 
            // BtnSalir
            // 
            this.BtnSalir.BackColor = System.Drawing.Color.PaleGreen;
            this.BtnSalir.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BtnSalir.BackgroundImage")));
            this.BtnSalir.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.BtnSalir.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnSalir.Location = new System.Drawing.Point(12, 323);
            this.BtnSalir.Name = "BtnSalir";
            this.BtnSalir.Size = new System.Drawing.Size(232, 71);
            this.BtnSalir.TabIndex = 2;
            this.BtnSalir.UseVisualStyleBackColor = false;
            this.BtnSalir.Visible = false;
            this.BtnSalir.Click += new System.EventHandler(this.BtnSalir_Click);
            this.BtnSalir.Enter += new System.EventHandler(this.BtnSalir_Enter);
            this.BtnSalir.Leave += new System.EventHandler(this.BtnSalir_Leave);
            this.BtnSalir.MouseLeave += new System.EventHandler(this.BtnSalir_MouseLeave);
            this.BtnSalir.MouseHover += new System.EventHandler(this.BtnSalir_MouseHover);
            // 
            // BtnPersonaje
            // 
            this.BtnPersonaje.BackColor = System.Drawing.Color.PaleGreen;
            this.BtnPersonaje.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BtnPersonaje.BackgroundImage")));
            this.BtnPersonaje.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BtnPersonaje.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnPersonaje.Location = new System.Drawing.Point(12, 173);
            this.BtnPersonaje.Name = "BtnPersonaje";
            this.BtnPersonaje.Size = new System.Drawing.Size(232, 71);
            this.BtnPersonaje.TabIndex = 1;
            this.BtnPersonaje.UseVisualStyleBackColor = false;
            this.BtnPersonaje.Visible = false;
            this.BtnPersonaje.Click += new System.EventHandler(this.BtnPersonaje_Click);
            this.BtnPersonaje.Enter += new System.EventHandler(this.BtnPersonaje_Enter);
            this.BtnPersonaje.Leave += new System.EventHandler(this.BtnPersonaje_Leave);
            this.BtnPersonaje.MouseLeave += new System.EventHandler(this.BtnPersonaje_MouseLeave);
            this.BtnPersonaje.MouseHover += new System.EventHandler(this.BtnPersonaje_MouseHover);
            // 
            // BtnComenzar
            // 
            this.BtnComenzar.BackColor = System.Drawing.Color.LawnGreen;
            this.BtnComenzar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BtnComenzar.BackgroundImage")));
            this.BtnComenzar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BtnComenzar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnComenzar.Location = new System.Drawing.Point(12, 12);
            this.BtnComenzar.Name = "BtnComenzar";
            this.BtnComenzar.Size = new System.Drawing.Size(232, 71);
            this.BtnComenzar.TabIndex = 0;
            this.BtnComenzar.UseVisualStyleBackColor = false;
            this.BtnComenzar.Visible = false;
            this.BtnComenzar.Click += new System.EventHandler(this.BtnComenzar_Click);
            this.BtnComenzar.Enter += new System.EventHandler(this.BtnComenzar_Enter);
            this.BtnComenzar.Leave += new System.EventHandler(this.BtnComenzar_Leave);
            this.BtnComenzar.MouseLeave += new System.EventHandler(this.BtnComenzar_MouseLeave);
            this.BtnComenzar.MouseHover += new System.EventHandler(this.BtnComenzar_MouseHover);
            // 
            // picture1
            // 
            this.picture1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picture1.BackgroundImage")));
            this.picture1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picture1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picture1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.picture1.Location = new System.Drawing.Point(131, 188);
            this.picture1.Name = "picture1";
            this.picture1.Size = new System.Drawing.Size(250, 250);
            this.picture1.TabIndex = 3;
            this.picture1.TabStop = false;
            this.picture1.Visible = false;
            this.picture1.Click += new System.EventHandler(this.picture1_Click);
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.panel3D);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "GameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Escuadron Suicida";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameForm_FormClosing);
            this.Load += new System.EventHandler(this.GameForm_Load);
            this.panel3D.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picture3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture1)).EndInit();
            this.ResumeLayout(false);

        }
      
        #endregion
        private System.Windows.Forms.Button BtnComenzar;
        private System.Windows.Forms.Button BtnSalir;
        private System.Windows.Forms.Button BtnPersonaje;
        internal System.Windows.Forms.Panel panel3D;
        private System.Windows.Forms.PictureBox picture3;
        private System.Windows.Forms.PictureBox picture2;
        private System.Windows.Forms.PictureBox picture1;
    }
}

