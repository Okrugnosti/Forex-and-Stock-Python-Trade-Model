using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using Microsoft.Office.Interop.Excel;
using Exc = Microsoft.Office.Interop.Excel; // http://wladm.narod.ru/C_Sharp/comexcel.html#1
using System.Diagnostics;
using System.Threading; // управление процессами


namespace R10
{
    /* класс для входа в файл с данными, его преобразованиями и вывод готовых результатов для анализа */
    /* на выходе возврашает одну строку разбиую по ячейчам массива */

    public class File_Data // класс чтения данных из файла
    {
        private StreamReader fileReader; // место хранения потока
        // private string FilePath; // место записи пути к файлу
        private string Chislo_in; // место считывания строки из потока
        private string[] MODUS = new string[8]; // массив для разбиения строки на столбцы (первого уровня)
        public decimal[] Chislo_out = new decimal[10]; // массив для записи чисел
        public int index_end_file = 0; // индекс конца текстового файла

        public StreamReader Open_File(Parametrs Parametr) // метод по открытию файла
        {
            string[] General_Directory = Directory.GetCurrentDirectory().Split(new char[] { '\\' }); // корневая директория с EXE файлом
            fileReader = new StreamReader(General_Directory[0] + Parametr.File_ExPort);
            return fileReader;
        }



        // формат данных: SPFB.RTS,30,20120525,100000,127160.00000,127980.00000,126300.00000,127770.00000

        public int Read_File(Parametrs Parametr) // метод чтению строки из файла
        {

            if ((Chislo_in = fileReader.ReadLine()) != null) // считываем строку и записываем ее в переменную с проверкой условия на конец файла
            {

                MODUS = Chislo_in.Split(new char[] { ',' }); // установка разделителя строки на столбцы и запись в массив  

                // формат данных: SPFB.RTS пропускаем

                // формат данных: 30 (таймфрейм)
                Chislo_out[0] = decimal.Parse(MODUS[1]);

                // формат данных: 20120525 (дата)
                Chislo_out[1] = decimal.Parse(MODUS[2]);

                // формат данных: 100000 (время)
                Chislo_out[2] = decimal.Parse(MODUS[3]);

                // формат данных: 127160.00000 (цена открытия)
                Chislo_out[3] = decimal.Parse(MODUS[4].Replace(Parametr.RAZDEL, ','));

                // формат данных: 127980.00000 (цена максимальная)
                Chislo_out[4] = decimal.Parse(MODUS[5].Replace(Parametr.RAZDEL, ','));

                // формат данных: 126300.00000 (цена минимальная)
                Chislo_out[5] = decimal.Parse(MODUS[6].Replace(Parametr.RAZDEL, ','));

                // формат данных: 127770.00000 (цена закрытия)
                Chislo_out[6] = decimal.Parse(MODUS[7].Replace(Parametr.RAZDEL, ','));

                Chislo_out[7] = 1; // подгоняем под формат ODBC вывода

                Chislo_out[8] = Chislo_out[6]; // подгоняем под формат ODBC вывода

                return 0;

            }

            else return 1;


        }


        public void Close_File() // метод по закрытию потока
        {
            fileReader.Close();

        }

    }

    public class Class_ODBC_Reader // класс чтения данных из Access базы данных
    {
        // ************** переменные для конвертации в бары *******************

        // private int Start_Market = 36000; // переменная для хранения данных по началу торгов в секундах           

        // private int Taym_Freym = 300; //торговый таймфрейм выраженный в секундах
        private int Taym_Open; // время начала таймфрейма
        private int Taym_Close; // время окончания таймфрейма

        Reconvertot_Time a1 = new Reconvertot_Time();
        ExPort_File a2 = new ExPort_File();

        // формат данных: 30,20120525,100000,127160.00000,127980.00000,126300.00000,127770.00000

        public decimal[] Chislo_out = new decimal[10]; // формируемая свечка

