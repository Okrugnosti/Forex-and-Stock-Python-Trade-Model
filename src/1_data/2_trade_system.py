import time
from tqdm import tqdm
from datetime import datetime  # Класс по работе с датой
import pandas as pd  # DataFrame
import data_preprocessing

'''
Модуль 2. Торговый модуль. Моделируем торговые сигналы на исторической выборке
'''

# временная метка для отсечки скорости алгоритма
t1 = datetime.now(tz=None)


#считываем данные из подготовленного файле, устанавливаем дату в качестве индекса
trade_cube = pd.read_csv('D://GitHub/Forex-and-Stock-Python-Trade-Model/data/interim/1_Trade_Cube_1.txt', sep=',')
trade_cube['D'] = pd.to_datetime(trade_cube['D'], format='%Y-%m-%d %H:%M:%S')
trade_cube = trade_cube.set_index('D')

# Инициализация переменных участвующих в торговой модели

Visota_Kanala = 850  # устанавливаем высоту канала (пункты)
Nachalo_Otsheta = 50  # устанавливаем начальную точку отсчета (пункты)
H_Stop = 2  # высота стопа (в кубиках)
Filter_Stop = 0  # ценовой фильтр между стопами (в кубиках) - позволяет перейти к модели с торговлей по CASH
Uroven = (round(trade_cube['<CLOSE>'].iloc[0] / Visota_Kanala, 0) * Visota_Kanala) + Nachalo_Otsheta  # сетка

Stop_Bay_1 = 0  # стоп заявка на закрытие позии (выход из шортов в кеш)
Stop_Bay_2 = Uroven + Visota_Kanala  # стоп заявка на открытие позции (выход из кеша в лонг)
Stop_Sell_1 = 0  # cтоп заявка на закрытие позции (выход из лонгов в кеш)
Stop_Sell_2 = Uroven - Visota_Kanala  # стоп заявка на открытие позиции (выход из кеша в шорты)

Volume_Position = 0  # размер текущей торговой позиции (в контрактах)
Plan_Volume_Position = 1  # размер плановой торговой позиции (в контрактах)
Buy = 1  # направлении торговой позиции (покупки)
Sell = -1  # направление торговой позиции (продажи)
Cash = 0  # направлении торговой позции (в деньгах)
Vector_Position = Cash  # текущее направлении торговой позиции (Bay/Sell/Cash)
PriceClose = trade_cube['<CLOSE>'].iloc[0]  # значение цены последней сдеки (в пунктах)

Open_Position = 0  # цена открытия позиции
Close_Position = 0  # уровень закрытия позиции

Shag_Cen = 0  # шаг движения цены (в кубиках), когда цена закрытия переходит на новый уровень

Sum_Sdelok = 0  # счетчик количества сделок
Local_Profit = 0  # переменная учитывающая сумму прибыль/убытка от проведенной операции
Max_Profit = 0  # рассчет планки прироста капитала
Local_Drogdawn = 0  # Рассчет локальной просадки
Local_Drogdawn = 0  # Рассчет локальной просадки
Max_Drogdawn = 0  # расчет максимальной просадки
Doli_Drogdawn = 0  # доля текущей просадки от максимальной просадки
Rezultat_Sdelki = 0  # результат сделки
Rezultat_Sdelki_Cub = 0  # результат сделки
Rezultat_Sdelki_Rub = 0

Stoimosti_Shaga_Cen = 0  # стоимость шага цены сделки

Rezultat_Sdelki_Summ = 0  # результат сделки
Rezultat_Sdelki_Cub_Summ = 0
Rezultat_Sdelki_Rub_Summ = 0

#    trade_cube['<HIGH>'].iloc[i]   - max
#    trade_cube['<LOW>'].iloc[i]    - min
#    trade_cube['<CLOSE>'].iloc[i]  - close
#    trade_cube['<PX_OPEN>'].iloc[i]  - курс доллара

# инициализация массива статистики
trade_cube['<Sum_Sdelok>'] = Sum_Sdelok
trade_cube['<Open_Position>'] = Open_Position
trade_cube['<Volume_Position>'] = Volume_Position
trade_cube['<Vector_Position>'] = Vector_Position

trade_cube['<Stop_Bay_1>'] = Stop_Bay_1
trade_cube['<Stop_Bay_2>'] = Stop_Bay_2
trade_cube['<Stop_Sell_1>'] = Stop_Sell_1
trade_cube['<Stop_Sell_2>'] = Stop_Sell_2

