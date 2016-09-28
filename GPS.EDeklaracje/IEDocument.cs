using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.EDeklaracje
{
    public interface IEMasterDocument
    {
        object GetEDocument(decimal? amount);
    }
}
