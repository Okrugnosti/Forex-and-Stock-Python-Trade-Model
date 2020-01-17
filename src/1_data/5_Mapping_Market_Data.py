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

trade_cube = pd.read_csv('../../data/interim/2_Resalt_Trade_Model_1.txt', sep=',')
trade_cube = trade_cube.set_index('D')


with open('../../data/interim/2_Resalt_Trade_Model_1.pickle', 'wb') as f:
    pickle.dump(trade_cube, f)