        // Chislo_out[0] = таймфрейм
        // Chislo_out[1] = дата
        // Chislo_out[2] = время
        // Chislo_out[3] = open
        // Chislo_out[4] = max
        // Chislo_out[5] = min
        // Chislo_out[6] = close
        // Chislo_out[7] = index индекс показывает равно ли время предыдущей свечи
        // Chislo_out[8] = last close
        // Chislo_out[9] = bumaga

        private decimal[] Chislo_out_reserv = new decimal[9]; // резервная свеча для хранения котировок предыдущей свечи


        private int Nomer;  // переменная хранит номер последней считанной сделки
        private decimal Cena;  // переменная хранения последней считанной цены


        // ****************** переменные для работы с базой данной по ODBC ******** 

        private string[] taymer = new string[8]; // массив для разбиения строки на столбцы (первого уровня)
        private int[] taymer_out = new int[8]; // массив для записи чисе


        private string x = "WHERE Index = "; // данная переменная хранит част SQL запроса в базу Access
        public string provider = "provider=Microsoft.Jet.OlEDB.4.0;data source=";
        private int nomer_sdelki = 0; // вводим начальное значение индексатора в таблце (точка отправки вывода)
        private int AAA = 0; // индекс для чтения первого значения из базы
        private string filter; // переменная хранит объединенную часть команды из x и nomer_sdelki
        // private string Baza = "RIU1"; // переменная хранит название таблицы с котировками из базы данных Access
        private string a, b, c, d, e, stroka; // переменные для хранения считываемых данных

        public OleDbDataReader myOleDbDataReader; // создаем объект OleDbDataReader
        public OleDbConnection myOleDbConnection; // создаем объект OleDbDataReader
        public OleDbCommand myOleDbCommand;       // создаем объект OleDbDataReader


        public OleDbConnection Open_Access(Parametrs Parametr) // метод по открытию базы данных
        {

            // Формируем строку с параметрами подключения к файлу базы данных
            string[] General_Directory = Directory.GetCurrentDirectory().Split(new char[] { '\\' }); // корневая директория с EXE файлом
            string connectionString = provider + General_Directory[0] + Parametr.Access;
            //"provider=Microsoft.Jet.OLEDB.4.0;" +
            //"data source=E:\\VisualStudio8.0\\ODBC\\QUOTE.mdb";
            // на операционных системах с х64 разрядной системой не поддерживается провайдер.
            // нужно в компиляторе указывать х86 рядрядную обработку

            // создаем объект OleDbConnection для соединения с Бд и передаем его конструктору строку с параметрами подключения
            OleDbConnection myOleDbConnection = new OleDbConnection(connectionString);

            return myOleDbConnection;
        }

        public OleDbCommand Open_Command(OleDbConnection myOleDbConnection, Parametrs Parametr) // метод по открытию базы данных
        {
            // создаем объект OleDbCommand
            OleDbCommand myOleDbCommand = myOleDbConnection.CreateCommand();

            // открываем соединение с БД с помощью метода Open() объекта OleDbConnection
            myOleDbConnection.Open();

            Taym_Open = Parametr.Start_Market;
            Taym_Close = Taym_Open + Parametr.Taym_Freym;

            return myOleDbCommand;
        }


