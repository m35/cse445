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

                Proj3Services.Service1Client srv = new Proj3Services.Service1Client();
                decimal solarIndex = srv.SolarIntensity(lat, lon);

                if (solarIndex == -1)
                {
                    lblSolarIndex.Text = String.Format("No solar data available at (latitude {0}, longitude {1})", txtLat.Text, txtLon.Text);
                }
                else
                {
                    lblSolarIndex.Text = String.Format("The solar intensity index at (latitude {0}, longitude {1}) is {2}", txtLat.Text, txtLon.Text, solarIndex);
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

            Proj3Services.Service1Client srv = new Proj3Services.Service1Client();
            string filteredText = srv.WordFilter(text);

            txtFiltered.Text = filteredText;
        }
    }
}