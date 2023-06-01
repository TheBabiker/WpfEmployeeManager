using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using WpfEmployeeManager.Models;
using System.Configuration;
using System.IO;
using Microsoft.Win32;
using System.Linq;

namespace WpfEmployeeManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   HttpClient client = new HttpClient();
        private List<Employee> employees;
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

        private void LoadEmployeesData(List<Employee> employees)
        {
            dgEmployee.DataContext = employees;
            this.employees = employees;
        }

        private async void GetEmployees()
        {
                lblMessage.Content = "";
            try
            {
                var response = await client.GetStringAsync("users");
                var employees = JsonConvert.DeserializeObject<List<Employee>>(response);

                if(employees is not null )
                {
                this.LoadEmployeesData(employees);
                }
               
            }
            catch (Exception ex)
            {
                lblMessage.Content = ex.Message;
            }
        }

        private async void SaveEmployee(Employee employee)
        {
            try
            {
                await client.PostAsJsonAsync("users", employee);

            }
            catch (Exception ex)
            {
                lblMessage.Content = ex.Message;

            }
        }

        private async void UpdateEmployee(Employee employee)
        {
            try
            {
                await client.PutAsJsonAsync("users/" + employee.id, employee);

            }
            catch (Exception ex)
            {
                lblMessage.Content = ex.Message;

            }
        }

        private async void DeleteEmployee(int employeeId)
        {
            try
            {
                await client.DeleteAsync("users/" + employeeId);
                lblMessage.Content = "Employee Deleted";

            }
            catch (Exception ex)
            {
                lblMessage.Content = "Error deleting employee";

            }
        }

        private void BtnSaveEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var employee = new Employee()
                {
                    id = Convert.ToInt32(txtEmployeeId.Text),
                    name = txtName.Text,
                    email = txtEmail.Text,
                    gender = txtGender.Text,
                    status = txtStatus.Text,
                };

                if (employee.id == 0 && !string.IsNullOrEmpty(employee.name))
                {
                    this.SaveEmployee(employee);
                    lblMessage.Content = "Employee Saved, reload list";

                }
                else if (employee.id != 0)
                {
                    this.UpdateEmployee(employee);
                    lblMessage.Content = "Employee Updated, reload list";

                }


            }
            catch (Exception ex)
            {
                lblMessage.Content = "Error saving employee";

            }

            txtEmployeeId.Text = 0.ToString();
            txtName.Text = "";
            txtEmail.Text = "";
            txtGender.Text = "";
            txtStatus.Text = "";

        }

        void BtnEditEmployee(object sender, RoutedEventArgs e)
        {
            Employee? employee = ((FrameworkElement)sender).DataContext as Employee;
            if (employee != null)
            {
                txtEmployeeId.Text = employee.id.ToString();
                txtName.Text = employee.name;
                txtEmail.Text = employee.email;
                txtGender.Text = employee.gender;
                txtStatus.Text = employee.status;
               
                SetEmployeeData(employee);
            }
              
          

        }

        private void SetEmployeeData(Employee employee)
        {
            txtEmployeeId.Text = employee?.id.ToString();
            txtName.Text = employee?.name;
            txtEmail.Text = employee?.email;
            txtGender.Text = employee?.gender;
            txtStatus.Text = employee?.status;
        }

        void BtnDeleteEmployee(object sender, RoutedEventArgs e)
        {
            Employee? employee = ((FrameworkElement)sender).DataContext as Employee;
            this.DeleteEmployee(employee.id);
            
            this.GetEmployees();
        }

        private void ExportToCSV()
        {
            var builder = new StringBuilder();
            builder.AppendLine("id, name, email, gender, status");
            if (this.employees is null)
            {
                MessageBox.Show("No data have been loaded!, first you need to load your data");

            }
            else
            {
              foreach (var employee in this.employees)
              {
                builder.AppendLine($"{employee.id}, {employee.name}, {employee.email}, {employee.gender}, {employee.status}");
              }

              var csvData = builder.ToString();

              var saveFileDialog = new SaveFileDialog
              {
                Filter = "CSV Files (*.csv)|*.csv",
                FileName = "employees.csv"
              };

              if (saveFileDialog.ShowDialog() == true)
              {
                var filePath = saveFileDialog.FileName;
                File.WriteAllText(filePath, csvData);
                MessageBox.Show("CSV file exported successfully!");
              }
            }
        }

         void ButtonExport_Click(object sender, RoutedEventArgs e)
        {
            this.ExportToCSV();
        }
    }
}
