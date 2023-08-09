using Koshary_Architecture.DatabaseContext;
using Koshary_Architecture.Models;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;

namespace Koshary_Architecture
{
    public partial class AddNewEmployeeByManagerForm : Form
    {
        List<string> Errors = new List<string>();
        EmployeesSqlServerDatabaseContext SqlServerDatabaseContext = new EmployeesSqlServerDatabaseContext();
        Employee NewEmployee = new Employee();
        public AddNewEmployeeByManagerForm()
        {
            InitializeComponent();
        }

        private void AddNewEmployeeButton_Click(object sender, EventArgs e)
        {
            CheckIfEmployeeAgeIsEqualOrGreaterThan21Years();
            CheckIfEmailHasCorrectFormat();
            CheckIfEmployeePhoneNumberIsUnique();
            CheckIfEmployeeHasAtLeastOneSkill();

            ConfigMessageLabels();

            if (Errors.Count > 0)
            {
                DisplayErrorMessage();
            }
            else
            {
                CreateNewEmployee();
                SaveNewEmployeeOnDatabase();
                SendEmailNotificationToAdmin();
                DisplaySuccessMessage();
            }
        }

        private void CheckIfEmployeePhoneNumberIsUnique()
        {
            bool isExist = SqlServerDatabaseContext
                           .Employees
                           .Where(e => e.PhoneNumber == EmployeePhoneNumberTextBox.Text)
                           .Any();
            if (isExist)
            {
                Errors.Add("Phone number is already exist , please enter unique phone number. ");
            }
        }

        private void CheckIfEmployeeAgeIsEqualOrGreaterThan21Years()
        {
            DateTime today = DateTime.Now;
            var age = today.Year - EmployeeBirthDatePickerBox.Value.Year;
            if (age < 21)
            {
                Errors.Add("Employee Age Must be Equal Or Greater Than 21 Years.");
            }
        }

        private void CheckIfEmailHasCorrectFormat()
        {
            bool isEmail = Regex.IsMatch(EmployeeEmailTextBox.Text,
                                         @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                                         RegexOptions.IgnoreCase);

            if (!isEmail)
            {
                Errors.Add("Employee email must has correct format.");
            }

        }

        private void CheckIfEmployeeHasAtLeastOneSkill()
        {
            if (string.IsNullOrWhiteSpace(EmployeeSkillTextBox_1.Text))
            {
                Errors.Add("Employee must has at least one skill , " +
                           "please ensure the skill is entered on the first cell. ");
            }
        }

        private void CreateNewEmployee()
        {
            NewEmployee = new Employee();
            NewEmployee.Name = EmployeeNameTextBox.Text;
            NewEmployee.Birthdate = EmployeeBirthDatePickerBox.Value;
            NewEmployee.Email = EmployeeEmailTextBox.Text;
            NewEmployee.PhoneNumber = EmployeePhoneNumberTextBox.Text;
            NewEmployee.Country = CountryTextBox.Text;
            NewEmployee.City = CityTextBox.Text;
            NewEmployee.StreetAndBuildingNumber = EmployeeStreetAndBuildingNumberTextBox.Text;

            List<EmployeeSkill> employeeSkills = new List<EmployeeSkill>();
            employeeSkills.Add(new EmployeeSkill { SkillName = EmployeeSkillTextBox_1.Text });
            if (!string.IsNullOrEmpty(EmployeeSkillTextBox_2.Text))
                employeeSkills.Add(new EmployeeSkill { SkillName = EmployeeSkillTextBox_2.Text });
            if (!string.IsNullOrEmpty(EmployeeSkillTextBox_3.Text))
                employeeSkills.Add(new EmployeeSkill { SkillName = EmployeeSkillTextBox_3.Text });
            if (!string.IsNullOrEmpty(EmployeeSkillTextBox_4.Text))
                employeeSkills.Add(new EmployeeSkill { SkillName = EmployeeSkillTextBox_4.Text });
            if (!string.IsNullOrEmpty(EmployeeSkillTextBox_5.Text))
                employeeSkills.Add(new EmployeeSkill { SkillName = EmployeeSkillTextBox_5.Text });
            if (!string.IsNullOrEmpty(EmployeeSkillTextBox_6.Text))
                employeeSkills.Add(new EmployeeSkill { SkillName = EmployeeSkillTextBox_6.Text });

            NewEmployee.EmployeeSkills = employeeSkills;
        }

        private void SaveNewEmployeeOnDatabase()
        {
            SqlServerDatabaseContext.Employees.Add(NewEmployee);
            SqlServerDatabaseContext.SaveChanges();
        }

        private void SendEmailNotificationToAdmin()
        {
            /*
            https://www.c-sharpcorner.com/article/sending-email-using-c-sharp/
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            message.From = new MailAddress("FromMailAddress");
            message.To.Add(new MailAddress("ToMailAddress"));
            message.Subject = "Test";
            message.IsBodyHtml = true; //to make message body as html
            message.Body = htmlString;
            smtp.Port = 587;
            smtp.Host = "smtp.gmail.com"; //for gmail host
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("FromMailAddress", "password");
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Send(message);
            */
        }

        private void DisplaySuccessMessage()
        {
            SuccessMessageLabel.Text = NewEmployee.Name + " is created successfuly :)";
            SuccessMessageLabel.Visible = true;
        }

        private void DisplayErrorMessage()
        {
            string errorMessage = string.Empty;
            foreach (var error in Errors)
            {
                errorMessage += error + "\r\n";
            }
            ErrorsMessageLabel.Text = errorMessage;
            ErrorsMessageLabel.Visible = true;
        }

        private void ConfigMessageLabels()
        {
            SuccessMessageLabel.Text = "";
            SuccessMessageLabel.Visible = false;
            ErrorsMessageLabel.Text = "";
            ErrorsMessageLabel.Visible = false;
        }


    }
}