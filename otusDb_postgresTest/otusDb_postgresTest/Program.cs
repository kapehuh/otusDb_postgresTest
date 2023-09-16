using Npgsql;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

internal class Program
{
    const string connectionString = "Host=localhost;Username=postgres;Password=admin;Database=otusDB";
    private static void Main(string[] args)
    {
        int i;
        do
        {
            Console.Out.WriteLine("Введите число:\n" +
            "1 - Заполнить таблицы данными\n" +
            "2 - Вывести содержимое таблиц\n" +
            "3 - Добавить новую запись\n" +
            "4 - Выход\n");
            if (int.TryParse(Console.ReadLine(), out i))
            {
                switch (i)
                {
                    case 1:
                        FillTables();
                        continue;
                    case 2:
                        ReadTables();
                        continue;
                    case 3:
                        InsertRow();
                        continue;
                    case 4:
                        continue;
                    default:
                        Console.Out.WriteLine("Введите число от 1 до 4");
                        continue;
                        //break;
                }
            } 
        } while (i != 4);
        //CreateOtusBD();
    }

    private static void InsertRow()
    {
        int i;
        while (true)
        {
            Console.Out.WriteLine(
            "1 - Добавить новую запись в таблицу продавцов\n" +
            "2 - Добавить новую запись в таблицу покупателей\n" +
            "3 - Добавить новую запись в таблицу заказов\n" +
            "4 - Отмена\n");
            if (int.TryParse(Console.ReadLine(), out i))
            {
                switch (i)
                {
                    case 1:
                        AddNewSeller();
                        continue;
                    case 2:
                        AddNewBuyer();
                        continue;
                    case 3:
                        AddNewOrder();
                        continue;
                    case 4:
                        return;
                    default:
                        return;
                }
            }
        }
    }