        public OleDbDataReader Read_Access(OleDbConnection myOleDbConnection, OleDbCommand myOleDbCommand, Parametrs Parametr) // метод по чтению данных из базы      
        {

            //ищем первую строку в нашей базе:
            if (AAA == 0)
            {
                myOleDbCommand.CommandText =
                    "SELECT Top 1 Index " +      // сюда прописываем название столбцов в таблице базы данных(обращаем внимание на пробелы)
                    "FROM " + Parametr.Baza;       // сюда прописывается сортируемое слово
                myOleDbDataReader = myOleDbCommand.ExecuteReader();
                myOleDbDataReader.Read();
                if (myOleDbDataReader.HasRows != false)
                {
                    nomer_sdelki = int.Parse(myOleDbDataReader["Index"].ToString());

                    myOleDbDataReader.Close();// закрываем OleDbDataReader методом Close()
                    AAA++;
                }
            }


            // организуем цикл считывания данных из базы access
            if (nomer_sdelki != 0)
            {


                filter = x + nomer_sdelki.ToString(); // создаем часть запроса на поиск индексируемой строки

                // задаем SQL-запрос к базе данных в свойстве CommandText объекта OleDbCommand
                // Результатом запроса должны быть данные клинета с именем Кто-то

                myOleDbCommand.CommandText =
                "SELECT Nomer, Data, Vrema, Bymaga, Cena " +      // сюда прописываем название столбцов в таблице базы данных(обращаем внимание на пробелы)
                "FROM " + Parametr.Baza + " " +                // сюда прописываем название таблицы в базе данных
                filter;       // сюда прописывается сортируемое слово


                // присваиваем объекту OleDbDataReader и вызываем метод ExecuteReader() для выполнения введенного SQL-запроса

                myOleDbDataReader = myOleDbCommand.ExecuteReader();

                // Читаем строку ответа базы данных с помощью метода Read() объекта OleDbDataReader
                myOleDbDataReader.Read();


                // отображаем результат запроса
                // с проверкой на наличие в таблице данных по номеру индекса

                if (myOleDbDataReader.HasRows != false)
                {
                    // записываем результаты в переменные. 
                    // эти данные служат для вывода в бот. 
                    // точка подключения торгового ядра

                    e = myOleDbDataReader["Bymaga"].ToString();
                    d = myOleDbDataReader["Data"].ToString();
                    a = myOleDbDataReader["Nomer"].ToString();
                    b = myOleDbDataReader["Vrema"].ToString();
                    c = myOleDbDataReader["Cena"].ToString();



                    Nomer = int.Parse(a); //преобразуем номер сделки последней считанной транзакции в число
                    Cena = decimal.Parse(c); // преобразуем цену последней считанной транзакции в число


                    taymer = b.Split(new char[] { ':' }); // установка разделителя строки на столбцы и запись в массив  

                    // массив данных содержит время последней обработанной транзакции
                    taymer_out[0] = int.Parse(taymer[0]);
                    taymer_out[1] = int.Parse(taymer[1]);
                    taymer_out[2] = int.Parse(taymer[2]);
                    taymer_out[3] = taymer_out[0] * 60 * 60 + taymer_out[1] * 60 + taymer_out[2]; // время последней транзакции в секундах



                    //***********************строим свечку********************//
                    // Cena - цена последней транзации
                    // Nomer - номер сделки последней транзакции
                    // taymer_out[3] - время в секундах последней транзакции

                    // Chislo_out[0] = таймфрейм
                    // Chislo_out[1] = дата
                    // Chislo_out[2] = время
                    // Chislo_out[3] = open
                    // Chislo_out[4] = max
                    // Chislo_out[5] = min
                    // Chislo_out[6] = close
                    // Chislo_out[7] = index индекс показывает равно ли время предыдущей свечи
                    // Chislo_out[8] = last close


                    // Start_Market = 36000
                    // Taym_Freym = 300
                    // Taym_Open
                    // Taym_Close

                    if (taymer_out[3] >= Taym_Open & taymer_out[3] < (Taym_Close))
                    {
                        // расчет текущей свечи, заполняем данные

                        Chislo_out[0] = Parametr.Taym_Freym;
                        Chislo_out[1] = a1.Metod_Reconvertor_Data(d);
                        Chislo_out[2] = a1.Metod_Reconvertor_Time(Taym_Open);// преобразуем время в нужный формат


                        if (taymer_out[3] == Taym_Open & Chislo_out[3] == 0)
                        {
                            // случай для пустой свечи
                            Chislo_out[3] = Cena; // рассчитываем цену открытия
                            Chislo_out[4] = Cena;
                            Chislo_out[5] = Cena;
                            Chislo_out[6] = Cena;

                        }
                        else
                        {
                            // случай для непустой свечи
                            if (Cena > Chislo_out[4])
                            {
                                Chislo_out[4] = Cena; // рассчитываем максимум

                            }

                            if (Cena < Chislo_out[5])
                            {
                                Chislo_out[5] = Cena; // рассчитываем минимум

                            }

                            Chislo_out[6] = Cena; // обновляем последнюю сделку
                        }



                    }
                    else // если время таймфрейма для свечки подошло к концу, начинаем строим новую свечу
                    {



                        // сдвигаем тайфрейм пока не найдем нужный интервал

                        while (taymer_out[3] > Taym_Open & taymer_out[3] >= (Taym_Close))
                        {
                            Taym_Open = Taym_Open + Parametr.Taym_Freym;
                            Taym_Close = Taym_Close + Parametr.Taym_Freym;
                        }

                        // !!! нужно обнулить свечу, в нее идут новые данные

                        Chislo_out[2] = a1.Metod_Reconvertor_Time(Taym_Open);
                        Chislo_out[3] = Cena;
                        Chislo_out[4] = Cena;
                        Chislo_out[5] = Cena;
                        Chislo_out[6] = Cena;

                    }


                    // Chislo_out[0] = таймфрейм
                    // Chislo_out[1] = дата
                    // Chislo_out[2] = время
                    // Chislo_out[3] = open
                    // Chislo_out[4] = max
                    // Chislo_out[5] = min
                    // Chislo_out[6] = close
                    // Chisli_out[7] = index индекс показывает равно ли время предыдущей свечи
                    // Chislo_out[8] = last close



                    // записываем резервную свечу

                    Chislo_out[8] = Chislo_out_reserv[6];

                    if (Chislo_out[2] != Chislo_out_reserv[2] & Chislo_out_reserv[2] != 0)
                    {
                        Chislo_out[7] = 1; // если равно еденица, значит свечка сформированна

                        // записываем свечку в файл

                        // Chislo_out[0] = таймфрейм
                        // Chislo_out[1] = дата
                        // Chislo_out[2] = время
                        // Chislo_out[3] = open
                        // Chislo_out[4] = max
                        // Chislo_out[5] = min
                        // Chislo_out[6] = close
                        // Chislo_out[7] = index индекс показывает равно ли время предыдущей свечи
                        // Chislo_out[8] = last close
                        // e = бумага
                        // преобразуем данные в строку вида:
                        // формат данных: SPFB.RTS,30,20120525,100000,127160.00000,127980.00000,126300.00000,127770.00000
                        stroka = e + "," + Chislo_out_reserv[0].ToString() + "," +
                                           Chislo_out_reserv[1].ToString() + "," +
                                           Chislo_out_reserv[2].ToString() + "," +

                                           Chislo_out_reserv[3].ToString() + "," +
                                           Chislo_out_reserv[4].ToString() + "," +
                                           Chislo_out_reserv[5].ToString() + "," +
                                           Chislo_out_reserv[6].ToString();
                        a2.Do_ExPort_File(Parametr, stroka);


                    }
                    else
                    {
                        Chislo_out[7] = 0;

                    }


                    Chislo_out_reserv[0] = Chislo_out[0];
                    Chislo_out_reserv[1] = Chislo_out[1];
                    Chislo_out_reserv[2] = Chislo_out[2];
                    Chislo_out_reserv[3] = Chislo_out[3];
                    Chislo_out_reserv[4] = Chislo_out[4];
                    Chislo_out_reserv[5] = Chislo_out[5];
                    Chislo_out_reserv[6] = Chislo_out[6];
                    Chislo_out_reserv[7] = Chislo_out[7];
                    Chislo_out_reserv[8] = Chislo_out[8];



                    // выводим данные
                    //Console.WriteLine(
                    //    //myOleDbDataReader["Nomer"] + " " + // воводим данные из ячейки в столбце ...
                    //myOleDbDataReader["Vrema"] + " " +  // воводим данные из ячейки в столбце ...
                    //myOleDbDataReader["Cena"] + " " + // воводим данные из ячейки в столбце ...

                    //Chislo_out[0] + " " +
                    //    //Chislo_out[1] + " " +
                    //Chislo_out[2] + " " +
                    //Chislo_out[3] + " " +
                    //Chislo_out[4] + " " +
                    //Chislo_out[5] + " " +
                    //Chislo_out[6] + " " +
                    //Chislo_out[7] + " " +
                    //Chislo_out[8]

                    //);

                    nomer_sdelki++; // увеличиваем значение индексируемого столбца на +1

                }


                // закрываем OleDbDataReader методом Close()
                myOleDbDataReader.Close();

            }

            return myOleDbDataReader;
        }


