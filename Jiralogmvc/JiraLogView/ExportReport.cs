using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JiralogMVC.Controller;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace JiralogMVC.View
{
    public partial class JiralogView
    {
        const string MailToList =  @"";
        RadioButton GetCheckedRadio(Control container)
        {
            foreach (var control in container.Controls)
            {
                RadioButton radio = control as RadioButton;

                if (radio != null && radio.Checked)
                {
                    return radio;
                }
            }

            return null;
        }
        public void ListViewToCSV(ListView listView, string filePath, bool includeHidden)
        {
            StringBuilder sb = new StringBuilder();

            //Making columns!
            foreach (ColumnHeader ch in listView.Columns)
            {
                sb.Append(ch.Text + ",");
            }
            sb.AppendLine();

            //Looping through items and subitems
            foreach (ListViewItem lvi in listView.Items)
            {
                foreach (ListViewItem.ListViewSubItem lvs in lvi.SubItems)
                {
                    string str = lvs.Text;
                    str = str.Replace(',',';');
                    if (str == string.Empty)
                        sb.Append(" ,");
                    else
                        sb.Append(str + ",");
                }
                sb.AppendLine();
            }
            SaveToFile(filePath, sb);
        }
        public string ListViewToHTML(ListView lv)
        {
            bool bAssignmentReport = (this.ReportType == CommonData.ReportType.REPORT_TEAM_ASSIGNED) ? true : false;
            string tablestyle =
                        @"table
                        {
                        font-family:Arial, Helvetica, sans-serif;
                        width:100%;
                        border-collapse:collapse;
                        }
                        td,th
                        {
                        font-size:1em;
                        border:1px solid #2098bf;
                        padding:3px 7px 2px 7px;
                        }
                        th 
                        {
                        font-size:1.1em;
                        text-align:left;
                        padding-top:5px;
                        padding-bottom:4px;
                        bg-color:#A7C942;
                        color:#ffffff;
                        }
                        tr.alt td 
                        {
                        color:#000000;
                        bg-color:#EAF2D3;
                        }";
            string strHtmlbody = "<html><head>"
                                + "<style>"
                                + tablestyle
                                + "</style>"
                                  +"</head><body>";

            if (rdoDefectleakage.Checked)
                strHtmlbody += String.Format("<p><b>Resolved ::  {0}      Reopened :: {1}</b></p>", _issuesResolved, _issuesReopened);


            strHtmlbody += "</br> <table style=\"font-family:arial;\" border=\"1\" cellspacing=\"0\">";
            //Making columns!
            strHtmlbody += "<tr>";
            foreach (ColumnHeader ch in lv.Columns)
            {
                //sb.Append(ch.Text + ",");
                strHtmlbody += String.Format("<td><b>{0}</b></td>", ch.Text);
            }
            strHtmlbody += "</tr>";

            //Looping through items and subitems
            foreach (ListViewItem lvi in lv.Items)
            {
                bool bWorkNotLogged = false;
                bool bWorkNotAssigned = false;
                strHtmlbody += "<tr>";
                foreach (ListViewItem.ListViewSubItem lvs in lvi.SubItems)
                {        
                    if (bAssignmentReport && String.Compare(lvs.Text, "WORK NOT ASSIGNED") == 0)
                        bWorkNotAssigned = true;

                    if (bWorkNotLogged)
                    {
                        strHtmlbody += String.Format("<td colspan=\"8\" <font color=\"red\">WORK NOT LOGGED</td>");
                        break;
                    }
                    else if (bWorkNotAssigned)
                    {
                        strHtmlbody += String.Format("<td colspan=\"7\" <font color=\"red\">WORK NOT ASSIGNED</td>");
                        break;
                    }
                    else
                    {
                        strHtmlbody += String.Format("<td>{0}</td>", lvs.Text);
                    }
                }
                if (bAssignmentReport && bWorkNotAssigned)
                    strHtmlbody += "</td>";
                strHtmlbody += "</tr>";
            }
            strHtmlbody += "</table><br /></body></html>";

            return strHtmlbody;
        }
        private void SaveToFile(string fileName, StringBuilder sb)
        {
            System.IO.TextWriter w = new System.IO.StreamWriter(fileName);
            w.Write(sb.ToString());
            w.Flush();
            w.Close();
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            //Export to CSV file name           
            
            string fileName = "";
            string worklogfilename = "";
            string teamname = JiralogModel.GetTeamNameFromType(Team);
            DateTime dt1 = dtPicker.Value; DateTime dt2 = dtPickerEnd.Value;

            if (this.ReportType == Controller.CommonData.ReportType.REPORT_TEAM_PERFORMANCE)
                worklogfilename = String.Format(teamname + "-{0}-{1}-{2}-to-{3}-{4}-{5}-TeamPerformanceReport.csv", dt1.Year, dt1.Month, dt1.Day, dt2.Year, dt2.Month, dt2.Day);
            else if (this.ReportType == Controller.CommonData.ReportType.REPORT_DEFECT_LEAKAGE)
                worklogfilename = String.Format(teamname+"-{0}-{1}-{2}-to-{3}-{4}-{5}-DefectLeakageReport.csv", dt1.Year, dt1.Month, dt1.Day, dt2.Year, dt2.Month, dt2.Day);
            else if (this.ReportType == Controller.CommonData.ReportType.REPORT_TEAM_ASSIGNED)
            {
                DateTime dt = DateTime.Now;
                worklogfilename = String.Format(teamname+"-{0}-{1}-{2}-WorkAssignmentonSTC.csv", dt.Year, dt.Month, dt.Day);
            }
            else if (this.ReportType == Controller.CommonData.ReportType.REPORT_JIRA_COMMENTS)
            {
                DateTime dt = DateTime.Now;
                worklogfilename = String.Format(teamname + "-{0}-{1}-{2}-JiraComments.csv", dt1.Year, dt1.Month, dt1.Day);
            }
            fileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + worklogfilename;
            ListViewToCSV(lvReport, fileName, false);
            System.Diagnostics.Process.Start(fileName);
        }
        private void btnSenddailyreport_Click(object sender, EventArgs e)
        {
            Outlook.Application outlookApp = new Outlook.Application();
            Outlook._MailItem mailItem = (Outlook._MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);

            mailItem.To = "";
            DateTime dt = dtPicker.Value;
            string strDate = String.Format("{0}/{1}/{2}", dt.Day, dt.Month, dt.Year);
            mailItem.Subject = "WORKLOG REPORT for " + strDate;
            //mailItem.CC = rdoDoseIQDev.Checked ? "TT_DEV_TEAM@symphonyteleca.com" : (rdoServer.Checked ? "TT_SERVER_TEAM@symphonyteleca.com" : "TT_QC_TEAM@symphonyteleca.com");

            mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;

            string tablestyle =
                        @"table
                        {
                        font-family:Arial, Helvetica, sans-serif;
                        width:100%;
                        border-collapse:collapse;
                        }
                        td,th
                        {
                        font-size:1em;
                        border:1px solid #98bf21;
                        padding:3px 7px 2px 7px;
                        }
                        th 
                        {
                        font-size:1.1em;
                        text-align:left;
                        padding-top:5px;
                        padding-bottom:4px;
                        bg-color:#A7C942;
                        color:#ffffff;
                        }
                        tr.alt td 
                        {
                        color:#000000;
                        bg-color:#EAF2D3;
                        }";
            string strHtmlbody = "<html><head>"
                                +"<style>"
                                + tablestyle
                                +"</style>"
                    +"</head>"
                    + "<body><p><b>Following is the work log report for the team for " + strDate + "</b></p>"
                    + "</br> <table border=\"1\" cellspacing=\"0\">"
                    + "<tr><td><b>NAME</b></td><td><b>JIRA ID</b></td><td><b>WORK LOG</b></td></tr>";

            foreach (ListViewItem lvi in lvReport.Items)
            {
                strHtmlbody += String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", lvi.SubItems[0].Text, lvi.SubItems[1].Text, lvi.SubItems[7].Text);
            }

            strHtmlbody += "</table><br /></body></html>";
            mailItem.HTMLBody = strHtmlbody;
            mailItem.Display(true);
        }
        private void btnMailtoDefaulters_Click(object sender, EventArgs e)
        {
            string maillist = "", namelist = "";
            string mailcontent = "";
            DateTime dt = dtPicker.Value;

            Outlook.Application outlookApp = new Outlook.Application();
            Outlook._MailItem mailItem = (Outlook._MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);

            foreach (Member mem in _listDefaulters)
            {
                maillist += mem.emailid + ";";
                namelist += CommonFunc.GetFirstName(mem.name) + ", ";
            }
            string strDate = String.Format("{0}/{1}/{2}", dt.Day, dt.Month, dt.Year);
            mailItem.To = maillist+"\n\n";
           // mailItem.CC = rdoDoseIQDev.Checked ? "TT_DEV_TEAM@symphonyteleca.com" : (rdoServer.Checked ? "TT_SERVER_TEAM@symphonyteleca.com" : "TT_QC_TEAM@symphonyteleca.com");
            mailItem.Subject = "[ ALERT ] You missed your JIRA Work Log for " + strDate;

            string strHtmlbody = "<html><head>"
                   + "</head>"
                   + "<body style=\"font-family:calibri\"><p><b>Dear " + namelist + "</b></p>"
                   + "<p>You have missed to log your work in JIRA for " + strDate + "</p>"
                   + "<p>Please log it today for this missed date</p>"
                   + "<p>Regards,<br>Avanish</p>"
                   + "</body></html>";

            mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;
            mailItem.HTMLBody = strHtmlbody;
            mailItem.Display(true);
        }
        private void btnSendMailReport_Click(object sender, EventArgs e)
        {
            //Mail Config 
            Outlook.Application outlookApp = new Outlook.Application();
            Outlook._MailItem mailItem = (Outlook._MailItem)outlookApp.CreateItem(Outlook.OlItemType.olMailItem);

            DateTime dt = dtPicker.Value;
            DateTime dtEnd = dtPickerEnd.Value;

            string strDate = String.Format("{0}/{1}/{2}", dt.Day, dt.Month, dt.Year);
            string strEndDate = String.Format("{0}/{1}/{2}", dtEnd.Day, dtEnd.Month, dtEnd.Year);

            string strReportDate;

            //Finding Subject ****************************************************************
            if (rdoDefectleakage.Checked )
                strReportDate = "during " + strDate + " to " + strEndDate;
            else
                strReportDate = "";

            RadioButton rb1 = GetCheckedRadio((Control)grpBoxTeam);
            RadioButton rb2 = GetCheckedRadio((Control)grpBoxReportType);
            mailItem.Subject = "TTGOC " + rb1.Text + " Team -:-" + rb2.Text + " Report " + strReportDate;
            //********************************************************************************
            
            mailItem.To = MailToList;
            mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;
            mailItem.HTMLBody = ListViewToHTML(lvReport);
            mailItem.Display(true);
        }
    }
}
