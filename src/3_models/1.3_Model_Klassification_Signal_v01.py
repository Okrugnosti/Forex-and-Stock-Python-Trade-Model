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

import sklearn as sk  # Машинное обучение

# модели линейных регрессий
from sklearn.linear_model import LinearRegression
from sklearn.linear_model import Ridge
from sklearn.linear_model import Lasso
from sklearn.linear_model import ElasticNet

# линейные модели классификации
from sklearn.linear_model import LogisticRegression
from sklearn.svm import LinearSVC

from sklearn.metrics import classification_report

from sklearn.utils import shuffle
from sklearn.externals import joblib

# временная метка для отсечки скорости алгоритма
t1 = datetime.now(tz=None)

# ОБЩИЙ ПОРЯДОК ДЕЙСТВИЙ

#1. Загружаем данные из файла
with open('../../data/interim/5_Learning_set.pickle', 'rb') as f:
    dataset = pickle.load(f)

#2. Разделяем DataSet на "Х-фичи" и "Y-прогноз"
Y = dataset['Rezultat_Sdelki_Retro_Binar_Plas_Minus']
X = dataset.drop(['Rezultat_Sdelki_Retro_Binar_Plas_Minus', '<Rezultat_Sdelki_Retrospektiva>'], 1)

print('DataSet для ML модели:\n', X.columns)
'''
DataSet для ML модели:
'<OPEN>', '<HIGH>', '<LOW>', '<CLOSE>', '<VOL>', 'PX_OPEN',
'momentum_period_5m', 'momentum_period_15m', 'momentum_period_1h',
'momentum_period_6h', 'momentum_period_12h',
'rsi_period_15m', 'rsi_period_1h', 'rsi_period_6h', 'rsi_period_12h',
'ema_period_15m', 'ema_period_1h', 'ema_period_6h', 'ema_period_12h',
'macd_12_26_9', 'macdsignal_12_26_9', 'macdhist_12_26_9'

'Rezultat_Sdelki_Retro_Binar_Plas_Minus',
'''

X = X.iloc[:, 0:-1]
X_train, X_test, Y_train, Y_test = sk.model_selection.train_test_split(X, Y, train_size=0.5, random_state=0)

'''
#График 1 - плотность рассеивания обучающей выборки
plt.plot(Y_train, 'o', label="Y_train")
plt.plot(Y_test, 'v', label="Y_test")
plt.legend(ncol=2, loc=(0, 1.05))
plt.ylabel("Y_train")
plt.xlabel("Y_test")
plt.show()
'''

#3. Запускаем обучение ML-модели

#линейная регрессия - классификация
lg = LogisticRegression(penalty='l2', solver='lbfgs')
lg.fit(X_train, Y_train)

print('\n', 'LogisticRegression')
print("Правильность на обучающем наборе: {:.2f}".format(lg.score(X_train, Y_train)))
print("Правильность на тестовом наборе: {:.2f}".format(lg.score(X_test, Y_test)))

#МЕТРИКИ (ЛОГ.РЕГРЕССИЯ)
#scoring = 'accuracy' - точность
#scoring = 'neg_log_loss' - логарифмическая потеря
#scoring = 'roc_auc' - площадь под кривой ROC
# - матрица путаницы
#classification_report - классификационный отчет

#МЕТРИКИ (ЛИНЕЙНАЯ РЕГРЕССИЯ)
#scoring = 'neg_mean_absolute_error' - cредняя абсолютная ошибка
#scoring = 'neg_mean_squared_error' - cредняя квадратическая ошибка (или MSE)
#scoring = 'r2' - метрика R ^ 2 (или R Squared)


#4. выгрузка параметров модели в файл
joblib.dump(lg, '../../data/interim/6.3_ML_Lg_Binare_Class_Signal_01.txt')

#4.1 загрузка готовых параметров модели из файла
#используемся в случае долгого обучения модели
#lg = joblib.load('../../data/interim/6.3_ML_Lg_Binare_Class_Signal_01.txt')


########################################

# .score() принимает в качестве аргументов предсказатель x и регрессор y, и возвращает значение
# .intercept_это скаляр
# .coef_- массив весов
# .predict() - предсказание ответа

print('lg.coef_- матрица значимых признаков и их параметров: \n', lg.coef_)

'''
#График 2 - выделение значимых признаков
fruits = X.columns
counts = lg.coef_
plt.plot(fruits, counts[0], '^', label="Значимость признаков в ML")
plt.xticks(rotation=90)
plt.legend()
plt.show()
'''

#5. Запускаем прогноз на всей выборке для оценки результатов
dataset['Log_Regression_Predict'] = lg.predict(X)

predicted = lg.predict(X)
report = classification_report(Y, predicted)
print(report)

#6. Вероятность прогноза
Veroatnost_Priznaka = lg.predict_proba(X)
dataset['Veroatnost_Priznaka'] = Veroatnost_Priznaka[:, 0]

#7. Вывод результа прогноза
Data_Analysis = dataset[['PX_OPEN', '<Rezultat_Sdelki_Retrospektiva>', 'Rezultat_Sdelki_Retro_Binar_Plas_Minus',
                         'Log_Regression_Predict', 'Veroatnost_Priznaka']]
print(Data_Analysis)

#8. Экспорт данных в файл
with open('../../data/interim/7_Analysis_of_simulation_results_01.pickle', 'wb') as f:
    pickle.dump(Data_Analysis, f)

Data_Analysis.to_csv('../../data/interim/7_Analysis_of_simulation_results_01.csv', encoding='utf-8', sep=',')  # index=None,
#Data_Analysis.to_excel('../../data/interim/7_Analysis_of_simulation_results_01.xlsx', startrow=3, index=True)

###########################
print("Write File - Ok")
t2 = datetime.now(tz=None)
print("Время работы алгоритма:", t2 - t1)