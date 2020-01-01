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
import ta #https://github.com/bukosabino/ta

import sklearn as sk  # Машинное обучение

#модели линейных регрессий
from sklearn.linear_model import LinearRegression
from sklearn.linear_model import Ridge
from sklearn.linear_model import Lasso
#линейные модели классификации
from sklearn.linear_model import ElasticNet
from sklearn.linear_model import LogisticRegression
from sklearn.svm import LinearSVC
from sklearn.utils import shuffle


def features_crieted_price_series(trade_cube, colums_name ,time_convert, price_series):

    convert_market_data = trade_cube.resample(time_convert) \
        .agg({'<OPEN>': 'first',
              '<HIGH>': np.max,
              '<LOW>': np.min,
              '<CLOSE>': 'last',
              '<VOL>': np.sum})

    # удаление пустых строчек
    convert_market_data = convert_market_data.dropna()
    #print(convert_market_data)


    viborka = convert_market_data['<CLOSE>'].count()

    # создание пустого массива для хранения отрезка данных
    price_massive = pd.DataFrame(columns=range(0, price_series))
    #print(price_massive)

    # заполнение матрицы отрезками
    for i in tqdm(range(0, viborka)):
        if i > (price_series + 1):
            x = convert_market_data['<CLOSE>'].iloc[i - price_series: i-1]
            x.index = range(0, price_series-1)
            price_massive = price_massive.append(x)
            price_massive.ix[i - price_series - 2, price_series-1] = convert_market_data.index[i]

    convert_market_data = price_massive.set_index([price_series-1])

    # устанавливаем индекс в виде даты
    convert_market_data['DD'] = convert_market_data.index
    convert_market_data['DD'] = pd.to_datetime(convert_market_data['DD'], format='%Y-%m-%d %H:%M:%S')
    print(convert_market_data) #, drop=False))

    #расчет индикаторов
    std_x = convert_market_data.T.std() #стандартное отклонение
    means_x = convert_market_data.T.mean() #средняя цена

    return convert_market_data


#считываем данные из подготовленного файле, устанавливаем дату в качестве индекса
trade_cube = pd.read_csv('D://GitHub/Forex-and-Stock-Python-Trade-Model/data/interim/3_Train_Massiv_1.txt', sep=',')
trade_cube['D'] = pd.to_datetime(trade_cube['D'], format='%Y-%m-%d %H:%M:%S')
trade_cube = trade_cube.set_index('D')
print(trade_cube.info())
print(trade_cube)

#price_series_close_5D = features_crieted_price_series(trade_cube, '<CLOSE>', '1D', 5)
price_series_close_90D = features_crieted_price_series(trade_cube, '<CLOSE>', '5M', 10)
#price_series_close_360D = features_crieted_price_series(trade_cube, '<CLOSE>', '1M', 12)



# добавляем в выборку usd_rub
frame = trade_cube, price_series_close_90D
trade_cube = pd.concat(frame, sort=True)


# протягиваем курс доллара на промежуточные тайм-фреймы
# формируем итоговую выборку
#trade_cube['PX_OPEN'] = trade_cube['PX_OPEN'].sort_index().fillna(method='ffill')


#trade_cube = create_volatility_model(trade_cube)

#trade_cube.to_csv('D://GitHub/Forex-and-Stock-Python-Trade-Model/data/processed/1_Train_Features_Massiv_1.txt', encoding='utf-8', sep=',')  # index=None,
print('Txt file write')

trade_cube.ix[0:10000].to_excel('D://GitHub/Forex-and-Stock-Python-Trade-Model/data/processed/1_Features_1.xlsx', startrow=3, index=True)
print('Excel file write')


