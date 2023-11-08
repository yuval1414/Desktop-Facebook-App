using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;
using FacebookLogic;

namespace BasicFacebookFeatures
{
    public partial class FormMain : Form
    {
        private AppManager m_AppManager;
        private int m_CurrentSelectedAlbumIndex = 0;

        private delegate void fetchListDelegate<T>(out List<T> o_FechList);

        public FormMain()
        {
            InitializeComponent();
            makeRoundPicture();
            FacebookWrapper.FacebookService.s_CollectionLimit = 50;
        }

        private void formApplication_Load(object sender, EventArgs e)
        {
            m_AppManager = AppManager.Instance;
            switchWindowMode(true);
        }

        private void loginBT_Click(object sender, EventArgs e)
        {
            string loginFailErrorMassage;

            try
            {
                if (m_AppManager.Login(out loginFailErrorMassage))
                {
                    switchWindowMode(false);
                    loadAllTabs();
                    initializeSubjectAndObservers();
                }
                else
                {
                    MessageBox.Show(loginFailErrorMassage, "Login Failed");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Login Failed: " + ex.Message);
            }
        }

        private void loadAllTabs()
        {
            new Thread(loadUserDataToForm).Start();
            new Thread(loadPostsTab).Start();
            new Thread(loadLikedPagesTab).Start();
            new Thread(loadAlbumsTab).Start();
            new Thread(loadEventsTab).Start();
            new Thread(loadGroupsTab).Start();
            new Thread(loadTodayPictureTab).Start();
            new Thread(loadPerformaceHighlightsTab).Start();
        }

        private void loadTab(eTabChoiceToLoad i_Choice)
        {
            switch (i_Choice)
            {
                case eTabChoiceToLoad.post:
                    loadListBoxInTab<string>(ref postsListBox, fetchAllPosts, "posts");
                    break;
                case eTabChoiceToLoad.likedPages:
                    loadListBoxInTab<Page>(ref likedPagesListBox, m_AppManager.FetchLikedPagesList, "liked pages");
                    break;
                case eTabChoiceToLoad.events:
                    loadListBoxInTab<string>(ref eventListBox, m_AppManager.FetchEventsList, "events");
                    break;
                case eTabChoiceToLoad.groups:
                    loadListBoxInTab<Group>(ref groupsListBox, m_AppManager.FetchGroupsList, "groups");
                    break;
                default:
                    break;
            }
        }

        private void initializeSubjectAndObservers()
        {
            m_AppManager.AddObserverToSubject(m_AppManager.Albums);
        }

        private void logoutBT_Click(object sender, EventArgs e)
        {
            try
            {
                m_AppManager.Logout();
                switchWindowMode(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void loadPostsTab()
        {
            postsListBox.Invoke(new Action(() => loadTab(eTabChoiceToLoad.post)));
        }

        private void postsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayPicture(ref postPictureBox, ref postsListBox, m_AppManager.FetchPostPictureByIndex, "post");

            try
            {
                Post selected = m_AppManager.FetchPostByIndex(postsListBox.SelectedIndex);
                commentsListBox.Items.Clear();

                if (selected.Comments.Count() == 0)
                {
                    commentsListBox.Items.Add("no comments");
                }
                else
                {
                    foreach (Comment comment in selected.Comments)
                    {
                        commentsListBox.Items.Add(comment);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading comments: " + ex.Message);
            }
        }

        private void loadLikedPagesTab()
        {
            postsListBox.Invoke(new Action(() => loadTab(eTabChoiceToLoad.likedPages)));
        }

        private void likedPages_SelectedIndexChange(object sender, EventArgs e)
        {
            displayPicture(ref pagePictureBox, ref likedPagesListBox, m_AppManager.FetchPagePictureByIndex, "liked page");
        }

        private void loadAlbumsTab()
        {
            try
            {
                var allAlbums = m_AppManager.Albums.Albums;

                if (!albumsListBox.InvokeRequired)
                {
                    albumBindingSource.DataSource = allAlbums;
                }
                else
                {
                    albumsListBox.Invoke(new Action(() => albumBindingSource.DataSource = allAlbums));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error albums load Failed: " + ex.Message);
            }
        }

        private void loadEventsTab()
        {
            postsListBox.Invoke(new Action(() => loadTab(eTabChoiceToLoad.events)));
        }

        private void events_SelectedIndexChange(object sender, EventArgs e)
        {
            displayPicture(ref eventPictureBox, ref eventListBox, m_AppManager.FetchEventPictureByIndex, "event");
        }

        private void loadGroupsTab()
        {
            postsListBox.Invoke(new Action(() => loadTab(eTabChoiceToLoad.groups)));
        }

        private void groups_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayGroupInfo();
        }

        private void switchWindowMode(bool i_isLoginMode)
        {
            LoginBT.Enabled = i_isLoginMode;
            LogoutBT.Enabled = !i_isLoginMode;
            contolTAB.Visible = !i_isLoginMode;
            toLoginMessageTextBox.Visible = i_isLoginMode;
        }

        private void makeRoundPicture()
        {
            System.Drawing.Drawing2D.GraphicsPath graphicShapeOfObject = new System.Drawing.Drawing2D.GraphicsPath();
            Region regionOfPicture;

            graphicShapeOfObject.AddEllipse(0, 0, pictureBoxProfile.Width, pictureBoxProfile.Height);
            regionOfPicture = new Region(graphicShapeOfObject);
            pictureBoxProfile.Region = regionOfPicture;
        }

        private void loadUserDataLB()
        {
            profileNameLB.Text = m_AppManager.FetchUserName();
            numberOfFriendsLB.Text = m_AppManager.FetchNumberOfFriends();
        }

        private void loadUserDataToForm()
        {
            try
            {
                pictureBoxProfile.LoadAsync(m_AppManager.FetchUserProfilePicture());
                this.Invoke(new Action(() => loadUserDataLB()));
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error loading User data: " + ex.Message);
            }
        }

        private void loadListBoxInTab<T>(ref ListBox io_ListBox, fetchListDelegate<T> fetchList, string i_TabName)
        {
            List<T> list;
            io_ListBox.Items.Clear();
            try
            {
                fetchList(out list);
                if (list == null)
                {
                    throw new Exception("could not load " + i_TabName);
                }

                if (list.Count == 0)
                {
                    io_ListBox.Items.Add("no " + i_TabName + " available");
                }
                else
                {
                    foreach (T contentType in list)
                    {
                        io_ListBox.Items.Add(contentType);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading " + i_TabName + " : " + ex.Message);
            }
        }

        private void loadTodayPictureTab()
        {
            try
            {
                string albumName;
                todayPictureBox.ImageLocation = m_AppManager.FetchRandomPictureFromAnAlbum(out albumName);
                albumNameLB.Text = albumName;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Today's picture: " + ex.Message);
            }
        }

        private void loadPerformaceHighlightsTab()
        {
            try
            {
                m_AppManager.PerformanceHighlightsBuilder();
                PerformanceHighlights performanceHighlightsData = m_AppManager.PerformanceHighlights;

                popularHourResultLB.Text = performanceHighlightsData.Hour;
                popularMonthResultLB.Text = performanceHighlightsData.Month;
                activeYearResultLB.Text = performanceHighlightsData.Year;
                maxPhotosAlbumResultLB.Text = performanceHighlightsData.AlbumName;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Performace highlights: " + ex.Message);
            }
        }

        private void displayPicture(ref PictureBox io_PictureBox, ref ListBox io_ListBox, Func<int, string> fetchPictureByIndex, string i_TabName)
        {
            try
            {
                io_PictureBox.ImageLocation = fetchPictureByIndex(io_ListBox.SelectedIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading " + i_TabName + " picture: " + ex.Message);
            }
        }

        private void displayGroupInfo()
        {
            displayPicture(ref groupPictureBox, ref groupsListBox, m_AppManager.FetchGroupPictureByIndex, "group");
            displayGroupDescription();
        }

        private void displayGroupDescription()
        {
            try
            {
                descriptionTextBox.Text = m_AppManager.FetchGroupDescriptionByIndex(groupsListBox.SelectedIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading group description: " + ex.Message);
            }
        }

        private void allPostsRBT_CheckedChanged(object sender, EventArgs e)
        {
            if(allPostsRBT.Checked == true)
            {
                loadListBoxInTab<string>(ref postsListBox, fetchAllPosts, "posts");
            }
        }

        private void photosRBT_CheckedChanged(object sender, EventArgs e)
        {
            if (photosRBT.Checked == true)
            {
                loadListBoxInTab<string>(ref postsListBox, fetchPostsOnlyWithPhotos, "posts");
            }
        }

        private void descripstionRBT_CheckedChanged(object sender, EventArgs e)
        {
            if (descripstionRBT.Checked == true)
            {
                loadListBoxInTab<string>(ref postsListBox, fetchPostsOnlyWithDescription, "posts");
            }
        }

        private void fetchAllPosts(out List<string> o_PostsListAllName)
        {
            m_AppManager.FetchPostsNameList(out o_PostsListAllName, FacebookLogic.ePostsToPresent.allPosts);
        }

        private void fetchPostsOnlyWithPhotos(out List<string> o_PostsListAllName)
        {
            m_AppManager.FetchPostsNameList(out o_PostsListAllName, FacebookLogic.ePostsToPresent.onlyWithPhotos);
        }

        private void fetchPostsOnlyWithDescription(out List<string> o_PostsListAllName)
        {
            m_AppManager.FetchPostsNameList(out o_PostsListAllName, FacebookLogic.ePostsToPresent.onlyWithDescription);
        }

        private void ascendingRBT_CheckedChanged(object sender, EventArgs e)
        {
            if (ascendingRBT.Checked == true)
            {
                loadListBoxInTab<Page>(ref likedPagesListBox, m_AppManager.FetchAscendingLikedPagesList, "liked pages");
            }
        }

        private void descendingRBT_CheckedChanged(object sender, EventArgs e)
        {
            if (descendingRBT.Checked == true)
            {
                loadListBoxInTab<Page>(ref likedPagesListBox, m_AppManager.FetchDescendingLikedPagesList, "liked pages");
            }
        }

        private void originalRBT_CheckedChanged(object sender, EventArgs e)
        {
            if (originalRBT.Checked == true)
            {
                loadListBoxInTab<Page>(ref likedPagesListBox, m_AppManager.FetchLikedPagesList, "liked pages");
            }
        }

        private void performanceHighlightsTab_Enter(object sender, EventArgs e)
        {
            if(maxPhotosAlbumResultLB.Text != null)
            {
                if (maxPhotosAlbumResultLB.Text != m_AppManager.PerformanceHighlights.AlbumName)
                {
                    maxPhotosAlbumResultLB.Text = m_AppManager.PerformanceHighlights.AlbumName;
                }
            }
        }

        private void RenameAlbumField_Leave(object sender, EventArgs e) // CHECK!!!
        {
            Album selectedAlbum = albumsListBox.Items[m_CurrentSelectedAlbumIndex] as Album;

            if (selectedAlbum.Name != nameTextBox1.Text)
            {
                m_AppManager.albumNameChanged(m_CurrentSelectedAlbumIndex, nameTextBox1.Text);
            }
        }

        private void saveAlbumIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_CurrentSelectedAlbumIndex = albumsListBox.SelectedIndex;
        }
    }
}
