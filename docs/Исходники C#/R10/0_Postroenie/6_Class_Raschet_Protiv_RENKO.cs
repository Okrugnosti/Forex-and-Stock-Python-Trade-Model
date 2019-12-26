using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R10
{
    // ИНДИКАТОР Protiv_RENKO 5 min_850

    public class Class_Raschet_Protiv_RENKO // работает против трендовой системы
    {
        Class_Capital_System C01 = new Class_Capital_System();

        private decimal Delta_hMAX; // увеличивает отрицательную высоту размаха
        private decimal Delta_hMIN; // увеличивает положительную глубину погружения

        private int Cikl; // количество обращений вокруг точки входа
        private int Plan_Cikl = 0; // счетчик количества обращений вокруг точки входа

        public decimal in_Sell = 0; // точка входа в шорт
        public decimal Buf_in_Sell = 0; // (запоминаем) точка входа в шорт
        public decimal Stop_Bay = 0; // точка выхода из шортов в Cash (убыточная)
        public decimal z_Bay = 0; // заявка на закрытие шортов (прибыльная)
        public decimal Buf_z_Bay = 0; // (запоминаем) заявка на закрытие шортов (прибыльная)
        public decimal Shag_Sell = 0; // уровень против позиции

        public decimal in_Bay = 0; // точка входа в лонг
        public decimal Buf_in_Bay = 0; // (запоминаем) точка входа в лонг
        public decimal Stop_Sell = 0; // точка выхода из покупок в Cash (убыточная)
        public decimal z_Sell = 0; // точка выхода из покупок в Cash (прибыльная)
        public decimal Buf_z_Sell = 0; // (запоминаем) точка выхода из покупок в Cash (прибыльная)
        public decimal Shag_Bay = 0; // уровень против позиции

        public int Volume_Position = 0; // размер текущей торговой позиции (в контрактах)
        public int Plan_Volume_Position; // размер плановой торговой позиции (в контрактах)
        public decimal Vector_Position = 0; // текущее направлении торговой позиции (Bay/Sell/Cash)
        public decimal Bay = 1; // направлении торговой позиции (покупки)
        public decimal Sell = -1; // направление торговой позиции (продажи)
        public decimal Cash = 0; // направлении торговой позции (в деньгах)
        public decimal PriceClose = 0; // значение цены последней сдеки (в пунктах)

        public decimal Open_Position = 0; // цена открытия позиции
        public decimal Close_Position = 0; //уровень закрытия позиции

        public decimal Sum_Sdelok = 0; // счетчик количества сделок
        public decimal Local_Profit; // переменная учитывающая сумму прибыль/убытка от проведенной операции
        public decimal Max_Profit; // рассчет планки прироста капитала
        public decimal Local_Drogdawn; // Рассчет локальной просадки
        public decimal Max_Drogdawn; // расчет максимальной просадки
        public decimal Doli_Drogdawn = 0; // доля текущей просадки от максимальной просадки
        public decimal Rezultat_Sdelki = 0; // результат сделки
        public decimal Rezerv_Rezultat_Sdelki =0;

        public string str_x; // строка для вывода данных в файл




        public decimal Raschet_Protiv_RENKO_5_850(decimal[] Chislo_out, decimal Stop_Sell_2, decimal Stop_Bay_2, decimal Visota_Kanala, decimal Vector_Renko, int i, int Regim_Rabota, int sistem_vector, Parametrs Parametr)
        {
            Delta_hMAX = Parametr.Delta_hMAX;
            Delta_hMIN = Parametr.Delta_hMIN;
            Cikl = Parametr.Cikl;
            Plan_Volume_Position = C01.Stavka_xR01(Parametr);
            
            Class_Consructor_Transactiy QUIK = new Class_Consructor_Transactiy();
            
            
            // если изначально мы находились в кеше и всю торговлю начинаем заново, тогда
            // расставляем стопы на вход в обратном порядке стопам трендовой системы

            if (Vector_Position == Cash & in_Sell == 0 & in_Bay == 0)
            {
                in_Sell = Stop_Bay_2; // ЗАЯВКА на вход в продажи
                in_Bay = Stop_Sell_2; // ЗАЯВКА на вход в покупки

            }

            if (Regim_Rabota == 1 & sistem_vector == 2) // если у нас включен торговый режим
            {
                // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                // в начале все снимаем

                QUIK.Consructor_Transactiy(19, 1, 0, 0, Parametr); // in_Sell - точка входа в шорт
                QUIK.Consructor_Transactiy(29, 1, 0, 0, Parametr); // Stop_Bay - точка выхода из продаж в Cash (убыточная)
                QUIK.Consructor_Transactiy(20, 1, 0, 0, Parametr); // z_Bay - точка выхода из продаж в Cash (прибыльная)

                QUIK.Consructor_Transactiy(20, 2, 0, 0, Parametr); // in_Bay - точка входа в лонг
                QUIK.Consructor_Transactiy(28, 1, 0, 0, Parametr); // Stop_Sell - точка выхода из покупок в Cash (убыточная)
                QUIK.Consructor_Transactiy(19, 2, 0, 0, Parametr); // z_Sell = 0 - точка выхода из покупок в Cash (прибыльная)

                // теперь выставляем нужные
                QUIK.Consructor_Transactiy(1, 1, in_Sell, Plan_Volume_Position, Parametr); // in_Sell - точка входа в шорт
                QUIK.Consructor_Transactiy(2, 2, in_Bay, Plan_Volume_Position, Parametr); // in_Bay - точка входа в лонг
                                            
             }



            //ценовые данные текущего этажа
            // x.Chislo_out[0] - таймфрейм 
            // x.Chislo_out[1] - дата
            // x.Chislo_out[2] - время
            // x.Chislo_out[3] - open
            // x.Chislo_out[4] - max
            // x.Chislo_out[5] - min
            // x.Chislo_out[6] - close


            // если цена пересекает заявку на вход в SHORT
            // входим в продажу

            if (Vector_Position == Cash & Chislo_out[4] >= in_Sell & in_Bay == 0)
            {

                Vector_Position = Sell;
                Volume_Position = Plan_Volume_Position;

                Shag_Sell = in_Sell + Visota_Kanala;
                Stop_Bay = Shag_Sell + (Visota_Kanala * Delta_hMAX); // увеличение

                Buf_in_Sell = in_Sell; // середина

                z_Bay = in_Sell - (Visota_Kanala * Delta_hMIN); // уменьшение
                Buf_z_Bay = z_Bay;
                in_Bay = z_Bay - Visota_Kanala;

                Plan_Cikl++;

                Sum_Sdelok++;
                Open_Position = in_Sell;
                Close_Position = 0;


                if (Regim_Rabota == 1 & sistem_vector == 2) // если у нас включен торговый режим
                {
                    // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                    // в начале все снимаем


                    QUIK.Consructor_Transactiy(19, 1, 0, 0, Parametr); // in_Sell - точка входа в шорт
                    QUIK.Consructor_Transactiy(29, 1, 0, 0, Parametr); // Stop_Bay - точка выхода из продаж в Cash (убыточная)
                    QUIK.Consructor_Transactiy(20, 1, 0, 0, Parametr); // z_Bay - точка выхода из продаж в Cash (прибыльная)

                    QUIK.Consructor_Transactiy(20, 2, 0, 0, Parametr); // in_Bay - точка входа в лонг
                    QUIK.Consructor_Transactiy(28, 1, 0, 0, Parametr); // Stop_Sell - точка выхода из покупок в Cash (убыточная)
                    QUIK.Consructor_Transactiy(19, 2, 0, 0, Parametr); // z_Sell = 0 - точка выхода из покупок в Cash (прибыльная)

                    // теперь выставляем нужные

                    QUIK.Consructor_Transactiy(10, 1, Stop_Bay, Plan_Volume_Position, Parametr); // Stop_Bay - точка выхода из продаж в Cash (убыточная)
                    QUIK.Consructor_Transactiy(2, 1, z_Bay, Plan_Volume_Position, Parametr); // z_Bay - точка выхода из продаж в Cash (прибыльная)
                    QUIK.Consructor_Transactiy(2, 2, in_Bay, Plan_Volume_Position, Parametr); // in_Bay - точка входа в лонг
                 
                }
                
                
                
                in_Sell = 0;

                if (Regim_Rabota == 1 & sistem_vector == 2)
                {
                        // выводим данные на консоль

                        Console.WriteLine(
                        "xR01" + " " +
                        Chislo_out[1] + " " +
                        Chislo_out[2] + " " +
                        "Vector_Position: " + Vector_Position + " " +
                        "Volume_Position: " + Volume_Position + " " +

                        "InSell: " + in_Sell + " " +
                        "SpotBuy: " + Stop_Bay + " " +
                        "z_Buy: " + z_Bay + " " +

                        "in_Buy: " + in_Bay + " " +
                        "Stop_Sell: " + Stop_Sell + " " +
                        "z_Sell: " + z_Sell
                        );// выводим приказ на консоль
                }

            }


            // если цена пересекает заявку на вход в LONG
            // входим в покупки

            if (Vector_Position == Cash & Chislo_out[5] <= in_Bay & in_Sell == 0)
            {

                Vector_Position = Bay;
                Volume_Position = Plan_Volume_Position;

                Shag_Bay = in_Bay - Visota_Kanala;
                Stop_Sell = Shag_Bay - (Visota_Kanala * Delta_hMAX); // увеличение

                Buf_in_Bay = in_Bay; // середина

                z_Sell = in_Bay + (Visota_Kanala * Delta_hMIN); // уменьшение
                Buf_z_Sell = z_Sell;
                in_Sell = z_Sell + Visota_Kanala;

                Plan_Cikl++;

                Sum_Sdelok++;
                Open_Position = in_Bay;
                Close_Position = 0;

                if (Regim_Rabota == 1 & sistem_vector == 2) // если у нас включен торговый режим
                {
                    // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                    // в начале все снимаем

                    QUIK.Consructor_Transactiy(19, 1, 0, 0, Parametr); // in_Sell - точка входа в шорт
                    QUIK.Consructor_Transactiy(29, 1, 0, 0, Parametr); // Stop_Bay - точка выхода из продаж в Cash (убыточная)
                    QUIK.Consructor_Transactiy(20, 1, 0, 0, Parametr); // z_Bay - точка выхода из продаж в Cash (прибыльная)

                    QUIK.Consructor_Transactiy(20, 2, 0, 0, Parametr); // in_Bay - точка входа в лонг
                    QUIK.Consructor_Transactiy(28, 1, 0, 0, Parametr); // Stop_Sell - точка выхода из покупок в Cash (убыточная)
                    QUIK.Consructor_Transactiy(19, 2, 0, 0, Parametr); // z_Sell = 0 - точка выхода из покупок в Cash (прибыльная)

                    // теперь выставляем нужные
                    QUIK.Consructor_Transactiy(1, 1, in_Sell, Plan_Volume_Position, Parametr); // in_Sell - точка входа в шорт

                    QUIK.Consructor_Transactiy(9, 1, Stop_Sell, Plan_Volume_Position, Parametr); // Stop_Sell - точка выхода из покупок в Cash (убыточная)
                    QUIK.Consructor_Transactiy(1, 2, z_Sell, Plan_Volume_Position, Parametr); // z_Sell = 0 - точка выхода из покупок в Cash (прибыльная)
                   
                }

                in_Bay = 0;

                if (Regim_Rabota == 1 & sistem_vector == 2)
                {
                    // выводим данные на консоль

                    Console.WriteLine(
                    "xR01" + " " +
                    Chislo_out[1] + " " +
                    Chislo_out[2] + " " +
                    "Vector_Position: " + Vector_Position + " " +
                    "Volume_Position: " + Volume_Position + " " +

                    "InSell: " + in_Sell + " " +
                    "SpotBuy: " + Stop_Bay + " " +
                    "z_Buy: " + z_Bay + " " +

                    "in_Buy: " + in_Bay + " " +
                    "Stop_Sell: " + Stop_Sell + " " +
                    "z_Sell: " + z_Sell
                    );// выводим приказ на консоль
                }
            }


            // если мы в CASH, но пересечения не произашло. но уровень стопов в ренков изменился
            // тогда подтягиваем наши стопы

            if (Vector_Position == Cash & in_Sell != 0)
            {
                in_Sell = Stop_Bay_2;

                if (Regim_Rabota == 1 & sistem_vector == 2) // если у нас включен торговый режим
                {
                    // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                    // в начале все снимаем

                    QUIK.Consructor_Transactiy(19, 1, 0, 0, Parametr); // in_Sell - точка входа в шорт
                    

                    // теперь выставляем нужные
                    QUIK.Consructor_Transactiy(1, 1, in_Sell, Plan_Volume_Position, Parametr); // in_Sell - точка входа в шорт
                    
                    
                }

            }


            if (Vector_Position == Cash & in_Bay != 0)
            {

                in_Bay = Stop_Sell_2;

                if (Regim_Rabota == 1 & sistem_vector == 2) // если у нас включен торговый режим
                {
                    // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                    // в начале все снимаем
                    QUIK.Consructor_Transactiy(20, 2, 0, 0, Parametr); // in_Bay - точка входа в лонг
                    
                    // теперь выставляем нужные
                    QUIK.Consructor_Transactiy(2, 2, in_Bay, Plan_Volume_Position, Parametr); // in_Bay - точка входа в лонг
                    
                }
            }

            // если мы в SHORT
            // цена опускается до уровня фиксации ПРИБЫЛИ zBay


            if (Vector_Position == Sell & Chislo_out[5] <= z_Bay & Volume_Position == Plan_Volume_Position & Plan_Cikl != Cikl)
            {
                in_Sell = Buf_in_Sell; // середина

                Buf_z_Bay = z_Bay;
                z_Bay = 0; // уменьшение 


                if (Regim_Rabota == 1 & sistem_vector == 2) // если у нас включен торговый режим
                {
                    // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                    // в начале все снимаем

                    QUIK.Consructor_Transactiy(19, 1, 0, 0, Parametr); // in_Sell - точка входа в шорт
                    QUIK.Consructor_Transactiy(20, 1, 0, 0, Parametr); // z_Bay - точка выхода из продаж в Cash (прибыльная)
                                       

                    // теперь выставляем нужные
                    QUIK.Consructor_Transactiy(1, 1, in_Sell, Plan_Volume_Position, Parametr); // in_Sell - точка входа в шорт
                                        
                }
                
                
                Volume_Position = 0;

                Plan_Cikl++;


                Sum_Sdelok++;
                Close_Position = Buf_z_Bay;
                Local_Profit = Local_Profit + (Open_Position - Close_Position);
                Rezultat_Sdelki = (Open_Position - Close_Position) / Visota_Kanala;


                if (Regim_Rabota == 1 & sistem_vector == 2)
                {
                        // выводим данные на консоль

                        Console.WriteLine(
                        "xR01" + " " +
                        Chislo_out[1] + " " +
                        Chislo_out[2] + " " +
                        "Vector_Position: " + Vector_Position + " " +
                        "Volume_Position: " + Volume_Position + " " +

                        "InSell: " + in_Sell + " " +
                        "SpotBuy: " + Stop_Bay + " " +
                        "z_Buy: " + z_Bay + " " +

                        "in_Buy: " + in_Bay + " " +
                        "Stop_Sell: " + Stop_Sell + " " +
                        "z_Sell: " + z_Sell
                        );// выводим приказ на консоль
                }

            }


            // если мы в LONG 
            // цена поднимается до уровня фиксации ПРИБЫЛИ zSell

            if (Vector_Position == Bay & Chislo_out[4] >= z_Sell & Volume_Position == Plan_Volume_Position & Plan_Cikl != Cikl)
            {

                in_Bay = Buf_in_Bay;

                Buf_z_Sell = z_Sell;
                z_Sell = 0;

                if (Regim_Rabota == 1 & sistem_vector == 2) // если у нас включен торговый режим
                {
                    // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                    // в начале все снимаем

                    QUIK.Consructor_Transactiy(20, 2, 0, 0, Parametr); // in_Bay - точка входа в лонг
                    QUIK.Consructor_Transactiy(19, 2, 0, 0, Parametr); // z_Sell = 0 - точка выхода из покупок в Cash (прибыльная)
                    
                    // теперь выставляем нужные
                    QUIK.Consructor_Transactiy(2, 2, in_Bay, Plan_Volume_Position, Parametr); // in_Bay - точка входа в лонг
                    
                }


                Volume_Position = 0;

                Plan_Cikl++;

                Sum_Sdelok++;
                Close_Position = Buf_z_Sell;
                Local_Profit = Local_Profit + (Close_Position - Open_Position);
                Rezultat_Sdelki = (Close_Position - Open_Position) / Visota_Kanala;


                if (Regim_Rabota == 1 & sistem_vector == 2)
                {
                    // выводим данные на консоль

                    Console.WriteLine(
                    "xR01" + " " +
                    Chislo_out[1] + " " +
                    Chislo_out[2] + " " +
                    "Vector_Position: " + Vector_Position + " " +
                    "Volume_Position: " + Volume_Position + " " +

                    "InSell: " + in_Sell + " " +
                    "SpotBuy: " + Stop_Bay + " " +
                    "z_Buy: " + z_Bay + " " +

                    "in_Buy: " + in_Bay + " " +
                    "Stop_Sell: " + Stop_Sell + " " +
                    "z_Sell: " + z_Sell
                    );// выводим приказ на консоль
                }

            }


            // еслим мы в SHORT
            // цена поднимается до уровня начального входа in_Sell

            if (Vector_Position == Sell & Chislo_out[4] >= Buf_in_Sell & Volume_Position == 0 & Plan_Cikl != Cikl)
            {

                Buf_in_Sell = in_Sell;
                z_Bay = Buf_z_Bay;


                if (Regim_Rabota == 1 & sistem_vector == 2) // если у нас включен торговый режим
                {
                    // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                    // в начале все снимаем

                    QUIK.Consructor_Transactiy(19, 1, 0, 0, Parametr); // in_Sell - точка входа в шорт
                    QUIK.Consructor_Transactiy(20, 1, 0, 0, Parametr); // z_Bay - точка выхода из продаж в Cash (прибыльная)
                    
                    // теперь выставляем нужные
                    QUIK.Consructor_Transactiy(2, 1, z_Bay, Plan_Volume_Position, Parametr); // z_Bay - точка выхода из продаж в Cash (прибыльная)
                                        
                }


                Volume_Position = Plan_Volume_Position;

                Sum_Sdelok++;
                Open_Position = in_Sell;

                Plan_Cikl++;


                if (Regim_Rabota == 1 & sistem_vector == 2)
                {
                    // выводим данные на консоль

                    Console.WriteLine(
                    "xR01" + " " +
                    Chislo_out[1] + " " +
                    Chislo_out[2] + " " +
                    "Vector_Position: " + Vector_Position + " " +
                    "Volume_Position: " + Volume_Position + " " +

                    "InSell: " + in_Sell + " " +
                    "SpotBuy: " + Stop_Bay + " " +
                    "z_Buy: " + z_Bay + " " +

                    "in_Buy: " + in_Bay + " " +
                    "Stop_Sell: " + Stop_Sell + " " +
                    "z_Sell: " + z_Sell
                    );// выводим приказ на консоль
                }
            }


            // если мы в LONG
            // цена поднимается до уровня начального входа в in_Bay


            if (Vector_Position == Bay & Chislo_out[5] <= Buf_in_Bay & Volume_Position == 0 & Plan_Cikl != Cikl)
            {

                Buf_in_Bay = in_Bay;
                z_Sell = Buf_z_Sell;


                if (Regim_Rabota == 1 & sistem_vector == 2) // если у нас включен торговый режим
                {
                    // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                    // в начале все снимаем

                    QUIK.Consructor_Transactiy(20, 2, 0, 0, Parametr); // in_Bay - точка входа в лонг
                    QUIK.Consructor_Transactiy(19, 2, 0, 0, Parametr); // z_Sell = 0 - точка выхода из покупок в Cash (прибыльная)

                    // теперь выставляем нужные
                    QUIK.Consructor_Transactiy(1, 2, z_Sell, Plan_Volume_Position, Parametr); // z_Sell = 0 - точка выхода из покупок в Cash (прибыльная)
                    
                }
                
                
                Volume_Position = Plan_Volume_Position;

                Plan_Cikl++;

                Sum_Sdelok++;
                Open_Position = in_Bay;

                if (Regim_Rabota == 1 & sistem_vector == 2)
                {
                    // выводим данные на консоль

                    Console.WriteLine(
                    "xR01" + " " +
                    Chislo_out[1] + " " +
                    Chislo_out[2] + " " +
                    "Vector_Position: " + Vector_Position + " " +
                    "Volume_Position: " + Volume_Position + " " +

                    "InSell: " + in_Sell + " " +
                    "SpotBuy: " + Stop_Bay + " " +
                    "z_Buy: " + z_Bay + " " +

                    "in_Buy: " + in_Bay + " " +
                    "Stop_Sell: " + Stop_Sell + " " +
                    "z_Sell: " + z_Sell
                    );// выводим приказ на консоль
                }

            }


            // если мы в SHORT 
            // цена опускается до уровня переворота в LONG

            if (Vector_Position == Sell & Chislo_out[5] <= in_Bay)
            {

                Shag_Sell = 0;
                Stop_Bay = 0; // увеличение

                in_Sell = 0;
                Buf_in_Sell = 0; // середина

                z_Bay = 0; // уменьшение
                Buf_z_Bay = 0;


                Vector_Position = Bay;
                Volume_Position = Plan_Volume_Position;

                Shag_Bay = in_Bay - Visota_Kanala;
                Stop_Sell = Shag_Bay - Visota_Kanala; // увеличение

                Buf_in_Bay = in_Bay; // середина

                z_Sell = in_Bay + Visota_Kanala; // уменьшение
                Buf_z_Sell = z_Sell;
                in_Sell = z_Sell + Visota_Kanala;


                if (Regim_Rabota == 1 & sistem_vector == 2) // если у нас включен торговый режим
                {
                    // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                    // в начале все снимаем


                    QUIK.Consructor_Transactiy(19, 1, 0, 0, Parametr); // in_Sell - точка входа в шорт
                    QUIK.Consructor_Transactiy(29, 1, 0, 0, Parametr); // Stop_Bay - точка выхода из продаж в Cash (убыточная)
                    QUIK.Consructor_Transactiy(20, 1, 0, 0, Parametr); // z_Bay - точка выхода из продаж в Cash (прибыльная)

                    QUIK.Consructor_Transactiy(20, 2, 0, 0, Parametr); // in_Bay - точка входа в лонг
                    QUIK.Consructor_Transactiy(28, 1, 0, 0, Parametr); // Stop_Sell - точка выхода из покупок в Cash (убыточная)
                    QUIK.Consructor_Transactiy(19, 2, 0, 0, Parametr); // z_Sell = 0 - точка выхода из покупок в Cash (прибыльная)

                    // теперь выставляем нужные
                    QUIK.Consructor_Transactiy(1, 1, in_Sell, Plan_Volume_Position, Parametr); // in_Sell - точка входа в шорт
                    QUIK.Consructor_Transactiy(9, 1, Stop_Sell, Plan_Volume_Position, Parametr); // Stop_Sell - точка выхода из покупок в Cash (убыточная)
                    QUIK.Consructor_Transactiy(1, 2, z_Sell, Plan_Volume_Position, Parametr); // z_Sell = 0 - точка выхода из покупок в Cash (прибыльная)
                 
                }


                in_Bay = 0;

                Sum_Sdelok++;
                Open_Position = Buf_in_Bay;


                if (Regim_Rabota == 1 & sistem_vector == 2)
                {
                    // выводим данные на консоль

                    Console.WriteLine(
                    "xR01" + " " +
                    Chislo_out[1] + " " +
                    Chislo_out[2] + " " +
                    "Vector_Position: " + Vector_Position + " " +
                    "Volume_Position: " + Volume_Position + " " +

                    "InSell: " + in_Sell + " " +
                    "SpotBuy: " + Stop_Bay + " " +
                    "z_Buy: " + z_Bay + " " +

                    "in_Buy: " + in_Bay + " " +
                    "Stop_Sell: " + Stop_Sell + " " +
                    "z_Sell: " + z_Sell
                    );// выводим приказ на консоль
                }

            }


            // если мы в LONG
            // цена поднимается до уровня переворота в SHORT 

            if (Vector_Position == Bay & Chislo_out[4] >= in_Sell)
            {

                Shag_Bay = 0;
                Stop_Sell = 0; // увеличение

                in_Bay = 0;
                Buf_in_Bay = 0; // середина

                z_Sell = 0; // уменьшение
                Buf_z_Sell = 0;

                Vector_Position = Sell;
                Volume_Position = Plan_Volume_Position;

                Shag_Sell = in_Sell + Visota_Kanala;
                Stop_Bay = Shag_Sell + Visota_Kanala; // увеличение

                Buf_in_Sell = in_Sell; // середина

                z_Bay = in_Sell - Visota_Kanala; // уменьшение
                Buf_z_Bay = z_Bay;
                in_Bay = z_Bay - Visota_Kanala;


                if (Regim_Rabota == 1 & sistem_vector == 2) // если у нас включен торговый режим
                {
                    // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                    // в начале все снимаем

                    QUIK.Consructor_Transactiy(19, 1, 0, 0, Parametr); // in_Sell - точка входа в шорт
                    QUIK.Consructor_Transactiy(29, 1, 0, 0, Parametr); // Stop_Bay - точка выхода из продаж в Cash (убыточная)
                    QUIK.Consructor_Transactiy(20, 1, 0, 0, Parametr); // z_Bay - точка выхода из продаж в Cash (прибыльная)

                    QUIK.Consructor_Transactiy(20, 2, 0, 0, Parametr); // in_Bay - точка входа в лонг
                    QUIK.Consructor_Transactiy(28, 1, 0, 0, Parametr); // Stop_Sell - точка выхода из покупок в Cash (убыточная)
                    QUIK.Consructor_Transactiy(19, 2, 0, 0, Parametr); // z_Sell = 0 - точка выхода из покупок в Cash (прибыльная)

                    // теперь выставляем нужные
                    QUIK.Consructor_Transactiy(10, 1, Stop_Bay, Plan_Volume_Position, Parametr); // Stop_Bay - точка выхода из продаж в Cash (убыточная)
                    QUIK.Consructor_Transactiy(2, 1, z_Bay, Plan_Volume_Position, Parametr); // z_Bay - точка выхода из продаж в Cash (прибыльная)

                    QUIK.Consructor_Transactiy(2, 2, in_Bay, Plan_Volume_Position, Parametr); // in_Bay - точка входа в лонг
                    
                }
                
                
                
                in_Sell = 0;

                Sum_Sdelok++;
                Open_Position = Buf_in_Sell;


                if (Regim_Rabota == 1 & sistem_vector == 2)
                {
                    // выводим данные на консоль

                    Console.WriteLine(
                    "xR01" + " " +
                    Chislo_out[1] + " " +
                    Chislo_out[2] + " " +
                    "Vector_Position: " + Vector_Position + " " +
                    "Volume_Position: " + Volume_Position + " " +

                    "InSell: " + in_Sell + " " +
                    "SpotBuy: " + Stop_Bay + " " +
                    "z_Buy: " + z_Bay + " " +

                    "in_Buy: " + in_Bay + " " +
                    "Stop_Sell: " + Stop_Sell + " " +
                    "z_Sell: " + z_Sell
                    );// выводим приказ на консоль
                }

            }


            // если мы SHORT
            // цена поднимается до уровня передвижения стопа +1 по RENKO

            if (Vector_Position == Sell & Chislo_out[8] >= Shag_Sell & Chislo_out[7] == 1)
            {

                in_Bay = Stop_Sell_2;
                z_Bay = Stop_Sell_2;


                if (Regim_Rabota == 1 & sistem_vector == 2) // если у нас включен торговый режим
                {
                    // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                    // в начале все снимаем
                    QUIK.Consructor_Transactiy(20, 1, 0, 0, Parametr); // z_Bay - точка выхода из продаж в Cash (прибыльная)
                    QUIK.Consructor_Transactiy(20, 2, 0, 0, Parametr); // in_Bay - точка входа в лонг
                    

                    // теперь выставляем нужные
                    QUIK.Consructor_Transactiy(2, 1, z_Bay, Plan_Volume_Position, Parametr); // z_Bay - точка выхода из продаж в Cash (прибыльная)
                    QUIK.Consructor_Transactiy(2, 2, in_Bay, Plan_Volume_Position, Parametr); // in_Bay - точка входа в лонг
                    
                }


                if (Regim_Rabota == 1 & sistem_vector == 2)
                {
                    // выводим данные на консоль

                    Console.WriteLine(
                    "xR01" + " " +
                    Chislo_out[1] + " " +
                    Chislo_out[2] + " " +
                    "Vector_Position: " + Vector_Position + " " +
                    "Volume_Position: " + Volume_Position + " " +

                    "InSell: " + in_Sell + " " +
                    "SpotBuy: " + Stop_Bay + " " +
                    "z_Buy: " + z_Bay + " " +

                    "in_Buy: " + in_Bay + " " +
                    "Stop_Sell: " + Stop_Sell + " " +
                    "z_Sell: " + z_Sell
                    );// выводим приказ на консоль
                }

            }

            // если мы в LONG 
            // цена опускается до уровня передвижения стопа -1 по RENKO


            if (Vector_Position == Bay & Chislo_out[8] <= Shag_Bay & Chislo_out[7] == 1)
            {

                in_Sell = Stop_Bay_2;
                z_Sell = Stop_Bay_2;


                if (Regim_Rabota == 1 & sistem_vector == 2) // если у нас включен торговый режим
                {
                    // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                    // в начале все снимаем

                    QUIK.Consructor_Transactiy(19, 1, 0, 0, Parametr); // in_Sell - точка входа в шорт
                    QUIK.Consructor_Transactiy(19, 2, 0, 0, Parametr); // z_Sell = 0 - точка выхода из покупок в Cash (прибыльная)

                    // теперь выставляем нужные
                    QUIK.Consructor_Transactiy(1, 1, in_Sell, Plan_Volume_Position, Parametr); // in_Sell - точка входа в шорт
                    QUIK.Consructor_Transactiy(1, 2, z_Sell, Plan_Volume_Position, Parametr); // z_Sell = 0 - точка выхода из покупок в Cash (прибыльная)
                    
                }


                if (Regim_Rabota == 1 & sistem_vector == 2)
                {
                    // выводим данные на консоль

                    Console.WriteLine(
                    "xR01" + " " +
                    Chislo_out[1] + " " +
                    Chislo_out[2] + " " +
                    "Vector_Position: " + Vector_Position + " " +
                    "Volume_Position: " + Volume_Position + " " +

                    "InSell: " + in_Sell + " " +
                    "SpotBuy: " + Stop_Bay + " " +
                    "z_Buy: " + z_Bay + " " +

                    "in_Buy: " + in_Bay + " " +
                    "Stop_Sell: " + Stop_Sell + " " +
                    "z_Sell: " + z_Sell
                    );// выводим приказ на консоль
                }

            }

            // если мы в SHORT
            // цена поднимается до уровня выхода в CASH

            if (Vector_Position == Sell & Chislo_out[4] >= Stop_Bay)
            {

                Vector_Position = Cash;
                Volume_Position = 0;

                Shag_Sell = 0;
                in_Sell = 0;
                Buf_in_Sell = 0;
                z_Bay = 0;
                Buf_z_Bay = 0;

                in_Bay = Stop_Sell_2;

                Sum_Sdelok++;
                Close_Position = Stop_Bay;
                Local_Profit = Local_Profit + (Open_Position - Close_Position);
                Rezultat_Sdelki = (Open_Position - Close_Position) / Visota_Kanala;

                if (Regim_Rabota == 1 & sistem_vector == 2) // если у нас включен торговый режим
                {
                    // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                    // в начале все снимаем

                    QUIK.Consructor_Transactiy(19, 1, 0, 0, Parametr); // in_Sell - точка входа в шорт
                    QUIK.Consructor_Transactiy(29, 1, 0, 0, Parametr); // Stop_Bay - точка выхода из продаж в Cash (убыточная)
                    QUIK.Consructor_Transactiy(20, 1, 0, 0, Parametr); // z_Bay - точка выхода из продаж в Cash (прибыльная)

                    QUIK.Consructor_Transactiy(20, 2, 0, 0, Parametr); // in_Bay - точка входа в лонг
                    QUIK.Consructor_Transactiy(28, 1, 0, 0, Parametr); // Stop_Sell - точка выхода из покупок в Cash (убыточная)
                    QUIK.Consructor_Transactiy(19, 2, 0, 0, Parametr); // z_Sell = 0 - точка выхода из покупок в Cash (прибыльная)

                    // теперь выставляем нужные
                    QUIK.Consructor_Transactiy(2, 2, in_Bay, Plan_Volume_Position, Parametr); // in_Bay - точка входа в лонг
                                        
                }

                Stop_Bay = 0;


                if (Regim_Rabota == 1 & sistem_vector == 2)
                {
                    // выводим данные на консоль

                    Console.WriteLine(
                    "xR01" + " " +
                    Chislo_out[1] + " " +
                    Chislo_out[2] + " " +
                    "Vector_Position: " + Vector_Position + " " +
                    "Volume_Position: " + Volume_Position + " " +

                    "InSell: " + in_Sell + " " +
                    "SpotBuy: " + Stop_Bay + " " +
                    "z_Buy: " + z_Bay + " " +

                    "in_Buy: " + in_Bay + " " +
                    "Stop_Sell: " + Stop_Sell + " " +
                    "z_Sell: " + z_Sell
                    );// выводим приказ на консоль
                }

            }


            // если мы в LONG
            // цена опускается до уровня выхода в CASH

            if (Vector_Position == Bay & Chislo_out[5] <= Stop_Sell)
            {
                Vector_Position = Cash;
                Volume_Position = 0;

                Shag_Bay = 0;
                in_Bay = 0;
                Buf_in_Bay = 0;
                z_Sell = 0;
                Buf_z_Sell = 0;

                in_Sell = Stop_Bay_2;

                Sum_Sdelok++;
                Close_Position = Stop_Sell;
                Local_Profit = Local_Profit + (Close_Position - Open_Position);
                Rezultat_Sdelki = (Close_Position - Open_Position) / Visota_Kanala;

                if (Regim_Rabota == 1 & sistem_vector == 2) // если у нас включен торговый режим
                {
                    // int COD_Transactions_x, int COD_Transactions_z, decimal PRICE, int QUANTITY
                    // в начале все снимаем

                    QUIK.Consructor_Transactiy(19, 1, 0, 0, Parametr); // in_Sell - точка входа в шорт
                    QUIK.Consructor_Transactiy(29, 1, 0, 0, Parametr); // Stop_Bay - точка выхода из продаж в Cash (убыточная)
                    QUIK.Consructor_Transactiy(20, 1, 0, 0, Parametr); // z_Bay - точка выхода из продаж в Cash (прибыльная)

                    QUIK.Consructor_Transactiy(20, 2, 0, 0, Parametr); // in_Bay - точка входа в лонг
                    QUIK.Consructor_Transactiy(28, 1, 0, 0, Parametr); // Stop_Sell - точка выхода из покупок в Cash (убыточная)
                    QUIK.Consructor_Transactiy(19, 2, 0, 0, Parametr); // z_Sell = 0 - точка выхода из покупок в Cash (прибыльная)

                    // теперь выставляем нужные
                    QUIK.Consructor_Transactiy(1, 1, in_Sell, Plan_Volume_Position, Parametr); // in_Sell - точка входа в шорт
                    
                }

                Stop_Sell = 0;


                if (Regim_Rabota == 1 & sistem_vector == 2)
                {
                    // выводим данные на консоль

                    Console.WriteLine(
                    "xR01" + " " +
                    Chislo_out[1] + " " +
                    Chislo_out[2] + " " +
                    "Vector_Position: " + Vector_Position + " " +
                    "Volume_Position: " + Volume_Position + " " +

                    "InSell: " + in_Sell + " " +
                    "SpotBuy: " + Stop_Bay + " " +
                    "z_Buy: " + z_Bay + " " +

                    "in_Buy: " + in_Bay + " " +
                    "Stop_Sell: " + Stop_Sell + " " +
                    "z_Sell: " + z_Sell
                    );// выводим приказ на консоль
                }

                if (Regim_Rabota == 1 & sistem_vector == 2)
                {
                    QUIK.Consructor_Transactiy(0, 1, 0, 0, Parametr); // вызов функции перенесения заявок
                }

            }



            // рассчет оценки рисков // 


            if (Max_Profit < Local_Profit) // расчет планки максимального капитала
            {
                Max_Profit = Local_Profit;
            }

            Local_Drogdawn = Local_Profit - Max_Profit - 0.001m; //рассчет локальной просадки

            if (Max_Drogdawn > Local_Drogdawn) // расчет планки максимальной просадки
            {
                Max_Drogdawn = Local_Drogdawn;
            }

            if (Max_Drogdawn != 0)// расчет доли от максимальной просадки
            {
                Doli_Drogdawn = Local_Drogdawn / Max_Drogdawn;
            }

            str_x = "." + Sum_Sdelok.ToString() + "." + Vector_Position.ToString() + "." + Open_Position.ToString() + "." + Close_Position.ToString()
                     + "." + Stop_Sell.ToString() + "." + Shag_Bay.ToString() + "." + z_Sell.ToString()
                     + "." + z_Bay.ToString() + "." + Shag_Sell.ToString() + "." + Stop_Bay.ToString()
                     + "." + Rezultat_Sdelki.ToString() + "." + Local_Profit.ToString() + "." + Max_Profit.ToString() + "." + Local_Drogdawn.ToString() + "." + Max_Drogdawn.ToString()
                     + "." + Doli_Drogdawn.ToString() + "." + sistem_vector.ToString();

            Rezerv_Rezultat_Sdelki = Rezultat_Sdelki * Visota_Kanala;
            Rezultat_Sdelki = 0;


            return 0;
        }


    }
}
