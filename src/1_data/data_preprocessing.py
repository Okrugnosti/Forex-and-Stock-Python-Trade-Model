import os  # Системные функции
import pandas as pd  # DataFrame
import numpy as np  # Матрицы

def data_frame_inicialization(data_cube, directory, sep2):
    
    '''
    ОПИСАНИЕ ФУНКЦИИ:
    Извлекаем файлы_исходники с рыночными данными из папки. Объединяем файлы в единый DataFrame

    Входные параметры:
    directory = 'D://TradeSystem/DownLoadData/SPFB.RTS/'  - директория в которой лежат файлы с данными
    data_cube                                             -
    sep2                                                  - разделитель
    '''
    
    #создаем массив имен файлов
    files = os.listdir(directory)

    #считываем данные в DataFrame
    for i in range(len(files)):
        path = directory + files[i]
        print(path)
        dataset = pd.read_csv(path, sep=sep2)
        frames = [data_cube, dataset]
        #склеиваем датасети в один DataFrame
        data_cube = pd.concat(frames)

        print(data_cube)

    return data_cube


def data_convertor(data_cube, data_format):
    '''
    # функция преобразования даты и времени из строки в формат datetime
    # входные параметры:
    # data_format                                                 - задает формат конвертации '%Y%m%d%H%M%S'
    '''
    data_cube['D'] = pd.to_datetime(data_cube['D'], format=data_format)
    print(data_cube['D'].dtypes)
    #    data_cube['D'] = datetime.now(tz=None)

    #   ii = data_cube['<DATE>'].count()

    #    for i in range(0, ii):
    #        time_ymd = str(data_cube['<DATE>'].iloc[i])
    #        time_hms = str(data_cube['<TIME>'].iloc[i])
    #        time_ymd_hms = datetime.strptime(time_ymd + time_hms, format)
    #        data_cube['D'].iloc[i] = time_ymd_hms
    #        print(i)

    # устанавливаем дату в качестве индекса
    data_cube = data_cube.set_index('D')
    print(data_cube)

    return data_cube


def discretizacia_data(time_convert, data_cube):
    #конвертация времени в 5-ти минутные таймфреймы

    convert_market_data = data_cube.resample(time_convert) \
        .agg({'<OPEN>': 'first',
              '<HIGH>': np.max, '<LOW>': np.min,
              '<CLOSE>': 'last',
              '<VOL>': np.sum})

    # удаление пустых строчек

    return convert_market_data.dropna()


def data_frame_write(data_cube, directory):
    # функция записи DataFrame в файл
    # directory = 'D://TradeSystem/DownLoadData/SPFB.RTS/'  - директория в которой лежат файлы с данными

    data_cube.to_csv(directory, encoding='utf-8', sep=',')  # index=None,

    return print("Write File Ok")