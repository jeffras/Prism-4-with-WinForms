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
            this.ProductsList = new System.Windows.Forms.ComboBox();
            this.ProductsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ProductsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // SaveSelection
            // 
            this.SaveSelection.Location = new System.Drawing.Point(72, 58);
            this.SaveSelection.Name = "SaveSelection";
            this.SaveSelection.Size = new System.Drawing.Size(167, 23);
            this.SaveSelection.TabIndex = 0;
            this.SaveSelection.Text = "Save Selection";
            this.SaveSelection.UseVisualStyleBackColor = true;
            this.SaveSelection.Click += new System.EventHandler(this.OnSaveSelection);
            // 
            // ProductsList
            // 
            this.ProductsList.DataSource = this.ProductsBindingSource;
            this.ProductsList.DisplayMember = "Name";
            this.ProductsList.FormattingEnabled = true;
            this.ProductsList.Location = new System.Drawing.Point(19, 20);
            this.ProductsList.Name = "ProductsList";
            this.ProductsList.Size = new System.Drawing.Size(220, 21);
            this.ProductsList.TabIndex = 2;
            // 
            // ProductsBindingSource
            // 
            this.ProductsBindingSource.DataMember = "Products";
            this.ProductsBindingSource.DataSource = typeof(ModuleView_ViewModel);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "label1";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // ModuleView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ProductsList);
            this.Controls.Add(this.SaveSelection);
            this.Name = "ModuleView";
            this.Size = new System.Drawing.Size(256, 104);
            ((System.ComponentModel.ISupportInitialize)(this.ProductsBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SaveSelection;
        private System.Windows.Forms.ComboBox ProductsList;
        private System.Windows.Forms.BindingSource ProductsBindingSource;
        private System.Windows.Forms.Label label1;
    }
}
