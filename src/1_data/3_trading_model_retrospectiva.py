import os  # Системные функции
from datetime import datetime  # Класс по работе с датой
import pytz  # База часовых поясов
import openpyxl as xl  # Excel
import numpy as np  # Матрицы
import pandas as pd  # DataFrame
import matplotlib.pyplot as plt  # Построение графиков
import seaborn as sns  # Построение графиков
import sklearn as sk  # Машинное обучение
import scipy  # Выполнениt научных и инженерных расчётов
import data_preprocessing

#считываем данные из подготовленного файле, устанавливаем дату в качестве индекса
trade_cube = pd.read_csv('D://GitHub/Forex-and-Stock-Python-Trade-Model/data/interim/2_Resalt_Trade_Model_1.txt', sep=',')
trade_cube = trade_cube.set_index('D')

o1 = trade_cube['<Close_Position>'].replace(0, np.nan).fillna(method = 'bfill')
trade_cube['<Close_Position_Retrospektiva>'] = o1
print(trade_cube.info())

'''
def f(x):
    return x['<CLOSE>'] - x['<Open_Position>']

o2 = trade_cube.apply(f, axis=1)
'''

o3 = trade_cube.apply(lambda x:
                              x['<CLOSE>'] - x['<Open_Position>']
                              if x['<Vector_Position>'] == 1
                              else x['<Open_Position>'] - x['<CLOSE>'], axis=1)

trade_cube['<Rezultat_Sdelki_Retrospektiva>'] = o3

data_preprocessing.data_frame_write(trade_cube, 'D://GitHub/Forex-and-Stock-Python-Trade-Model/data/interim/3_Train_Massiv_1.txt')
trade_cube.to_excel('D://GitHub/Forex-and-Stock-Python-Trade-Model/data/interim/3_Train_Massiv_1.xlsx', startrow=3, index=True)

print('File Write')
