namespace Yahtzee
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btn_playerCount = new System.Windows.Forms.Button();
            this.btn_calculate = new System.Windows.Forms.Button();
            this.lbl_playerCount = new System.Windows.Forms.Label();
            this.nup_playerCount = new System.Windows.Forms.NumericUpDown();
            this.btn_newGame = new System.Windows.Forms.Button();
            this.lbl_yahtzeeAt = new System.Windows.Forms.Label();
            this.nup_fontSize = new System.Windows.Forms.NumericUpDown();
            this.lbl_fontSize = new System.Windows.Forms.Label();
            this.cbx_autoSize = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nup_playerCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nup_fontSize)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_playerCount
            // 
            this.btn_playerCount.AutoSize = true;
            this.btn_playerCount.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_playerCount.Location = new System.Drawing.Point(92, 23);
            this.btn_playerCount.Name = "btn_playerCount";
            this.btn_playerCount.Size = new System.Drawing.Size(68, 23);
            this.btn_playerCount.TabIndex = 1;
            this.btn_playerCount.Text = "Anwenden";
            this.btn_playerCount.UseVisualStyleBackColor = true;
            this.btn_playerCount.Click += new System.EventHandler(this.btn_playerCount_Click);
            // 
            // btn_calculate
            // 
            this.btn_calculate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_calculate.AutoSize = true;
            this.btn_calculate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_calculate.Location = new System.Drawing.Point(848, 731);
            this.btn_calculate.Name = "btn_calculate";
            this.btn_calculate.Size = new System.Drawing.Size(74, 23);
            this.btn_calculate.TabIndex = 2;
            this.btn_calculate.Text = "Ausrechnen";
            this.btn_calculate.UseVisualStyleBackColor = true;
            this.btn_calculate.Click += new System.EventHandler(this.btn_calculate_Click);
            // 
            // lbl_playerCount
            // 
            this.lbl_playerCount.AutoSize = true;
            this.lbl_playerCount.Location = new System.Drawing.Point(11, 25);
            this.lbl_playerCount.Name = "lbl_playerCount";
            this.lbl_playerCount.Size = new System.Drawing.Size(42, 13);
            this.lbl_playerCount.TabIndex = 3;
            this.lbl_playerCount.Text = "Spieler:";
            this.lbl_playerCount.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // nup_playerCount
            // 
            this.nup_playerCount.Location = new System.Drawing.Point(54, 23);
            this.nup_playerCount.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.nup_playerCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nup_playerCount.Name = "nup_playerCount";
            this.nup_playerCount.Size = new System.Drawing.Size(32, 20);
            this.nup_playerCount.TabIndex = 7;
            this.nup_playerCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btn_newGame
            // 
            this.btn_newGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_newGame.AutoSize = true;
            this.btn_newGame.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btn_newGame.Location = new System.Drawing.Point(12, 731);
            this.btn_newGame.Name = "btn_newGame";
            this.btn_newGame.Size = new System.Drawing.Size(74, 23);
            this.btn_newGame.TabIndex = 8;
            this.btn_newGame.Text = "Neues Spiel";
            this.btn_newGame.UseVisualStyleBackColor = true;
            this.btn_newGame.Click += new System.EventHandler(this.btn_newGame_Click);
            // 
            // lbl_yahtzeeAt
            // 
            this.lbl_yahtzeeAt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_yahtzeeAt.AutoSize = true;
            this.lbl_yahtzeeAt.Location = new System.Drawing.Point(92, 736);
            this.lbl_yahtzeeAt.Name = "lbl_yahtzeeAt";
            this.lbl_yahtzeeAt.Size = new System.Drawing.Size(65, 13);
            this.lbl_yahtzeeAt.TabIndex = 3;
            this.lbl_yahtzeeAt.Text = "Kniffel bei: 1";
            this.lbl_yahtzeeAt.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // nup_fontSize
            // 
            this.nup_fontSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nup_fontSize.Location = new System.Drawing.Point(890, 23);
            this.nup_fontSize.Maximum = new decimal(new int[] {
            72,
            0,
            0,
            0});
            this.nup_fontSize.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.nup_fontSize.Name = "nup_fontSize";
            this.nup_fontSize.Size = new System.Drawing.Size(32, 20);
            this.nup_fontSize.TabIndex = 12;
            this.nup_fontSize.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.nup_fontSize.ValueChanged += new System.EventHandler(this.nup_fontSize_ValueChanged);
            // 
            // lbl_fontSize
            // 
            this.lbl_fontSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_fontSize.AutoSize = true;
            this.lbl_fontSize.Location = new System.Drawing.Point(817, 25);
            this.lbl_fontSize.Name = "lbl_fontSize";
            this.lbl_fontSize.Size = new System.Drawing.Size(67, 13);
            this.lbl_fontSize.TabIndex = 13;
            this.lbl_fontSize.Text = "Schriftgröße:";
            // 
            // cbx_autoSize
            // 
            this.cbx_autoSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbx_autoSize.AutoSize = true;
            this.cbx_autoSize.Checked = true;
            this.cbx_autoSize.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbx_autoSize.Location = new System.Drawing.Point(831, 49);
            this.cbx_autoSize.Name = "cbx_autoSize";
            this.cbx_autoSize.Size = new System.Drawing.Size(91, 17);
            this.cbx_autoSize.TabIndex = 14;
            this.cbx_autoSize.Text = "Autom. Größe";
            this.cbx_autoSize.UseVisualStyleBackColor = true;
            this.cbx_autoSize.CheckedChanged += new System.EventHandler(this.cbx_autoSize_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMargin = new System.Drawing.Size(0, 40);
            this.ClientSize = new System.Drawing.Size(934, 766);
            this.Controls.Add(this.cbx_autoSize);
            this.Controls.Add(this.lbl_fontSize);
            this.Controls.Add(this.nup_fontSize);
            this.Controls.Add(this.btn_newGame);
            this.Controls.Add(this.nup_playerCount);
            this.Controls.Add(this.lbl_playerCount);
            this.Controls.Add(this.btn_calculate);
            this.Controls.Add(this.btn_playerCount);
            this.Controls.Add(this.lbl_yahtzeeAt);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(350, 250);
            this.Name = "MainForm";
            this.Text = "Kniffel Punkte Rechner";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nup_playerCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nup_fontSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btn_playerCount;
        private System.Windows.Forms.Button btn_calculate;
        private System.Windows.Forms.Label lbl_playerCount;
        private System.Windows.Forms.NumericUpDown nup_playerCount;
        private System.Windows.Forms.Button btn_newGame;
        private System.Windows.Forms.Label lbl_yahtzeeAt;
        private System.Windows.Forms.NumericUpDown nup_fontSize;
        private System.Windows.Forms.Label lbl_fontSize;
        private System.Windows.Forms.CheckBox cbx_autoSize;
    }
}

