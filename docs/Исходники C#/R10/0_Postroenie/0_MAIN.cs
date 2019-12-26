using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.IO; // IO
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb; // ODBC
using System.Runtime.InteropServices; // API 
using R10; //  создаем ссылку на пространство имен в котором записаны все наши прикрепленные классы

/* ИНСТРУКЦИЯ ПО ЗАПУСКУ ТОРГОВОЙ СИСТЕМЫ
 * 
 * 1. необходимо объявить ВАРИАЦЮ системы с блоком управляющих параметров
 * 2. Каждому параметру необходимо присвоить уникальное имя = вариации системы
 * название параметра = [инструмент][таймфрейм][высота канала]
 * 3. Parametrs Si_1min_30 = new Parametrs(); создается набор параметров системы
 * 4. В классе 0_Parametrs.cs необходимо создать новые параметры для торговой системы
 * 
 * 5. При заполнении параметров нужно определиться, данная вариацию будет потреблять котировки из файлов
 * которые уже применяются для ранее объявленной вариации или нет.
 * 6. Если мы объявляем новую вариацию с новым типом котировок, в таком случае метод
 * Inicialisacion_Massiv(index, index_Dubler, ref index_end_file, ref Chislo_out)
 * должен сожержать данные из своей вариации
 * а index_Dubler = 0
 * в классе с параетрами необходимо прописать путь к файлам с котировками
 * 
 * 7. Если мы используем котировки из вышеобъявленных вариаций тогда в методе
 * Inicialisacion_Massiv(index, index_Dubler, ref index_end_file, ref Chislo_out)
 * index_end_file и ref Chislo_out должны стоять той вариации, котировки которой мы используем
 * а index_Dubler = 1
 * 
 * 8. В блок if (indexR01 == Index_Stadii & indexR02 == Index_Stadii) необходимо добавть и параметры нашей вариации
 * 
 * 9. не забываем в QUIK настроить экспорт ODBC 
 * 
 * ПРИМЕР НАСТРОЙКИ СИСТЕМЫ
 * 
 * static void Main(string[] args)
        {

            TimeSpan Point1, Point2, Point3; // точки засекания времени
            int Index_Senhronizacii = 1; // индекс синхронизации стадий прохождения портфеля торговых систем
            int Index_Stadii = Index_Senhronizacii + 1; // индекс стадий торговых систем
            // всего 6 стадий, соответствующих классу Robot


            
            // ОБЪЯВЛЕНИЕ ВАРИАЦИЙ #1 Si_1min_30
            Parametrs Si_1min_30 = new Parametrs(); // создаем экземпляр структуры типа Parametrs
            Parametrs_Si_1min_30 Var_Si_1min_30 = new Parametrs_Si_1min_30(); // объявляем класс по заполнению экземпляра структуры Var_Si_1min_30 с типом данных Parametrs
            Si_1min_30 = Var_Si_1min_30.Inicializaciz(); // присваиваем данные экземпляру Si_1min_30 

            Class_Robot R01 = new Class_Robot(); // создаем клон робота с парамерами Si_1min_30
            int indexR01 = 1; // переменная стадий работы робота Si_1min_30
            decimal[] Chislo_out_Si_1min_30 = new decimal[10]; // массив дублирования считанных котировок
            int index_Dubler_Si_1min_30 = 0; // индекс дублирования котировок (0-нет дублирования, 1 - дублирует).
            int index_end_file_Si_1min_30 = 0; // индекс конца текстового файла
            
            

            // ОБЪЯВЛЕНИЕ ВАРИАЦИЙ #2 Si_1min_60
            Parametrs Si_1min_60 = new Parametrs(); // создаем экземпляр структуры типа Parametrs
            Parametrs_Si_1min_60 Var_Si_1min_60 = new Parametrs_Si_1min_60(); // объявляем класс по заполнению экземпляра структуры Var_Si_1min_30 с типом данных Parametrs
            Si_1min_60 = Var_Si_1min_60.Inicializaciz(); // присваиваем данные экземпляру Si_1min_30

            Class_Robot R02 = new Class_Robot(); // создаем клон робота с парамерами Si_1min_30
            int indexR02 = 1; // переменная стадий работы робота Si_1min_30
            
            decimal[] Chislo_out_Si_1min_60 = new decimal[10]; // массив дублирования считанных котировок
            int index_Dubler_Si_1min_60 = 1; // индекс дублирования котировок (0-нет дублирования, 1 - дублирует).
            
            int index_end_file_Si_1min_60 = 0; // индекс конца текстового файла




            // ЗАПУСКАЕМ РОБОТОВ
            Point1 = DateTime.Now.TimeOfDay;

            while (Index_Senhronizacii != 5)
            {

                if (Index_Senhronizacii != Index_Stadii)
                {
                    indexR01 = R01.Inicialisacion_Massiv(indexR01, index_Dubler_Si_1min_30, ref index_end_file_Si_1min_30, ref Chislo_out_Si_1min_30, Si_1min_30); // запускаем версию робота #Si_1min_30
                    indexR02 = R02.Inicialisacion_Massiv(indexR02, index_Dubler_Si_1min_60, ref index_end_file_Si_1min_30, ref Chislo_out_Si_1min_30, Si_1min_60); // запускаем версию робота #Si_1min_60
                }

                if (indexR01 == Index_Stadii & indexR02 == Index_Stadii) // когда все портфели торговых систем сравняются в одной стадии, переходим на новую стадию
                {
                    Index_Senhronizacii++;
                    Index_Stadii++;
                }

                Point2 = DateTime.Now.TimeOfDay;
                Point3 = Point2 - Point1;
                Console.Clear(); // очищаем экран
                Console.WriteLine("Время в работе:" + " " + Point3);

            }

            Point2 = DateTime.Now.TimeOfDay;
            Point3 = Point2 - Point1;
            Console.Clear();
            Console.WriteLine("Прогрузка файла с настройками завершена:" + " " + Point3);

        }
    }
*/

