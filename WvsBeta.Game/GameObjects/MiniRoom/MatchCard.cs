using System.Collections.Generic;

namespace WvsBeta.Game.GameObjects.MiniRoom
{
    public class MatchCard
    {
        public static Dictionary<int, MatchCard> MatchCards = new Dictionary<int, MatchCard>();

        public int ID { get; set; }
        public bool Private { get; set; }
        public string Title { get; protected set; }
        public string Password { get; protected set; }
        public byte MaxUsers { get; protected set; }
        public byte EnteredUsers { get; protected set; }
        public GameCharacter[] Users { get; protected set; }
        public bool Opened { get; protected set; }
        public bool CloseRequest { get; protected set; }
        public bool GameStarted { get; protected set; }
        public bool Tournament { get; protected set; }

        public MatchCard(string pTitle, bool pPrivate, string pPassword)
        {
            ID = Server.Instance.MatchCardIDs.NextValue();
            MatchCards.Add(ID, this);
            Title = pTitle;
            Password = pPassword;
            MaxUsers = 2;
            Users = new GameCharacter[MaxUsers];
            Opened = false;
            Private = pPrivate;
            CloseRequest = false;
            GameStarted = false;
            Tournament = false;
        }

        public byte GetEmptySlot()
        {
            for (byte i = 0; i < MaxUsers; i++)
            {
                if (Users[i] == null) return i;
            }
            return 0xFF;
        }

        public void RemovePlayer(GameCharacter pCharacter)
        {
            Users[pCharacter.RoomSlotId] = null;
            pCharacter.RoomSlotId = 0;
            EnteredUsers--;
        }

        public void AddPlayer(GameCharacter pCharacter)
        {
            EnteredUsers++;
            pCharacter.RoomSlotId = GetEmptySlot();
            Users[pCharacter.RoomSlotId] = pCharacter;
        }

        public static MatchCard CreateRoom(GameCharacter pOwner, string pTitle, bool pPrivate, string pPassword)
        {
            MatchCard matchcard = new MatchCard(pTitle, pPrivate, pPassword);
            matchcard.AddPlayer(pOwner);
            return matchcard;
        }
    }
}
