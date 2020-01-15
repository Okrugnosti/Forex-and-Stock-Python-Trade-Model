import os  # Системные функции
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
import ta  # https://github.com/bukosabino/ta

import sklearn as sk  # Машинное обучение

# модели линейных регрессий
from sklearn.linear_model import LinearRegression
from sklearn.linear_model import Ridge
from sklearn.linear_model import Lasso
from sklearn.linear_model import ElasticNet

# линейные модели классификации
from sklearn.linear_model import LogisticRegression
from sklearn.svm import LinearSVC

from sklearn.utils import shuffle
from sklearn.externals import joblib

# временная метка для отсечки скорости алгоритма
t1 = datetime.now(tz=None)


# ОБЩИЙ ПОРЯДОК ДЕЙСТВИЙ

# 1. Загружаем данные из CSV-файла
dataset = pd.read_csv('../../data/processed/1_Features_3_Load.05.csv', sep=';')
# dtype={'Pattern_Price_Delta_Price_200 Day Last': np.str,
#       'Pattern_3M(Max)_Rezultat_Sdelki_Cub_Summ_60Day_Svertka': np.str,
#       'Pattern_1M(Max)_Rezultat_Sdelki_Cub_Summ_60Day_Svertka': np.str})

# 2. Устанавливаем дату и время в качестве индекса
dataset['D'] = pd.to_datetime(dataset['D'].astype(str), format='%d.%m.%Y %H:%M')
dataset = dataset.set_index('D')

# 3. Препроцессинг исходных данных.

# 3.1 Выделение Dammi переменных
# Партия 1
Dammi_Minutes_Fitch = pd.get_dummies(dataset['Признак минуты'])
Dammi_Hours_Fitch = pd.get_dummies(dataset['Признак часа'])
Dammi_Day_Fitch = pd.get_dummies(dataset['Признак дня'])
Dammi_Month_Fitch = pd.get_dummies(dataset['Признак месяца'])
Dammi_Qvartal_Fitch = pd.get_dummies(dataset['Признак квартала'])

# Партия 1.1
Dammi_Price_If_New_Max = pd.get_dummies(dataset['Price_If_New_Max'])
Dammi_Price_If_New_Min = pd.get_dummies(dataset['Price_If_New_Min'])

# Партия 1.2.
Dammi_VOL_If_New_Max = pd.get_dummies(dataset['VOL_If_New_Max'])
Dammi_VOL_If_New_Min = pd.get_dummies(dataset['VOL_If_New_Min'])

# Партия 2
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

# Партия 3
Dammi_Pattern_Price_Delta_Price_200_Day_Last = pd.get_dummies(dataset['Pattern_Price_Delta_Price_200_Day_Last'])
Dammi_Pattern_Rezultat_Sdelki_Cub_Summ_60Day_3Mmax_Svertka = pd.get_dummies(
    dataset['Pattern_Rezultat_Sdelki_Cub_Summ_60Day_3Mmax_Svertka'])
Dammi_Pattern_Rezultat_Sdelki_Cub_Summ_60Day_1Mmax_Svertka = pd.get_dummies(
    dataset['Pattern_Rezultat_Sdelki_Cub_Summ_60Day_1Mmax_Svertka'])

# 4.2 Выделение Dammi переменных
# Разбиение временного ряда на интевалы.
# Выделение паттернов (сутки+недельные) - эффект памяти

########################################################


# 4. Формируем консолидированную выборку для ML
drop_frame = [dataset,
              Dammi_Minutes_Fitch, Dammi_Hours_Fitch,
              Dammi_Day_Fitch, Dammi_Month_Fitch, Dammi_Qvartal_Fitch,

              Dammi_Price_If_New_Max, Dammi_Price_If_New_Min,
              Dammi_VOL_If_New_Max, Dammi_VOL_If_New_Min,

              Dammi_P_MV_5vs15_Fitch, Dammi_P_MV_5vs30_Fitch, Dammi_P_MV_5vs60_Fitch,
              Dammi_P_MV_15vs30_Fitch, Dammi_P_MV_15vs60_Fitch, Dammi_P_MV_15vs90_Fitch,
              Dammi_P_MV_30vs60_Fitch, Dammi_P_MV_30vs90_Fitch, Dammi_P_MV_30vs200_Fitch,
              Dammi_P_MV_60vs90_Fitch, Dammi_P_MV_60vs200_Fitch, Dammi_P_MV_90vs200_Fitch,

              Dammi_Pattern_Price_Delta_Price_200_Day_Last,
              Dammi_Pattern_Rezultat_Sdelki_Cub_Summ_60Day_3Mmax_Svertka,
              Dammi_Pattern_Rezultat_Sdelki_Cub_Summ_60Day_1Mmax_Svertka
              ]

