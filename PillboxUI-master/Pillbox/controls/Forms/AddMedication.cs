
using Pillbox.entries;
using Pillbox.Managers;
using System.Net.Http.Metrics;
using System.Text.Json;
using System.Web;
using System.Windows.Forms;


namespace Pillbox.controls.Forms
{
    public partial class AddMedication : Form
    {
        private CommunicationManager _commsManager;
        private TaskManager _taskManger;

        public AddMedication(CommunicationManager communicationManager, TaskManager taskManager)
        {
            InitializeComponent();
            _commsManager = communicationManager;
            _commsManager.ReplyReceivedForText += Handle_ReplyReceived;
            _taskManger = taskManager;
        }

        private void CameraButton_Click(object sender, EventArgs e)
        {
            CameraCapture cameraCapture = new CameraCapture();
            cameraCapture.ShowDialog();
            string scanned = cameraCapture.outputString;
            _commsManager.SendMessage(scanned, ChannelOwner.Internal);
            MessageBox.Show("Please wait while we process the scan. Once done verify or complete details and click Save", "Processing", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Handle_ReplyReceived(object? sender, ReplyReceivedEventArgs e)
        {
            var json = e.Reply;
            var opts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var info = JsonSerializer.Deserialize<MedicationEntry>(json, opts);

            OngoingCheckBox.Checked = info.IsOngoing;
            MedicationNameBox.Text = info.MedicationName;
            DoseUpDown.Value = (decimal)info.Dose;
            UnitsBox.Text = info.DoseUnit;
            QuantityUpDown.Value = info.Quantity;
            PeriodBox.Text = info.Period;
            TimesUpDown.Value = info.Times;
            CountUpDown.Value = info.BoxCount;
            if (info.ExpDate != null) { ExpDateTimePicker.Value = DateTime.Parse(info.ExpDate); }

        }

        private void label7_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Expire date of the medication");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Click on the name of the field you need more information");
        }

        private void label5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Number of pills to take at a time. for example One to be taken twice a day, put 1");
        }

