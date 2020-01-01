using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace R10
{
    // УПРАВЛЯЮЩАЯ БАЗА КОМАНД ДЛЯ QUIK

    
        
        public class ExPort_File_TRI // класс записи данных в файл
        {
            public string FilePath; // место записи пути к файлу
            public StreamWriter fstr_out;

            public StreamWriter ExPort_File_Metod(string file_tri, string comands) // открытие файла для записи
            {
                FilePath = file_tri; // адрес экспортируемых данных
                fstr_out = new StreamWriter(FilePath);
                fstr_out.WriteLine(comands); // (запись данных в буфер) записываем команду в tri файл
                fstr_out.Close(); // закрытие потока, вывод данных из буфера в файл

                return fstr_out;
            }
        }

        public class QUIK_COMMANDS // класс составных переменных для команд в QUIK
        {
            public string CLASSCODE = "CLASSCODE="; //Код класса, по которому выполняется транзакция, например EQBR. Обязательный параметр
            public string SECCODE = "SECCODE="; //Код инструмента, по которому выполняется транзакция, например SBER

            public string ACTION = "ACTION="; //Вид транзакции, имеющий одно из следующих значений:
            public string NEW_ORDER = "NEW_ORDER";//●	новая заявка
            public string NEW_STOP_ORDER = "NEW_STOP_ORDER";//●	новая стоп-заявка
            public string KILL_ORDER = "KILL_ORDER";//●	снять заявку
            public string KILL_STOP_ORDER = "KILL_STOP_ORDER";//●	снять стоп-заявку
            public string KILL_ALL_ORDERS = "KILL_ALL_ORDERS";//●	снять все заявки из торговой системы 
            public string KILL_ALL_STOP_ORDERS = "KILL_ALL_STOP_ORDERS";//●	снять все стоп-заявки
            public string KILL_ALL_FUTURES_ORDERS = "KILL_ALL_FUTURES_ORDERS";//●	снять все заявки на рынке FORTS
            public string KILL_RTS_T4_LONG_LIMIT = "KILL_RTS_T4_LONG_LIMIT";//●	удалить лимит открытых позиций на спот-рынке RTS Standard
            public string KILL_RTS_T4_SHORT_LIMIT = "KILL_RTS_T4_SHORT_LIMIT";//●	удалить лимит открытых позиций клиента по спот-активу на рынке RTS Standard
            public string MOVE_ORDERS = "MOVE_ORDERS";//●	переставить заявки на рынке FORTS
            public string NEW_QUOTE = "NEW_QUOTE";//●	новая безадресная заявка
            public string KILL_QUOTE = "KILL_QUOTE";//●	снять безадресную заявку

            public string FIRM_ID = "FIRM_ID=";// Идентификатор участника торгов (код фирмы)
            public string ACCOUNT = "ACCOUNT=";// Номер счета Трейдера, обязательный параметр. Параметр чувствителен к верхнему/нижнему регистру символов
            public string CLIENT_CODE = "CLIENT_CODE=";// 20-ти символьное составное поле, может содержать код клиента и текстовый комментарий с тем же разделителем, что и при вводе заявки вручную. Необязательный параметр
            public string TYPE = "TYPE=";// Тип заявки, необязательный параметр. Значения: «L» – лимитированная (по умолчанию), «M» – рыночная
            public string OPERATION = "OPERATION=";// 	Направление заявки, обязательный параметр. Значения: «S» – продать, «B» – купить

            public string EXECUTION_CONDITION = "EXECUTION_CONDITION=";//	Условие исполнения заявки, необязательный параметр. Возможные значения:
            public string PUT_IN_QUEUE = "PUT_IN_QUEUE";//●	поставить в очередь (по умолчанию),
            public string FILL_OR_KILL = "FILL_OR_KILL";//●	немедленно или отклонить,
            public string KILL_BALANCE = "KILL_BALANCE";//●	снять остаток

            public string QUANTITY = "QUANTITY=";//	Количество лотов в заявке, обязательный параметр
            public string PRICE = "PRICE=";//	Цена заявки, за единицу инструмента. Обязательный параметр. При выставлении рыночной заявки (TYPE=M) на Срочном рынке FORTS необходимо указывать значение цены – укажите наихудшую (минимально или максимально возможную – в зависимости от направленности), заявка все равно будет исполнена по рыночной цене. Для других рынков при выставлении рыночной заявки укажите price=0
            public string STOPPRICE = "STOPPRICE=";//	Стоп-цена, за единицу инструмента. Используется только при «ACTION» = «NEW_STOP_ORDER»

            public string STOP_ORDER_KIND = "STOP_ORDER_KIND=";//	Тип стоп-заявки. Возможные значения:
            public string SIMPLE_STOP_ORDER = "SIMPLE_STOP_ORDER";//●	стоп-лимит,
            public string CONDITION_PRICE_BY_OTHER_SEC = "CONDITION_PRICE_BY_OTHER_SEC";//●	с условием по другой бумаге,
            public string WITH_LINKED_LIMIT_ORDER = "WITH_LINKED_LIMIT_ORDER";//●	со связанной заявкой,
            public string TAKE_PROFIT_STOP_ORDER = "TAKE_PROFIT_STOP_ORDER";//●	тэйк-профит,
            public string TAKE_PROFIT_AND_STOP_LIMIT_ORDER = "TAKE_PROFIT_AND_STOP_LIMIT_ORDER";//●	тэйк-профит и стоп-лимит,
            public string ACTIVATED_BY_ORDER_SIMPLE_STOP_ORDER = "";//●	стоп-лимит по исполнению заявки,
            public string ACTIVATED_BY_ORDER_TAKE_PROFIT_STOP_ORDER = "ACTIVATED_BY_ORDER_TAKE_PROFIT_STOP_ORDER";//●	тэйк-профит по исполнению заявки,
            public string ACTIVATED_BY_ORDER_TAKE_PROFIT_AND_STOP_LIMIT_ORDER = "ACTIVATED_BY_ORDER_TAKE_PROFIT_AND_STOP_LIMIT_ORDER";//●	тэйк-профит и стоп-лимит по исполнению заявки
            public string NOLE = "";//Если параметр пропущен, то считается, что заявка имеет тип «стоп-лимит»

            public string STOPPRICE_CLASSCODE = "STOPPRICE_CLASSCODE=";//	Класс инструмента условия. Используется только при «STOP_ORDER_KIND» = «CONDITION_PRICE_BY_OTHER_SEC»
            public string STOPPRICE_SECCODE = "STOPPRICE_SECCODE=";//	Код инструмента условия. Используется только при «STOP_ORDER_KIND» = «CONDITION_PRICE_BY_OTHER_SEC»
            public string STOPPRICE_CONDITION = "STOPPRICE_CONDITION=";//	Направление предельного изменения стоп-цены. Используется только при «STOP_ORDER_KIND» = «CONDITION_PRICE_BY_OTHER_SEC». Возможные значения: «<=» или «>=»
            public string LINKED_ORDER_PRICE = "";//	Цена связанной лимитированной заявки. Используется только при «STOP_ORDER_KIND» = «WITH_LINKED_LIMIT_ORDER»

            public string EXPIRY_DATE = "EXPIRY_DATE=";//	Срок действия стоп-заявки. Возможные значения:
            public string GTC = "GTC";//●	до отмены
            public string TODAY = "TODAY";//●	до окончания текущей торговой сессии,
            public string DATA = "";//●	Дата в формате «ГГГГММДД», где «ГГГГ» – год, «ММ» – месяц, «ДД» – день

            public string STOPPRICE2 = "STOPPRICE2=";//	Цена условия «стоп-лимит» для заявки типа «Тэйк-профит и стоп-лимит»
            public string MARKET_STOP_LIMIT = "MARKET_STOP_LIMIT=";//	Признак исполнения заявки по рыночной цене при наступлении условия «стоп-лимит». Значения «YES» или «NO». Параметр заявок типа «Тэйк-профит и стоп-лимит»
            public string MARKET_TAKE_PROFIT = "MARKET_TAKE_PROFIT=";//	Признак исполнения заявки по рыночной цене при наступлении условия «тэйк-профит». Значения «YES» или «NO». Параметр заявок типа «Тэйк-профит и стоп-лимит»
            public string IS_ACTIVE_IN_TIME = "IS_ACTIVE_IN_TIME=";//	Признак действия заявки типа «Тэйк-профит и стоп-лимит» в течение определенного интервала времени. Значения «YES» или «NO»
            public string ACTIVE_FROM_TIME = "ACTIVE_FROM_TIME=";//	Время начала действия заявки типа «Тэйк-профит и стоп-лимит» в формате «ЧЧММСС»
            public string ACTIVE_TO_TIME = "";//	Время окончания действия заявки типа «Тэйк-профит и стоп-лимит» в формате «ЧЧММСС»

            public string ACTION2 = "";//Применяется при «ACTION» = «NEW_NEG_DEAL», «ACTION» = «NEW_REPO_NEG_DEAL» или «ACTION» = «NEW_EXT_REPO_NEG_DEAL»
            public string ORDER_KEY = "ORDER_KEY=";//	Номер заявки, снимаемой из торговой системы
            public string ACTION3 = "";//Применяется при «ACTION» = «KILL_ORDER» или «ACTION» = «KILL_NEG_DEAL» или «ACTION» = «KILL_QUOTE»
            public string STOP_ORDER_KEY = "STOP_ORDER_KEY=";//	Номер стоп-заявки, снимаемой из торговой системы. Применяется только при «ACTION» = «KILL_STOP_ORDER»
            public string TRANS_ID = "TRANS_ID=";//	Уникальный идентификационный номер заявки, значение от 1 до 2 294 967 294


            public string COMMENT = "COMMENT=";//	Текстовый комментарий, указанный в заявке. Используется при снятии группы заявок

            public string KILL_IF_LINKED_ORDER_PARTLY_FILLED = "KILL_IF_LINKED_ORDER_ PARTLY_FILLED=";//	Признак снятия стоп-заявки при частичном исполнении связанной лимитированной заявки. Используется только при «STOP_ORDER_KIND» = «WITH_LINKED_LIMIT_ORDER». Возможные значения: «YES» или «NO»
            public string OFFSET = "OFFSET=";//	Величина отступа от максимума (минимума) цены последней сделки. Используется при «STOP_ORDER_KIND» = «TAKE_PROFIT_STOP_ORDER» или «ACTIVATED_BY_ORDER_TAKE_PROFIT_STOP_ORDER»

            public string OFFSET_UNITS = "OFFSET_UNITS=";//	Единицы измерения отступа. Возможные значения:
            public string PERCENTS = "PERCENTS";//●	в процентах (шаг изменения – одна сотая процента),
            public string PRICE_UNITS = "PRICE_UNITS";//●	в параметрах цены (шаг изменения равен шагу цены по данному инструменту)

            public string STOP_ORDER_KIND2 = "";//Используется при «STOP_ORDER_KIND» = «TAKE_PROFIT_STOP_ORDER» или «ACTIVATED_BY_ORDER_TAKE_PROFIT_STOP_ORDER»
            public string SPREAD = "SPREAD=";//	Величина защитного спрэда. Используется при «STOP_ORDER_KIND» = «TAKE_PROFIT_STOP_ORDER» или «ACTIVATED_BY_ORDER_TAKE_PROFIT_STOP_ORDER»

            public string SPREAD_UNITS = "SPREAD_UNITS=";//	Единицы измерения защитного спрэда. Возможные значения:
            public string PERCENTS_2 = "PERCENTS";//●	в процентах (шаг изменения – одна сотая процента),
            public string PRICE_UNITS_2 = "PRICE_UNITS";//●	в параметрах цены (шаг изменения равен шагу цены по данному инструменту)

            public string STOP_ORDER_KIND3 = "";//Используется при «STOP_ORDER_KIND» = «TAKE_PROFIT_STOP_ORDER» или «ACTIVATED_BY_ORDER_TAKE_PROFIT_STOP_ORDER»
            public string BASE_ORDER_KEY = "BASE_ORDER_KEY=";//	Регистрационный номер заявки-условия. Используется при «STOP_ORDER_KIND» = «ACTIVATED_BY_ORDER_SIMPLE_STOP_ORDER» или «ACTIVATED_BY_ORDER_TAKE_PROFIT_STOP_ORDER»
            public string USE_BASE_ORDER_ALANCE = "USE_BASE_ORDER_ BALANCE=";//	Признак использования в качестве объема заявки «по исполнению» исполненного количества бумаг заявки-условия. Возможные значения: «YES» или «NO». Используется при «STOP_ORDER_KIND» = «ACTIVATED_BY_ORDER_SIMPLE_STOP_ORDER» или «ACTIVATED_BY_ORDER_TAKE_PROFIT_STOP_ORDER»
            public string ACTIVATE_IF_BASE_ORDER_PARTLY_FILLED = "ACTIVATE_IF_BASE_ ORDER_PARTLY_FILLED=";//	Признак активации заявки «по исполнению» при частичном исполнении заявки-условия. Возможные значения: «YES» или «NO». Используется при «STOP_ORDER_KIND» = «ACTIVATED_BY_ORDER_SIMPLE_STOP_ORDER» или «ACTIVATED_BY_ORDER_TAKE_PROFIT_STOP_ORDER»
            public string BASE_CONTRACT = "BASE_CONTRACT=";//	Идентификатор базового контракта для фьючерсов или опционов. Обязательный параметр снятия заявок на рынке FORTS

            public string MODE = "MODE=";//	Режим перестановки заявок на рынке FORTS. Параметр операции «ACTION» = «MOVE_ORDERS» Возможные значения:
            public string MODE0 = "0";//●	«0» – оставить количество в заявках без изменения,
            public string MODE1 = "1";//●	«1» – изменить количество в заявках на новые,
            public string MODE2 = "2";//●	«2» – при несовпадении новых количеств с текущим хотя бы в одной заявке, обе заявки снимаются

            public string FIRST_ORDER_NUMBER = "FIRST_ORDER_NUMBER=";//	Номер первой заявки
            public string FIRST_ORDER_NEW_QUANTITY = "FIRST_ORDER_NEW_ QUANTITY=";//	Количество в первой заявке
            public string FIRST_ORDER_NEW_PRICE = "FIRST_ORDER_NEW_ PRICE=";//	Цена в первой заявке
            public string SECOND_ORDER_NUMBER = "SECOND_ORDER_ NUMBER=";//	Номер второй заявки
            public string SECOND_ORDER_NEW_QUANTITY = "SECOND_ORDER_NEW_ QUANTITY=";//	Количество во второй заявке
            public string SECOND_ORDER_NEW_PRICE = "SECOND_ORDER_NEW_ PRICE=";//	Цена во второй заявке

            public string KILL_ACTIVE_ORDERS = "KILL_ACTIVE_ORDERS=";//	Признак снятия активных заявок по данному инструменту. Используется только при «ACTION» = «NEW_QUOTE». Возможные значения: «YES» или «NO»
            public string NEG_TRADE_OPERATION = "NEG_TRADE_OPERATION=";//	Направление операции в сделке, подтверждаемой отчетом
            public string NEG_TRADE_NUMBER = "NEG_TRADE_NUMBER=";//	Номер подтверждаемой отчетом сделки для исполнения
            public string VOLUMEMN = "VOLUMEMN=";//	Лимит открытых позиций, при «Тип лимита» = «Ден.средства» или «Всего»
            public string VOLUMEPL = "VOLUMEPL=";//	Лимит открытых позиций, при «Тип лимита» = «Залоговые ден.средства»
            public string CHECK_LIMITS = "CHECK_LIMITS=";//	Признак проверки попадания цены заявки в диапазон допустимых цен. Параметр Срочного рынка FORTS. Необязательный параметр транзакций установки новых заявок по классам «Опционы ФОРТС» и «РПС: Опционы ФОРТС». Возможные значения: «YES» – выполнять проверку, «NO» – не выполнять

        }

        public class CLASSCODE // коды биржевых торговых секций
        {

            public string NGCB_MMVB_A_A = "EQBR"; // код биржи ММВБ-акции-А (негосударственные ценные бумаги)
            public string NGCB_MMVB_O_A = "EQOB"; // код биржи ММВБ-облигации-А
            public string NGCB_MMVB_A_A2 = "EQBS"; // код биржи ММВБ-акции-A2
            public string NGCB_MMVB_O_A2 = "EQOS"; // код биржи ММВБ-облигации-A2
            public string NGCB_MMVB_A_B = "EQNL"; // код биржи ММВБ-акции-Б
            public string NGCB_MMVB_O_B = "EQNO"; // код биржи ММВБ-облигации-Б
            public string NGCB_MMVB_A_C = "EQLV"; // код биржи ММВБ-акции-B
            public string NGCB_MMVB_O_C = "EQOV"; // код биржи ММВБ-облигации-B
            public string NGCB_MMVB_A_VN = "EQNE"; // код биржи ММВБ-акции-Внесписочные
            public string NGCB_MMVB_O_VN = "EQNB"; // код биржи ММВБ-облигации-Внесписочные
            public string GKO_MMVB_O_VN = "MAIN"; // код биржи ММВБ-облигации ГКО


            public string FORTS_Futures = "SPBFUT"; // код биржи фьючерсы ФОРТС
            public string FORTS_Option = "SPBOPT"; // код биржи опционы ФОРТС

            public string Standard = "RTSST"; // код биржи Standard

        }

        public class SECCODE // тикеры наиболее ликвидных ЦБ
        {

            public string SBER = "SBER"; //    Сбербанк России ОАО ао	ММВБ: А1-Акции	EQBR
            public string LKOH = "LKOH"; //	НК ЛУКОЙЛ (ОАО) - ао	ММВБ: А1-Акции	EQBR
            public string SBERP = "SBERP"; //	Сбербанк России ОАО ап	ММВБ: А1-Акции	EQBR
            public string URKA = "URKA"; //	Уралкалий (ОАО) ао	ММВБ: А1-Акции	EQBR
            public string RTKM = "RTKM"; //	Ростелеком (ОАО) ао.	ММВБ: А1-Акции	EQBR
            public string TATN = "TATN"; //	ОАО "Татнефть" ао	ММВБ: А1-Акции	EQBR
            public string HYDR = "HYDR"; //	ОАО "РусГидро"	ММВБ: А1-Акции	EQBR
            public string MRKH = "MRKH"; //	"Холдинг МРСК" ОАО ао	ММВБ: А1-Акции	EQBR
            public string NVTK = "NVTK"; //	ОАО "НОВАТЭК" ао	ММВБ: А1-Акции	EQBR
            public string MGNT = "MGNT"; //	"Магнит" ОАО ао	ММВБ: А1-Акции	EQBR
            public string MTSS = "MTSS"; //	Мобильные ТелеСистемы (ОАО) ао	ММВБ: А1-Акции	EQBR
            public string MTLR = "MTLR"; //	Мечел ОАО ао	ММВБ: А1-Акции	EQBR
            public string IRAO = "IRAO"; //	"ИНТЕР РАО ЕЭС" ОАО ао	ММВБ: А1-Акции	EQBR

            public string GMKN = "GMKN"; //	ГМК "Нор.Никель" ОАО ао	ММВБ: А2-Акции	EQBS

            public string VTBR = "VTBR"; //	ао ОАО Банк ВТБ	ММВБ: Б-Акции	EQNL
            public string TRNFP = "TRNFP"; //	акц.пр. ОАО АК "Транснефть"	ММВБ: Б-Акции	EQNL
            public string ROSN = "ROSN"; //	ОАО "НК "Роснефть"	ММВБ: Б-Акции	EQNL
            public string SNGS = "SNGS"; //    Сургутнефтегаз ОАО акции об.	ММВБ: Б-Акции	EQNL
            public string CHMF = "CHMF"; //    Северсталь (ОАО)ао	ММВБ: Б-Акции	EQNL
            public string FEES = "FEES"; //    "ФСК ЕЭС" ОАО ао	ММВБ: Б-Акции	EQNL
            public string NLMK = "NLMK"; 	//  ОАО "НЛМК" ао	ММВБ: Б-Акции	EQNL
            public string SNGSP = "SNGSP"; //	Сургутнефтегаз ОАО ап	ММВБ: Б-Акции	EQNL
            public string VSMO = "VSMO"; //	Верхнесалдинское МПО (ОАО) ао	ММВБ: Б-Акции	EQNL
            public string MAGN = "MAGN"; //	"Магнитогорск.мет.комб" ОАО ао	ММВБ: Б-Акции	EQNL

            public string GAZP = "GAZP"; //	"Газпром" (ОАО) ао	ММВБ: Внесписочные акции	EQNE

            public string RIH2 = "RIH2"; //	RTS-3.12	РТС : Фьючерсы FORTS	SPBFUT
            public string RIM2 = "RIM2"; //	RTS-6.12	РТС : Фьючерсы FORTS	SPBFUT
            public string RIU2 = "RIU2"; //	RTS-9.12	РТС : Фьючерсы FORTS	SPBFUT
            public string RIZ2 = "RIZ2"; //	RTS-12.12	РТС : Фьючерсы FORTS	SPBFUT

            public string SiU2 = "SiU2"; //	Si-9.12	РТС : Фьючерсы FORTS	SPBFUT
            public string BRQ2 = "BRQ2"; //	BR-8.12	РТС : Фьючерсы FORTS	SPBFUT
            public string SRU2 = "SRU2"; //	SBRF-9.12	РТС : Фьючерсы FORTS	SPBFUT
            public string EDU2 = "EDU2"; //	ED-9.12	РТС : Фьючерсы FORTS	SPBFUT
            public string GZU2 = "GZU2"; //	GAZR-9.12	РТС : Фьючерсы FORTS	SPBFUT
            public string GDU2 = "GDU2"; //	GOLD-9.12	РТС : Фьючерсы FORTS	SPBFUT

            public string SBER_St = "SBER"; //    SBER	Standard	RTSST
            public string LKOH_St = "LKOH"; //    LKOH	Standard	RTSST
            public string GAZP_St = "GAZP"; //    GAZP	Standard	RTSST

        }

        public class Persona_ID // персональные данные брокерского договора
        {
            public string CLIENT_CODE = "11673";
            public string DBO_Q = "11673-Q";
            public string ACCOUNT = "MG0094600144";
            public string Broker_COD = "MG0094600000";
            public string TKS = "L01-00000F00";
            public string FORTS = "15003bf";
            public string Standart = "15st392";
        }

        public class Order_QUIK // торговые команды в QUIK
        {

            public string order; // переменная которая будет содержать сформированный торговый приказ


            // 1# Заявка на продажу лимитированная
            public string Order_Sell_Limit(string ACCOUNT, string CLIENT_CODE, string TRANS_ID, string CLASSCODE, 
                                           string SECCODE, string PRICE, string QUANTITY)
            {
                
                order = "ACCOUNT=" + ACCOUNT + "; " +               // ACCOUNT=NL0080000043;
                        "CLIENT_CODE=" + CLIENT_CODE  + "; " +      // CLIENT_CODE=11673;
                        "TYPE = L"  + "; " +                        // TYPE=L;
                        "TRANS_ID=" + TRANS_ID + "; " +             // TRANS_ID=1;
                        "CLASSCODE=" + CLASSCODE + "; " +           // CLASSCODE=SPBFUT;
                        "SECCODE=" + SECCODE + "; " +               // SECCODE=RIU2; 
                        "ACTION=NEW_ORDER" + "; " +                 // ACTION=NEW_ORDER; 
                        "OPERATION=S" + "; " +                      // OPERATION=S; 
                        "PRICE=" + PRICE  +"; " +                   // PRICE=43,21; 
                        "QUANTITY=" + QUANTITY + ";";               // QUANTITY=1;

                return order;
            }

            // 2# Заявка на покупку лимитированная
            public string Order_Buy_Limit(string ACCOUNT, string CLIENT_CODE, string TRANS_ID, string CLASSCODE,
                                           string SECCODE, string PRICE, string QUANTITY)
            {
                order = "ACCOUNT=" + ACCOUNT + "; " +               // ACCOUNT=NL0080000043;
                        "CLIENT_CODE=" + CLIENT_CODE + "; " +       // CLIENT_CODE=11673;
                        "TYPE = L" + "; " +                         // TYPE=L;
                        "TRANS_ID=" + TRANS_ID + "; " +             // TRANS_ID=1;
                        "CLASSCODE=" + CLASSCODE + "; " +           // CLASSCODE=SPBFUT;
                        "SECCODE=" + SECCODE + "; " +               // SECCODE=RIU2; 
                        "ACTION=NEW_ORDER" + "; " +                 // ACTION=NEW_ORDER; 
                        "OPERATION=B" + "; " +                      // OPERATION=B; 
                        "PRICE=" + PRICE + "; " +                   // PRICE=43,21; 
                        "QUANTITY=" + QUANTITY + ";";               // QUANTITY=1;

                return order;
            }

            // 3# Заявка на продажу рыночная
            public string Order_Sell_Market(string ACCOUNT, string CLIENT_CODE, string TRANS_ID, string CLASSCODE,
                                           string SECCODE, string QUANTITY)
            {
                order = "ACCOUNT=" + ACCOUNT + "; " +               // ACCOUNT=NL0080000043;
                        "CLIENT_CODE=" + CLIENT_CODE + "; " +       // CLIENT_CODE=11673;
                        "TYPE = M" + "; " +                         // TYPE=L;
                        "TRANS_ID=" + TRANS_ID + "; " +             // TRANS_ID=1;
                        "CLASSCODE=" + CLASSCODE + "; " +           // CLASSCODE=SPBFUT;
                        "SECCODE=" + SECCODE + "; " +               // SECCODE=RIU2; 
                        "ACTION=NEW_ORDER" + "; " +                 // ACTION=NEW_ORDER; 
                        "OPERATION=S" + "; " +                      // OPERATION=S; 
                        "PRICE=0" + "; " +                          // PRICE=0; 
                        "QUANTITY=" + QUANTITY + ";";               // QUANTITY=1;

                return order;
            }

            // 4# Заявка на покупку рыночная 
             public string Order_Buy_Market(string ACCOUNT, string CLIENT_CODE, string TRANS_ID, string CLASSCODE,
                                           string SECCODE, string QUANTITY)
            {
                order = "ACCOUNT=" + ACCOUNT + "; " +               // ACCOUNT=NL0080000043;
                        "CLIENT_CODE=" + CLIENT_CODE + "; " +       // CLIENT_CODE=11673;
                        "TYPE = M" + "; " +                         // TYPE=L;
                        "TRANS_ID=" + TRANS_ID + "; " +             // TRANS_ID=1;
                        "CLASSCODE=" + CLASSCODE + "; " +           // CLASSCODE=SPBFUT;
                        "SECCODE=" + SECCODE + "; " +               // SECCODE=RIU2; 
                        "ACTION=NEW_ORDER" + "; " +                 // ACTION=NEW_ORDER; 
                        "OPERATION=B" + "; " +                      // OPERATION=B; 
                        "PRICE=0" + "; " +                          // PRICE=0; 
                        "QUANTITY=" + QUANTITY + ";";               // QUANTITY=1;

                return order;
            }

            // 5# Стоп-лимит на продажу со сроком жизни до определенной даты
            public string Stop_Order_Sell_Data(string ACCOUNT, string CLIENT_CODE, string TRANS_ID, string CLASSCODE,
                                           string SECCODE, string QUANTITY, string SIGNAL_STOP_PRISE, string STOP_PRISE, 
                                           string EXPIRY_DATE)
            {
                order = "ACTION=NEW_STOP_ORDER" + "; " +            // ACTION=NEW_STOP_ORDER; 
                        "ACCOUNT=" + ACCOUNT + "; " +               // ACCOUNT=NL0080000043; 
                        "TRANS_ID=" + TRANS_ID + "; " +             // TRANS_ID=1;
                        "CLASSCODE=" + CLASSCODE + "; " +           // CLASSCODE=SPBFUT;
                        "SECCODE=" + SECCODE + "; " +               // SECCODE=RIU2; 
                        "OPERATION=S" + "; " +                      // OPERATION=S;  
                        "QUANTITY=" + QUANTITY + "; " +              // QUANTITY=1; 
                        "CLIENT_CODE=" + CLIENT_CODE + "; " +       // CLIENT_CODE=11673;
                        "STOPPRICE=" + SIGNAL_STOP_PRISE + "; " +   // STOPPRICE=7.3; 
                        "PRICE=" + STOP_PRISE + "; " +              // PRICE=7.0; 
                        "EXPIRY_DATE=" + EXPIRY_DATE + ";";         // EXPIRY_DATE=20110519;

                return order;
            }

            // 6# Стоп-лимит на покупку со сроком жизни до определенной даты
            public string Stop_Order_Buy_Data(string ACCOUNT, string CLIENT_CODE, string TRANS_ID, string CLASSCODE,
                                           string SECCODE, string QUANTITY, string SIGNAL_STOP_PRISE, string STOP_PRISE,
                                           string EXPIRY_DATE)
            {
                order = "ACTION=NEW_STOP_ORDER" + "; " +            // ACTION=NEW_STOP_ORDER; 
                        "ACCOUNT=" + ACCOUNT + "; " +               // ACCOUNT=NL0080000043; 
                        "TRANS_ID=" + TRANS_ID + "; " +             // TRANS_ID=1;
                        "CLASSCODE=" + CLASSCODE + "; " +           // CLASSCODE=SPBFUT;
                        "SECCODE=" + SECCODE + "; " +               // SECCODE=RIU2; 
                        "OPERATION=B" + "; " +                      // OPERATION=B;  
                        "QUANTITY=" + QUANTITY + "; " +              // QUANTITY=1; 
                        "CLIENT_CODE=" + CLIENT_CODE + "; " +       // CLIENT_CODE=11673;
                        "STOPPRICE=" + SIGNAL_STOP_PRISE + "; " +   // STOPPRICE=7.3; 
                        "PRICE=" + STOP_PRISE + "; " +              // PRICE=7.0; 
                        "EXPIRY_DATE=" + EXPIRY_DATE + ";";         // EXPIRY_DATE=20110519;

                return order;
            }

            // 7# Стоп-лимит на продажу со сроком жизни до конца торговой сессии
            public string Stop_Order_Sell_Today(string ACCOUNT, string CLIENT_CODE, string TRANS_ID, string CLASSCODE,
                                           string SECCODE, string QUANTITY, string SIGNAL_STOP_PRISE, string STOP_PRISE)
            {
                order = "ACTION=NEW_STOP_ORDER" + "; " +            // ACTION=NEW_STOP_ORDER; 
                        "ACCOUNT=" + ACCOUNT + "; " +               // ACCOUNT=NL0080000043; 
                        "TRANS_ID=" + TRANS_ID + "; " +             // TRANS_ID=1;
                        "CLASSCODE=" + CLASSCODE + "; " +           // CLASSCODE=SPBFUT;
                        "SECCODE=" + SECCODE + "; " +               // SECCODE=RIU2; 
                        "OPERATION=S" + "; " +                      // OPERATION=S;  
                        "QUANTITY=" + QUANTITY + "; " +              // QUANTITY=1; 
                        "CLIENT_CODE=" + CLIENT_CODE + "; " +       // CLIENT_CODE=11673;
                        "STOPPRICE=" + SIGNAL_STOP_PRISE + "; " +   // STOPPRICE=7.3; 
                        "PRICE=" + STOP_PRISE + "; " +              // PRICE=7.0; 
                        "EXPIRY_DATE=" + "TODAY;";                  // EXPIRY_DATE=TODAY;

                return order;
            }

            // 8# Стоп-лимит на покупку со сроком жизни до конца торговой сессии
            public string Stop_Order_Bay_Today(string ACCOUNT, string CLIENT_CODE, string TRANS_ID, string CLASSCODE,
                                           string SECCODE, string QUANTITY, string SIGNAL_STOP_PRISE, string STOP_PRISE)
            {
                order = "ACTION=NEW_STOP_ORDER" + "; " +            // ACTION=NEW_STOP_ORDER; 
                        "ACCOUNT=" + ACCOUNT + "; " +               // ACCOUNT=NL0080000043; 
                        "TRANS_ID=" + TRANS_ID + "; " +             // TRANS_ID=1;
                        "CLASSCODE=" + CLASSCODE + "; " +           // CLASSCODE=SPBFUT;
                        "SECCODE=" + SECCODE + "; " +               // SECCODE=RIU2; 
                        "OPERATION=B" + "; " +                      // OPERATION=B;  
                        "QUANTITY=" + QUANTITY + "; " +              // QUANTITY=1; 
                        "CLIENT_CODE=" + CLIENT_CODE + "; " +       // CLIENT_CODE=11673;
                        "STOPPRICE=" + SIGNAL_STOP_PRISE + "; " +   // STOPPRICE=7.3; 
                        "PRICE=" + STOP_PRISE + "; " +              // PRICE=7.0; 
                        "EXPIRY_DATE=" + "TODAY;";                  // EXPIRY_DATE=TODAY;

                return order;
            }

            // 9# Стоп-лимит на продажу со сроком жизни до отмены
            public string Stop_Order_Sell_GTC(string ACCOUNT, string CLIENT_CODE, string TRANS_ID, string CLASSCODE,
                                           string SECCODE, string QUANTITY, string SIGNAL_STOP_PRISE, string STOP_PRISE)
            {
                order = "ACTION=NEW_STOP_ORDER" + "; " +            // ACTION=NEW_STOP_ORDER; 
                        "ACCOUNT=" + ACCOUNT + "; " +               // ACCOUNT=NL0080000043; 
                        "TRANS_ID=" + TRANS_ID + "; " +             // TRANS_ID=1;
                        "CLASSCODE=" + CLASSCODE + "; " +           // CLASSCODE=SPBFUT;
                        "SECCODE=" + SECCODE + "; " +               // SECCODE=RIU2; 
                        "OPERATION=S" + "; " +                      // OPERATION=S;  
                        "QUANTITY=" + QUANTITY + "; " +              // QUANTITY=1; 
                        "CLIENT_CODE=" + CLIENT_CODE + "; " +       // CLIENT_CODE=11673;
                        "STOPPRICE=" + SIGNAL_STOP_PRISE + "; " +   // STOPPRICE=7.3; 
                        "PRICE=" + STOP_PRISE + "; " +              // PRICE=7.0; 
                        "EXPIRY_DATE=" + "GTC;";                  // EXPIRY_DATE=GTS;

                return order;
            }

            // 10# Стоп-лимит на покупку со сроком жизни до отмены
            public string Stop_Order_Buy_GTC(string ACCOUNT, string CLIENT_CODE, string TRANS_ID, string CLASSCODE,
                                           string SECCODE, string QUANTITY, string SIGNAL_STOP_PRISE, string STOP_PRISE)
            {
                order = "ACTION=NEW_STOP_ORDER" + "; " +            // ACTION=NEW_STOP_ORDER; 
                        "ACCOUNT=" + ACCOUNT + "; " +               // ACCOUNT=NL0080000043; 
                        "TRANS_ID=" + TRANS_ID + "; " +             // TRANS_ID=1;
                        "CLASSCODE=" + CLASSCODE + "; " +           // CLASSCODE=SPBFUT;
                        "SECCODE=" + SECCODE + "; " +               // SECCODE=RIU2; 
                        "OPERATION=B" + "; " +                      // OPERATION=S;  
                        "QUANTITY=" + QUANTITY + "; " +              // QUANTITY=1; 
                        "CLIENT_CODE=" + CLIENT_CODE + "; " +       // CLIENT_CODE=11673;
                        "STOPPRICE=" + SIGNAL_STOP_PRISE + "; " +   // STOPPRICE=7.3; 
                        "PRICE=" + STOP_PRISE + "; " +              // PRICE=7.0; 
                        "EXPIRY_DATE=" + "GTC;";                  // EXPIRY_DATE=GTS;

                return order;
            }


            // 11# Стоп-заявка с условием по другой бумаге Ростелеком -ао, продажа 15 лотов по цене 7,000, 
            // условие по Ростелеком-ап, условие стоп-цены <= 8,000	
            // ACTION=NEW_STOP_ORDER; STOP_ORDER_KIND=CONDITION_PRICE_BY_OTHER_SEC; ACCOUNT= NL0080000043; 
            // QUANTITY=15; TRANS_ID=15; CLASSCODE=EQBR; SECCODE=RTKM; STOPPRICE_CLASSCODE=EQBR; STOPPRICE_SECCODE=RTKMP; 
            // STOPPRICE_CONDITION=<=; OPERATION=S; CLIENT_CODE=1001; STOPPRICE=8.0; PRICE=7.0;

            // 12# Стоп-заявка со связанной заявкой РусГидро, покупка 15 лотов по цене 8,500, стоп-цена >= 8,000, 
            // и лимитированная заявка по 6,000	
            // ACTION=NEW_STOP_ORDER; STOP_ORDER_KIND=WITH_LINKED_LIMIT_ORDER; ACCOUNT= NL0080000043; TRANS_ID=16; 
            // CLASSCODE=EQBR; SECCODE=HYDR; OPERATION=B; QUANTITY=15; CLIENT_CODE=1001; STOPPRICE=8.0; PRICE=8.5; 
            // LINKED_ORDER_PRICE=6.0; KILL_IF_LINKED_ORDER_PARTLY_FILLED=NO;

            // 13# Тэйк-профит Лукойл, покупка 1 лота, активация при достижении цены 265 с отступом в 5% 
            // и защитным интервалом в 5 пипсов, срок исполнения до 06.07.2010	
            // ACTION=NEW_STOP_ORDER; TRANS_ID=8; STOP_ORDER_KIND=TAKE_PROFIT_STOP_ORDER; STOPPRICE=265; 
            // CLIENT_CODE=Q5; OPERATION=B; SECCODE=LKOH; CLASSCODE=EQBR; ACCOUNT=L01-00000F00; QUANTITY=1; 
            // EXPIRY_DATE=20100706; OFFSET=5; OFFSET_UNITS=PERCENTS; SPREAD=5; SPREAD_UNITS=PRICE_UNITS;

            // 14# Тэйк-профит и стоп-лимит Лукойл, покупка 1 лота, активация тэйк-профита при достижении цены 2000 
            // с отступом в 5% и защитным спрэдом в 3%, стоп-цена 2222, цена лимитированной заявки 2255, время действия с 10:00:01 по 19:45:45	
            // ACTION=NEW_STOP_ORDER; TRANS_ID=10055; CLASSCODE= EQBR; SECCODE=LKOH; ACCOUNT=L01-00000F00; 
            // CLIENT_CODE=Q7; OPERATION=B; QUANTITY=1; PRICE=2255; STOPPRICE=2000; 
            // STOP_ORDER_KIND=TAKE_PROFIT_AND_STOP_LIMIT_ORDER; OFFSET=5; OFFSET_UNITS=PERCENTS; SPREAD=3; 
            // SPREAD_UNITS=PERCENTS; MARKET_TAKE_PROFIT=NO; STOPPRICE2=2222; IS_ACTIVE_IN_TIME=YES; 
            // ACTIVE_FROM_TIME=100001; ACTIVE_TO_TIME=194545; MARKET_STOP_LIMIT=NO


            // 15# Тэйк-профит по исполнению заявки по частичному исполнению заявки с номером 81874488 
            // выставить тэйк-профит на покупку Лукойла в объеме исполненной части заявки-условия, 
            // при достижении цены 265 с отступом 10 пипсов и защитным интервалом 10 пипсов	
            // ACTION=NEW_STOP_ORDER; TRANS_ID=11; STOP_ORDER_KIND=ACTIVATED_BY_ORDER_TAKE_PROFIT_STOP_ORDER; 
            // BASE_ORDER_KEY=81874488; USE_BASE_ORDER_BALANCE=yes; ACTIVATE_IF_BASE_ORDER_PARTLY_FILLED=yes; 
            // SPREAD=10; OFFSET=10; OFFSET_UNITS=PRICE_UNITS; SPREAD_UNITS=PRICE_UNITS; STOPPRICE=265; 
            // CLIENT_CODE=Q5; OPERATION=B; SECCODE=LKOH; CLASSCODE=EQBR; ACCOUNT=L01-00000F00;

            // 16# Стоп-лимит по исполнению заявки по частичному исполнению заявки с номером 81874488 
            // выставить стоп-лимит на покупку Лукойла в объеме исполненной части заявки-условия, 
            // со стоп-ценой 271 и ценой заявки 270	
            // ACTION=NEW_STOP_ORDER; TRANS_ID=12; STOP_ORDER_KIND=ACTIVATED_BY_ORDER_SIMPLE_STOP_ORDER; 
            // BASE_ORDER_KEY=81874488; USE_BASE_ORDER_BALANCE=yes; ACTIVATE_IF_BASE_ORDER_PARTLY_FILLED=yes; 
            // PRICE=270; STOPPRICE=271; CLASSCODE=EQBR; SECCODE=LKOH; ACCOUNT=L01-00000F00; OPERATION=B; CLIENT_CODE=Q5;

            // 17# Тэйк-профит и стоп-лимит по исполнению заявки По частичному исполнению заявки с номером 123456 
            // выставить тэйк-профит на покупку Лукойла по рыночной цене, активация при достижении цены 2000 
            // с отступом в 5 пипсов, и стоп-лимит: стоп-цена 1990, исполнение по рыночной цене	
            // ACTION=NEW_STOP_ORDER; TRANS_ID=10060; CLASSCODE= EQBR; SECCODE=LKOH; ACCOUNT=L01-00000F00; 
            // CLIENT_CODE=Q7; OPERATION=B; PRICE=2010; STOPPRICE=2000; 
            // STOP_ORDER_KIND=ACTIVATED_BY_ORDER_TAKE_PROFIT_AND_STOP_LIMIT_ORDER; OFFSET=5; OFFSET_UNITS=PRICE_UNITS; 
            // SPREAD=3; SPREAD_UNITS=PRICE_UNITS; BASE_ORDER_KEY=123456; USE_BASE_ORDER_BALANCE=YES; 
            // ACTIVATE_IF_BASE_ORDER_PARTLY_FILLED=YES; MARKET_TAKE_PROFIT=YES; STOPPRICE2=1990; MARKET_STOP_LIMIT=YES

            // 18# Айсберг-заявка На фондовом рынке ММВБ, купить 100 лотов Аэрофлота по цене 70, видимое количество лотов 
            // в очереди – 10	
            // CLASSCODE=EQBR; TRANS_ID=2; ACTION=Ввод айсберг заявки; Торговый счет=S01-00000F00; 
            // К/П=Купля;Тип=Лимитная;Тип по цене=по разным ценам; Тип по остатку=поставить в очередь; 
            // Тип ввода значения цены=По цене; Инструмент=AFLT; Цена=70; Лоты=100; Видимое количество=10;




            // 19# Снятие заявки с номером 503983	

            public string Kill_Order_Persona(string TRANS_ID, string ORDER_KEY, string CLASSCODE, string SECCODE)
            {
                order = "CLASSCODE = " + CLASSCODE + "; " + // CLASSCODE=SPBFUT;
                         "SECCODE=" + SECCODE + "; " +      // SECCODE=RIU2; 
                         "TRANS_ID=" + TRANS_ID + "; " +    // TRANS_ID=1;
                         "ACTION=KILL_ORDER" + "; " +       // ACTION=KILL_ORDER; 
                         "ORDER_KEY=" + ORDER_KEY + "; ";    // ORDER_KEY=503983;
                                
                return order;
            }



            // 20# Снятие всех заявок клиента с кодом Q6	
            // TRANS_ID=1; CLASSCODE=EQBR; ACTION=KILL_ALL_ORDERS; CLIENT_CODE=Q6;

           
            // 21# Снятие стоп-заявоки с номером
            public string Kill_Stop_Order_Persona(string TRANS_ID, string ORDER_KEY, string CLASSCODE, string SECCODE)
            {
                order = "CLASSCODE = " + CLASSCODE + "; " + // CLASSCODE=SPBFUT;
                         "SECCODE=" + SECCODE + "; " +      // SECCODE=RIU2; 
                         "TRANS_ID=" + TRANS_ID + "; " +    // TRANS_ID=1;
                         "ACTION=KILL_STOP_ORDER" + "; " +       // ACTION=KILL_ORDER; 
                         "STOP_ORDER_KEY=" + ORDER_KEY + "; ";    // STOP_ORDER_KEY=503983;

                return order;
            }


            
            
            // 22# Снятие всех стоп-заявок с направлением «на покупку»
	        // TRANS_ID=2; CLASSCODE=EQBR; ACTION=KILL_ALL_STOP_ORDERS; OPERATION=B;
            
            // 23# Снятие всех стоп-заявок с направлением «на продажу»
            // TRANS_ID=2; CLASSCODE=EQBR; ACTION=KILL_ALL_STOP_ORDERS; OPERATION=S;




            // 24# Снятие всех заявок на срочном рынке FORTS на покупку контрактов на курс акций Ростелеком -ао	
            // TRANS_ID=50; ACCOUNT=SPBFUT00001; ACTION=KILL_ALL_FUTURES_ORDERS; OPERATION=B; 
            // CLASSCODE=SPBFUT; BASE_CONTRACT=RTKM;
            
            // 25# Снятие всех заявок на срочном рынке FORTS на продажу контрактов на курс акций Ростелеком -ао	
            // TRANS_ID=50; ACCOUNT=SPBFUT00001; ACTION=KILL_ALL_FUTURES_ORDERS; OPERATION=S; 
            // CLASSCODE=SPBFUT; BASE_CONTRACT=RTKM;

            // 26# Удаление лимита открытых позиций на спот-рынке RTS Standard	
            // TRANS_ID=99; ACTION=KILL_RTS_T4_LONG_LIMIT; FIRM_ID= SPBFUT389; ACCOUNT=389_011; CLASSCODE=RTSST;

            // 27# Удаление лимита открытых позиций клиента по спот-активу на рынке RTS Standard	
            // TRANS_ID=117; ACTION=KILL_RTS_T4_SHORT_LIMIT; FIRM_ID= SPBFUT389; ACCOUNT=389_011; 
            // SECCODE=GAZP; CLASSCODE=RTSST;

            // 28# Перестановка заявок на срочном рынке FORTS	
            // ACTION=MOVE_ORDERS; TRANS_ID=333; CLASSCODE=SPBFUT; SECCODE=EBM6; MODE=1; FIRST_ORDER_NUMBER=21445064; 
            // FIRST_ORDER_NEW_PRICE=10004; FIRST_ORDER_NEW_QUANTITY=4; SECOND_ORDER_NUMBER=21445065; 
            // SECOND_ORDER_NEW_PRICE=10004; SECOND_ORDER_NEW_QUANTITY=;

            // 29# Безадресная заявка на покупку РусГидро, 1 лот по 15.0 руб., коду расчетов T0, 
            // с признаком снятия активных безадресных заявок «НЕТ»	
            // ACTION=NEW_QUOTE; TRANS_ID=779; CLASSCODE=PSEQ; SECCODE=HYDR; OPERATION=B; QUANTITY=1; 
            // PRICE=15.0; SETTLE_CODE=T0; KILL_ACTIVE_ORDERS=NO;

            // 30# Снятие безадресной заявки с номером 15919	
            // ACTION=KILL_QUOTE; TRANS_ID=781; CLASSCODE=PSEQ; SECCODE=HYDR; ORDER_KEY=15919;

        }

        //namespace TRI // рабочая область
        //{
        //    class Program
        //    {
        //        static void Main(string[] args)
        //        {

        //            TimeSpan Point1, Point3; // переменные для задания интервального генератора транзакций
        //            string a = "0:0:0.2";

        //            //TimeSpan Point2 = new TimeSpan(0, 0, 1);
        //            TimeSpan Point2 = TimeSpan.Parse(a);


        //            int i = 0; // счетчик

        //            string file_tri = @"E:\VisualStudio8.0\TRI файловые транзакции\ExPort_Trasaq.tri"; // адрес к файлу tri
        //            string file_trr = @"E:\VisualStudio8.0\TRI файловые транзакции\History.trr"; // адрес к файлу trr
        //            string file_tro = @"E:\VisualStudio8.0\TRI файловые транзакции\Result.tro"; // адрес к файлу tro

        //            string pusto = ""; // переменная для обнуления файлов
        //            string order; // переменная хранит торговое поручение

        //            ExPort_File_TRI transaq = new ExPort_File_TRI(); // объект класса по записи в файл


        //            QUIK_COMMANDS a1 = new QUIK_COMMANDS(); // объект класса с командами в QUIK
        //            CLASSCODE a2 = new CLASSCODE(); // объект класса с тикерами торговых секций
        //            SECCODE a3 = new SECCODE(); // объект класса с тикерами бумаг
        //            Persona_ID a4 = new Persona_ID(); // объект класса с персональными данными брокерского договора
        //            Order_QUIK a5 = new Order_QUIK(); // объект класса с торговыми ордерами

        //            decimal PRICE_2 = 145000; // цена в цифровом выражении
        //            decimal QUANTITY_2 = 1; // количество в цифровом выражении
        //            int TRANS_ID_2; // уникальный номер транзакции в цифровом вариенте
        //            int SIGNAL_STOP_PRISE_2=143000; // STOPPRICE=7.3; сигнальная цена в стоп заявке
        //            int STOP_PRISE_2=142000; // PRICE=7.0; предельная цена исполения в стоп заявки      

        //            string ACCOUNT = a4.FORTS; // ACCOUNT=NL0080000043;
        //            string CLIENT_CODE = a4.CLIENT_CODE; // CLIENT_CODE=11673;
        //            string TRANS_ID; // TRANS_ID=1, присваиваем ниже
        //            string CLASSCODE = a2.FORTS_Futures; // CLASSCODE=SPBFUT
        //            string SECCODE = a3.RIU2; // SECCODE=RIU2
        //            string PRICE = PRICE_2.ToString(); // цена
        //            string QUANTITY = QUANTITY_2.ToString(); // количество
        //            string SIGNAL_STOP_PRISE = SIGNAL_STOP_PRISE_2.ToString(); // STOPPRICE=7.3; сигнальная цена в стоп заявке
        //            string STOP_PRISE = STOP_PRISE_2.ToString(); // PRICE=7.0; предельная цена исполения в стоп заявки
        //            string EXPIRY_DATE = "20121231"; // EXPIRY_DATE=20110519;дата до которой действует стоп заявка                  

        //            while (i != 100) // генератор транзакций с временным промежутком
        //            {

        //                TRANS_ID_2 = i; // присваиваем уникальный номер транзакции
        //                TRANS_ID = TRANS_ID_2.ToString(); // уникальный номер транзакции

        //                // генерируем интервал времени

        //                Point1 = DateTime.Now.TimeOfDay;
        //                Point3 = Point1 + Point2;


        //                Console.WriteLine(Point1);
        //                Console.WriteLine(Point2);
        //                Console.WriteLine(Point3);

        //                while (Point1 <= Point3)
        //                {
        //                    Point1 = DateTime.Now.TimeOfDay;
        //                }


        //                Console.WriteLine(Point1);
        //                Console.WriteLine(Point3);

        //                // интервал времени закончен


        //                // формируем транзакцию
        //                order = a5.Order_Sell_Limit(ACCOUNT, CLIENT_CODE, TRANS_ID, CLASSCODE, SECCODE, PRICE, QUANTITY);


        //                // отправляем транзакцию
        //                transaq.ExPort_File_Metod(file_trr, pusto); // затираем все данные
        //                transaq.ExPort_File_Metod(file_tro, pusto); // затираем все данные
        //                transaq.ExPort_File_Metod(file_tri, order); // отправляем новую транзакцию


        //                Console.WriteLine(i + " " + order);
        //                Console.WriteLine("");
        //                i++;

        //            }

        //        }
        //    }
        //}
    
}
