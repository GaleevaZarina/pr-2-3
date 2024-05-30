using System.Diagnostics;
using System.Globalization;
using System.Reflection.Metadata;
using Newtonsoft.Json;
using Npgsql;

namespace RKIS2_3;

public class DatabaseRequests
{
    /// Регистрация пользователя
    /// login - логин нового пользователя
    /// password - пароль нового пользователя
    public static void AddUserInTable(string login, string password)
    {
        var querySql = $"INSERT INTO users (login, password) VALUES ('{login}', '{password}')";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        cmd.ExecuteNonQuery();
        Console.WriteLine("Вы успешно зарегестрировались!");
    }

    
    /// Авторизация пользователя
    /// login - логин пользователя
    /// password - пароль пользователя
    public static void LogInUsers(string login, string password)
    {
        var querySql = "SELECT * FROM users";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            // Проверка на вход в систему
            if (reader[1].ToString() == login && reader[2].ToString() == password)
            {
                Console.WriteLine("Вы зашли в систему!");
                break;
            }
            else
            {
                Console.WriteLine("Неверный логин или пароль");
                break;
            }
        }
        Console.WriteLine();
    }

    
    /// Проверка на существование пользователя
    /// Возвращает string[] paramsUser - массив параметров пользователя (логин и пароль)
    public static string[] CheckUser()
    {
        var querySql = "SELECT * FROM users";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        using var reader = cmd.ExecuteReader();

        string[] paramsUser = new string[2]; // массив параметров пользователя
        while (reader.Read())
        {
            Console.WriteLine("Придумайте логин: ");
            string login = Console.ReadLine();
            Console.WriteLine("Придумайте пароль: ");
            string password = Console.ReadLine();

            if (reader[1].ToString() == login) // Проверка занят ли такой логин
            {
                Console.WriteLine("Такой логин уже занят");
            }
            else if(reader[2].ToString() == password) // Проверка занят ли такой пароль
            {
                Console.WriteLine("Такой пароль уже занят");
            }
            else
            {
                // Если все в порядке, записываем логин и пароль в массив paramsUser
                paramsUser[0] = login;
                paramsUser[1] = password;
                break;
            }

        }
        Console.WriteLine();
        return paramsUser;
    }


    /// Получение id пользователя
    /// Возвращает id пользователя, который авторизовался
    public static int GetId(string login, string password)
    {
        var querySql = "SELECT * FROM users";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        using var reader = cmd.ExecuteReader();
        
        int idUser = -1; // Если пользователь не авторизовался id = -1
        while (reader.Read())
        {
            if (reader[1].ToString() == login && reader[2].ToString() == password)
            {
                idUser = int.Parse(reader[0].ToString()); // Если пользователь найден в системе idUser присваивается соответсвующий id
                break;
            }
            
        }
        Console.WriteLine();
        return idUser;
        
    }
    
    /// Получение списка всех задач пользоателя по его id
    /// id - айди пользователя, который авторизовался
    public static void GetAllTasksTable(int id)
    {
        var querySql = "SELECT * FROM tasks";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            if (int.Parse(reader[4].ToString()) == id) // Поиск всех задач пользователя с данным id
            {
                Console.Write($"№{reader[0]} ---> Название: {reader[1]}\n");
            }
            
        }
        Console.WriteLine();
    }

   
    /// Получение списка задач по выбранной категории для пользователя по его id
    /// input - команда выбора категории списка задач
    /// id - айди пользователя, который авторизовался
    public static void GetTasksTable(int input, int id)
    {
        var querySql = "SELECT * FROM tasks";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        using var reader = cmd.ExecuteReader();
        
        DateTime nowTime = DateTime.Today;
        switch (input)
        {
            // 0 - Список предстоящих задач
            case 0:
                Console.WriteLine("Список предстоящих задач: ");
                while (reader.Read())
                {
                    if (nowTime < DateTime.Parse(reader[3].ToString(), CultureInfo.InvariantCulture) && int.Parse(reader[4].ToString()) == id)
                    {
                        Console.WriteLine($"№{reader[0]} ---> Название: {reader[1]}\n");
                    }
                }
                Console.WriteLine();
                break;
            // 1 - Список прошедших задач
            case 1:
                Console.WriteLine("Список прошедших задач: ");
                while (reader.Read())
                {
                    if (nowTime > DateTime.Parse(reader[3].ToString(), CultureInfo.InvariantCulture) && int.Parse(reader[4].ToString()) == id) //DateTime.ParseExact(reader[i], "dd/MM/yyyy", CultureInfo.InvariantCulture))
                    {
                        Console.WriteLine($"№{reader[0]} ---> Название: {reader[1]}\n");
                    }
                }
                Console.WriteLine();
                break;
            // 2 - Список задач на сегодня
            case 2:
                Console.WriteLine("\nВаши задачи на сегодня:");
                while (reader.Read())
                {
                    if (nowTime.ToString("dd/MM/yyyy").Replace('.', '/') == reader[3].ToString() && int.Parse(reader[4].ToString()) == id)
                    {
                        Console.WriteLine($"№{reader[0]} ---> Название: {reader[1]}\n");
                    }
                }
                Console.WriteLine();
                break;
            // 3 - Список задач на завтра
            case 3:
                while (reader.Read())
                {
                    if (nowTime.AddDays(1).ToString("dd/MM/yyyy").Replace('.', '/') == reader[3].ToString() && int.Parse(reader[4].ToString()) == id)
                    {
                        Console.WriteLine($"№{reader[0]} ---> Название: {reader[1]}\n");
                    }
                }
                Console.WriteLine();
                break;
            // 4 - Список задач на неделю
            case 4:
                while (reader.Read())
                {
                    for (int i = 0; i < 7; i++)
                    {
                        if (nowTime.AddDays(i).ToString("dd/MM/yyyy").Replace('.', '/') == reader[3].ToString() && int.Parse(reader[4].ToString()) == id)
                        {
                            Console.WriteLine($"№{reader[0]} ---> Название: {reader[1]}\n");
                        }
                    }
                }
                Console.WriteLine();
                break;

        }
    }
    
    
    /// Добавление новой задачи для пользователя по его id
    /// title - Название новой задачи
    /// description - описание новой задачи
    /// date - срок выполнения новой задачи
    /// id - айди пользователя, который авторизовался
    public static void AddTaskInTable(string title, string description, string date, int id)
    {
        var querySql = $"INSERT INTO tasks (title, description, date, id_user) VALUES ('{title}', '{description}', '{date}', '{id}')";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        cmd.ExecuteNonQuery();
    }

    
    /// Изменение параметра задачи для пользователя по его id
    /// input - команда выбора изменения параметра задачи
    /// code - номер задачи, которую пользователь хочет изменить
    /// id - айди пользователя, который авторизовался
    public static void EditionTaskInTable(int input, int code, int id)
    {
        var querySql = "SELECT * FROM tasks";

        if (input == 0) // 0 - Изменить название
        {
            Console.Write("введите новое название: ");
            string newTitle = Console.ReadLine();
            querySql = $"UPDATE tasks SET title = '{newTitle}' WHERE code = '{code}' AND id_user = '{id}'";
        }
        else if (input == 1) // 1 - Изменить описание
        {
            Console.Write("введите новое описание: ");
            string newDescription = Console.ReadLine();
            querySql = $"UPDATE tasks SET description = '{newDescription}' WHERE code = '{code}' AND id_user = '{id}'";
        }
        else if (input == 2) // 2 - Изменить срок выполнения задачи
        {
            Console.Write("введите новый срок выполнения (дд/мм/гггг - 01/01/0001): ");
            string newDate = Console.ReadLine();
            querySql = $"UPDATE tasks SET date = '{newDate}' WHERE code = '{code}' AND id_user = '{id}'";
        }
        
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        cmd.ExecuteNonQuery();
    }
    
    
    /// Удаление задачи пользователя по его id
    /// code - номер задачи, которую пользователь хочет удалить
    /// id - айди пользователя, который авторизовался
    public static void DeletionTaskInTable(int code, int id)
    {
        var querySql = $"DELETE FROM tasks WHERE code = {code} AND id_user = '{id}'";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        cmd.ExecuteNonQuery();
    }

    
    /// Проверка наличия у пользователя задачи с введеным номером
    /// id - айди пользователя, который авторизовался
    public static int CheckTask(int id)
    {
        var querySql = "SELECT * FROM tasks";
        using var cmd = new NpgsqlCommand(querySql, DatabaseService.GetSqlConnection());
        using var reader = cmd.ExecuteReader();

        int code = -1; // Если задачи с таким номером нет в базе, то code = -1
        while (reader.Read())
        {
            Console.Write("Введите номер задачи: ");
            code = int.Parse(Console.ReadLine());
            if (int.Parse(reader[0].ToString()) == code && int.Parse(reader[4].ToString()) == id)
            {
                break;
            }
            else
            {
                Console.WriteLine("У вас нет задачи с таким номером");
            }
        }
        Console.WriteLine();
        return code;
    }

}