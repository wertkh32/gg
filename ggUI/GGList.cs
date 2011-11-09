using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace gg
{
    /// <summary>
    /// Defines the To Do List object
    /// <author>Yeo Jie Shun</author>
    /// <author>Peng ZiWei</author>
    /// <author>Sing Keng Hua</author>
    /// </summary>
    public class GGList
    {
        private List<GGItem> list;
        private List<string> tags;
        private List<int> tagCount;
        private List<GGItem> deletedList;

        public GGList()
        {
            list = new List<GGItem>();
            tags = new List<string>();
            tagCount = new List<int>();
            deletedList = new List<GGItem>();
        }

        private void UpdateTagListForNewItem(string TagOfNewItem)
        {
            if (!TagExists(TagOfNewItem))
            {
                tags.Add(TagOfNewItem);
                tagCount.Add(1);
            }
            else
            {
                int tagIndex = GetTagIndex(TagOfNewItem);
                Debug.WriteLineIf(tagIndex < 0, "GGList.UpdateTagListForDeletedItem: tagIndex < 0");
                Debug.Assert(tagIndex >= 0);
                tagCount[tagIndex]++;

            }
        }

        private void UpdateTagListForDeletedItem(string TagOfDeletedItem)
        {
            int tagIndex = -1;
            tagIndex = GetTagIndex(TagOfDeletedItem);

            Debug.WriteLineIf(tagIndex < 0, "GGList.UpdateTagListForDeletedItem: tagIndex < 0");
            Debug.Assert(tagIndex >= 0);

            if (tagCount[tagIndex] == 1)
            {//remove tag and tag count
                tagCount.RemoveAt(tagIndex);
                tags.RemoveAt(tagIndex);
            }
            else
            {//update tag count
                tagCount[tagIndex]--;
            }
        }

        public int GetTagIndex(string TagToFind)
        {
            return tags.FindIndex(delegate(string Tag)
            { 
                return Tag.CompareTo(TagToFind) == 0; 
            });
        }

        public bool TagExists(string TagToFind)
        {
            return GetTagIndex(TagToFind) != -1;
        }

        public int GetTagCount(string TagToFind)
        {
            if (TagExists(TagToFind))
                return tagCount[GetTagIndex(TagToFind)];
            else return -1;
        }

        public List<string> GetTagList()
        {
            return tags;
        }

        public List<int> GetTagCountList()
        {
            return tagCount;
        }

        public void AddGGItem(GGItem ggItem)
        {
            UpdateTagListForNewItem(ggItem.GetTag());
            list.Add(ggItem);
        }

        public void RemoveGGItem(GGItem ggItem)
        {
            UpdateTagListForDeletedItem(ggItem.GetTag());
            list.Remove(ggItem);
            deletedList.Add(ggItem);
        }

        public int IndexOfGGItem(GGItem item)
        {
            return list.IndexOf(item);
        }

        public void InsertGGItem(int index, GGItem item)
        {
            UpdateTagListForNewItem(item.GetTag());
            list.Insert(index, item);
        }

        public GGItem GetGGItemAt(int index)
        {
            Debug.WriteLineIf(index >= list.Count || index < 0, "GGItem.GetGGItemAt(): Index out of bound");
            Debug.Assert(index < list.Count && index >= 0);
            return list[index];
        }

        public void SetGGItemAt(int index, GGItem newItem)
        {
            Debug.WriteLineIf(index >= list.Count || index < 0, "GGItem.SetGGItemAt(): Index out of bound");
            Debug.Assert(index < list.Count && index >= 0);
            list[index] = newItem;
        }

        public void RemoveGGItemAt(int index)
        {
            Debug.WriteLineIf(index >= list.Count || index < 0, "GGItem.RemoveGGItemAt(): Index out of bound");
            Debug.Assert(index < list.Count && index >= 0);
            UpdateTagListForDeletedItem(list.ElementAt(index).GetTag());
            deletedList.Add(list[index]);
            list.RemoveAt(index);
        }

        public void SortGGList()
        {
            list.Sort();
        }

        public int CountGGList()
        {
            return list.Count;
        }

        public List<GGItem> GetInnerList()
        {
            return this.list;
        }

        public void SetInnerList(List<GGItem> list)
        {
            this.list = list;
            for (int i = 0; i < list.Count; i++)
            {
                UpdateTagListForNewItem(list[i].GetTag());
            }
        }

        public List<GGItem> GetDeletedList()
        {
            return this.deletedList;
        }

        public void SetDeletedList(List<GGItem> deletedList)
        {
            this.deletedList = deletedList;
        }

        public void ClearGGList()
        {
            for (int i = 0; i < list.Count; i++)
            {
                deletedList.Add(list[i]);
            }
            list.Clear();
        }

        public bool ContainsGGItem(GGItem item)
        {
            return list.Contains(item);
        }

        public void CopyToArray(GGItem[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }
          
        public override bool Equals(object obj)
        {
            GGList otherGgList = obj as GGList;
            bool isNotGGListObject = otherGgList == null;
            if (isNotGGListObject)
            {
                Debug.WriteLine("GGList.Equals: Object passed in is not a GGList object", "error");
                return false;
            }
            if (this.CountGGList() != otherGgList.CountGGList())
                return false;

            for (int i = 0; i < this.CountGGList(); i++)
            {
                GGItem ggItem = this.GetGGItemAt(i);
                GGItem otherGgItem = ((GGList)obj).GetGGItemAt(i);
                if (!ggItem.Equals(otherGgItem))
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            string result = string.Empty;
            for (int i = 0; i < this.GetInnerList().Count; i++)
            {
                if (i == 0)
                    result = this.GetGGItemAt(i).ToString();
                else
                    result = result + ", " + this.GetGGItemAt(i).ToString();
            }
            return result;
        }
    }
}
