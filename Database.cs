using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace sideQuest
{
    public class CompletedQuest
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string QuestName { get; set; }
        public int XP { get; set; }
        public string Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public int Saved {  get; set; }
    }

    public class QuestPhoto
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int QuestId { get; set; }
        public string PhotoPath { get; set; }
    }

    public class Database
    {
        private SQLiteConnection db;

        public Database()
        {
            string dbPath = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
                "sidequests.db"
            );
            db = new SQLiteConnection(dbPath);
            db.CreateTable<CompletedQuest>();
            db.CreateTable<QuestPhoto>();
        }

        // Save a completed quest
        public int SaveQuest(CompletedQuest quest)
        {
            db.Insert(quest);
            return quest.Id;
        }

        // Get all completed quests
        public List<CompletedQuest> GetAllQuests()
        {
            return db.Table<CompletedQuest>().ToList();
        }

        // Get quests by date
        public List<CompletedQuest> GetQuestsByDate(string date)
        {
            return db.Table<CompletedQuest>().Where(q => q.Date == date).ToList();
        }

        // Save a photo linked to a quest
        public void SavePhoto(int questId, string photoPath)
        {
            db.Insert(new QuestPhoto { QuestId = questId, PhotoPath = photoPath });
        }

        // Get all photos for a quest
        public List<QuestPhoto> GetPhotosForQuest(int questId)
        {
            return db.Table<QuestPhoto>().Where(p => p.QuestId == questId).ToList();
        }
        public bool IsQuestCompleted(string questName)
        {
            return db.Table<CompletedQuest>().Any(q => q.QuestName == questName && q.Saved == 1);
        }
        public void ClearDatabase()
        {
            db.DeleteAll<CompletedQuest>();
            db.DeleteAll<QuestPhoto>();
        }
    }
}