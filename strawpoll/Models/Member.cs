using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace strawpoll.Models
{
    public class Member
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MemberID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        // NotMapped = field will not be stored as a column in the DB
        [NotMapped]
        public string Token { get; set; }

        public List<Poll> Polls { get; set; }
        public List<Friend> Friends { get; set; }
        // navigation property of Friend.MemberWhoModified
        public List<Friend> FriendsModified { get; set; }
    }
}