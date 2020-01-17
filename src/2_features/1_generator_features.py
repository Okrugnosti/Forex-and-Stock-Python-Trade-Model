import talib
import pickle

with open('../../data/interim/3_Train_Massiv_2.pickle', 'rb') as f:
    df_trade_cube = pickle.load(f)

close = df_trade_cube['<CLOSE>']
# Momentum
df_trade_cube['momentum_period_5m'] = talib.MOM(close, timeperiod=1)
df_trade_cube['momentum_period_15m'] = talib.MOM(close, timeperiod=15 / 5)
df_trade_cube['momentum_period_1h'] = talib.MOM(close, timeperiod=60 / 5)
df_trade_cube['momentum_period_6h'] = talib.MOM(close, timeperiod=60 * 6 / 5)
df_trade_cube['momentum_period_12h'] = talib.MOM(close, timeperiod=60 * 12 / 5)

# RSI
df_trade_cube['rsi_period_15m'] = talib.RSI(close, timeperiod=15 / 5)
df_trade_cube['rsi_period_1h'] = talib.RSI(close, timeperiod=60 / 5)
df_trade_cube['rsi_period_6h'] = talib.RSI(close, timeperiod=60 * 6 / 5)
df_trade_cube['rsi_period_12h'] = talib.RSI(close, timeperiod=60 * 12 / 5)


talib.RSI(close,)

# # ROC
# df_trade_cube['roc_period_5m'] = talib.ROC(close, timeperiod=1)
# df_trade_cube['roc_period_15m'] = talib.ROC(close, timeperiod=15 / 5)
# df_trade_cube['roc_period_1h'] = talib.ROC(close, timeperiod=60 / 5)
# df_trade_cube['roc_period_6h'] = talib.ROC(close, timeperiod=60 * 6 / 5)
# df_trade_cube['roc_period_12h'] = talib.ROC(close, timeperiod=60 * 12 / 5)

# EMA
df_trade_cube['ema_period_15m'] = talib.EMA(close, timeperiod=15 / 5)
df_trade_cube['ema_period_1h'] = talib.EMA(close, timeperiod=60 / 5)
df_trade_cube['ema_period_6h'] = talib.EMA(close, timeperiod=60 * 6 / 5)
df_trade_cube['ema_period_12h'] = talib.EMA(close, timeperiod=60 * 12 / 5)

# MACD
macd, macdsignal, macdhist = talib.MACD(close, fastperiod=12, slowperiod=26, signalperiod=9)

df_trade_cube['macd_12_26_9'] = macd
df_trade_cube['macdsignal_12_26_9'] = macdsignal
df_trade_cube['macdhist_12_26_9'] = macdhist


'''
для тренировки модели, подготовить столбец со значениями:
если результат сделки >0 then 'plus' else 'minus'
'''

with open('../../data/interim/5_Learning_set.pickle', 'wb') as f:
    pickle.dump(df_trade_cube, f)

