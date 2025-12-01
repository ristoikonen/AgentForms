namespace AgentForms
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            listViewProcesses = new ListView();
            colMemory = new ColumnHeader();
            colName = new ColumnHeader();
            colPid = new ColumnHeader();
            colThreads = new ColumnHeader();
            colStartTime = new ColumnHeader();
            colResponding = new ColumnHeader();
            btnRefresh = new Button();
            btnEndProcess = new Button();
            btnRunElevated = new Button();
            timerUpdate = new System.Windows.Forms.Timer(components);
            lblStatus = new Label();
            txtFilter = new TextBox();
            lblFilter = new Label();
            SuspendLayout();
            // 
            // listViewProcesses
            // 
            listViewProcesses.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listViewProcesses.BackColor = SystemColors.Window;
            listViewProcesses.Columns.AddRange(new ColumnHeader[] { colMemory, colName, colPid, colThreads, colStartTime, colResponding });
            listViewProcesses.FullRowSelect = true;
            listViewProcesses.GridLines = true;
            listViewProcesses.Location = new Point(12, 41);
            listViewProcesses.Name = "listViewProcesses";
            listViewProcesses.Size = new Size(760, 358);
            listViewProcesses.TabIndex = 0;
            listViewProcesses.UseCompatibleStateImageBehavior = false;
            listViewProcesses.View = View.Details;
            listViewProcesses.ColumnClick += listViewProcesses_ColumnClick;
            listViewProcesses.DoubleClick += listViewProcesses_DoubleClick;
            // 
            // colMemory
            // 
            colMemory.Text = "Memory (MB)";
            colMemory.TextAlign = HorizontalAlignment.Right;
            colMemory.Width = 100;
            // 
            // colName
            // 
            colName.Text = "Process";
            colName.Width = 240;
            // 
            // colPid
            // 
            colPid.Text = "PID";
            colPid.Width = 70;
            // 
            // colThreads
            // 
            colThreads.Text = "Threads";
            colThreads.Width = 70;
            // 
            // colStartTime
            // 
            colStartTime.Text = "Start Time";
            colStartTime.Width = 160;
            // 
            // colResponding
            // 
            colResponding.Text = "Responding";
            colResponding.Width = 80;
            // 
            // btnRefresh
            // 
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.Location = new Point(778, 38);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(90, 26);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnEndProcess
            // 
            btnEndProcess.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnEndProcess.Location = new Point(778, 70);
            btnEndProcess.Name = "btnEndProcess";
            btnEndProcess.Size = new Size(90, 26);
            btnEndProcess.TabIndex = 2;
            btnEndProcess.Text = "End Process";
            btnEndProcess.UseVisualStyleBackColor = true;
            btnEndProcess.Click += btnEndProcess_Click;
            // 
            // btnRunElevated
            // 
            btnRunElevated.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRunElevated.Location = new Point(778, 102);
            btnRunElevated.Name = "btnRunElevated";
            btnRunElevated.Size = new Size(90, 26);
            btnRunElevated.TabIndex = 6;
            btnRunElevated.Text = "Run Elevated";
            btnRunElevated.UseVisualStyleBackColor = true;
            btnRunElevated.Click += btnRunElevated_Click;
            // 
            // timerUpdate
            // 
            timerUpdate.Interval = 2000;
            timerUpdate.Tick += timerUpdate_Tick;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblStatus.Location = new Point(12, 402);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(760, 23);
            lblStatus.TabIndex = 3;
            lblStatus.Text = "Status: Ready";
            // 
            // txtFilter
            // 
            txtFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFilter.Location = new Point(56, 12);
            txtFilter.Name = "txtFilter";
            txtFilter.Size = new Size(716, 23);
            txtFilter.TabIndex = 4;
            txtFilter.KeyDown += txtFilter_KeyDown;
            // 
            // lblFilter
            // 
            lblFilter.AutoSize = true;
            lblFilter.Location = new Point(12, 15);
            lblFilter.Name = "lblFilter";
            lblFilter.Size = new Size(36, 15);
            lblFilter.TabIndex = 5;
            lblFilter.Text = "Filter:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(880, 434);
            Controls.Add(lblFilter);
            Controls.Add(txtFilter);
            Controls.Add(lblStatus);
            Controls.Add(btnRunElevated);
            Controls.Add(btnEndProcess);
            Controls.Add(btnRefresh);
            Controls.Add(listViewProcesses);
            MinimumSize = new Size(640, 300);
            Name = "Form1";
            Text = "Processes";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListView listViewProcesses;
        private System.Windows.Forms.ColumnHeader colMemory;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colPid;
        private System.Windows.Forms.ColumnHeader colThreads;
        private System.Windows.Forms.ColumnHeader colStartTime;
        private System.Windows.Forms.ColumnHeader colResponding;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnEndProcess;
        private System.Windows.Forms.Button btnRunElevated;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label lblFilter;
    }
}
