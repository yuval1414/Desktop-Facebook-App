using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacebookWrapper.ObjectModel;

namespace FacebookLogic.Strategy
{
    public class DescendingOrder : ISort
    {
        public void Sort(ref List<Page> io_List)
        {
            io_List = io_List.OrderBy(page => page.Name).ToList();
            io_List.Reverse();
        }
    }
}
