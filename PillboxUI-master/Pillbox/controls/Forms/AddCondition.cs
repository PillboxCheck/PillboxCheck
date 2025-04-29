
using Pillbox.entries;
using System.Net.Http.Metrics;
using System.Text.Json;
using Pillbox.entries;


namespace Pillbox.controls.Forms
{
    public partial class AddCondition : Form
    {

        public AddCondition()
        {
            InitializeComponent();

        }


        private void AddCondition_Load(object sender, EventArgs e)
        {
            
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(NameBox.Text) || string.IsNullOrEmpty(TypeBox.Text) || string.IsNullOrEmpty(SeverityBox.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }
            Condition condition = new Condition();
            condition.Name = NameBox.Text;
            condition.Type = TypeBox.Text;
            condition.Severity = SeverityBox.Text;

            var result = SqliteDataAccess.FindEntry<Condition, string>("CONDITIONS", "NAME", condition.Name);
            if (result != null)
            {
                SqliteDataAccess.Update<Condition>(condition, "CONDITIONS", "NAME=@Name, TYPE=@Type, SEVERITY=@Severity", "NAME=@Name");
            }
            else
            {
                SqliteDataAccess.Save<Condition>(condition, "CONDITIONS", "NAME, TYPE, SEVERITY", "@Name, @Type, @Severity");
            }
            this.Close();

        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TypeBox.SelectedIndex == 0)
            {
                SeverityBox.Items.Clear();
                SeverityBox.Items.Add("NONE");
                SeverityBox.Items.Add("LOW");
                SeverityBox.Items.Add("MEDIUM");
                SeverityBox.Items.Add("HIGH");
                SeverityBox.Items.Add("CHRONIC");
            }
            else if (TypeBox.SelectedIndex == 1)
            {
                SeverityBox.Items.Clear();
                SeverityBox.Items.Add("CLASS 1");
                SeverityBox.Items.Add("CLASS 2");
                SeverityBox.Items.Add("CLASS 3");
                SeverityBox.Items.Add("CLASS 4");
                SeverityBox.Items.Add("CLASS 5");
                SeverityBox.Items.Add("CLASS 6");
            }
            else
            {
              SeverityBox.Items.Clear();
                SeverityBox.Items.Add("NONE");
                SeverityBox.Items.Add("MILD");
                SeverityBox.Items.Add("MODERATE");
                SeverityBox.Items.Add("SEVERE");
            }
        }
    }
}