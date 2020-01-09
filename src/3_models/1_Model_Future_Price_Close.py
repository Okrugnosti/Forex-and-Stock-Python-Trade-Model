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

#ОБЩИЙ ПОРЯДОК ДЕЙСТВИЙ

#1. Загружаем данные из CSV-файла
dataset = pd.read_csv('D://GitHub/Forex-and-Stock-Python-Trade-Model/data/processed/1_Features_3_Load.csv', sep=';')

#2. Устанавливаем дату и время в качестве индекса
dataset['D'] = pd.to_datetime(dataset['D'].astype(str), format='%d.%m.%Y %H:%M')
dataset = dataset.set_index('D')
#print('dataset: \n', dataset.info())

#3. Препроцессинг исходных данных. Выделение Dammi переменных

#Партия 1
Dammi_Minutes_Fitch = pd.get_dummies(dataset['Признак минуты'])
Dammi_Hours_Fitch = pd.get_dummies(dataset['Признак часа'])
Dammi_Day_Fitch = pd.get_dummies(dataset['Признак дня'])
Dammi_Month_Fitch = pd.get_dummies(dataset['Признак месяца'])
Dammi_Qvartal_Fitch = pd.get_dummies(dataset['Признак квартала'])

#Партия 2
Dammi_P_MV_5vs15_Fitch = pd.get_dummies(dataset['P_MV_5vs15'])
Dammi_P_MV_5vs30_Fitch = pd.get_dummies(dataset['P_MV_5vs30'])
Dammi_P_MV_5vs60_Fitch = pd.get_dummies(dataset['P_MV_5vs60'])
Dammi_P_MV_15vs30_Fitch = pd.get_dummies(dataset['P_MV_15vs30'])
Dammi_P_MV_15vs60_Fitch = pd.get_dummies(dataset['P_MV_15vs60'])
Dammi_P_MV_15vs90_Fitch = pd.get_dummies(dataset['P_MV_15vs90'])
Dammi_P_MV_30vs60_Fitch = pd.get_dummies(dataset['P_MV_30vs60'])
Dammi_P_MV_30vs90_Fitch = pd.get_dummies(dataset['P_MV_30vs90'])
Dammi_P_MV_30vs200_Fitch = pd.get_dummies(dataset['P_MV_30vs200'])
Dammi_P_MV_60vs90_Fitch = pd.get_dummies(dataset['P_MV_60vs90'])
Dammi_P_MV_60vs200_Fitch = pd.get_dummies(dataset['P_MV_60vs200'])
Dammi_P_MV_90vs200_Fitch = pd.get_dummies(dataset['P_MV_90vs200'])

#Партия 3
Dammi_P_MV_5vs15_IF_Fitch = pd.get_dummies(dataset['P_MV_5vs15 (IF)'])
Dammi_P_MV_5vs15_IF_Fitch = Dammi_P_MV_5vs15_IF_Fitch[['Buy', 'Sell']]

Dammi_P_MV_5vs30_IF_Fitch = pd.get_dummies(dataset['P_MV_5vs30 (IF)'])
Dammi_P_MV_5vs30_IF_Fitch = Dammi_P_MV_5vs15_IF_Fitch[['Buy', 'Sell']]

Dammi_P_MV_5vs60_IF_Fitch = pd.get_dummies(dataset['P_MV_5vs60 (IF)'])
Dammi_P_MV_5vs60_IF_Fitch = Dammi_P_MV_5vs15_IF_Fitch[['Buy', 'Sell']]

Dammi_P_MV_15vs30_IF_Fitch = pd.get_dummies(dataset['P_MV_15vs30 (IF)'])
Dammi_P_MV_15vs30_IF_Fitch = Dammi_P_MV_5vs15_IF_Fitch[['Buy', 'Sell']]

Dammi_P_MV_15vs60_IF_Fitch = pd.get_dummies(dataset['P_MV_15vs60 (IF)'])
Dammi_P_MV_15vs60_IF_Fitch = Dammi_P_MV_5vs15_IF_Fitch[['Buy', 'Sell']]

Dammi_P_MV_15vs90_IF_Fitch = pd.get_dummies(dataset['P_MV_15vs90 (IF)'])
Dammi_P_MV_15vs90_IF_Fitch = Dammi_P_MV_5vs15_IF_Fitch[['Buy', 'Sell']]