namespace R09
{
    class R08
    {
        
        // ОДНА ВАРИАЦИЯ
        static void Main(string[] args)
        {

            /// **** ГЛОБАЛЬНЫЕ ПЕРЕМЕННЫЕ ***  ///
            
            TimeSpan Point1, Point2, Point3; // точки засекания времени
            int Index_Senhronizacii = 1; // индекс синхронизации стадий прохождения портфеля торговых систем
            int Index_Stadii = Index_Senhronizacii + 1; // индекс стадий торговых систем
            // всего 6 стадий, соответствующих классу Robot

            // СТАВКИ НА СИСТЕМУ
            Stavka Stavka = new Stavka(); // создаем экземпляр структуры типа Parametrs
            Parametrs_Stavka St = new Parametrs_Stavka(); // объявляем класс по заполнению экземпляра
            Stavka = St.Inicializaciz(); // присваиваем данные экземпляру Stavka




            /// **** ВАРИАЦИИ ***  ///

            // ОБЪЯВЛЕНИЕ ВАРИАЦИЙ #1 Si_1min_30
            Parametrs Si_1min_30 = new Parametrs(); // создаем экземпляр структуры типа Parametrs
            Parametrs_Si_1min_30 Var_Si_1min_30 = new Parametrs_Si_1min_30(); // объявляем класс по заполнению экземпляра структуры Var_Si_1min_30 с типом данных Parametrs
            Si_1min_30 = Var_Si_1min_30.Inicializaciz(); // присваиваем данные экземпляру Si_1min_30 

            Class_Robot R01 = new Class_Robot(); // создаем клон робота с парамерами Si_1min_30
            int indexR01 = 1; // переменная стадий работы робота Si_1min_30
            decimal[] Chislo_out_Si_1min_30 = new decimal[10]; // массив дублирования считанных котировок
            int index_Dubler_Si_1min_30 = 0; // индекс дублирования котировок (0-нет дублирования, 1 - дублирует).
            int index_end_file_Si_1min_30 = 0; // индекс конца текстового файла




            /// **** КОД ЗАПУСКА ***  ///

            // ЗАПУСКАЕМ РОБОТОВ
            Point1 = DateTime.Now.TimeOfDay;

            while (Index_Senhronizacii != 6)
            {

                if (Index_Senhronizacii != Index_Stadii)
                {
                    indexR01 = R01.Inicialisacion_Massiv(indexR01, index_Dubler_Si_1min_30, ref index_end_file_Si_1min_30, ref Chislo_out_Si_1min_30, Si_1min_30, Stavka.ST_SBER_1min_032); // запускаем версию робота #Si_1min_30
                }

                if (indexR01 == Index_Stadii) // когда все портфели торговых систем сравняются в одной стадии, переходим на новую стадию
                {
                    Index_Senhronizacii++;
                    Index_Stadii++;
                }

                Point2 = DateTime.Now.TimeOfDay;
                Point3 = Point2 - Point1;
                Console.Clear(); // очищаем экран
                Console.WriteLine("Время в работе:" + " " + Point3);

            }

            Point2 = DateTime.Now.TimeOfDay;
            Point3 = Point2 - Point1;
            Console.Clear();
            Console.WriteLine("Прогрузка файла с настройками завершена:" + " " + Point3);

        }
        
