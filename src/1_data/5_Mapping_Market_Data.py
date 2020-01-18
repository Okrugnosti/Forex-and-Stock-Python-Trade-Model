import pickle
import pandas as pd
import time
from tqdm import tqdm
from datetime import datetime
import matplotlib.pyplot as plt  # Построение графиков


'''
в файл 3_Train_Massiv_1.txt добаляется маркет-дата из папок Raw с маркет-датой.
+ выравнивается дата по часовым поясам
на выходе файл "3_Train_Massiv_2" который используется в скриптах для генерации обучающей выборки

дополнительно проводится разметка выборку для Бинарной классификации
'''

with open('../../data/interim/3_Train_Massiv_1.pickle', 'rb') as f:
    trade_cube = pickle.load(f)

trade_cube['Rezultat_Sdelki_Retro_Binar_Plas_Minus'] = trade_cube['<Rezultat_Sdelki_Retrospektiva>']\
    .apply(lambda x: 'Plus' if x >= 0 else 'Minus')

print(trade_cube['Rezultat_Sdelki_Retro_Binar_Plas_Minus'])
print(trade_cube.columns)

with open('../../data/interim/3_Train_Massiv_2.pickle', 'wb') as f:
    pickle.dump(trade_cube, f)

print('File Write')