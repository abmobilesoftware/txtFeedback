using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models.Helpers
{
    public class ReportsMenuItem
    {
        public int itemId;
        public string itemName;
        public Boolean leaf;
        public int parent;

        public ReportsMenuItem(int iItemId, string iItemName, Boolean iLeaf, int iParent)
        {
            itemId = iItemId;
            itemName = iItemName;
            leaf = iLeaf;
            parent = iParent;
        }
    }
}