trade_cube['<Close_Position>'] = Close_Position
trade_cube['<Rezultat_Sdelki>'] = Rezultat_Sdelki
trade_cube['<Rezultat_Sdelki_Cub>'] = Rezultat_Sdelki_Cub
trade_cube['<Rezultat_Sdelki_Rub>'] = Rezultat_Sdelki_Rub

trade_cube['<Rezultat_Sdelki_Summ>'] = Rezultat_Sdelki_Summ
trade_cube['<Rezultat_Sdelki_Cub_Summ>'] = Rezultat_Sdelki_Cub_Summ
trade_cube['<Rezultat_Sdelki_Rub_Summ>'] = Rezultat_Sdelki_Rub_Summ

# Начинаем моделирование торговых сигналов на исторических данных
viborka = trade_cube['<CLOSE>'].count()


for i in tqdm(range(0, viborka)):

    PriceClose = trade_cube['<CLOSE>'].iloc[i]
    PriceHigh = trade_cube['<HIGH>'].iloc[i]
    PricaLow = trade_cube['<LOW>'].iloc[i]

    ##iF.1 CASH
    if (Vector_Position == Cash):  # если торговая позиция "в кеше" тогда проверяем исполнение стопов на вход в позицию
        # если мы в кеше, то это значит что стопов на вЫход стоять не должно
        # должны стоять стопы только на вход
        if (PriceHigh >= Stop_Bay_2):  # проверка на пересечение стоп цены на вход в лонги
            # если максимум свечи пересек цену стопа
            Vector_Position = Buy  # направлеи позиции ставим на лонг
            Volume_Position = Plan_Volume_Position  # размер позиции устанавливаем на размер плановой открытой позиции
            Open_Position = Stop_Bay_2  # устанавливаем цену открытия позиции
            Stop_Sell_1 = Open_Position - (
                        H_Stop * Visota_Kanala)  # устанавливаем стоп заявку на выход в Кешь из лонгов
            Stop_Sell_2 = Open_Position - (
                        (H_Stop + Filter_Stop) * Visota_Kanala)  # устанавливае стоп заявку на вход в Шорты

            Sum_Sdelok = 1  # если мы сменили позицию, значит произошла сделка

            Stop_Bay_1 = 0  # сбрасываем он нам не нужен
            Stop_Bay_2 = 0  # сбрасываем он нам не нужен


            '''
            # **** запись статистики в массив ****
            trade_cube['<Sum_Sdelok>'].iloc[i] = Sum_Sdelok
            trade_cube['<Open_Position>'].iloc[i] = Open_Position
            trade_cube['<Volume_Position>'].iloc[i] = Volume_Position
            trade_cube['<Vector_Position>'].iloc[i] = Vector_Position

            trade_cube['<Stop_Bay_1>'].iloc[i] = Stop_Bay_1
            trade_cube['<Stop_Bay_2>'].iloc[i] = Stop_Bay_2
            trade_cube['<Stop_Sell_1>'].iloc[i] = Stop_Sell_1
            trade_cube['<Stop_Sell_2>'].iloc[i] = Stop_Sell_2
            '''

        else:

            if (PricaLow <= Stop_Sell_2):  # идет проверка на пересечение стоп цены на вход в шорт
                # если минимум свечи пересек цену стопа
                Vector_Position = Sell  # направлеи позиции ставим на лонг
                Volume_Position = Plan_Volume_Position  # размер позиции устанавливаем на размер плановой открытой позиции
                Open_Position = Stop_Sell_2  # устанавливаем цену открытия позиции на уровне стопа
                Stop_Bay_1 = Open_Position + (
                            H_Stop * Visota_Kanala)  # устанавливаем стоп заявку на выход в Кешь из Шортов
                Stop_Bay_2 = Open_Position + (
                            (H_Stop + Filter_Stop) * Visota_Kanala)  # устанавливае стоп заявку на вход в Лонги

                Sum_Sdelok = 1  # если мы сменили позицию, значит произошла сделка

                Stop_Sell_1 = 0  # сбрасываем он нам не нужен
                Stop_Sell_2 = 0  # сбрасываем он нам не нужен

                '''
                # **** запись статистики в массив ****
                trade_cube['<Sum_Sdelok>'].iloc[i] = Sum_Sdelok
                trade_cube['<Open_Position>'].iloc[i] = Open_Position
                trade_cube['<Volume_Position>'].iloc[i] = Volume_Position
                trade_cube['<Vector_Position>'].iloc[i] = Vector_Position

                trade_cube['<Stop_Bay_1>'].iloc[i] = Stop_Bay_1
                trade_cube['<Stop_Bay_2>'].iloc[i] = Stop_Bay_2
                trade_cube['<Stop_Sell_1>'].iloc[i] = Stop_Sell_1
                trade_cube['<Stop_Sell_2>'].iloc[i] = Stop_Sell_2
                '''

    ##iF.1 закончили проверку находясь CASH

    ##eLSE проверки в позициях
    else:

        # если мы находимся в BUY
        if (Vector_Position == Buy):

            # проверка на пересечения стопа 1-го уровня
            if (PricaLow <= Stop_Sell_1):  # идет проверка на пересечение стоп цены на выход в кешь
                # если минимум свечи пересек цену стопа
                Vector_Position = Cash  # направлеи позиции ставим на Cash
                Volume_Position = 0  # размер позиции устанавливаем на размер плановой открытой позиции
                Close_Position = Stop_Sell_1  # устанавливаем цену закрытия позиции
                Stop_Sell_2 = Close_Position - (Filter_Stop * Visota_Kanala)  # Корректируем стоп заявку на вход в шорт
                Stop_Bay_2 = Close_Position + (
                            H_Stop * Visota_Kanala)  # Устанавливаем стоп заявку на обратный вход в лонг

                # Sum_Sdelok +=1                                  #если мы сменили позицию, значит произошла сделка
                Rezultat_Sdelki = (Close_Position - Open_Position)  # - Parametr.Proskalzivania
                Rezultat_Sdelki_Cub = Rezultat_Sdelki / Visota_Kanala
                Rezultat_Sdelki_Rub = Rezultat_Sdelki / 100 * trade_cube['PX_OPEN'].iloc[i - 1]
                Rezultat_Sdelki_Summ = Rezultat_Sdelki_Summ + Rezultat_Sdelki
                Rezultat_Sdelki_Cub_Summ = Rezultat_Sdelki_Cub_Summ + Rezultat_Sdelki_Cub
                Rezultat_Sdelki_Rub_Summ = Rezultat_Sdelki_Rub_Summ + Rezultat_Sdelki_Rub


                Stop_Sell_1 = 0  # сбрасываем он нам не нужен
                Stop_Bay_1 = 0  # cбрасываем он нам не нужен

                Open_Position = 0  # сбрасываем он нам не нужен
                # закончили проверку на пересечения стопа 1-го уровня


                # **** запись статистики в массив ****
                trade_cube['<Sum_Sdelok>'].iloc[i-1] = Sum_Sdelok
                trade_cube['<Close_Position>'].iloc[i-1] = Close_Position
                trade_cube['<Rezultat_Sdelki>'].iloc[i-1] = Rezultat_Sdelki
                trade_cube['<Rezultat_Sdelki_Cub>'].iloc[i-1] = Rezultat_Sdelki_Cub
                trade_cube['<Rezultat_Sdelki_Rub>'].iloc[i - 1] = Rezultat_Sdelki_Rub


                # trade_cube = Metod_Analiz_Dohod_Sistem(trade_cube, i)

                # проверка на пересечения стопа 2-го уровня
                if (PricaLow <= Stop_Sell_2):  # идет проверка на пересечение стоп цены на вход в шорт
                    # если минимум свечи пересек цену стопа

                    Vector_Position = Sell  # направлеи позиции ставим на Шорт
                    Volume_Position = Plan_Volume_Position  # размер позиции устанавливаем на размер плановой открытой позиции
                    Open_Position = Stop_Sell_2  # устанавливаем цену открытия позиции
                    Stop_Bay_1 = Open_Position + (
                                H_Stop * Visota_Kanala)  # Устанавливаем уровень стопа на выход в кешь из шортов
                    Stop_Bay_2 = Stop_Bay_1 + (
                                Filter_Stop * Visota_Kanala)  # Устанавливаем уровень стопа на выход в лонг из кеша

                    Sum_Sdelok = 1  # если мы сменили позицию, значит произошла сделка

                    Stop_Sell_1 = 0  # сбрасываем он нам не нужен
                    Stop_Sell_2 = 0  # сбрасываем он нам не нужен
                    # закончили проверку на пересечения стопа 2-го уровня


                    '''
                    # **** запись статистики в массив ****
                    trade_cube['<Sum_Sdelok>'].iloc[i] = Sum_Sdelok
                    trade_cube['<Open_Position>'].iloc[i] = Open_Position
                    trade_cube['<Volume_Position>'].iloc[i] = Volume_Position
                    trade_cube['<Vector_Position>'].iloc[i] = Vector_Position

                    trade_cube['<Stop_Bay_1>'].iloc[i] = Stop_Bay_1
                    trade_cube['<Stop_Bay_2>'].iloc[i] = Stop_Bay_2
                    trade_cube['<Stop_Sell_1>'].iloc[i] = Stop_Sell_1
                    trade_cube['<Stop_Sell_2>'].iloc[i] = Stop_Sell_2
                    '''


            # подтягиваем стопы, если цена закрытия выросла на 1 и более кубов
            else:
                if ((((PriceClose - Stop_Sell_1) - (
                        H_Stop * Visota_Kanala)) / Visota_Kanala) >= 1):  # & Chislo_out[7] == 1):
                    # рассчитываем кол-во кубов и подтягиваем стопы
                    Shag_Cen = round(((PriceClose - Stop_Sell_1) - (H_Stop * Visota_Kanala)) / Visota_Kanala, 0)
                    Stop_Sell_1 = Stop_Sell_1 + (
                                Shag_Cen * Visota_Kanala)  # передвигаем стоп на выход в кешь из лонгов на шаг цены в кубиках
                    Stop_Sell_2 = Stop_Sell_1 - (
                                Filter_Stop * Visota_Kanala)  # передвигаем стоп на вход в шорт из кеша на шаг цены в кубиках

                    '''
                    # **** запись статистики в массив ****
                    trade_cube['<Stop_Sell_1>'].iloc[i] = Stop_Sell_1
                    trade_cube['<Stop_Sell_2>'].iloc[i] = Stop_Sell_2
                    '''


        # если мы находимся в SELL
        else:

            if (Vector_Position == Sell):  # если мы находимся в продажах

                # проверка на пересечения стопа 1-го уровня
                if (PriceHigh >= Stop_Bay_1):  # идет проверка на пересечение стоп цены на выход в кешь
                    # если максимум свечи пересек цену стопа

                    Vector_Position = Cash  # направлеи позиции ставим на Cash
                    Volume_Position = 0  # размер позиции устанавливаем на размер плановой открытой позиции
                    Close_Position = Stop_Bay_1  # устанавливаем цену закрытия позиции
                    Stop_Bay_2 = Close_Position + (Filter_Stop * Visota_Kanala)  # корректируем стопа для входа в лонги
                    Stop_Sell_2 = Close_Position - (
                                H_Stop * Visota_Kanala)  # Устанавливаем уровень стопа на обратный вход в шорт из кеша

                    # Sum_Sdelok +=1                                      #если мы сменили позицию, значит произошла сделка
                    Rezultat_Sdelki = (Open_Position - Close_Position)  # - Parametr.Proskalzivania результат сделки
                    Rezultat_Sdelki_Cub = Rezultat_Sdelki / Visota_Kanala
                    Rezultat_Sdelki_Rub = Rezultat_Sdelki / 100 * trade_cube['PX_OPEN'].iloc[i - 1]
                    Rezultat_Sdelki_Summ = Rezultat_Sdelki_Summ + Rezultat_Sdelki
                    Rezultat_Sdelki_Cub_Summ = Rezultat_Sdelki_Cub_Summ + Rezultat_Sdelki_Cub
                    Rezultat_Sdelki_Rub_Summ = Rezultat_Sdelki_Rub_Summ + Rezultat_Sdelki_Rub

                    Stop_Bay_1 = 0  # сбрасываем он нам не нужен
                    Stop_Sell_1 = 0  # сбрасываем он нам не нужен

                    Open_Position = 0  # сбрасываем он нам не нужен
                    # закончили проверку стопа на пересечения стопа 1-го уровня

                    # **** запись статистики в массив ****
                    trade_cube['<Sum_Sdelok>'].iloc[i-1] = Sum_Sdelok
                    trade_cube['<Close_Position>'].iloc[i-1] = Close_Position
                    trade_cube['<Rezultat_Sdelki>'].iloc[i-1] = Rezultat_Sdelki
                    trade_cube['<Rezultat_Sdelki_Cub>'].iloc[i-1] = Rezultat_Sdelki_Cub
                    trade_cube['<Rezultat_Sdelki_Rub>'].iloc[i - 1] = Rezultat_Sdelki_Rub


                    # trade_cube = Metod_Analiz_Dohod_Sistem(trade_cube,i)

                    # проверка на пересечения стопа 2-го уровня
                    if (PriceHigh >= Stop_Bay_2):  # идет проверка на пересечение стоп цены на вход в лонг
                        # если максимум свечи пересек цену стопа

                        Vector_Position = Buy  # направлеи позиции ставим на Шорт
                        Volume_Position = Plan_Volume_Position  # размер позиции устанавливаем на размер плановой открытой позиции
                        Open_Position = Stop_Bay_2  # устанавливаем цену открытия позиции
                        Stop_Sell_1 = Open_Position - (
                                    H_Stop * Visota_Kanala)  # Устанавливаем уровень стопа на выход в кешь из лонгов
                        Stop_Sell_2 = Stop_Sell_1 - (
                                    Filter_Stop * Visota_Kanala)  # Устанавливаем уровень стопа на вход в шорт из кеша

                        Sum_Sdelok = 1  # если мы сменили позицию, значит произошла сделка

                        Stop_Bay_1 = 0  # сбрасываем он нам не нужен
                        Stop_Bay_2 = 0  # сбрасываем он нам не нужен
                        # закончили проверку на пересечения стопа 2-го уровня

                        '''
                        # **** запись статистики в массив ****
                        trade_cube['<Sum_Sdelok>'].iloc[i] = Sum_Sdelok
                        trade_cube['<Open_Position>'].iloc[i] = Open_Position
                        trade_cube['<Volume_Position>'].iloc[i] = Volume_Position
                        trade_cube['<Vector_Position>'].iloc[i] = Vector_Position

                        trade_cube['<Stop_Bay_1>'].iloc[i] = Stop_Bay_1
                        trade_cube['<Stop_Bay_2>'].iloc[i] = Stop_Bay_2
                        trade_cube['<Stop_Sell_1>'].iloc[i] = Stop_Sell_1
                        trade_cube['<Stop_Sell_2>'].iloc[i] = Stop_Sell_2
                        '''


                # подтягиваем стопы, если цена закрытия выросла на 1 и более кубов
                else:
                    if ((((Stop_Bay_1 - PriceClose) - (
                            H_Stop * Visota_Kanala)) / Visota_Kanala) >= 1):  # & Chislo_out[7] == 1):
                        # рассчитываем кол-во кубов и подтягиваем стопы
                        Shag_Cen = round(((Stop_Bay_1 - PriceClose) - (H_Stop * Visota_Kanala)) / Visota_Kanala, 0)
                        Stop_Bay_1 = Stop_Bay_1 - (
                                    Shag_Cen * Visota_Kanala)  # передвигаем стоп на выход в кешь из шортов на шаг цены в кубиках
                        Stop_Bay_2 = Stop_Bay_1 + (
                                    Filter_Stop * Visota_Kanala)  # передвигаем стоп на вход в лонг из кеша на шаг цены в кубиках

                        '''
                        # **** запись статистики в массив ****
                        trade_cube['<Stop_Bay_1>'].iloc[i] = Stop_Bay_1
                        trade_cube['<Stop_Bay_2>'].iloc[i] = Stop_Bay_2
                        '''
    # **** запись статистики в массив ****
    #trade_cube['<Sdeloka>'].iloc[i] = Sum_Sdelok
    trade_cube['<Open_Position>'].iloc[i] = Open_Position
    trade_cube['<Volume_Position>'].iloc[i] = Volume_Position
    trade_cube['<Vector_Position>'].iloc[i] = Vector_Position

    trade_cube['<Stop_Bay_1>'].iloc[i] = Stop_Bay_1
    trade_cube['<Stop_Bay_2>'].iloc[i] = Stop_Bay_2
    trade_cube['<Stop_Sell_1>'].iloc[i] = Stop_Sell_1
    trade_cube['<Stop_Sell_2>'].iloc[i] = Stop_Sell_2

    #trade_cube['<Close_Position>'].iloc[i] = Close_Position
    #trade_cube['<Rezultat_Sdelki>'].iloc[i] = Rezultat_Sdelki
    #trade_cube['<Rezultat_Sdelki_Cub'].iloc[i] = Rezultat_Sdelki_Cub
    trade_cube['<Rezultat_Sdelki_Summ>'].iloc[i] = Rezultat_Sdelki_Summ
    trade_cube['<Rezultat_Sdelki_Cub_Summ>'].iloc[i] = Rezultat_Sdelki_Cub_Summ
    trade_cube['<Rezultat_Sdelki_Rub_Summ>'].iloc[i] = Rezultat_Sdelki_Rub_Summ

# запись результатов в файл
data_preprocessing.data_frame_write(trade_cube, 'D://GitHub/Forex-and-Stock-Python-Trade-Model/data/interim/2_Resalt_Trade_Model_1.txt')
#trade_cube.to_excel('D://GitHub/Forex-and-Stock-Python-Trade-Model/data/interim/2_Resalt_Trade_Model_1.xlsx', startrow=3, index=True)


print(trade_cube.info())
print(trade_cube['<Rezultat_Sdelki_Summ>'].describe())

t2 = datetime.now(tz=None)
print(t2 - t1)