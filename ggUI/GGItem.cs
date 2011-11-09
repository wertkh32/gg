using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace gg
{
    public class GGItem : IComparable<GGItem>
    {
        private string description;
        private DateTime endDate;
        private string tag;
        // last modified time at local
        private DateTime lastModifiedTime;
        // absolute url of the coresponding event on Google Calendar; empty if not synced
        private string eventAbsoluteUrl;
        private string path;

        public static DateTime DEFAULT_ENDDATE = DateTime.MinValue;

        public GGItem()
        {
           
        }

        public GGItem(string description)
            : this(description,
                DEFAULT_ENDDATE, "misc", DateTime.Now, string.Empty, string.Empty) { }

        public GGItem(string description, DateTime endDate) 
            : this(description, 
                endDate, "misc", DateTime.Now, string.Empty, string.Empty) { }

        public GGItem(string description, DateTime endDate, string tag)
            : this(description, endDate, tag, DateTime.Now, string.Empty, string.Empty) { }

        public GGItem(string description, DateTime endDate, string tag, DateTime lastModifiedTime, string url)
            : this(description, endDate, tag, lastModifiedTime, url, string.Empty) { }

        public GGItem(string description, DateTime endDate, string tag, DateTime lastModifiedTime, string url, string path)
        {
            this.description = description;
            this.endDate = endDate;
            this.tag = tag;
            this.lastModifiedTime = lastModifiedTime;
            this.eventAbsoluteUrl = url;
            this.path = path;
        }

        public string GetDescription()
        {
            return this.description;
        }

        public void SetDescription(string description)
        {
            this.description = description;
            lastModifiedTime = DateTime.Now;
        }

        public DateTime GetEndDate()
        {
            return this.endDate;
        }

        public void SetEndDate(DateTime endDate)
        {
            this.endDate = endDate;
            lastModifiedTime = DateTime.Now;
        }

        public string GetTag()
        {
            return this.tag;
        }

        public void SetTag(string tag)
        {
            this.tag = tag;
            lastModifiedTime = DateTime.Now;
        }

        public DateTime GetLastModifiedTime()
        {
            return this.lastModifiedTime;
        }

        public void SetLastModifiedTime(DateTime lastModifiedTime)
        {
            this.lastModifiedTime = lastModifiedTime;
        }

        public string GetEventAbsoluteUrl()
        {
            return this.eventAbsoluteUrl;
        }

        public void SetEventAbsoluteUrl(string eventAbsoluteUrl)
        {
            this.eventAbsoluteUrl = eventAbsoluteUrl;
        }

        public string GetPath()
        {
            return this.path;
        }

        public void SetPath(string newPath)
        {
            this.path = newPath;
        }

        public override string ToString()
        {
            bool tagIsMisc = this.tag.Equals("misc");
            bool dateIsMinValue = endDate == (DateTime.MinValue);
            bool pathIsEmpty = this.path.Equals("");
            string endDateToShow = string.Empty;
            string endDateWithMonthNames = this.endDate.ToString("d MMM yy h:mm tt");

            if (endDate.Date == DateTime.Now.Date)
                endDateToShow = "Today " + endDate.ToString("h:mm tt");
            else if (endDate.Date == DateTime.Now.AddDays(1).Date)
                endDateToShow = "Tomorrow " + endDate.ToString("h:mm tt");
            else
                endDateToShow = endDateWithMonthNames;

            return string.Format("{0}{3}|{1}|{2}",
                this.description,
                tagIsMisc ? "" : "#" + this.tag,
                dateIsMinValue ? "" : "" + endDateToShow,
                pathIsEmpty ? "" : "*");
        }

        public override bool Equals(object obj)
        {
            GGItem otherGgItem = obj as GGItem;
            bool isNotGGItemObject = otherGgItem == null;
            if (isNotGGItemObject)
            {
                Debug.WriteLine("GGItem.Equals: Object passed in is not a GGItem object", "error");
                return false;
            }
            return this.GetDescription().Equals(otherGgItem.GetDescription()) &&
                        this.GetEndDate().Equals(otherGgItem.GetEndDate()) &&
                        this.GetTag().Equals(otherGgItem.GetTag());
        }

        #region IComparable<GGItem> Members

        public int CompareTo(GGItem otherItem)
        {
            if((this.endDate < otherItem.endDate))
            {
                return -1;
            }
            else if (this.endDate > otherItem.endDate)
            {
                return 1;
            }
            else // they are equal
            {
                return this.description.CompareTo(otherItem.description);
            }
        }

        #endregion
    }

}
