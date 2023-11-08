using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;

namespace FacebookLogic
{
    internal class PostsData
    {
        private FacebookObjectCollection<Post> m_Posts;

        public PostsData(FacebookObjectCollection<Post> i_Posts)
        {
            m_Posts = i_Posts;
        }

        public void FetchPostsNameList(out List<string> o_PostsList, ePostsToPresent i_TypeToPresent)
        {
            o_PostsList = new List<string>();

            foreach (Post post in m_Posts)
            {
                if ((post.Message != null) &&
                    ((i_TypeToPresent == ePostsToPresent.allPosts) || (i_TypeToPresent == ePostsToPresent.onlyWithDescription)))
                {
                    o_PostsList.Add(post.Message);
                }
                else if ((post.Caption != null) &&
                        ((i_TypeToPresent == ePostsToPresent.allPosts) || (i_TypeToPresent == ePostsToPresent.onlyWithDescription)))
                {
                    o_PostsList.Add(post.Caption);
                }
                else if ((post.Message == null) &&
                        (post.Caption == null) &&
                        ((i_TypeToPresent == ePostsToPresent.allPosts) || (i_TypeToPresent == ePostsToPresent.onlyWithPhotos)))
                {
                    o_PostsList.Add(string.Format("[{0}]", post.Type));
                }
            }

            if (o_PostsList.Count == 0)
            {
                o_PostsList.Add("No Posts to retrieve");
            }
        }

        public Post FetchPostByIndex(int i_Index)
        {
            return m_Posts[i_Index];
        }

        public string FetchPostPictureByIndex(int i_Index)
        {
            return m_Posts[i_Index].PictureURL;
        }

        private void fetchPostsList(out List<Post> o_PostsList)
        {
            o_PostsList = new List<Post>();

            foreach (Post post in m_Posts)
            {
                if (post != null)
                {
                    o_PostsList.Add(post);
                }
            }
        }

        public string FetchPostsPopularHour()
        {
            int popularHour = fetchPostPreformance(eTimeUnits.numberOfHours, eTimeUnits.hour);

            return string.Format(popularHour.ToString() + ":00");
        }

        public string FetchPostsPopularMonth()
        {
            int popularMonth = fetchPostPreformance(eTimeUnits.numberOfMonths, eTimeUnits.month);

            return new DateTime(2023, popularMonth, 1).ToString("MMMM");
        }

        private int fetchPostPreformance(eTimeUnits i_ArraySize, eTimeUnits i_Unit)
        {
            List<Post> postList;
            int[] arrayOfTimeUnit = new int[(int)i_ArraySize];
            int maxPostsPostedInTimeUnit;
            int popularTimeUnit = 1, valueUnitTime;

            fetchPostsList(out postList);

            foreach (Post post in postList)
            {
                valueUnitTime = (i_Unit == eTimeUnits.hour) ? post.CreatedTime.Value.Hour : post.CreatedTime.Value.Month - 1;
                arrayOfTimeUnit[valueUnitTime]++;
            }

            maxPostsPostedInTimeUnit = arrayOfTimeUnit.Max();

            while (arrayOfTimeUnit[popularTimeUnit - 1] != maxPostsPostedInTimeUnit)
            {
                popularTimeUnit++;
            }

            return (i_Unit == eTimeUnits.hour) ? --popularTimeUnit : popularTimeUnit;
        }

        public string FetchPostsActiveYear()
        {
            List<Post> postList;
            Dictionary<int, int> yearsOfPosts = new Dictionary<int, int>();

            fetchPostsList(out postList);

            foreach (Post post in postList)
            {
                if (yearsOfPosts.ContainsKey(post.CreatedTime.Value.Year))
                {
                    yearsOfPosts[post.CreatedTime.Value.Year]++;
                }
                else
                {
                    yearsOfPosts.Add(post.CreatedTime.Value.Year, 1);
                }
            }

            return yearsOfPosts.Aggregate((x, y) => x.Value > y.Value ? x : y).Key.ToString();
        }
    }
}
