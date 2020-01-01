using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// Math; // http://msdn.microsoft.com/ru-ru/library/system.math.aspx

namespace R10
{
    public class Analiz_Dohod_Sistem
    {
        // БЛОК А
        public decimal SummSdelok = 0; // +Общее число сделок, шт
        public decimal Summ_Prib_Sdelok = 0; // +Сумма приб. сделок, шт
        public decimal Summ_Ubitok_Sdelok = 0; // +Сумма убыт. сделок, шт.
        public decimal Percent_Prib_Sdelok = 0;// +Процент приб. сделок
        public decimal Percent_Ubitok_Sdelok = 0;// +Процент убыт. сделок
        public decimal Sootnosh_Pr_Ub_Sdelok = 0;// +Соотношение между кол. Пр/уб сделок

        // БЛОК Б
        public decimal Summ_Local_Profits_Sdelok = 0; // +Сумма результатов только прибыльных сделок, руб
        public decimal Srenb_Prib_na_Sd = 0;// +Средняя прибыль на сделку, руб.
        public decimal Sred_Percent_Prib_na_Sd = 0;// +Средняя прибыль за сделку, %
        
        public decimal Summ_Local_Drogdawn_Sdelok = 0; // +Сумма результатов только убыточных сделок, руб.
        public decimal Srenb_Ubit_na_Sd = 0;// +Средний убыток на сделку, руб
        public decimal Sred_Percent_Ubit_na_Sd = 0;// +Средний убыток за сделку, %

        public decimal Sootnosh_Sred_Pr_Ub_Sdelok = 0; // +Соотношение между ср. приб/уб. На сделку
        public decimal Sootnosh_Summ_Loc_Pro_Ub_Sdelok = 0; // Соотношение между суммой рузул. приб/уб. Сделок
                
        // БЛОК В
        public decimal Max_Profit_One_Sdelka = 0; // +максимальная прибыль в одной сделке, руб.
        public decimal Percent_Max_Profit_One_Sdelka = 0; // процент максимальной прибыль в одной сделке, %.
                
        public decimal Max_Drogdawn_One_Sdelka = 0; // +максимальный убыток в одной сделке, руб.
        public decimal Percent_Max_Drogdawn_One_Sdelka = 0; // процент максимального убыток в одной сделке, %.
                

        // БЛОК Г
        public decimal Max_Drogdawn = 0; // +Максимальная просадка, руб.
                
        // БЛОК Д 
        public decimal Start_Capital = 0;//+Стартовый капиталл, руб

        // БЛОК Е
        public decimal Max_Profit = 0;// +Максимальная прибыль, руб
        public decimal Local_Profit_Percent = 0;// +Чистая прибыль, %.
        public decimal Local_Profit = 0; // +Чистая прибыль, руб

        public decimal Premia_Za_Risk = 0; // Премия за риск (соотношение чистой прибыли, на макс просадку, %)

        public decimal Local_Drogdawn = 0; // +текущий уровень просадки, руб
        public decimal Doli_Drogdawn = 0; // +текущая глубина просадки в % от максимальной просадки
                
        // БЛОК Ж
        public decimal Mat_Og = 0;// +Математическое ожидание, руб
        public decimal Percent_Mat_Og = 0;// Математическое ожидание, %
         
        public string Str_x = "";
        public string[] Reestr = new string[30];

        // БЛОК Metod_Raschet_Korreliacii
        

        private int u = 0; // переменная цикла
        
        public const int Razmer_V = 100000; // глубина выборки в массивах
        public int Razmer_V2 = 0; // глубина выборки в массивов с учетом корректировок
        public int Sdvig = 75000; // сдвиг времени, для корректного отображдения расчетов.
                
        public decimal[] Baza_Prosadok = new decimal[Razmer_V]; // создаем массив для хранения выборки просадок
        public decimal[] Baza_Volatiliti = new decimal[Razmer_V]; // создаем массив для хранения выборки волатильности

        public decimal Sred_Pros = 0; // средняя просадок
        public decimal Sred_Vol = 0; // средняя волатильности

        public decimal Delta_Pros, Delta_Pros_Coren = 0; // делители и знаменатели
        public decimal Delta_Vol, Delta_Vol_Coren = 0; // делители и знаменатели

        public int x, i; 

        public decimal Delitel;
        public decimal Znamenatel;

        public decimal Koreliacia;
        public decimal Standart_Otlonenie_Pros;
        public decimal Standart_Otlonenie_Vol;

        public decimal[] Reesr_Koreliacia = new decimal[3000]; // создаем массив для хранения реестра корреляций

