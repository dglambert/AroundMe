using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AroundMe
{
    class FlickrImage
    {

        public Uri Image320 { get; set; }
        public Uri Image1024 { get; set; }

        public async static Task<List<FlickrImage>> GetFlickrImages(string flickrApiKey, string topic, double latitude = double.NaN, double longitude = double.NaN, double radius = double.NaN)
        { 
            HttpClient client = new HttpClient();

            var baseUrl = getBaseUrl(flickrApiKey, topic, latitude, longitude, radius);
            string flickrResult = "";
            try
            {
                flickrResult = await client.GetStringAsync(baseUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            FlickrData apiData = JsonConvert.DeserializeObject<FlickrData>(flickrResult);

            List<FlickrImage> images = new List<FlickrImage>();


            if(apiData.stat == "ok")
            {
                foreach(Photo data in apiData.photos.photo)
                {
                    FlickrImage img = new FlickrImage();

                    string photoUrl = "http://farm{0}.staticflickr.com/{1}/{2}_{3}";

                    string baseFlickrUrl = string.Format(photoUrl, data.farm, data.server, data.id, data.secret);

                    img.Image320 = new Uri(baseFlickrUrl + "_n.jpg");
                    img.Image1024 = new Uri(baseFlickrUrl + "_b.jpg");

                    images.Add(img);

                }
            }
            return images;
        }

        private static string getBaseUrl(string flickrApiKey, string topic, double latitude = double.NaN, double longitude = double.NaN, double radius = double.NaN)
        {
            // Licenses
            // http://www.flickr.com/services/api/flickr.photos.licenses.getInfo.html
            /*
                <license id="4" name="Attribution License" url="http://creativecommons.org/licenses/by/2.0/" />
                <license id="5" name="Attribution-ShareAlike License" url="http://creativecommons.org/licenses/by-sa/2.0/" />
                <license id="6" name="Attribution-NoDerivs License" url="http://creativecommons.org/licenses/by-nd/2.0/" />
                <license id="7" name="No known copyright restrictions" url="http://flickr.com/commons/usage/" />
             */

            string license = "4,5,6,7";
            license = license.Replace(",", "%2C");


            if(!double.IsNaN(latitude))
                latitude = Math.Round(latitude,5);

            if (!double.IsNaN(longitude))
                longitude = Math.Round(longitude, 5);


            // http://api.flickr.com/services/rest/?method=flickr.photos.search&api_key=ca4afcf60495c01b9bc775ab80e6382a&text=observatory&lat=41.8988&lon=-87.6123&radius=10&format=json&nojsoncallback=1&api_sig=6c1ad6748bf3d1c39ca4b4722b2ef0c8

            string url = "http://api.flickr.com/services/rest/" +
                "?method=flickr.photos.search" +
                "&license={0}" +
                "&api_key={1}" +
                "&format=json" +
                "&nojsoncallback=1";

            var baseUrl = string.Format(url, license, flickrApiKey);

            if (!string.IsNullOrWhiteSpace(topic))
                baseUrl += string.Format("&text=%22{0}%22", topic);

            if (!double.IsNaN(latitude) && !double.IsNaN(longitude))
                baseUrl += string.Format("&lat={0}&lon={1}", latitude, longitude);

            if (!double.IsNaN(radius))
                baseUrl += string.Format("&radius={0}", radius);

            return baseUrl;

        }

    }
}
