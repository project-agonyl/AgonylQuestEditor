using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AgonylQuestEditor
{
    public partial class FormQuestEditor : Form
    {
        public string QuestFile;

        private byte[] _questFileData;
        private A3Quest _currentQuestData;

        public FormQuestEditor()
        {
            InitializeComponent();
        }

        private void FormQuestEditor_Load(object sender, EventArgs e)
        {
            this.Text = "Quest Editor - " + Path.GetFileName(this.QuestFile);
            this.SaveQuestFileDialog.FileName = Path.GetFileName(this.QuestFile);
            this.SaveQuestFileDialog.InitialDirectory = new FileInfo(this.QuestFile).Directory.Name;
            this._questFileData = File.ReadAllBytes(this.QuestFile);
            this._currentQuestData = new A3Quest()
            {
                Id = BitConverter.ToUInt16(this._questFileData.Take(2).ToArray(), 0),
                StartNpcId = BitConverter.ToUInt16(this._questFileData.Skip(4).Take(2).ToArray(), 0),
                SubmitNpcId = BitConverter.ToUInt16(this._questFileData.Skip(8).Take(2).ToArray(), 0),
                LowLevel = BitConverter.ToUInt16(this._questFileData.Skip(32).Take(2).ToArray(), 0),
                HighLevel = BitConverter.ToUInt16(this._questFileData.Skip(36).Take(2).ToArray(), 0),
                Experience = BitConverter.ToUInt32(this._questFileData.Skip(80).Take(4).ToArray(), 0),
                Woonz = BitConverter.ToUInt32(this._questFileData.Skip(84).Take(4).ToArray(), 0),
                Lore = BitConverter.ToUInt32(this._questFileData.Skip(88).Take(4).ToArray(), 0),
                NextQuestId = 0xffff,
            };
            if (this._questFileData.Length == 798)
            {
                this._currentQuestData.NextQuestId = BitConverter.ToUInt16(this._questFileData.Skip(786).Take(2).ToArray(), 0);
            }

            for (var i = 0; i < 3; i++)
            {
                this._currentQuestData.A3QuestItemRewards[i] = new A3QuestItemReward()
                {
                    Id = BitConverter.ToUInt32(this._questFileData.Skip(44 + (i * 4)).Take(4).ToArray(), 0),
                    Count = BitConverter.ToUInt16(this._questFileData.Skip(68 + (i * 4)).Take(2).ToArray(), 0),
                };
            }

            for (var i = 0; i < 6; i++)
            {
                this._currentQuestData.A3QuestRequirements[i] = new A3QuestRequirement()
                {
                    MapId = BitConverter.ToUInt16(this._questFileData.Skip(100 + (i * 96)).Take(2).ToArray(), 0),
                    MonsterId = BitConverter.ToUInt16(this._questFileData.Skip(112 + (i * 96)).Take(2).ToArray(), 0),
                    MonsterCount = BitConverter.ToUInt16(this._questFileData.Skip(116 + (i * 96)).Take(2).ToArray(), 0),
                    QuestItem = BitConverter.ToUInt16(this._questFileData.Skip(124 + (i * 96)).Take(2).ToArray(), 0),
                    QuestItemCount = BitConverter.ToUInt16(this._questFileData.Skip(172 + (i * 96)).Take(2).ToArray(), 0),
                };
            }

            this.LoadTextboxesWithQuestData();
        }

        private void ResetFieldsButton_Click(object sender, EventArgs e)
        {
            this.LoadTextboxesWithQuestData();
        }

        private void SaveQuestButton_Click(object sender, EventArgs e)
        {
            try
            {
                var outFileData = this._questFileData;
                if (!Utils.IsEmptyData(this.QuestIdTextbox.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 0, BitConverter.GetBytes(Convert.ToUInt16(this.QuestIdTextbox.Text)));
                }

                if (!Utils.IsEmptyData(this.StartNpcTextbox.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 4, BitConverter.GetBytes(Convert.ToUInt16(this.StartNpcTextbox.Text)));
                }

                if (!Utils.IsEmptyData(this.SubmitNpcTextbox.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 8, BitConverter.GetBytes(Convert.ToUInt16(this.SubmitNpcTextbox.Text)));
                }

                if (!Utils.IsEmptyData(this.MinLevelTextbox.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 32, BitConverter.GetBytes(Convert.ToUInt16(this.MinLevelTextbox.Text)));
                }

                if (!Utils.IsEmptyData(this.MaxLevelTextbox.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 36, BitConverter.GetBytes(Convert.ToUInt16(this.MaxLevelTextbox.Text)));
                }

                if (!Utils.IsEmptyData(this.ExpTextbox.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 80, BitConverter.GetBytes(Convert.ToUInt32(this.ExpTextbox.Text)));
                }

                if (!Utils.IsEmptyData(this.WoonzTextbox.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 84, BitConverter.GetBytes(Convert.ToUInt32(this.WoonzTextbox.Text)));
                }

                if (!Utils.IsEmptyData(this.LoreTextbox.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 88, BitConverter.GetBytes(Convert.ToUInt32(this.LoreTextbox.Text)));
                }

                if (!Utils.IsEmptyData(this.RewardItemIdTextbox1.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 44, BitConverter.GetBytes(Convert.ToUInt16(this.RewardItemIdTextbox1.Text)));
                }

                if (!Utils.IsEmptyData(this.RewardItemCountTextbox1.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 68, BitConverter.GetBytes(Convert.ToUInt16(this.RewardItemCountTextbox1.Text)));
                }

                if (!Utils.IsEmptyData(this.RewardItemIdTextbox2.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 44 + (1 * 4), BitConverter.GetBytes(Convert.ToUInt16(this.RewardItemIdTextbox2.Text)));
                }

                if (!Utils.IsEmptyData(this.RewardItemCountTextbox2.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 68 + (1 * 4), BitConverter.GetBytes(Convert.ToUInt16(this.RewardItemCountTextbox2.Text)));
                }

                if (!Utils.IsEmptyData(this.RewardItemIdTextbox3.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 44 + (2 * 4), BitConverter.GetBytes(Convert.ToUInt16(this.RewardItemIdTextbox3.Text)));
                }

                if (!Utils.IsEmptyData(this.RewardItemCountTextbox3.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 68 + (2 * 4), BitConverter.GetBytes(Convert.ToUInt16(this.RewardItemCountTextbox3.Text)));
                }

                if (!Utils.IsEmptyData(this.MapIdTextbox1.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 100, BitConverter.GetBytes(Convert.ToUInt16(this.MapIdTextbox1.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestMonsterTextbox1.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 112, BitConverter.GetBytes(Convert.ToUInt16(this.QuestMonsterTextbox1.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestMonsterCountTextbox1.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 116, BitConverter.GetBytes(Convert.ToUInt16(this.QuestMonsterCountTextbox1.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestItemTextbox1.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 124, BitConverter.GetBytes(Convert.ToUInt16(this.QuestItemTextbox1.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestItemCountTextbox1.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 172, BitConverter.GetBytes(Convert.ToUInt16(this.QuestItemCountTextbox1.Text)));
                }

                if (!Utils.IsEmptyData(this.MapIdTextbox2.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 100 + (1 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.MapIdTextbox2.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestMonsterTextbox2.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 112 + (1 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.QuestMonsterTextbox2.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestMonsterCountTextbox2.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 116 + (1 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.QuestMonsterCountTextbox2.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestItemTextbox2.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 124 + (1 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.QuestItemTextbox2.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestItemCountTextbox2.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 172 + (1 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.QuestItemCountTextbox2.Text)));
                }

                if (!Utils.IsEmptyData(this.MapIdTextbox3.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 100 + (2 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.MapIdTextbox3.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestMonsterTextbox3.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 112 + (2 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.QuestMonsterTextbox3.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestMonsterCountTextbox3.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 116 + (2 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.QuestMonsterCountTextbox3.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestItemTextbox3.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 124 + (2 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.QuestItemTextbox3.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestItemCountTextbox3.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 172 + (2 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.QuestItemCountTextbox3.Text)));
                }

                if (!Utils.IsEmptyData(this.MapIdTextbox4.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 100 + (3 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.MapIdTextbox4.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestMonsterTextbox4.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 112 + (3 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.QuestMonsterTextbox4.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestMonsterCountTextbox4.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 116 + (3 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.QuestMonsterCountTextbox4.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestItemTextbox4.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 124 + (3 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.QuestItemTextbox4.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestItemCountTextbox4.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 172 + (3 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.QuestItemCountTextbox4.Text)));
                }

                if (!Utils.IsEmptyData(this.MapIdTextbox5.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 100 + (4 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.MapIdTextbox5.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestMonsterTextbox5.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 112 + (4 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.QuestMonsterTextbox5.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestMonsterCountTextbox5.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 116 + (4 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.QuestMonsterCountTextbox5.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestItemTextbox5.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 124 + (4 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.QuestItemTextbox5.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestItemCountTextbox5.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 172 + (4 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.QuestItemCountTextbox5.Text)));
                }

                if (!Utils.IsEmptyData(this.MapIdTextbox6.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 100 + (5 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.MapIdTextbox6.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestMonsterTextbox6.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 112 + (5 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.QuestMonsterTextbox6.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestMonsterCountTextbox6.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 116 + (5 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.QuestMonsterCountTextbox6.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestItemTextbox6.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 124 + (5 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.QuestItemTextbox6.Text)));
                }

                if (!Utils.IsEmptyData(this.QuestItemCountTextbox6.Text))
                {
                    Utils.ReplaceBytesAt(ref outFileData, 172 + (5 * 96), BitConverter.GetBytes(Convert.ToUInt16(this.QuestItemCountTextbox6.Text)));
                }

                if (this.SaveQuestFileDialog.ShowDialog() != DialogResult.Cancel)
                {
                    File.WriteAllBytes(this.SaveQuestFileDialog.FileName, outFileData);
                    _ = MessageBox.Show("Saved the file " + Path.GetFileName(this.SaveQuestFileDialog.FileName), "Agonyl Quest Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message, "Agonyl Quest Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Textbox_TextChanged(object sender, EventArgs e)
        {
            var textbox = sender as TextBox;
            if (System.Text.RegularExpressions.Regex.IsMatch(textbox.Text, "[^0-9]"))
            {
                _ = MessageBox.Show("Please enter only numbers", "Agonyl Quest Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textbox.Text = string.Empty;
            }
        }

        private void LoadTextboxesWithQuestData()
        {
            this.QuestIdTextbox.Text = this._currentQuestData.Id.ToString();
            this.StartNpcTextbox.Text = this._currentQuestData.StartNpcId.ToString();
            this.SubmitNpcTextbox.Text = this._currentQuestData.SubmitNpcId.ToString();
            if (!Utils.IsEmptyData(this._currentQuestData.LowLevel))
            {
                this.MinLevelTextbox.Text = this._currentQuestData.LowLevel.ToString();
            }

            if (!Utils.IsEmptyData(this._currentQuestData.HighLevel))
            {
                this.MaxLevelTextbox.Text = this._currentQuestData.HighLevel.ToString();
            }

            if (!Utils.IsEmptyData(this._currentQuestData.NextQuestId))
            {
                this.NextQuestTextbox.Text = this._currentQuestData.NextQuestId.ToString();
            }

            this.ExpTextbox.Text = this._currentQuestData.Experience.ToString();
            this.WoonzTextbox.Text = this._currentQuestData.Woonz.ToString();
            this.LoreTextbox.Text = this._currentQuestData.Lore.ToString();
            if (!Utils.IsEmptyData(this._currentQuestData.A3QuestItemRewards[0].Id))
            {
                this.RewardItemIdTextbox1.Text = this._currentQuestData.A3QuestItemRewards[0].Id.ToString();
                this.RewardItemCountTextbox1.Text = this._currentQuestData.A3QuestItemRewards[0].Count.ToString();
            }

            if (!Utils.IsEmptyData(this._currentQuestData.A3QuestItemRewards[1].Id))
            {
                this.RewardItemIdTextbox2.Text = this._currentQuestData.A3QuestItemRewards[1].Id.ToString();
                this.RewardItemCountTextbox2.Text = this._currentQuestData.A3QuestItemRewards[1].Count.ToString();
            }

            if (!Utils.IsEmptyData(this._currentQuestData.A3QuestItemRewards[2].Id))
            {
                this.RewardItemIdTextbox3.Text = this._currentQuestData.A3QuestItemRewards[2].Id.ToString();
                this.RewardItemCountTextbox3.Text = this._currentQuestData.A3QuestItemRewards[2].Count.ToString();
            }

            if (!Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[0].MonsterId) && !Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[0].MapId))
            {
                this.MapIdTextbox1.Text = this._currentQuestData.A3QuestRequirements[0].MapId.ToString();
                this.QuestMonsterTextbox1.Text = this._currentQuestData.A3QuestRequirements[0].MonsterId.ToString();
                if (!Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[0].MonsterCount))
                {
                    this.QuestMonsterCountTextbox1.Text = this._currentQuestData.A3QuestRequirements[0].MonsterCount.ToString();
                }

                if (!Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[0].QuestItem))
                {
                    this.QuestItemTextbox1.Text = this._currentQuestData.A3QuestRequirements[0].QuestItem.ToString();
                    this.QuestItemCountTextbox1.Text = this._currentQuestData.A3QuestRequirements[0].QuestItemCount.ToString();
                }
            }

            if (!Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[1].MonsterId) && !Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[1].MapId))
            {
                this.MapIdTextbox2.Text = this._currentQuestData.A3QuestRequirements[1].MapId.ToString();
                this.QuestMonsterTextbox2.Text = this._currentQuestData.A3QuestRequirements[1].MonsterId.ToString();
                if (!Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[1].MonsterCount))
                {
                    this.QuestMonsterCountTextbox2.Text = this._currentQuestData.A3QuestRequirements[0].MonsterCount.ToString();
                }

                if (!Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[1].QuestItem))
                {
                    this.QuestItemTextbox2.Text = this._currentQuestData.A3QuestRequirements[1].QuestItem.ToString();
                    this.QuestItemCountTextbox2.Text = this._currentQuestData.A3QuestRequirements[1].QuestItemCount.ToString();
                }
            }

            if (!Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[2].MonsterId) && !Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[2].MapId))
            {
                this.MapIdTextbox3.Text = this._currentQuestData.A3QuestRequirements[2].MapId.ToString();
                this.QuestMonsterTextbox3.Text = this._currentQuestData.A3QuestRequirements[2].MonsterId.ToString();
                if (!Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[2].MonsterCount))
                {
                    this.QuestMonsterCountTextbox3.Text = this._currentQuestData.A3QuestRequirements[0].MonsterCount.ToString();
                }

                if (!Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[2].QuestItem))
                {
                    this.QuestItemTextbox3.Text = this._currentQuestData.A3QuestRequirements[2].QuestItem.ToString();
                    this.QuestItemCountTextbox3.Text = this._currentQuestData.A3QuestRequirements[2].QuestItemCount.ToString();
                }
            }

            if (!Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[3].MonsterId) && !Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[3].MapId))
            {
                this.MapIdTextbox4.Text = this._currentQuestData.A3QuestRequirements[3].MapId.ToString();
                this.QuestMonsterTextbox4.Text = this._currentQuestData.A3QuestRequirements[3].MonsterId.ToString();
                if (!Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[3].MonsterCount))
                {
                    this.QuestMonsterCountTextbox4.Text = this._currentQuestData.A3QuestRequirements[3].MonsterCount.ToString();
                }

                if (!Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[3].QuestItem))
                {
                    this.QuestItemTextbox4.Text = this._currentQuestData.A3QuestRequirements[3].QuestItem.ToString();
                    this.QuestItemCountTextbox4.Text = this._currentQuestData.A3QuestRequirements[3].QuestItemCount.ToString();
                }
            }

            if (!Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[4].MonsterId) && !Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[4].MapId))
            {
                this.MapIdTextbox5.Text = this._currentQuestData.A3QuestRequirements[4].MapId.ToString();
                this.QuestMonsterTextbox5.Text = this._currentQuestData.A3QuestRequirements[4].MonsterId.ToString();
                if (!Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[4].MonsterCount))
                {
                    this.QuestMonsterCountTextbox5.Text = this._currentQuestData.A3QuestRequirements[4].MonsterCount.ToString();
                }

                if (!Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[4].QuestItem))
                {
                    this.QuestItemTextbox5.Text = this._currentQuestData.A3QuestRequirements[4].QuestItem.ToString();
                    this.QuestItemCountTextbox5.Text = this._currentQuestData.A3QuestRequirements[4].QuestItemCount.ToString();
                }
            }

            if (!Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[5].MonsterId) && !Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[5].MapId))
            {
                this.MapIdTextbox6.Text = this._currentQuestData.A3QuestRequirements[5].MapId.ToString();
                this.QuestMonsterTextbox6.Text = this._currentQuestData.A3QuestRequirements[5].MonsterId.ToString();
                if (!Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[5].MonsterCount))
                {
                    this.QuestMonsterCountTextbox6.Text = this._currentQuestData.A3QuestRequirements[5].MonsterCount.ToString();
                }

                if (!Utils.IsEmptyData(this._currentQuestData.A3QuestRequirements[5].QuestItem))
                {
                    this.QuestItemTextbox6.Text = this._currentQuestData.A3QuestRequirements[5].QuestItem.ToString();
                    this.QuestItemCountTextbox6.Text = this._currentQuestData.A3QuestRequirements[5].QuestItemCount.ToString();
                }
            }
        }
    }
}
