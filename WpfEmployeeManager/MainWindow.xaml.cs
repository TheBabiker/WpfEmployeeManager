using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using WpfEmployeeManager.Models;
using System.Diagnostics;
using System.Configuration;


namespace WpfEmployeeManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   HttpClient client = new HttpClient();
        public MainWindow()
        {
            client.BaseAddress = new Uri("https://gorest.co.in/public/v2/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
                );
            string apiToken = ConfigurationManager.AppSettings["ApiToken"];
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);
            InitializeComponent();
        }

        private void BtnLoadEmployees_Click(object sender, RoutedEventArgs e)
        {
            this.GetEmployees();
        }

        private async void GetEmployees()
        {
            lblMessage.Content = "";
            var response = await client.GetStringAsync("users");
            var employees = JsonConvert.DeserializeObject<List<Employee>>(response);
            dgEmployee.DataContext = employees;
        }

        private async void SaveEmployee(Employee employee)
        {
            await client.PostAsJsonAsync("users", employee);
        }

        private async void UpdateEmployee(Employee employee)
        {
            await client.PutAsJsonAsync("users/"+ employee.id, employee);
        }

        private async void DeleteEmployee(int employeeId)
        {
            await client.DeleteAsync("users/"+ employeeId);
        }

        private void BtnSaveEmployee_Click(object sender, RoutedEventArgs e)
        {
            var employee = new Employee()
            {
                id = Convert.ToInt32(txtEmployeeId.Text),
                name = txtName.Text,
                email = txtEmail.Text,
                gender = txtGender.Text,
                status = txtStatus.Text,
            };

            if (employee.id == 0)
            {
                this.SaveEmployee(employee);
                lblMessage.Content = "Employee Saved";
            }
            else
            {
                this.UpdateEmployee(employee);
                lblMessage.Content = "Employee Updated";
            }

            txtEmployeeId.Text = 0.ToString();
            txtName.Text = "";
            txtEmail.Text = "";
            txtGender.Text = "";
            txtStatus.Text = "";

            GetEmployees();

        }

        void BtnEditEmployee(object sender, RoutedEventArgs e)
        {
            Employee employee = ((FrameworkElement)sender).DataContext as Employee;
       
      
                txtEmployeeId.Text = employee.id.ToString();
                txtName.Text = employee.name;
                txtEmail.Text = employee.email;
                txtGender.Text = employee.gender;
                txtStatus.Text = employee.status;

            
           
        }

        void BtnDeleteEmployee(object sender, RoutedEventArgs e)
        {
            Employee employee = ((FrameworkElement)sender).DataContext as Employee;
            this.DeleteEmployee(employee.id);
            GetEmployees();
        }
    }
}
