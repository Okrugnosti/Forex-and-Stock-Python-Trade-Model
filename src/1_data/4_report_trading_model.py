import os  # Системные функции
from datetime import datetime  # Класс по работе с датой
import pytz  # База часовых поясов
import openpyxl as xl  # Excel
import numpy as np  # Матрицы
import pandas as pd  # DataFrame
import matplotlib.pyplot as plt  # Построение графиков
import seaborn as sns  # Построение графиков
#import plotly # Построение графиков https://proglib.io/p/plotly/
import sklearn as sk  # Машинное обучение
import scipy  # Выполнениt научных и инженерных расчётов
import data_preprocessing

def raschet_Max_Drogdawn(x, Max_Risc):
    Local_Profit = Max_Drogdawn = Local_Drogdawn = Max_Drogdawn = Max_Profit = 0

    for i in x.index:

        Local_Profit = Local_Profit + x['<Rezultat_Sdelki_Rub>'].loc[i]

        if (Max_Profit < Local_Profit):
            Max_Profit = Local_Profit  # Максимальная прибыль, руб

        Local_Drogdawn = Local_Profit - Max_Profit  # рассчет локальной просадки

        if (Max_Drogdawn > Local_Drogdawn):
            Max_Drogdawn = Local_Drogdawn  # Максимальная просадка, руб.

        #if (Max_Drogdawn != 0):  # расчет доли от максимальной просадки
        #    Doli_Drogdawn = Local_Drogdawn / Max_Drogdawn * 100

        x['<Local_Drogdawn>'].loc[i] = Local_Drogdawn

    x['<Start_Capital>'] = Max_Drogdawn / Max_Risc * -100  # Стартовый капиталл, руб
    x['<Local_Drogdawn>'] = x['<Local_Drogdawn>'] / Max_Drogdawn * 100

    return x


def report_trade_model(period_agrigacii, Max_Risc):
    #считываем данные из подготовленного файле, устанавливаем дату в качестве индекса
    trade_cube = pd.read_csv('D://GitHub/Saint_Perersburg_Trading_Model_a/data/interim/2_Resalt_Trade_Model_1.txt', sep=',')
    trade_cube['D'] = pd.to_datetime(trade_cube['D'], format='%Y-%m-%d %H:%M:%S')
    trade_cube = trade_cube.set_index('D')
    print(trade_cube.info())

        #отбор транзакций с результатами сделок
    o = trade_cube[(trade_cube['<Sum_Sdelok>'] == 1)][['<OPEN>', '<CLOSE>', '<Sum_Sdelok>', '<Rezultat_Sdelki_Cub>', '<Rezultat_Sdelki>', '<Rezultat_Sdelki_Rub>', '<Rezultat_Sdelki_Rub_Summ>']]
    o['<Local_Drogdawn>'] = 0
    o = raschet_Max_Drogdawn(o, Max_Risc)
    print(o.info())

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
    print(o)
    #print(o.info())

    #отбор транзакций с результатами убыточных сделок
    o2 = trade_cube[(trade_cube['<Sum_Sdelok>'] == 1)& (trade_cube['<Rezultat_Sdelki>'] <= 0)][['<Sum_Sdelok>', '<Rezultat_Sdelki>']]
    o2 = o2.groupby(pd.Grouper(freq=period_agrigacii)).sum()
    print(o2)

    #склеивание выборок
    pivot_table = pd.merge(o, o2, how = 'inner', left_index = True, right_index = True)
    #print(pivot_table)

    #расчет дополнительных аналитик
    pivot_table['<Benchmark>'] = (pivot_table['<CLOSE>'] / pivot_table['<OPEN>'] - 1) * 100
    pivot_table['<% losing trades>'] = pivot_table['<Sum_Sdelok>_y'] / pivot_table['<Sum_Sdelok>_x'] * 100
    pivot_table['<mean_loss_trades_punkt>'] = pivot_table['<Rezultat_Sdelki>_y'] / pivot_table['<Sum_Sdelok>_y']
    pivot_table['<mean_trades_punkt>'] = pivot_table['<Rezultat_Sdelki>_x'] / pivot_table['<Sum_Sdelok>_x']
    pivot_table['<Profitability>'] = pivot_table['<Rezultat_Sdelki_Rub>'] / pivot_table['<Start_Capital>'] * 100
    pivot_table['<Profitability_Summ>'] = pivot_table['<Rezultat_Sdelki_Rub_Summ>'] / pivot_table['<Start_Capital>'] * 100

    print(pivot_table)
    print(pivot_table.info())

    #Переименование столбцов в сводной таблице
    pivot_table2 = pivot_table.rename({

        '<Benchmark>' : 'Benchmark (доходность рынка), %',

        '<Sum_Sdelok>_x' : 'Кол-во сделок, шт.',
        '<Sum_Sdelok>_y': 'Кол-во убыточных сделок, шт.',
        '<% losing trades>': 'Доля убыточных сделок, %',

        '<Rezultat_Sdelki>_y': 'Результат убыточных сделок, руб.',
        '<mean_loss_trades_punkt>': 'Средений убыток на сделку, пункты',

        '<Rezultat_Sdelki_Cub>' : 'Прибыль, куб.',
        '<Rezultat_Sdelki>_x' : 'Прибыль, пункты',
        '<Rezultat_Sdelki_Rub>' : 'Прибыль, у.е',
        '<Rezultat_Sdelki_Rub_Summ>' : 'Накопленная прибыль, у.е',

        '<Start_Capital>': 'Обеспечение, у.е./контракт',

        '<mean_trades_punkt>': 'Средняя прибыль на сделку, пункты',

        '<Profitability>' : 'Доходность, %',
        '<Profitability_Summ>' : 'Накопленная Доходность, %',

        '<Local_Drogdawn>' : 'Текущая просадка, % от DrogDown'

    }, axis=1)  # new method


    pivot_table2 = pivot_table2[['Benchmark (доходность рынка), %',
                                 'Кол-во сделок, шт.',
                                 'Доля убыточных сделок, %',
                                 'Прибыль, у.е',
                                 'Доходность, %',
                                 'Накопленная прибыль, у.е',
                                 'Накопленная Доходность, %',
                                 'Текущая просадка, % от DrogDown',
                                 'Средняя прибыль на сделку, пункты',
                                 'Обеспечение, у.е./контракт'
                                 ]]

    #вывод отчета в Excel
    pivot_table2.to_excel('D://GitHub/Saint_Perersburg_Trading_Model_a/data/interim/4_Traiding_Repotr_1.xlsx', startrow=3, index=True)
    print('File Write')

    # смотрим данным на графике
    #fig = plt.figure()
    #pivot_table['<Local_Drogdawn>'].plot()
    #pivot_table['<Profitability_Summ>'].plot()

    plt.show()

    return 0


