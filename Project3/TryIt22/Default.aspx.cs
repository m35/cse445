using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TryIt22
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLookupSolar_Click(object sender, EventArgs e)
        {
            try
            {
                decimal lat = decimal.Parse(txtLat.Text);
                decimal lon = decimal.Parse(txtLon.Text);

                decimal solarIndex = -1;

                if (solarIndex == -1)
                {
                    lblSolarIndex.Text = String.Format("No solar data available at {0} {1}", txtLat.Text, txtLon.Text);
                }
                else
                {
                    lblSolarIndex.Text = String.Format("The solar intensity index at {0} {1} is {2}", txtLat.Text, txtLon.Text, solarIndex);
                }
            }
            catch (Exception ex)
            {
                lblSolarIndex.Text = ex.Message;
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string text = txtText.Text;

            string filteredText = text;

            txtFiltered.Text = filteredText;
        }
    }
}