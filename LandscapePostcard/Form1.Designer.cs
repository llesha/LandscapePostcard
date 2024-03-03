namespace LandscapePostcard
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bCreatePlant = new System.Windows.Forms.Button();
            this.bCreateLandscape = new System.Windows.Forms.Button();
            this.bCreatePerspective = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1033, 671);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(895, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // bCreatePlant
            // 
            this.bCreatePlant.Location = new System.Drawing.Point(718, 12);
            this.bCreatePlant.Name = "bCreatePlant";
            this.bCreatePlant.Size = new System.Drawing.Size(100, 42);
            this.bCreatePlant.TabIndex = 2;
            this.bCreatePlant.Text = "Create Plant";
            this.bCreatePlant.UseVisualStyleBackColor = true;
            this.bCreatePlant.Click += new System.EventHandler(this.bCreatePlant_Click);
            // 
            // bCreateLandscape
            // 
            this.bCreateLandscape.Location = new System.Drawing.Point(824, 12);
            this.bCreateLandscape.Name = "bCreateLandscape";
            this.bCreateLandscape.Size = new System.Drawing.Size(88, 42);
            this.bCreateLandscape.TabIndex = 3;
            this.bCreateLandscape.Text = "Create Landscape";
            this.bCreateLandscape.UseVisualStyleBackColor = true;
            this.bCreateLandscape.Click += new System.EventHandler(this.bCreateLandscape_Click);
            // 
            // bCreatePerspective
            // 
            this.bCreatePerspective.Location = new System.Drawing.Point(918, 12);
            this.bCreatePerspective.Name = "bCreatePerspective";
            this.bCreatePerspective.Size = new System.Drawing.Size(103, 42);
            this.bCreatePerspective.TabIndex = 4;
            this.bCreatePerspective.Text = "Create Perspective";
            this.bCreatePerspective.UseVisualStyleBackColor = true;
            this.bCreatePerspective.Click += new System.EventHandler(this.bCreatePerspective_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1033, 671);
            this.Controls.Add(this.bCreatePerspective);
            this.Controls.Add(this.bCreateLandscape);
            this.Controls.Add(this.bCreatePlant);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bCreatePlant;
        private System.Windows.Forms.Button bCreateLandscape;
        private System.Windows.Forms.Button bCreatePerspective;
    }
}

