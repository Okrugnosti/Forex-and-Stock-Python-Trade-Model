using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R10
{
    // МОЛЕКУЛА ПРОСАДОК

    public class Class_Raschet_Sredney_Local_Drogdawn // класс расчета волатильности
    {
        
        private int U2; // размерность выборки
        private int u; // переменная цикла
        public decimal Loc; // среднее в формуле среднеквадратического отклонения

        public const int SR_P = 840; // задаем глубину выборки для расчета просадок в массиве Baza_Sredney_Local_Drogdawn

        public decimal[] Baza_Sredney_Local_Drogdawn = new decimal[SR_P]; // создаем массив для хранения данных цен закрытия

        public decimal Raschet_Sredney_Local_Drogdawn(decimal Local_Drogdawn, Parametrs Parametr) // метод расчета средней локальных просадок
        {
            U2 = SR_P; // размерность выборки
            u = U2 - 1; //счетчик 
            Loc = 0; // переменная для хранения средней цены


            while (u > 0) // сдвиг данных массива на 1 ячейку с удалением последней записи
            {
                Baza_Sredney_Local_Drogdawn[u] = Baza_Sredney_Local_Drogdawn[(u - 1)];
                Loc = Loc + Baza_Sredney_Local_Drogdawn[u]; // рассчитываем базу средней
                
                u--;
            }

            Baza_Sredney_Local_Drogdawn[0] = -Local_Drogdawn; // новую цену закрытия загоняем в массив

            u = U2;

            Loc = (Loc + Baza_Sredney_Local_Drogdawn[0]) / u; // рассчитываем среднюю для дисперсии

            return Loc;
        }

    }

    public class Class_Raschet_Baza_Prosadok // ведение базы данных просадок
    {
        
        private int u; // переменная цикла
        private int u_i; // переменная суммы вызовометода
        private int U2; // размерность выборки 
        public const int V_BP = 10000; // задаем размер массива для хранения Baza_Prosadok

        public decimal[,] Baza_Prosadok = new decimal[V_BP, 3]; // создаем массив для хранения значений волатильности

        public int Raschet_Baza_Prosadok(decimal[] Chislo_out, decimal Loc, int i, Parametrs Parametr) // метод по заполнению базы волатильности
        {
            // Baza_Prosadok (3200 строк) (2 столбца)
            // 1-й столбец содержит дату дня последнего добавления цены закрытия
            // 2-й столбец содержит расчетную волатильность в этот день
            // 3-й столбец содержит наш расчетный индекс i

            u = 1;
            U2 = V_BP;

            while (u < (U2)) // сдвиг данных массива на 1 ячейку с удалением последней записи
            {
                Baza_Prosadok[(u - 1), 0] = Baza_Prosadok[(u), 0];
                Baza_Prosadok[(u - 1), 1] = Baza_Prosadok[(u), 1];
                Baza_Prosadok[(u - 1), 2] = Baza_Prosadok[(u), 2];
                u++;
            }

            Baza_Prosadok[(V_BP - 1), 0] = Chislo_out[1]; // новую дату расчета волатильности загоняем в массив
            Baza_Prosadok[(V_BP - 1), 1] = Loc; // новую расчетную волатильность загоняем в массив
            Baza_Prosadok[(V_BP - 1), 2] = i; // индекс i загоняем в массив

            u_i++;

            return u_i;

        }

    }

    public class Class_Raschet_Molecula_Prosadok // расчет молекулы волатильности
    {

        private int U2; // размерность выборки 
        private int u; // переменная цикла
        private int u_i; // переменная цикла
        private int u_y; // переменная учета пика

        public const int V_BP = 1000; // задаем размер массива для хранения Baza_Prosadok

        public decimal[,] Baza_Molecula_P = new decimal[V_BP, 8]; // создаем массив для хранения данных молекулы волатильности
        // (3200 строк) (6 столбцов) 

        private decimal Filter; // фильтра просадок
        private decimal Filter_Max; // фильтруем волатильность на максимум
        private decimal Filter_Min; // фильтруем волатильность на минимум

        public int Raschet_Molekula_Prosadok(decimal[,] Baza_Prosadok, int Index_Molecula_Prosadok, int i, Parametrs Parametr) // метод по расчету молекулы волатильности
        {
            i = i + Parametr.sdvig_prosadok;
            
            // Массив формата 6 столбцов
            // Baza_Molecula_V[х, 0] - столбец с дата
            // Baza_Molecula_V[х, 1] - столбец с расстоянием
            // Baza_Molecula_V[х, 2] - столбец с точкой Max цикла
            // Baza_Molecula_V[х, 3] - столбец с точкой Min цикла
            // Baza_Molecula_V[х, 4] - столбец с значением Max цикла
            // Baza_Molecula_V[х, 5] - столбец с значением Min цикла
            // Baza_Molecula_V[х, 7] - столбец с i

            U2 = V_BP;
            Filter = Parametr.Filter_Prosadok;
            u_i = 0;
            u = Index_Molecula_Prosadok;
            u_y = 0;
            Filter_Max = 0;
            Filter_Min = 0;


            if (Baza_Molecula_P[0, 0] != 0)
            {


                // **************************** считаем для НЕ пустого массива ********** 


                if ((Baza_Molecula_P[(u), 4] > 0) & (Baza_Molecula_P[(u), 2] == 1))
                {

                    // start ********считаем ПРОДОЛЖЕНИЕ MAX ******* 

                    u_i = Convert.ToInt32(Baza_Molecula_P[u, 3]); // преобразуем данные 
                    u_i--; // смещаем данные на один цикл назад, т.к. база волатильности перезаписалась

                    Filter_Max = Baza_Molecula_P[(u), 4]; // устанавливаем фильтр на текущий размер волатильности в цикле

                    // фильтруем значения по 100 пунктов

                    while ((Baza_Prosadok[(u_i), 1] > (Filter_Max - Filter)) & (u_i < (U2 - 1))) // фильтруем на максимум
                    {
                        if (Filter_Max < Baza_Prosadok[(u_i), 1])
                        {
                            Filter_Max = Baza_Prosadok[(u_i), 1];
                            u_y = u_i; // присваиваем координаты локального максимума

                        }

                        u_i++;
                    }

                    if (Baza_Prosadok[(u_i), 1] <= (Filter_Max - Filter)) // присваиваем индекс завершенности
                    {
                        Baza_Molecula_P[u, 2] = 0;
                        u_y = u_i;
                        Baza_Molecula_P[u, 6] = u; // присваиваем номер экстремума
                    }
                    else
                    {
                        Baza_Molecula_P[u, 2] = 1;
                        u_i--;
                        u_y = u_i;
                    }

                    Baza_Molecula_P[(u - 1), 3] = Baza_Molecula_P[(u - 1), 3] - 1;

                    // записываем результат в молекулу волатильности

                    Baza_Molecula_P[u, 0] = Baza_Prosadok[(u_y), 0]; // присваиваем дату
                    Baza_Molecula_P[u, 1] = u_i - Baza_Molecula_P[(u - 1), 3]; // устанавливаем расстояние от предыдущей точки
                    Baza_Molecula_P[u, 3] = u_i; // устанавливаем координаты последней остановки в массиве
                    Baza_Molecula_P[u, 4] = Filter_Max; // записываем значение максимума
                    Baza_Molecula_P[u, 5] = 0;
                    Baza_Molecula_P[u, 7] = i;

                    // end ********считаем ПРОДОЛЖЕНИЕ MAX *******

                }





                if ((Baza_Molecula_P[(u), 5] > 0) & (Baza_Molecula_P[(u), 2] == 1))
                {

                    // start ********считаем ПРОДОЛЖАЕМ MIN ******* 

                    u_i = Convert.ToInt32(Baza_Molecula_P[u, 3]); // преобразуем данные 
                    u_i--; // смещаем данные на один цикл назад, т.к. база волатильности перезаписалась

                    Filter_Min = Baza_Molecula_P[(u), 5]; // устанавливаем фильтр на текущий размер волатильности в цикле

                    // фильтруем значения по 100 пунктов

                    while ((Baza_Prosadok[(u_i), 1] < (Filter_Min + Filter)) & (u_i < (U2 - 1))) // фильтруем на максимум
                    {
                        if (Filter_Min > Baza_Prosadok[(u_i), 1])
                        {
                            Filter_Min = Baza_Prosadok[(u_i), 1];
                            u_y = u_i; // присваиваем координаты локального максимума

                        }

                        u_i++;
                    }

                    if (Baza_Prosadok[(u_i), 1] >= (Filter_Min + Filter)) // присваиваем индекс завершенности
                    {
                        Baza_Molecula_P[u, 2] = 0;
                        u_y = u_i;
                        Baza_Molecula_P[u, 6] = u; // присваиваем номер экстремума
                    }
                    else
                    {
                        Baza_Molecula_P[u, 2] = 1;
                        u_i--;
                        u_y = u_i;
                    }

                    Baza_Molecula_P[(u - 1), 3] = Baza_Molecula_P[(u - 1), 3] - 1;

                    // записываем результат в молекулу волатильности

                    Baza_Molecula_P[u, 0] = Baza_Prosadok[(u_y), 0]; // присваиваем дату
                    Baza_Molecula_P[u, 1] = u_i - Baza_Molecula_P[(u - 1), 3]; // устанавливаем расстояние от предыдущей точки
                    Baza_Molecula_P[u, 3] = u_i; // устанавливаем координаты последней остановки в массиве
                    Baza_Molecula_P[u, 4] = 0; // записываем значение максимума
                    Baza_Molecula_P[u, 5] = Filter_Min;
                    Baza_Molecula_P[u, 7] = i;


                    // end ********считаем ПРОДОЛЖАЕМ MIN *******

                }





                if ((Baza_Molecula_P[(u), 5] > 0) & (Baza_Molecula_P[(u), 2] != 1))
                {

                    // start ********считаем НОВЫЙ MAX ******* 

                    u_i = Convert.ToInt32(Baza_Molecula_P[u, 3]); // получаем указатель на последнюю точку остановки
                    u_i--; // смещаем значение на одну еденицу из-за перезаписи массива волатильности после нового цикла расчетов

                    Filter_Max = Baza_Prosadok[(u_i), 1]; // устанавливаем фильтр на текущий размер волатильности в цикле


                    while ((Baza_Prosadok[(u_i), 1] >= (Filter_Max - Filter)) & (u_i < (U2 - 1))) // фильтруем на минимум
                    {
                        if (Filter_Max < Baza_Prosadok[(u_i), 1])
                        {
                            Filter_Max = Baza_Prosadok[(u_i), 1];
                            u_y = u_i; // присваиваем координаты локального максимума

                        }

                        u_i++;
                    }

                    // записываем результат в молекулу волатильности


                    if (Baza_Prosadok[(u_i), 1] <= (Filter_Max - Filter)) // присваиваем индекс завершенности
                    {
                        Baza_Molecula_P[(u + 1), 2] = 0;
                        Baza_Molecula_P[(u + 1), 6] = (u + 1); // присваиваем номер экстремума
                    }
                    else
                    {
                        Baza_Molecula_P[(u + 1), 2] = 1;
                        u_i--;
                        u_y = u_i;
                    }

                    Baza_Molecula_P[(u), 3] = Baza_Molecula_P[(u), 3] - 1;

                    Baza_Molecula_P[(u + 1), 0] = Baza_Prosadok[(u_y), 0]; // присваиваем дату
                    Baza_Molecula_P[(u + 1), 1] = u_y - Baza_Molecula_P[u, 3]; // устанавливаем расстояние от предыдущей точки
                    Baza_Molecula_P[(u + 1), 3] = u_y; // устанавливаем координаты последней остановки в массиве
                    Baza_Molecula_P[(u + 1), 4] = Filter_Max;
                    Baza_Molecula_P[(u + 1), 5] = 0; // записываем значение минимума
                    Baza_Molecula_P[(u + 1), 7] = i;

                    u = u + 1; //возвращаем индекс генома


                    // edn ********считаем НОВЫЙ MAX *******

                }





                if ((Baza_Molecula_P[(u), 4] > 0) & (Baza_Molecula_P[(u), 2] != 1))
                {

                    // start ********считаем НОВЫЙ MIN ******* 

                    u_i = Convert.ToInt32(Baza_Molecula_P[u, 3]); // получаем указатель на последнюю точку остановки
                    u_i--; // смещаем значение на одну еденицу из-за перезаписи массива волатильности после нового цикла расчетов

                    Filter_Min = Baza_Prosadok[(u_i), 1]; // устанавливаем фильтр на текущий размер волатильности в цикле


                    while ((Baza_Prosadok[(u_i), 1] <= (Filter_Min + Filter)) & (u_i < (U2 - 1))) // фильтруем на минимум
                    {
                        if (Filter_Min > Baza_Prosadok[(u_i), 1])
                        {
                            Filter_Min = Baza_Prosadok[(u_i), 1];
                            u_y = u_i; // присваиваем координаты локального максимума

                        }

                        u_i++;
                    }

                    // записываем результат в молекулу волатильности


                    if (Baza_Prosadok[(u_i), 1] >= (Filter_Min + Filter)) // присваиваем индекс завершенности
                    {
                        Baza_Molecula_P[(u + 1), 2] = 0;
                        Baza_Molecula_P[(u + 1), 6] = (u + 1); // присваиваем номер экстремума
                    }
                    else
                    {
                        Baza_Molecula_P[(u + 1), 2] = 1;
                        u_i--;
                        u_y = u_i;
                    }

                    Baza_Molecula_P[(u), 3] = Baza_Molecula_P[(u), 3] - 1;

                    Baza_Molecula_P[(u + 1), 0] = Baza_Prosadok[(u_y), 0]; // присваиваем дату
                    Baza_Molecula_P[(u + 1), 1] = u_y - Baza_Molecula_P[u, 3]; // устанавливаем расстояние от предыдущей точки
                    Baza_Molecula_P[(u + 1), 3] = u_y; // устанавливаем координаты последней остановки в массиве
                    Baza_Molecula_P[(u + 1), 4] = 0;
                    Baza_Molecula_P[(u + 1), 5] = Filter_Min; // записываем значение минимума
                    Baza_Molecula_P[(u + 1), 7] = i;

                    u = u + 1; //возвращаем индекс генома


                    // end ********считаем НОВЫЙ MIN *******

                }








            }
            else  // считаем для ПУСТОГО массива в обе стороны
            {

                // **************************** считаем для ПУСТОГО массива **********



                u = 0;

                if (Baza_Prosadok[0, 1] > Baza_Prosadok[50, 1]) // проверяем рост или падение волатильности
                {
                    // если волатильность падает

                    // обращаемся к базе волатильности к дате следующей за детектированной датой

                    u_i = 0;


                    // фильтруем значения по 100 пунктов
                    Filter_Min = Baza_Prosadok[(u_i), 1]; // устанавливаем фильтр на текущий размер волатильности в цикле

                    while ((Baza_Prosadok[(u_i), 1] < (Filter_Min + Filter)) & (u_i < (U2 - 1))) // фильтруем на минимум
                    {
                        if (Filter_Min > Baza_Prosadok[(u_i), 1])
                        {
                            Filter_Min = Baza_Prosadok[(u_i), 1];
                            u_y = u_i; // присваиваем координаты локального максимума


                            if (Baza_Prosadok[(u_i), 1] < (Filter_Min + Filter)) // присваиваем индекс завершенности
                            {
                                Baza_Molecula_P[u, 2] = 0;
                                Baza_Molecula_P[(u), 6] = (u); // присваиваем номер экстремума
                            }
                            else Baza_Molecula_P[u, 2] = 1;

                        }

                        u_i++;
                    }

                    // записываем результат в молекулу волатильности

                    Baza_Molecula_P[u, 0] = Baza_Prosadok[(u_y), 0]; // присваиваем дату
                    Baza_Molecula_P[u, 1] = u_y; // устанавливаем расстояние от предыдущей точки
                    Baza_Molecula_P[u, 3] = u_y; // устанавливаем координаты последней остановки в массиве
                    Baza_Molecula_P[u, 4] = 0;
                    Baza_Molecula_P[u, 5] = Filter_Min; // записываем значение минимума
                    Baza_Molecula_P[u, 7] = i;


                }
                else // пустая молекула, если волатильность растет
                {


                    // обращаемся к базе волатильности к дате следующей за детектированной датой

                    u_i = 0;


                    // фильтруем значения по 100 пунктов

                    Filter_Max = Baza_Prosadok[(u_i), 1]; // устанавливаем фильтр на текущий размер волатильности в цикле

                    while ((Baza_Prosadok[(u_i), 1] > (Filter_Max - Filter)) & (u_i < (U2 - 1))) // фильтруем на максимум
                    {
                        if (Filter_Max < Baza_Prosadok[(u_i), 1])
                        {
                            Filter_Max = Baza_Prosadok[(u_i), 1]; // присваиваем значение локального максимума
                            u_y = u_i; // присваиваем координаты локального максимума

                            if (Baza_Prosadok[(u_i), 1] > (Filter_Max - Filter)) // присваиваем индекс завершенности
                            {
                                Baza_Molecula_P[u, 2] = 0;
                                Baza_Molecula_P[(u), 6] = (u); // присваиваем номер экстремума
                            }
                            else Baza_Molecula_P[u, 2] = 1;
                        }

                        u_i++;
                    }

                    // записываем результат в молекулу волатильности

                    Baza_Molecula_P[u, 0] = Baza_Prosadok[(u_y), 0]; // присваиваем дату
                    Baza_Molecula_P[u, 1] = u_y; // устанавливаем расстояние от предыдущей точки
                    Baza_Molecula_P[u, 3] = u_y; // устанавливаем координаты последней остановки в массиве
                    Baza_Molecula_P[u, 4] = Filter_Max; // записываем значение максимума
                    Baza_Molecula_P[u, 5] = 0;
                    Baza_Molecula_P[u, 7] = i;

                }

            }


            return u;
        }
        
    }
}
