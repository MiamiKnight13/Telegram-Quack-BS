using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class UserState
{
    public bool isAdmin {  get; set; }
    public bool isWaitingForTournamentName { get; set; }
    public bool isWaitingForMaxParticipants { get; set; }
    public bool isWaitingForTournamentPrice { get; set; }
    public bool isWaitingForTournamentId { get; set; }
}