    private static void AddNewOrder()
    {
        try
        {
            List<long> tempid = new List<long>(); long i = 0;
            
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var sql = @"SELECT id, name, email FROM public.sellers";
                //using var cmd = new NpgsqlCommand(sql, connection);
                using (var cmd = new NpgsqlCommand(sql, connection))
                {
                    var rdr = cmd.ExecuteReader();
                    Console.WriteLine("Продавцы:");
                    while (rdr.Read())
                    {
                        Console.WriteLine(String.Format("|{0,22}|{1,22}|{2,22}|", rdr.GetValue(0), rdr.GetValue(1), rdr.GetValue(2)));
                        tempid.Add((long)rdr.GetValue(0));
                    }
                    Console.WriteLine("\n");
                }
            }
            Console.Out.WriteLine("Кто продавец?");
            long.TryParse(Console.ReadLine(), out i);
            if (tempid.Contains(i))
            {
                Console.Out.WriteLine("Введите название товара и цену через запятую:\nПример: Карандаш, 100\n");
                string[]? temp = Console.ReadLine().Split(',').ToArray();
                if (temp.Length == 2)
                {
                    using var connection = new NpgsqlConnection(connectionString);
                    connection.Open();
                    var sql = @"
INSERT INTO orders(ordername, sel_id, quantity, price) 
VALUES (:_ordername, :_sel_id, :_quantity, :_price);
";
                    using var cmd = new NpgsqlCommand(sql, connection);
                    var parameters = cmd.Parameters;
                    //parameters.Add(new NpgsqlParameter("_id", "default nextval('sellers_id_seq')"));
                    parameters.Add(new NpgsqlParameter("_ordername", temp[0]));
                    parameters.Add(new NpgsqlParameter("_sel_id", i));
                    parameters.Add(new NpgsqlParameter("_quantity", 1));
                    parameters.Add(new NpgsqlParameter("_price", Convert.ToInt64(temp[1])));
                    var affectedRowsCount = cmd.ExecuteNonQuery().ToString();
                    Console.WriteLine($"Запись успешно добавлена");
                }
                else
                {
                    Console.WriteLine("Ошибка ввода - название и цена через запятую:\nПример: Карандаш, 100\n");
                }
            }
            else
            {
                Console.Out.WriteLine("Продавец с таким id не найден. Повторите попытку.\n");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    // =================================================================
    private static void AddNewSeller()
    {
        Console.Out.WriteLine("Введите имя и email через запятую:\nПример: Алексей Суворов, логин@почта.ru\n");
        string[]? temp = Console.ReadLine().Split(',').ToArray();
        if (temp.Length == 2)
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            var sql = @"
INSERT INTO sellers(name, email) 
VALUES (:_name, :_email);
";
            using var cmd = new NpgsqlCommand(sql, connection);
            var parameters = cmd.Parameters;
            //parameters.Add(new NpgsqlParameter("_id", "default nextval('sellers_id_seq')"));
            parameters.Add(new NpgsqlParameter("_name", temp[0]));
            parameters.Add(new NpgsqlParameter("_email", temp[1]));
            var affectedRowsCount = cmd.ExecuteNonQuery().ToString();
            Console.WriteLine($"Запись успешно добавлена");
        }
        else
        {
            Console.WriteLine("Ошибка ввода - имя и email через запятую:\nПример: Алексей Суворов, логин@почта.ru\n");
        }
    }
    private static void AddNewBuyer()
    {
        Console.Out.WriteLine("Введите имя и email через запятую:\nПример: Алексей Суворов, логин@почта.ru\n");
        string[]? temp = Console.ReadLine().Split(',').ToArray();
        if (temp.Length == 2)
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            var sql = @"
INSERT INTO buyers(name, email) 
VALUES (:_name, :_email);
";
            using var cmd = new NpgsqlCommand(sql, connection);
            var parameters = cmd.Parameters;
            parameters.Add(new NpgsqlParameter("_name", temp[0]));
            parameters.Add(new NpgsqlParameter("_email", temp[1]));
            var affectedRowsCount = cmd.ExecuteNonQuery().ToString();
            Console.WriteLine($"Запись успешно добавлена");
        }
        else
        {
            Console.WriteLine("Ошибка ввода - имя и email через запятую:\nПример: Алексей Суворов, логин@почта.ru\n");
        }
    }
    // =================================================================
    private static void ReadTables()
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            var sql = @"SELECT name, email FROM public.sellers";
            //using var cmd = new NpgsqlCommand(sql, connection);
            using (var cmd = new NpgsqlCommand(sql, connection))
            {
                var rdr = cmd.ExecuteReader();
                Console.WriteLine("Продавцы:");
                while (rdr.Read())
                {
                    Console.WriteLine(String.Format("|{0,22}|{1,22}|", rdr.GetValue(0), rdr.GetValue(1)));
                }
                Console.WriteLine("\n");
            }
        }

        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            var sql2 = @"SELECT name, email FROM public.buyers";
            using var cmd2 = new NpgsqlCommand(sql2, connection);
            var rdr2 = cmd2.ExecuteReader();
            Console.WriteLine("Покупатели:");
            while (rdr2.Read())
            {
                Console.WriteLine(String.Format("|{0,20}|{1,20}|", rdr2.GetValue(0), rdr2.GetValue(1)));
            }
            Console.WriteLine("\n"); cmd2.Dispose();
        }

        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            //var sql3 = @"SELECT name ordername, cust_id, buyer_id FROM public.orders";
            var sql3 = @"SELECT t1.ordername, t2.name, t3.name FROM public.orders AS t1 LEFT JOIN public.sellers AS t2 ON t1.sel_id = t2.id LEFT JOIN public.buyers AS t3 ON t1.buy_id = t3.id";
            using var cmd3 = new NpgsqlCommand(sql3, connection);
            var rdr3 = cmd3.ExecuteReader();
            Console.WriteLine("Заказы:");
            Console.WriteLine(String.Format("|{0,23}|{1,23}|{2,23}|", "Товар", "Продавец", "Покупатель"));
            while (rdr3.Read())
            {
                Console.WriteLine(String.Format("|{0,23}|{1,23}|{2,23}|", rdr3.GetValue(0), rdr3.GetValue(1), rdr3.GetValue(2)));
            }
            Console.WriteLine("\n");
        }
        
    }

    private static void FillTables()
    {
        //bool rslt = default;
        try
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            //    var selectExistStudents = @"SELECT EXISTS (
            //SELECT FROM 
            //    pg_tables
            //WHERE 
            //    schemaname = 'public' AND 
            //    tablename  = 'clients'
            //)";
            var createTables = @"
-- Table: public.sellers
CREATE SEQUENCE IF NOT EXISTS sellers_id_seq;
ALTER SEQUENCE IF EXISTS sellers_id_seq RESTART WITH 1;
CREATE TABLE IF NOT EXISTS public.sellers
(
    id bigint NOT NULL default nextval('sellers_id_seq'),
    name character varying(255) NOT NULL,
    email character varying(255) NOT NULL,
    CONSTRAINT seller_pkey PRIMARY KEY (id)
)
TABLESPACE pg_default;
ALTER TABLE IF EXISTS public.sellers
    OWNER to postgres;

-- Table: public.buyers
CREATE SEQUENCE IF NOT EXISTS buyers_id_seq;
ALTER SEQUENCE IF EXISTS buyers_id_seq RESTART WITH 1;
CREATE TABLE IF NOT EXISTS public.buyers
(
    id bigint NOT NULL default nextval('buyers_id_seq'),
    name character varying(255) COLLATE pg_catalog.default NOT NULL,
    email character varying(255) COLLATE pg_catalog.default NOT NULL,
    CONSTRAINT buyers_pkey PRIMARY KEY (id)
)
TABLESPACE pg_default;
ALTER TABLE IF EXISTS public.buyers
    OWNER to postgres;

-- Table: public.Orders
CREATE SEQUENCE IF NOT EXISTS orders_id_seq;
ALTER SEQUENCE IF EXISTS orders_id_seq RESTART WITH 1;
CREATE TABLE IF NOT EXISTS public.orders
(
    id bigint NOT NULL default nextval('orders_id_seq'),
    orderName character varying(255) COLLATE pg_catalog.default,
    sel_id bigint,
    quantity bigint NOT NULL,
    price bigint NOT NULL,
    buy_id bigint,
    CONSTRAINT orders_pkey PRIMARY KEY (id),
    CONSTRAINT fk_selid FOREIGN KEY (sel_id)
        REFERENCES public.Sellers (id) ON DELETE CASCADE,
    CONSTRAINT fk_buyid FOREIGN KEY (buy_id)
        REFERENCES public.Buyers (id) ON DELETE CASCADE
)
TABLESPACE pg_default;
ALTER TABLE IF EXISTS public.Orders
    OWNER to postgres;
-- adding data
INSERT INTO public.sellers 
    VALUES (nextval('sellers_id_seq'), 'Жадный продавец', 'продаю@дешево.ру'), 
    (nextval('sellers_id_seq'), 'Очень жадный продавец', 'продаю@дешево.ру'), 
    (nextval('sellers_id_seq'), 'Обманщик', 'миллион@затовар.ру'), 
    (nextval('sellers_id_seq'), 'Адекватный продавец', 'цена@заштуку.ру'), 
    (nextval('sellers_id_seq'), 'Продавец-новичек', 'старт@продаж.ру') ON CONFLICT (id) DO NOTHING;
INSERT INTO public.buyers
	VALUES (nextval('buyers_id_seq'), 'Иванов Иван', 'ИИ@авито.ру'), 
	(nextval('buyers_id_seq'), 'Кузнецов Василий', 'КВ@авито.ру'), 
	(nextval('buyers_id_seq'), 'Петров Эдуард', 'ПЭ@авито.ру'), 
	(nextval('buyers_id_seq'), 'Козлов Андрей', 'КА@авито.ру'), 
	(nextval('buyers_id_seq'), 'Сидоров Петр', 'СП@авито.ру') ON CONFLICT (id) DO NOTHING;
INSERT INTO public.orders
	VALUES (default, 'Мотоцикл Honda', '1', '1', '250000', '4'), 
    (default, 'Автомобиль Жигули', '1', '1', '150000', '4'), 
    (default, 'Игральные карты', '2', '1', '150', '1'), 
    (default, 'Кофе Hausbrandt', '3', '1', '650', '2'), 
    (default, 'Велосипед горный', '5', '1', '12000', '5') ON CONFLICT (id) DO NOTHING;
";
            using var cmd = new NpgsqlCommand(createTables, connection);
            cmd.ExecuteNonQuery().ToString();
            Console.WriteLine("Таблицы добавлены.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static void CheckOtusDB()
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        var sql = @"SELECT EXISTS (
    SELECT FROM 
        pg_tables
    WHERE 
        schemaname = 'public' AND 
        tablename  = 'clients'
    )";
        using var cmd = new NpgsqlCommand(sql, connection);
        var rdr = cmd.ExecuteReader(); 
        Console.WriteLine($"count: {rdr}");
        //if (rdr.Read())
        //{
        //    bool rslt = (bool)rdr[0];
        //    Console.WriteLine($"count: {rslt}");
        //}
        //var affectedRowsCount = cmd.ExecuteNonQuery().ToString();
    }
}