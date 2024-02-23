using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Yahtzee
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        // kniffel = Yahtzee


        // strings for the textboxes that we need to reuse multiple times
        const string str_result_simples = " Gessamt: ";
        const string str_bonusAt63 = " Bonus bei 63: ";
        const string str_topSum = " Gesammt oben: ";
        const string str_bottomSum = " Gesammt(unten) : ";
        const string str_bottomTopSum = " Gesammt(oben) : ";
        const string str_result = " Endsumme: ";
        const string str_ranking = " Platz: ";
        const string str_entries = "Einträge: ";

        private byte yahtzeeAt = 1; // for houserules. this number will change every new game and determins what number all dice have to be for a yahtzee
    
        public void incYahtzeeAt()
        {
            yahtzeeAt++;
            if (yahtzeeAt > 6)
                yahtzeeAt = 1;
            lbl_yahtzeeAt.Text = "Kniffel bei: " + yahtzeeAt;
        }
        public struct playerComponents
        {
            public GroupBox groupBox_player;
            public Label[] labels;
            public NumericUpDown[] simpleDice;  // ones, twoes, ..., sixes
            public NumericUpDown[] pairs;
            public NumericUpDown[] xoak; // x of a kind

            public NumericUpDown nup_chance;
            public NumericUpDown nup_yahtzee;

            public CheckBox cbx_fullHouse;
            public CheckBox cbx_smallStraight;
            public CheckBox cbx_largeStraight;
            public CheckBox cbx_yahtzee;

            public CheckBox[] forfits;

            public Label lbl_result_simples;
            public Label lbl_bonusAt63;
            public Label lbl_topSum;

            public Label lbl_bottomSum;
            public Label lbl_bottomTopSum;
            public Label lbl_result;
            

            public TextBox tbx_name;

            public Label lbl_ranking;
            public Label lbl_numEntries;

            public uint entries;
        };

        private List<decimal> prev_twoPairs_value = new List<decimal>();
        private List<playerComponents> playerSheets = new List<playerComponents>();

        // events
        private void updateEntries_cbx_ValueChanged(object sender, EventArgs e)
        {
            CheckBox nup = sender as CheckBox;
            if (nup.Tag == null) return;

            int id = (int)nup.Tag;
            if ((uint)id >= playerSheets.Count) return;
            playerComponents components = playerSheets[id];

            if (nup.Checked)
                components.entries++;
            else
                components.entries--;

            playerSheets[id] = components;
            playerSheets[id].lbl_numEntries.Text = str_entries + playerSheets[id].entries;
        }

        private void nup_ext_ValueChanged(object sender, EventArgs e) // update all nups
        {

            NumericUpDown nup = sender as NumericUpDown;

            nup.Value = roundToIncrement(nup.Value, nup.Increment);
            prevValExt prev = (prevValExt)nup.Tag;

            if (nup.Value > 0 && nup.Value < prev.min) //example increment is 2, but minumun value is 4, but we can't set minimum value to 4, because we need 0 | therefor we allow 0, but check if the value is below the minimum.
            {
                if (prev.prev == 0) // if the previous value was 0, then we need to set to the min value,
                    nup.Value = prev.min;
                else                // if it was not zero, then it must become zero
                    nup.Value = 0;
            }

            prev.prev = nup.Value;
            nup.Tag = prev;



            // update entries count
            if (nup.Tag == null) return;

            prevValExt Tag = (prevValExt)nup.Tag;
            if ((uint)Tag.playerID >= playerSheets.Count) return;
            playerComponents components = playerSheets[Tag.playerID];

            if (nup.Value != 0 && Tag.prev == 0)
                components.entries++;
            else if (nup.Value == 0 && Tag.prev != 0)
                components.entries--;

            playerSheets[Tag.playerID] = components;
            playerSheets[Tag.playerID].lbl_numEntries.Text = str_entries + playerSheets[Tag.playerID].entries;
        }

        private void cbx_forfit(object sender, EventArgs e)
        {
            CheckBox cbx = sender as CheckBox;
            forfitID Tag = (forfitID)cbx.Tag;

            if (Tag.checkBoxID <= 5)
            {
                playerSheets[Tag.playerID].simpleDice[Tag.checkBoxID].Value = 0;
                playerSheets[Tag.playerID].simpleDice[Tag.checkBoxID].Enabled = !cbx.Checked;

                {
                    CheckBox cbx3 = new CheckBox();
                    cbx3.Checked = cbx.Checked;
                    cbx3.Tag = Tag.playerID;
                    updateEntries_cbx_ValueChanged(cbx3, e);
                }
                return;
            }

            switch (Tag.checkBoxID)
            {
                case 6:
                    playerSheets[Tag.playerID].pairs[0].Value = 0;
                    playerSheets[Tag.playerID].pairs[0].Enabled = !cbx.Checked;
                    break;
                case 7:
                    playerSheets[Tag.playerID].pairs[1].Value = 0;
                    playerSheets[Tag.playerID].pairs[1].Enabled = !cbx.Checked;
                    break;
                case 8:
                    playerSheets[Tag.playerID].xoak[1].Value = 0;
                    playerSheets[Tag.playerID].xoak[1].Enabled = !cbx.Checked;
                    break;
                case 9:
                    playerSheets[Tag.playerID].xoak[0].Value = 0;
                    playerSheets[Tag.playerID].xoak[0].Enabled = !cbx.Checked;
                    break;
                case 10:
                    playerSheets[Tag.playerID].cbx_fullHouse.Checked = false;
                    playerSheets[Tag.playerID].cbx_fullHouse.Enabled = !cbx.Checked;
                    break;
                case 11:
                    playerSheets[Tag.playerID].cbx_smallStraight.Checked = false;
                    playerSheets[Tag.playerID].cbx_smallStraight.Enabled = !cbx.Checked;
                    break;
                case 12:
                    playerSheets[Tag.playerID].cbx_largeStraight.Checked = false;
                    playerSheets[Tag.playerID].cbx_largeStraight.Enabled = !cbx.Checked;
                    break;
                case 13:
                    playerSheets[Tag.playerID].cbx_yahtzee.Checked = false;
                    playerSheets[Tag.playerID].cbx_yahtzee.Enabled = !cbx.Checked;
                    break;
                case 14:
                    playerSheets[Tag.playerID].nup_chance.Value = 0;
                    playerSheets[Tag.playerID].nup_chance.Enabled = !cbx.Checked;
                    break;

                default:
                    MessageBox.Show("Unknown Forfit Action (cbx: " + Tag.checkBoxID.ToString() + ")");
                    break;
            }



            CheckBox cbx2 = new CheckBox();
            cbx2.Checked = cbx.Checked;
            cbx2.Tag = Tag.playerID;
            updateEntries_cbx_ValueChanged(cbx2, e);
        }

        private void cbx_yahtzeeTreestate(object sender, EventArgs e) // handles the yahtzee checkbox behavioir
        {
            CheckBox cbx = sender as CheckBox;
            yahtzeeID Tag = (yahtzeeID)cbx.Tag;

            switch (cbx.CheckState)
            {
                case CheckState.Checked:
                    playerSheets[Tag.playerID].nup_yahtzee.Visible = false;
                    playerSheets[Tag.playerID].nup_yahtzee.Value = 0;
                    playerSheets[Tag.playerID].labels[Tag.labelID].Visible = false;
                    cbx.Text = "Kniffel";
                    {
                        CheckBox cbx2 = new CheckBox();
                        cbx2.Checked = cbx.Checked;
                        cbx2.Tag = Tag.playerID;
                        updateEntries_cbx_ValueChanged(cbx2, e);
                    }
                    break;
                case CheckState.Indeterminate:
                    playerSheets[Tag.playerID].nup_yahtzee.Visible = true;
                    playerSheets[Tag.playerID].nup_yahtzee.Value = 50;
                    playerSheets[Tag.playerID].labels[Tag.labelID].Visible = true;
                    cbx.Text = "";
                    break;
                case CheckState.Unchecked:
                    playerSheets[Tag.playerID].nup_yahtzee.Visible = false;
                    playerSheets[Tag.playerID].nup_yahtzee.Value = 0;
                    playerSheets[Tag.playerID].labels[Tag.labelID].Visible = false;
                    cbx.Text = "Kniffel";
                    {
                        CheckBox cbx2 = new CheckBox();
                        cbx2.Checked = cbx.Checked;
                        cbx2.Tag = Tag.playerID;
                        updateEntries_cbx_ValueChanged(cbx2, e);
                    }
                    break;
            }
        }

        // tag stucts
        private struct yahtzeeID
        {
            public int playerID;
            public int labelID;

            public yahtzeeID(int playerID, int labelID)
            {
                this.playerID = playerID;
                this.labelID = labelID;
            }
        }
        private struct forfitID
        {
            public int playerID;
            public int checkBoxID;

            public forfitID(int playerID, int checkBoxID)
            {
                this.playerID = playerID;
                this.checkBoxID = checkBoxID;
            }
        }
        private struct prevValExt{
            public decimal prev;
            public decimal min;
            public int playerID;
            public prevValExt(decimal min, int playerID)
            {
                prev = 0;
                this.min = min;
                this.playerID = playerID;
            }
        };



        private decimal roundToIncrement(decimal number, decimal increment)
        {
            return ((int)(number / increment)) * increment;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            btn_playerCount_Click(sender,e); // clicks the buttion to create player sheets.
            resizeWindow();
        }

        public Point getGroupBoxLocation(Point offset)
        {
            if (playerSheets.Count == 0)
                return offset;

            // x = latest Group box x + 15 + offset x + Last Group Box Size
            // y = offset y
            return new Point(playerSheets[playerSheets.Count - 1].groupBox_player.Location.X + offset.X + 15 + playerSheets[playerSheets.Count - 1].groupBox_player.Size.Width, offset.Y);
        }

        private void btn_playerCount_Click(object sender, EventArgs e)
        {

            if (nup_playerCount.Value == playerSheets.Count)
                return;




            if (nup_playerCount.Value < playerSheets.Count)
            {
                for (int i = playerSheets.Count-1; i >= nup_playerCount.Value; i--)
                {
                    this.Controls.Remove(playerSheets[i].groupBox_player);
                    playerSheets[i].groupBox_player.Dispose();
                    playerSheets.RemoveAt(i);
                    prev_twoPairs_value.RemoveAt(i);
                }
                resizeWindow();
                return;
            }





            {
                for (int i = 0 + playerSheets.Count; i < nup_playerCount.Value; i++)
                {
                    playerComponents components = new playerComponents();

                    components.groupBox_player = new GroupBox();
                    components.lbl_result = new Label();
                    components.lbl_bottomTopSum = new Label();
                    components.lbl_bottomSum = new Label();
                    components.lbl_topSum = new Label();
                    components.lbl_bonusAt63 = new Label();
                    components.lbl_result_simples = new Label();
                    components.lbl_ranking = new Label();

                    components.cbx_yahtzee = new CheckBox();
                    components.cbx_largeStraight = new CheckBox();
                    components.cbx_smallStraight = new CheckBox();
                    components.cbx_fullHouse = new CheckBox();

                    components.forfits = new CheckBox[15];

                    components.nup_chance = new NumericUpDown();
                    components.nup_yahtzee = new NumericUpDown();

                    components.tbx_name = new TextBox();
                    
                    components.labels = new Label[15];
                    components.simpleDice = new NumericUpDown[6];
                    components.pairs = new NumericUpDown[2];
                    components.xoak = new NumericUpDown[2];

                    components.lbl_numEntries = new Label();
                    components.entries = 0;

                    for (int j = 0; j < components.labels.Length; j++)
                    {
                        components.labels[j] = new Label();
                        components.groupBox_player.Controls.Add(components.labels[j]);
                    }

                    for (int j = 0; j < components.forfits.Length; j++)
                    {
                        components.forfits[j] = new CheckBox();
                        components.groupBox_player.Controls.Add(components.forfits[j]);
                    }

                    components.groupBox_player.SuspendLayout();

                    for (int j = 0; j < components.simpleDice.Length; j++)
                    {
                        components.simpleDice[j] = new NumericUpDown();
                        ((System.ComponentModel.ISupportInitialize)(components.simpleDice[j])).BeginInit();
                        components.groupBox_player.Controls.Add(components.simpleDice[j]);
                    }
                    for (int j = 0; j < components.pairs.Length; j++)
                    {
                        components.pairs[j] = new NumericUpDown();
                        ((System.ComponentModel.ISupportInitialize)(components.pairs[j])).BeginInit();
                        components.groupBox_player.Controls.Add(components.pairs[j]);
                    }
                    for (int j = 0; j < components.xoak.Length; j++)
                    {
                        components.xoak[j] = new NumericUpDown();
                        ((System.ComponentModel.ISupportInitialize)(components.xoak[j])).BeginInit();
                        components.groupBox_player.Controls.Add(components.xoak[j]);
                    }

                    ((System.ComponentModel.ISupportInitialize)(components.nup_chance)).BeginInit();
                    ((System.ComponentModel.ISupportInitialize)(components.nup_yahtzee)).BeginInit();

                    SuspendLayout();


                    // 
                    // groupBox_player
                    // 
                    components.groupBox_player.Controls.Add(components.lbl_ranking);
                    components.groupBox_player.Controls.Add(components.lbl_result);
                    components.groupBox_player.Controls.Add(components.lbl_bottomTopSum);
                    components.groupBox_player.Controls.Add(components.lbl_bottomSum);
                    components.groupBox_player.Controls.Add(components.lbl_topSum);
                    components.groupBox_player.Controls.Add(components.lbl_bonusAt63);
                    components.groupBox_player.Controls.Add(components.lbl_result_simples);

                    components.groupBox_player.Controls.Add(components.cbx_yahtzee);
                    components.groupBox_player.Controls.Add(components.cbx_largeStraight);
                    components.groupBox_player.Controls.Add(components.cbx_smallStraight);
                    components.groupBox_player.Controls.Add(components.cbx_fullHouse);

                    components.groupBox_player.Controls.Add(components.nup_chance);
                    components.groupBox_player.Controls.Add(components.nup_yahtzee);

                    components.groupBox_player.Controls.Add(components.tbx_name);
                    components.groupBox_player.Controls.Add(components.lbl_numEntries);

                    int labelIndex = 0;

                    components.groupBox_player.Location = getGroupBoxLocation(new System.Drawing.Point(15, 50));
                    components.groupBox_player.Name = "groupBox_player_" + i;
                    components.groupBox_player.Size = new System.Drawing.Size(160, 670);
                    components.groupBox_player.TabIndex = 0;
                    components.groupBox_player.TabStop = false;
                    components.groupBox_player.Text = "Spieler " + (i+1);
                    components.groupBox_player.AutoSize = true;
                    components.groupBox_player.AutoSizeMode = AutoSizeMode.GrowAndShrink;

                    // 
                    // lbl_result
                    // 
                    components.lbl_result.AutoSize = true;
                    components.lbl_result.Location = new System.Drawing.Point(45, 602);
                    components.lbl_result.Name = "lbl_result" + i;
                    components.lbl_result.Size = new System.Drawing.Size(62, 13);
                    components.lbl_result.TabIndex = 33;
                    components.lbl_result.Text = str_result;
                    // 
                    // lbl_bottomTopSum
                    // 
                    components.lbl_bottomTopSum.AutoSize = true;
                    components.lbl_bottomTopSum.Location = new System.Drawing.Point(45, 576);
                    components.lbl_bottomTopSum.Name = "lbl_bottomTopSum_" + i;
                    components.lbl_bottomTopSum.Size = new System.Drawing.Size(84, 13);
                    components.lbl_bottomTopSum.TabIndex = 32;
                    components.lbl_bottomTopSum.Text = str_bottomTopSum;
                    // 
                    // lbl_bottomSum
                    // 
                    components.lbl_bottomSum.AutoSize = true;
                    components.lbl_bottomSum.Location = new System.Drawing.Point(45, 550);
                    components.lbl_bottomSum.Name = "lbl_bottomSum_" + i;
                    components.lbl_bottomSum.Size = new System.Drawing.Size(87, 13);
                    components.lbl_bottomSum.TabIndex = 31;
                    components.lbl_bottomSum.Text = str_bottomSum;
                    // 
                    // lbl_topSum_1
                    // 
                    components.lbl_topSum.AutoSize = true;
                    components.lbl_topSum.Location = new System.Drawing.Point(45, 284);
                    components.lbl_topSum.Name = "lbl_topSum_" + i;
                    components.lbl_topSum.Size = new System.Drawing.Size(73, 13);
                    components.lbl_topSum.TabIndex = 30;
                    components.lbl_topSum.Text = str_topSum;
                    // 
                    // lbl_bonusAt63
                    // 
                    components.lbl_bonusAt63.AutoSize = true;
                    components.lbl_bonusAt63.Location = new System.Drawing.Point(45, 258);
                    components.lbl_bonusAt63.Name = "lbl_bonusAt63_" + i;
                    components.lbl_bonusAt63.Size = new System.Drawing.Size(72, 13);
                    components.lbl_bonusAt63.TabIndex = 29;
                    components.lbl_bonusAt63.Text = str_bonusAt63;
                    // 
                    // lbl_result_simples
                    // 
                    components.lbl_result_simples.AutoSize = true;
                    components.lbl_result_simples.Location = new System.Drawing.Point(45, 232);
                    components.lbl_result_simples.Name = "lbl_result_simples:" + i;
                    components.lbl_result_simples.Size = new System.Drawing.Size(54, 13);
                    components.lbl_result_simples.TabIndex = 28;
                    components.lbl_result_simples.Text = str_result_simples;
                    // 
                    // cbx_yahtzee
                    // 
                    components.cbx_yahtzee.AutoSize = true;
                    components.cbx_yahtzee.Location = new System.Drawing.Point(46, 483);
                    components.cbx_yahtzee.Name = "cbx_yahtzee_" + i;
                    components.cbx_yahtzee.Size = new System.Drawing.Size(55, 17);
                    components.cbx_yahtzee.TabIndex = 27;
                    components.cbx_yahtzee.Text = "Kniffel";
                    components.cbx_yahtzee.UseVisualStyleBackColor = true;
                    components.cbx_yahtzee.ThreeState = true;
                    components.cbx_yahtzee.CheckStateChanged += new System.EventHandler(cbx_yahtzeeTreestate);
                    components.cbx_yahtzee.Tag = new yahtzeeID(i,labelIndex);

                    // 
                    // lbl_yahtzee
                    // 
                    components.labels[labelIndex].AutoSize = true;
                    components.labels[labelIndex].Location = new System.Drawing.Point(6, 483);
                    components.labels[labelIndex].Name = "lbl_yahtzee_" + i;
                    components.labels[labelIndex].Size = new System.Drawing.Size(44, 13);
                    components.labels[labelIndex].TabIndex = 24;
                    components.labels[labelIndex].Text = "Kniffel";
                    components.labels[labelIndex].Visible = false;
                    labelIndex++;

                    // 
                    // nup_yahtzee
                    // 
                    components.nup_yahtzee.Location = new System.Drawing.Point(76, 480);
                    components.nup_yahtzee.Maximum = new decimal(new int[] { 0, 1, 0, 0 });
                    components.nup_yahtzee.Name = "nup_yahtzee_" + i;
                    components.nup_yahtzee.Size = new System.Drawing.Size(56, 20);
                    components.nup_yahtzee.TabIndex = 23;
                    components.nup_yahtzee.Visible = false;
                    // 
                    // cbx_bigSteet
                    // 
                    components.cbx_largeStraight.AutoSize = true;
                    components.cbx_largeStraight.Location = new System.Drawing.Point(46, 460);
                    components.cbx_largeStraight.Name = "cbx_bigSteet_" + i;
                    components.cbx_largeStraight.Size = new System.Drawing.Size(89, 17);
                    components.cbx_largeStraight.TabIndex = 26;
                    components.cbx_largeStraight.Text = "Große Straße";
                    components.cbx_largeStraight.UseVisualStyleBackColor = true;
                    components.cbx_largeStraight.CheckStateChanged += new System.EventHandler(updateEntries_cbx_ValueChanged);
                    components.cbx_largeStraight.Tag = i;
                    // 
                    // cbx_smallSteet
                    // 
                    components.cbx_smallStraight.AutoSize = true;
                    components.cbx_smallStraight.Location = new System.Drawing.Point(46, 436);
                    components.cbx_smallStraight.Name = "cbx_smallSteet_" + i;
                    components.cbx_smallStraight.Size = new System.Drawing.Size(89, 17);
                    components.cbx_smallStraight.TabIndex = 25;
                    components.cbx_smallStraight.Text = "Kleine Straße";
                    components.cbx_smallStraight.UseVisualStyleBackColor = true;
                    components.cbx_smallStraight.CheckStateChanged += new System.EventHandler(updateEntries_cbx_ValueChanged);
                    components.cbx_smallStraight.Tag = i;
                    // 
                    // lbl_chance
                    // 
                    components.labels[labelIndex].AutoSize = true;
                    components.labels[labelIndex].Location = new System.Drawing.Point(6, 517);
                    components.labels[labelIndex].Name = "lbl_chance_" + i;
                    components.labels[labelIndex].Size = new System.Drawing.Size(44, 13);
                    components.labels[labelIndex].TabIndex = 24;
                    components.labels[labelIndex].Text = "Chance";
                    labelIndex++;
                    // 
                    // nup_chance
                    // 
                    components.nup_chance.Location = new System.Drawing.Point(76, 515);
                    components.nup_chance.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
                    components.nup_chance.Name = "nup_chance_" + i;
                    components.nup_chance.Size = new System.Drawing.Size(56, 20);
                    components.nup_chance.TabIndex = 23;
                    components.nup_chance.ValueChanged += new System.EventHandler(nup_ext_ValueChanged);
                    components.nup_chance.Tag = new prevValExt(0, i);
                    // 
                    // lbl_voak
                    // 
                    components.labels[labelIndex].AutoSize = true;
                    components.labels[labelIndex].Location = new System.Drawing.Point(6, 383);
                    components.labels[labelIndex].Name = "lbl_voak_" + i;
                    components.labels[labelIndex].Size = new System.Drawing.Size(63, 13);
                    components.labels[labelIndex].TabIndex = 22;
                    components.labels[labelIndex].Text = "Viererpasch";
                    labelIndex++;
                    // 
                    // nup_voak
                    // 
                    components.xoak[0].Location = new System.Drawing.Point(76, 381);
                    components.xoak[0].Maximum = new decimal(new int[] { 30, 0, 0, 0 });
                    components.xoak[0].Name = "nup_voak_" + i;
                    components.xoak[0].Size = new System.Drawing.Size(56, 20);
                    components.xoak[0].TabIndex = 21;
                    components.xoak[0].Tag = new prevValExt(5,i);
                    components.xoak[0].ValueChanged += new System.EventHandler(nup_ext_ValueChanged);
                    // 
                    // lbl_toak
                    // 
                    components.labels[labelIndex].AutoSize = true;
                    components.labels[labelIndex].Location = new System.Drawing.Point(6, 357);
                    components.labels[labelIndex].Name = "lbl_toak_" + i;
                    components.labels[labelIndex].Size = new System.Drawing.Size(64, 13);
                    components.labels[labelIndex].TabIndex = 20;
                    components.labels[labelIndex].Text = "Dreierpasch";
                    labelIndex++;
                    // 
                    // nup_toak
                    // 
                    components.xoak[1].Location = new System.Drawing.Point(76, 355);
                    components.xoak[1].Maximum = new decimal(new int[] { 30, 0, 0, 0 });
                    components.xoak[1].Name = "nup_toak_" + i;
                    components.xoak[1].Size = new System.Drawing.Size(56, 20);
                    components.xoak[1].TabIndex = 19;
                    components.xoak[1].Tag = new prevValExt(5,i);
                    components.xoak[1].ValueChanged += new System.EventHandler(nup_ext_ValueChanged);
                    // 
                    // lbl_sixes
                    // 
                    components.labels[labelIndex].AutoSize = true;
                    components.labels[labelIndex].Location = new System.Drawing.Point(6, 200);
                    components.labels[labelIndex].Name = "lbl_sixes_" + i;
                    components.labels[labelIndex].Size = new System.Drawing.Size(22, 13);
                    components.labels[labelIndex].TabIndex = 18;
                    components.labels[labelIndex].Text = "6er";
                    labelIndex++;
                    // 
                    // nup_sixes
                    // 
                    components.simpleDice[5].Increment = new decimal(new int[] { 6, 0, 0, 0 });
                    components.simpleDice[5].Location = new System.Drawing.Point(48, 198);
                    components.simpleDice[5].Maximum = new decimal(new int[] { 30, 0, 0, 0 });
                    components.simpleDice[5].Name = "nup_sixes_" + i;
                    components.simpleDice[5].Size = new System.Drawing.Size(56, 20);
                    components.simpleDice[5].TabIndex = 17;
                    components.simpleDice[5].ValueChanged += new System.EventHandler(nup_ext_ValueChanged);
                    components.simpleDice[5].Tag = new prevValExt(0, i);
                    // 
                    // lbl_fives
                    // 
                    components.labels[labelIndex].AutoSize = true;
                    components.labels[labelIndex].Location = new System.Drawing.Point(6, 174);
                    components.labels[labelIndex].Name = "lbl_fives_" + i;
                    components.labels[labelIndex].Size = new System.Drawing.Size(22, 13);
                    components.labels[labelIndex].TabIndex = 16;
                    components.labels[labelIndex].Text = "5er";
                    labelIndex++;
                    // 
                    // nup_fives
                    // 
                    components.simpleDice[4].Increment = new decimal(new int[] { 5, 0, 0, 0 });
                    components.simpleDice[4].Location = new System.Drawing.Point(48, 172);
                    components.simpleDice[4].Maximum = new decimal(new int[] { 25, 0, 0, 0 });
                    components.simpleDice[4].Name = "nup_fives_" + i;
                    components.simpleDice[4].Size = new System.Drawing.Size(56, 20);
                    components.simpleDice[4].TabIndex = 15;
                    components.simpleDice[4].ValueChanged += new System.EventHandler(nup_ext_ValueChanged);
                    components.simpleDice[4].Tag = new prevValExt(0, i);
                    // 
                    // lbl_fours
                    // 
                    components.labels[labelIndex].AutoSize = true;
                    components.labels[labelIndex].Location = new System.Drawing.Point(6, 148);
                    components.labels[labelIndex].Name = "lbl_fours_" + i;
                    components.labels[labelIndex].Size = new System.Drawing.Size(22, 13);
                    components.labels[labelIndex].TabIndex = 14;
                    components.labels[labelIndex].Text = "4er";
                    labelIndex++;
                    // 
                    // nup_fours
                    // 
                    components.simpleDice[3].Increment = new decimal(new int[] { 4, 0, 0, 0 });
                    components.simpleDice[3].Location = new System.Drawing.Point(48, 146);
                    components.simpleDice[3].Maximum = new decimal(new int[] { 20, 0, 0, 0 });
                    components.simpleDice[3].Name = "nup_fours_" + i;
                    components.simpleDice[3].Size = new System.Drawing.Size(56, 20);
                    components.simpleDice[3].TabIndex = 13;
                    components.simpleDice[3].ValueChanged += new System.EventHandler(nup_ext_ValueChanged);
                    components.simpleDice[3].Tag = new prevValExt(0, i);
                    // 
                    // lbl_threes
                    // 
                    components.labels[labelIndex].AutoSize = true;
                    components.labels[labelIndex].Location = new System.Drawing.Point(6, 122);
                    components.labels[labelIndex].Name = "lbl_threes_" + i;
                    components.labels[labelIndex].Size = new System.Drawing.Size(22, 13);
                    components.labels[labelIndex].TabIndex = 12;
                    components.labels[labelIndex].Text = "3er";
                    labelIndex++;
                    // 
                    // nup_threes
                    // 
                    components.simpleDice[2].Increment = new decimal(new int[] { 3, 0, 0, 0 });
                    components.simpleDice[2].Location = new System.Drawing.Point(48, 120);
                    components.simpleDice[2].Maximum = new decimal(new int[] { 15, 0, 0, 0 });
                    components.simpleDice[2].Name = "nup_threes_" + i;
                    components.simpleDice[2].Size = new System.Drawing.Size(56, 20);
                    components.simpleDice[2].TabIndex = 11;
                    components.simpleDice[2].ValueChanged += new System.EventHandler(nup_ext_ValueChanged);
                    components.simpleDice[2].Tag = new prevValExt(0, i);
                    // 
                    // lbl_twos
                    // 
                    components.labels[labelIndex].AutoSize = true;
                    components.labels[labelIndex].Location = new System.Drawing.Point(6, 96);
                    components.labels[labelIndex].Name = "lbl_twos_" + i;
                    components.labels[labelIndex].Size = new System.Drawing.Size(22, 13);
                    components.labels[labelIndex].TabIndex = 10;
                    components.labels[labelIndex].Text = "2er";
                    labelIndex++;
                    // 
                    // nup_twos
                    // 
                    components.simpleDice[1].Increment = new decimal(new int[] { 2, 0, 0, 0 });
                    components.simpleDice[1].Location = new System.Drawing.Point(48, 94);
                    components.simpleDice[1].Maximum = new decimal(new int[] { 10, 0, 0, 0 });
                    components.simpleDice[1].Name = "nup_twos_" + i;
                    components.simpleDice[1].Size = new System.Drawing.Size(56, 20);
                    components.simpleDice[1].TabIndex = 9;
                    components.simpleDice[1].ValueChanged += new System.EventHandler(nup_ext_ValueChanged);
                    components.simpleDice[1].Tag = new prevValExt(0, i);
                    // 
                    // tbx_name
                    // 
                    components.tbx_name.Location = new System.Drawing.Point(48, 25);
                    components.tbx_name.Name = "tbx_name_" + i;
                    components.tbx_name.Size = new System.Drawing.Size(100, 20);
                    components.tbx_name.TabIndex = 8;
                    // 
                    // lbl_ones
                    // 
                    components.labels[labelIndex].AutoSize = true;
                    components.labels[labelIndex].Location = new System.Drawing.Point(6, 70);
                    components.labels[labelIndex].Name = "lbl_ones_" + i;
                    components.labels[labelIndex].Size = new System.Drawing.Size(22, 13);
                    components.labels[labelIndex].TabIndex = 7;
                    components.labels[labelIndex].Text = "1er";
                    labelIndex++;
                    // 
                    // nup_ones
                    // 
                    components.simpleDice[0].Location = new System.Drawing.Point(48, 68);
                    components.simpleDice[0].Maximum = new decimal(new int[] { 5, 0, 0, 0 });
                    components.simpleDice[0].Name = "nup_ones_" + i;
                    components.simpleDice[0].Size = new System.Drawing.Size(56, 20);
                    components.simpleDice[0].TabIndex = 6;
                    components.simpleDice[0].ValueChanged += new System.EventHandler(nup_ext_ValueChanged);
                    components.simpleDice[0].Tag = new prevValExt(0, i);
                    // 
                    // cbx_fullHouse
                    // 
                    components.cbx_fullHouse.AutoSize = true;
                    components.cbx_fullHouse.Location = new System.Drawing.Point(46, 413);
                    components.cbx_fullHouse.Name = "cbx_fullHouse_" + i;
                    components.cbx_fullHouse.Size = new System.Drawing.Size(76, 17);
                    components.cbx_fullHouse.TabIndex = 5;
                    components.cbx_fullHouse.Text = "Full House";
                    components.cbx_fullHouse.UseVisualStyleBackColor = true;
                    components.cbx_fullHouse.CheckStateChanged += new System.EventHandler(updateEntries_cbx_ValueChanged);
                    components.cbx_fullHouse.Tag = i;
                    // 
                    // lbl_name
                    // 
                    components.labels[labelIndex].AutoSize = true;
                    components.labels[labelIndex].Location = new System.Drawing.Point(6, 28);
                    components.labels[labelIndex].Name = "lbl_name_" + i;
                    components.labels[labelIndex].Size = new System.Drawing.Size(38, 13);
                    components.labels[labelIndex].TabIndex = 4;
                    components.labels[labelIndex].Text = "Name:";
                    labelIndex++;
                    //
                    // lbl_twoPair
                    // 
                    components.labels[labelIndex].AutoSize = true;
                    components.labels[labelIndex].Location = new System.Drawing.Point(6, 331);
                    components.labels[labelIndex].Name = "lbl_twoPair_" + i;
                    components.labels[labelIndex].Size = new System.Drawing.Size(55, 13);
                    components.labels[labelIndex].TabIndex = 37;
                    components.labels[labelIndex].Text = "Zwei Paar";
                    labelIndex++;
                    // 
                    // nup_twoPair
                    // 
                    components.pairs[1].Increment = new decimal(new int[] { 2, 0, 0, 0 });
                    components.pairs[1].Location = new System.Drawing.Point(76, 329);
                    components.pairs[1].Maximum = new decimal(new int[] { 24, 0, 0, 0 });
                    components.pairs[1].Name = "nup_twoPair_" + i;
                    components.pairs[1].Size = new System.Drawing.Size(56, 20);
                    components.pairs[1].TabIndex = 36;
                    components.pairs[1].ValueChanged += new System.EventHandler(nup_ext_ValueChanged);
                    components.pairs[1].Tag = new prevValExt(4,i);
                    // 
                    // lbl_onePair
                    // 
                    components.labels[labelIndex].AutoSize = true;
                    components.labels[labelIndex].Location = new System.Drawing.Point(6, 305);
                    components.labels[labelIndex].Name = "lbl_onePair_" + i;
                    components.labels[labelIndex].Size = new System.Drawing.Size(47, 13);
                    components.labels[labelIndex].TabIndex = 35;
                    components.labels[labelIndex].Text = "Ein Paar";
                    labelIndex++;
                    // 
                    // nup_onePair
                    // 
                    components.pairs[0].Increment = new decimal(new int[] { 2, 0, 0, 0 });
                    components.pairs[0].Location = new System.Drawing.Point(76, 303);
                    components.pairs[0].Maximum = new decimal(new int[] { 12, 0, 0, 0 });
                    components.pairs[0].Name = "nup_onePair_" + 1;
                    components.pairs[0].Size = new System.Drawing.Size(56, 20);
                    components.pairs[0].TabIndex = 34;
                    components.pairs[0].ValueChanged += new System.EventHandler(nup_ext_ValueChanged);
                    components.pairs[0].Tag = new prevValExt(0, i);
                    // 
                    // lbl_ranking
                    // 
                    components.lbl_ranking.AutoSize = true;
                    components.lbl_ranking.Location = new System.Drawing.Point(45, 628);
                    components.lbl_ranking.Name = "lbl_ranking_" + i;
                    components.lbl_ranking.Size = new System.Drawing.Size(62, 13);
                    components.lbl_ranking.TabIndex = 37;
                    components.lbl_ranking.Text = str_ranking;

                    //
                    // cbx_forfit_*
                    //
                    int[] forfitsYposistions = new int[] { 68, 94, 120, 146, 172, 198, 303, 329, 355, 381, 413, 436, 460, 483, 515 };
                    int noTextCheckboxWith = 15;
                    int noTextCheckboxHeight = 14;
                    int forfitPosX = components.groupBox_player.Size.Width - noTextCheckboxWith - 5;

                    if (components.forfits.Length != forfitsYposistions.Length) MessageBox.Show("Array length mishmash!");

                    for (int j = 0; j < components.forfits.Length; j++)
                    {
                        components.forfits[j].AutoSize = true;
                        components.forfits[j].Location = new System.Drawing.Point(forfitPosX, forfitsYposistions[j]);
                        components.forfits[j].Name = "cbx_fullHouse_" + j + "_" + i;
                        components.forfits[j].Size = new System.Drawing.Size(noTextCheckboxWith, noTextCheckboxHeight);
                        components.forfits[j].TabIndex = 5;
                        components.forfits[j].Text = "";
                        components.forfits[j].UseVisualStyleBackColor = true;
                        components.forfits[j].Tag = new forfitID(i,j);
                        components.forfits[j].CheckedChanged += new System.EventHandler(cbx_forfit);
                    }


                    // 
                    // lbl_forfit
                    // 
                    components.labels[labelIndex].AutoSize = true;
                    components.labels[labelIndex].Name = "lbl_forfit_" + i;
                    components.labels[labelIndex].Size = new System.Drawing.Size(72, 13);
                    components.labels[labelIndex].TabIndex = 29;
                    components.labels[labelIndex].Text = "Streichen";

                    Point lbl_forfit_pos = new Point(
                        components.groupBox_player.Size.Width -  components.labels[labelIndex].Size.Width -5,
                        components.forfits[0].Location.Y - components.labels[labelIndex].Size.Height - 5
                    );

                    components.labels[labelIndex].Location = lbl_forfit_pos;
                    labelIndex++;

                    //
                    // lbl_numEntries
                    //
                    components.lbl_numEntries.AutoSize = true;
                    components.lbl_numEntries.Location = new System.Drawing.Point(45, 650);
                    components.lbl_numEntries.Name = "lbl_numEntries_" + i;
                    components.lbl_numEntries.Size = new System.Drawing.Size(62, 13);
                    components.lbl_numEntries.TabIndex = 37;
                    components.lbl_numEntries.Text = str_entries + "0";

                    // 
                    // Yahtzee From
                    // 

                    this.Controls.Add(components.groupBox_player);
                    components.groupBox_player.ResumeLayout(false);
                    components.groupBox_player.PerformLayout();

                    ((System.ComponentModel.ISupportInitialize)(components.nup_chance)).EndInit();
                    ((System.ComponentModel.ISupportInitialize)(components.nup_yahtzee)).EndInit();
                    for (int j = 0; j < components.xoak.Length; j++)
                        ((System.ComponentModel.ISupportInitialize)(components.xoak[j])).EndInit();
                    for (int j = 0; j < components.simpleDice.Length; j++)
                        ((System.ComponentModel.ISupportInitialize)(components.simpleDice[j])).EndInit();
                    for (int j = 0; j < components.pairs.Length; j++)
                        ((System.ComponentModel.ISupportInitialize)(components.pairs[j])).EndInit();

                    this.ResumeLayout(false);
                    this.PerformLayout();

                    playerSheets.Add(components);
                    prev_twoPairs_value.Add(0);

                }
            }
            nup_fontSize_ValueChanged(sender, e);
            resizeWindow();
        }

        private struct Score
        {
            public int index;
            public int score;

            public Score(int index, int score)
            {
                this.index = index;
                this.score = score;
            }
        };

        private void btn_calculate_Click(object sender, EventArgs e)
        {

            List<Score> scores = new List<Score>();

            for (int i = 0;i < playerSheets.Count; i++)
            {

                int topSum = 0, bottomSum = 0;

                foreach (NumericUpDown nup in playerSheets[i].simpleDice)
                    topSum += (int)nup.Value;

                playerSheets[i].lbl_result_simples.Text = "Gesammt: "+ topSum;

                if (topSum >= 63)
                {
                    playerSheets[i].lbl_bonusAt63.Text = "Bonus bei 63: 35";
                    topSum += 35;
                    playerSheets[i].lbl_topSum.Text = "Gesamt oben: "+ topSum;
                }
                else
                {
                    playerSheets[i].lbl_bonusAt63.Text = "Bonus bei 63: 0";
                    playerSheets[i].lbl_topSum.Text = "Gesamt oben: " + topSum;
                }

                foreach (NumericUpDown nup in playerSheets[i].xoak)
                    bottomSum += (int)nup.Value;
                foreach (NumericUpDown nup in playerSheets[i].pairs)
                    bottomSum += (int)nup.Value;
                bottomSum += (int)playerSheets[i].nup_chance.Value;

                if (playerSheets[i].cbx_fullHouse.Checked)
                    bottomSum += 25;
                if (playerSheets[i].cbx_smallStraight.Checked)
                    bottomSum += 30;
                if (playerSheets[i].cbx_largeStraight.Checked)
                    bottomSum += 40;
                switch (playerSheets[i].cbx_yahtzee.CheckState)
                {
                    case CheckState.Checked:
                        bottomSum += 50;
                        break;
                    case CheckState.Indeterminate:
                        bottomSum += (int)playerSheets[i].nup_yahtzee.Value;
                        break;
                }
                    

                playerSheets[i].lbl_bottomTopSum.Text = playerSheets[i].lbl_topSum.Text;
                playerSheets[i].lbl_bottomSum.Text = "Gessamt (unten): " + bottomSum;
                playerSheets[i].lbl_result.Text = "Endsumme: " + (topSum + bottomSum);

                scores.Add(new Score(i,topSum + bottomSum));
            }

            int rank = 1;

            while(scores.Count > 0)
            {
                int maxNum = 0;
                // get max score
                foreach (Score score in scores)
                {
                    if (score.score > maxNum)
                        maxNum = score.score;
                }

                // find all who are equal to the max score

                for (int i = 0; i < scores.Count; i++)
                {
                    if (scores[i].score == maxNum)
                    {
                        playerSheets[scores[i].index].lbl_ranking.Text = str_ranking + rank;

                        Font defaultFont = playerSheets[scores[i].index].lbl_ranking.Font;

                        if (rank == 1)
                            playerSheets[scores[i].index].lbl_ranking.Font = new Font(defaultFont.FontFamily, defaultFont.Size, FontStyle.Bold);
                        else
                            playerSheets[scores[i].index].lbl_ranking.Font = new Font(defaultFont.FontFamily, defaultFont.Size, FontStyle.Regular);

                        scores.RemoveAt(i);
                        i--;
                    }
                }

                rank++;
            }
        }


        private void btn_newGame_Click(object sender, EventArgs e)
        {
            incYahtzeeAt();
            for (int i = 0;i < playerSheets.Count; i++)
            {

                playerComponents updatedComponents = playerSheets.ElementAt(i);
                


                foreach (NumericUpDown simpleDice in updatedComponents.simpleDice)
                    simpleDice.Value = 0;
                foreach (NumericUpDown pairs in updatedComponents.pairs)
                    pairs.Value = 0;
                foreach (NumericUpDown xoak in updatedComponents.xoak)
                    xoak.Value = 0;

                updatedComponents.nup_chance.Value = 0;

                updatedComponents.cbx_fullHouse.Checked = false;
                updatedComponents.cbx_smallStraight.Checked = false;
                updatedComponents.cbx_largeStraight.Checked = false;
                updatedComponents.cbx_yahtzee.Checked = false;


                 foreach (CheckBox forfits in updatedComponents.forfits)
                            forfits.Checked = false;

                updatedComponents.lbl_result_simples.Text = str_result_simples;
                updatedComponents.lbl_bonusAt63.Text = str_bonusAt63;
                updatedComponents.lbl_topSum.Text = str_topSum;

                updatedComponents.lbl_bottomSum.Text = str_bottomSum;
                updatedComponents.lbl_bottomTopSum.Text = str_bottomTopSum;
                updatedComponents.lbl_result.Text = str_result;


                updatedComponents.lbl_ranking.Text = str_ranking;

                Font defaultFont = updatedComponents.lbl_ranking.Font;
                updatedComponents.lbl_ranking.Font = new Font(defaultFont.FontFamily, defaultFont.Size, FontStyle.Regular);

                updatedComponents.entries = 0;

                playerSheets[i] = updatedComponents;
            }
        }

        private int countEntries(int i)
        {
            return 55;
        }

       

        private void nup_fontSize_ValueChanged(object sender, EventArgs e)
        {
            Size nupSize = new Size(32, 20);
            changeFontSize(lbl_playerCount);

            changeFontSize(nup_playerCount);
            adjustLength(nup_playerCount, nupSize);
            repositionAndRefont(nup_playerCount, lbl_playerCount, 7, nup_playerCount.Location.Y);

            repositionAndRefont(btn_playerCount, nup_playerCount, 6, btn_playerCount.Location.Y);



            int orginalPos = nup_fontSize.Location.X;

            changeFontSize(nup_fontSize);
            adjustLength(nup_fontSize, nupSize);
            repositionAndRefont(nup_fontSize, orginalPos - (int)((nup_fontSize.Width - nupSize.Width) / (nup_fontSize.Value / 2)), nup_fontSize.Location.Y); // it somehow works. this makes sure the thing is visiable and does not wander to the left

            repositionAndRefont(lbl_fontSize, nup_fontSize, -6, lbl_fontSize.Location.Y);
            changeFontSize(cbx_autoSize);
            repositionAndRefont(cbx_autoSize, nup_fontSize.Location.X + nup_fontSize.Width - cbx_autoSize.Width, lbl_fontSize, 10);

            changeFontSize(btn_newGame);
            repositionAndRefont(lbl_yahtzeeAt, btn_newGame, 7, btn_newGame.Location.Y + (lbl_yahtzeeAt.Height / 2));
            changeFontSize(btn_calculate);

            nup_fontSize_valueChanged_groupBoxPositioning();
            resizeWindow();
        }
        private void nup_fontSize_valueChanged_groupBoxPositioning()
        {
            this.AutoScrollPosition = new Point(0, 0);
            this.AutoScroll = false;

            Size nupSize = new Size(56, 20);
            const int firstGroupBoxOffset = 11;
            for (int i = 0; i < playerSheets.Count; i++)
            {
                playerComponents components = playerSheets.ElementAt(i);

                if (i == 0)
                {
                    repositionAndRefont(components.groupBox_player, firstGroupBoxOffset, lbl_playerCount, 11);
                }
                else if (i != 0)
                {
                    repositionAndRefont(components.groupBox_player, playerSheets.ElementAt(i - 1).groupBox_player, 15, lbl_playerCount, 11);
                }

                // see posRef.png for an bad visual explenation
                const int leftMargin = 6;

                repositionAndRefont(findControl(components.labels, "lbl_name_" + i), leftMargin, 20 + (int)nup_fontSize.Value);
                repositionAndRefont(components.tbx_name, findControl(components.labels, "lbl_name_" + i), 4, 20 + (int)nup_fontSize.Value);


                string[] numsNames = new string[] { "lbl_ones_", "lbl_twos_", "lbl_threes_", "lbl_fours_", "lbl_fives_", "lbl_sixes_" };
                int buffer = TextRenderer.MeasureText(findControl(components.labels, "lbl_forfit_" + i).Text, lbl_fontSize.Font).Height;

                repositionAndRefont(components.simpleDice[0], components.tbx_name.Location.X, components.tbx_name, 13 + buffer);
                adjustLength(components.simpleDice[0], nupSize);
                repositionAndRefont(findControl(components.labels, numsNames[0] + i), leftMargin, components.simpleDice[0].Location.Y + 2);
                for (int j = 1; j < components.simpleDice.Length; j++)
                {
                    repositionAndRefont(components.simpleDice[j], components.tbx_name.Location.X, components.simpleDice[j - 1], 5);
                    adjustLength(components.simpleDice[j], nupSize);
                    repositionAndRefont(findControl(components.labels, numsNames[j] + i), leftMargin, components.simpleDice[j].Location.Y + 2);
                }

                repositionAndRefont(components.lbl_result_simples, components.tbx_name.Location.X, components.simpleDice[5], 14);
                repositionAndRefont(components.lbl_bonusAt63, components.tbx_name.Location.X, components.lbl_result_simples, 13);
                repositionAndRefont(components.lbl_topSum, components.tbx_name.Location.X, components.lbl_bonusAt63, 13);


                {       // not the final position, but can already get important x values that we can use as anchor, even if the y values are wrong at this stage.
                    changeFontSize(findControl(components.labels, "lbl_toak_" + i));
                    repositionAndRefont(components.xoak[0], findControl(components.labels, "lbl_toak_" + i), 6, 380);
                }

                repositionAndRefont(components.pairs[0], components.xoak[0].Location.X, components.lbl_topSum, 7);
                repositionAndRefont(components.pairs[1], components.xoak[0].Location.X, components.pairs[0], 7);
                repositionAndRefont(components.xoak[0], components.xoak[0].Location.X, components.pairs[1], 7);
                repositionAndRefont(components.xoak[1], components.xoak[0].Location.X, components.xoak[0], 7);

                adjustLength(components.pairs[0], nupSize);
                adjustLength(components.pairs[1], nupSize);
                adjustLength(components.xoak[0], nupSize);
                adjustLength(components.xoak[1], nupSize);

                repositionAndRefont(findControl(components.labels, "lbl_onePair_" + i), leftMargin, components.pairs[0].Location.Y + 2);
                repositionAndRefont(findControl(components.labels, "lbl_twoPair_" + i), leftMargin, components.pairs[1].Location.Y + 2);
                repositionAndRefont(findControl(components.labels, "lbl_toak_" + i), leftMargin, components.xoak[0].Location.Y + 2);
                repositionAndRefont(findControl(components.labels, "lbl_voak_" + i), leftMargin, components.xoak[1].Location.Y + 2);


                repositionAndRefont(components.cbx_fullHouse, components.tbx_name.Location.X, components.xoak[1], 7);
                repositionAndRefont(components.cbx_smallStraight, components.tbx_name.Location.X, components.cbx_fullHouse, 7);
                repositionAndRefont(components.cbx_largeStraight, components.tbx_name.Location.X, components.cbx_smallStraight, 7);
                repositionAndRefont(components.cbx_yahtzee, components.tbx_name.Location.X, components.cbx_largeStraight, 7);

                repositionAndRefont(components.nup_yahtzee, components.cbx_yahtzee.Location.X + 18, components.cbx_yahtzee.Location.Y);
                adjustLength(components.nup_yahtzee, nupSize);
                repositionAndRefont(findControl(components.labels, "lbl_yahtzee_" + i), leftMargin, components.cbx_yahtzee.Location.Y + 2);

                repositionAndRefont(components.nup_chance, components.xoak[0].Location.X, components.nup_yahtzee, 8);
                adjustLength(components.nup_chance, nupSize);
                repositionAndRefont(findControl(components.labels, "lbl_chance_" + i), leftMargin, components.nup_chance.Location.Y + 2);

                repositionAndRefont(components.lbl_bottomSum, components.tbx_name.Location.X, components.nup_chance, 14);
                repositionAndRefont(components.lbl_bottomTopSum, components.tbx_name.Location.X, components.lbl_bottomSum, 13);
                repositionAndRefont(components.lbl_result, components.tbx_name.Location.X, components.lbl_bottomTopSum, 13);
                repositionAndRefont(components.lbl_ranking, components.tbx_name.Location.X, components.lbl_result, 13);
                repositionAndRefont(findControl(components.labels, "lbl_numEntries_" + i), components.tbx_name.Location.X, components.lbl_ranking, 13);



                //
                // forfit checkboxes
                //

                repositionAndRefont(components.forfits[8], components.xoak[0], 8, components.xoak[1].Location.Y);

                for (int j = 0; j < components.simpleDice.Length; j++)
                    repositionAndRefont(components.forfits[j], components.forfits[8].Location.X, components.simpleDice[j].Location.Y);

                repositionAndRefont(components.forfits[6], components.forfits[8].Location.X, components.pairs[0].Location.Y);
                repositionAndRefont(components.forfits[7], components.forfits[8].Location.X, components.pairs[1].Location.Y);
                repositionAndRefont(components.forfits[9], components.forfits[8].Location.X, components.xoak[0].Location.Y);

                repositionAndRefont(components.forfits[12], components.cbx_largeStraight, 3, 0);

                repositionAndRefont(components.forfits[10], components.forfits[12].Location.X, components.cbx_fullHouse.Location.Y);
                repositionAndRefont(components.forfits[11], components.forfits[12].Location.X, components.cbx_smallStraight.Location.Y);
                repositionAndRefont(components.forfits[12], components.forfits[12].Location.X, components.cbx_largeStraight.Location.Y);
                repositionAndRefont(components.forfits[13], components.forfits[12].Location.X, components.cbx_yahtzee.Location.Y);

                repositionAndRefont(components.forfits[14], components.forfits[8].Location.X, components.nup_chance.Location.Y);


                repositionAndRefont(findControl(components.labels, "lbl_forfit_" + i), components.forfits[8].Location.X - findControl(components.labels, "lbl_forfit_" + i).Width + components.forfits[8].Width, components.tbx_name, 5);

                //components.tbx_name.Size = new Size((int)((float)(100.0 * (float)((float)components.tbx_name.Height / (float)20))), 20);
                adjustLength(components.tbx_name, new Size(100, 20));

            }
            this.AutoScroll = true;
        }


        private void adjustLength(Control control, Size orginalSize)
        {
            if (control == null) return;
            changeFontSize(control);
            control.Size = new Size((int)((float)(orginalSize.Width * (float)((float)control.Height / (float)orginalSize.Height))), orginalSize.Height);
        } // adjust the lenght of textboxes as they scale due to font size

        private Control findControl(Control[] controls, string name)
        {
            foreach (Control control in controls)
                if (control.Name.Equals(name))
                    return control;

            return null;
        }  // finds a control based on its name




        private void changeFontSize(Control Target)
        {
            Font font = Target.Font;
            Target.Font = new Font(font.FontFamily, (float)nup_fontSize.Value);
        } // changes the font, but does not reposition the element


        //
        // these 4 funcions reposition the target Control either based on a fixed offest or based on an offset from another Control
        //

        private void repositionAndRefont(Control Target, int horizontalDistance, int verticalDistance)
        {
            if (Target == null) return;
            changeFontSize(Target);
            Target.Location = new Point(horizontalDistance,         verticalDistance);
        }
     
        private void repositionAndRefont(Control Target, Control horizontalAnkor, int horizontalDistance, int verticalDistance)
        {
            if (Target == null) return;
            changeFontSize(Target);

            int horizontalSizeADJ = horizontalAnkor.Width;
            if (horizontalDistance < 0) horizontalSizeADJ = -(Target.Width);

            Target.Location = new Point(horizontalAnkor.Location.X + horizontalDistance + horizontalSizeADJ,        verticalDistance);
        }

        private void repositionAndRefont(Control Target, Control horizontalAnkor, int horizontalDistance, Control verticalAnkor, int verticalDistance)
        {
            if (Target == null) return;
            changeFontSize(Target);

            int horizontalSizeADJ = horizontalAnkor.Width;
            if (horizontalDistance < 0) horizontalSizeADJ = -(Target.Width);

            int verticalSizeADJ = verticalAnkor.Height;
            if (verticalDistance < 0) verticalSizeADJ = -(Target.Height);

            Target.Location = new Point(horizontalAnkor.Location.X + horizontalDistance + horizontalSizeADJ,         verticalAnkor.Location.Y + verticalDistance + verticalSizeADJ);
        }

        private void repositionAndRefont(Control Target, int horizontalDistance, Control verticalAnkor, int verticalDistance)
        {
            if (Target == null) return;
            changeFontSize(Target);

            int verticalSizeADJ = verticalAnkor.Height;
            if (verticalDistance < 0) verticalSizeADJ = -(Target.Height);

            Target.Location = new Point(horizontalDistance, verticalAnkor.Location.Y + verticalDistance + verticalSizeADJ);
        }


  




        private void resizeWindow()
        {
            if (!cbx_autoSize.Checked) return;

            Size diff = new Size(15,15) + this.Size - this.ClientSize;
            GroupBox grb = playerSheets[playerSheets.Count - 1].groupBox_player;
            this.Size = new Size(grb.Location.X + grb.Size.Width + diff.Width, btn_newGame.Location.Y + btn_newGame.Size.Height + diff.Height);


            this.Update();
            // readhust if elements are overlapping
            int mov = lbl_fontSize.Location.X - (btn_playerCount.Location.X + btn_playerCount.Width + 15);
            if (mov < 0)
            {
                mov = 0 - mov; // make mov positive
                this.Size = new Size(this.Size.Width + mov, this.Size.Height);
            }
            correctOutOfBounds(this);
        }

        private void correctOutOfBounds(Form form)
        {
            // Get the current screen
            Screen currentScreen = Screen.FromControl(form);

            // Calculate the new bounds of the form
            Rectangle newBounds = form.Bounds;

            // Ensure the new width doesn't go beyond the screen width
            if (newBounds.Right > currentScreen.WorkingArea.Right)
            {
                newBounds.Width = currentScreen.WorkingArea.Right - newBounds.Left;
            }

            // Ensure the new height doesn't go beyond the screen height
            if (newBounds.Bottom > currentScreen.WorkingArea.Bottom)
            {
                newBounds.Height = currentScreen.WorkingArea.Bottom - newBounds.Top;
            }

            // Apply the new bounds to the form
            form.Bounds = newBounds;
        }

        private void cbx_autoSize_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
                resizeWindow();
        }
    }
}
