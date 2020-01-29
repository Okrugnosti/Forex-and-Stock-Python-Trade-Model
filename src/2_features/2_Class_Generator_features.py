import os  # Системные функции
from datetime import datetime  # Класс по работе с датой
import pytz  # База часовых поясов
import openpyxl as xl  # Excel
import numpy as np  # Матрицы
import pandas as pd  # DataFrame
import matplotlib.pyplot as plt  # Построение графиков
import seaborn as sns  # Построение графиков
import plotly # Построение графиков https://proglib.io/p/plotly/
import scipy  # Выполнениt научных и инженерных расчётов
import mglearn
from tqdm import tqdm
import talib
import pickle
import time
import sklearn as sk  # Машинное обучение


'''
ИСТОЧНИКИ:
CDS Россия.xls, CDS США 5Y.xls, 1M LIBOR USD.xls,
3M LIBOR USD.xls, 12M LIBOR USD, MOSPRIME 1M.xls,
MOSPRIME 3M.xls, MOSPRIME 6M.xls, Nikkei 225.xls,
S_P 500.xls, FTSE 100.xls, DAX.xls

VIX.xlsx, USGG2YR Index.xlsx, USGG10YR Index.xlsx,
Brent.xlsx, Gold spot.xlsx

SPFB.RTS, USDRUB REGN Curncy.csv
'''

#БЛОК 1:
# Загрузка данных
with open('../../data/interim/3_Train_Massiv_2.pickle', 'rb') as f:
    DataFrame_Raw_01 = pickle.load(f)

DataFrame_Raw_01 = DataFrame_Raw_01[['<OPEN>', '<HIGH>', '<LOW>', '<CLOSE>', '<VOL>', 'PX_OPEN',
                                     '<Rezultat_Sdelki_Cub>', '<Rezultat_Sdelki_Rub>',
                                     '<Rezultat_Sdelki_Retrospektiva>', 'Rezultat_Sdelki_Retro_Binar_Plas_Minus']]

# Корректировка названий столбцов к формату "Тикер_Таймфрейм_Индикатор"
DataFrame_Raw_01.columns = ['Ri_5M_Price_Open', 'Ri_5M_Price_High', 'Ri_5M_Price_Low', 'Ri_5M_Price_Close', 'Ri_5M_Price_Volume',
                            'USDRUB_1D_Price_Open',
                            'Renko_5M_850p_Rezultat_Sdelki_Cub', 'Renko_5M_850p_Rezultat_Sdelki_Rub',
                            'Renko_5M_850p_Rezultat_Sdelki_Retrospektiva', 'Renko_5M_850p_Rezultat_Sdelki_Retrospektiva_Plus_Minus']

# Понижение размера файла - приведение типов
DataFrame_Raw_01 = DataFrame_Raw_01.iloc[:, 0:-1].astype('int')
print(DataFrame_Raw_01.info())


# Выделение признаков времени
DataFrame_Raw_01['data'] = DataFrame_Raw_01.index
#DataFrame_Raw_02 = DataFrame_Raw_01['data'].apply(lambda x: pd.DataFrame({
#                        'a': str(x).split(' ')}))

DataFrame_Raw_01['day'] = DataFrame_Raw_01['data'].str.split(' ').str.get(0)
DataFrame_Raw_01['year'] = DataFrame_Raw_01['day'].str.split('-').str.get(0)
DataFrame_Raw_01['month'] = DataFrame_Raw_01['day'].str.split('-').str.get(1)
DataFrame_Raw_01['day'] = DataFrame_Raw_01['day'].str.split('-').str.get(2)

DataFrame_Raw_01['hour'] = DataFrame_Raw_01['data'].str.split(' ').str.get(1)
DataFrame_Raw_01['minute'] = DataFrame_Raw_01['hour'].str.split(':').str.get(1)
DataFrame_Raw_01['hour'] = DataFrame_Raw_01['hour'].str.split(':').str.get(0)

print(DataFrame_Raw_01[['year', 'month', 'day', 'hour', 'minute']])

Dammi_Minutes_Fitch = pd.get_dummies(DataFrame_Raw_01['minute'])
Dammi_Hours_Fitch = pd.get_dummies(DataFrame_Raw_01['hour'])
Dammi_Day_Fitch = pd.get_dummies(DataFrame_Raw_01['day'])
Dammi_Month_Fitch = pd.get_dummies(DataFrame_Raw_01['month'])
Dammi_Qvartal_Fitch = pd.get_dummies(DataFrame_Raw_01['year'])

dammi_time = [DataFrame_Raw_01,
              Dammi_Minutes_Fitch, Dammi_Hours_Fitch,
              Dammi_Day_Fitch, Dammi_Month_Fitch, Dammi_Qvartal_Fitch,
            ]

DataFrame_Raw_02 = pd.concat(dammi_time, axis=1, sort=False)

print(DataFrame_Raw_02.columns)
#выделить признак назнвания дня datetime.isoweekday()
#выделить признак квартала

'''
#вывод значений
for i in range(100, 168):
    name = 'Ri_5M_Price_Close_T' + str(i)
    DataFrame_Raw_01[name] = 0
    DataFrame_Raw_01[name] = DataFrame_Raw_01['Ri_5M_Price_Close'].shift(i)
    DataFrame_Raw_01[name] = DataFrame_Raw_01[name].fillna(0)
    DataFrame_Raw_01[name] = DataFrame_Raw_01[name].astype('int')

print(DataFrame_Raw_01.info())

DataFrame_Raw_01.iloc[500010, 10:168].plot()
DataFrame_Raw_01.iloc[500011, 10:168].plot()
DataFrame_Raw_01.iloc[500012, 10:168].plot()

plt.show()
'''

DataFrame_Raw_01.index = pd.to_datetime(DataFrame_Raw_01.index)

'''
print(DataFrame_Raw_01.pivot(index = DataFrame_Raw_01.resample('1D'),
                       columns = DataFrame_Raw_01.resample('5T'),
                       values = DataFrame_Raw_01['Ri_5M_Price_Open']))
'''

'''
DataFrame_Raw_01_Pivot_Price = pd.pivot_table(DataFrame_Raw_01, index=DataFrame_Raw_01.index.date,
                     #columns=DataFrame_Raw_01.index.hour,
                     columns=DataFrame_Raw_01.index.minute,
                     values='Ri_5M_Price_Open')

#f = lambda x: (x - x.std()) / x.mean()
f = lambda x: (x - x.iloc[0])
DataFrame_Raw_01_Pivot_Price = DataFrame_Raw_01_Pivot_Price.apply(f, axis=1)

for i in range(3000, 3500):
    DataFrame_Raw_01_Pivot_Price.iloc[i].plot()
plt.show()

'''

'''
pd.plotting.scatter_matrix(DataFrame_Raw_01_Pivot_Price)
plt.show()
'''

'''

corr = DataFrame_Raw_01_Pivot_Price.corr()
corr.to_excel('../../data/reports/Corr_Hour_Hour_01.xlsx', startrow=3, index=True)

#print(DataFrame_Raw_01['Ri_5M_Price_Close'].resample('5T').sum())
'''


# Удаление не числовых значений
# Приведение дискретности
# Объединение в сводный Data Frame