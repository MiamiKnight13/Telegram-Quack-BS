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
    public bool isWaitingForTIDToAdd { get; set; }
    public long? IdToAdd { get; set; }
    public int TIdToAdd { get; set; }
    public bool isWaitingForIdToAdd { get; set; }
    public bool isWaitingForSupportMessage { get; set; }
    public bool isWaitingForTIdToRemove { get; set; }
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
