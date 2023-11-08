using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;
using FacebookLogic.Observer;

namespace FacebookLogic
{
    public sealed class AppManager : IObserver
    {
        private static AppManager s_Instance = null;
        private static object s_LockObject = new Object();
        private LoginResult m_LoginResult;
        private PostsData m_Posts;
        private LikedPagesData m_Pages;
        private AlbumsData m_Albums;
        private EventsData m_Events;
        private GroupsData m_Groups;
        private PerformanceHighlights m_PerformanceHighlights;

        private AppManager()
        {
            m_LoginResult = new LoginResult();
        }

        public static AppManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    lock (s_LockObject)
                    {
                        if (s_Instance == null)
                        {
                            s_Instance = new AppManager();
                        }
                    }
                }

                return s_Instance;
            }
        }

        public LoginResult LoginResult
        {
            get
            {
                return m_LoginResult;
            }
        }

        public AlbumsData Albums
        {
            get
            {
                return m_Albums;
            }

            set
            {
                m_Albums = value;
            }
        }

        public PerformanceHighlights PerformanceHighlights
        {
            get
            {
                return m_PerformanceHighlights;
            }
        }

        public eTimeUnits eTimeUnits
        {
            get => default;
            set
            {
            }
        }

        public bool Login(out string o_LoginFailedErrorMessage)
        {
            bool loggedInSucceeded = LoginToFacebook(out o_LoginFailedErrorMessage);

            if (loggedInSucceeded)
            {
                initializeMembers();
            }

            return loggedInSucceeded;
        }

        public bool LoginToFacebook(out string o_LoginFailedErrorMessage)
        {
            m_LoginResult = FacebookService.Login(
                "2015024608834507",
                "email",
                "public_profile",
                "user_age_range",
                "user_birthday",
                "user_events",
                "user_friends",
                "user_gender",
                "user_hometown",
                "user_likes",
                "user_link",
                "user_location",
                "user_photos",
                "user_posts",
                "groups_access_member_info",
                "user_videos",
                "instagram_manage_comments");

            o_LoginFailedErrorMessage = m_LoginResult.ErrorMessage;

            return !string.IsNullOrEmpty(m_LoginResult.AccessToken);
        }

        private void initializeMembers()
        {
            m_Posts = new PostsData(m_LoginResult.LoggedInUser.Posts);
            m_Pages = new LikedPagesData(m_LoginResult.LoggedInUser.LikedPages);
            m_Albums = new AlbumsData(m_LoginResult.LoggedInUser.Albums);
            m_Events = new EventsData(m_LoginResult.LoggedInUser.Events);
            m_Groups = new GroupsData(m_LoginResult.LoggedInUser.Groups);
            m_PerformanceHighlights = new PerformanceHighlights();
        }

        public string FetchUserProfilePicture()
        {
            return m_LoginResult.LoggedInUser.PictureNormalURL;
        }

        public string FetchUserName()
        {
            return m_LoginResult.LoggedInUser.Name;
        }

        public string FetchNumberOfFriends()
        {
            string numberOfFriends;

            try
            {
                numberOfFriends = m_LoginResult.LoggedInUser.FriendLists.Count.ToString();
            }
            catch (Facebook.FacebookOAuthException)
            {
                numberOfFriends = "unavailble";
            }

            return numberOfFriends;
        }

        public void FetchPostsNameList(out List<string> o_PostsList, ePostsToPresent i_TypeToPresent)
        {
            m_Posts.FetchPostsNameList(out o_PostsList, i_TypeToPresent);
        }

        public Post FetchPostByIndex(int i_Index)
        {
            return m_Posts.FetchPostByIndex(i_Index);
        }

        public string FetchPostPictureByIndex(int i_Index)
        {
            return m_Posts.FetchPostPictureByIndex(i_Index);
        }

        public void FetchAscendingLikedPagesList(out List<Page> o_LikedPagesList)
        {
            m_Pages.FetchAscendingLikedPagesList(out o_LikedPagesList);
        }

        public void FetchDescendingLikedPagesList(out List<Page> o_LikedPagesList)
        {
            m_Pages.FetchDescendingLikedPagesList(out o_LikedPagesList);
        }

        public string FetchPagePictureByIndex(int i_Index)
        {
            return m_Pages.FetchPagePictureByIndex(i_Index);
        }

        public string FetchRandomPictureFromAnAlbum(out string albumName)
        {
            return m_Albums.FetchRandomPictureFromAnAlbum(out albumName);
        }

        public void FetchLikedPagesList(out List<Page> o_LikedPagesList)
        {
            m_Pages.FetchList(out o_LikedPagesList);
        }

        public void FetchEventsList(out List<string> o_EventsList)
        {
            m_Events.FetchList(out o_EventsList);
        }

        public void FetchGroupsList(out List<Group> o_groupsList)
        {
            m_Groups.FetchList(out o_groupsList);
        }

        public string FetchEventPictureByIndex(int i_Index)
        {
            return m_Events.FetchEventPictureByIndex(i_Index);
        }

        public string FetchGroupPictureByIndex(int i_Index)
        {
            return m_Groups.FetchGroupPictureByIndex(i_Index);
        }

        public string FetchGroupDescriptionByIndex(int i_Index)
        {
            return m_Groups.FetchGroupDescriptionByIndex(i_Index);
        }

        public void PerformanceHighlightsBuilder()
        {
            int albumIndex;

            if (m_PerformanceHighlights.isNull())
            {
                m_PerformanceHighlights.Hour = m_Posts.FetchPostsPopularHour();
                m_PerformanceHighlights.Month = m_Posts.FetchPostsPopularMonth();
                m_PerformanceHighlights.Year = m_Posts.FetchPostsActiveYear();
                m_PerformanceHighlights.AlbumName = m_Albums.FetchAlbumWithMaxPictures(out albumIndex);
                m_PerformanceHighlights.AlbumIndex = albumIndex;
            }
        }

        public void albumNameChanged(int i_AlbumIndex, string i_NewName)
        {
            m_Albums.SetAlbumNameByIndex(i_AlbumIndex, i_NewName);
            m_Albums.Notify();
        }

        public void AddObserverToSubject(ISubject i_Subject)
        {
            i_Subject.Attach(this);

        }

        public void RemoveObserverFromSubject(ISubject i_Subject)
        {
            i_Subject.Detach(this);
        }


        public void UpdateAlbumName()
        {
            string albumName = m_Albums.GetAlbumNameByIndex(m_PerformanceHighlights.AlbumIndex);
            if (albumName != m_PerformanceHighlights.AlbumName)
            {
                m_PerformanceHighlights.AlbumName = albumName;
            }
        }

        public void Logout()
        {
            FacebookService.LogoutWithUI();
        }
    }
}
