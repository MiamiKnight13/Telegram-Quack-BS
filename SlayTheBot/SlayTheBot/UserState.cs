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
    public int IdToReg {  get; set; }
    public bool isWaitingForTIDToAdd { get; set; }
    public long? IdToAdd { get; set; }
    public int TIdToAdd { get; set; }
    public bool isWaitingForIdToAdd { get; set; }
}
