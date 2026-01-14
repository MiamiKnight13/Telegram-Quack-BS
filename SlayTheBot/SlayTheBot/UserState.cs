using SlayTheBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class UserState
{
    //available for users
    public bool isAdmin {  get; set; }
    public bool isWaitingForSupportMessage { get; set; }
    public int TIDToReg { get; set; }

    //not available for users
    public bool isWaitingForTournamentName { get; set; }
    public bool isWaitingForMaxParticipants { get; set; }
    public bool isWaitingForTournamentPrice { get; set; }
    public bool isWaitingForTournamentStarPrice { get; set; }
    public string TournamentStarPrice { get; set;}
    public bool isWaitingForTournamentId { get; set; }
    public bool isWaitingForTIDToAddPar { get; set; }
    public int TIdToAddPar { get; set; }
    public bool isWaitingForIdToAddInTournament { get; set; }
    public long? IdToAddInTournament { get; set; }
    public bool isWaitingForTIdToDelete { get; set; }
    public int MessageCode { get; set; }
    public bool isWaitingForTIdToCheckList { get; set; }
    public bool isWaitingForMessageToSendToAllParticipants { get; set; }
    public bool isWaitingForTIdToSendMessageToAllParticipants { get; set; }
    public int TIdToSendMessageToAllParticipants { get; set; }
    public bool isWaitingForUserIdToRemove { get; set; }
    public bool isWaitingForTIdToRemoveUser { get; set; }
    public int TIdToRemoveUser { get; set; }
    public bool isWaitingForUserIdToConfirm {  get; set; }
}
