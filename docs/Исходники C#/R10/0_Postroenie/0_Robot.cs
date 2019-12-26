using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R10
{
    public class Class_Robot
    {

        public int i, i1 = 0; // счетчик цикла
        public int sistem_vector = 0; // вектор который показывает какая система сейчас работает
        public decimal sovmeshen_profit = 0; // значенение совещенной прибыли по управлению капиталом

        TimeSpan Point1; // текущее время
        TimeSpan Point2 = new TimeSpan(10, 00, 00); // время начала торгов
        
        public int Index_Molecula_Volatiliti = 0; // индекс последнего генома в молекуле волатильности
        public int Index_Molecula_Prosadok = 0; // индекс последнего генома в молекуле просадок

        public File_Data x = new File_Data(); // создаем объект класса по импорту данных котироков
        public Class_ODBC_Reader x1 = new Class_ODBC_Reader(); // создает объект класса по ипорту данных их ACCESS базы данных
        public ExPort_File y = new ExPort_File(); // создаем объект класса экспорта данных преобразования
        public ExcelValuesWriter Ex = new ExcelValuesWriter(); // класс по работе с екселем
               
        public Analiz_Dohod_Sistem k0 = new Analiz_Dohod_Sistem(); // класс по анализу результатов сделок (Классическая R01)
        
        public Class_RENKO a1 = new Class_RENKO(); //класс по расчету индикатора системы
             

        public int Inicialisacion_Massiv(int index, int index_Dubler, ref int index_end_file, ref decimal[] Chislo_out, Parametrs Parametr, int Stavka)
        {

            // сюда мы запишем переменную в которой будут хранится путь к настойкам

            int Regim_Rabota; // данный индекс показывает в каком режиме вести работу робота
            //Regim_Rabota = 1 БОЕВОЙ РЕЖИМ
            //Regim_Rabota = 0 ХОЛОСТОЙ РЕЖИМ, модуль выставлея заявок API не вызывается



            //* блок ВВОДА/ВЫВОДА файлового обмена *// 

            if (index == 1) // инициализация массивов
            {

                if (index_Dubler == 0)
                {
                    x.Open_File(Parametr); // открываем поток файла для экспорта данных
                    Chislo_out = x.Chislo_out;

                    x.Read_File(Parametr); // в качестве ценовых параметров прошлого торгового дня используем цены из первой строки
                }
                

                a1.Dano_Renko_5_min_850(Chislo_out, Stavka, Parametr);
                
                index = 2;
                return index;

            }

            if (index == 2)// цикл обработки данных
            {

                if (index_Dubler == 0)
                {

                    if ((x.Read_File(Parametr) != 1))
                    {
                        Chislo_out = x.Chislo_out;
                        index_end_file = 0;
                    }
                    else index_end_file = 1;
                    
                }

                if ((index_end_file == 0))
                {
                    // считываем строку котировок из файла экспорта и расфасовываем котировки для обработки
                    Regim_Rabota = 0; // холостой режим

                    i++; // счетчик цикла

                    Chislo_out[9] = i;
                                        
                    sistem_vector = 1;
                    a1.Renko_5_min_850(Chislo_out, i, Regim_Rabota, sistem_vector, Stavka, Parametr);
                    
                    k0.Metod_Analiz_Dohod_Sistem(a1.Rezerv_Rezultat_Sdelki, Parametr); // анализ результативности систему
                                        
                }
                else
                {
                    index = 3;
                    return index;
                }
            }


            if (index == 3)//*    БЛОК ЗАКРЫТИЯ ПОТОКОВ ДЛЯ ФАЙЛОВОГО ЧТЕНИЯ *//
            {

                Ex.Write_Excel("R01", k0.Reestr, 3, Parametr);  // записываем в ексель бэк тест
                

                if (index_Dubler == 0)
                {
                    x.Close_File(); // закрываем поток ЭКСПОРТИРУЕМОГО файла
                }

                index = 4;
                return index;

            }



            if (index == 4)// работаем с Access
            {   // инициализация массивов

                i1 = i; // очень важная переменная. виртуальное время. с помощью нее определяется работа модуля управления капиталом


                if (index_Dubler == 0)
                {
                    x1.myOleDbConnection = x1.Open_Access(Parametr); // открываем поток базы данных для экспорта данных
                    x1.myOleDbCommand = x1.Open_Command(x1.myOleDbConnection, Parametr);
                }


                Regim_Rabota = 1; // боевой режим
                a1.Renko_5_min_850(Chislo_out, i1, Regim_Rabota, sistem_vector, Stavka, Parametr); // выставляем стопы

                index = 5;
                return index;
            }



            if (index == 5)  // цикл обработки данных
            // считываем строку котировок из базы данных и расфасовываем котировки для обработки
            // пока не наступит время 235000 
            {

                Point1 = DateTime.Now.TimeOfDay; // текущее время

                if (Point1 >= Point2)
                {

                    if (x1.Chislo_out[2] != 235000)
                    {

                        Regim_Rabota = 1; // боевой режим


                        if (index_Dubler == 0) // проверка режима дублирования
                        {

                            x1.Read_Access(x1.myOleDbConnection, x1.myOleDbCommand, Parametr);
                            Chislo_out = x1.Chislo_out;

                        }



                        i++; // счетчик цикла

                        if (Chislo_out[7] == 1)
                        {
                            i1++;
                            Chislo_out[9] = i1;
                        }

                        a1.Renko_5_min_850(Chislo_out, i1, Regim_Rabota, sistem_vector, Stavka, Parametr);



                    }
                    else
                    {
                        index = 6;
                        return index;
                    }

                }

            }


            if (index == 6)//*    БЛОК ЗАКРЫТИЯ ПОТОКОВ  для чтения из базы данных Access*//
            {

                if (index_Dubler == 0)
                {
                    x1.Close_Access(x1.myOleDbConnection); // закрываем базу данных
                }

                index = 7;
                return index;
            }

            return index;

        }

    }
}
