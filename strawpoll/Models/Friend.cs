using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace strawpoll.Models
{
    public enum FriendStatus
    {
        Pending,
        Accepted,
        Declined,
        Blocked
    }

    public class Friend
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long FriendID { get; set; }
        public FriendStatus FriendStatus { get; set; }

        // member 1
        public long MemberID { get; set; }
        [ForeignKey("MemberID")]
        public Member Member { get; set; }
        // member 2
        // NOTE: this is set to 'long?' to avoid 'multiple cascade paths' error as a (temporary) solution
        public long? MemberFriendID { get; set; }
        [ForeignKey("MemberFriendID")]
        [InverseProperty("Friends")]
        public Member MemberFriend { get; set; }
    }
}