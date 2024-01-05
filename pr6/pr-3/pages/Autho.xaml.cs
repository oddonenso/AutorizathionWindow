using pr_3.models;
using System;
using System.Collections.Generic;
using System.Linq;
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
using pr_3.pages;
using System.Security.Cryptography;

namespace pr_3.pages
{
    /// <summary>
    /// Логика взаимодействия для Autho.xaml
    /// </summary>
    public partial class Autho : Page
    {
        private string role;

        public Autho()
        {
            InitializeComponent();
            
            tboxCaptcha.Visibility = Visibility.Hidden;
            tblockCaptcha.Visibility = Visibility.Hidden;
            btnCaptcha.Visibility = Visibility.Hidden;
        }

        private void btnEnterGuests_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Client());
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            string login = tbxLogin.Text;
            string rawPassword = pasboxPassword.Password; // для пароля используем Password, а не Text

            RegisterUser(login, rawPassword);
        }
        int countUnsuccessful = 0;
        public static class HashPasswords
        {
            public static string Hash(string password)
            {
                // Создаем объект класса SHA256CryptoServiceProvider
                var sha256 = new SHA256CryptoServiceProvider();

                // Вычисляем хеш пароля
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                // Возвращаем хеш в виде строки
                return BitConverter.ToString(bytes).Replace("-", "");
            }
        }
        private void LoginUser()
        {
            PhotoStudioEntities1 photoStudioEntities = new PhotoStudioEntities1();
            clients user = new clients();
            string Login = tbxLogin.Text;
            string Password = HashPasswords.Hash(pasboxPassword.Password.Replace("\"", ""));

           user = photoStudioEntities.clients.Where(p =>  p.login == Login).FirstOrDefault();
            if (countUnsuccessful < 1)
            {
                if (user != null)
                {
                    if (user.password == Password)
                    {
                        LoadForm(photoStudioEntities.clients.ToString());
                        tbxLogin.Text = "";
                        tboxCaptcha.Text = "";
                        MessageBox.Show("вы вошли под логином: " + photoStudioEntities.clients.ToString());
                    }
                    else
                    {
                        MessageBox.Show("неверный пароль");
                        GenerateCaptcha();
                        countUnsuccessful++;
                    }
                }
                else
                {
                    MessageBox.Show("пользователя с логином '" + Login + "' не существует");
                }
            }
        }
        private void RegisterUser(string login, string rawPassword)
        {
            using (var photoStudioEntities = new PhotoStudioEntities1())
            {
                var user = new clients
                {
                    login = login, // Устанавливаем значение логина
                    password = HashPassword(rawPassword) // Хешируем пароль и устанавливаем хеш
                                                         // Другие свойства пользователя (имя, фамилия, телефон и т.д.)
                };

                photoStudioEntities.clients.Add(user); // Добавляем пользователя в контекст
                photoStudioEntities.SaveChanges(); // Сохраняем изменения в базе данных
            }
        }
        private void LoadForm(string _role)
        {
            switch (_role)
            {
                //клиент -- посмотреть свои данные и обьекты 
                case "1":
                    NavigationService.Navigate(new Client());
                    break;    
                case "2":
                    NavigationService.Navigate(new employee());
                    break;
            }
        }


        private void GenerateCaptcha()
        {
            tboxCaptcha.Visibility = Visibility.Visible;
            tblockCaptcha.Visibility = Visibility.Visible;
            btnCaptcha.Visibility = Visibility.Visible;

            Random rdm = new Random();
            int rndNum = rdm.Next(1, 53);
            tboxCaptcha.Text = rndNum.ToString();
        }
        private void btnEnterGuests_Click_1(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnEnter_Click_1(object sender, RoutedEventArgs e)
        {
            if (role == "client")
            {
                NavigationService.Navigate(new Client());
            }
            else if (role == "employee")
            {
                NavigationService.Navigate(new employee());
            }
            else
            {
                // Handle invalid role
                MessageBox.Show("Invalid role");
            }
        }
     private string HashPassword(string password)
{
    using (var sha256 = System.Security.Cryptography.SHA256.Create())
    {
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }
}

        private void btnCaptcha_Click(object sender, RoutedEventArgs e)
        {
            if (countUnsuccessful >= 1)
            {
                if (tboxCaptcha.Text != tblockCaptcha.Text)
                {
                    MessageBox.Show("Неверная капча");
                    GenerateCaptcha();
                }
                if (tboxCaptcha.Text == tblockCaptcha.Text)
                {
                    countUnsuccessful = 0;
                    MessageBox.Show("капча введена правильно");
                    tboxCaptcha.Visibility = Visibility.Hidden;
                    tblockCaptcha.Visibility = Visibility.Hidden;
                    btnCaptcha.Visibility = Visibility.Hidden;
                }
            }
        }

        private void btnSign_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Registration());
        }
    }
    }

