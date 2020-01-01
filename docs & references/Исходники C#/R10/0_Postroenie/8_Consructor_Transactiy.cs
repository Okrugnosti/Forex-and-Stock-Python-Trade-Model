using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace R10
{
    class Class_Consructor_Transactiy
    {

        // КАТАЛОГ ТРАНЗАКЦИЙ

        // 1# Заявка на продажу лимитированная
        // 2# Заявка на покупку лимитированная
        // 3# Заявка на покупку рыночная
        // 4# Заявка на продажу рыночная
        // 5# Стоп-лимит на продажу со сроком жизни до определенной даты
        // 6# Стоп-лимит на покупку со сроком жизни до определенной даты
        // 7# Стоп-лимит на продажу со сроком жизни до конца торговой сессии
        // 8# Стоп-лимит на покупку со сроком жизни до конца торговой сессии
        // 9# Стоп-лимит на продажу со сроком жизни до отмены
        // 10# Стоп-лимит на покупку со сроком жизни до отмены
        // 11# Стоп-заявка с условием по другой бумаге Ростелеком -ао, продажа 15 лотов по цене 7,000,
        // 12# Стоп-заявка со связанной заявкой РусГидро, покупка 15 лотов по цене 8,500, стоп-цена >= 8,000
        // 13# Тэйк-профит Лукойл, покупка 1 лота, активация при достижении цены 265 с отступом в 5%
        // 14# Тэйк-профит и стоп-лимит Лукойл, покупка 1 лота, активация тэйк-профита при достижении цены 2000
        // 15# Тэйк-профит по исполнению заявки по частичному исполнению заявки с номером 81874488
        // 16# Стоп-лимит по исполнению заявки по частичному исполнению заявки с номером 81874488 
        // 17# Тэйк-профит и стоп-лимит по исполнению заявки По частичному исполнению заявки с номером 123456
        // 18# Айсберг-заявка На фондовом рынке ММВБ, купить 100 лотов Аэрофлота по цене 70,
        // 19# Снятие заявки с номером 503983	
        // 20# Снятие всех заявок клиента с кодом Q6	
        // 21# Снятие стоп-заявоки с номером
        // 22# Снятие всех стоп-заявок с направлением «на покупку»
        // 23# Снятие всех стоп-заявок с направлением «на продажу»
        // 24# Снятие всех заявок на срочном рынке FORTS на покупку контрактов на курс акций Ростелеком -ао	
        // 25# Снятие всех заявок на срочном рынке FORTS на продажу контрактов на курс акций Ростелеком -ао	
        // 26# Удаление лимита открытых позиций на спот-рынке RTS Standard	
        // 27# Удаление лимита открытых позиций клиента по спот-активу на рынке RTS Standard	
        // 28# Перестановка заявок на срочном рынке FORTS	
        // 29# Безадресная заявка на покупку РусГидро, 1 лот по 15.0 руб., коду расчетов T0, 
        // 30# Снятие безадресной заявки с номером 15919
        
        public string [,,] Katalog_Tranzakciy = new string[50,10,100]; // каталог транзакций
       
        
        // формат массива
        // Katalog_Tranzakciy [n, 1] - код торгового поручения
       
        // новые    
        // Katalog_Tranzakciy [n, 2] - номер API транзакции
        // Katalog_Tranzakciy [n, 3] - цена
        // Katalog_Tranzakciy [n, 4] - строка отправленной команды
        // Katalog_Tranzakciy [n, 5] - объем
        
        // старые
        // Katalog_Tranzakciy [n, 6] - номер API транзакции
        // Katalog_Tranzakciy [n, 7] - цена
        // Katalog_Tranzakciy [n, 8] - строка отправленной команды
        // Katalog_Tranzakciy [n, 9] - объем

        // [x.y.100] - устанавливает глубину, в которую пишутся однотипные заявки. 
                   // Скажем в системе могут быть три вида стопов на покупку.
                    // x / y - определяет вид колиство строк и столбцов
                    // z - определяет подслои данных одной строки 

        // int COD_Transactions - определяет какую именно трансакцию вызывает стратегия
        // decimal Cena - цена в транзакции
        // int Volume_Position - размер ставки в транзакции

        public void Consructor_Transactiy(int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY, Parametrs Parametr)
        {

            Order_QUIK Quik = new Order_QUIK();
            Modul_API API = new Modul_API();
            
            TimeSpan Real_Time, // переменные для запуска автоматического переноса позиций
                     Point_a,
                     Point_b,
                     Point_c,
                     Point_d;
                          
            string a = "18:44:0.00";
            string b = "19:01:0.00";
            string c = "23:49:0.00";
            string d = "10:01:0.00";

            Point_a = TimeSpan.Parse(a); // создаем времЕнные точки
            Point_b = TimeSpan.Parse(b);
            Point_c = TimeSpan.Parse(c);
            Point_d = TimeSpan.Parse(d);

            Real_Time = DateTime.Now.TimeOfDay; // считаем текущее время

            string[] General_Directory = Directory.GetCurrentDirectory().Split(new char[] { '\\' }); // корневая директория с EXE файлом
            string QUIK_PATH = (General_Directory[0] + Parametr.QUIK_PATH); // директория к QUIK
            
            string ACCOUNT = Parametr.ACCOUNT;
            string CLASSCODE = Parametr.CLASSCODE;
            string CLIENT_CODE = Parametr.CLIENT_CODE;
            string SECCODE = Parametr.SECCODE; 
            string TRANS_ID = Parametr.TRANS_ID;

            
            decimal SIGNAL_STOP_PRISE = PRICE; // сигнальаня цена срабатывания стопа
            decimal STOP_PRICE_Min = PRICE; // цена до которой будут продавать по рынку
            decimal STOP_PRICE_Max = PRICE; // цена до которой будут покупать по рынку
            int EXPIRY_DATE = 20121002; // дата, до которой действует стоп заявка EXPIRY_DATE=20110519


            long ORDER_KEY = 123456789; // номер заявки присвоенной при выставлении
            string order; // переменная содержит сформированную команду транзакции 
                       

            // вызов транзакций по кодам


            // 1# Заявка на продажу лимитированная
            if (COD_Transactions_x == 1)
            {
                
                    order = Quik.Order_Sell_Limit(ACCOUNT, CLIENT_CODE, TRANS_ID, CLASSCODE,
                                         SECCODE, PRICE.ToString(), QUANTITY.ToString());

                    // вызываем API функцию, она смотрит изминения, и отправляет транзакцию, записывает номер транзакции, передает управление торговой системе
                    ORDER_KEY = API.Send_Transaktions(order, QUIK_PATH);

                    Katalog_Tranzakciy[COD_Transactions_x, 6, COD_Transactions_z] = ORDER_KEY.ToString();
                    Katalog_Tranzakciy[COD_Transactions_x, 7, COD_Transactions_z] = PRICE.ToString();
                    Katalog_Tranzakciy[COD_Transactions_x, 8, COD_Transactions_z] = order;
                    Katalog_Tranzakciy[COD_Transactions_x, 9, COD_Transactions_z] = QUANTITY.ToString();
                
                            
            }
                                    
            
            // 2# Заявка на покупку лимитированная
            if (COD_Transactions_x == 2)
            {

                
                    order = Quik.Order_Buy_Limit(ACCOUNT, CLIENT_CODE, TRANS_ID, CLASSCODE,
                                         SECCODE, PRICE.ToString(), QUANTITY.ToString());

                    // вызываем API функцию, она смотрит изминения, и отправляет транзакцию, записывает номер транзакции, передает управление торговой системе
                    ORDER_KEY = API.Send_Transaktions(order, QUIK_PATH);

                    Katalog_Tranzakciy[COD_Transactions_x, 6, COD_Transactions_z] = ORDER_KEY.ToString();
                    Katalog_Tranzakciy[COD_Transactions_x, 7, COD_Transactions_z] = PRICE.ToString();
                    Katalog_Tranzakciy[COD_Transactions_x, 8, COD_Transactions_z] = order;
                    Katalog_Tranzakciy[COD_Transactions_x, 9, COD_Transactions_z] = QUANTITY.ToString();

                

            }


            // 3# Заявка на продажу рыночная
            if (COD_Transactions_x == 3)
            {
                    order = Quik.Order_Sell_Market(ACCOUNT, CLIENT_CODE, TRANS_ID, CLASSCODE,
                                         SECCODE, QUANTITY.ToString());
                               
                    // вызываем API функцию, она смотрит изминения, и отправляет транзакцию, записывает номер транзакции, передает управление торговой системе
                    ORDER_KEY = API.Send_Transaktions(order, QUIK_PATH);

                    Katalog_Tranzakciy[COD_Transactions_x, 6, COD_Transactions_z] = ORDER_KEY.ToString();
                    Katalog_Tranzakciy[COD_Transactions_x, 7, COD_Transactions_z] = PRICE.ToString();
                    Katalog_Tranzakciy[COD_Transactions_x, 8, COD_Transactions_z] = order;
                    Katalog_Tranzakciy[COD_Transactions_x, 9, COD_Transactions_z] = QUANTITY.ToString();
                
            }

            
            // 4# Заявка на покупку рыночная
            if (COD_Transactions_x == 4)
            {
                    order = Quik.Order_Buy_Market(ACCOUNT, CLIENT_CODE, TRANS_ID, CLASSCODE,
                                         SECCODE, QUANTITY.ToString());

                
                    // вызываем API функцию, она смотрит изминения, и отправляет транзакцию, записывает номер транзакции, передает управление торговой системе
                    ORDER_KEY = API.Send_Transaktions(order, QUIK_PATH);

                    Katalog_Tranzakciy[COD_Transactions_x, 6, COD_Transactions_z] = ORDER_KEY.ToString();
                    Katalog_Tranzakciy[COD_Transactions_x, 7, COD_Transactions_z] = PRICE.ToString();
                    Katalog_Tranzakciy[COD_Transactions_x, 8, COD_Transactions_z] = order;
                    Katalog_Tranzakciy[COD_Transactions_x, 9, COD_Transactions_z] = QUANTITY.ToString();
                
            }

            
            // 5# Стоп-лимит на продажу со сроком жизни до определенной даты
            if (COD_Transactions_x == 5)
            {

                    order = Quik.Stop_Order_Sell_Data(ACCOUNT, CLIENT_CODE, TRANS_ID, CLASSCODE,
                                           SECCODE, QUANTITY.ToString(), SIGNAL_STOP_PRISE.ToString(), STOP_PRICE_Min.ToString(),
                                           EXPIRY_DATE.ToString());

                    // вызываем API функцию, она смотрит изминения, и отправляет транзакцию, записывает номер транзакции, передает управление торговой системе
                    ORDER_KEY = API.Send_Transaktions(order, QUIK_PATH);

                    Katalog_Tranzakciy[COD_Transactions_x, 6, COD_Transactions_z] = ORDER_KEY.ToString();
                    Katalog_Tranzakciy[COD_Transactions_x, 7, COD_Transactions_z] = PRICE.ToString();
                    Katalog_Tranzakciy[COD_Transactions_x, 8, COD_Transactions_z] = order;
                    Katalog_Tranzakciy[COD_Transactions_x, 9, COD_Transactions_z] = QUANTITY.ToString();
                
                
            }

            
            // 6# Стоп-лимит на покупку со сроком жизни до определенной даты
            if (COD_Transactions_x == 6)
            {
                    order = Quik.Stop_Order_Buy_Data(ACCOUNT, CLIENT_CODE, TRANS_ID, CLASSCODE,
                                           SECCODE, QUANTITY.ToString(), SIGNAL_STOP_PRISE.ToString(), STOP_PRICE_Max.ToString(),
                                           EXPIRY_DATE.ToString());

                    // вызываем API функцию, она смотрит изминения, и отправляет транзакцию, записывает номер транзакции, передает управление торговой системе
                    ORDER_KEY = API.Send_Transaktions(order, QUIK_PATH);

                    Katalog_Tranzakciy[COD_Transactions_x, 6, COD_Transactions_z] = ORDER_KEY.ToString();
                    Katalog_Tranzakciy[COD_Transactions_x, 7, COD_Transactions_z] = PRICE.ToString();
                    Katalog_Tranzakciy[COD_Transactions_x, 8, COD_Transactions_z] = order;
                    Katalog_Tranzakciy[COD_Transactions_x, 9, COD_Transactions_z] = QUANTITY.ToString();
                

            }


            // 7# Стоп-лимит на продажу со сроком жизни до конца торговой сессии
            if (COD_Transactions_x == 7)
            {

                    order = Quik.Stop_Order_Sell_Today(ACCOUNT, CLIENT_CODE, TRANS_ID, CLASSCODE,
                                           SECCODE, QUANTITY.ToString(), SIGNAL_STOP_PRISE.ToString(), STOP_PRICE_Min.ToString());

                    // вызываем API функцию, она смотрит изминения, и отправляет транзакцию, записывает номер транзакции, передает управление торговой системе
                    ORDER_KEY = API.Send_Transaktions(order, QUIK_PATH);

                    Katalog_Tranzakciy[COD_Transactions_x, 6, COD_Transactions_z] = ORDER_KEY.ToString();
                    Katalog_Tranzakciy[COD_Transactions_x, 7, COD_Transactions_z] = PRICE.ToString();
                    Katalog_Tranzakciy[COD_Transactions_x, 8, COD_Transactions_z] = order;
                    Katalog_Tranzakciy[COD_Transactions_x, 9, COD_Transactions_z] = QUANTITY.ToString();
                

            }

            
            // 8# Стоп-лимит на покупку со сроком жизни до конца торговой сессии
            if (COD_Transactions_x == 8)
            {

                    order = Quik.Stop_Order_Bay_Today(ACCOUNT, CLIENT_CODE, TRANS_ID, CLASSCODE,
                                           SECCODE, QUANTITY.ToString(), SIGNAL_STOP_PRISE.ToString(), STOP_PRICE_Max.ToString());

                               
                    // вызываем API функцию, она смотрит изминения, и отправляет транзакцию, записывает номер транзакции, передает управление торговой системе
                    ORDER_KEY = API.Send_Transaktions(order, QUIK_PATH);

                    Katalog_Tranzakciy[COD_Transactions_x, 6, COD_Transactions_z] = ORDER_KEY.ToString();
                    Katalog_Tranzakciy[COD_Transactions_x, 7, COD_Transactions_z] = PRICE.ToString();
                    Katalog_Tranzakciy[COD_Transactions_x, 8, COD_Transactions_z] = order;
                    Katalog_Tranzakciy[COD_Transactions_x, 9, COD_Transactions_z] = QUANTITY.ToString();
                
            }

            // 9# Стоп-лимит на продажу со сроком жизни до отмены
            if (COD_Transactions_x == 9)
            {
                    order = Quik.Stop_Order_Sell_GTC(ACCOUNT, CLIENT_CODE, TRANS_ID, CLASSCODE,
                                           SECCODE, QUANTITY.ToString(), SIGNAL_STOP_PRISE.ToString(), STOP_PRICE_Min.ToString());

                    // вызываем API функцию, она смотрит изминения, и отправляет транзакцию, записывает номер транзакции, передает управление торговой системе
                    ORDER_KEY = API.Send_Transaktions(order, QUIK_PATH);

                    Katalog_Tranzakciy[COD_Transactions_x, 6, COD_Transactions_z] = ORDER_KEY.ToString();
                    Katalog_Tranzakciy[COD_Transactions_x, 7, COD_Transactions_z] = PRICE.ToString();
                    Katalog_Tranzakciy[COD_Transactions_x, 8, COD_Transactions_z] = order;
                    Katalog_Tranzakciy[COD_Transactions_x, 9, COD_Transactions_z] = QUANTITY.ToString();
                

            }

            // 10# Стоп-лимит на покупку со сроком жизни до отмены
            if (COD_Transactions_x == 10)
            {
                    order = Quik.Stop_Order_Buy_GTC(ACCOUNT, CLIENT_CODE, TRANS_ID, CLASSCODE,
                                           SECCODE, QUANTITY.ToString(), SIGNAL_STOP_PRISE.ToString(), STOP_PRICE_Max.ToString());

                
                    // вызываем API функцию, она смотрит изминения, и отправляет транзакцию, записывает номер транзакции, передает управление торговой системе
                    ORDER_KEY = API.Send_Transaktions(order, QUIK_PATH);

                    Katalog_Tranzakciy[COD_Transactions_x, 6, COD_Transactions_z] = ORDER_KEY.ToString();
                    Katalog_Tranzakciy[COD_Transactions_x, 7, COD_Transactions_z] = PRICE.ToString();
                    Katalog_Tranzakciy[COD_Transactions_x, 8, COD_Transactions_z] = order;
                    Katalog_Tranzakciy[COD_Transactions_x, 9, COD_Transactions_z] = QUANTITY.ToString();
                

            }

            // вызов транзакций по кодам
            if (COD_Transactions_x == 11)
            {

            }

            // вызов транзакций по кодам
            if (COD_Transactions_x == 12)
            {

            }

            // вызов транзакций по кодам
            if (COD_Transactions_x == 13)
            {

            }

            // вызов транзакций по кодам
            if (COD_Transactions_x == 14)
            {

            }

            // вызов транзакций по кодам
            if (COD_Transactions_x == 15)
            {

            }

            // вызов транзакций по кодам
            if (COD_Transactions_x == 16)
            {

            }

            // вызов транзакций по кодам
            if (COD_Transactions_x == 17)
            {

            }

            // вызов транзакций по кодам
            if (COD_Transactions_x == 18)
            {

            }

            // 19# Снятие заявки на продажу лимитированной с номером 503983
            if (COD_Transactions_x == 19 & Katalog_Tranzakciy[1, 6, COD_Transactions_z] != "")
            {
                order = Quik.Kill_Order_Persona(TRANS_ID, Katalog_Tranzakciy[1, 6, COD_Transactions_z], CLASSCODE, SECCODE);
                                                
                // вызываем API функцию, она смотрит изминения, и отправляет транзакцию, записывает номер транзакции, передает управление торговой системе

                ORDER_KEY = API.Send_Transaktions(order, QUIK_PATH);

                Katalog_Tranzakciy[COD_Transactions_x, 6, COD_Transactions_z] = ORDER_KEY.ToString();
                Katalog_Tranzakciy[COD_Transactions_x, 7, COD_Transactions_z] = PRICE.ToString();
                Katalog_Tranzakciy[COD_Transactions_x, 8, COD_Transactions_z] = order;
                Katalog_Tranzakciy[COD_Transactions_x, 9, COD_Transactions_z] = QUANTITY.ToString();

                // сняли, обнулили строку, где была действующая заяка
                Katalog_Tranzakciy[1, 6, COD_Transactions_z] = "";
                Katalog_Tranzakciy[1, 7, COD_Transactions_z] = "";
                Katalog_Tranzakciy[1, 8, COD_Transactions_z] = "";
                Katalog_Tranzakciy[1, 9, COD_Transactions_z] = "";
                
              }


            // 20# Снятие заявки на покупку лимитированную с номером 503983
            if (COD_Transactions_x == 20 & Katalog_Tranzakciy[1, 6, COD_Transactions_z] != "")
            {
                order = Quik.Kill_Order_Persona(TRANS_ID, Katalog_Tranzakciy[2, 6, COD_Transactions_z], CLASSCODE, SECCODE); 

                // вызываем API функцию, она смотрит изминения, и отправляет транзакцию, записывает номер транзакции, передает управление торговой системе
                ORDER_KEY = API.Send_Transaktions(order, QUIK_PATH);

                Katalog_Tranzakciy[COD_Transactions_x, 6, COD_Transactions_z] = ORDER_KEY.ToString();
                Katalog_Tranzakciy[COD_Transactions_x, 7, COD_Transactions_z] = PRICE.ToString();
                Katalog_Tranzakciy[COD_Transactions_x, 8, COD_Transactions_z] = order;
                Katalog_Tranzakciy[COD_Transactions_x, 9, COD_Transactions_z] = QUANTITY.ToString();

                // сняли, обнулили строку, где была действующая заяка
                Katalog_Tranzakciy[2, 6, COD_Transactions_z] = "";
                Katalog_Tranzakciy[2, 7, COD_Transactions_z] = "";
                Katalog_Tranzakciy[2, 8, COD_Transactions_z] = "";
                Katalog_Tranzakciy[2, 9, COD_Transactions_z] = "";

            }

            // 21# Снятие заявки на продажу рыночная с номером 503983
            if (COD_Transactions_x == 21 & Katalog_Tranzakciy[1, 6, COD_Transactions_z] != "")
            {
                order = Quik.Kill_Order_Persona(TRANS_ID, Katalog_Tranzakciy[3, 6, COD_Transactions_z], CLASSCODE, SECCODE);

                // вызываем API функцию, она смотрит изминения, и отправляет транзакцию, записывает номер транзакции, передает управление торговой системе
                ORDER_KEY = API.Send_Transaktions(order, QUIK_PATH);

                Katalog_Tranzakciy[COD_Transactions_x, 6, COD_Transactions_z] = ORDER_KEY.ToString();
                Katalog_Tranzakciy[COD_Transactions_x, 7, COD_Transactions_z] = PRICE.ToString();
                Katalog_Tranzakciy[COD_Transactions_x, 8, COD_Transactions_z] = order;
                Katalog_Tranzakciy[COD_Transactions_x, 9, COD_Transactions_z] = QUANTITY.ToString();

                // сняли, обнулили строку, где была действующая заяка
                Katalog_Tranzakciy[3, 6, COD_Transactions_z] = "";
                Katalog_Tranzakciy[3, 7, COD_Transactions_z] = "";
                Katalog_Tranzakciy[3, 8, COD_Transactions_z] = "";
                Katalog_Tranzakciy[3, 9, COD_Transactions_z] = "";
            }

            // 22# Снятие заявки на покупку рыночная с номером 503983
            if (COD_Transactions_x == 22 & Katalog_Tranzakciy[1, 6, COD_Transactions_z] != "")
            {
                order = Quik.Kill_Order_Persona(TRANS_ID, Katalog_Tranzakciy[4, 6, COD_Transactions_z], CLASSCODE, SECCODE);

                // вызываем API функцию, она смотрит изминения, и отправляет транзакцию, записывает номер транзакции, передает управление торговой системе
                ORDER_KEY = API.Send_Transaktions(order, QUIK_PATH);

                Katalog_Tranzakciy[COD_Transactions_x, 6, COD_Transactions_z] = ORDER_KEY.ToString();
                Katalog_Tranzakciy[COD_Transactions_x, 7, COD_Transactions_z] = PRICE.ToString();
                Katalog_Tranzakciy[COD_Transactions_x, 8, COD_Transactions_z] = order;
                Katalog_Tranzakciy[COD_Transactions_x, 9, COD_Transactions_z] = QUANTITY.ToString();

                // сняли, обнулили строку, где была действующая заяка
                Katalog_Tranzakciy[4, 6, COD_Transactions_z] = "";
                Katalog_Tranzakciy[4, 7, COD_Transactions_z] = "";
                Katalog_Tranzakciy[4, 8, COD_Transactions_z] = "";
                Katalog_Tranzakciy[4, 9, COD_Transactions_z] = "";
            }
                

            // вызов транзакций по кодам
            if (COD_Transactions_x == 23)
            {

            }

            // 24# Снятие стоп-заявоки на продажу со сроком жизни до определенной даты с номером 
            if (COD_Transactions_x == 24 & Katalog_Tranzakciy[1, 6, COD_Transactions_z] != "")
            {


                order = Quik.Kill_Stop_Order_Persona(TRANS_ID, Katalog_Tranzakciy[5, 6, COD_Transactions_z], CLASSCODE, SECCODE);
                                             
                // вызываем API функцию, она смотрит изминения, и отправляет транзакцию, записывает номер транзакции, передает управление торговой системе
                ORDER_KEY = API.Send_Transaktions(order, QUIK_PATH);

                Katalog_Tranzakciy[COD_Transactions_x, 6, COD_Transactions_z] = ORDER_KEY.ToString();
                Katalog_Tranzakciy[COD_Transactions_x, 7, COD_Transactions_z] = PRICE.ToString();
                Katalog_Tranzakciy[COD_Transactions_x, 8, COD_Transactions_z] = order;
                Katalog_Tranzakciy[COD_Transactions_x, 9, COD_Transactions_z] = QUANTITY.ToString();

                // сняли, обнулили строку, где была действующая заяка
                Katalog_Tranzakciy[5, 6, COD_Transactions_z] = "";
                Katalog_Tranzakciy[5, 7, COD_Transactions_z] = "";
                Katalog_Tranzakciy[5, 8, COD_Transactions_z] = "";
                Katalog_Tranzakciy[5, 9, COD_Transactions_z] = "";
            }

            // 25# Снятие стоп-заявоки на покупку со сроком жизни до определенной даты с номером
            if (COD_Transactions_x == 25 & Katalog_Tranzakciy[1, 6, COD_Transactions_z] != "")
            {
                order = Quik.Kill_Stop_Order_Persona(TRANS_ID, Katalog_Tranzakciy[6, 6, COD_Transactions_z], CLASSCODE, SECCODE);

                // вызываем API функцию, она смотрит изминения, и отправляет транзакцию, записывает номер транзакции, передает управление торговой системе
                ORDER_KEY = API.Send_Transaktions(order, QUIK_PATH);

                Katalog_Tranzakciy[COD_Transactions_x, 6, COD_Transactions_z] = ORDER_KEY.ToString();
                Katalog_Tranzakciy[COD_Transactions_x, 7, COD_Transactions_z] = PRICE.ToString();
                Katalog_Tranzakciy[COD_Transactions_x, 8, COD_Transactions_z] = order;
                Katalog_Tranzakciy[COD_Transactions_x, 9, COD_Transactions_z] = QUANTITY.ToString();

                // сняли, обнулили строку, где была действующая заяка
                Katalog_Tranzakciy[6, 6, COD_Transactions_z] = "";
                Katalog_Tranzakciy[6, 7, COD_Transactions_z] = "";
                Katalog_Tranzakciy[6, 8, COD_Transactions_z] = "";
                Katalog_Tranzakciy[6, 9, COD_Transactions_z] = "";
            }

            // 26# Снятие стоп-заявоки на продажу со сроком жизни до конца торгового дня с номером
            if (COD_Transactions_x == 26 & Katalog_Tranzakciy[1, 6, COD_Transactions_z] != "")
            {
                order = Quik.Kill_Stop_Order_Persona(TRANS_ID, Katalog_Tranzakciy[7, 6, COD_Transactions_z], CLASSCODE, SECCODE);

                // вызываем API функцию, она смотрит изминения, и отправляет транзакцию, записывает номер транзакции, передает управление торговой системе
                ORDER_KEY = API.Send_Transaktions(order, QUIK_PATH);

                Katalog_Tranzakciy[COD_Transactions_x, 6, COD_Transactions_z] = ORDER_KEY.ToString();
                Katalog_Tranzakciy[COD_Transactions_x, 7, COD_Transactions_z] = PRICE.ToString();
                Katalog_Tranzakciy[COD_Transactions_x, 8, COD_Transactions_z] = order;
                Katalog_Tranzakciy[COD_Transactions_x, 9, COD_Transactions_z] = QUANTITY.ToString();

                // сняли, обнулили строку, где была действующая заяка
                Katalog_Tranzakciy[7, 6, COD_Transactions_z] = "";
                Katalog_Tranzakciy[7, 7, COD_Transactions_z] = "";
                Katalog_Tranzakciy[7, 8, COD_Transactions_z] = "";
                Katalog_Tranzakciy[7, 9, COD_Transactions_z] = "";
            }

            // 27# Снятие стоп-заявоки на покупку со сроком жизни до конца торгового дня с номером
            if (COD_Transactions_x == 27 & Katalog_Tranzakciy[1, 6, COD_Transactions_z] != "")
            {
                order = Quik.Kill_Stop_Order_Persona(TRANS_ID, Katalog_Tranzakciy[8, 6, COD_Transactions_z], CLASSCODE, SECCODE);

                // вызываем API функцию, она смотрит изминения, и отправляет транзакцию, записывает номер транзакции, передает управление торговой системе
                ORDER_KEY = API.Send_Transaktions(order, QUIK_PATH);

                Katalog_Tranzakciy[COD_Transactions_x, 6, COD_Transactions_z] = ORDER_KEY.ToString();
                Katalog_Tranzakciy[COD_Transactions_x, 7, COD_Transactions_z] = PRICE.ToString();
                Katalog_Tranzakciy[COD_Transactions_x, 8, COD_Transactions_z] = order;
                Katalog_Tranzakciy[COD_Transactions_x, 9, COD_Transactions_z] = QUANTITY.ToString();

                // сняли, обнулили строку, где была действующая заяка
                Katalog_Tranzakciy[8, 6, COD_Transactions_z] = "";
                Katalog_Tranzakciy[8, 7, COD_Transactions_z] = "";
                Katalog_Tranzakciy[8, 8, COD_Transactions_z] = "";
                Katalog_Tranzakciy[8, 9, COD_Transactions_z] = "";
            }

            // 28# Снятие стоп-заявоки на продажу со сроком жизни до отмены с номером
            if (COD_Transactions_x == 28 & Katalog_Tranzakciy[1, 6, COD_Transactions_z] != "")
            {
                order = Quik.Kill_Stop_Order_Persona(TRANS_ID, Katalog_Tranzakciy[9, 6, COD_Transactions_z], CLASSCODE, SECCODE);

                // вызываем API функцию, она смотрит изминения, и отправляет транзакцию, записывает номер транзакции, передает управление торговой системе
                ORDER_KEY = API.Send_Transaktions(order, QUIK_PATH);

                Katalog_Tranzakciy[COD_Transactions_x, 6, COD_Transactions_z] = ORDER_KEY.ToString();
                Katalog_Tranzakciy[COD_Transactions_x, 7, COD_Transactions_z] = PRICE.ToString();
                Katalog_Tranzakciy[COD_Transactions_x, 8, COD_Transactions_z] = order;
                Katalog_Tranzakciy[COD_Transactions_x, 9, COD_Transactions_z] = QUANTITY.ToString();

                // сняли, обнулили строку, где была действующая заяка
                Katalog_Tranzakciy[9, 6, COD_Transactions_z] = "";
                Katalog_Tranzakciy[9, 7, COD_Transactions_z] = "";
                Katalog_Tranzakciy[9, 8, COD_Transactions_z] = "";
                Katalog_Tranzakciy[9, 9, COD_Transactions_z] = "";
            }

            // 29# Снятие стоп-заявоки на покупку со сроком жизни до отмены с номером
            if (COD_Transactions_x == 29 & Katalog_Tranzakciy[1, 6, COD_Transactions_z] != "")
            {
                order = Quik.Kill_Stop_Order_Persona(TRANS_ID, Katalog_Tranzakciy[10, 6, COD_Transactions_z], CLASSCODE, SECCODE);

                // вызываем API функцию, она смотрит изминения, и отправляет транзакцию, записывает номер транзакции, передает управление торговой системе
                ORDER_KEY = API.Send_Transaktions(order, QUIK_PATH);

                Katalog_Tranzakciy[COD_Transactions_x, 6, COD_Transactions_z] = ORDER_KEY.ToString();
                Katalog_Tranzakciy[COD_Transactions_x, 7, COD_Transactions_z] = PRICE.ToString();
                Katalog_Tranzakciy[COD_Transactions_x, 8, COD_Transactions_z] = order;
                Katalog_Tranzakciy[COD_Transactions_x, 9, COD_Transactions_z] = QUANTITY.ToString();

                // сняли, обнулили строку, где была действующая заяка
                Katalog_Tranzakciy[10, 6, COD_Transactions_z] = "";
                Katalog_Tranzakciy[10, 7, COD_Transactions_z] = "";
                Katalog_Tranzakciy[10, 8, COD_Transactions_z] = "";
                Katalog_Tranzakciy[10, 9, COD_Transactions_z] = "";
            }

            // вызов транзакций по кодам
            if (COD_Transactions_x == 30)
            {

            }

            // вызов транзакций по кодам
            if (COD_Transactions_x == 31)
            {

            }

            // вызов транзакций по кодам
            if (COD_Transactions_x == 32)
            {

            }

            // вызов транзакций по кодам
            if (COD_Transactions_x == 33)
            {

            }

            // вызов транзакций по кодам
            if (COD_Transactions_x == 34)
            {

            }

            // вызов транзакций по кодам
            if (COD_Transactions_x == 35)
            {

            }

            // вызов транзакций по кодам
            if (COD_Transactions_x == 36)
            {

            }

            // вызов транзакций по кодам
            if (COD_Transactions_x == 37)
            {

            }

            // вызов транзакций по кодам
            if (COD_Transactions_x == 38)
            {

            }

            
        }


    }
}
