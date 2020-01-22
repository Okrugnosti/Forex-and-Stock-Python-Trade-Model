import pickle

from backtesting import Backtest, Strategy

with open('../../data/interim/1_Trade_Cube_1.pickle', 'rb') as f:
    trade_cube = pickle.load(f)

trade_cube.columns = ['Open', 'High', 'Low', 'Close', 'Volume', 'Close_usd']

class SmaCross(Strategy):
    def init(self):
        self.vector_position = 0
        self.visota_kanala = 850  # устанавливаем высоту канала (пункты)
        self.nachalo_otsheta = 50  # устанавливаем начальную точку отсчета (пункты)
        self.h_stop = 2  # высота стопа (в кубиках)
        self.filter_stop = 0  # ценовой фильтр между стопами (в кубиках) - позволяет перейти к модели с торговлей по CASH
        self.uroven = (round(self.data.Close[0] / self.visota_kanala,
                             0) * self.visota_kanala) + self.nachalo_otsheta  # сетка

        self.stop_bay_2 = self.uroven + self.visota_kanala  # стоп заявка на открытие позции (выход из кеша в лонг)
        self.stop_sell_2 = self.uroven - self.visota_kanala  # стоп заявка на открытие позиции (выход из кеша в шорты)

    def next(self):
        PriceClose = self.data.Close[-1]
        PriceHigh = self.data.High[-1]
        PricaLow = self.data.Low[-1]
        if (PriceHigh >= self.stop_bay_2):  # проверка на пересечение стоп цены на вход в лонги
            # если максимум свечи пересек цену стопа
            self.stop_sell_2 = PriceClose - (
                    (self.h_stop + self.filter_stop) * self.visota_kanala)  # устанавливае стоп заявку на вход в Шорты
            stop_loss = PriceClose - self.h_stop * self.visota_kanala
            self.buy(sl=stop_loss)
            self.stop_bay_2 = 1e6

        elif (PricaLow <= self.stop_sell_2):  # идет проверка на пересечение стоп цены на вход в шорт
            # если минимум свечи пересек цену стопа
            self.stop_bay_2 = PriceClose + (
                    (
                            self.h_stop + self.filter_stop) * self.visota_kanala)  # устанавливае стоп заявку на вход в Шорты
            self.sell(sl=(PriceClose + self.h_stop * self.visota_kanala))
            self.stop_sell_2 = 0
        elif self.position.is_long:
            if ((((PriceClose - self.orders.sl) - (
                    self.h_stop * self.visota_kanala)) / self.visota_kanala) >= 1):  # & Chislo_out[7] == 1):
                # рассчитываем кол-во кубов и подтягиваем стопы
                shag_cen = round(
                    ((PriceClose - self.orders.sl) - (self.h_stop * self.visota_kanala)) / self.visota_kanala, 0)
                stop_sell_1 = self.orders.sl + (
                        shag_cen * self.visota_kanala)  # передвигаем стоп на выход в кешь из лонгов на шаг цены в кубиках
                self.orders.set_sl(stop_sell_1)
                self.stop_sell_2 = stop_sell_1 - (
                        self.filter_stop * self.visota_kanala)
        elif self.position.is_short:
            if ((((self.orders.sl - PriceClose) - (
                    self.h_stop * self.visota_kanala)) / self.visota_kanala) >= 1):  # & Chislo_out[7] == 1):
                # рассчитываем кол-во кубов и подтягиваем стопы
                shag_cen = round(
                    ((self.orders.sl - PriceClose) - (self.h_stop * self.visota_kanala)) / self.visota_kanala, 0)
                stop_bay_1 = self.orders.sl - (
                        shag_cen * self.visota_kanala)  # передвигаем стоп на выход в кешь из шортов на шаг цены в кубиках
                self.orders.set_sl(stop_bay_1)
                self.stop_bay_2 = stop_bay_1 + (
                        self.filter_stop * self.visota_kanala)  # передвигаем стоп на вход в лонг из кеша на шаг цены в кубиках


bt = Backtest(trade_cube, SmaCross,
              cash=120000, commission=0, margin=1, trade_on_close=True)
output = bt.run()

# вывод графиков
# bt.plot()
print(output)