dataset2 = pd.concat(drop_frame, axis=1, sort=False)

# 5. Удаляем не совместимые с ML столбцы
columns = [
    'Признак минуты', 'Признак часа', 'Признак дня',
    'Признак месяца', 'Признак квартала',

    'Price_If_New_Max', 'Price_If_New_Min',
    'VOL_If_New_Max', 'VOL_If_New_Min',

    'P_MV_5vs15', 'P_MV_5vs30', 'P_MV_5vs60', 'P_MV_15vs30',
    'P_MV_15vs60', 'P_MV_15vs90', 'P_MV_30vs60', 'P_MV_30vs90',
    'P_MV_30vs200', 'P_MV_60vs90', 'P_MV_60vs200', 'P_MV_90vs200',
    'Price_If_New_Max', 'Price_If_New_Min', 'VOL_If_New_Max', 'VOL_If_New_Min',

    'P_MV_5vs15 (IF)', 'P_MV_5vs30 (IF)', 'P_MV_5vs60 (IF)', 'P_MV_15vs30 (IF)',
    'P_MV_15vs60 (IF)', 'P_MV_15vs90 (IF)', 'P_MV_30vs60 (IF)',
    'P_MV_30vs90 (IF)', 'P_MV_30vs200 (IF)', 'P_MV_60vs90 (IF)',
    'P_MV_60vs200 (IF)', 'P_MV_90vs200 (IF)', '<Rezultat_Sdelki_Cub>',

    'Pattern_Price_Delta_Price_200_Day_Last',
    'Pattern_Rezultat_Sdelki_Cub_Summ_60Day_3Mmax_Svertka',
    'Pattern_Rezultat_Sdelki_Cub_Summ_60Day_1Mmax_Svertka'

]

dataset2 = dataset2.drop(columns, 1)

print('Data Set с DammiPer - columns name (1/5): \n', dataset2.iloc[:, 0:50].columns)
print('Data Set с DammiPer - columns name (2/5): \n', dataset2.iloc[:, 50:100].columns)
print('Data Set с DammiPer - columns name (3/5): \n', dataset2.iloc[:, 100:150].columns)
print('Data Set с DammiPer - columns name (4/5): \n', dataset2.iloc[:, 150:200].columns)
print('Data Set с DammiPer - columns name (5/5): \n', dataset2.iloc[:, 200:250].columns)

