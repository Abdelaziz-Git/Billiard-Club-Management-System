using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Bunifu.UI.WinForms;

namespace Billiard_v1
{
    public partial class BilliardManagementSystemForm : Form
    {
        string FilePath = @"C:\Users\ABDELAZIZ\Documents\Billiard_Project_Database.txt";
        public BilliardManagementSystemForm()
        {
            InitializeComponent();
            
        }


    
    private void Form1_Load(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
            progBarOccupiedTables.Maximum = DateTimes.Count;
            UploadtxtFileDataToListViewHistory();
        }
        public class clsHistoryInfo
        {
            public string PlayerName { get; set; }
            public string TableNumber { get; set; }
            public DateTime TimePlayed { get; set; }
            public DateTime PlayDate { get; set; }
            public float Price { get; set; }

            public clsHistoryInfo(string playerName, string tableNumber, DateTime timePlayed, DateTime playDate, float price)
            {
                PlayerName = playerName;
                TableNumber = tableNumber;
                TimePlayed = timePlayed;
                PlayDate = playDate;
                Price = price;
            }
        }

        List<DateTime> DateTimes = new List<DateTime>
        {
            new DateTime(2024, 12, 6, 0, 0, 0),
            new DateTime(2024, 12, 6, 0, 0, 0),
            new DateTime(2024, 12, 6, 0, 0, 0),
            new DateTime(2024, 12, 6, 0, 0, 0),
            new DateTime(2024, 12, 6, 0, 0, 0),
            new DateTime(2024, 12, 6, 0, 0, 0)
        };
        int TodayIncom, Last30DaysIncom;
        private void ChangeBtnStarText(Button btnStart)
        {

            btnStart.Text = btnStart.Text == "Start" ? "Pause" : "Start";
        }
        private void AddTimePerSecondToLblTime(ref DateTime dt, Label lblTime)
        {
            dt = dt.AddSeconds(1);
            lblTime.Text = dt.ToString("HH:mm:ss");
        }
        private float GetOrUpdatePrice(DateTime dt, Label lblPrice)
        {
            float Price;
            Price = ((float)(dt.Hour * 3600 + dt.Minute * 60 + dt.Second)) * ((float)numericUpDownPrice.Value / 3600);

            lblPrice.Text = Price.ToString("F2") + "DH";
            return Price;
        }
        private void UpdatelblTime()
        {
            for (int i = 0; i < DateTimes.Count; i++)
            {
                Button btnStart = Controls.Find($"btnStartTable{i + 1}", true).FirstOrDefault() as Button;
                Label lblTime = Controls.Find($"lblTimeTable{i + 1}", true).FirstOrDefault() as Label;
                Label lblPrice = Controls.Find($"lblPriceTable{i + 1}", true).FirstOrDefault() as Label;

                if (btnStart != null && btnStart.Text == "Pause")
                {
                    DateTime temp = DateTimes[i];
                    AddTimePerSecondToLblTime(ref temp, lblTime);
                    DateTimes[i] = temp;

                    GetOrUpdatePrice(DateTimes[i], lblPrice);
                }
            }

        }
        private void UpdateTimer1()
        {

            if (btnStartTable1.Text == "Pause" || btnStartTable2.Text == "Pause" || btnStartTable3.Text == "Pause" ||
               btnStartTable4.Text == "Pause" || btnStartTable5.Text == "Pause" || btnStartTable6.Text == "Pause")
                Timer1.Start();
            else
                Timer1.Stop();
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            ChangeBtnStarText((Button)sender);
            UpdateTimer1();
        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            UpdatelblTime();
        }
        private void ResetTable(Button btnTag)
        {
            for (int i = 0; i < DateTimes.Count; i++)
            {
                TextBox txtPlayer = Controls.Find($"txtPlayerTable{i + 1}", true).FirstOrDefault() as TextBox;
                Button btnStart = Controls.Find($"btnStartTable{i + 1}", true).FirstOrDefault() as Button;
                Label lblTime = Controls.Find($"lblTimeTable{i + 1}", true).FirstOrDefault() as Label;
                Label lblPrice = Controls.Find($"lblPriceTable{i + 1}", true).FirstOrDefault() as Label;

                if (btnTag != null && btnTag.Tag.ToString() == ($"{i + 1}"))
                {
                    txtPlayer.Clear();
                    btnStart.Text = "Start";
                    DateTimes[i] = new DateTime(2024, 12, 6, 0, 0, 0);
                    lblTime.Text = "00:00:00";
                    lblPrice.Text = "00DH";
                }

            }
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetTable((Button)sender);
        }
        private void UpdateProgBarOccupiedTables()
        {
            int NumberOfOccupiedTables = 0;

            for (int i = 0; i < DateTimes.Count; i++)
            {
                Button btnStart = Controls.Find($"btnStartTable{i + 1}", true).FirstOrDefault() as Button;
                if (btnStart.Text == "Pause")
                    NumberOfOccupiedTables++;
            }
            progBarOccupiedTables.Value = NumberOfOccupiedTables;

            // calculate Circle progrese value
            int NumberOfEmptyTables = DateTimes.Count - progBarOccupiedTables.Value;
            lblNumberOfEmptyTables.Text = $"Number of empty tables: {NumberOfEmptyTables}";
        }
        private void btnStart_TextChanged(object sender, EventArgs e)
        {
            UpdateProgBarOccupiedTables();
        }
        clsHistoryInfo GetTableInfoAfterButtonEndClick(Button btnEnd)
        {
            GroupBox gbTable = new GroupBox();
            TextBox txtPlayer = new TextBox();
            DateTime TimePlayed = new DateTime();
            DateTime PlayDate = new DateTime();
            float Price = new float();

            for (int i = 0; i < DateTimes.Count; i++)
            {
                Button btnEndx = Controls.Find($"btnEndTable{i + 1}", true).FirstOrDefault() as Button;

                if (btnEndx != null && btnEndx.Name == btnEnd.Name)
                {
                    gbTable = Controls.Find($"gbTable{i + 1}", true).FirstOrDefault() as GroupBox;
                    Label lblPrice = Controls.Find($"lblPriceTable{i + 1}", true).FirstOrDefault() as Label;
                    txtPlayer = Controls.Find($"txtPlayerTable{i + 1}", true).FirstOrDefault() as TextBox;
                    Price = GetOrUpdatePrice(DateTimes[i], lblPrice);
                    TimePlayed = DateTimes[i];
                    PlayDate = DateTime.Now;
                }
            }

            clsHistoryInfo HistoryInfo = new clsHistoryInfo(

                txtPlayer.Text,
                gbTable.Text,
                TimePlayed,
                PlayDate,
                Price

            );

            return HistoryInfo;
        }
        private string Convert_clsHistoryInfoObjectToLineRecord(clsHistoryInfo HistoryInfo)
        {
            return $"{HistoryInfo.PlayerName}#{HistoryInfo.TableNumber}" +
                $"#{HistoryInfo.TimePlayed.ToString("HH:mm:ss")}" +
                $"#{HistoryInfo.Price.ToString()}#{HistoryInfo.PlayDate.ToString("MM/dd/yyyy HH:mm")}";
        }
        clsHistoryInfo Convert_LineToclsHistoryInfoObject(string Line)
        {

            char[] seperator = { '#' };
            string[] LineSplit = Line.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            if (LineSplit.Length < 5)
            {
                MessageBox.Show("The line does not contain enough data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            DateTime TimePlayed;
            if (!DateTime.TryParseExact(LineSplit[2], "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out TimePlayed))
            {
                MessageBox.Show("The date string is not in the correct format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            DateTime PlayDate;
            if (!DateTime.TryParseExact(LineSplit[4], "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out PlayDate))
            {
                MessageBox.Show("The date string is not in the correct format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            float Price;
            if (!float.TryParse(LineSplit[3], out Price))
            {
                MessageBox.Show("The Price is not a valid float.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            clsHistoryInfo HistoryInfo = new clsHistoryInfo(
                LineSplit[0],
                LineSplit[1],
                TimePlayed,
                PlayDate,
                Price
                );

            return HistoryInfo;
        }
        private void UploadtxtFileDataToListViewHistory()
        {
            string[] LinesFromFile = File.ReadAllLines(FilePath);
            if (LinesFromFile.Length > 0)
            {
                lvHistory.Items.Clear();
                foreach (string Line in LinesFromFile)
                {
                    clsHistoryInfo historyInfo = Convert_LineToclsHistoryInfoObject(Line);
                    InsertTableInfoToListViewHistory(historyInfo);
                }
            }
        }
        private void Add_clsHistoryObjectLineToTxtFile(clsHistoryInfo historyInfo)
        {
            string Line = Convert_clsHistoryInfoObjectToLineRecord(historyInfo);
            string fileContent = File.ReadAllText(FilePath);
            if (!fileContent.EndsWith(Environment.NewLine))
            {
                // Add a newline if the file does not end with one
                File.AppendAllText(FilePath, Environment.NewLine);
            }
            File.AppendAllText(FilePath, Line + Environment.NewLine);
        }
        private void InsertTableInfoToListViewHistory(clsHistoryInfo HistoryInfo)
        {
            if (HistoryInfo != null && HistoryInfo.Price > 0)
            {
                ListViewItem item = new ListViewItem(HistoryInfo.PlayerName,0);
                item.SubItems.Add(HistoryInfo.TableNumber);
                item.SubItems.Add(HistoryInfo.TimePlayed.ToString("HH:mm:ss"));
                item.SubItems.Add(HistoryInfo.Price.ToString("F2") + "DH");
                item.SubItems.Add(HistoryInfo.PlayDate.ToString("g"));
                lvHistory.Items.Add(item);
            }
        }
        private DialogResult btnEndMessage(Button btnEnd)
        {
            clsHistoryInfo historyInfo = GetTableInfoAfterButtonEndClick(btnEnd);
            return MessageBox.Show($"Are you sure you want to finish with {historyInfo.PlayerName} and reset {historyInfo.TableNumber}?"
                , $"End play in {historyInfo.TableNumber}", MessageBoxButtons.OKCancel);
        }
        private void ManageHistoryData(Button btnEnd)
        {
            if (btnEndMessage(btnEnd) == DialogResult.OK)
            {
                Add_clsHistoryObjectLineToTxtFile(GetTableInfoAfterButtonEndClick(btnEnd));
                UploadtxtFileDataToListViewHistory();
                ResetTable(btnEnd);
            }

        }
        private void btnEnd_Click(object sender, EventArgs e)
        {
            ManageHistoryData((Button)sender);
            CalculateIncoms();
            UpdateCirclesProgressIncoms();
        }
        Color GetColorChoicedByColorDialog()
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                return colorDialog1.Color;
            }
            return Color.Transparent;
        }
        private void changeBackColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control SourceControl = cmsChangeBackColor.SourceControl;

            if (SourceControl != null)
            {
                SourceControl.BackColor = GetColorChoicedByColorDialog();
            }
        }

        private void btnManageTables_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void bunifuToggleSwitch21_CheckedChanged(object sender, EventArgs e)
        {
            if (ToggleSwitch.Checked)
            {
                tpHome.BackColor = Color.Black;
            }
            else
                tpHome.BackColor = Color.White;

            gpLast30DaysIncom.BackColor = tpHome.BackColor;
            gbTodayIncom.BackColor = tpHome.BackColor;


        }

        void CalculateIncoms()
        {
            DateTime FirstDayInLast30Days = new DateTime();
            FirstDayInLast30Days = DateTime.Now.Subtract(TimeSpan.FromDays(30));

            string[] LinesFromFile = File.ReadAllLines(FilePath);
            if (LinesFromFile.Length > 0)
            {
                TodayIncom = 0;
                Last30DaysIncom = 0;
                foreach (string Line in LinesFromFile)
                {
                    clsHistoryInfo historyInfo = Convert_LineToclsHistoryInfoObject(Line);
                    if (historyInfo.PlayDate.ToString("d") == DateTime.Now.ToString("d"))
                    {
                        TodayIncom += (int)historyInfo.Price;
                    }
                    if (historyInfo.PlayDate >= FirstDayInLast30Days)
                    {
                        Last30DaysIncom += (int)historyInfo.Price;
                    }
                }
            }
        }

        void SearchInHistoryTableByTxtName()
        {
            string[] LinesFromFile = File.ReadAllLines(FilePath);
            if (LinesFromFile.Length > 0)
            {
                lvHistory.Items.Clear();
                foreach (string Line in LinesFromFile)
                {
                    clsHistoryInfo historyInfo = Convert_LineToclsHistoryInfoObject(Line);
                    if(historyInfo.PlayerName.ToLower()==txtSearch.Text.ToLower())
                    {
                        InsertTableInfoToListViewHistory(historyInfo);
                    }
                }
            }
            if (txtSearch.Text.Length == 0)
                UploadtxtFileDataToListViewHistory();

        }
        void SearchInHistoryTableByDatePicker(DateTime date)
        {
            string[] LinesFromFile = File.ReadAllLines(FilePath);
            if (LinesFromFile.Length > 0)
            {
                lvHistory.Items.Clear();
                foreach (string Line in LinesFromFile)
                {
                    clsHistoryInfo historyInfo = Convert_LineToclsHistoryInfoObject(Line);
                    if (historyInfo.PlayDate.ToString("d") == date.ToString("d"))
                    {
                        InsertTableInfoToListViewHistory(historyInfo);
                    }
                }
            }
            if (!bunifuToggleSwitch1.Checked)
                UploadtxtFileDataToListViewHistory();
        }
        private void txtSearch_TextChange(object sender, EventArgs e)
        {
            SearchInHistoryTableByTxtName();
        }

        void UpdateToggleSwitchForDatePickerInTabHistory()
        {
            if (bunifuToggleSwitch1.Checked)
                DatePicker1.Enabled = true;
            else
            {
                DatePicker1.Enabled = false;
                UploadtxtFileDataToListViewHistory();
            }
        }
        private void bunifuToggleSwitch1_CheckedChanged(object sender, BunifuToggleSwitch.CheckedChangedEventArgs e)
        {
            UpdateToggleSwitchForDatePickerInTabHistory();
        }
        private void DatePicker1_CloseUp(object sender, EventArgs e)
        {
            SearchInHistoryTableByDatePicker(DatePicker1.Value);
        }
        void UpdateCirclesProgressIncoms()
        {
            CircleProgressTodayIncom.Value = 0;
            CircleProgressLast30DaysIncom.Value = 0;
            if (TodayIncom > 0)
            {
                CircleProgressTodayIncom.Maximum = TodayIncom * 2;
                CircleProgressTodayIncom.Value = TodayIncom;
            }
           
            CircleProgressLast30DaysIncom.Maximum = Last30DaysIncom * 2;
            CircleProgressLast30DaysIncom.Value = Last30DaysIncom;
            
        }

        private void tpLogin_Click(object sender, EventArgs e)
        {

        }

        private void btnLoginIntpLogin_Click(object sender, EventArgs e)
        {
            if(txtUsername.Text==txtUsername.Tag.ToString()&&txtPassword.Text==txtPassword.Tag.ToString())
            {
                tabControl1.SelectedIndex = 0;
                btnHome.Enabled = true;
                btnManageTables.Enabled = true;
                btnHistory.Enabled = true;
            }
            else
            {
                MessageBox.Show("Invalid Username/Password!", "Login error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            CalculateIncoms();
            UpdateCirclesProgressIncoms();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
        }

       
    }
}
