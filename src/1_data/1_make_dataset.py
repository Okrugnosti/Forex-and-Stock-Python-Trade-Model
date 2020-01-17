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
import pickle

#созданные мною библиотеки
import data_preprocessing

'''
Модуль 1. Склейка базовых данных и генерация сводной стартовой выборки для торговой мдели
см. Trade_Cube_1.txt
'''

# временная метка для отсечки скорости алгоритма
t1 = datetime.now(tz=None)

trade_cube = pd.DataFrame()
usd_rub = pd.DataFrame()

# получаем "склеинный" массив исходных данных из разных исходных файлов
trade_cube = data_preprocessing.data_frame_inicialization(trade_cube, '../../data/external/SPFB.RTS/', ',') #os.path.dirname('\data\external\SPFB.RTS')
usd_rub = data_preprocessing.data_frame_inicialization(usd_rub, '../../data/external/USD_RUB/', ';') #os.path.dirname('/data/external/USD_RUB/')

# преобразуем дату и время из строки в формат datetime
trade_cube['D'] = trade_cube['<DATE>'].astype(str) + trade_cube['<TIME>'].astype(str)
trade_cube = data_preprocessing.data_convertor(trade_cube, '%Y%m%d%H%M%S')

usd_rub['D'] = usd_rub['Date'].astype(str)
usd_rub = data_preprocessing.data_convertor(usd_rub, '%d.%m.%Y')

# конвертация времени в таймфреймы, удаляем пустые значения
# записываем результаты в файл
trade_cube = data_preprocessing.discretizacia_data('5T', trade_cube)

# добавляем в выборку usd_rub
frame = trade_cube, usd_rub
trade_cube = pd.concat(frame, sort=True)

# протягиваем курс доллара на промежуточные тайм-фреймы
# формируем итоговую выборку
trade_cube['PX_OPEN'] = trade_cube['PX_OPEN'].sort_index().fillna(method='ffill')
trade_cube = trade_cube[['<OPEN>', '<HIGH>', '<LOW>', '<CLOSE>', '<VOL>', 'PX_OPEN']].sort_index().dropna()

# запись результатов из pandas DataFrame в pickle формат и в файл
with open('../../data/interim/1_Trade_Cube_1.pickle', 'wb') as f:
     pickle.dump(trade_cube, f)

#data_preprocessing.data_frame_write(trade_cube, '../../data/interim/1_Trade_Cube_1.txt') #'D://GitHub/Forex-and-Stock-Python-Trade-Models/data/interim/1_Trade_Cube_1.txt'


print(trade_cube.info())
print(trade_cube)

t2 = datetime.now(tz=None)
print('File Write Ok')
print(t2 - t1)