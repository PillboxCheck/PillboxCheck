using Pillbox.controls;
using Pillbox.controls.Forms;
using Pillbox.entries;
using Pillbox.Managers;
using Pillbox.Services;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Text.Json;

namespace Pillbox
{
    public partial class mainForm : Form
    {

        public CommunicationManager commsManager = new CommunicationManager(serverIp: "127.0.0.1", port: 52712);
        private VoiceAssistantService _voiceAssistant;
        private NotifyIcon _notifyIcon;
        public TaskManager taskManager;// = new TaskManager();
        public OverDueManager dueManager;

        public mainForm()
        {

            InitializeComponent();
            LoadingForm lf = new LoadingForm();
            lf.CommsManager = commsManager;
            DialogResult r = lf.ShowDialog();
            if (r == DialogResult.OK)
            {
                Debug.WriteLine("Connected");
            }
            else
            {
                Debug.WriteLine("Not Accepted");
                this.Close();

            }
            SummaryPDF.GenerateSummaryPDF(IsInternal: true);
            _voiceAssistant = new VoiceAssistantService();
            _voiceAssistant.CommandRecognized += VoiceAssistant_CommandRecognized;
            _voiceAssistant.QuestionRecognized += VoiceAssistant_QuestionRecognized;

            commsManager.ReplyReceivedForVoice += Handle_VoiceReply;
            commsManager.ReplyReceivedForEvent += Handle_ReplyReceivedforEvent;

            taskManager = new TaskManager();
            taskManager.TaskDue += Handle_TaskDue;
            taskManager.ExpiredTask += Handle_TaskExpired;
            

            _notifyIcon = new NotifyIcon
            {
                Visible = true,
                Icon = Icon
            };
            dueManager = new OverDueManager();
            this.Load += MainForm_Load;
            this.FormClosing += MainForm_FormClosing;
        }

        private void Handle_VoiceReply(object? sender, ReplyReceivedEventArgs e)
        {
            _voiceAssistant.SpeakTTS(e.Reply);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _voiceAssistant.Start();
            Debug.WriteLine("VOICE ASSISTANT STARTED");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _voiceAssistant.Stop();
            commsManager.Disconnect();
            taskManager.Dispose();
            dueManager.Dispose();
            Application.Exit();
        }

        private void VoiceAssistant_CommandRecognized(object sender, string command)
        {
            // Ensure safe UI operations by invoking on the main thread.
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => ProcessCommand(command)));
            }
            else
            {
                ProcessCommand(command);
            }
            //Refresh();
            Debug.WriteLine("==========================================================");
            Debug.WriteLine($"Command recognized: {command}");
            Debug.WriteLine("==========================================================");
        }

        private void ProcessCommand(string command)
        {
            // Process the command as needed.
            switch (command)
            {
                case "open file":
                    //OpenFile();
                    break;
                case "save file":
                    //SaveFile();
                    break;
                case "close application":
                    this.Close();
                    break;
                case "open dashboard" or "show dashboard":
                    LoadDashboard();
                    break;
                case "open inventory" or "show inventory":
                    LoadInventory();
                    break;
                case "open chat" or "show chat":
                    LoadChat();
                    break;
                case "open history" or "show history":
                    LoadHistory(); break;

                case "open settings" or "show settings":
                    LoadHelp();
                    break;

                default:
                    MessageBox.Show($"Command not recognized: {command}");
                    break;


            }
        }
        private void addUserControl(UserControl uc)
        {
            uc.Dock = DockStyle.Fill;
            panel4.Controls.Clear();
            panel4.Controls.Add(uc);
            uc.BringToFront();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            //AI CHAT BUTTON
            LoadChat();
        }

        private void LoadChat()
        {
            //AI CHAT BUTTON
            Chat uc = new Chat(commsManager);
            addUserControl(uc);

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            LoadHistory();
        }

        private void LoadHistory()
        {
            // History buttton
            PersonalDetails uc = new PersonalDetails(commsManager);
            addUserControl(uc);
        }




        private void guna2Button1_Click(object sender, EventArgs e)
        {
            //DASHBOARD BUTTON
            LoadDashboard();

        }

        private void LoadDashboard()
        {
            UserControl uc = new Dashboard(taskManager, dueManager);
            addUserControl(uc);
        }

        private void Close_Click(object sender, EventArgs e)
        {
            commsManager.Disconnect();
            Application.Exit();
        }

        private void Minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Inventory_Click(object sender, EventArgs e)
        {
            LoadInventory();
        }

        private void LoadInventory()
        {
            Inventory uc = new Inventory(commsManager, taskManager);
            addUserControl(uc);
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            LoadHelp();
        }

        private void LoadHelp()
        {
            HelpControl uc = new HelpControl(commsManager,dueManager);
            addUserControl(uc);
        }

        private void VoiceImage_Click(object sender, EventArgs e)
        {
            VoiceImage.Checked = _voiceAssistant.IsActivated();
        }

        private void VoiceAssistant_QuestionRecognized(object sender, string question)
        {
            if (question == "" || question == null)
            {
                return;
            }
            commsManager.SendMessage(question, ChannelOwner.VoiceAssistant);
            _voiceAssistant.SpeakTTS("Sure, give me a couple of minutes while i process your question!");
        }
        private void Handle_TaskDue(object sender, ScheduledTask task)
        {
            _notifyIcon.ShowBalloonTip(1000, "PillBoxCheck", $"Time for you to take your: {task.Name}", ToolTipIcon.Info);
            System.Media.SystemSounds.Exclamation.Play();
            _voiceAssistant.SpeakTTS($"Time for you to take your: {task.Name}");
            dueManager.AddTask(new OverdueTask { Name = task.Name, ScheduledTime = task.ScheduledTime });
        }
        private void Handle_TaskExpired(object sender, ScheduledTask task)
        {
            _notifyIcon.ShowBalloonTip(1000, "PillBoxCheck", $"The Medication {task.Name}, has expired! Please Discard it and update stock with New one if Needed.", ToolTipIcon.Warning);
            System.Media.SystemSounds.Beep.Play();
            _voiceAssistant.SpeakTTS($"The Medication: {task.Name} has expired!");
        }
        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            LoadDashboard();
        }

        private void Handle_ReplyReceivedforEvent(object? sender, ReplyReceivedEventArgs e)
        {
            var json = e.Reply;
            var opts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            EventScanResult result = JsonSerializer.Deserialize<EventScanResult>(json, opts);

            foreach (ScheduledTask task in result.Events)
            {
                taskManager.AddScheduledTask(task);
            }

        }
    }
}