        // НЕСКОЛЬКО ВАРИАЦИЙ
        //static void Main(string[] args)
        //{

        //    TimeSpan Point1, Point2, Point3; // точки засекания времени
        //    int Index_Senhronizacii = 1; // индекс синхронизации стадий прохождения портфеля торговых систем
        //    int Index_Stadii = Index_Senhronizacii + 1; // индекс стадий торговых систем
        //    // всего 6 стадий, соответствующих классу Robot
            
            
        //    // ОБЪЯВЛЕНИЕ ВАРИАЦИЙ #1 Si_1min_30
        //    Parametrs Si_1min_30 = new Parametrs(); // создаем экземпляр структуры типа Parametrs
        //    Parametrs_Si_1min_30 Var_Si_1min_30 = new Parametrs_Si_1min_30(); // объявляем класс по заполнению экземпляра структуры Var_Si_1min_30 с типом данных Parametrs
        //    Si_1min_30 = Var_Si_1min_30.Inicializaciz(); // присваиваем данные экземпляру Si_1min_30 

        //    Class_Robot R01 = new Class_Robot(); // создаем клон робота с парамерами Si_1min_30
        //    int indexR01 = 1; // переменная стадий работы робота Si_1min_30
        //    decimal[] Chislo_out_Si_1min_30 = new decimal[10]; // массив дублирования считанных котировок
        //    int index_Dubler_Si_1min_30 = 0; // индекс дублирования котировок (0-нет дублирования, 1 - дублирует).
        //    int index_end_file_Si_1min_30 = 0; // индекс конца текстового файла
            
            

        //    // ОБЪЯВЛЕНИЕ ВАРИАЦИЙ #2 Si_1min_60
        //    Parametrs Si_1min_60 = new Parametrs(); // создаем экземпляр структуры типа Parametrs
        //    Parametrs_Si_1min_60 Var_Si_1min_60 = new Parametrs_Si_1min_60(); // объявляем класс по заполнению экземпляра структуры Var_Si_1min_30 с типом данных Parametrs
        //    Si_1min_60 = Var_Si_1min_60.Inicializaciz(); // присваиваем данные экземпляру Si_1min_30

        //    Class_Robot R02 = new Class_Robot(); // создаем клон робота с парамерами Si_1min_30
        //    int indexR02 = 1; // переменная стадий работы робота Si_1min_30
            
        //    decimal[] Chislo_out_Si_1min_60 = new decimal[10]; // массив дублирования считанных котировок
        //    int index_Dubler_Si_1min_60 = 1; // индекс дублирования котировок (0-нет дублирования, 1 - дублирует).
            
        //    int index_end_file_Si_1min_60 = 0; // индекс конца текстового файла




        //    // ЗАПУСКАЕМ РОБОТОВ
        //    Point1 = DateTime.Now.TimeOfDay;

        //    while (Index_Senhronizacii != 5)
        //    {

        //        if (Index_Senhronizacii != Index_Stadii)
        //        {
        //            indexR01 = R01.Inicialisacion_Massiv(indexR01, index_Dubler_Si_1min_30, ref index_end_file_Si_1min_30, ref Chislo_out_Si_1min_30, Si_1min_30); // запускаем версию робота #Si_1min_30
        //            indexR02 = R02.Inicialisacion_Massiv(indexR02, index_Dubler_Si_1min_60, ref index_end_file_Si_1min_30, ref Chislo_out_Si_1min_30, Si_1min_60); // запускаем версию робота #Si_1min_60
        //        }

        //        if (indexR01 == Index_Stadii & indexR02 == Index_Stadii) // когда все портфели торговых систем сравняются в одной стадии, переходим на новую стадию
        //        {
        //            Index_Senhronizacii++;
        //            Index_Stadii++;
        //        }

        //        Point2 = DateTime.Now.TimeOfDay;
        //        Point3 = Point2 - Point1;
        //        Console.Clear(); // очищаем экран
        //        Console.WriteLine("Время в работе:" + " " + Point3);

        //    }

        //    Point2 = DateTime.Now.TimeOfDay;
        //    Point3 = Point2 - Point1;
        //    Console.Clear();
        //    Console.WriteLine("Прогрузка файла с настройками завершена:" + " " + Point3);

        //}
    }
}
