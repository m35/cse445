using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Project3
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {

        /// <summary>
        /// Get solar intensity as a geo-location.
        /// </summary>
        /// <remarks>
        /// A service that returns the annual average sunshine index of a given position (latitude,
        /// longitude). This service can be used for deciding if installing solar energy device is
        /// effective at the location.
        /// </remarks>
        /// <param name="latitude">-90 to 90</param>
        /// <param name="longitude">-180 to 180</param>
        /// <returns>
        /// A number reflecting the annual average solar intensity at the location,
        /// or -1 if no data exists at that location.
        /// </returns>
        [OperationContract]
        decimal SolarIntensity(decimal latitude, decimal longitude);

        /// <summary>
        /// Filter tags and function words out of a string.
        /// </summary>
        /// <remarks>
        /// Analyze a string of words and filter out the function words (stop words) such as "a",
        /// "an", "in", "on", "the", "is", "are", "am", as well as the element tag names and
        /// attribute names quoted in angle brackets &lt;...&gt;, if the string represents an XML
        /// page or HTML source page.
        /// </remarks>
        /// <param name="text">A string</param>
        /// <returns>A string with the stop words removed</returns>
        [OperationContract]
        string WordFilter(string text);

        [OperationContract]
        string[] EcoFriendlySoftware(int count);

        [OperationContract]
        string[] EcoFriendlyProducts(int count);
    }


}
