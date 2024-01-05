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
using System.Windows.Threading;

namespace pr_3.pages
{
    /// <summary>
    /// Логика взаимодействия для Autho.xaml
    /// </summary>
    public partial class Autho : Page
    {
        private string role;
        private int countUnsuccessfulAttempts = 0;  // Обновленное имя переменной
        private DateTime unlockTime;
        private const int maxAttempts = 3;
        private const int lockDurationSeconds = 10;

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
            if (!String.IsNullOrEmpty(tbxLogin.Text) && !String.IsNullOrEmpty(pasboxPassword.Password))
            {
                // Проверка наличия капчи и вызов метода для её проверки
                if (tboxCaptcha.Visibility == Visibility.Visible)
                {
                    if (!CheckCaptcha())
                    {
                        return; // Остановить выполнение метода, так как капча не прошла
                    }
                }

                LoginUser();

                // Продолжить выполнение метода, так как капча не используется или успешно прошла проверку
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль");
            }
        }

        private bool CheckCaptcha()
        {
            
            if (countUnsuccessfulAttempts >= maxAttempts)
            {
                if (tboxCaptcha.Text != tblockCaptcha.Text)
                {
                    MessageBox.Show("Неверная капча");
                    GenerateCaptcha();
                    return false; 
                }
                if (tboxCaptcha.Text == tblockCaptcha.Text)
                {
                    countUnsuccessfulAttempts = 0;
                    MessageBox.Show("Капча введена правильно");
                    tboxCaptcha.Visibility = Visibility.Hidden;
                    tblockCaptcha.Visibility = Visibility.Hidden;
                    btnCaptcha.Visibility = Visibility.Hidden;
                    btnCaptcha.IsEnabled = true; 
                    return true; 
                }
            }

            return true; // Если капча не используется, возвращаем true
        }








        private void LoginUser()
        {
            PhotoStudioEntities1 dbContext = new PhotoStudioEntities1();
            clients user = new clients();
            string Login = tbxLogin.Text;
            string Password = HashPasswords.HashPasswords.Hash(pasboxPassword.Password.Replace("\"", ""));
            user = dbContext.clients.Where(p => p.login == Login).FirstOrDefault();
            if (countUnsuccessfulAttempts < maxAttempts)
            {
                if (user != null)
                {
                    if (user.password == Password)
                    {
                        LoadForm(user.RoleId.ToString());
                        tbxLogin.Text = "";
                        tboxCaptcha.Text = "";
                        MessageBox.Show("вы вошли под логином: " + user.login.ToString());
                        countUnsuccessfulAttempts = 0; // Сбрасываем счетчик при успешной авторизации
                    }
                    else
                    {
                        MessageBox.Show("неверный пароль");
                        countUnsuccessfulAttempts++;

                        if (countUnsuccessfulAttempts == maxAttempts)
                        {
                            // Блокируем элементы управления и устанавливаем время разблокировки
                            LockControls();
                            unlockTime = DateTime.Now.AddSeconds(lockDurationSeconds);
                            UpdateUnlockTimeText();
                            StartUnlockTimer();
                        }

                        // Добавляем вызов генерации капчи при неверном вводе пароля
                        GenerateCaptcha();
                    }
                }
                else
                {
                    MessageBox.Show("пользователя с логином '" + Login + "' не существует");
                }
            }
        }
        private void LockControls()
        {
            tbxLogin.IsEnabled = false;
            pasboxPassword.IsEnabled = false;
            btnEnter.IsEnabled = false;
            btnEnterGuests.IsEnabled = false;
            btnSign.IsEnabled = false;
            btnCaptcha.IsEnabled = false; // Блокировка кнопки капчи
        }

        private void UnlockControls()
        {
            tbxLogin.IsEnabled = true;
            pasboxPassword.IsEnabled = true;
            btnEnter.IsEnabled = true;
            btnEnterGuests.IsEnabled = true;
            btnSign.IsEnabled = true;
            btnCaptcha.IsEnabled = true; // Разблокировка кнопки капчи
        }


        private void UpdateUnlockTimeText()
        {
            TimeSpan remainingTime = unlockTime - DateTime.Now;
            tblockTimerr.Text = $"Осталось: {remainingTime.TotalSeconds:F0} секунд.";
        }

        private void StartUnlockTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (sender, e) =>
            {
                UpdateUnlockTimeText();
                if (DateTime.Now >= unlockTime)
                {
                    UnlockControls();
                    tblockTimerr.Text = string.Empty;
                    timer.Stop();
                }
            };
            timer.Start();
        }



        private void LoadForm(string _role)
        {
            switch (_role)
            {
                //клиент -- посмотреть свои данные и обьекты 
                case "1":
                    NavigationService.Navigate(new Client());
                    break;
                //админ -- умеет все 
                case "2":
                    NavigationService.Navigate(new Admin());
                    break;
                //Сотрудник отдела вневедомственной охраны -- составляет договоры 
                case "3":
                    NavigationService.Navigate(new Employee());
                    break;
            }
        }


        private void GenerateCaptcha()
        {
            tboxCaptcha.Visibility = Visibility.Visible;
            tblockCaptcha.Visibility = Visibility.Visible;
            btnCaptcha.Visibility = Visibility.Visible;
            Random rdm = new Random();
            int rndNum = rdm.Next(1, 3);
            switch (rndNum)
            {
                case 1:
                    tblockCaptcha.Text = "ju2sT8Cds";
                    tblockCaptcha.TextDecorations = TextDecorations.Strikethrough;
                    break;
                case 2:
                    tblockCaptcha.Text = "iNmK2cl";
                    tblockCaptcha.TextDecorations = TextDecorations.Strikethrough;
                    break;
                case 3:
                    tblockCaptcha.Text = "uOozGk95";
                    tblockCaptcha.TextDecorations = TextDecorations.Strikethrough;
                    break;
            }
        }
        private void btnEnterGuests_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnEnter_Click_1(object sender, RoutedEventArgs e)
        {

        }


        private void btnCaptcha_Click(object sender, RoutedEventArgs e)
        {
            if (btnCaptcha.IsEnabled)
            {
                if (countUnsuccessfulAttempts >= 1)
                {
                    if (tboxCaptcha.Text != tblockCaptcha.Text)
                    {
                        MessageBox.Show("Неверная капча");
                        GenerateCaptcha();
                    }
                    if (tboxCaptcha.Text == tblockCaptcha.Text)
                    {
                        countUnsuccessfulAttempts = 0;
                        MessageBox.Show("капча введена правильно");
                        tboxCaptcha.Visibility = Visibility.Hidden;
                        tblockCaptcha.Visibility = Visibility.Hidden;
                        btnCaptcha.Visibility = Visibility.Hidden;
                    }
                }

                // Добавим блокировку кнопки капчи при достижении максимального числа неудачных попыток
                if (countUnsuccessfulAttempts >= maxAttempts)
                {
                    btnCaptcha.IsEnabled = false;
                }
            }
            else
            {
                MessageBox.Show("Кнопка капчи заблокирована.");
            }
        }





        private void btnSign_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Registration());
        }
    }
}
