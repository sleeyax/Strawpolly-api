using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace strawpoll.Models
{
    public class PollAnswer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PollAnswerID { get; private set; }
        public string Answer { get; set; }

        public long PollID { get; set; }
        public Poll Poll { get; set; }
        public List<PollVote> Votes { get; set; }
    }
}