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
    public class AlbumsData : ISubject
    {
        private FacebookObjectCollection<Album> m_Albums;
        private List<IObserver> m_Observers = new List<IObserver>();

        public AlbumsData(FacebookObjectCollection<Album> i_Albums)
        {
            m_Albums = i_Albums;
        }

        public FacebookObjectCollection<Album> Albums
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

        public string GetAlbumNameByIndex(int i_Index)
        {
            string albumName = null;

            if (m_Albums.Count > i_Index)
            {
                albumName = m_Albums[i_Index].Name;
            }

            return albumName;
        }

        public void SetAlbumNameByIndex(int i_Index, string i_NewName)
        {
            if (m_Albums.Count > i_Index)
            {
                m_Albums[i_Index].Name = i_NewName;
            }
        }

        public string FetchRandomPictureFromAnAlbum(out string albumName)
        {
            Random random = new Random();
            int numberOfAlbums = m_Albums.Count;
            int randomAlbum = random.Next(numberOfAlbums);
            int randomPicture = random.Next(m_Albums[randomAlbum].Photos.Count);

            albumName = m_Albums[randomAlbum].Name;

            return m_Albums[randomAlbum].Photos[randomPicture].PictureNormalURL;
        }

        public string FetchAlbumWithMaxPictures(out int o_AlbumIndex)
        {
            string albumNameWithMaxPictures = " ";
            int maxPictures = 0;
            o_AlbumIndex = 0;

            for (int i = 0; i < m_Albums.Count; i++)
            {
                if (m_Albums[i].Photos.Count() > maxPictures)
                {
                    maxPictures = m_Albums[i].Photos.Count();
                    albumNameWithMaxPictures = m_Albums[i].Name;
                    o_AlbumIndex = i;
                }
            }

            return albumNameWithMaxPictures;
        }

        public void Attach(IObserver i_Observer)
        {
            m_Observers.Add(i_Observer);
        }

        public void Detach(IObserver i_Observer)
        {
            m_Observers.Remove(i_Observer);
        }

        public void Notify()
        {
            foreach (IObserver observer in m_Observers)
            {
                observer.UpdateAlbumName();
            }
        }
    }
}
