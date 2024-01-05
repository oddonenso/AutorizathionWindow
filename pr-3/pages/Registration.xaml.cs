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
using static pr_3.pages.Autho;

namespace pr_3.pages
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Page
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void btnSign_Click(object sender, RoutedEventArgs e)
        {
            string login = tbxLogin.Text;
            string password = HashPasswords.HashPasswords.Hash(tbxPassword.Text.Replace("\"", ""));
            string name = tbxName.Text;
            string surname = tbxSurname.Text;
            string phone = tbxPhone.Text;
            string otchestvo = tbxOtchestvo.Text;

            if (!String.IsNullOrEmpty(phone) && !String.IsNullOrEmpty(surname) && !String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(login) && !String.IsNullOrEmpty(password))
            {
                if (tbxPassword.Text.Length >= 6) //больше 6 символов
                {
                    if (phone.Length == 18)
                    {
                        if (!CheckUserLoginExists(login)) //существует ли чел с таким ником
                        {
                            SaveUser(login, password, name, surname, phone, otchestvo);
                        }
                        else
                        {
                            MessageBox.Show("Пользователь с таким логином уже существует");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Номер телефона должен иметь формат +9 (999) 999-99-99");
                    }
                }
                else
                {
                    MessageBox.Show("Пароль должен иметь длину не менее 6 символов");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите данные");
            }
        }
        private void SaveUser(string login, string password, string name, string surname, string phone, string otchestvo)
        {
            var dbContext = new PhotoStudioEntities1();
            var user = new clients();
            user.login = login;
            user.password = password;
            user.name = name;
            user.surname = surname;
            user.phone = phone;
            user.otchestvo = otchestvo;
            user.RoleId = 1;

            dbContext.clients.Add(user);
            dbContext.SaveChanges();
            MessageBox.Show("Пользователь успешно зарегистрирован");
        }
        private bool CheckUserLoginExists(string login)
        {
            using (var dbContext = new PhotoStudioEntities1()) //создани е экземпляра
            {
                return dbContext.clients.Where(p => p.login == login).Any(); //проверка сущ ли юзер
            }
        }
    }
}
