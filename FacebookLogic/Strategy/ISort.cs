using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacebookWrapper.ObjectModel;

namespace FacebookLogic.Strategy
{
    public interface ISort
    {
        void Sort(ref List<Page> io_List);
    }
}