        public int Close_Access(OleDbConnection myOleDbConnection) //метод по закрытию базы данных 
        {

            // работа по чтению данных закончена, закрываем соединение с БД
            myOleDbConnection.Close();
            return 1;

        }
    }

    public class ExPort_File // классы записи данных в файл
    {
        public StreamWriter fstr_out;

        public StreamWriter ExPort_File_Metod(Parametrs Parametr) // открытие файла для записи
        {
            string[] General_Directory = Directory.GetCurrentDirectory().Split(new char[] { '\\' }); // корневая директория с EXE файлом
            fstr_out = new StreamWriter(General_Directory[0] + Parametr.File_ExPort);
            return fstr_out;
        }

        public void ExPort_Writer_File_Metod(string str_x) // запись данных в буфер
        {
            fstr_out.WriteLine(str_x);
            return;
        }

        public void Do_ExPort_File(Parametrs Parametr, string stroka) // открытие файла для ДОзаписи
        {
            // преобразуем данные в строку вида:
            // формат данных: SPFB.RTS,30,20120525,100000,127160.00000,127980.00000,126300.00000,127770.00000

            string[] General_Directory = Directory.GetCurrentDirectory().Split(new char[] { '\\' }); // корневая директория с EXE файлом
            //FilePath = General_Directory[0] + P01.File_ExPort; 
            StreamWriter fstr_out = new StreamWriter(File.Open((General_Directory[0] + Parametr.File_ExPort), // адрес экспортируемых данных
                                    FileMode.Append)); // открытие файла в режиме дозаписи в конец файла
            fstr_out.WriteLine(stroka);
            fstr_out.Close();
            return;
        }

