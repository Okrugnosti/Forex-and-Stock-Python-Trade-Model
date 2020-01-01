using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

// данный код позволяет создавать структуры данных, инициализировать их и передавать в качестве исходных данных другим методам
// недостаток данной decimal в том, что в случае дополнения структуры новыми данными, придестя приводить в порядок не только блок
// где объявляется структура, но и каждый медот, который создает вариацию исходных данных


namespace R10
{
    // СТРУКТУРА НАСТРОЕК:
    public struct Parametrs // объявляем СТРУКТУРУ данных
    {
        // 1_File_Work.cs
        public string[] General_Directory; // корневая директория с EXE файлом

        public string File_ExPort; // путь к файлу с историческими котировками
        // *формат данных: SPFB.RTS,30,20120525,100000,127160.00000,127980.00000,126300.00000,127770.00000

        public string File_InPORT; // путь к файлу, куда заносятся результаты бэк теста

        public char RAZDEL;// *разделить разрядов на разных версиях разный. есть '.' есть ','

        // работа с Access
        public int Start_Market; // переменная для хранения данных по началу торгов в секундах
        public int Taym_Freym; //торговый таймфрейм выраженный в секундах
        public string Access; // путь к базе данных с котировками
        public string Baza; // переменная хранит название таблицы с котировками из базы данных Access

        // работа с екселем
        public string Excel_Path; // путь к файлу с екселем
        public string list; // название листа в екселе
        public string zagolovok_visota_kanala; //
        public string zagolovok_stolbca; //
        public int N_stroks; // количество срок в блоке выводы данных 
        public int stroka;  // номер строки с которой начинаем вывод 
        public int stolbec; // номер столбца с которого начинаем вывод


        // 2_Molecula_Volatiliti.cs

        public int sdvig_korrelacii; // переменная которая сдвигает i на опережающий интервал времени. Применяется для корретировки корреляции волатильности и просадок
        public decimal Filter_Volatiliti; // фильтра волатильности, предназначен для расчет циклов волатильности

        // public int SR_V; // задаем параметр усреднения для расчета средней волатильности за одни интервал
        // public int V_BPC_V; //задает размер архива волатильности для анализа экстремумов
        // public int V_BP_V; // просто задает размер массивов. они должны быть одинаковыми в двух последних методах


        // 3_Molecula_Prosadoc.cs

        public int sdvig_prosadok; // переменная которая сдвигает i на опережающий интервал времени. Применяется для корретировки корреляции волатильности и просадок
        public decimal Filter_Prosadok; // фильтра волатильности, предназначен для расчет циклов волатильности

        // public int SR_P; // задаем параметр усреднения для расчета средней просадки за одни интервал
        // public int V_BPC_P; //задает размер архива просадок для анализа экстремумов
        // public int V_BP_P; // просто задает размер массивов. они должны быть одинаковыми в двух последних методах


        // 5_Class_RENKO_5_min_850.cs

        public decimal Visota_Kanala; // устанавливаем высоту канала (пункты)
        public decimal Nachalo_Otsheta; // устанавливаем начальную точку отсчета (пункты)
        public decimal H_Stop; // высота стопа (в кубиках)
        public decimal Filter_Stop; // ценовой фильтр между стопами (в кубиках)
        public decimal Max_Drogdawn_R01; // расчет максимальной просадки
        public decimal Proskalzivania; //проскальзывания в сделках

        // 6_Class_Raschet_Protiv_RENKO_5_850.cs

        public decimal Delta_hMAX; // увеличивает отрицательную высоту размаха
        public decimal Delta_hMIN; // увеличивает положительную глубину погружения
        public int Cikl; // количество обращений вокруг точки входа

        public decimal Max_Drogdawn_xR01; // расчет максимальной просадки
        

        // 7_Class_Capital_System.cs
        
        public int Positions_R01;
        public int Positions_xR01;

        // 7_Reconvertot_Time.cs

        // 8_API.cs

        // 8_Consructor_Transactiy.cs - класс с параметрами объявляется в нутри тела. поэтому нужно не забывать и там переименовывать

        public string QUIK_PATH;
        public string ACCOUNT;
        public string CLASSCODE;
        public string CLIENT_CODE;
        public string SECCODE;
        public string TRANS_ID; // это номер транзакции по которому будут сниматься заявки имено нашего робота. 