Dammi_P_MV_30vs60_IF_Fitch = pd.get_dummies(dataset['P_MV_30vs60 (IF)'])
Dammi_P_MV_30vs60_IF_Fitch = Dammi_P_MV_5vs15_IF_Fitch[['Buy', 'Sell']]

Dammi_P_MV_30vs90_IF_Fitch = pd.get_dummies(dataset['P_MV_30vs90 (IF)'])
Dammi_P_MV_30vs90_IF_Fitch = Dammi_P_MV_5vs15_IF_Fitch[['Buy', 'Sell']]

Dammi_P_MV_30vs200_IF_Fitch = pd.get_dummies(dataset['P_MV_30vs200 (IF)'])
Dammi_P_MV_30vs200_IF_Fitch = Dammi_P_MV_5vs15_IF_Fitch[['Buy', 'Sell']]

Dammi_P_MV_60vs90_IF_Fitch = pd.get_dummies(dataset['P_MV_60vs90 (IF)'])
Dammi_P_MV_60vs90_IF_Fitch = Dammi_P_MV_5vs15_IF_Fitch[['Buy', 'Sell']]

Dammi_P_MV_60vs200_IF_Fitch = pd.get_dummies(dataset['P_MV_60vs200 (IF)'])
Dammi_P_MV_60vs200_IF_Fitch = Dammi_P_MV_5vs15_IF_Fitch[['Buy', 'Sell']]

Dammi_P_MV_90vs200_IF_Fitch = pd.get_dummies(dataset['P_MV_90vs200 (IF)'])
Dammi_P_MV_90vs200_IF_Fitch = Dammi_P_MV_5vs15_IF_Fitch[['Buy', 'Sell']]



#4. Формируем консолидированную выборку для ML

frame = [dataset,
          Dammi_Minutes_Fitch, Dammi_Hours_Fitch,
          Dammi_Day_Fitch, Dammi_Month_Fitch, Dammi_Qvartal_Fitch,

          Dammi_P_MV_5vs15_Fitch, Dammi_P_MV_5vs30_Fitch, Dammi_P_MV_5vs60_Fitch,
          Dammi_P_MV_15vs30_Fitch, Dammi_P_MV_15vs60_Fitch, Dammi_P_MV_15vs90_Fitch,
          Dammi_P_MV_30vs60_Fitch, Dammi_P_MV_30vs90_Fitch, Dammi_P_MV_30vs200_Fitch,
          Dammi_P_MV_60vs90_Fitch, Dammi_P_MV_60vs200_Fitch, Dammi_P_MV_90vs200_Fitch,

          Dammi_P_MV_5vs15_IF_Fitch, Dammi_P_MV_5vs30_IF_Fitch,
          Dammi_P_MV_5vs60_IF_Fitch, Dammi_P_MV_15vs30_IF_Fitch,
          Dammi_P_MV_15vs60_IF_Fitch, Dammi_P_MV_15vs90_IF_Fitch,
          Dammi_P_MV_30vs60_IF_Fitch, Dammi_P_MV_30vs90_IF_Fitch,
          Dammi_P_MV_30vs200_IF_Fitch, Dammi_P_MV_60vs90_IF_Fitch,
          Dammi_P_MV_60vs200_IF_Fitch, Dammi_P_MV_90vs200_IF_Fitch
        ]


dataset2 = pd.concat(frame, axis=1, sort=False)
#print('dataset2: \n', dataset2.info())

#5. Удаляем не совместимые с ML столбцы

columns = ['Признак минуты', 'Признак часа', 'Признак дня',
            'Признак месяца', 'Признак квартала', 'P_MV_5vs15',
            'P_MV_5vs30', 'P_MV_5vs60', 'P_MV_15vs30',
            'P_MV_15vs60', 'P_MV_15vs90', 'P_MV_30vs60',
            'P_MV_30vs90', 'P_MV_30vs200', 'P_MV_60vs90',
            'P_MV_60vs200', 'P_MV_90vs200',
            'Price_If_New_Max', 'Price_If_New_Min', 'VOL_If_New_Max', 'VOL_If_New_Min',
            'P_MV_5vs15 (IF)',
            'P_MV_5vs30 (IF)', 'P_MV_5vs60 (IF)', 'P_MV_15vs30 (IF)',
            'P_MV_15vs60 (IF)', 'P_MV_15vs90 (IF)', 'P_MV_30vs60 (IF)',
            'P_MV_30vs90 (IF)', 'P_MV_30vs200 (IF)', 'P_MV_60vs90 (IF)',
            'P_MV_60vs200 (IF)', 'P_MV_90vs200 (IF)', '<Rezultat_Sdelki_Cub>'
            ]