period_agrigacii = 'Y'
Max_Risc = 50

report_trade_model(period_agrigacii, Max_Risc)


'''
print(o.groupby(o.index.month).agg(
    {'<Sum_Sdelok>' : np.max,
     '<Rezultat_Sdelki>' : np.sum}))


#print(trade_cube['<Sum_Sdelok>'].groupby([trade_cube.index.month, trade_cube['<Rezultat_Sdelki_Cub>']]).sum())

kol_sdelok = trade_cube[(trade_cube['<Sum_Sdelok>'] == 1)]['<Sum_Sdelok>']
kol_sdelok_losing = trade_cube[(trade_cube['<Sum_Sdelok>'] == 1)& (trade_cube['<Rezultat_Sdelki>'] <= 0)]['<Rezultat_Sdelki>']
kol_sdelok_profit = trade_cube[(trade_cube['<Sum_Sdelok>'] == 1)& (trade_cube['<Rezultat_Sdelki>'] > 0)]['<Rezultat_Sdelki>']

kol_sdelok_mes = kol_sdelok['<Sum_Sdelok>'].groupby(kol_sdelok.index.month).count()
kol_sdelok_losing_mes = kol_sdelok_losing.groupby(kol_sdelok_losing.index.month)

print(kol_sdelok_mes)

#kol_sdelok_mes.merge(kol_sdelok_mes, kol_sdelok_losing_mes, how = 'inner', left_index = True, right_index = True)

Rezultat_Sdelki_Cub_Summ = trade_cube['<Rezultat_Sdelki_Cub>'].groupby(trade_cube.index.month).sum()
Rezultat_Sdelki_Summ = trade_cube['<Rezultat_Sdelki>'].groupby(trade_cube.index.month).sum()
Rezultat_Sdelki_Rub_Summ = trade_cube['<Rezultat_Sdelki_Rub>'].groupby(trade_cube.index.month).sum()

#pivot_rez = pd.DataFrame(
#    {
#        'kol_sdelok' : kol_sdelok_mes,
#        'kol_sdelok_losing' : kol_sdelok_losing_mes
#    })

#print(pivot_rez)

#расчет сделок и доли сделок
o = trade_cube[(trade_cube['<Sum_Sdelok>'] == 1)][['<Sum_Sdelok>', '<Rezultat_Sdelki>']]
o2 = trade_cube[(trade_cube['<Sum_Sdelok>'] == 1)& (trade_cube['<Rezultat_Sdelki>'] <= 0)][['<Sum_Sdelok>', '<Rezultat_Sdelki>']]
a1 = o.groupby(o.index.month).count()
a2 = o2.groupby(o2.index.month).count()
o3 = pd.merge(a1, a2, how = 'inner', left_index = True, right_index = True)
o3['<Sum_Sdelok>_y'] = o3['<Sum_Sdelok>_y'].astype(float)
o3['<Sum_Sdelok>_x'] = o3['<Sum_Sdelok>_x'].astype(float)
o3['<% losing trades>'] = o3['<Sum_Sdelok>_y'] / o3['<Sum_Sdelok>_x']
print(o3[['<Sum_Sdelok>_x', '<% losing trades>']])
print(o3.info())

#pivot_table


#print(o.groupby('<Sum_Sdelok>').sum())

#o['<Sum_Sdelok>'].groupby(trade_cube.index.month).count()

#print(trade_cube[trade_cube[['<Sum_Sdelok>'] !=0) & (trade_cube['<Rezultat_Sdelki>'] > 0)]['<Sum_Sdelok>', '<Rezultat_Sdelki>'])

#print(trade_cube[(trade_cube['<Sum_Sdelok>'] !=0) & (trade_cube['<Rezultat_Sdelki>'] > 0)].set_index(['<Sum_Sdelok>','<Rezultat_Sdelki>'], drop = False))
#print(o.groupby(trade_cube.index.month).count())


print(trade_cube.set_index(['<Sum_Sdelok>','<Rezultat_Sdelki>']))

#print(trade_cube['<Sum_Sdelok>'],['<Rezultat_Sdelki>'])

kol_vo_sdelok = trade_cube['<Sum_Sdelok>'].groupby(trade_cube.index.month).sum()
print(kol_vo_sdelok)

kol_vo_sdelok_ubitok = trade_cube[(trade_cube['<Sum_Sdelok>'] !=0) & (trade_cube['<Rezultat_Sdelki>'] > 0)]['<Rezultat_Sdelki>']
o = kol_vo_sdelok_ubitok.groupby(kol_vo_sdelok_ubitok.index.month).count()
print(o)


# print(kol_vo_sdelok_ubitok.columns)
print(len(kol_vo_sdelok))
print(len(kol_vo_sdelok_ubitok))
#u = kol_vo_sdelok_ubitok  / kol_vo_sdelok


print(pd.concat([kol_vo_sdelok, kol_vo_sdelok_ubitok]))


df = pd.DataFrame({'state': ['CA', 'WA', 'CO', 'AZ'] * 3,
                   'office_id': list(range(1, 7)) * 2,
                   'sales': [np.random.randint(100000, 999999)
                             for _ in range(12)]})
state_office = df.groupby(['state', 'office_id']).agg({'sales': 'sum'})
# Change: groupby state_office and divide by sum
state_pcts = state_office.groupby(level=0).apply(lambda x:
                                                 100 * x / float(x.sum()))


print(state_office)

print(df)

print(state_pcts)



def f(x):
    return x['<CLOSE>'] - x['<Open_Position>']

o2 = trade_cube.apply(f, axis=1)


o3 = trade_cube.apply(lambda x:
                              x['<CLOSE>'] - x['<Open_Position>']
                              if x['<Vector_Position>'] == 1
                              else x['<Open_Position>'] - x['<CLOSE>'], axis=1)



#kol_vo_sdelok = trade_cube['<Sum_Sdelok>'].resample('1M', how = 'sum')
#print(trade_cube['<Sum_Sdelok>'].groupby([trade_cube.index.month, trade_cube['<Rezultat_Sdelki_Cub>']]).sum())


#print(trade_cube[trade_cube['<Sum_Sdelok>'] !=0]['<Sum_Sdelok>'].groupby(trade_cube.index.month))

      #.groupby(trade_cube.index.month)['<Rezultat_Sdelki>'].count())


#print(trade_cube[trade_cube['<Rezultat_Sdelki>'] >= 0]['<Sum_Sdelok>'].sum()) #.groupby(trade_cube.index.month)
#print(trade_cube['<Rezultat_Sdelki>'].groupby(trade_cube['<Rezultat_Sdelki>']).count())
#print(trade_cube[(trade_cube['<Sum_Sdelok>'] !=0) & (trade_cube['<Rezultat_Sdelki>'] <=0)].groupby(trade_cube['<Rezultat_Sdelki>'])['<Sum_Sdelok>'].count())
#print(trade_cube[(trade_cube['<Sum_Sdelok>'] !=0) & (trade_cube['<Rezultat_Sdelki>'] <=0)].groupby(trade_cube.index.month)['<Sum_Sdelok>'].count())
'''