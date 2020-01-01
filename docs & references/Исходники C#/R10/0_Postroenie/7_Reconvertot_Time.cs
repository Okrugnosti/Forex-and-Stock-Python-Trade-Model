using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R10
{


    // Класс по конвертации формата времени выраженного в секундах
    // в формат ЧЧММСС

    class Reconvertot_Time
    {

        decimal hour;
        decimal minets;
        decimal second;

        decimal time;

        // x задается в секундах.  
        public decimal Metod_Reconvertor_Time(int x)
        {
            hour = x / 3600;
            Math.Floor(hour);

            minets = (x - (hour * 3600))/60;
            Math.Floor(minets);

            second = x - (hour * 3600 + minets * 60);

            time = hour * 10000 + minets * 100 + second;

            return time;
        }

        public decimal Metod_Reconvertor_Data(string x)
        {
            decimal data;
            string[] DATA = new string[4]; // массив
            DATA = x.Split(new char[] { '.' }); // установка

            data = (decimal.Parse(DATA[2]) * 10000) + (decimal.Parse(DATA[1]) * 100) + decimal.Parse(DATA[0]);

            return data;
        }

    }
}
