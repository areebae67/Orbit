using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace applicationform
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void avdform_Click(object sender, EventArgs e)
        {

        }

       
            private void btnSubmit(object sender, EventArgs e)
        {
            // Required Fields Validation
            if (string.IsNullOrWhiteSpace(txtCompany.Text) ||
                string.IsNullOrWhiteSpace(txtContact.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Please fill required fields.");
                return;
            }

            // Ad Type Price
            decimal price = 0;

            if (rdoFull.Checked)
                price = 1000;
            else if (rdoHalf.Checked)
                price = 600;
            else if (rdoQuarter.Checked)
                price = 400;
            else if (rdoClassified.Checked)
                price = 200;
            else
            {
                MessageBox.Show("Please select an Ad Type.");
                return;
            }

            // Payment Method Validation
            if (!rdoVisa.Checked && !rdoMaster.Checked && !rdoAmex.Checked)
            {
                MessageBox.Show("Please select a Payment Method.");
                return;
            }

            // Frequency Count
            int days = weekdays.CheckedItems.Count;

            if (days == 0)
            {
                MessageBox.Show("Please select at least one day.");
                return;
            }

            // Calculate Total
            decimal total = price * days;
            txtTotal.Text = total.ToString();

            MessageBox.Show("Advertisement Submitted Successfully!");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {

        }
    }
    }