        // 9_Analiz_Dohod_Sistem.cs

        public decimal Price_SHAG_CENA; // стоимость шага цены
        public decimal Max_Stavka_Riska; // максимальный убыток в % от начального капитала

    }

    // ВАРИАЦИИ: #1 Si_1min_30
    public class Parametrs_Si_1min_30 // ВАРИАЦИЯ 1 СТРУКТУРЫ Parametrs
    {
        public Parametrs Inicializaciz() // метод возвращает готовую структуру
        {
            Parametrs Struct = new Parametrs(); // создается копия структуры данных типа Parametrs

            // БЛОК ПРИСВОЕНИЯ ЗНАЧЕНИЙ

            // 1_File_Work.cs
            // string [] General_Directory = Directory.GetCurrentDirectory().Split(new char[] { '\\' }); // корневая директория с EXE файлом

            Struct.File_ExPort = @"\InvestProjekt\VisualStudio8.0\#R02\R10\3_Reestr\St_SBER_1min\ExPORT_Si_1min.txt"; // путь к файлу с историческими котировками
            // *формат данных: SPFB.RTS,30,20120525,100000,127160.00000,127980.00000,126300.00000,127770.00000

            Struct.File_InPORT = @"\InvestProjekt\VisualStudio8.0\#R02\R10\3_Reestr\St_SBER_1min\InPORT_Si.txt"; // путь к файлу, куда заносятся результаты бэк теста

            Struct.RAZDEL = ',';// *разделить разрядов на разных версиях разный. есть '.' есть ','

            // работа с Access
            Struct.Start_Market = 36000; // переменная для хранения данных по началу торгов в секундах
            Struct.Taym_Freym = 60; //торговый таймфрейм выраженный в секундах
            Struct.Access = @"\InvestProjekt\VisualStudio8.0\#R02\R10\QUOTE.mdb"; // путь к базе данных с котировками
            Struct.Baza = "Si"; // переменная хранит название таблицы с котировками из базы данных Access

            // работа с екселем
            Struct.Excel_Path = @"\InvestProjekt\VisualStudio8.0\#R02\R10\SSV.xls"; // путь к файлу с екселем
            Struct.list = "1_min_30"; // название листа в екселе
            Struct.zagolovok_visota_kanala = "0.3"; //
            Struct.zagolovok_stolbca = "1_min_0.3"; //
            Struct.stroka = 1; // номер строки с которой начинаем вывод
            Struct.N_stroks = 30; // количество срок в блоке выводы данных
            // Struct.stolbec = 1; // номер столбца с которого начинаем вывод

            // 2_Molecula_Volatiliti.cs

            Struct.sdvig_korrelacii = 75000; // переменная которая сдвигает i на опережающий интервал времени. Применяется для корретировки корреляции волатильности и просадок
            Struct.Filter_Volatiliti = 10; // фильтра волатильности, предназначен для расчет циклов волатильности

            // Struct.SR_V = 840; // задаем параметр усреднения для расчета средней волатильности за одни интервал
            // Struct.V_BPC_V = 3200; //задает размер архива волатильности для анализа экстремумов
            // Struct.V_BP_V = 3200; //просто задает размер массивов. они должны быть одинаковыми в двух последних методах


            // 3_Molecula_Prosadoc.cs

            Struct.sdvig_prosadok = 1; // переменная которая сдвигает i на опережающий интервал времени. Применяется для корретировки корреляции волатильности и просадок
            Struct.Filter_Prosadok = 5; // фильтра волатильности, предназначен для расчет циклов волатильности

            // Struct.SR_P = 840; // задаем параметр усреднения для расчета средней просадки за одни интервал
            // Struct.V_BPC_P = 300; //задает размер архива просадок для анализа экстремумов
            // Struct.V_BP_P = 3200; // просто задает размер массивов. они должны быть одинаковыми в двух последних методах


            // 5_Class_RENKO_5_min_850.cs

            Struct.Visota_Kanala = 0.2m; // устанавливаем высоту канала (пункты)
            Struct.Nachalo_Otsheta = 0.2m; // устанавливаем начальную точку отсчета (пункты)
            Struct.H_Stop = 2; // высота стопа (в кубиках)
            Struct.Filter_Stop = 0; // ценовой фильтр между стопами (в кубиках)
            Struct.Max_Drogdawn_R01 = 0.1m; // расчет максимальной просадки
            Struct.Proskalzivania = 0.001m; //проскальзывания в сделках

            // 6_Class_Raschet_Protiv_RENKO_5_850.cs

            Struct.Delta_hMAX = 2; // увеличивает отрицательную высоту размаха
            Struct.Delta_hMIN = 1; // увеличивает положительную глубину погружения
            Struct.Cikl = 10; // количество обращений вокруг точки входа

            Struct.Max_Drogdawn_xR01 = 0.1m; // расчет максимальной просадки


            // 7_Class_Capital_System.cs
            Struct.Positions_R01 = 1;
            Struct.Positions_xR01 = 1;

            // 7_Reconvertot_Time.cs

            // 8_API.cs

            // 8_Consructor_Transactiy.cs - класс с параметрами объявляется в нутри тела. поэтому нужно не забывать и там переименовывать

            Struct.QUIK_PATH = @"\InvestProjekt\VisualStudio8.0\QUIK\QUIK - KIT-FinanceRU";
            Struct.ACCOUNT = "15st392";
            Struct.CLASSCODE = "RTSST";
            Struct.CLIENT_CODE = "xR01";
            Struct.SECCODE = "SBER";
            Struct.TRANS_ID = "34"; // это номер транзакции по которому будут сниматься заявки имено нашего робота. 

            // 9_Analiz_Dohod_Sistem.cs

            Struct.Price_SHAG_CENA = 1; // стоимость шага цены
            Struct.Max_Stavka_Riska = 30; // максимальный убыток в % от начального капитала

            return Struct; // возвращаем готовую структуру
        }
    }