        public string Metod_Analiz_Dohod_Sistem(decimal Rezultat_Sdelki, Parametrs Parametr) // система по анализу результативности системы
        {

            Rezultat_Sdelki = Rezultat_Sdelki * Parametr.Price_SHAG_CENA; // выражаем в УЕ результат сделки
            Local_Profit = Local_Profit + Rezultat_Sdelki; // чистая прибыль, руб

            
            // БЛОК-А
            if (Rezultat_Sdelki != 0)
            {
                SummSdelok++; // Общее число сделок

                if (Rezultat_Sdelki > 0) 
                {
                    Summ_Prib_Sdelok++; // Сумма приб. сделок
                    Percent_Prib_Sdelok = Summ_Prib_Sdelok / SummSdelok * 100; //Процент приб. сделок

                    Summ_Local_Profits_Sdelok = Summ_Local_Profits_Sdelok + Rezultat_Sdelki; // Сумма результатов только прибыльных сделок
                    Srenb_Prib_na_Sd = Summ_Local_Profits_Sdelok / Summ_Prib_Sdelok; // Средняя прибыль на сделку, руб

                    if (Rezultat_Sdelki > Max_Profit_One_Sdelka)
                    {
                        Max_Profit_One_Sdelka = Rezultat_Sdelki; // максимальная прибыль в одной сделке, руб.
                    }

                    if (Max_Profit < Local_Profit)
                    {
                        Max_Profit = Local_Profit; // Максимальная прибыль, руб
                    }

                }

                Local_Drogdawn = Local_Profit - Max_Profit - 0.001m; //рассчет локальной просадки

                if (Rezultat_Sdelki < 0)
                {
                    Summ_Ubitok_Sdelok++; // Сумма убыт. сделок
                    Percent_Ubitok_Sdelok = Summ_Ubitok_Sdelok / SummSdelok * 100; //Процент убыт. сделок

                    Summ_Local_Drogdawn_Sdelok = Summ_Local_Drogdawn_Sdelok + Rezultat_Sdelki; // Сумма результатов только убыточных сделок
                    Srenb_Ubit_na_Sd = Summ_Local_Drogdawn_Sdelok / Summ_Ubitok_Sdelok; // Средний убыток на сделку, руб

                    if (Rezultat_Sdelki < Max_Profit_One_Sdelka)
                    {
                        Max_Drogdawn_One_Sdelka = Rezultat_Sdelki; // максимальный убыток в одной сделке, руб.
                    }

                    if (Max_Drogdawn > Local_Drogdawn) 
                    {
                        Max_Drogdawn = Local_Drogdawn; // Максимальная просадка, руб.
                    }
                    
                }
 
            }
                              

            if (Max_Drogdawn != 0)// расчет доли от максимальной просадки
            {
                Doli_Drogdawn = Local_Drogdawn / Max_Drogdawn *100;
            }

            Start_Capital = Max_Drogdawn / 30 * -100; //Стартовый капиталл, руб

            if (Summ_Ubitok_Sdelok != 0)
            {
                Sootnosh_Pr_Ub_Sdelok = Summ_Prib_Sdelok / Summ_Ubitok_Sdelok; //Соотношение между кол. Пр/уб сделок


                Sred_Percent_Prib_na_Sd = Srenb_Prib_na_Sd / Start_Capital * 100; // Средняя прибыль за сделку, %
                Sred_Percent_Ubit_na_Sd = Srenb_Ubit_na_Sd / Start_Capital * 100; // Средний убыток за сделку, %

                Percent_Max_Profit_One_Sdelka = Max_Profit_One_Sdelka / Start_Capital * 100; // процент максимальной прибыль в одной сделке, %.
                Percent_Max_Drogdawn_One_Sdelka = Max_Drogdawn_One_Sdelka / Start_Capital * 100; // процент максимального убыток в одной сделке, %

                Sootnosh_Sred_Pr_Ub_Sdelok = Sred_Percent_Prib_na_Sd / -Sred_Percent_Ubit_na_Sd; // +Соотношение между ср. приб/уб. На сделку
                Sootnosh_Summ_Loc_Pro_Ub_Sdelok = Summ_Local_Profits_Sdelok / -Summ_Local_Drogdawn_Sdelok; // Соотношение между суммой рузул. приб/уб. Сделок

                Local_Profit_Percent = Local_Profit / Start_Capital * 100; // Чистая доходность, в %
                Premia_Za_Risk = Local_Profit_Percent / 30; // Премия за риск (соотношение чистой прибыли, на макс просадку, %)


                Mat_Og = Local_Profit / SummSdelok; // математическое ожидание, руб.
                Percent_Mat_Og = Mat_Og / Start_Capital * 100; // математическое ожидание, %
            }

            // вывод расчетов (сокращенная версия)
            //Str_x = Rezultat_Sdelki.ToString() + ";" + Local_Profit.ToString() + ";" + Max_Profit.ToString() + ";" +
            //        Local_Drogdawn.ToString() + ";" + Max_Drogdawn.ToString(); 
                     
            // вывод расчет (полня версия)
            Str_x =
                // БЛОК А
                SummSdelok + ";" +  // Общее число сделок, шт
                Summ_Prib_Sdelok + ";" +  // Сумма приб. сделок, шт
                Percent_Prib_Sdelok + ";" + // Процент приб. сделок
                Summ_Ubitok_Sdelok + ";" +  // Сумма убыт. сделок, шт.
                Percent_Ubitok_Sdelok + ";" + // Процент убыт. сделок
                Sootnosh_Pr_Ub_Sdelok + ";" +  // Соотношение между кол. Пр/уб сделок

                // БЛОК Б
                Summ_Local_Profits_Sdelok + ";" + // Сумма результатов только прибыльных сделок, руб
                Srenb_Prib_na_Sd + ";" + // Средняя прибыль на сделку, руб.
                Sred_Percent_Prib_na_Sd + ";" +  // Средняя прибыль за сделку, %

                Summ_Local_Drogdawn_Sdelok + ";" +  // Сумма результатов только убыточных сделок, руб.
                Srenb_Ubit_na_Sd + ";" + // Средний убыток на сделку, руб
                Sred_Percent_Ubit_na_Sd + ";" + // Средний убыток за сделку, %

                Sootnosh_Sred_Pr_Ub_Sdelok + ";" + // +Соотношение между ср. приб/уб. На сделку
                Sootnosh_Summ_Loc_Pro_Ub_Sdelok + ";" + // Соотношение между суммой рузул. приб/уб. Сделок

                // БЛОК В
                Max_Profit_One_Sdelka + ";" + // максимальная прибыль в одной сделке, руб.
                Percent_Max_Profit_One_Sdelka + ";" + // процент максимальной прибыль в одной сделке, %.
                
                Max_Drogdawn_One_Sdelka + ";" + // максимальный убыток в одной сделке, руб.
                Percent_Max_Drogdawn_One_Sdelka + ";" + // процент максимального убыток в одной сделке, %

                // БЛОК Г
                Max_Drogdawn + ";" + // Максимальная просадка, руб.

                // БЛОК Д 
                Start_Capital + ";" + // Стартовый капиталл, руб

                // БЛОК Е
                Max_Profit + ";" + // Максимальная прибыль, руб
                
                Local_Profit + ";" + // Чистая прибыль, руб
                Local_Profit_Percent + ";" + // +Чистая прибыль, %.

                Premia_Za_Risk + ";" +  // Премия за риск (соотношение чистой прибыли, на макс просадку, %)

                Doli_Drogdawn + ";" + // текущая глубина просадки в % от максимальной просадки
                Local_Drogdawn + ";" + // текущий уровень просадки, руб
                
                // БЛОК Ж
                Mat_Og + ";" +  // +Математическое ожидание
                Percent_Mat_Og;



                // заполнение массива
                 // БЛОК А
                Reestr[1] = SummSdelok.ToString();  // Общее число сделок, шт
                Reestr[2] = Summ_Prib_Sdelok.ToString();  // Сумма приб. сделок, шт
                Reestr[3] = Percent_Prib_Sdelok.ToString(); // Процент приб. сделок
                Reestr[4] = Summ_Ubitok_Sdelok.ToString();  // Сумма убыт. сделок, шт.
                Reestr[5] = Percent_Ubitok_Sdelok.ToString(); // Процент убыт. сделок
                Reestr[6] = Sootnosh_Pr_Ub_Sdelok.ToString();  // Соотношение между кол. Пр/уб сделок

                // БЛОК Б
                Reestr[7] = Summ_Local_Profits_Sdelok.ToString(); // Сумма результатов только прибыльных сделок, руб
                Reestr[8] = Srenb_Prib_na_Sd.ToString(); // Средняя прибыль на сделку, руб.
                Reestr[9] = Sred_Percent_Prib_na_Sd.ToString();  // Средняя прибыль за сделку, %

                Reestr[10] = Summ_Local_Drogdawn_Sdelok.ToString(); // Сумма результатов только убыточных сделок, руб.
                Reestr[11] = Srenb_Ubit_na_Sd.ToString(); // Средний убыток на сделку, руб
                Reestr[12] = Sred_Percent_Ubit_na_Sd.ToString(); // Средний убыток за сделку, %

                Reestr[13] = Sootnosh_Sred_Pr_Ub_Sdelok.ToString(); // +Соотношение между ср. приб/уб. На сделку
                Reestr[14] = Sootnosh_Summ_Loc_Pro_Ub_Sdelok.ToString(); // Соотношение между суммой рузул. приб/уб. Сделок

                // БЛОК В
                Reestr[15] = Max_Profit_One_Sdelka.ToString(); // максимальная прибыль в одной сделке, руб.
                Reestr[16] = Percent_Max_Profit_One_Sdelka.ToString(); // процент максимальной прибыль в одной сделке, %.

                Reestr[17] = Max_Drogdawn_One_Sdelka.ToString(); // максимальный убыток в одной сделке, руб.
                Reestr[18] = Percent_Max_Drogdawn_One_Sdelka.ToString(); // процент максимального убыток в одной сделке, %

                // БЛОК Г
                Reestr[19] = Max_Drogdawn.ToString(); // Максимальная просадка, руб.

                // БЛОК Д 
                Reestr[20] = Start_Capital.ToString(); // Стартовый капиталл, руб

                // БЛОК Е
                Reestr[21] = Max_Profit.ToString(); // Максимальная прибыль, руб

                Reestr[22] = Local_Profit.ToString(); // Чистая прибыль, руб
                Reestr[23] = Local_Profit_Percent.ToString();  // +Чистая прибыль, %.

                Reestr[24] = Premia_Za_Risk.ToString(); // Премия за риск (соотношение чистой прибыли, на макс просадку, %)

                Reestr[25] = Doli_Drogdawn.ToString();// текущая глубина просадки в % от максимальной просадки
                Reestr[26] = Local_Drogdawn.ToString(); // текущий уровень просадки, руб
                
                // БЛОК Ж
                Reestr[27] = Mat_Og.ToString();  // +Математическое ожидание
                Reestr[28] = Percent_Mat_Og.ToString();
                        
            return Str_x;
        }

