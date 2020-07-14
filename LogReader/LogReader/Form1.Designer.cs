namespace LogReader
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.SearchTextBox = new System.Windows.Forms.TextBox();
            this.SearchButton = new System.Windows.Forms.Button();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.BodyTextBox = new System.Windows.Forms.TextBox();
            this.FromDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.ToDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.DirectoryTextBox = new System.Windows.Forms.TextBox();
            this.TitleListView = new System.Windows.Forms.ListView();
            this.DateColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TitleColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DirectoryColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.BodyColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ReloadButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SearchTextBox
            // 
            this.SearchTextBox.Location = new System.Drawing.Point(339, 33);
            this.SearchTextBox.Multiline = true;
            this.SearchTextBox.Name = "SearchTextBox";
            this.SearchTextBox.Size = new System.Drawing.Size(433, 142);
            this.SearchTextBox.TabIndex = 3;
            this.SearchTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchTextBox_KeyDown);
            // 
            // SearchButton
            // 
            this.SearchButton.Location = new System.Drawing.Point(651, 5);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(60, 21);
            this.SearchButton.TabIndex = 4;
            this.SearchButton.Text = "Search";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(306, 5);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(27, 21);
            this.BrowseButton.TabIndex = 5;
            this.BrowseButton.Text = "..";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // BodyTextBox
            // 
            this.BodyTextBox.Location = new System.Drawing.Point(12, 33);
            this.BodyTextBox.Multiline = true;
            this.BodyTextBox.Name = "BodyTextBox";
            this.BodyTextBox.ReadOnly = true;
            this.BodyTextBox.Size = new System.Drawing.Size(321, 516);
            this.BodyTextBox.TabIndex = 7;
            // 
            // FromDateTimePicker
            // 
            this.FromDateTimePicker.Checked = false;
            this.FromDateTimePicker.CustomFormat = "yyyy-MM-dd hh:mm:ss";
            this.FromDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.FromDateTimePicker.Location = new System.Drawing.Point(339, 5);
            this.FromDateTimePicker.Name = "FromDateTimePicker";
            this.FromDateTimePicker.ShowCheckBox = true;
            this.FromDateTimePicker.ShowUpDown = true;
            this.FromDateTimePicker.Size = new System.Drawing.Size(150, 21);
            this.FromDateTimePicker.TabIndex = 8;
            // 
            // ToDateTimePicker
            // 
            this.ToDateTimePicker.Checked = false;
            this.ToDateTimePicker.CustomFormat = "yyyy-MM-dd hh:mm:ss";
            this.ToDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.ToDateTimePicker.Location = new System.Drawing.Point(495, 5);
            this.ToDateTimePicker.Name = "ToDateTimePicker";
            this.ToDateTimePicker.ShowCheckBox = true;
            this.ToDateTimePicker.ShowUpDown = true;
            this.ToDateTimePicker.Size = new System.Drawing.Size(150, 21);
            this.ToDateTimePicker.TabIndex = 9;
            this.ToDateTimePicker.Tag = "";
            // 
            // DirectoryTextBox
            // 
            this.DirectoryTextBox.Location = new System.Drawing.Point(12, 6);
            this.DirectoryTextBox.Name = "DirectoryTextBox";
            this.DirectoryTextBox.ReadOnly = true;
            this.DirectoryTextBox.Size = new System.Drawing.Size(288, 21);
            this.DirectoryTextBox.TabIndex = 10;
            // 
            // TitleListView
            // 
            this.TitleListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.TitleColumn,
            this.DateColumn,
            this.DirectoryColumn,
            this.BodyColumn});
            this.TitleListView.FullRowSelect = true;
            this.TitleListView.HideSelection = false;
            this.TitleListView.Location = new System.Drawing.Point(339, 181);
            this.TitleListView.MultiSelect = false;
            this.TitleListView.Name = "TitleListView";
            this.TitleListView.Size = new System.Drawing.Size(433, 368);
            this.TitleListView.TabIndex = 11;
            this.TitleListView.UseCompatibleStateImageBehavior = false;
            this.TitleListView.View = System.Windows.Forms.View.Details;
            this.TitleListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.TitleListView_ColumnClick);
            this.TitleListView.SelectedIndexChanged += new System.EventHandler(this.TitleLIstView_SelectedIndexChanged);
            this.TitleListView.DoubleClick += new System.EventHandler(this.TitleListView_DoubleClick);
            // 
            // DateColumn
            // 
            this.DateColumn.Text = "Timestamp";
            this.DateColumn.Width = 120;
            // 
            // TitleColumn
            // 
            this.TitleColumn.Text = "Title";
            this.TitleColumn.Width = 240;
            // 
            // DirectoryColumn
            // 
            this.DirectoryColumn.Width = 0;
            // 
            // BodyColumn
            // 
            this.BodyColumn.Width = 0;
            // 
            // ReloadButton
            // 
            this.ReloadButton.Location = new System.Drawing.Point(712, 5);
            this.ReloadButton.Name = "ReloadButton";
            this.ReloadButton.Size = new System.Drawing.Size(60, 21);
            this.ReloadButton.TabIndex = 12;
            this.ReloadButton.Text = "Reload";
            this.ReloadButton.UseVisualStyleBackColor = true;
            this.ReloadButton.Click += new System.EventHandler(this.ReloadButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.ReloadButton);
            this.Controls.Add(this.TitleListView);
            this.Controls.Add(this.DirectoryTextBox);
            this.Controls.Add(this.ToDateTimePicker);
            this.Controls.Add(this.FromDateTimePicker);
            this.Controls.Add(this.BodyTextBox);
            this.Controls.Add(this.BrowseButton);
            this.Controls.Add(this.SearchButton);
            this.Controls.Add(this.SearchTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "LogReader";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox SearchTextBox;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.TextBox BodyTextBox;
        private System.Windows.Forms.DateTimePicker ToDateTimePicker;
        private System.Windows.Forms.DateTimePicker FromDateTimePicker;
        private System.Windows.Forms.TextBox DirectoryTextBox;
        private System.Windows.Forms.ListView TitleListView;
        private System.Windows.Forms.ColumnHeader DateColumn;
        private System.Windows.Forms.ColumnHeader TitleColumn;
        private System.Windows.Forms.ColumnHeader DirectoryColumn;
        private System.Windows.Forms.ColumnHeader BodyColumn;
        private System.Windows.Forms.Button ReloadButton;
    }
}

