using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;
using FacebookLogic.Strategy;

namespace FacebookLogic
{
    internal class LikedPagesData : TemplateMethod<Page>
    {
        private FacebookObjectCollection<Page> m_LikedPages;

        public LikedPagesData(FacebookObjectCollection<Page> i_LikedPages)
        {
            m_LikedPages = i_LikedPages;
        }

        public override void FetchList(out List<Page> o_List)
        {
            o_List = new List<Page>();

            foreach (Page page in m_LikedPages)
            {
                if (page != null)
                {
                    o_List.Add(page);
                }
            }
        }

        public void FetchAscendingLikedPagesList(out List<Page> o_LikedPagesList)
        {
            FetchList(out o_LikedPagesList);
            ISort ascendingOrder = new AscendingOrder();
            ascendingOrder.Sort(ref o_LikedPagesList);
        }

        public void FetchDescendingLikedPagesList(out List<Page> o_LikedPagesList)
        {
            FetchList(out o_LikedPagesList);
            ISort descendingOrder = new DescendingOrder();
            descendingOrder.Sort(ref o_LikedPagesList);
        }

        public string FetchPagePictureByIndex(int i_Index)
        {
            return m_LikedPages[i_Index].PictureNormalURL;
        }
    }
}