        private void label2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Dose of the Medication. For Example for Apixaban 5mg put 5");
        }

        private void label6_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The Units your medication is on. For Example mg for Miligrams. Note: mcg means micrograms");
        }

        private void label8_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Number of times to take a dose in a period. for example One to be taken twice a day, put 2");

        }

        private void label3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Frequency to take a medication.\n" +
                "DAY per day\n" +
                "WEEK per week \n" +
                "MONTH per Month \n" +
                "YEAR per year \n" +
                "MORNING to be taken in the mornings eg with breakfast \n" +
                "NOON to be taken around midday eg with Lunch \n" +
                "NIGHT to be taken at night eg.before bed \n" +
                "MEAL to be taken with each meal so MORNING and NOON and NIGHT"
                );

        }

        private void label9_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Number of pills that the box Contains.");

        }

        private void label4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Number of days you need to take this medication. If it is a contineous or ongoing medication leave at 0 and tick the check box");
        }

        private void label1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Name of the Medication. For Example: Apixaban");
        }

        private void AddMedication_Load(object sender, EventArgs e)
        {

        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (PeriodBox.Text == null || ((int)HoursPicker.Value+(int)MinutesPicker.Value ==0))
            {
                MessageBox.Show("Please make sure to Select a Period. And if selected CUSTOM most have at least 1 min");
                return;
            }

            MedicationEntry entry = new MedicationEntry();
            entry.MedicationName = MedicationNameBox.Text;
            entry.Dose = (decimal)DoseUpDown.Value;
            entry.BoxCount = (int)CountUpDown.Value;
            entry.Quantity = (int)QuantityUpDown.Value;
            entry.Times = (int)TimesUpDown.Value;
            entry.Period = PeriodBox.Text.ToUpper() == "CUSTOM"? PeriodBox.Text+">"+ (int)HoursPicker.Value+":"+(int)MinutesPicker.Value: PeriodBox.Text;
            entry.DoseUnit = UnitsBox.Text;
            entry.IsOngoing = OngoingCheckBox.Checked;
            entry.ExpDate = ExpDateTimePicker.Value.ToString("yyyy-MM-dd");

            DateTime startTime = DateTime.Today.Add(startTimePicker.Value.TimeOfDay);
            if (_taskManger.breakfastTime == DateTime.MinValue && entry.Period == "MORNING")
            {
                _taskManger.breakfastTime = startTime;
            }
            else if (_taskManger.lunchTime == DateTime.MinValue && entry.Period == "NOON")
            {
                _taskManger.lunchTime = startTime;
            }
            else if (_taskManger.dinnerTime == DateTime.MinValue && entry.Period == "NIGHT")
            {
                _taskManger.dinnerTime = startTime;
            }


            var result = SqliteDataAccess.FindEntry<MedicationEntry, string>("MEDICATIONS", "MedicationName", entry.MedicationName);
            if (result != null)
            {
                entry.Stock = result.Stock + entry.BoxCount;
                entry.StartDate = result.StartDate;
                entry.EndDate = entry.IsOngoing ? "" : DateTime.Now.AddDays((int)nDaysUpDown.Value).ToString("yyyy-MM-dd");
                entry.Restock = NeedRestock(entry);
                // when updating
                string setClause = @"
                        DOSE        = @Dose,
                        DOSEUNIT    = @DoseUnit,
                        QUANTITY    = @Quantity,
                        PERIOD      = @Period,
                        TIMES       = @Times,
                        BOXCOUNT    = @BoxCount,
                        EXPDATE     = @ExpDate,
                        ISONGOING   = @IsOngoing,
                        STOCK       = @Stock,
                        STARTDATE   = @StartDate,
                        ENDDATE     = @EndDate,
                        RESTOCK     = @Restock
                    ";

                string condition = "MedicationName=@MedicationName";
                SqliteDataAccess.Update(entry, "MEDICATIONS", setClause, condition);
                _taskManger.RemoveTask(entry.MedicationName);
            }
            else
            {
                entry.Stock = entry.BoxCount;
                entry.StartDate = DateTime.Now.ToString("yyyy-MM-dd");
                entry.EndDate = entry.IsOngoing ? "" : DateTime.Now.AddDays((int)nDaysUpDown.Value).ToString("yyyy-MM-dd");
                entry.Restock = NeedRestock(entry);

                string fields = "MedicationName, DOSE, DOSEUNIT, QUANTITY, PERIOD, TIMES, BOXCOUNT, EXPDATE, ISONGOING, STOCK, STARTDATE, ENDDATE, RESTOCK";
                string props = "@MedicationName, @Dose, @DoseUnit, @Quantity, @Period, @Times, @BoxCount, @ExpDate, @IsOngoing, @Stock, @StartDate, @EndDate, @Restock";


                SqliteDataAccess.Save(entry, "MEDICATIONS", fields, props);

            }


            _taskManger.AddRecurringPillTasks(entry, startTime);

            MessageBox.Show("Medication Added Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private bool NeedRestock(MedicationEntry entry)
        {
            List<string> oneADay = new List<string> { "DAY", "MORNING", "NOON", "NIGHT" };
            string basePeriod = entry.Period.Contains(">") ? entry.Period.Split('>')[0] : entry.Period;

            if (entry.Times == 0)
                return false;

            int totalDays = (int)nDaysUpDown.Value;
            int quantityPerDose = entry.Quantity;
            int totalPillsNeeded;

            switch (basePeriod)
            {
                case "DAY":
                case "MORNING":
                case "NOON":
                case "NIGHT":
                    totalPillsNeeded = totalDays * quantityPerDose;
                    break;
                case "WEEK":
                    totalPillsNeeded = (totalDays * quantityPerDose) / 7;
                    break;
                case "MONTH":
                    totalPillsNeeded = (totalDays * quantityPerDose) / 30;
                    break;
                case "YEAR":
                    totalPillsNeeded = (totalDays * quantityPerDose) / 365;
                    break;
                case "MEAL":
                    totalPillsNeeded = totalDays * quantityPerDose * 3;
                    break;
                case "CUSTOM":
                    double totalHours = (double)HoursPicker.Value + ((double)MinutesPicker.Value / 60);
                    int dosesPerDay = (int)(24 / totalHours);
                    totalPillsNeeded = totalDays * quantityPerDose * dosesPerDay;
                    break;
                default:
                    totalPillsNeeded = totalDays * quantityPerDose;
                    break;
            }

            return entry.Stock < totalPillsNeeded*15 && entry.Stock > 1;
        }


        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label11_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Time you will start taking the medication. For example 9:00 AM for a morning pill.\n " +
                "Note: if time is previous to actual time it will start next day");
        }

        private void PeriodBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (PeriodBox.SelectedItem?.ToString().ToUpper())
            {
                case "DAY":
                    MinutesPicker.Enabled = false;
                    HoursPicker.Enabled = false;
                    HoursPicker.Value = 24;
                    break;
                case "MORNING":
                    MinutesPicker.Enabled = false;
                    HoursPicker.Enabled = false;
                    HoursPicker.Value = 24;
                    TimesUpDown.Value = 1;
                    TimesUpDown.Enabled = false;
                    if (_taskManger.breakfastTime != DateTime.MinValue)
                    {
                        startTimePicker.Value = DateTime.Today.Add(_taskManger.breakfastTime.TimeOfDay);
                    }
                    break;
                case "NOON":
                    MinutesPicker.Enabled = false;
                    HoursPicker.Enabled = false;
                    HoursPicker.Value = 24;
                    TimesUpDown.Value = 1;
                    TimesUpDown.Enabled = false;
                    if (_taskManger.lunchTime != DateTime.MinValue)
                    {
                        startTimePicker.Value = DateTime.Today.Add(_taskManger.lunchTime.TimeOfDay);
                    }
                    break;
                case "NIGHT":
                    MinutesPicker.Enabled = false;
                    HoursPicker.Enabled = false;
                    HoursPicker.Value = 24;
                    TimesUpDown.Value = 1;
                    TimesUpDown.Enabled = false;
                    if (_taskManger.dinnerTime != DateTime.MinValue)
                    {
                        startTimePicker.Value = DateTime.Today.Add(_taskManger.dinnerTime.TimeOfDay);
                    }
                    break;
                case "WEEK":
                    MinutesPicker.Enabled = false;
                    HoursPicker.Enabled = false;
                    HoursPicker.Value = 0;
                    break;
                case "MONTH":
                    MinutesPicker.Enabled = false;
                    HoursPicker.Enabled = false;
                    HoursPicker.Value = 0;
                    break;
                case "YEAR":
                    MinutesPicker.Enabled = false;
                    HoursPicker.Enabled = false;
                    HoursPicker.Value = 0;
                    break;
                case "MEAL":
                    MinutesPicker.Enabled = false;
                    HoursPicker.Enabled = false;
                    HoursPicker.Value = 0;
                    break;
                case "CUSTOM":
                    MinutesPicker.Enabled = true;
                    HoursPicker.Enabled = true;
                    TimesUpDown.Value = 1;
                    TimesUpDown.Enabled = false;
                    break;
                default:
                    PeriodBox.SelectedItem = "CUSTOM";
                    MinutesPicker.Enabled = true;
                    HoursPicker.Enabled = true;
                    break;
            }
        }


        private void label12_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("How far apart one dosis most be from the other. For example: for 1 dose every 6 hours put 6 in the Hrs counter and 0 in mins counter");
        }
    }
}