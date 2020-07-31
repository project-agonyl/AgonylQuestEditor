namespace AgonylQuestEditor
{
    public class A3Quest
    {
        public ushort Id;

        public ushort StartNpcId;

        public string StartNpcName;

        public ushort SubmitNpcId;

        public string SubmitNpcName;

        public ushort LowLevel;

        public ushort HighLevel;

        public string LevelRestriction;

        public uint Experience;

        public uint Woonz;

        public uint Lore;

        public A3QuestRequirement[] A3QuestRequirements = new A3QuestRequirement[6];

        public A3QuestItemReward[] A3QuestItemRewards = new A3QuestItemReward[3];

        public ushort NextQuestId;

        public string QuestFile;
    }
}