dataset3 = dataset2.drop(columns, 1)
#print('dataset3: \n', dataset3.iloc[:, 0:5].columns.names)

#6. Разделяем файл на "Х-фичи" и "Y-прогноз", путем выбора столбцов

s = 3
k = 63

Y = dataset3['<CLOSE>.1']
X = dataset3.drop('<CLOSE>.1', 1)

print(X.iloc[:, 0:50].columns)
print(X.iloc[:, 50:100].columns)
print(X.iloc[:, 100:159].columns)

X = X.iloc[:, s:k]

#обрезаем выборку
#Y = Y.iloc[50000:171155]
#X = X.iloc[50000:171155]

#print('Выборка Х.name:\n ', X.info())
#print('Выборка Y:\n ', Y)

'''
       #общее кол-во столбцов 159
        
       '<OPEN>', '<HIGH>', '<LOW>', '<CLOSE>', 'Delta_Price_ 1 Day Last',
       'Delta_Price_ 5 Day Last', 'Delta_Price_15 Day Last',
       'Delta_Price_30 Day Last', 'Delta_Price_60 Day Last',
       'Delta_Price_90 Day Last', 'Delta_Price_200 Day Last',
       'Price_ 1 Day Last', 'Price_ 5 Day Last', 'Price_15 Day Last',
       'Price_30 Day Last', 'Price_60 Day Last', 'Price_90 Day Last',
       'Price_200 Day Last', 'Price_Max 200 Day', 'Price_Min 200 Day',
       'Price_Delta Min - Max 200 Day', 'Price_Distancia_Min - Max 200 Day',
       '<VOL>', 'VOL-Summ-30', 'VOL_Max 200 Day', 'VOL_Min 200 Day',
       'VOL_Delta Min - Max 200 Day', 'VOL_Distancia_Min - Max 200 Day',
       'Price_Vol 5 Day', 'Price_Vol 15 Day', 'Price_Vol 30 Day',
       'Price_Vol 60 Day', 'Price_Vol 90 Day', 'Price_Vol 200 Day',
       'Price_Momentum 5 Day', 'Price_Momentum 15 Day',
       'Price_Momentum 30 Day', 'Price_Momentum 60 Day',
       'Price_Momentum 90 Day', 'Price_Momentum 200 Day',
       'Price_Moving_Average 5 Day', 'Price_Moving_Average 15 Day',
       'Price_Moving_Average 30 Day', 'Price_Moving_Average 60 Day',
       'Price_Moving_Average 90 Day', 'Price_Moving_Average 200 Day',
       'PX_OPEN', '<Rezultat_Sdelki_Cub>_Summ_60Day',
       '<Rezultat_Sdelki_Cub>_Summ_90Day', '<Rezultat_Sdelki_Cub>_Summ_200Day',

       'Raspredilenie_Sdelok -2', 'Raspredilenie_Sdelok -1', 'Raspredilenie_Sdelok 0',
       'Raspredilenie_Sdelok +1', 'Raspredilenie_Sdelok +2',
       'Raspredilenie_Sdelok +3', 'Raspredilenie_Sdelok +4',
       'Raspredilenie_Sdelok +5 >', 'Raspredilenie_Sdelok +10 >',
       'Raspredilenie_Sdelok +15 >', 'Raspredilenie_Sdelok +20 >',
       'Nakoplennaya glubina prosadki_Cub', '<Rezultat_Sdelki_Retrospektiva>',
       0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 10, 11, 12, 13, 14,
       15, 16, 17, 18, 19, 20, 21, 22, 23, 
       'воскресенье', 'вторник', 'понедельник', 'пятница',
       'среда', 'суббота', 'четверг', 'Август', 'Апрель', 'Декабрь', 'Июль'],

       'Июнь', 'Май', 'Март', 'Ноябрь', 'Октябрь', 'Сентябрь', 'Февраль',
       'Январь', 'Q1', 'Q2', 'Q3', 'Q4', 'Buy', 'Sell', 'Buy', 'Sell', 'Buy',
       'Sell', 'Buy', 'Sell', 'Buy', 'Sell', 'Buy', 'Sell', 'Buy', 'Sell',
       'Buy', 'Sell', 'Buy', 'Sell', 'Buy', 'Sell', 'Buy', 'Sell', 'Buy',
       'Sell', 'Buy', 'Sell', 'Buy', 'Sell', 'Buy', 'Sell', 'Buy', 'Sell',
       'Buy', 'Sell', 'Buy', 'Sell', 'Buy', 'Sell', 'Buy', 'Sell', 'Buy',
       'Sell', 'Buy', 'Sell', 'Buy', 'Sell', 'Buy'
'''

