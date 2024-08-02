using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

public class SitemapFetcher
{
    public async Task<List<string>> FetchLinksAsync(string sitemapUrl)
    {
        using (HttpClient client = new HttpClient())
        {
            string xmlContent = await client.GetStringAsync(sitemapUrl);
            XDocument xmlDoc = XDocument.Parse(xmlContent);
            return xmlDoc.Descendants(XName.Get("loc", "http://www.sitemaps.org/schemas/sitemap/0.9"))
                          .Select(loc => loc.Value)
                          .ToList();
        }
    }
}