        public void ExPort_Close_File_Metod() // закрытие потока, вывод данных из буфера в файл
        {
            fstr_out.Close();
            return;
        }

    }

    public class ExcelValuesWriter
    {

        public void Write_Excel(string name, string[] Reestr, int y, Parametrs Parametr)
        {
            // Сам сервер - объект Application или приложение Excel, 
            // может содержать одну или более книг, ссылки на которые содержит свойство Workbooks. 
            // Книги - объекты Workbook, могут содержать одну или более страниц, 
            // ссылки на которые содержит свойство Worksheets или (и) диаграмм - свойство Charts. 
            // Страницы - Worksheet, содержать объекты ячейки или группы ячеек, 
            // ссылки на которые становятся доступными через объект Range. 

            // Ниже в иерархии: строки, столбцы... Аналогично и для объекта Chart серии линий, легенды...

            //Process Ex1 = new Process();
            //Thread Thread = new System.Threading.Thread();
            //Thread.

            int x = Parametr.stroka; // индексатор строк в екселе
            // y - индексатор столбцов в екселе
            int i = 1; // индексатор массива

            Application xlApp = new ApplicationClass();
            Workbook xlWorkBook;
            Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            string[] General_Directory = Directory.GetCurrentDirectory().Split(new char[] { '\\' }); // корневая директория с EXE файлом
            string filePath = General_Directory[0] + Parametr.Excel_Path;

            //Если не существует файла то создать его
            bool isFileExist;

            FileInfo fInfo = new FileInfo(filePath);

            if (!fInfo.Exists)
            {
                xlWorkBook = xlApp.Workbooks.Add(misValue); //Добавить новый book в файл
                isFileExist = false;
            }
            else
            {
                //Открыть существующий файл
                xlWorkBook = xlApp.Workbooks.Open(
                    filePath,           // FileName - Имя открываемого файла
                    0,                  // UpdateLinks - Способ обновления ссылок в файле
                    false,              // ReadOnly - При значении true открытие только для чтения 
                    5,                  // Format - Определение формата символа разделителя
                    "",                 // Password - Пароль доступа к файлу до 15 символов
                    "",                 // WriteResPassword - Пароль на сохранение файла
                    true,               // IgnoreReadOnlyRecommended - При значении true отключается вывод  //запроса на работу без внесения изменений
                    XlPlatform.xlWindows, // Origin - Тип текстового файла 
                    "\t",               // Delimiter - Разделитель при Format = 6
                    false,              // Editable - Используется только для надстроек Excel 4.0
                    true,              // Notify - При значении true имя файла добавляется в  //список нотификации файлов
                    0,                  // Converter - Используется для передачи индекса конвертера файла //используемого для открытия файла    
                    true,              // AddToMRU - При true имя файла добавляется в список //открытых файлов
                    1,
                    0);


                // UpdateLinks - позволяет задать способ обновления ссылок в файле. 
                // Если данный параметр не задан, то выдается запрос на указание метода обновления. 
                // Значения: 0 - не обновлять ссылки; 1 - обновлять внешние ссылки; 2 - обновлять только удаленные ссылки; 3 - обновлять все ссылки.

                // Format - при работе с текстовыми файлами определяет символ разделителя для полей, заносимых в различные ячейки документа. 
                // Значения параметра: 1 - символ табуляции; 2 - запятая; 3 - пробел; 4 - точка с запятой; 5 - нет разделителя; 6 - другой символ, определенный в параметре Delimiter.


                isFileExist = true;
            }

            xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(Parametr.list); //Открытие вкладки "1_min"


            //БЛОК ЗАПОЛНЕНИЯ ФАЙЛА

            xlWorkSheet.Cells[x, y] = Parametr.zagolovok_visota_kanala; // высота канала
            x++;

            xlWorkSheet.Cells[x, y] = Parametr.zagolovok_stolbca; // таймфрейм
            x++;

            xlWorkSheet.Cells[x, y] = name; // заголовок вариации систему управления капиталом
            x++;


            while (i != 29)
            {

                xlWorkSheet.Cells[x, y] = Reestr[i]; //Запись значения в ряд = x, столбец = y
                x++;
                i++;
            }
            x = 1;
            i = 0;

            //Получить количество используемых столбцов
            //int columnsCount = xlWorkSheet.UsedRange.Columns.Count;

            //Получить количество используемых строк
            //int usedRowsCount = xlWorkSheet.UsedRange.Rows.Count;

            //Если файл существовал, просто сохранить его по умолчанию. Иначе сохранить в указанную директорию
            if (isFileExist)
            {
                xlWorkBook.Save();
            }
            else
            {
                xlWorkBook.SaveAs(filePath, XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue,
                misValue, XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            }

            xlApp.Visible = true;

            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();



            //БЛОК АНАЛИТИКИ ПО ОТКРЫТЫМ ПРОЦЕССАМ
            //Process[] myProcesses2 = Process.GetProcesses();
            //foreach (Process myProcess in myProcesses2)
            //{
            //Console.WriteLine(myProcess.ProcessName);
            //}


            //БЛОК ЗАКРЫТИЯ ВСЕХ ПРОЦЕССОВ EXCEL
            Process[] myProcesses;
            // Returns array containing all instances of Notepad.
            myProcesses = Process.GetProcessesByName("Excel");

            foreach (Process myProcess in myProcesses)
            {
                myProcess.Kill();
            }




        }

    }
}
