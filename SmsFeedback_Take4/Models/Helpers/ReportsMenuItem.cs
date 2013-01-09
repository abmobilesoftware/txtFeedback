using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmsFeedback_Take4.Models.Helpers
{
    public class ReportsMenuItem
    {
       /**
        * You should call a Action with the parameter itemID 
        */
        public int itemId;
        public string itemName;
        public Boolean leaf;
        public int parent;
        public string Action { get; set; }
        public string FriendlyName { get; set; }

        public ReportsMenuItem(int iItemId,
           string iItemName, 
           Boolean iLeaf, 
           int iParent,
           string action,
           string friendlyName)
        {
            itemId = iItemId;
            itemName = iItemName;
            leaf = iLeaf;
            parent = iParent;
            Action = action;
            FriendlyName = friendlyName;
        }
    }
}