        public decimal Metod_Raschet_Korreliacii(decimal Prosadka, decimal Volatiliti)
        {

            Razmer_V2 = Razmer_V + Sdvig;
            
            if (i < Razmer_V2) // заполнение массива и наполнение будущей средней индивидуальностями
            {
                
                if (u < (Razmer_V)) 
                {

                    if (i <= u & u <= Razmer_V2 - Sdvig) // условия соблюдения сдвига
                    {
                    Baza_Prosadok[(u)] = Prosadka; // заполняем массив
                    Sred_Pros = Sred_Pros + Prosadka; // суммируем к средней велечине
                    }

                    if (u >= Sdvig)
                    {
                        Baza_Volatiliti[(u)] = Volatiliti; // заполняем массив
                        Sred_Vol = Sred_Vol + Baza_Volatiliti[(u)]; // суммируем к средней велечине
                    }

                    u++;
                }

                i++;
            }

            
            
            if (i == Razmer_V2) // расчет средних, делителей и знаменателей
            {
                u = 0;

                Sred_Pros = Sred_Pros / (Razmer_V - Sdvig); // расчет средней просадки
                Sred_Vol = Sred_Vol / (Razmer_V - Sdvig); // расчет средней волатильности

                while (u != (Razmer_V - 1))
                {
                    Delta_Pros = (Baza_Prosadok[(u)] - Sred_Pros); // считаем дельту между средней и индивидуальным
                    Delta_Pros_Coren = Delta_Pros_Coren + ((Baza_Prosadok[(u)] - Sred_Pros) * (Baza_Prosadok[(u)] - Sred_Pros));

                    Delta_Vol = (Baza_Volatiliti[(u)] - Sred_Vol); // считаем дельту между средней и индивидуальным
                    Delta_Vol_Coren = Delta_Vol_Coren + ((Baza_Volatiliti[(u)] - Sred_Vol) * (Baza_Volatiliti[(u)] - Sred_Vol));

                    Znamenatel = Znamenatel + (Delta_Pros * Delta_Vol); // накапливаем произведение в знаменателе

                    u++;
                }
                         

                i++;
            }

            
            
            if (i == (Razmer_V2 + 1)) // расчет корреляция и дисперсии
            {

                Standart_Otlonenie_Pros = (decimal)(Math.Sqrt((double)(Delta_Pros_Coren / (Razmer_V - Sdvig))));
                Standart_Otlonenie_Vol = (decimal)(Math.Sqrt((double)(Delta_Vol_Coren / (Razmer_V - Sdvig))));
                Koreliacia = Znamenatel / (decimal)(Math.Sqrt((double)(Delta_Pros_Coren * Delta_Vol_Coren)));
                Reesr_Koreliacia[x] = Koreliacia; // ведение реестра корреляций
                x++;

                // сбрасываем все
                
                i = 0; // индексатор разделов
                u = 0; // индексатор массива

                Sred_Pros = 0; // средняя просадок (обнуляем)
                Sred_Vol = 0;  // средняя волатильности (обнуляем)

                Delta_Pros = 0;
                Delta_Pros_Coren = 0; // делители и знаменатели

                Delta_Vol = 0;
                Delta_Vol_Coren = 0; // делители и знаменатели

                Znamenatel = 0;              

            }

            // Console.Clear();
            // Console.WriteLine(Koreliacia);

           return Koreliacia;
            
        }

    }
}
