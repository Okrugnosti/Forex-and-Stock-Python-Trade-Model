'''
1. из файла 7_Analysis_of_simulation_results_01.pickle загружается DataSet для анализа
2. если прогноз положительный, то результат сделки умножается на вероятность
3. результат записывается в столбец "результат сделки"
4. в столбец "накопленный_результат" записывается накопленный результат всех сделок (
итерационное прибавление результатов сделок друг к дургу)
5. в столбце "локальный_максимум_накопленного_результата_сделок"...считается локальный макс.
если x_1>x_0 then x_1 else x_0
6. в столбце "разница_между_локальным максимум и накопленным_результатом" считается разница...
7. по столбцу в п.6 определяется самая большая просадки
8. самую большую просадку делим на 40 и умножаем на 100. определяем размер обеспечения на одну сделку
9. в новом столбце: делим значения в столбце "накопленный_результат" на обеспечения на одну сделку -> накопленная доходность
10. записываем данные в файл "8_Portfolio_return_modeling_01.pickle"
11. формируем таблицу со значениями на конец каждого месяца.
12. данные таблицы вывод в файл "8.1_Svod_Portfolio_return_modeling_01.pickle"
'''

import os  # Системные функции
import pickle

from datetime import datetime  # Класс по работе с датой
import pytz  # База часовых поясов
import openpyxl as xl  # Excel
import numpy as np  # Матрицы
import pandas as pd  # DataFrame
import matplotlib.pyplot as plt  # Построение графиков
import seaborn as sns  # Построение графиков
import plotly  # Построение графиков https://proglib.io/p/plotly/
import scipy  # Выполнениt научных и инженерных расчётов
import mglearn
from tqdm import tqdm

# временная метка для отсечки скорости алгоритма
t1 = datetime.now(tz=None)

#1. Загружаем данные из файла
with open('../../data/interim/7_Analysis_of_simulation_results_01.pickle', 'rb') as f:
    Money_Man_Model = pickle.load(f)

print(Money_Man_Model.columns)

'''
o = o.groupby(pd.Grouper(freq=period_agrigacii)).agg(
        {
            '<OPEN>': lambda x: x[0],
            '<CLOSE>': lambda x: x[-1:],
            '<Sum_Sdelok>' : lambda x: x.sum(),
            '<Rezultat_Sdelki_Cub>' : lambda x: x.sum(),
            '<Rezultat_Sdelki>' : lambda x: x.sum(),
            '<Rezultat_Sdelki_Rub>' : lambda x: x.sum(),
            '<Rezultat_Sdelki_Rub_Summ>' : lambda x: x[-1:],
            '<Local_Drogdawn>' : lambda x: x[-1:],
            '<Start_Capital>' : lambda x: x[-1:]
        })
'''

#если ML рекомендует совершить ставку на сигнал - передаем сигнал
Money_Man_Model['MM_Trade_Signal'] = Money_Man_Model['Log_Regression_Predict'].apply(lambda x: 1 if x == 'Plus' else 0)

#определяем размер прибыли/убытка на сделку
Money_Man_Model['Rezultat_Sdelki'] = Money_Man_Model['<Rezultat_Sdelki_Retrospektiva>'] \
                                   * Money_Man_Model['Veroatnost_Priznaka'] \
                                   * Money_Man_Model['MM_Trade_Signal']


Money_Man_Model['Rezultat_Sdelki_Rub'] = Money_Man_Model['Rezultat_Sdelki'] * Money_Man_Model['PX_OPEN'] / 100

def raschet_Max_Drogdawn(x, Max_Risc):
    Local_Profit = Max_Drogdawn = Local_Drogdawn = Max_Drogdawn = Max_Profit = Start_Capital = 0
    x['Local_Drogdawn'] = 0
    x['Start_Capital'] = 0
    x['Doli_Drogdawn'] = 0
    x['Rezultat_Sdelki_Rub_Sum'] = 0
    x['Profitability_Summ'] = 0
    x['Size_Stavka_Rub'] = 0

    for i in tqdm(x.index):

        Local_Profit = Local_Profit + x['Rezultat_Sdelki_Rub'].loc[i]
        x['Rezultat_Sdelki_Rub_Sum'].loc[i] = Local_Profit
        #x.set_value('Rezultat_Sdelki_Rub_Sum', 'i' , Local_Profit)

        if (Max_Profit < Local_Profit):
            Max_Profit = Local_Profit  # Максимальная прибыль, руб

        Local_Drogdawn = Local_Profit - Max_Profit  # рассчет локальной просадки

        if (Max_Drogdawn > Local_Drogdawn):
            Max_Drogdawn = Local_Drogdawn  # Максимальная просадка, руб.

        x['Local_Drogdawn'].loc[i] = Local_Drogdawn
        #x.set_value('Local_Drogdawn', 'i', Local_Drogdawn)


    Start_Capital = Max_Drogdawn / Max_Risc * -100  # Стартовый капиталл, руб
    x['Start_Capital'] = Start_Capital
    x['Profitability_Summ'] = x['Rezultat_Sdelki_Rub_Sum'] / Max_Drogdawn * - 100
    x['Doli_Drogdawn'] = x['Local_Drogdawn'] / Max_Drogdawn * 100
    x['Size_Stavka_Rub'] = x['Veroatnost_Priznaka'] * x['Start_Capital']


    print(x)
    return x

Money_Man_Model = raschet_Max_Drogdawn(Money_Man_Model, 50)

'''
Money_Man_Model['Rezultat_Sdelki_Rub_Sum'] = Money_Man_Model['Rezultat_Sdelki_Rub'].cumsum()
Money_Man_Model['Rezultat_Sdelki_Rub_Sum2'] = Money_Man_Model['Rezultat_Sdelki_Rub'].apply(lambda x: x + x[-1], axis=1)

#Money_Man_Model['Max_Local_Rezultat_Sdelki_Rub_Sum'] = 0
#Money_Man_Model['Max_Local_Rezultat_Sdelki_Rub_Sum'] = \
#    Money_Man_Model[['Rezultat_Sdelki_Rub_Sum', 'Max_Local_Rezultat_Sdelki_Rub_Sum']].apply(lambda x, y: x if x > y[-1] else y[-1])

print(Money_Man_Model[['Rezultat_Sdelki_Rub','Rezultat_Sdelki_Rub_Sum2']])
'''

print(Money_Man_Model)

#8. Экспорт данных в файл
with open('../../data/interim/8.1_Svod_Portfolio_return_modeling_01.pickle', 'wb') as f:
    pickle.dump(Money_Man_Model, f)

Money_Man_Model.to_csv('../../data/interim/8.1_Svod_Portfolio_return_modeling_01.csv', encoding='utf-8', sep=',')  # index=None,
#Money_Man_Model.to_excel('../../data/interim/8.1_Svod_Portfolio_return_modeling_01.xlsx', startrow=3, index=True)

###########################
print("Write File - Ok")
t2 = datetime.now(tz=None)
print("Время работы алгоритма:", t2 - t1)