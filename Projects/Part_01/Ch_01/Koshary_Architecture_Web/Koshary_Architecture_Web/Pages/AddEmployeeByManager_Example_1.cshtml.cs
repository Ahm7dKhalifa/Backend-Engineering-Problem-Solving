using Koshary_Architecture_Web.DatabaseContext.EfCoreWithSqlServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;
using Koshary_Architecture_Web.DatabaseContext.EfCoreWithSqlServer;
using Koshary_Architecture_Web.UI_Models;

namespace Koshary_Architecture_Web.Pages
{
    public class AddEmployeeByManager_Example_1_Model : PageModel
    {
        #region Properties
        List<string> Errors = new List<string>();
        EmployeesSqlServerDatabaseContext SqlServerDatabaseContext = new EmployeesSqlServerDatabaseContext();
        Employee NewEmployee = new Employee();
        public EmployeeFormModel EmployeeFormModel = new EmployeeFormModel();
        #endregion

        public void OnGet()
        {

        }

        public void OnPost(EmployeeFormModel newEmployee)
        {
            InitConfiguration(newEmployee);

            CheckIfEmployeeAgeIsEqualOrGreaterThan21Years();
            CheckIfEmailHasCorrectFormat();
            CheckIfEmployeePhoneNumberIsRequiredAndUnique();
            CheckIfEmployeeHasAtLeastOneSkill();

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

        private void InitConfiguration(EmployeeFormModel newEmployee)
        {
            Errors = new List<string>();

            EmployeeFormModel = newEmployee;
        }

        private void CheckIfEmployeeAgeIsEqualOrGreaterThan21Years()
        {
            DateTime today = DateTime.Now;
            var age = today.Year - EmployeeFormModel.Birthdate.Year;
            if (age < 21)
            {
                Errors.Add("Employee age should be equal or greater than 21 years.");
            }
        }

        private void CheckIfEmailHasCorrectFormat()
        {
            bool isEmail = Regex.IsMatch(EmployeeFormModel.Email,
                                         @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                                         RegexOptions.IgnoreCase);

            if (!isEmail)
            {
                Errors.Add("Employee email must has correct format.");
            }
        }

        private void CheckIfEmployeePhoneNumberIsRequiredAndUnique()
        {
            if (string.IsNullOrEmpty(EmployeeFormModel.PhoneNumber))
            {
                Errors.Add("Phone number is required. ");
            }

            bool isPhoneNumberExist =   SqlServerDatabaseContext
                                       .Employees
                                       .Where(e => e.PhoneNumber == EmployeeFormModel.PhoneNumber)
                                       .Any();
            if (isPhoneNumberExist)
            {
                Errors.Add("Phone number is already exist , please enter unique phone number. ");
            }
        }


        private void CheckIfEmployeeHasAtLeastOneSkill()
        {
            if (string.IsNullOrWhiteSpace(EmployeeFormModel.EmployeeSkill_1))
            {
                Errors.Add("Employee should has at least one skill , " +
                           "please ensure the skill is entered on the first cell. ");
            }
        }

        private void CreateNewEmployee()
        {
            NewEmployee = new Employee();
            NewEmployee.Name = EmployeeFormModel.Name;
            NewEmployee.Birthdate = EmployeeFormModel.Birthdate;
            NewEmployee.Email = EmployeeFormModel.Email;
            NewEmployee.PhoneNumber = EmployeeFormModel.PhoneNumber;
            NewEmployee.Country = EmployeeFormModel.Country;
            NewEmployee.City = EmployeeFormModel.City;
            NewEmployee.StreetAndBuildingNumber = EmployeeFormModel.StreetAndBuildingNumber;

            List<EmployeeSkill> employeeSkills = new List<EmployeeSkill>();
            employeeSkills.Add(new EmployeeSkill { SkillName = EmployeeFormModel.EmployeeSkill_1 });
            if (!string.IsNullOrEmpty(EmployeeFormModel.EmployeeSkill_2))
                employeeSkills.Add(new EmployeeSkill { SkillName = EmployeeFormModel.EmployeeSkill_2 });
            if (!string.IsNullOrEmpty(EmployeeFormModel.EmployeeSkill_3))
                employeeSkills.Add(new EmployeeSkill { SkillName = EmployeeFormModel.EmployeeSkill_3 });
            if (!string.IsNullOrEmpty(EmployeeFormModel.EmployeeSkill_4))
                employeeSkills.Add(new EmployeeSkill { SkillName = EmployeeFormModel.EmployeeSkill_4 });
            if (!string.IsNullOrEmpty(EmployeeFormModel.EmployeeSkill_5))
                employeeSkills.Add(new EmployeeSkill { SkillName = EmployeeFormModel.EmployeeSkill_5 });
            if (!string.IsNullOrEmpty(EmployeeFormModel.EmployeeSkill_6))
                employeeSkills.Add(new EmployeeSkill { SkillName = EmployeeFormModel.EmployeeSkill_6 });

            NewEmployee.EmployeeSkills = employeeSkills;
        }

        private void SaveNewEmployeeOnDatabase()
        {
            SqlServerDatabaseContext.Employees.Add(NewEmployee);
            SqlServerDatabaseContext.SaveChanges();
        }

        private void SendEmailNotificationToAdmin()
        {
            Console.WriteLine("Email is sent ...");
            Console.WriteLine("you can uncomment the code below to send actual email ");
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
            EmployeeFormModel.IsSuccess = true;

            string successMessage = "Employee " + NewEmployee.Name + " is added successfuly :)";
            EmployeeFormModel.Message = successMessage;
        }

        private void DisplayErrorMessage()
        {
            EmployeeFormModel.IsSuccess = false;

            string errorMessage = string.Empty;
            foreach (var error in Errors)
            {
                errorMessage += error + "\r\n";
            }
            EmployeeFormModel.Message = errorMessage;
        }
    }
}