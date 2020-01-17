import pickle
import pandas as pd
import time
from tqdm import tqdm
from datetime import datetime


'''
в файл 3_Train_Massiv_1.txt добаляется маркет-дата из папок Raw с маркет-датой.
+ выравнивается дата по часовым поясам
на выходе файл "3_Train_Massiv_2" который используется в скриптах для генерации обучающей выборки
'''


with open('../../data/interim/3_Train_Massiv_1.pickle', 'rb') as f:
    trade_cube = pickle.load(f)


with open('../../data/interim/3_Train_Massiv_2.pickle', 'wb') as f:
    pickle.dump(trade_cube, f)

print('File Write')