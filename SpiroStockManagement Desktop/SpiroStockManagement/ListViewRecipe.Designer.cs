namespace SpiroStockManagement
{
    partial class ListViewRecipe
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label_Name = new System.Windows.Forms.Label();
            this.label_Description = new System.Windows.Forms.Label();
            this.starRatingControl1 = new RatingControls.StarRatingControl();
            this.pictureBox_photo = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_photo)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox_photo, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(350, 126);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.label_Name, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label_Description, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.starRatingControl1, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(129, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(218, 120);
            this.tableLayoutPanel2.TabIndex = 2;
            this.tableLayoutPanel2.Click += new System.EventHandler(this.pictureBox_photo_Click);
            this.tableLayoutPanel2.DoubleClick += new System.EventHandler(this.pictureBox_photo_DoubleClick);
            this.tableLayoutPanel2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_photo_MouseClick);
            // 
            // label_Name
            // 
            this.label_Name.AutoSize = true;
            this.label_Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Name.Location = new System.Drawing.Point(3, 0);
            this.label_Name.Name = "label_Name";
            this.label_Name.Size = new System.Drawing.Size(64, 25);
            this.label_Name.TabIndex = 1;
            this.label_Name.Text = "label1";
            this.label_Name.Click += new System.EventHandler(this.pictureBox_photo_Click);
            this.label_Name.DoubleClick += new System.EventHandler(this.pictureBox_photo_DoubleClick);
            this.label_Name.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_photo_MouseClick);
            // 
            // label_Description
            // 
            this.label_Description.AutoSize = true;
            this.label_Description.Location = new System.Drawing.Point(7, 32);
            this.label_Description.Margin = new System.Windows.Forms.Padding(7);
            this.label_Description.Name = "label_Description";
            this.label_Description.Size = new System.Drawing.Size(88, 13);
            this.label_Description.TabIndex = 2;
            this.label_Description.Text = "label_Description";
            this.label_Description.Click += new System.EventHandler(this.pictureBox_photo_Click);
            this.label_Description.DoubleClick += new System.EventHandler(this.pictureBox_photo_DoubleClick);
            this.label_Description.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_photo_MouseClick);
            // 
            // starRatingControl1
            // 
            this.starRatingControl1.BottomMargin = 2;
            this.starRatingControl1.Enabled = false;
            this.starRatingControl1.HoverColor = System.Drawing.Color.Yellow;
            this.starRatingControl1.LeftMargin = 2;
            this.starRatingControl1.Location = new System.Drawing.Point(3, 99);
            this.starRatingControl1.Name = "starRatingControl1";
            this.starRatingControl1.OutlineColor = System.Drawing.Color.DarkGray;
            this.starRatingControl1.OutlineThickness = 1;
            this.starRatingControl1.RightMargin = 2;
            this.starRatingControl1.SelectedColor = System.Drawing.Color.RoyalBlue;
            this.starRatingControl1.SelectedStar = 0;
            this.starRatingControl1.Size = new System.Drawing.Size(120, 18);
            this.starRatingControl1.StarCount = 5;
            this.starRatingControl1.StarSpacing = 8;
            this.starRatingControl1.TabIndex = 3;
            this.starRatingControl1.Text = "starRatingControl1";
            this.starRatingControl1.TopMargin = 2;
            this.starRatingControl1.Click += new System.EventHandler(this.pictureBox_photo_Click);
            this.starRatingControl1.DoubleClick += new System.EventHandler(this.pictureBox_photo_DoubleClick);
            this.starRatingControl1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_photo_MouseClick);
            // 
            // pictureBox_photo
            // 
            this.pictureBox_photo.Location = new System.Drawing.Point(3, 3);
            this.pictureBox_photo.Name = "pictureBox_photo";
            this.pictureBox_photo.Size = new System.Drawing.Size(120, 120);
            this.pictureBox_photo.TabIndex = 3;
            this.pictureBox_photo.TabStop = false;
            this.pictureBox_photo.Click += new System.EventHandler(this.pictureBox_photo_Click);
            this.pictureBox_photo.DoubleClick += new System.EventHandler(this.pictureBox_photo_DoubleClick);
            this.pictureBox_photo.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_photo_MouseClick);
            // 
            // ListViewRecipe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ListViewRecipe";
            this.Size = new System.Drawing.Size(350, 126);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_photo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label_Name;
        private System.Windows.Forms.Label label_Description;
        private RatingControls.StarRatingControl starRatingControl1;
        private System.Windows.Forms.PictureBox pictureBox_photo;
    }
}
