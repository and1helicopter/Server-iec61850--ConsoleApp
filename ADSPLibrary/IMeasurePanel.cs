using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADSPLibrary
{
    public interface IMeasurePanel
    {
        int NeedWidth{ get; set;}
        int NeedHeight { get; set; }
        bool ResizePossible { get; set; }
        void InitModBusUnits();
    }
}
