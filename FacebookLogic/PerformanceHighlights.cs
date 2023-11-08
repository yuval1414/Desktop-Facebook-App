using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookLogic
{
    public class PerformanceHighlights
    {
        private string m_Hour = null;
        private string m_Month = null;
        private string m_Year = null;
        private string m_AlbumName = null;
        private int m_AlbumIndexInList;

        public string Hour
        {
            get
            {
                return m_Hour;
            }

            set
            {
                m_Hour = value;
            }
        }

        public string Month
        {
            get
            {
                return m_Month;
            }

            set
            {
                m_Month = value;
            }
        }

        public string Year
        {
            get
            {
                return m_Year;
            }

            set
            {
                m_Year = value;
            }
        }

        public string AlbumName
        {
            get
            {
                return m_AlbumName;
            }

            set
            {
                m_AlbumName = value;
            }
        }

        public int AlbumIndex
        {
            get
            {
                return m_AlbumIndexInList;
            }

            set
            {
                m_AlbumIndexInList = value;
            }
        }

        public bool isNull()
        {
            return m_Hour == null;
        }
    }
}
