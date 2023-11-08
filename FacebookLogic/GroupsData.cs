using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;

namespace FacebookLogic
{
    internal class GroupsData : TemplateMethod<Group>
    {
        private FacebookObjectCollection<Group> m_Groups;

        public GroupsData(FacebookObjectCollection<Group> i_Groups)
        {
            m_Groups = i_Groups;
        }

        public override void FetchList(out List<Group> o_List)
        {
            o_List = new List<Group>();

            foreach (Group group in m_Groups)
            {
                if (group != null)
                {
                    o_List.Add(group);
                }
            }
        }

        public string FetchGroupPictureByIndex(int i_Index)
        {
            return m_Groups[i_Index].PictureNormalURL;
        }

        public string FetchGroupDescriptionByIndex(int i_Index)
        {
            return m_Groups[i_Index].Description;
        }
    }
}
