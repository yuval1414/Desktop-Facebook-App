using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;

namespace FacebookLogic
{
    internal class EventsData : TemplateMethod<string>
    {
        private FacebookObjectCollection<Event> m_Events;

        public EventsData(FacebookObjectCollection<Event> i_Events)
        {
            m_Events = i_Events;
        }

        public override void FetchList(out List<string> o_List)
        {
            o_List = new List<string>();

            foreach (Event eventName in m_Events)
            {
                if (eventName != null)
                {
                    o_List.Add(eventName.Name);
                }
            }
        }

        public string FetchEventPictureByIndex(int i_Index)
        {
            return m_Events[i_Index].PictureNormalURL;
        }
    }
}
