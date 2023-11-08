using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookLogic.Observer
{
    public interface IObserver
    {
        void AddObserverToSubject(ISubject i_Subject);

        void RemoveObserverFromSubject(ISubject i_Subject);

        void UpdateAlbumName();

    }
}
