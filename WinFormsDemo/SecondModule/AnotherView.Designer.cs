namespace SecondPrismModule
{
    partial class AnotherView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.itemNameTB = new System.Windows.Forms.TextBox();
            this.btnAddItem = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // itemNameTB
            // 
            this.itemNameTB.Location = new System.Drawing.Point(19, 50);
            this.itemNameTB.Name = "itemNameTB";
            this.itemNameTB.Size = new System.Drawing.Size(153, 20);
            this.itemNameTB.TabIndex = 1;
            this.itemNameTB.TextChanged += new System.EventHandler(this.itemNameTB_TextChanged);
            // 
            // btnAddItem
            // 
            this.btnAddItem.Location = new System.Drawing.Point(178, 48);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(75, 23);
            this.btnAddItem.TabIndex = 2;
            this.btnAddItem.Text = "Create Item";
            this.btnAddItem.UseVisualStyleBackColor = true;
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // AnotherView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnAddItem);
            this.Controls.Add(this.itemNameTB);
            this.Controls.Add(this.label1);
            this.Name = "AnotherView";
            this.Size = new System.Drawing.Size(372, 150);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox itemNameTB;
        private System.Windows.Forms.Button btnAddItem;
    }
}
