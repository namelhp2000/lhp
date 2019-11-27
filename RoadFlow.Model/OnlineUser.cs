using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    public class OnlineUser : IEqualityComparer<OnlineUser>
    {
        // Methods
        public bool Equals(OnlineUser u1, OnlineUser u2)
        {
            return (u1.UserId == u2.UserId);
        }

        public int GetHashCode(OnlineUser u)
        {
            return u.UserId.GetHashCode();
        }
        public Guid UserId { get; set; }
        // Properties
        public string BrowseAgent { get; set; }

        public string IP { get; set; }

        public string City { get; set; }

        public DateTime LastTime { get; set; }

        public string LastUrl { get; set; }

        public DateTime LoginTime { get; set; }

        public int LoginType { get; set; }

        public string UniqueId { get; set; }

       

        public string UserName { get; set; }

        public string UserOrganize { get; set; }
    }



}