'''
       '<OPEN>', '<HIGH>', '<LOW>', '<CLOSE>', 'P_Max-Min',
       'Delta_Price_ 1 Day Last', 'Delta_Price_ 5 Day Last',
       'Delta_Price_15 Day Last', 'Delta_Price_30 Day Last',
       'Delta_Price_60 Day Last', 'Delta_Price_90 Day Last',
       'Delta_Price_200 Day Last', 'Price_ 1 Day Last', 'Price_ 5 Day Last',
       'Price_15 Day Last', 'Price_30 Day Last', 'Price_60 Day Last',
       'Price_90 Day Last', 'Price_200 Day Last', 'Price_Max 200 Day',
       'Price_Min 200 Day', 'Price_Delta Min - Max 200 Day',
       'Price_Distancia_Min - Max 200 Day', '<VOL>', 'VOL-Summ-30',
       'VOL_Max 200 Day', 'VOL_Min 200 Day', 'VOL_Delta Min - Max 200 Day',
       'VOL_Distancia_Min - Max 200 Day', 'VOL_V/Pmax-Pmin', 'Price_Vol 5 Day',
       'Price_Vol 15 Day', 'Price_Vol 30 Day', 'Price_Vol 60 Day',
       'Price_Vol 90 Day', 'Price_Vol 200 Day', 'Price_Momentum 5 Day',
       'Price_Momentum 15 Day', 'Price_Momentum 30 Day',
       'Price_Momentum 60 Day', 'Price_Momentum 90 Day',
       'Price_Momentum 200 Day', 'Price_Moving_Average 5 Day',
       'Price_Moving_Average 15 Day', 'Price_Moving_Average 30 Day',
       'Price_Moving_Average 60 Day', 'Price_Moving_Average 90 Day',
       'Price_Moving_Average 200 Day', 'PX_OPEN',
       '<Rezultat_Sdelki_Cub>_Summ_60Day'

       '<Rezultat_Sdelki_Cub>_Summ_90Day', '<Rezultat_Sdelki_Cub>_Summ_200Day',
       'Raspredilenie_Sdelok -2', 'Raspredilenie_Sdelok -1',
       'Raspredilenie_Sdelok 0', 'Raspredilenie_Sdelok +1',
       'Raspredilenie_Sdelok +2', 'Raspredilenie_Sdelok +3',
       'Raspredilenie_Sdelok +4', 'Raspredilenie_Sdelok +5 >',
       'Raspredilenie_Sdelok +10 >', 'Raspredilenie_Sdelok +15 >',
       'Raspredilenie_Sdelok +20 >', 'Nakoplennaya glubina prosadki_Cub',
       '<Rezultat_Sdelki_Retrospektiva>', '<CLOSE>.1', 'T_minutes_0',
       'T_minutes_10', 'T_minutes_15', 'T_minutes_20', 'T_minutes_25',
       'T_minutes_30', 'T_minutes_35', 'T_minutes_40', 'T_minutes_45',
       'T_minutes_5', 'T_minutes_50', 'T_minutes_55', 'T_hour_10', 'T_hour_11',
       'T_hour_12', 'T_hour_13', 'T_hour_14', 'T_hour_15', 'T_hour_16',
       'T_hour_17', 'T_hour_18', 'T_hour_19', 'T_hour_20', 'T_hour_21',
       'T_hour_22', 'T_hour_23', 'воскресенье', 'вторник', 'понедельник',
       'пятница', 'среда', 'суббота', 'четверг', 'Август'

       'Апрель', 'Декабрь', 'Июль', 'Июнь', 'Май', 'Март', 'Ноябрь', 'Октябрь',
       'Сентябрь', 'Февраль', 'Январь', 'Q1', 'Q2', 'Q3', 'Q4', 'Price_NewMax',
       'Price_NonNewMax', 'Price_NewMin', 'Price_NonNewMin', 'VOL_NewMax',
       'VOL_NonNewMax', 'VOL_NewMin', 'VOL_NonNewMin', 'P_MV_5vs15_Buy',
       'P_MV_5vs15_Sell', 'P_MV_5vs30_Buy', 'P_MV_5vs30_Sell',
       'P_MV_5vs60_Buy', 'P_MV_5vs60_Sell', 'P_MV_15vs30_Buy',
       'P_MV_15vs30_Sell', 'P_MV_15vs60_Buy', 'P_MV_15vs60_Sell',
       'P_MV_15vs90_Buy', 'P_MV_15vs90_Sell', 'P_MV_30vs60_Buy',
       'P_MV_30vs60_Sell', 'P_MV_30vs90_Buy', 'P_MV_30vs90_Sell',
       'P_MV_30vs200_Buy', 'P_MV_30vs200_Sell', 'P_MV_60vs90_Buy',
       'P_MV_60vs90_Sell', 'P_MV_60vs200_Buy', 'P_MV_60vs200_Sell',
       'P_MV_90vs200_Buy', 'P_MV_90vs200_Sell'

       '-0.5_-0.5_-0.5',
       '-0.5_-0.5_0', '-0.5_0_0', '0.5_0.5_0', '0.5_0.5_0.5', '0.5_0.5_1',
       '0.5_0_0', '0.5_0_0.5', '0.5_1.5_1.5', '0.5_1_0.5', '0.5_1_1',
       '0.5_1_1.5', '0_-0.5_-0.5', '0_0.5_0', '0_0.5_0.5', '0_0.5_1',
       '0_0.5_1.5', '0_0_-0.5', '0_0_0', '0_0_0.5', '0_0_1', '0_1_1.5',
       '1.5_1.5_2', '1.5_1_1', '1.5_2_2', '1_0.5_0.5', '1_0.5_1', '1_1.5_1.5',
       '1_1.5_2', '1_1_0.5', '1_1_1', '2_1.5_1', '2_1_1', '2_2_1', '2_2_1.5',
       '2_2_2', '-1_-1_-1', '-1_-1_1', '-1_-1_2', '-1_1_1', '-1_1_2',
       '-1_1_3'

       '-1_2_3', '1_-1_-1', '1_-1_1', '1_-1_2', '1_1_-1', '1_1_1', '1_1_2',
       '1_2_1', '1_2_2', '1_2_3', '1_3_3', '2_-1_-1', '2_-1_1', '2_-1_2',
       '2_1_1', '2_1_2', '2_2_1', '2_2_2', '2_2_3', '2_3_3', '3_-1_-1',
       '3_1_-1', '3_2_-1', '3_2_2', '3_3_-1', '3_3_1', '3_3_2', '3_3_3',
       '-1_-1_-1', '-1_-1_-2', '-1_-1_0', '-1_-2_-1', '-1_-2_-2', '-1_0_-1',
       '-1_0_0', '-1_0_2', '-2_-1_-1', '-2_-1_-2', '-2_-1_0', '-2_-2_-1',
       '-2_-2_-2', '0_-1_-1', '0_-1_0', '0_0_-1', '0_0_0', '0_0_2', '0_0_3',
       '0_2_0', '0_2_2', '0_2_3'

'''

