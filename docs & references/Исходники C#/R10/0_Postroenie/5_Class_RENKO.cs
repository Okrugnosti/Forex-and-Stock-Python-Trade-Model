using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R10
{
    // ИНДИКАТОР RENKO 5_min_850

    public class Class_RENKO // 
    {
        Class_Capital_System C01 = new Class_Capital_System();
        Analiz_Dohod_Sistem A01 = new Analiz_Dohod_Sistem();
        
        
        
        public int stapt_stop = 0; // индекс вывода заявок в начале торгов

        // переменные торгового decimal
        public decimal Uroven; // сетка 
        public decimal Visota_Kanala; // устанавливаем высоту канала (пункты)
        public decimal Nachalo_Otsheta; // устанавливаем начальную точку отсчета (пункты)
        public decimal H_Stop; // высота стопа (в кубиках)
        public decimal Filter_Stop; // ценовой фильтр между стопами (в кубиках)
        
        public decimal Stop_Bay_1; // стоп заявка на закрытие позии (выход из шортов в кеш)
        public decimal Stop_Bay_2; // стоп заявка на открытие позции (выход из кеша в лонг)
        public decimal Stop_Sell_1; // стоп заявка на закрытие позции (выход из лонгов в кеш)
        public decimal Stop_Sell_2; // стоп заявка на открытие позиции (выход из кеша в шорты)
        
        public int Volume_Position; // размер текущей торговой позиции (в контрактах)
        public int Plan_Volume_Position; // размер плановой торговой позиции (в контрактах)
        public decimal Vector_Position; // текущее направлении торговой позиции (Bay/Sell/Cash)
        public decimal Bay = 1; // направлении торговой позиции (покупки)
        public decimal Sell = -1; // направление торговой позиции (продажи)
        public decimal Cash = 0; // направлении торговой позции (в деньгах)
        public decimal PriceClose; // значение цены последней сдеки (в пунктах)


        public decimal Open_Position; // цена открытия позиции
        public decimal Close_Position; //уровень закрытия позиции

        public decimal Shag_Cen; //шаг движения цены на размер кубиках, когда цена закрытия переходит на новый уровень(в кубиках)


        public decimal Sum_Sdelok = 0; // счетчик количества сделок
        public decimal Local_Profit; // переменная учитывающая сумму прибыль/убытка от проведенной операции
        public decimal Max_Profit; // рассчет планки прироста капитала
        public decimal Local_Drogdawn; // Рассчет локальной просадки
        public decimal Max_Drogdawn; // расчет максимальной просадки
        public decimal Doli_Drogdawn = 0; // доля текущей просадки от максимальной просадки
        public decimal Rezultat_Sdelki = 0; // результат сделки 
        public decimal Rezerv_Rezultat_Sdelki = 0; // результат сделки


        public string str_x, str_x1; // строка для вывода данных в файл



        public string Dano_Renko_5_min_850(decimal[] Chislo_out, int Stavka, Parametrs Parametr) // метод по заданию параметров классической системы
        {

            Visota_Kanala = Parametr.Visota_Kanala;
            Nachalo_Otsheta = Parametr.Nachalo_Otsheta;
            H_Stop = Parametr.H_Stop;
            Filter_Stop = Parametr.Filter_Stop;

            //ценовые данные текущего этажа
            // x.Chislo_out[0] - таймфрейм 
            // x.Chislo_out[1] - дата
            // x.Chislo_out[2] - время
            // x.Chislo_out[3] - open
            // x.Chislo_out[4] - max
            // x.Chislo_out[5] - min
            // x.Chislo_out[6] - close

            // задаем начальные значения для торгового decimal
            PriceClose = Chislo_out[6]; // задаем цену последней сделки в качестве цены закрытия
            Uroven = (Math.Floor(Chislo_out[6] / Visota_Kanala) * Visota_Kanala) + Nachalo_Otsheta; // получаем первый уровень сетки
            Stop_Bay_1 = 0; // устанавливаем начальный стоп на закрытие шортов
            Stop_Sell_1 = 0; // устанавливаем начальный стоп на закрытие лонгов

            Stop_Bay_2 = Stop_Sell_2 = 0; 

            Stop_Bay_2 = Uroven + Visota_Kanala; // устанавливаем начальный стоп на покупку (сетка + 1 кубик)
            Stop_Sell_2 = Uroven - Visota_Kanala; // устанавливаем начальный стоп на продажу (сетка - 1 кубик) 
            Volume_Position = 0; // устанавливаем размер текущей торговой позиции
            Plan_Volume_Position = Stavka; // устанавливаем плановую торговую позицию в случае открытия позициц
            Vector_Position = Cash; // задаем начальное направлнии позиции
            Open_Position = 0; // задаем цену открытия позиции
            Close_Position = 0;

            Local_Profit = 0;
            Max_Profit = 0;

            Local_Drogdawn = 0;
            Max_Drogdawn = Parametr.Max_Drogdawn_R01; // задаем историческую просадку
                        
            return str_x;

        }

        public string Renko_5_min_850(decimal[] Chislo_out, int i, int Regim_Rabota, int sistem_vector, int Stavka, Parametrs Parametr) // метод по заданию параметров классической системы
        {
                        
            Class_Consructor_Transactiy QUIK = new Class_Consructor_Transactiy();
            Plan_Volume_Position = Stavka;
            
            // int Regim_Rabota - режим работы торгового робота 1 - заявки в API выставляются
            
            
            // алгоритм циклической обработки котировок
            if (Vector_Position == Cash) // если торговая позиция "в кеше" торгда проверяем исполнение стопов на вход в позицию
            {
                // если мы в кеше, то это значит что стопов на выход стоять не должно
                // должны стоять стопы только на вход

                if (Chislo_out[4] >= Stop_Bay_2) // идет проверка на пересечение стоп цены на вход в лонги
                // если максимум свечи пересек цену стопа 
                {
                    Vector_Position = Bay; // направлеи позиции ставим на лонг
                    Volume_Position = Plan_Volume_Position; // размер позиции устанавливаем на размер плановой открытой позиции
                    Open_Position = Stop_Bay_2; //устанавливаем цену открытия позиции
                    Stop_Sell_1 = Open_Position - (H_Stop * Visota_Kanala); // устанавливаем стоп заявку на выход в Кешь из лонгов
                    Stop_Sell_2 = Open_Position - ((H_Stop + Filter_Stop) * Visota_Kanala); // устанавливае стоп заявку на вход в Шорты

                    Sum_Sdelok++; // если мы сменили позицию, значит произошла сделка

                    Stop_Bay_1 = 0; // сбрасываем он нам не нужен
                    Stop_Bay_2 = 0; // сбрасываем он нам не нужен

                    if (Regim_Rabota == 1 & sistem_vector == 1 & Plan_Volume_Position != 0 ) // если у нас включен торговый режим
                    {
                        // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                        // в начале все снимаем
                        QUIK.Consructor_Transactiy(28, 1, 0, 0, Parametr); // Stop_Sell_1
                        QUIK.Consructor_Transactiy(28, 2, 0, 0, Parametr); // Stop_Sell_2
                        QUIK.Consructor_Transactiy(29, 1, 0, 0, Parametr); // Stop_Bay_1
                        QUIK.Consructor_Transactiy(29, 2, 0, 0, Parametr); // Stop_Bay_2
                        
                        // теперь выставляем нужные
                        QUIK.Consructor_Transactiy(9, 1, Stop_Sell_1, Plan_Volume_Position, Parametr); // Stop_Sell_1
                        QUIK.Consructor_Transactiy(9, 2, Stop_Sell_2, Plan_Volume_Position, Parametr); // Stop_Sell_2

                    }

                   

                }
                else
                {
                    if (Chislo_out[5] <= Stop_Sell_2) // идет проверка на пересечение стоп цены на вход в шорт
                    // если минимум свечи пересек цену стопа 
                    {
                        Vector_Position = Sell; // направлеи позиции ставим на лонг
                        Volume_Position = Plan_Volume_Position; // размер позиции устанавливаем на размер плановой открытой позиции
                        Open_Position = Stop_Sell_2; //устанавливаем цену открытия позиции на уровне стопа 
                        Stop_Bay_1 = Open_Position + (H_Stop * Visota_Kanala); // устанавливаем стоп заявку на выход в Кешь из Шортов
                        Stop_Bay_2 = Open_Position + ((H_Stop + Filter_Stop) * Visota_Kanala); // устанавливае стоп заявку на вход в Лонги

                        Sum_Sdelok++; // если мы сменили позицию, значит произошла сделка

                        Stop_Sell_1 = 0; // сбрасываем он нам не нужен
                        Stop_Sell_2 = 0; // сбрасываем он нам не нужен


                        if (Regim_Rabota == 1 & sistem_vector == 1 & Plan_Volume_Position != 0) // если у нас включен торговый режим
                        {
                            // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                            // в начале все снимаем
                            QUIK.Consructor_Transactiy(28, 1, 0, 0, Parametr); // Stop_Sell_1
                            QUIK.Consructor_Transactiy(28, 2, 0, 0, Parametr); // Stop_Sell_2
                            QUIK.Consructor_Transactiy(29, 1, 0, 0, Parametr); // Stop_Bay_1
                            QUIK.Consructor_Transactiy(29, 2, 0, 0, Parametr); // Stop_Bay_2

                            // теперь выставляем нужные
                            QUIK.Consructor_Transactiy(10, 1, Stop_Bay_1, Plan_Volume_Position, Parametr); // Stop_Bay_1
                            QUIK.Consructor_Transactiy(10, 2, Stop_Bay_2, Plan_Volume_Position, Parametr); // Stop_Bay_2

                        }

                       
                    }
                }



            }




            // -------------------- закончили проверку находясь в кеше ------------------ // 


            else // начали проверку стопов находясь в позиции
            {


                if (Vector_Position == Bay) // если мы находимся в покупке
                {



                    if (Chislo_out[5] <= Stop_Sell_1) // идет проверка на пересечение стоп цены на выход в кешь
                    // если минимум свечи пересек цену стопа 
                    {
                        Vector_Position = Cash; // направлеи позиции ставим на Cash
                        Volume_Position = 0; // размер позиции устанавливаем на размер плановой открытой позиции
                        Close_Position = Stop_Sell_1; //устанавливаем цену закрытия позиции
                        Stop_Sell_2 = Close_Position - (Filter_Stop * Visota_Kanala); // Корректируем стоп заявку на вход в шорт
                        Stop_Bay_2 = Close_Position + (H_Stop * Visota_Kanala); // Устанавливаем стоп заявку на обратный вход в лонг                  

                        Sum_Sdelok++; // если мы сменили позицию, значит произошла сделка
                        Rezultat_Sdelki = (Close_Position - Open_Position) - Parametr.Proskalzivania;

                        Stop_Sell_1 = 0; // сбрасываем он нам не нужен
                        Stop_Bay_1 = 0; // сбрасываем он нам не нужен

                        Open_Position = 0; // сбрасываем он нам не нужен



                        if (Regim_Rabota == 1 & sistem_vector == 1 & Plan_Volume_Position != 0) // если у нас включен торговый режим
                        {
                            // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                            // в начале все снимаем
                            QUIK.Consructor_Transactiy(28, 1, 0, 0, Parametr); // Stop_Sell_1
                            QUIK.Consructor_Transactiy(28, 2, 0, 0, Parametr); // Stop_Sell_2
                            QUIK.Consructor_Transactiy(29, 1, 0, 0, Parametr); // Stop_Bay_1
                            QUIK.Consructor_Transactiy(29, 2, 0, 0, Parametr); // Stop_Bay_2

                            // теперь выставляем нужные
                            QUIK.Consructor_Transactiy(9, 2, Stop_Sell_2, Plan_Volume_Position, Parametr); // Stop_Sell_2
                            QUIK.Consructor_Transactiy(10, 2, Stop_Bay_2, Plan_Volume_Position, Parametr); // Stop_Bay_2

                        }

                                                

                        //-----закончили проверку на пересечения стопа первого уровня---//


                        if (Chislo_out[5] <= Stop_Sell_2) // идет проверка на пересечение стоп цены на вход в шорт
                        // если минимум свечи пересек цену стопа 
                        {
                            Vector_Position = Sell; // направлеи позиции ставим на Шорт
                            Volume_Position = Plan_Volume_Position; // размер позиции устанавливаем на размер плановой открытой позиции
                            Open_Position = Stop_Sell_2; //устанавливаем цену открытия позиции
                            Stop_Bay_1 = Open_Position + (H_Stop * Visota_Kanala); // Устанавливаем уровень стопа на выход в кешь из шортов
                            Stop_Bay_2 = Stop_Bay_1 + (Filter_Stop * Visota_Kanala); // Устанавливаем уровень стопа на выход в лонг из кеша


                            Sum_Sdelok++; // если мы сменили позицию, значит произошла сделка

                            Stop_Sell_1 = 0; // сбрасываем он нам не нужен
                            Stop_Sell_2 = 0; // сбрасываем он нам не нужен


                            if (Regim_Rabota == 1 & sistem_vector == 1 & Plan_Volume_Position != 0) // если у нас включен торговый режим
                            {
                                // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                                // в начале все снимаем
                                QUIK.Consructor_Transactiy(28, 1, 0, 0, Parametr); // Stop_Sell_1
                                QUIK.Consructor_Transactiy(28, 2, 0, 0, Parametr); // Stop_Sell_2
                                QUIK.Consructor_Transactiy(29, 1, 0, 0, Parametr); // Stop_Bay_1
                                QUIK.Consructor_Transactiy(29, 2, 0, 0, Parametr); // Stop_Bay_2

                                // теперь выставляем нужные
                                QUIK.Consructor_Transactiy(10, 1, Stop_Bay_1, Plan_Volume_Position, Parametr); // Stop_Bay_1
                                QUIK.Consructor_Transactiy(10, 2, Stop_Bay_2, Plan_Volume_Position, Parametr); // Stop_Bay_2

                            }
                                                        

                            //-----закончили проверку на пересечения стопа второго уровня---//

                        }

                    }
                    else // если стоп заявка на выход в кешь не была пересечена,    
                    // проводим проверку на достижение новых уровней.
                    {
                        if ((((Chislo_out[8] - Stop_Sell_1) - (H_Stop * Visota_Kanala)) / Visota_Kanala) >= 1 & Chislo_out[7] == 1) // проверяем шаг цены в кубиках ценой закрытия
                        // если цена закрытия выросла на 1 кубик и более
                        {                   // рассчитываем шаг цены и округляем до целого кубика
                            Shag_Cen = Math.Floor(((Chislo_out[8] - Stop_Sell_1) - (H_Stop * Visota_Kanala)) / Visota_Kanala);
                            Stop_Sell_1 = Stop_Sell_1 + (Shag_Cen * Visota_Kanala); // передвигаем стоп на выход в кешь из лонгов на шаг цены в кубиках
                            Stop_Sell_2 = Stop_Sell_1 - (Filter_Stop * Visota_Kanala); // передвигаем стоп на вход в шорт из кеша на шаг цены в кубиках



                            if (Regim_Rabota == 1 & sistem_vector == 1) // если у нас включен торговый режим
                            {
                                // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                                // в начале все снимаем
                                QUIK.Consructor_Transactiy(28, 1, 0, 0, Parametr); // Stop_Sell_1
                                QUIK.Consructor_Transactiy(28, 2, 0, 0, Parametr); // Stop_Sell_2
                                QUIK.Consructor_Transactiy(29, 1, 0, 0, Parametr); // Stop_Bay_1
                                QUIK.Consructor_Transactiy(29, 2, 0, 0, Parametr); // Stop_Bay_2

                                // теперь выставляем нужные
                                QUIK.Consructor_Transactiy(9, 1, Stop_Sell_1, Plan_Volume_Position, Parametr); // Stop_Sell_1
                                QUIK.Consructor_Transactiy(9, 2, Stop_Sell_2, Plan_Volume_Position, Parametr); // Stop_Sell_2

                            }


                            
                        }

                        //------------закрыли проверку на скачек цены в сторону открытой позиции-------------////

                    }// --------------- закрыли всю ценовую проверку на позицию в лонгах ----- // 

                }

                else
                {

                    if (Vector_Position == Sell) // если мы находимся в продажах
                    {


                        if (Chislo_out[4] >= Stop_Bay_1) // идет проверка на пересечение стоп цены на выход в кешь
                        // если максимум свечи пересек цену стопа 
                        {
                            Vector_Position = Cash; // направлеи позиции ставим на Cash
                            Volume_Position = 0; // размер позиции устанавливаем на размер плановой открытой позиции
                            Close_Position = Stop_Bay_1; //устанавливаем цену закрытия позиции
                            Stop_Bay_2 = Close_Position + (Filter_Stop * Visota_Kanala); // корректируем стопа для входа в лонги
                            Stop_Sell_2 = Close_Position - (H_Stop * Visota_Kanala); // Устанавливаем уровень стопа на обратный вход в шорт из кеша

                            Sum_Sdelok++; // если мы сменили позицию, значит произошла сделка
                            Rezultat_Sdelki = (Open_Position - Close_Position) - Parametr.Proskalzivania; // результат сделки

                            Stop_Bay_1 = 0; // сбрасываем он нам не нужен
                            Stop_Sell_1 = 0; // сбрасываем он нам не нужен

                            Open_Position = 0; // сбрасываем он нам не нужен


                            if (Regim_Rabota == 1 & sistem_vector == 1 & Plan_Volume_Position != 0) // если у нас включен торговый режим
                            {
                                // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                                // в начале все снимаем
                                QUIK.Consructor_Transactiy(28, 1, 0, 0, Parametr); // Stop_Sell_1
                                QUIK.Consructor_Transactiy(28, 2, 0, 0, Parametr); // Stop_Sell_2
                                QUIK.Consructor_Transactiy(29, 1, 0, 0, Parametr); // Stop_Bay_1
                                QUIK.Consructor_Transactiy(29, 2, 0, 0, Parametr); // Stop_Bay_2

                                // теперь выставляем нужные
                                QUIK.Consructor_Transactiy(9, 2, Stop_Sell_2, Plan_Volume_Position, Parametr); // Stop_Sell_2
                                QUIK.Consructor_Transactiy(10, 2, Stop_Bay_2, Plan_Volume_Position, Parametr); // Stop_Bay_2
                                
                            }


                            

                            //-----закончили проверку на пересечения стопа первого уровня---//  


                            if (Chislo_out[4] >= Stop_Bay_2) // идет проверка на пересечение стоп цены на вход в лонг
                            // если максимум свечи пересек цену стопа 
                            {
                                Vector_Position = Bay; // направлеи позиции ставим на Шорт
                                Volume_Position = Plan_Volume_Position; // размер позиции устанавливаем на размер плановой открытой позиции
                                Open_Position = Stop_Bay_2; //устанавливаем цену открытия позиции 
                                Stop_Sell_1 = Open_Position - (H_Stop * Visota_Kanala); // Устанавливаем уровень стопа на выход в кешь из лонгов
                                Stop_Sell_2 = Stop_Sell_1 - (Filter_Stop * Visota_Kanala); // Устанавливаем уровень стопа на вход в шорт из кеша

                                Sum_Sdelok++; // если мы сменили позицию, значит произошла сделка

                                Stop_Bay_1 = 0; // сбрасываем он нам не нужен
                                Stop_Bay_2 = 0; // сбрасываем он нам не нужен


                                if (Regim_Rabota == 1 & sistem_vector == 1 & Plan_Volume_Position != 0) // если у нас включен торговый режим
                                {
                                    // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                                    // в начале все снимаем
                                    QUIK.Consructor_Transactiy(28, 1, 0, 0, Parametr); // Stop_Sell_1
                                    QUIK.Consructor_Transactiy(28, 2, 0, 0, Parametr); // Stop_Sell_2
                                    QUIK.Consructor_Transactiy(29, 1, 0, 0, Parametr); // Stop_Bay_1
                                    QUIK.Consructor_Transactiy(29, 2, 0, 0, Parametr); // Stop_Bay_2

                                    // теперь выставляем нужные
                                    QUIK.Consructor_Transactiy(9, 1, Stop_Sell_1, Plan_Volume_Position, Parametr); // Stop_Sell_1
                                    QUIK.Consructor_Transactiy(9, 2, Stop_Sell_2, Plan_Volume_Position, Parametr); // Stop_Sell_2

                                }

                                

                                //-----закончили проверку на пересечения стопа второго уровня---//

                            }

                        }
                        else // если стоп заявка на выход в кешь не была пересечена,    
                        // проводим проверку на достижение новых уровней.
                        {
                            if ((((Stop_Bay_1 - Chislo_out[8]) - (H_Stop * Visota_Kanala)) / Visota_Kanala) >= 1 & Chislo_out[7] == 1) // проверяем шаг цены в кубиках ценой закрытия
                            // если цена закрытия выросла на 1 кубик и более
                            {                   // рассчитываем шаг цены и округляем до целого кубика
                                Shag_Cen = Math.Floor(((Stop_Bay_1 - Chislo_out[8]) - (H_Stop * Visota_Kanala)) / Visota_Kanala);
                                Stop_Bay_1 = Stop_Bay_1 - (Shag_Cen * Visota_Kanala); // передвигаем стоп на выход в кешь из шортов на шаг цены в кубиках
                                Stop_Bay_2 = Stop_Bay_1 + (Filter_Stop * Visota_Kanala); // передвигаем стоп на вход в лонг из кеша на шаг цены в кубиках

                                if (Regim_Rabota == 1 & sistem_vector == 1 & Plan_Volume_Position != 0) // если у нас включен торговый режим
                                {
                                    // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                                    // в начале все снимаем
                                    QUIK.Consructor_Transactiy(28, 1, 0, 0, Parametr); // Stop_Sell_1
                                    QUIK.Consructor_Transactiy(28, 2, 0, 0, Parametr); // Stop_Sell_2
                                    QUIK.Consructor_Transactiy(29, 1, 0, 0, Parametr); // Stop_Bay_1
                                    QUIK.Consructor_Transactiy(29, 2, 0, 0, Parametr); // Stop_Bay_2

                                    // теперь выставляем нужные
                                    QUIK.Consructor_Transactiy(10, 1, Stop_Bay_1, Plan_Volume_Position, Parametr); // Stop_Bay_1
                                    QUIK.Consructor_Transactiy(10, 2, Stop_Bay_2, Plan_Volume_Position, Parametr); // Stop_Bay_2

                                }


                                

                            }

                            //------------закрыли проверку на скачек цены в сторону открытой позиции-------------////

                        }

                        // --------------- закрыли всю ценовую проверку на позицию в шортах ----- // 

                    }


                }

                

             }

            
           

            if (Regim_Rabota == 1 & stapt_stop == 0)
            {                                  
                    // теперь выставляем нужные
                    QUIK.Consructor_Transactiy(10, 1, Stop_Bay_1, Plan_Volume_Position, Parametr); // Stop_Bay_1
                    QUIK.Consructor_Transactiy(10, 2, Stop_Bay_2, Plan_Volume_Position, Parametr); // Stop_Bay_2
                    // теперь выставляем нужные
                    QUIK.Consructor_Transactiy(9, 1, Stop_Sell_1, Plan_Volume_Position, Parametr); // Stop_Sell_1
                    QUIK.Consructor_Transactiy(9, 2, Stop_Sell_2, Plan_Volume_Position, Parametr); // Stop_Sell_2
                
                stapt_stop++;
            }



            //* вывод расчетов на экран *//
            Close_Position = 0; // обнуляем за ненадобностью
            Rezerv_Rezultat_Sdelki = Rezultat_Sdelki;
            Rezultat_Sdelki = 0; // обнуляем результат сделки
                       

            return str_x;

        }



    }
}
