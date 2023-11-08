using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookLogic
{
    public abstract class TemplateMethod<T>
    {
        public abstract void FetchList(out List<T> o_List);
    }
}