# 6. Разделяем файл на "Х-фичи" и "Y-прогноз", путем выбора столбцов

Y = dataset2['<Rezultat_Sdelki_Retrospektiva_Class>']
X = dataset2.drop([
    '<CLOSE>_T+5M',
    '<CLOSE>_T+10M',
    '<CLOSE>_T+15M',
    '<CLOSE>_T+30M',
    '<CLOSE>_T+60M',
    '<CLOSE>_T+2H',
    '<CLOSE>_T+3H',
    '<CLOSE>_T+6H',
    '<CLOSE>_T+12H',
    '<CLOSE>_T+18H',
    '<Rezultat_Sdelki_Retrospektiva>',
    '<Rezultat_Sdelki_Retrospektiva_Class>'], 1)

# X = X.iloc[:, s:k]
X_train, X_test, Y_train, Y_test = sk.model_selection.train_test_split(X, Y, train_size=0.5, random_state=0)

print(Y_train)


'''
#График 1 - плотность рассеивания обучающей выборки
plt.plot(Y_train, 'o', label="Y_train")
plt.plot(Y_test, 'v', label="Y_test")
plt.legend(ncol=2, loc=(0, 1.05))
plt.ylabel("Y_train")
plt.xlabel("Y_test")
plt.show()
'''

# 8. Запускаем ML-алгоритмы. Проводим обучение

#Линейная регрессия - классификация
lg = LogisticRegression(penalty='l2', solver='lbfgs')
lg.fit(X_train, Y_train)

print('\n', 'LogisticRegression')
print("Правильность на обучающем наборе: {:.2f}".format(lg.score(X_train, Y_train)))
print("Правильность на тестовом наборе: {:.2f}".format(lg.score(X_test, Y_test)))

'''
# L1 - регуляризация
lg_SVC = LinearSVC.fit(X_train, Y_train)
print('\n', 'LinearSVC')
print("Правильность на обучающем наборе: {:.2f}".format(lg_SVC.score(X_train, Y_train)))
print("Правильность на контрольном наборе: {:.2f}".format(lg_SVC.score(X_test, Y_test)))
'''

#выгрузка параметров модели в файл
joblib.dump(lg, '../../data/interim/5.1_ML_Lg_Binare_Class_Signal_01.txt')
#загрузка готовых параметров модели из файла
#lg = joblib.load('../../data/interim/5_ML_Lasso_Price_T+5M.txt')


# 8.1 Моделируем на несколько дней вперед
########################################

# .score() принимает в качестве аргументов предсказатель x и регрессор y, и возвращает значение
# .intercept_это скаляр
# .coef_- массив весов
# .predict() - предсказание ответа

print('lg.coef_- матрица значимых признаков и их параметров: \n', lg.coef_)

'''
# График 2 - выделение значимых признаков
fruits = X.columns
counts = lg.coef_
plt.plot(fruits, counts[0], '^', label="Значимость признаков в ML")
plt.xticks(rotation=90)
plt.legend()
plt.show()
'''

# 9. Запускаем прогноз на контрольной выборке
dataset2['Log_Regression_Predict'] = lg.predict(X)

#Вывод вероятности признака
Veroatnost_Priznaka = lg.predict_proba(X)
print('Veroatnost_Priznaka: \n', Veroatnost_Priznaka[:, 0])

dataset2['Veroatnost_Priznaka'] = Veroatnost_Priznaka[:, 0]

#Вывод результатов прогноза
dataset2['Delta_Lg_X_Y'] = dataset2['Log_Regression_Predict'] == dataset2['<Rezultat_Sdelki_Retrospektiva_Class>']
print('Delta_Lg_X_Y: \n', dataset2['Delta_Lg_X_Y'])

'''
# График 3 - разница между фактом и прогнозом
plt.plot(dataset2['Delta_Lg_X_Y'], 'o', label="Разница между фактом и прогнозом")
plt.legend()
plt.show()
'''

# 10. Выводим результаты модели в сводный файл. Объединяем результаты, сравниваем прогноз с фактом
dataset2.to_csv('../../data/interim/6_Rezalt_ML_Lg_01.csv', encoding='utf-8', sep=',')  # index=None,
# dataset2.to_excel('../../data/interim/6_Rezalt_ML_Lg_01.xlsx', startrow=3, index=True)
print("Write File - Ok")

t2 = datetime.now(tz=None)
print("Время работы алгоритма:", t2 - t1)