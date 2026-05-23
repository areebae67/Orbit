namespace applicationform
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.avdform = new System.Windows.Forms.Label();
            this.company = new System.Windows.Forms.Label();
            this.contact = new System.Windows.Forms.Label();
            this.address = new System.Windows.Forms.Label();
            this.city = new System.Windows.Forms.Label();
            this.state = new System.Windows.Forms.Label();
            this.zip = new System.Windows.Forms.Label();
            this.phone = new System.Windows.Forms.Label();
            this.total = new System.Windows.Forms.Label();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.txtCompany = new System.Windows.Forms.TextBox();
            this.txtContact = new System.Windows.Forms.TextBox();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.txtCity = new System.Windows.Forms.TextBox();
            this.txtState = new System.Windows.Forms.TextBox();
            this.txtZip = new System.Windows.Forms.TextBox();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.rdoFull = new System.Windows.Forms.RadioButton();
            this.Adtype = new System.Windows.Forms.Label();
            this.rdoHalf = new System.Windows.Forms.RadioButton();
            this.rdoQuarter = new System.Windows.Forms.RadioButton();
            this.rdoClassified = new System.Windows.Forms.RadioButton();
            this.payment = new System.Windows.Forms.Label();
            this.rdoVisa = new System.Windows.Forms.RadioButton();
            this.rdoMaster = new System.Windows.Forms.RadioButton();
            this.rdoAmex = new System.Windows.Forms.RadioButton();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.freak = new System.Windows.Forms.Label();
            this.weekdays = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // avdform
            // 
            this.avdform.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.avdform.Location = new System.Drawing.Point(395, 31);
            this.avdform.Name = "avdform";
            this.avdform.Size = new System.Drawing.Size(210, 35);
            this.avdform.TabIndex = 0;
            this.avdform.Text = "Advertising Form";
            this.avdform.Click += new System.EventHandler(this.avdform_Click);
            // 
            // company
            // 
            this.company.AutoSize = true;
            this.company.Location = new System.Drawing.Point(60, 81);
            this.company.Name = "company";
            this.company.Size = new System.Drawing.Size(82, 13);
            this.company.TabIndex = 1;
            this.company.Text = "Company Name";
            // 
            // contact
            // 
            this.contact.AutoSize = true;
            this.contact.Location = new System.Drawing.Point(63, 107);
            this.contact.Name = "contact";
            this.contact.Size = new System.Drawing.Size(75, 13);
            this.contact.TabIndex = 2;
            this.contact.Text = "Contact Name";
            // 
            // address
            // 
            this.address.AutoSize = true;
            this.address.Location = new System.Drawing.Point(60, 133);
            this.address.Name = "address";
            this.address.Size = new System.Drawing.Size(92, 13);
            this.address.TabIndex = 3;
            this.address.Text = "Company Address";
            // 
            // city
            // 
            this.city.AutoSize = true;
            this.city.Location = new System.Drawing.Point(63, 155);
            this.city.Name = "city";
            this.city.Size = new System.Drawing.Size(24, 13);
            this.city.TabIndex = 4;
            this.city.Text = "City";
            // 
            // state
            // 
            this.state.AutoSize = true;
            this.state.Location = new System.Drawing.Point(213, 155);
            this.state.Name = "state";
            this.state.Size = new System.Drawing.Size(32, 13);
            this.state.TabIndex = 5;
            this.state.Text = "State";
            // 
            // zip
            // 
            this.zip.AutoSize = true;
            this.zip.Location = new System.Drawing.Point(397, 155);
            this.zip.Name = "zip";
            this.zip.Size = new System.Drawing.Size(22, 13);
            this.zip.TabIndex = 6;
            this.zip.Text = "Zip";
            // 
            // phone
            // 
            this.phone.AutoSize = true;
            this.phone.Location = new System.Drawing.Point(63, 194);
            this.phone.Name = "phone";
            this.phone.Size = new System.Drawing.Size(38, 13);
            this.phone.TabIndex = 7;
            this.phone.Text = "Phone";
            // 
            // total
            // 
            this.total.AutoSize = true;
            this.total.Location = new System.Drawing.Point(574, 411);
            this.total.Name = "total";
            this.total.Size = new System.Drawing.Size(31, 13);
            this.total.TabIndex = 8;
            this.total.Text = "Total";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(611, 449);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 23);
            this.btnSubmit.TabIndex = 9;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(611, 507);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 10;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(611, 478);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 11;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // txtCompany
            // 
            this.txtCompany.Location = new System.Drawing.Point(159, 74);
            this.txtCompany.Name = "txtCompany";
            this.txtCompany.Size = new System.Drawing.Size(100, 20);
            this.txtCompany.TabIndex = 12;
            // 
            // txtContact
            // 
            this.txtContact.Location = new System.Drawing.Point(159, 100);
            this.txtContact.Name = "txtContact";
            this.txtContact.Size = new System.Drawing.Size(100, 20);
            this.txtContact.TabIndex = 13;
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(159, 126);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(100, 20);
            this.txtAddress.TabIndex = 14;
            // 
            // txtCity
            // 
            this.txtCity.Location = new System.Drawing.Point(93, 155);
            this.txtCity.Name = "txtCity";
            this.txtCity.Size = new System.Drawing.Size(100, 20);
            this.txtCity.TabIndex = 15;
            // 
            // txtState
            // 
            this.txtState.Location = new System.Drawing.Point(251, 155);
            this.txtState.Name = "txtState";
            this.txtState.Size = new System.Drawing.Size(100, 20);
            this.txtState.TabIndex = 16;
            // 
            // txtZip
            // 
            this.txtZip.Location = new System.Drawing.Point(425, 155);
            this.txtZip.Name = "txtZip";
            this.txtZip.Size = new System.Drawing.Size(100, 20);
            this.txtZip.TabIndex = 17;
            // 
            // txtPhone
            // 
            this.txtPhone.Location = new System.Drawing.Point(107, 194);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(100, 20);
            this.txtPhone.TabIndex = 18;
            // 
            // rdoFull
            // 
            this.rdoFull.AutoSize = true;
            this.rdoFull.Location = new System.Drawing.Point(66, 298);
            this.rdoFull.Name = "rdoFull";
            this.rdoFull.Size = new System.Drawing.Size(69, 17);
            this.rdoFull.TabIndex = 19;
            this.rdoFull.TabStop = true;
            this.rdoFull.Text = "Full Page";
            this.rdoFull.UseVisualStyleBackColor = true;
            // 
            // Adtype
            // 
            this.Adtype.AutoSize = true;
            this.Adtype.Location = new System.Drawing.Point(63, 268);
            this.Adtype.Name = "Adtype";
            this.Adtype.Size = new System.Drawing.Size(47, 13);
            this.Adtype.TabIndex = 20;
            this.Adtype.Text = "Ad Type";
            // 
            // rdoHalf
            // 
            this.rdoHalf.AutoSize = true;
            this.rdoHalf.Location = new System.Drawing.Point(66, 321);
            this.rdoHalf.Name = "rdoHalf";
            this.rdoHalf.Size = new System.Drawing.Size(71, 17);
            this.rdoHalf.TabIndex = 21;
            this.rdoHalf.TabStop = true;
            this.rdoHalf.Text = "Half page";
            this.rdoHalf.UseVisualStyleBackColor = true;
            // 
            // rdoQuarter
            // 
            this.rdoQuarter.AutoSize = true;
            this.rdoQuarter.Location = new System.Drawing.Point(66, 344);
            this.rdoQuarter.Name = "rdoQuarter";
            this.rdoQuarter.Size = new System.Drawing.Size(88, 17);
            this.rdoQuarter.TabIndex = 22;
            this.rdoQuarter.TabStop = true;
            this.rdoQuarter.Text = " Quater Page";
            this.rdoQuarter.UseVisualStyleBackColor = true;
            // 
            // rdoClassified
            // 
            this.rdoClassified.AutoSize = true;
            this.rdoClassified.Location = new System.Drawing.Point(66, 367);
            this.rdoClassified.Name = "rdoClassified";
            this.rdoClassified.Size = new System.Drawing.Size(69, 17);
            this.rdoClassified.TabIndex = 23;
            this.rdoClassified.TabStop = true;
            this.rdoClassified.Text = "Classified";
            this.rdoClassified.UseVisualStyleBackColor = true;
            // 
            // payment
            // 
            this.payment.AutoSize = true;
            this.payment.Location = new System.Drawing.Point(397, 268);
            this.payment.Name = "payment";
            this.payment.Size = new System.Drawing.Size(87, 13);
            this.payment.TabIndex = 24;
            this.payment.Text = "Payment Method";
            // 
            // rdoVisa
            // 
            this.rdoVisa.AutoSize = true;
            this.rdoVisa.Location = new System.Drawing.Point(400, 298);
            this.rdoVisa.Name = "rdoVisa";
            this.rdoVisa.Size = new System.Drawing.Size(45, 17);
            this.rdoVisa.TabIndex = 25;
            this.rdoVisa.TabStop = true;
            this.rdoVisa.Text = "Visa";
            this.rdoVisa.UseVisualStyleBackColor = true;
            // 
            // rdoMaster
            // 
            this.rdoMaster.AutoSize = true;
            this.rdoMaster.Location = new System.Drawing.Point(400, 321);
            this.rdoMaster.Name = "rdoMaster";
            this.rdoMaster.Size = new System.Drawing.Size(82, 17);
            this.rdoMaster.TabIndex = 26;
            this.rdoMaster.TabStop = true;
            this.rdoMaster.Text = "Master Card";
            this.rdoMaster.UseVisualStyleBackColor = true;
            // 
            // rdoAmex
            // 
            this.rdoAmex.AutoSize = true;
            this.rdoAmex.Location = new System.Drawing.Point(400, 344);
            this.rdoAmex.Name = "rdoAmex";
            this.rdoAmex.Size = new System.Drawing.Size(109, 17);
            this.rdoAmex.TabIndex = 27;
            this.rdoAmex.TabStop = true;
            this.rdoAmex.Text = "American Express";
            this.rdoAmex.UseVisualStyleBackColor = true;
            // 
            // txtTotal
            // 
            this.txtTotal.Location = new System.Drawing.Point(611, 408);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(100, 20);
            this.txtTotal.TabIndex = 28;
            // 
            // freak
            // 
            this.freak.AutoSize = true;
            this.freak.Location = new System.Drawing.Point(63, 425);
            this.freak.Name = "freak";
            this.freak.Size = new System.Drawing.Size(54, 13);
            this.freak.TabIndex = 29;
            this.freak.Text = "frequency";
            // 
            // weekdays
            // 
            this.weekdays.FormattingEnabled = true;
            this.weekdays.Items.AddRange(new object[] {
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday",
            "Sunday"});
            this.weekdays.Location = new System.Drawing.Point(63, 449);
            this.weekdays.Name = "weekdays";
            this.weekdays.Size = new System.Drawing.Size(120, 94);
            this.weekdays.TabIndex = 30;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1003, 660);
            this.Controls.Add(this.weekdays);
            this.Controls.Add(this.freak);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.rdoAmex);
            this.Controls.Add(this.rdoMaster);
            this.Controls.Add(this.rdoVisa);
            this.Controls.Add(this.payment);
            this.Controls.Add(this.rdoClassified);
            this.Controls.Add(this.rdoQuarter);
            this.Controls.Add(this.rdoHalf);
            this.Controls.Add(this.Adtype);
            this.Controls.Add(this.rdoFull);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.txtZip);
            this.Controls.Add(this.txtState);
            this.Controls.Add(this.txtCity);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.txtContact);
            this.Controls.Add(this.txtCompany);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.total);
            this.Controls.Add(this.phone);
            this.Controls.Add(this.zip);
            this.Controls.Add(this.state);
            this.Controls.Add(this.city);
            this.Controls.Add(this.address);
            this.Controls.Add(this.contact);
            this.Controls.Add(this.company);
            this.Controls.Add(this.avdform);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label avdform;
        private System.Windows.Forms.Label company;
        private System.Windows.Forms.Label contact;
        private System.Windows.Forms.Label address;
        private System.Windows.Forms.Label city;
        private System.Windows.Forms.Label state;
        private System.Windows.Forms.Label zip;
        private System.Windows.Forms.Label phone;
        private System.Windows.Forms.Label total;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TextBox txtCompany;
        private System.Windows.Forms.TextBox txtContact;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.TextBox txtCity;
        private System.Windows.Forms.TextBox txtState;
        private System.Windows.Forms.TextBox txtZip;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.RadioButton rdoFull;
        private System.Windows.Forms.Label Adtype;
        private System.Windows.Forms.RadioButton rdoHalf;
        private System.Windows.Forms.RadioButton rdoQuarter;
        private System.Windows.Forms.RadioButton rdoClassified;
        private System.Windows.Forms.Label payment;
        private System.Windows.Forms.RadioButton rdoVisa;
        private System.Windows.Forms.RadioButton rdoMaster;
        private System.Windows.Forms.RadioButton rdoAmex;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.Label freak;
        private System.Windows.Forms.CheckedListBox weekdays;
    }
}

