using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace strawpoll.Models
{
    public class PollVote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PollVoteID { get; private set; }

        public long PollAnswerID { get; set; }
        public PollAnswer Answer { get; set; }
        public long MemberID { get; set; }
        public Member Member { get; set; }
    }
}