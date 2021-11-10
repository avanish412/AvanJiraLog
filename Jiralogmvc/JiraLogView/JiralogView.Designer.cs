namespace JiralogMVC.View
{
    partial class JiralogView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JiralogView));
            this.btnGenerateReport = new System.Windows.Forms.Button();
            this.lvReport = new System.Windows.Forms.ListView();
            this.btnExport = new System.Windows.Forms.Button();
            this.dtPicker = new System.Windows.Forms.DateTimePicker();
            this.pbLogprogress = new System.Windows.Forms.ProgressBar();
            this.lblMessage = new System.Windows.Forms.Label();
            this.txtUserid = new System.Windows.Forms.TextBox();
            this.txtPasswd = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblSelectdate = new System.Windows.Forms.Label();
            this.dtPickerEnd = new System.Windows.Forms.DateTimePicker();
            this.rdoDefectleakage = new System.Windows.Forms.RadioButton();
            this.lblResult1 = new System.Windows.Forms.Label();
            this.lblResult2 = new System.Windows.Forms.Label();
            this.rdoAssignedSTC = new System.Windows.Forms.RadioButton();
            this.cmbProjects = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.grpBoxTeam = new System.Windows.Forms.GroupBox();
            this.rdoDoseIQQAAutomation = new System.Windows.Forms.RadioButton();
            this.rdoDoseIQCommon = new System.Windows.Forms.RadioButton();
            this.rdoDoseIQQAManual = new System.Windows.Forms.RadioButton();
            this.rdoAllTeams = new System.Windows.Forms.RadioButton();
            this.rdoDoseIQDev = new System.Windows.Forms.RadioButton();
            this.btnMailtoDefaulters = new System.Windows.Forms.Button();
            this.cmbTeamMember = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.grpBoxReportType = new System.Windows.Forms.GroupBox();
            this.rdoComments = new System.Windows.Forms.RadioButton();
            this.rdoPerformance = new System.Windows.Forms.RadioButton();
            this.btnSendMailReport = new System.Windows.Forms.Button();
            this.lblResult3 = new System.Windows.Forms.Label();
            this.chkAssignedOnly = new System.Windows.Forms.CheckBox();
            this.btnPendingResult = new System.Windows.Forms.Button();
            this.pbWaiting = new System.Windows.Forms.PictureBox();
            this.btnDeleteRow = new System.Windows.Forms.Button();
            this.btnWatchIssue = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbRelease = new System.Windows.Forms.ComboBox();
            this.grpBoxTeam.SuspendLayout();
            this.grpBoxReportType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbWaiting)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGenerateReport
            // 
            this.btnGenerateReport.Location = new System.Drawing.Point(960, 641);
            this.btnGenerateReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerateReport.Name = "btnGenerateReport";
            this.btnGenerateReport.Size = new System.Drawing.Size(120, 57);
            this.btnGenerateReport.TabIndex = 0;
            this.btnGenerateReport.Text = "Generate Report";
            this.btnGenerateReport.UseVisualStyleBackColor = true;
            this.btnGenerateReport.Click += new System.EventHandler(this.btnGenerateReport_Click);
            // 
            // lvReport
            // 
            this.lvReport.FullRowSelect = true;
            this.lvReport.GridLines = true;
            this.lvReport.Location = new System.Drawing.Point(13, 102);
            this.lvReport.Margin = new System.Windows.Forms.Padding(4);
            this.lvReport.Name = "lvReport";
            this.lvReport.Size = new System.Drawing.Size(1548, 528);
            this.lvReport.TabIndex = 1;
            this.lvReport.UseCompatibleStateImageBehavior = false;
            this.lvReport.View = System.Windows.Forms.View.Details;
            this.lvReport.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvReport_ColumnClick);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(1096, 641);
            this.btnExport.Margin = new System.Windows.Forms.Padding(4);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(112, 57);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "Export to CSV";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // dtPicker
            // 
            this.dtPicker.Location = new System.Drawing.Point(116, 641);
            this.dtPicker.Margin = new System.Windows.Forms.Padding(4);
            this.dtPicker.Name = "dtPicker";
            this.dtPicker.Size = new System.Drawing.Size(265, 22);
            this.dtPicker.TabIndex = 3;
            // 
            // pbLogprogress
            // 
            this.pbLogprogress.Location = new System.Drawing.Point(373, 325);
            this.pbLogprogress.Margin = new System.Windows.Forms.Padding(4);
            this.pbLogprogress.Name = "pbLogprogress";
            this.pbLogprogress.Size = new System.Drawing.Size(863, 62);
            this.pbLogprogress.TabIndex = 4;
            this.pbLogprogress.Visible = false;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.ForeColor = System.Drawing.Color.Red;
            this.lblMessage.Location = new System.Drawing.Point(576, 75);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(337, 17);
            this.lblMessage.TabIndex = 6;
            this.lblMessage.Text = "Wait ... getting list of JIRA IDs to look into ....";
            this.lblMessage.Visible = false;
            // 
            // txtUserid
            // 
            this.txtUserid.Location = new System.Drawing.Point(106, 17);
            this.txtUserid.Margin = new System.Windows.Forms.Padding(4);
            this.txtUserid.Name = "txtUserid";
            this.txtUserid.Size = new System.Drawing.Size(156, 22);
            this.txtUserid.TabIndex = 7;
            // 
            // txtPasswd
            // 
            this.txtPasswd.Location = new System.Drawing.Point(107, 54);
            this.txtPasswd.Margin = new System.Windows.Forms.Padding(4);
            this.txtPasswd.Name = "txtPasswd";
            this.txtPasswd.PasswordChar = '*';
            this.txtPasswd.Size = new System.Drawing.Size(155, 22);
            this.txtPasswd.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "JIRA User ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 59);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "JIRA Password";
            // 
            // lblSelectdate
            // 
            this.lblSelectdate.AutoSize = true;
            this.lblSelectdate.Location = new System.Drawing.Point(16, 644);
            this.lblSelectdate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSelectdate.Name = "lblSelectdate";
            this.lblSelectdate.Size = new System.Drawing.Size(89, 17);
            this.lblSelectdate.TabIndex = 14;
            this.lblSelectdate.Text = "Select Date :";
            // 
            // dtPickerEnd
            // 
            this.dtPickerEnd.Location = new System.Drawing.Point(116, 673);
            this.dtPickerEnd.Margin = new System.Windows.Forms.Padding(4);
            this.dtPickerEnd.Name = "dtPickerEnd";
            this.dtPickerEnd.Size = new System.Drawing.Size(265, 22);
            this.dtPickerEnd.TabIndex = 16;
            // 
            // rdoDefectleakage
            // 
            this.rdoDefectleakage.AutoSize = true;
            this.rdoDefectleakage.Location = new System.Drawing.Point(17, 55);
            this.rdoDefectleakage.Margin = new System.Windows.Forms.Padding(4);
            this.rdoDefectleakage.Name = "rdoDefectleakage";
            this.rdoDefectleakage.Size = new System.Drawing.Size(129, 21);
            this.rdoDefectleakage.TabIndex = 18;
            this.rdoDefectleakage.TabStop = true;
            this.rdoDefectleakage.Text = "Defect Leakage";
            this.rdoDefectleakage.UseVisualStyleBackColor = true;
            this.rdoDefectleakage.CheckedChanged += new System.EventHandler(this.rdoReportType_CheckedChanged);
            // 
            // lblResult1
            // 
            this.lblResult1.AutoSize = true;
            this.lblResult1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResult1.Location = new System.Drawing.Point(396, 641);
            this.lblResult1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblResult1.Name = "lblResult1";
            this.lblResult1.Size = new System.Drawing.Size(139, 25);
            this.lblResult1.TabIndex = 19;
            this.lblResult1.Text = "Result Text 1";
            // 
            // lblResult2
            // 
            this.lblResult2.AutoSize = true;
            this.lblResult2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResult2.Location = new System.Drawing.Point(396, 673);
            this.lblResult2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblResult2.Name = "lblResult2";
            this.lblResult2.Size = new System.Drawing.Size(139, 25);
            this.lblResult2.TabIndex = 20;
            this.lblResult2.Text = "Result Text 2";
            // 
            // rdoAssignedSTC
            // 
            this.rdoAssignedSTC.AutoSize = true;
            this.rdoAssignedSTC.Location = new System.Drawing.Point(17, 26);
            this.rdoAssignedSTC.Margin = new System.Windows.Forms.Padding(4);
            this.rdoAssignedSTC.Name = "rdoAssignedSTC";
            this.rdoAssignedSTC.Size = new System.Drawing.Size(143, 21);
            this.rdoAssignedSTC.TabIndex = 21;
            this.rdoAssignedSTC.TabStop = true;
            this.rdoAssignedSTC.Text = "Assigned to Team";
            this.rdoAssignedSTC.UseVisualStyleBackColor = true;
            this.rdoAssignedSTC.CheckedChanged += new System.EventHandler(this.rdoReportType_CheckedChanged);
            // 
            // cmbProjects
            // 
            this.cmbProjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProjects.FormattingEnabled = true;
            this.cmbProjects.Location = new System.Drawing.Point(695, 10);
            this.cmbProjects.Margin = new System.Windows.Forms.Padding(4);
            this.cmbProjects.Name = "cmbProjects";
            this.cmbProjects.Size = new System.Drawing.Size(190, 24);
            this.cmbProjects.TabIndex = 25;
            this.cmbProjects.SelectedValueChanged += new System.EventHandler(this.cmbProjects_SelectedValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(553, 13);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 17);
            this.label3.TabIndex = 26;
            this.label3.Text = "Selected Project";
            // 
            // grpBoxTeam
            // 
            this.grpBoxTeam.Controls.Add(this.rdoDoseIQQAAutomation);
            this.grpBoxTeam.Controls.Add(this.rdoDoseIQCommon);
            this.grpBoxTeam.Controls.Add(this.rdoDoseIQQAManual);
            this.grpBoxTeam.Controls.Add(this.rdoAllTeams);
            this.grpBoxTeam.Controls.Add(this.rdoDoseIQDev);
            this.grpBoxTeam.Location = new System.Drawing.Point(270, 4);
            this.grpBoxTeam.Margin = new System.Windows.Forms.Padding(4);
            this.grpBoxTeam.Name = "grpBoxTeam";
            this.grpBoxTeam.Padding = new System.Windows.Forms.Padding(4);
            this.grpBoxTeam.Size = new System.Drawing.Size(275, 87);
            this.grpBoxTeam.TabIndex = 27;
            this.grpBoxTeam.TabStop = false;
            this.grpBoxTeam.Text = "Team";
            // 
            // rdoDoseIQQAAutomation
            // 
            this.rdoDoseIQQAAutomation.AutoSize = true;
            this.rdoDoseIQQAAutomation.Location = new System.Drawing.Point(127, 54);
            this.rdoDoseIQQAAutomation.Margin = new System.Windows.Forms.Padding(4);
            this.rdoDoseIQQAAutomation.Name = "rdoDoseIQQAAutomation";
            this.rdoDoseIQQAAutomation.Size = new System.Drawing.Size(124, 21);
            this.rdoDoseIQQAAutomation.TabIndex = 7;
            this.rdoDoseIQQAAutomation.TabStop = true;
            this.rdoDoseIQQAAutomation.Text = "QA Automation";
            this.rdoDoseIQQAAutomation.UseVisualStyleBackColor = true;
            this.rdoDoseIQQAAutomation.CheckedChanged += new System.EventHandler(this.TeamSelectionChanged);
            // 
            // rdoDoseIQCommon
            // 
            this.rdoDoseIQCommon.AutoSize = true;
            this.rdoDoseIQCommon.Location = new System.Drawing.Point(107, 24);
            this.rdoDoseIQCommon.Margin = new System.Windows.Forms.Padding(4);
            this.rdoDoseIQCommon.Name = "rdoDoseIQCommon";
            this.rdoDoseIQCommon.Size = new System.Drawing.Size(84, 21);
            this.rdoDoseIQCommon.TabIndex = 6;
            this.rdoDoseIQCommon.TabStop = true;
            this.rdoDoseIQCommon.Text = "Common";
            this.rdoDoseIQCommon.UseVisualStyleBackColor = true;
            this.rdoDoseIQCommon.CheckedChanged += new System.EventHandler(this.TeamSelectionChanged);
            // 
            // rdoDoseIQQAManual
            // 
            this.rdoDoseIQQAManual.AutoSize = true;
            this.rdoDoseIQQAManual.Location = new System.Drawing.Point(8, 55);
            this.rdoDoseIQQAManual.Margin = new System.Windows.Forms.Padding(4);
            this.rdoDoseIQQAManual.Name = "rdoDoseIQQAManual";
            this.rdoDoseIQQAManual.Size = new System.Drawing.Size(99, 21);
            this.rdoDoseIQQAManual.TabIndex = 5;
            this.rdoDoseIQQAManual.TabStop = true;
            this.rdoDoseIQQAManual.Text = "QA Manual";
            this.rdoDoseIQQAManual.UseVisualStyleBackColor = true;
            this.rdoDoseIQQAManual.CheckedChanged += new System.EventHandler(this.TeamSelectionChanged);
            // 
            // rdoAllTeams
            // 
            this.rdoAllTeams.AutoSize = true;
            this.rdoAllTeams.Location = new System.Drawing.Point(8, 23);
            this.rdoAllTeams.Margin = new System.Windows.Forms.Padding(4);
            this.rdoAllTeams.Name = "rdoAllTeams";
            this.rdoAllTeams.Size = new System.Drawing.Size(91, 21);
            this.rdoAllTeams.TabIndex = 4;
            this.rdoAllTeams.TabStop = true;
            this.rdoAllTeams.Text = "All Teams";
            this.rdoAllTeams.UseVisualStyleBackColor = true;
            this.rdoAllTeams.CheckedChanged += new System.EventHandler(this.TeamSelectionChanged);
            // 
            // rdoDoseIQDev
            // 
            this.rdoDoseIQDev.AutoSize = true;
            this.rdoDoseIQDev.Location = new System.Drawing.Point(207, 23);
            this.rdoDoseIQDev.Margin = new System.Windows.Forms.Padding(4);
            this.rdoDoseIQDev.Name = "rdoDoseIQDev";
            this.rdoDoseIQDev.Size = new System.Drawing.Size(54, 21);
            this.rdoDoseIQDev.TabIndex = 0;
            this.rdoDoseIQDev.TabStop = true;
            this.rdoDoseIQDev.Text = "Dev";
            this.rdoDoseIQDev.UseVisualStyleBackColor = true;
            this.rdoDoseIQDev.CheckedChanged += new System.EventHandler(this.TeamSelectionChanged);
            // 
            // btnMailtoDefaulters
            // 
            this.btnMailtoDefaulters.Location = new System.Drawing.Point(1356, 639);
            this.btnMailtoDefaulters.Margin = new System.Windows.Forms.Padding(4);
            this.btnMailtoDefaulters.Name = "btnMailtoDefaulters";
            this.btnMailtoDefaulters.Size = new System.Drawing.Size(207, 59);
            this.btnMailtoDefaulters.TabIndex = 28;
            this.btnMailtoDefaulters.Text = "Mail to Defaulters";
            this.btnMailtoDefaulters.UseVisualStyleBackColor = true;
            this.btnMailtoDefaulters.Click += new System.EventHandler(this.btnMailtoDefaulters_Click);
            // 
            // cmbTeamMember
            // 
            this.cmbTeamMember.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTeamMember.FormattingEnabled = true;
            this.cmbTeamMember.Items.AddRange(new object[] {
            "All Team Members"});
            this.cmbTeamMember.Location = new System.Drawing.Point(915, 37);
            this.cmbTeamMember.Margin = new System.Windows.Forms.Padding(4);
            this.cmbTeamMember.Name = "cmbTeamMember";
            this.cmbTeamMember.Size = new System.Drawing.Size(191, 24);
            this.cmbTeamMember.TabIndex = 29;
            this.cmbTeamMember.SelectedValueChanged += new System.EventHandler(this.cmbTeamMember_SelectedValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(957, 13);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 17);
            this.label4.TabIndex = 30;
            this.label4.Text = "Selected Member";
            // 
            // grpBoxReportType
            // 
            this.grpBoxReportType.Controls.Add(this.rdoComments);
            this.grpBoxReportType.Controls.Add(this.rdoPerformance);
            this.grpBoxReportType.Controls.Add(this.rdoAssignedSTC);
            this.grpBoxReportType.Controls.Add(this.rdoDefectleakage);
            this.grpBoxReportType.Location = new System.Drawing.Point(1125, 2);
            this.grpBoxReportType.Margin = new System.Windows.Forms.Padding(4);
            this.grpBoxReportType.Name = "grpBoxReportType";
            this.grpBoxReportType.Padding = new System.Windows.Forms.Padding(4);
            this.grpBoxReportType.Size = new System.Drawing.Size(302, 89);
            this.grpBoxReportType.TabIndex = 31;
            this.grpBoxReportType.TabStop = false;
            this.grpBoxReportType.Text = "Report Type";
            // 
            // rdoComments
            // 
            this.rdoComments.AutoSize = true;
            this.rdoComments.Location = new System.Drawing.Point(181, 52);
            this.rdoComments.Margin = new System.Windows.Forms.Padding(4);
            this.rdoComments.Name = "rdoComments";
            this.rdoComments.Size = new System.Drawing.Size(95, 21);
            this.rdoComments.TabIndex = 25;
            this.rdoComments.TabStop = true;
            this.rdoComments.Text = "Comments";
            this.rdoComments.UseVisualStyleBackColor = true;
            this.rdoComments.CheckedChanged += new System.EventHandler(this.rdoReportType_CheckedChanged);
            // 
            // rdoPerformance
            // 
            this.rdoPerformance.AutoSize = true;
            this.rdoPerformance.Location = new System.Drawing.Point(181, 26);
            this.rdoPerformance.Margin = new System.Windows.Forms.Padding(4);
            this.rdoPerformance.Name = "rdoPerformance";
            this.rdoPerformance.Size = new System.Drawing.Size(110, 21);
            this.rdoPerformance.TabIndex = 24;
            this.rdoPerformance.TabStop = true;
            this.rdoPerformance.Text = "Performance";
            this.rdoPerformance.UseVisualStyleBackColor = true;
            this.rdoPerformance.CheckedChanged += new System.EventHandler(this.rdoReportType_CheckedChanged);
            // 
            // btnSendMailReport
            // 
            this.btnSendMailReport.Location = new System.Drawing.Point(1225, 641);
            this.btnSendMailReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnSendMailReport.Name = "btnSendMailReport";
            this.btnSendMailReport.Size = new System.Drawing.Size(112, 57);
            this.btnSendMailReport.TabIndex = 32;
            this.btnSendMailReport.Text = "Send Report via Mail";
            this.btnSendMailReport.UseVisualStyleBackColor = true;
            this.btnSendMailReport.Click += new System.EventHandler(this.btnSendMailReport_Click);
            // 
            // lblResult3
            // 
            this.lblResult3.AutoSize = true;
            this.lblResult3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResult3.Location = new System.Drawing.Point(700, 656);
            this.lblResult3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblResult3.Name = "lblResult3";
            this.lblResult3.Size = new System.Drawing.Size(139, 25);
            this.lblResult3.TabIndex = 33;
            this.lblResult3.Text = "Result Text 3";
            // 
            // chkAssignedOnly
            // 
            this.chkAssignedOnly.AutoSize = true;
            this.chkAssignedOnly.Checked = true;
            this.chkAssignedOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAssignedOnly.Location = new System.Drawing.Point(1452, 20);
            this.chkAssignedOnly.Margin = new System.Windows.Forms.Padding(4);
            this.chkAssignedOnly.Name = "chkAssignedOnly";
            this.chkAssignedOnly.Size = new System.Drawing.Size(118, 21);
            this.chkAssignedOnly.TabIndex = 34;
            this.chkAssignedOnly.Text = "Assigned only";
            this.chkAssignedOnly.UseVisualStyleBackColor = true;
            // 
            // btnPendingResult
            // 
            this.btnPendingResult.Location = new System.Drawing.Point(1403, 635);
            this.btnPendingResult.Margin = new System.Windows.Forms.Padding(4);
            this.btnPendingResult.Name = "btnPendingResult";
            this.btnPendingResult.Size = new System.Drawing.Size(120, 69);
            this.btnPendingResult.TabIndex = 35;
            this.btnPendingResult.Text = "Generate Pending Report if any";
            this.btnPendingResult.UseVisualStyleBackColor = true;
            this.btnPendingResult.Visible = false;
            this.btnPendingResult.Click += new System.EventHandler(this.btnSenddailyreport_Click);
            // 
            // pbWaiting
            // 
            this.pbWaiting.Image = ((System.Drawing.Image)(resources.GetObject("pbWaiting.Image")));
            this.pbWaiting.Location = new System.Drawing.Point(580, 187);
            this.pbWaiting.Margin = new System.Windows.Forms.Padding(4);
            this.pbWaiting.Name = "pbWaiting";
            this.pbWaiting.Size = new System.Drawing.Size(396, 367);
            this.pbWaiting.TabIndex = 36;
            this.pbWaiting.TabStop = false;
            this.pbWaiting.Visible = false;
            // 
            // btnDeleteRow
            // 
            this.btnDeleteRow.Location = new System.Drawing.Point(1500, 47);
            this.btnDeleteRow.Margin = new System.Windows.Forms.Padding(4);
            this.btnDeleteRow.Name = "btnDeleteRow";
            this.btnDeleteRow.Size = new System.Drawing.Size(65, 42);
            this.btnDeleteRow.TabIndex = 37;
            this.btnDeleteRow.Text = "Delete Row";
            this.btnDeleteRow.UseVisualStyleBackColor = true;
            this.btnDeleteRow.Click += new System.EventHandler(this.btnDeleteRow_Click);
            // 
            // btnWatchIssue
            // 
            this.btnWatchIssue.Location = new System.Drawing.Point(1435, 47);
            this.btnWatchIssue.Margin = new System.Windows.Forms.Padding(4);
            this.btnWatchIssue.Name = "btnWatchIssue";
            this.btnWatchIssue.Size = new System.Drawing.Size(65, 42);
            this.btnWatchIssue.TabIndex = 38;
            this.btnWatchIssue.Text = "Watch\r\nIssue";
            this.btnWatchIssue.UseVisualStyleBackColor = true;
            this.btnWatchIssue.Visible = false;
            this.btnWatchIssue.Click += new System.EventHandler(this.btnWatchIssue_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(553, 44);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(119, 17);
            this.label5.TabIndex = 39;
            this.label5.Text = "Selected Release";
            // 
            // cmbRelease
            // 
            this.cmbRelease.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRelease.FormattingEnabled = true;
            this.cmbRelease.Location = new System.Drawing.Point(695, 42);
            this.cmbRelease.Margin = new System.Windows.Forms.Padding(4);
            this.cmbRelease.Name = "cmbRelease";
            this.cmbRelease.Size = new System.Drawing.Size(190, 24);
            this.cmbRelease.TabIndex = 40;
            this.cmbRelease.SelectedValueChanged += new System.EventHandler(this.cmbRelease_SelectedValueChanged);
            // 
            // JiralogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1579, 708);
            this.Controls.Add(this.cmbRelease);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnWatchIssue);
            this.Controls.Add(this.btnPendingResult);
            this.Controls.Add(this.btnMailtoDefaulters);
            this.Controls.Add(this.btnDeleteRow);
            this.Controls.Add(this.pbWaiting);
            this.Controls.Add(this.chkAssignedOnly);
            this.Controls.Add(this.lblResult3);
            this.Controls.Add(this.btnGenerateReport);
            this.Controls.Add(this.btnSendMailReport);
            this.Controls.Add(this.grpBoxReportType);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.pbLogprogress);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.lblResult2);
            this.Controls.Add(this.lblResult1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbTeamMember);
            this.Controls.Add(this.grpBoxTeam);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbProjects);
            this.Controls.Add(this.dtPickerEnd);
            this.Controls.Add(this.lblSelectdate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPasswd);
            this.Controls.Add(this.txtUserid);
            this.Controls.Add(this.dtPicker);
            this.Controls.Add(this.lvReport);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "JiralogView";
            this.Text = "Jira Work Log";
            this.grpBoxTeam.ResumeLayout(false);
            this.grpBoxTeam.PerformLayout();
            this.grpBoxReportType.ResumeLayout(false);
            this.grpBoxReportType.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbWaiting)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGenerateReport;
        private System.Windows.Forms.ListView lvReport;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.DateTimePicker dtPicker;
        private System.Windows.Forms.ProgressBar pbLogprogress;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.TextBox txtUserid;
        private System.Windows.Forms.TextBox txtPasswd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblSelectdate;
        private System.Windows.Forms.DateTimePicker dtPickerEnd;
        private System.Windows.Forms.RadioButton rdoDefectleakage;
        private System.Windows.Forms.Label lblResult1;
        private System.Windows.Forms.Label lblResult2;
        private System.Windows.Forms.RadioButton rdoAssignedSTC;
        private System.Windows.Forms.ComboBox cmbProjects;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox grpBoxTeam;
        private System.Windows.Forms.RadioButton rdoDoseIQDev;
        private System.Windows.Forms.Button btnMailtoDefaulters;
        private System.Windows.Forms.ComboBox cmbTeamMember;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox grpBoxReportType;
        private System.Windows.Forms.Button btnSendMailReport;
        private System.Windows.Forms.Label lblResult3;
        private System.Windows.Forms.CheckBox chkAssignedOnly;
        private System.Windows.Forms.Button btnPendingResult;
        private System.Windows.Forms.PictureBox pbWaiting;
        private System.Windows.Forms.Button btnDeleteRow;
        private System.Windows.Forms.RadioButton rdoPerformance;
        private System.Windows.Forms.RadioButton rdoComments;
        private System.Windows.Forms.Button btnWatchIssue;
        private System.Windows.Forms.RadioButton rdoAllTeams;
        private System.Windows.Forms.RadioButton rdoDoseIQQAAutomation;
        private System.Windows.Forms.RadioButton rdoDoseIQCommon;
        private System.Windows.Forms.RadioButton rdoDoseIQQAManual;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbRelease;
    }
}