#for col_name in range(63):
#    print(col_name)

print(X[0])


#7. Разбиваем на тестовую и проверочную выборку

# загружем выборку и разбиваем не тестовую и боевыую
X_train, X_test, Y_train, Y_test = sk.model_selection.train_test_split(X, Y, train_size = 0.5, random_state=0)

#print(X_train.info())
#print(Y_train.info())


'''
#График 3
plt.plot(X_train, 'o', label="Y_train")
plt.plot(X_test, 'v', label="Y_test")
plt.legend(ncol=2, loc=(0, 1.05))
plt.ylabel("X_train")
plt.xlabel("X_test")
plt.show()
'''

#8. Запускаем ML-алгоритмы. Проводим обучение

#Модель линейной регрессии
'''
lr = LinearRegression().fit(X_train, Y_train)
print('\n', 'LinearRegressi')
print("Правильность на обучающем наборе: {:.2f}".format(lr.score(X_train, Y_train)))
print("Правильность на тестовом наборе: {:.2f}".format(lr.score(X_test, Y_test)))
'''

# L1 - регуляризация
lasso = Lasso(alpha=0.71, max_iter=100000).fit(X_train, Y_train)
print('\n', 'lasso')
print("Правильность на обучающем наборе: {:.2f}".format(lasso.score(X_train, Y_train)))
print("Правильность на контрольном наборе: {:.2f}".format(lasso.score(X_test, Y_test)))
print("Количество использованных признаков: {}".format(np.sum(lasso.coef_ != 0)))


'''
# L1 и L2 - регуляризация
ElasticN = ElasticNet(alpha=0.01, random_state=0).fit(X_train, Y_train)
print('\n', 'ElasticN')
print("Правильность на обучающем наборе: {:.2f}".format(ElasticN.score(X_train, Y_train)))
print("Правильность на контрольном наборе: {:.2f}".format(ElasticN.score(X_test, Y_test)))
print("Количество использованных признаков: {}".format(np.sum(ElasticN.coef_ != 0)))
'''


    #.score() принимает в качестве аргументов предсказатель x и регрессор y, и возвращает значение
    #.intercept_это скаляр
    #.coef_- массив весов
    #.predict() - предсказание ответа


#График 3
fruits = X.columns
counts = lasso.coef_
plt.plot(fruits, counts, '^', label="Значимость признаков в ML")
plt.xticks(rotation = 90)
plt.legend()
plt.show()

print('lr.coef_: \n', lasso.coef_)

#9. Запускаем прогноз на контрольной выборке

dataset3['Line_Regression_Predict'] = lasso.predict(dataset3.iloc[:, s:k]) #, 1) X) #[['a', 'b', 'c']])
#print('Line_Regression_Predict: \n', dataset3)

dataset3['Delta_X_Y'] = dataset3['Line_Regression_Predict'] - dataset3['<CLOSE>']
dataset3['Delta%_X_Y'] = dataset3['Delta_X_Y'] / dataset3['<CLOSE>'] * 100


'''
#График 2 - разница между фактом и прогнозом
plt.plot(dataset3['Delta_X_Y'], 'o', label="Delta_X_Y")
plt.show()
'''

print('Delta_X_Y: \n', dataset3['Delta_X_Y'])


'''
#График 1
plt.plot(dataset3[['<CLOSE>']], 'o', label="Факт")
plt.plot(dataset3['Line_Regression_Predict'], 'v', label="Прогноз")
plt.legend(ncol=2, loc=(0, 1.05))
plt.ylabel("Значение")
plt.xlabel("Итерация")
plt.show()
'''


#10. Выводим результаты модели в сводный файл. Объединяем результаты, сравниваем прогноз с фактом

#dataset3.to_csv('D://GitHub/Forex-and-Stock-Python-Trade-Model/data/interim/6_DataSet_fot_ML.txt', encoding='utf-8', sep=',')  # index=None,
#dataset3.to_excel('D://GitHub/Forex-and-Stock-Python-Trade-Model/data/interim/6_DataSet_fot_ML.xlsx', startrow=3, index=True)
print("Write File Ok")
