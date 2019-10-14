using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace strawpoll.Models
{
    public class Poll
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PollID { get; private set; }
        public string Name { get; set; }

        // creator of the Poll
        public long? MemberID { get; set; }
        public Member Creator { get; set; }

        public List<PollAnswer> Answers { get; set; }
        public List<PollParticipant> Participants { get; set; }
    }
}