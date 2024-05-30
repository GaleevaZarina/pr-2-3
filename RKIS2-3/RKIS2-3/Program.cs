using System;

namespace RKIS2_3
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // Список команд
            Console.WriteLine("\t0 - Войти\n\t" +
                              "1 - Регистрация\n\t");
            
            int input = int.Parse(Console.ReadLine()); // Ввод команды
            int id = -1;

            bool checkLogIn = true; // Переменная для проверки входа в систему
            while (checkLogIn != false)
            {
                string login = "";
                string password = "";
                
                switch (input)
                {
                    case 0: // Вход
                        Console.WriteLine("Введите свой логин: ");
                        login = Console.ReadLine();
                        Console.WriteLine("Введите свой пароль: ");
                        password = Console.ReadLine();
                        DatabaseRequests.LogInUsers(login, password); // Вызов метода авторизации
                        break;
                
                    case 1: // Регистрация
                        string[] paramUser = DatabaseRequests.CheckUser();
                        login = paramUser[0];
                        password = paramUser[1];
                        DatabaseRequests.AddUserInTable(login, password); // Вызов метода регистрации
                        break;
                    
                    default:
                        Console.Write("ТАКОЙ КОМАНДЫ НЕТ -_-");
                        break;
                }
                
                // Проверка входа в систему
                id = DatabaseRequests.GetId(login, password); // Получени айди пользователя
                if (id != -1)
                {
                    checkLogIn = false;
                }
                
            }

            bool exit = true; // Перемнная для выхода из программы
            while (exit != false)
            {
                // Список команд
                Console.WriteLine(
                    "\n\t\tКОМАНДЫ:\n\t" +
                    "0 - Посмотреть список всех задач\n\t" +
                    "1 - Посмотреть задачи (предстоящие, прошедшие, на сегодня, завтра, неделю)\n\t" +
                    "2 - Добавить задачу\n\t" +
                    "3 - Редактировать задачу\n\t" +
                    "4 - Удалить задачу\n\t" +
                    "5 - Выход из приложения\n");

                input = int.Parse(Console.ReadLine()); // Ввод команды

                switch (input)
                {
                    case 0: // Просмотр списка всех задач
                        DatabaseRequests.GetAllTasksTable(id);
                        break;
                    case 1: // Просмотр задач по категории
                        ViewingTasks(id);
                        break;
                    case 2:
                        AddTask(id); // Добавление задачи 
                        break;
                    case 3:
                        EditionTask(id); // Изменение задачи
                        break;
                    case 4:
                        DeletionTask(id); // Удаление задачи
                        break;
                    case 5:
                        Console.WriteLine("\n\tХОРОШЕГО ДНЯ! ^_^");
                        exit = false; // Выход из программы
                        break;
                    default:
                        Console.WriteLine("\n\tТАКОЙ КОМАНДЫ НЕТ -_-");
                        break;


                }

            }

        }

        
        /// Просмотр задач по категории
        /// id - айди пользователя, который авторизовался
        public static void ViewingTasks(int id)
        {
            // Список команд выбора списка задач
            Console.WriteLine("\tПосмотреть:\n\t" +
                              "0 - Список предстоящих задач\n\t" +
                              "1 - Список прошедших задач\n\t" +
                              "2 - Список задач на сегодня\n\t" +
                              "3 - Список задач на завтра\n\t" +
                              "4 - Список задач на неделю\n\t" +
                              "5 - Назад\n\t");

            int input = int.Parse(Console.ReadLine()); // ввод команды

            DatabaseRequests.GetTasksTable(input, id); // Получение списка задач
        }

        
        /// Добавление задачи
        /// id - айди пользователя, который авторизовался
        public static void AddTask(int id)
        {
            Console.WriteLine("\n\tСОЗДАНИЕ ЗАДАЧИ...");
            
            Console.Write("Введите название задачи: ");
            string title = Console.ReadLine();
            
            Console.Write("Введите описание задачи: ");
            string description = Console.ReadLine();
            
            Console.Write("Введите срок выполнения задачи (дд/мм/гггг - 01/01/0001): ");
            string date = Console.ReadLine();

            DatabaseRequests.AddTaskInTable(title, description, date, id); // Вызов метода добавления задачи
            
            Console.WriteLine("\n\tЗАДАЧА ДОБАВЛЕНА");
        }

        
        /// Изменение задачи
        /// id - айди пользователя, который авторизовался
        public static void EditionTask(int id)
        {
            int code = DatabaseRequests.CheckTask(id); // Вызов метода проверки на существование задачи с таким номером
            
            // Список команд для изменения
            Console.WriteLine("\t0 - Изменить название\n\t" +
                              "1 - Изменить описание\n\t" +
                              "2 - Изменить срок выполнения задачи\n\t");

            int input = int.Parse(Console.ReadLine()); // Ввод команды

            Console.WriteLine("\n\tРЕДАКТИРОВАНИЕ ЗАДАЧИ...");
            
            DatabaseRequests.EditionTaskInTable(input, code, id); // Вызов метода редактирования задачи
            
            Console.WriteLine("\n\tЗАДАЧА ИЗМЕНЕНА");
        }
        
        
        /// Удаление задачи
        /// id - айди пользователя, который авторизовался
        public static void DeletionTask(int id)
        {
            int code = DatabaseRequests.CheckTask(id); // Вызов метода проверки на существование задачи с таким номером

            Console.WriteLine("\n\tУДАЛЕНИЕ ЗАДАЧИ...");
            
            DatabaseRequests.DeletionTaskInTable(code, id); // Вызов метода удаления задачи
            
            Console.WriteLine("\n\tЗАДАЧА УДАЛЕНА");
        }
        

    }
}