    // ВАРИАЦИИ: #2 Si_1min_60
    public class Parametrs_Si_1min_60 // ВАРИАЦИЯ 1 СТРУКТУРЫ Parametrs
    {
        public Parametrs Inicializaciz() // метод возвращает готовую структуру
        {
            Parametrs Struct = new Parametrs(); // создается копия структуры данных типа Parametrs

            // БЛОК ПРИСВОЕНИЯ ЗНАЧЕНИЙ

            // 1_File_Work.cs
            // string [] General_Directory = Directory.GetCurrentDirectory().Split(new char[] { '\\' }); // корневая директория с EXE файлом

            Struct.File_ExPort = @"\InvestProjekt\VisualStudio8.0\#R02\R10\3_Reestr\Si_1min_60\ExPORT_Si_1min.txt"; // путь к файлу с историческими котировками
            // *формат данных: SPFB.RTS,30,20120525,100000,127160.00000,127980.00000,126300.00000,127770.00000

            Struct.File_InPORT = @"\InvestProjekt\VisualStudio8.0\#R02\R10\3_Reestr\Si_1min\InPORT_Si.txt"; // путь к файлу, куда заносятся результаты бэк теста

            Struct.RAZDEL = '.';// *разделить разрядов на разных версиях разный. есть '.' есть ','

            // работа с Access
            Struct.Start_Market = 36000; // переменная для хранения данных по началу торгов в секундах
            Struct.Taym_Freym = 60; //торговый таймфрейм выраженный в секундах
            Struct.Access = @"\InvestProjekt\VisualStudio8.0\#R02\R10\QUOTE.mdb"; // путь к базе данных с котировками
            Struct.Baza = "Si"; // переменная хранит название таблицы с котировками из базы данных Access

            // работа с екселем
            Struct.Excel_Path = @"\InvestProjekt\VisualStudio8.0\#R02\R10\SSV.xls"; // путь к файлу с екселем
            Struct.list = "1_min_60"; // название листа в екселе
            Struct.zagolovok_visota_kanala = "60"; //
            Struct.zagolovok_stolbca = "1_min_60"; //
            Struct.stroka = 1; // номер строки с которой начинаем вывод
            Struct.N_stroks = 30; // количество срок в блоке выводы данных
            // Struct.stolbec = 1; // номер столбца с которого начинаем вывод

            // 2_Molecula_Volatiliti.cs

            Struct.sdvig_korrelacii = 75000; // переменная которая сдвигает i на опережающий интервал времени. Применяется для корретировки корреляции волатильности и просадок
            Struct.Filter_Volatiliti = 10; // фильтра волатильности, предназначен для расчет циклов волатильности

            // Struct.SR_V = 840; // задаем параметр усреднения для расчета средней волатильности за одни интервал
            // Struct.V_BPC_V = 3200; //задает размер архива волатильности для анализа экстремумов
            // Struct.V_BP_V = 3200; //просто задает размер массивов. они должны быть одинаковыми в двух последних методах


            // 3_Molecula_Prosadoc.cs

            Struct.sdvig_prosadok = 1; // переменная которая сдвигает i на опережающий интервал времени. Применяется для корретировки корреляции волатильности и просадок
            Struct.Filter_Prosadok = 5; // фильтра волатильности, предназначен для расчет циклов волатильности

            // Struct.SR_P = 840; // задаем параметр усреднения для расчета средней просадки за одни интервал
            // Struct.V_BPC_P = 300; //задает размер архива просадок для анализа экстремумов
            // Struct.V_BP_P = 3200; // просто задает размер массивов. они должны быть одинаковыми в двух последних методах


            // 5_Class_RENKO_5_min_850.cs

            Struct.Visota_Kanala = 30; // устанавливаем высоту канала (пункты)
            Struct.Nachalo_Otsheta = 40; // устанавливаем начальную точку отсчета (пункты)
            Struct.H_Stop = 2; // высота стопа (в кубиках)
            Struct.Filter_Stop = 0; // ценовой фильтр между стопами (в кубиках)
            Struct.Max_Drogdawn_R01 = 0.1m; // расчет максимальной просадки
            Struct.Proskalzivania = 0.001m; //проскальзывания в сделках

            // 6_Class_Raschet_Protiv_RENKO_5_850.cs

            Struct.Delta_hMAX = 2; // увеличивает отрицательную высоту размаха
            Struct.Delta_hMIN = 1; // увеличивает положительную глубину погружения
            Struct.Cikl = 10; // количество обращений вокруг точки входа

            Struct.Max_Drogdawn_xR01 = 0.1m; // расчет максимальной просадки


            // 7_Class_Capital_System.cs
            
            Struct.Positions_R01 = 1;
            Struct.Positions_xR01 = 1;

            // 7_Reconvertot_Time.cs

            // 8_API.cs

            // 8_Consructor_Transactiy.cs - класс с параметрами объявляется в нутри тела. поэтому нужно не забывать и там переименовывать

            Struct.QUIK_PATH = @"\InvestProjekt\VisualStudio8.0\QUIK\QUIK - KIT-FinanceRU\";
            Struct.ACCOUNT = "15003bf";
            Struct.CLASSCODE = "SPBFUT";
            Struct.CLIENT_CODE = "xR01";
            Struct.SECCODE = "SiZ2";
            Struct.TRANS_ID = "34"; // это номер транзакции по которому будут сниматься заявки имено нашего робота. 

            // 9_Analiz_Dohod_Sistem.cs

            Struct.Price_SHAG_CENA = 1; // стоимость шага цены
            Struct.Max_Stavka_Riska = 30; // максимальный убыток в % от начального капитала

            return Struct; // возвращаем готовую структуру
        }
    }
}


namespace R10
{

    // СТРУКТУРА СТАВОК:
    public struct Stavka // объявляем СТРУКТУРУ данных
    {
        // Ставка плановой позиции на систему

        // MICEX
        
        // FORTS

        // Standart
        public int ST_SBER_1min_032; // ставка на систему
        
    }

    // ВАРИАЦИИ: #1 Si_1min_30
    public class Parametrs_Stavka // ВАРИАЦИЯ 1 СТРУКТУРЫ Parametrs
    {
        public Stavka Inicializaciz() // метод возвращает готовую структуру
        {
            Stavka Struct = new Stavka(); // создается копия структуры данных типа Parametrs

            // Ставка плановой позиции на систему

            // MICEX
        
            // FORTS

            // Standart
            Struct.ST_SBER_1min_032 = 1; // ставка на систему

            return Struct; // возвращаем готовую структуру
        }
    }

}