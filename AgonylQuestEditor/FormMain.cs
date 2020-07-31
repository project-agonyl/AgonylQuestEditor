using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AgonylQuestEditor
{
    public partial class FormMain : Form
    {
        private Queue<A3Quest> _a3Quests = new Queue<A3Quest>();
        private BindingList<A3Quest> _a3QuestsBound = new BindingList<A3Quest>();

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            if (!File.Exists(Utils.GetMyDirectory() + Path.DirectorySeparatorChar + "NPC.bin"))
            {
                _ = MessageBox.Show("Please place NPC.bin in same folder as this application", "Agonyl Quest Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            Utils.LoadNpcData();
            this.dataGridView.AutoGenerateColumns = false;
            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Id",
                Name = "ID",
                Width = 50,
            });
            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "StartNpcName",
                Name = "Start NPC",
                Width = 100,
            });
            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "EndNpcName",
                Name = "Submit NPC",
                Width = 100,
            });
            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "LevelRestriction",
                Name = "Level",
                Width = 100,
            });
            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Experience",
                Name = "Experience",
                Width = 100,
            });
            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Woonz",
                Name = "Woonz",
                Width = 100,
            });
            this.dataGridView.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Lore",
                Name = "Lore",
                Width = 100,
            });
            this.dataGridView.DataSource = this._a3QuestsBound;
            this.ReloadDataButton.Enabled = false;
        }

        private void ChooseQuestFolderButton_Click(object sender, EventArgs e)
        {
            if (this.FolderBrowser.ShowDialog() == DialogResult.OK)
            {
                this.CurrentQuestFolderLabel.Text = this.FolderBrowser.SelectedPath;
                this.QuestDataLoaderBgWorker.RunWorkerAsync();
            }
        }

        private void ReloadDataButton_Click(object sender, EventArgs e)
        {
            if (this.QuestDataLoaderBgWorker.IsBusy)
            {
                _ = MessageBox.Show("Quest loader is busy!", "Agonyl Quest Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this._a3QuestsBound.Clear();
            this._a3Quests.Clear();
            this.QuestDataLoaderBgWorker.RunWorkerAsync();
            this.ReloadDataButton.Enabled = false;
            this.ChooseQuestFolderButton.Enabled = false;
        }

        private void QuestDataLoaderBgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!Directory.Exists(this.CurrentQuestFolderLabel.Text))
            {
                return;
            }

            foreach (var file in Directory.GetFiles(this.CurrentQuestFolderLabel.Text, "*.dat"))
            {
                var fileData = File.ReadAllBytes(file);
                var lowLevel = BitConverter.ToUInt16(fileData.Skip(32).Take(2).ToArray(), 0);
                var highLevel = BitConverter.ToUInt16(fileData.Skip(36).Take(2).ToArray(), 0);
                var levelRestriction = lowLevel + " to " + highLevel;
                if (highLevel == 0xffff && lowLevel == 0xffff)
                {
                    levelRestriction = "No limit";
                }
                else if (highLevel == 0xffff)
                {
                    levelRestriction = lowLevel + " and above";
                }
                else if (lowLevel == 0xffff)
                {
                    levelRestriction = "Upto " + highLevel;
                }

                var questData = new A3Quest()
                {
                    Id = BitConverter.ToUInt16(fileData.Take(2).ToArray(), 0),
                    StartNpcName = Utils.GetNpcName(BitConverter.ToUInt16(fileData.Skip(4).Take(2).ToArray(), 0)),
                    SubmitNpcName = Utils.GetNpcName(BitConverter.ToUInt16(fileData.Skip(8).Take(2).ToArray(), 0)),
                    LevelRestriction = levelRestriction,
                    Experience = BitConverter.ToUInt32(fileData.Skip(80).Take(4).ToArray(), 0),
                    Woonz = BitConverter.ToUInt32(fileData.Skip(84).Take(4).ToArray(), 0),
                    Lore = BitConverter.ToUInt32(fileData.Skip(88).Take(4).ToArray(), 0),
                    QuestFile = file,
                };
                this._a3Quests.Enqueue(questData);
                this.QuestDataLoaderBgWorker.ReportProgress(questData.Id);
            }
        }

        private void QuestDataLoaderBgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (this._a3Quests.Count != 0)
            {
                this._a3QuestsBound.Add(this._a3Quests.Dequeue());
            }
        }

        private void QuestDataLoaderBgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.dataGridView.Refresh();
            if (this._a3QuestsBound.Count == 0)
            {
                _ = MessageBox.Show("Could not find any quest file", "Agonyl Quest Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                this.FixEmptyCells();
                _ = MessageBox.Show("Loaded " + this._a3QuestsBound.Count + " quests", "Agonyl Quest Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            this.ReloadDataButton.Enabled = true;
            this.ChooseQuestFolderButton.Enabled = true;
        }

        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var dataIndexNo = this.dataGridView.Rows[e.RowIndex].Index;
            var questEditorForm = new FormQuestEditor();
            questEditorForm.QuestFile = this._a3QuestsBound[dataIndexNo].QuestFile;
            questEditorForm.ShowDialog();
        }

        private void dataGridView_DragEnter(object sender, DragEventArgs e)
        {
            if (!this.QuestDataLoaderBgWorker.IsBusy)
            {
                // Check if the Data format of the file(s) can be accepted
                // (we only accept file drops from Windows Explorer, etc.)
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    // modify the drag drop effects to Move
                    e.Effect = DragDropEffects.All;
                }
                else
                {
                    // no need for any drag drop effect
                    e.Effect = DragDropEffects.None;
                }
            }
        }

        private void dataGridView_DragDrop(object sender, DragEventArgs e)
        {
            if (!this.QuestDataLoaderBgWorker.IsBusy)
            {
                // still check if the associated data from the file(s) can be used for this purpose
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    // Fetch the file(s) names with full path here to be processed
                    var fileList = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (Path.GetFileName(fileList[0]).IndexOf(".dat") != -1)
                    {
                        var questEditorForm = new FormQuestEditor();
                        questEditorForm.QuestFile = fileList[0];
                        questEditorForm.ShowDialog();
                    }
                }
            }
        }

        // This fix is needed as I cannot figure out why cell gets empty though data is available
        private void FixEmptyCells()
        {
            foreach (DataGridViewRow rw in this.dataGridView.Rows)
            {
                if (rw.Index >= this._a3QuestsBound.Count)
                {
                    break;
                }

                for (var i = 0; i < rw.Cells.Count; i++)
                {
                    if (rw.Cells[i].Value == null)
                    {
                        switch (i)
                        {
                            case 0:
                                rw.Cells[i].Value = this._a3QuestsBound[rw.Index].Id;
                                break;

                            case 1:
                                rw.Cells[i].Value = this._a3QuestsBound[rw.Index].StartNpcName;
                                break;

                            case 2:
                                rw.Cells[i].Value = this._a3QuestsBound[rw.Index].SubmitNpcName;
                                break;

                            case 3:
                                rw.Cells[i].Value = this._a3QuestsBound[rw.Index].LevelRestriction;
                                break;

                            case 4:
                                rw.Cells[i].Value = this._a3QuestsBound[rw.Index].Experience;
                                break;

                            case 5:
                                rw.Cells[i].Value = this._a3QuestsBound[rw.Index].Woonz;
                                break;

                            case 6:
                                rw.Cells[i].Value = this._a3QuestsBound[rw.Index].Lore;
                                break;
                        }
                    }
                }
            }
        }
    }
}
