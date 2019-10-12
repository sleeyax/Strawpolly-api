using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace strawpoll.Models
{
    public class PollParticipant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PollParticipantID { get; private set; }

        public long PollID { get; set; }
        public Poll Poll { get; set; }
        public long MemberID { get; set; }
        public Member Member { get; set; }
    }
}