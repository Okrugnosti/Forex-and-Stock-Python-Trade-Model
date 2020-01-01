using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R10
{
    // СИСТЕМА УПРАВЛЕНИЯ КАПИТАЛОМ 

    public class Class_Capital_System // нужно создавать
    {

        public int Stavka_R01(Parametrs Parametr)
        {
            int Plan_Positions_R01 = 1;
            return Plan_Positions_R01;
            
        }

        public int Stavka_xR01(Parametrs Parametr)
        {
            int Plan_Positions_xR01 = 1;

            return Plan_Positions_xR01;

        }
        
    }

    public class Class_Capital_System_Reversor // нужно создавать
    {

        int rele_1 = 0; 
        int n = 0; // индекс массива
        int futur_t = 0; // будущий i
            
        int sistem_vector = 0; // вектор, указывает какая система торгует
                               // sistem_vector = 1 - тредовая 
                               // sistem_vector = 2 - боковая
                               // sistem_vector = 0 - cash
        
        public int Capital_System_Volatiliti_Reversor(decimal[,] Baza_Molecula_V, int t) // модель управления капиталом на базе анализа волатильности с реверсивным методом
        {
                        
            /// <вводная>
            /// t - это наш виртуальный показатель времени
            /// Массив волатильности формата 7 столбцов
            /// Baza_Molecula_V[х, 0] - столбец с дата
            /// Baza_Molecula_V[х, 1] - столбец с расстоянием
            /// Baza_Molecula_V[х, 2] - столбец с точкой Max цикла
            /// Baza_Molecula_V[х, 3] - столбец с точкой Min цикла
            /// Baza_Molecula_V[х, 4] - столбец с значением Max цикла
            /// Baza_Molecula_V[х, 5] - столбец с значением Min цикла
            /// Baza_Molecula_V[х, 7] - столбец с значением i 
            /// <конец/>

            // расчет этого блока ведется только в том случает пока в массив не определено 
            // первое будущее время
            if (rele_1 == 0)
            {
                futur_t = (int)Baza_Molecula_V[n, 7]; // переконвертируем заодно в int

                if (futur_t > 0) { rele_1++; } // пока не присвоется будущее время

                sistem_vector = 0; // пока находимся в кеше
            }


            // расчет этого блока ведется только в том случае если заполнение массива началось
            if (rele_1 == 1)
            {
                futur_t = (int)Baza_Molecula_V[n, 7];
                // реальное время не достигло будущего времени
                if (t < futur_t)
                {
                    // вариант 1
                    // в массиве заполнено значение MAX
                    if (Baza_Molecula_V[n, 4] > 0)
                    {
                        sistem_vector = 2;
                    }


                    // вариант 2
                    // в массиве заполнено значение MIN
                    if (Baza_Molecula_V[n, 5] > 0)
                    {
                        sistem_vector = 1;
                    }
                }
                // если время достигнуто то переходим к новой строке массива
                // и новому отрезку будущего
                else
                {
                    
                        n++;
                        futur_t = (int)Baza_Molecula_V[n, 7];
                    

                }
            }
                       
            
        
        
            //Console.WriteLine("sistem_vector: " + sistem_vector);     
            return sistem_vector;
           
        }


    }
}
