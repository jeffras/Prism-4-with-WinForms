namespace FirstPrismModule
{
    partial class ModuleView
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
            this.components = new System.ComponentModel.Container();
            this.SaveSelection = new System.Windows.Forms.Button();
            this.ProductsCB = new System.Windows.Forms.ComboBox();
            this.ProductsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ProductsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // SaveSelection
            // 
            this.SaveSelection.Location = new System.Drawing.Point(138, 58);
            this.SaveSelection.Name = "SaveSelection";
            this.SaveSelection.Size = new System.Drawing.Size(101, 23);
            this.SaveSelection.TabIndex = 0;
            this.SaveSelection.Text = "Save Selection";
            this.SaveSelection.UseVisualStyleBackColor = true;
            this.SaveSelection.Click += new System.EventHandler(this.OnSaveSelection);
            // 
            // ProductsCB
            // 
            this.ProductsCB.DataSource = this.ProductsBindingSource;
            this.ProductsCB.DisplayMember = "Name";
            this.ProductsCB.FormattingEnabled = true;
            this.ProductsCB.Location = new System.Drawing.Point(19, 20);
            this.ProductsCB.Name = "ProductsCB";
            this.ProductsCB.Size = new System.Drawing.Size(220, 21);
            this.ProductsCB.TabIndex = 2;
            // 
            // ProductsBindingSource
            // 
            this.ProductsBindingSource.DataMember = "Products";
            this.ProductsBindingSource.DataSource = typeof(FirstPrismModule.ModuleView_ViewModel);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(19, 58);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(101, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Save All";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ModuleView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ProductsCB);
            this.Controls.Add(this.SaveSelection);
            this.Name = "ModuleView";
            this.Size = new System.Drawing.Size(256, 104);
            ((System.ComponentModel.ISupportInitialize)(this.ProductsBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button SaveSelection;
        private System.Windows.Forms.ComboBox ProductsCB;
        private System.Windows.Forms.BindingSource ProductsBindingSource;
        private System.Windows.Forms.Button button1;
    }
}
