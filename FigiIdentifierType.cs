using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naklih.Com.FigiClassLib
{
    public enum FigiIdentifierType
    {
        ID_ISIN, //ISIN
        ID_BB_UNIQUE, //    Unique Bloomberg Identifier
        ID_SEDOL, //    Sedol Number
        ID_COMMON, //   Common Code
        ID_WERTPAPIER, //   Wertpapierkennnummer/WKN
        ID_CUSIP, //    CUSIP
        ID_BB, //   ID BB
        ID_ITALY, //    Italian Identifier Number
        ID_EXCH_SYMBOL, //  Local Exchange Security Symbol
        ID_FULL_EXCHANGE_SYMBOL, // Full Exchange Symbol
        COMPOSITE_ID_BB_GLOBAL, //  Composite Financial Instrument Global Identifier
        ID_BB_GLOBAL_SHARE_CLASS_LEVEL, //  Share Class Financial Instrument Global Identifier
        ID_BB_SEC_NUM_DES, //   Security ID Number Description
        ID_BB_GLOBAL, //    Financial Instrument Global Identifier (FIGI)
        TICKER, //  Ticker
        ID_CUSIP_8_CHR, //  CUSIP (8 Characters Only)
        OCC_SYMBOL, //  OCC Symbol
        UNIQUE_ID_FUT_OPT, //   Unique Identifier for Future Option
        OPRA_SYMBOL, // OPRA Symbol
        TRADING_SYSTEM_IDENTIFIER //   Trading System Identifier
    }
}
