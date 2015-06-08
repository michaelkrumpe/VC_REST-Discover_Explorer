using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Net.Sockets;

namespace VC_REST_Discover_Explorer.VCCREST
{
    /*
     *  C#.NET Example Verizon Cloud API REST Client created by Michael Krumpe (michael.krumpe@verizon.com)
     *  API Documentation: http://cloud.verizon.com/documentation/
     */

    public class restapi
    {
        public string   accesskey = String.Empty, 
                        secretkey = String.Empty, 
                        cloudspace = String.Empty,
                        acctid = String.Empty,
                        strtosign = String.Empty,
                        uriroot_master, 
                        options;

        public static string apiuri { get; set; }
        public static string    root_compute    = "/api/compute/", 
                                root_admin      = "/api/admin/";


        public string getvmlist(string str_offset = "0", string str_limit = "100")
        {
            /* api notes:       GET https://{Verizon URI}/api/compute/vm[?[offset=x][&limit=y]]
                                public string options_example = "?[offset=x][&limit=y]";                */

            string  rURI = root_admin + "vm", 
                    apiuri = uriroot_master + rURI + "?"; if (str_offset != "") { apiuri += "offset=" + str_offset + "&"; } apiuri += "limit=" + str_limit;
            
            string  akey = accesskey, 
                    actionverb = "GET", 
                    acceptcontenttype = "application/vnd.terremark.ecloud.vm.v1+json",
                    str_xtmrkdate = x_tmrk_date(), 
                    str_xtmrknonce = x_tmrk_nonce();


            /* CanonicalizedHeaders string "x-tmrk" values only */
            ICollection<KeyValuePair<String, String>> HeaderCollec = new Dictionary<String, String>();
            HeaderCollec.Add(new KeyValuePair<String, String>("x-tmrk-nonce", str_xtmrknonce));
            if (actionverb != "GET") { HeaderCollec.Add(new KeyValuePair<String, String>("Accept", acceptcontenttype)); }
            if (cloudspace != "") { HeaderCollec.Add(new KeyValuePair<String, String>("x-tmrk-cloudspace", cloudspace)); }
            if (acctid != "") { HeaderCollec.Add(new KeyValuePair<String, String>("x-tmrk-acct", acctid)); }

            string headercano_str = SortCanoHeaders(HeaderCollec.Select(kvp => new CanonicalizedHeaders() { pname = kvp.Key, pvalue = kvp.Value }));


            /* CanonicalizedResource string - Resource URI after Root URL, and Querystring values only */
            ICollection<KeyValuePair<String, String>> ResourceCollec = new Dictionary<String, String>();
            if (str_offset != "") { ResourceCollec.Add(new KeyValuePair<String, String>("limit", str_limit)); }
                                    ResourceCollec.Add(new KeyValuePair<String, String>("offset", str_offset));

            string  resourcecano_str = SortCanoResources(ResourceCollec.Select(kvp => new CanonicalizedResources() { pname = kvp.Key, pvalue = kvp.Value }), 
                    rURI.Replace(uriroot_master, "")),
                    shacanosignature = hashSignature(actionverb, str_xtmrknonce, str_xtmrkdate, acceptcontenttype, headercano_str, resourcecano_str),
                    x_tmrk_authstr = x_tmrk_authorization(shacanosignature);

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            var vClient = new HttpClient();
            HttpRequestMessage vMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(apiuri));      //vMessage.Headers.Add("Accept", acceptcontenttype);
                vMessage.Headers.Add("x-tmrk-authorization", x_tmrk_authstr);
                vMessage.Headers.Add("Date", str_xtmrkdate);
                vMessage.Headers.Add("x-tmrk-nonce", str_xtmrknonce);
                if (acctid != "") { vMessage.Headers.Add("x-tmrk-acct", acctid); }
                if (cloudspace != "") { vMessage.Headers.Add("x-tmrk-cloudspace", cloudspace); }

            var clientResponse = vClient.SendAsync(vMessage).Result;
            string clientContent = clientResponse.Content.ReadAsStringAsync().Result;

            return clientContent;
        }

        public string testconn()
        {
            string  rURI = root_compute,
                    apiuri = uriroot_master + rURI, 
                    akey = accesskey,
                    actionverb = "GET",
                    acceptcontenttype = "",             //"application/vnd.terremark.ecloud.root.v1+json",
                    str_xtmrkdate = x_tmrk_date(),
                    str_xtmrknonce = x_tmrk_nonce();


            /* CanonicalizedHeaders string "x-tmrk" values only */
            ICollection<KeyValuePair<String, String>> HeaderCollec = new Dictionary<String, String>();
            
            if (actionverb != "GET") { HeaderCollec.Add(new KeyValuePair<String, String>("Accept", acceptcontenttype)); }
            if (acctid != "") { HeaderCollec.Add(new KeyValuePair<String, String>("x-tmrk-acct", acctid)); }
            if (cloudspace != "") { HeaderCollec.Add(new KeyValuePair<String, String>("x-tmrk-cloudspace", cloudspace)); }
            HeaderCollec.Add(new KeyValuePair<String, String>("x-tmrk-nonce", str_xtmrknonce));

            string headercano_str = SortCanoHeaders(HeaderCollec.Select(kvp => new CanonicalizedHeaders() { pname = kvp.Key, pvalue = kvp.Value }));


            /* CanonicalizedResource string - Resource URI after Root URL, and Querystring values only */
            ICollection<KeyValuePair<String, String>> ResourceCollec = new Dictionary<String, String>();

            string resourcecano_str = SortCanoResources(ResourceCollec.Select(kvp => new CanonicalizedResources() { pname = kvp.Key, pvalue = kvp.Value }),
                    rURI.Replace(uriroot_master, "")),
                    shacanosignature = hashSignature(actionverb, str_xtmrknonce, str_xtmrkdate, acceptcontenttype, headercano_str, resourcecano_str),
                    x_tmrk_authstr = x_tmrk_authorization(shacanosignature);

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            var vClient = new HttpClient();
            HttpRequestMessage vMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(apiuri));
            vMessage.Headers.Add("x-tmrk-authorization", x_tmrk_authstr);                           //x-tmrk-authorization
            vMessage.Headers.Add("Date", str_xtmrkdate);                                            //Date
            vMessage.Headers.Add("x-tmrk-nonce", str_xtmrknonce);                                   //x-tmrk-nonce
            if (acctid != "") { vMessage.Headers.Add("x-tmrk-acct", acctid); }                      //x-tmrk-acct
            if (cloudspace != "") { vMessage.Headers.Add("x-tmrk-cloudspace", cloudspace); }        //x-tmrk-cloudspace

            var clientResponse = vClient.SendAsync(vMessage).Result;
            //string clientContent = clientResponse.Content.ReadAsStringAsync().Result;
            string clientContent = clientResponse.StatusCode.ToString();

            return clientContent;
        }

        public string powervm(string vmURI)
        {
            string  akey = accesskey, actionverb = "POST", 
                    acceptcontenttype = "application/vnd.terremark.ecloud.job.v1+json",
                    str_xtmrkdate = x_tmrk_date(), 
                    str_xtmrknonce = x_tmrk_nonce();

            /* CanonicalizedHeaders string "x-tmrk" values only */
            ICollection<KeyValuePair<String, String>> HeaderCollec = new Dictionary<String, String>();
            //HeaderCollec.Add(new KeyValuePair<String, String>("Accept", acceptcontenttype));
            HeaderCollec.Add(new KeyValuePair<String, String>("x-tmrk-nonce", str_xtmrknonce));
            if (acctid != "") { HeaderCollec.Add(new KeyValuePair<String, String>("x-tmrk-acct", acctid)); }
            if (cloudspace != "") { HeaderCollec.Add(new KeyValuePair<String, String>("x-tmrk-cloudspace", cloudspace)); }

            
            string headercano_str = SortCanoHeaders(HeaderCollec.Select(kvp => new CanonicalizedHeaders() { pname = kvp.Key, pvalue = kvp.Value }));


            /* CanonicalizedResource string - Resource URI after Root URL, and Querystring values only */
            ICollection<KeyValuePair<String, String>> ResourceCollec = new Dictionary<String, String>();
            //ResourceCollec.Add(new KeyValuePair<String, String>("offset", str_offset));

            string resourcecano_str = SortCanoResources(ResourceCollec.Select(kvp => new CanonicalizedResources() { pname = kvp.Key, pvalue = kvp.Value }), vmURI.Replace(uriroot_master, ""));

            string shacanosignature = hashSignature(actionverb, str_xtmrknonce, str_xtmrkdate, acceptcontenttype, headercano_str, resourcecano_str);

            string x_tmrk_authstr = x_tmrk_authorization(shacanosignature);

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            var vClient = new HttpClient();     

            HttpRequestMessage vMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(vmURI));
            vMessage.Headers.Add("x-tmrk-authorization", x_tmrk_authstr);
            //vMessage.Headers.Add("Accept", acceptcontenttype);
            vMessage.Headers.Add("Date", str_xtmrkdate);
            vMessage.Headers.Add("x-tmrk-nonce", str_xtmrknonce); 
            if (acctid != "") { vMessage.Headers.Add("x-tmrk-acct", acctid); }
            if (cloudspace != "") { vMessage.Headers.Add("x-tmrk-cloudspace", cloudspace); }

            var clientResponse = vClient.SendAsync(vMessage).Result;
            string clientContent = clientResponse.Content.ReadAsStringAsync().Result;

            return clientContent;
        }

        public string getuserlist()
        {

            string  rURI = root_admin + "user/",                //https://iadg2.cloud.verizon.com/api/admin/user/
                    apiuri = uriroot_master + rURI;             //apiuri = "https://iadg2.cloud.verizon.com/api/admin/";
                   
            string  akey = accesskey,
                    actionverb = "GET",
                    acceptcontenttype = "application/vnd.terremark.ecloud.user.v1+json; type=group",
                    str_xtmrkdate = x_tmrk_date(),
                    str_xtmrknonce = x_tmrk_nonce();


            /* CanonicalizedHeaders string "x-tmrk" values only */
            ICollection<KeyValuePair<String, String>> HeaderCollec = new Dictionary<String, String>();
            HeaderCollec.Add(new KeyValuePair<String, String>("x-tmrk-nonce", str_xtmrknonce));
            if (actionverb != "GET") { HeaderCollec.Add(new KeyValuePair<String, String>("Accept", acceptcontenttype)); }
            if (acctid != "") { HeaderCollec.Add(new KeyValuePair<String, String>("x-tmrk-acct", acctid)); }
            if (cloudspace != "") { HeaderCollec.Add(new KeyValuePair<String, String>("x-tmrk-cloudspace", cloudspace)); }

            string headercano_str = SortCanoHeaders(HeaderCollec.Select(kvp => new CanonicalizedHeaders() { pname = kvp.Key, pvalue = kvp.Value }));


            /* CanonicalizedResource string - Resource URI after Root URL, and Querystring values only */
            ICollection<KeyValuePair<String, String>> ResourceCollec = new Dictionary<String, String>();

            string resourcecano_str = SortCanoResources(ResourceCollec.Select(kvp => new CanonicalizedResources() { pname = kvp.Key, pvalue = kvp.Value }),
                    rURI.Replace(uriroot_master, "")),
                    shacanosignature = hashSignature(actionverb, str_xtmrknonce, str_xtmrkdate, acceptcontenttype, headercano_str, resourcecano_str),
                    x_tmrk_authstr = x_tmrk_authorization(shacanosignature);

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            var vClient = new HttpClient();
            HttpRequestMessage vMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(apiuri));      //vMessage.Headers.Add("Accept", acceptcontenttype);
            vMessage.Headers.Add("x-tmrk-authorization", x_tmrk_authstr);
            vMessage.Headers.Add("Date", str_xtmrkdate);
            vMessage.Headers.Add("x-tmrk-nonce", str_xtmrknonce);
            if (acctid != "") { vMessage.Headers.Add("x-tmrk-acct", acctid); }
            if (cloudspace != "") { vMessage.Headers.Add("x-tmrk-cloudspace", cloudspace); }

            var clientResponse = vClient.SendAsync(vMessage).Result;
            string clientContent = clientResponse.Content.ReadAsStringAsync().Result;

            return clientContent;
        }

        public string getlist(string listuri, string listcontenttype, string actionverb = "GET")
        {

            string  rURI = listuri,
                    apiuri = uriroot_master + rURI,
                    akey = accesskey,
                    acceptcontenttype = "",
                    str_xtmrkdate = x_tmrk_date(),
                    str_xtmrknonce = x_tmrk_nonce();


            /* CanonicalizedHeaders string "x-tmrk" values only */
            ICollection<KeyValuePair<String, String>> HeaderCollec = new Dictionary<String, String>();

            if (actionverb != "GET") {
                acceptcontenttype = listcontenttype;
                HeaderCollec.Add(new KeyValuePair<String, String>("Accept", acceptcontenttype)); }
            if (acctid != "") { HeaderCollec.Add(new KeyValuePair<String, String>("x-tmrk-acct", acctid)); }
            if (cloudspace != "") { HeaderCollec.Add(new KeyValuePair<String, String>("x-tmrk-cloudspace", cloudspace)); }
            HeaderCollec.Add(new KeyValuePair<String, String>("x-tmrk-nonce", str_xtmrknonce));

            string headercano_str = SortCanoHeaders(HeaderCollec.Select(kvp => new CanonicalizedHeaders() { pname = kvp.Key, pvalue = kvp.Value }));
            
            /* CanonicalizedResource string - Resource URI after Root URL, and Querystring values only */
            ICollection<KeyValuePair<String, String>> ResourceCollec = new Dictionary<String, String>();

            string resourcecano_str = SortCanoResources(ResourceCollec.Select(kvp => new CanonicalizedResources() { pname = kvp.Key, pvalue = kvp.Value }),
                    rURI.Replace(uriroot_master, "")),
                    shacanosignature = hashSignature(actionverb, str_xtmrknonce, str_xtmrkdate, acceptcontenttype, headercano_str, resourcecano_str),
                    x_tmrk_authstr = x_tmrk_authorization(shacanosignature);

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            
            var vClient = new HttpClient();
            HttpRequestMessage vMessage = new HttpRequestMessage(new HttpMethod(actionverb), new Uri(apiuri));
            vMessage.Headers.Add("x-tmrk-authorization", x_tmrk_authstr);                           //x-tmrk-authorization
            vMessage.Headers.Add("Date", str_xtmrkdate);                                            //Date
            vMessage.Headers.Add("x-tmrk-nonce", str_xtmrknonce);                                   //x-tmrk-nonce
            if (acctid != "") { vMessage.Headers.Add("x-tmrk-acct", acctid); }                      //x-tmrk-acct
            if (cloudspace != "") { vMessage.Headers.Add("x-tmrk-cloudspace", cloudspace); }        //x-tmrk-cloudspace

            var clientResponse = vClient.SendAsync(vMessage).Result;
            string clientContent = clientResponse.Content.ReadAsStringAsync().Result;
           
            return clientContent;
        }

        
        /************************************************************************************************************/

        public ICollection<KeyValuePair<String, String>> CanoCollec = new Dictionary<String, String>();


        public string hashSignature(string restverb, string nonce, string servertime, string contenttype = "", string canonicalheader = "", string canonicalresource = "")
        {
            /*  http://cloud.verizon.com/documentation/CloudAPISignatureGeneration.htm
             *  The CloudAPI signature is mathematically represented as follows:
             *  Signature = Base64([ HMAC-SHA1 | HMAC-SHA256 | HMAC-SHA512 ](UTF-8(StringToSign)))
             *  
             *  restverb: The REST method in uppercase, such as GET, POST, PATCH, PUT, DELETE.
             *  
             *  contenttype: The MIME type of the data being sent, if applicable. For example, 
             *  for GET OPTIONS and DELETE, field would be blank.
             */

            string stringtosign = restverb + "\n";

            if (contenttype != "") { stringtosign += contenttype; }

            stringtosign += "\n" + servertime + "\n";

            if (canonicalheader != "")
            {
                stringtosign += canonicalheader;
                /* The canonicalized headers. This field is required when using x-tmrk-date. 
                 * Otherwise, a blank line. */
            }
            else { stringtosign += "\n"; }

            if (canonicalresource != "")
            {
                stringtosign += canonicalresource;
                /* The resource string, all lowercase. For example: "/api/compute/vm", 
                 * followed by any parameters. */
            }

            strtosign = stringtosign;

            try
            {
                byte[] keyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
                byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(stringtosign);
                using (var hmacsha256 = new HMACSHA256(keyByte))
                {
                    byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                    return Convert.ToBase64String(hashmessage);
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Authorization(string signature)
        {
            /* Authorization: CloudApi AccessKey="{accessKey}" SignatureType="{signatureType]" Signature="{signature}" */
            return "CloudApi AccessKey=" + accesskey + " SignatureType=HmacSHA256 Signature=" + signature;
        }

        public string x_tmrk_authorization(string signature)
        {
            /*  xtmrk-authorization: CloudApi AccessKey="{accessKey}" SignatureType="{signatureType}" Signature="{signature} */
            //return "x-tmrk-authorization: CloudApi AccessKey=" + accesskey + " SignatureType=HmacSHA256 Signature=" + signature;
            return "CloudApi AccessKey=" + accesskey + " SignatureType=HmacSHA256 Signature=" + signature;
        }

        public string x_tmrk_date()
        {
            /*  x-tmrk-date: {currentUtcDateTime}  *  Example: x-tmrk-date: Tue, 22 August 2012 11:00:00 GMT     */
            return getservertime();
        }

        public string x_tmrk_nonce()
        {
            /*  x-tmrk-non: {unique-time-millis}  Example: x-tmrk-nonce: 1345758294  */
            return getservertime(true); //true param for milliseconds format
        }
        static readonly DateTime Epoch = new DateTime(1970, 1, 1);
        static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Epoch).TotalMilliseconds;
        }

        public string getservertime(bool milli = false)
        {
            String returntime;

            if (milli)
            {
                /* Based on Unix Epock Total Millisends time since Jan 1, 1970
                 * Unique, client-generated value that the server uses, with the timestamp, to prevent replay attacks
                 * 14176672148803039 .*/

                returntime = CurrentTimeMillis().ToString();
            }
            else
            {
                returntime = System.DateTime.UtcNow.ToString("R");
            }

            return returntime.ToString();
        }


        public class CanonicalizedHeaders : CanoHeaders
        {
            public string pname { get; set; }
            public string pvalue { get; set; }
        }

        public interface CanoHeaders
        {
            // Structure for CanonicalizedHeaders List
            string pname { get; set; }
            string pvalue { get; set; }
        }

        public string SortCanoHeaders(IEnumerable<CanoHeaders> CanonicalHeader)
        {
            /*  Expecting list of parameters and values for Headers per CanonicalHeaders class structure
             * 
             *  http://cloud.verizon.com/documentation/HowToConnectTheCanonicalizedHeadersString.htm
             *  All headers with x-tmrk-, to lowercase, sort lexiograpically by header field name asc (only once ea.), 
             *  replace breaks with " ", Trim around ":", Append new line char ea. header field, concat to single string
             */

            CanonicalHeader.OrderBy(x => x.pname);
            var listHeaders = from CanoHeaders in CanonicalHeader
                              select new { param = CanoHeaders.pname.ToString(), value = CanoHeaders.pvalue.ToString() };   //where CanoHeaders.pname != "x-tmrk-authorization"

            string returnheaders = String.Empty;

            foreach (var row in listHeaders)
            {
                returnheaders += row.param.ToString() + ":" + row.value.ToString() + "\n";
            }

            return returnheaders.ToLower().ToString();
        }


        public class CanonicalizedResources : CanoResources
        {
            public string pname { get; set; }
            public string pvalue { get; set; }
        }

        public interface CanoResources
        {
            // Structure for CanonicalizedResources List
            string pname { get; set; }
            string pvalue { get; set; }
        }

        public string SortCanoResources(IEnumerable<CanoResources> CanonicalResource, string ResourceURI)
        {
            /*  Expecting list of parameters and values for Headers per CanonicalizedResources class structure
             * 
             *  http://cloud.verizon.com/documentation/HowToConnectTheCanonicalizedHeadersString.htm
             *  All headers with x-tmrk-, to lowercase, sort lexiograpically by header field name asc (only once ea.), 
             *  replace breaks with " ", Trim around ":", Append new line char ea. header field, concat to single string
             */

            CanonicalResource.OrderBy(x => x.pname);
            var listHeaders = from CanoHeaders in CanonicalResource
                              select new { param = CanoHeaders.pname, value = CanoHeaders.pvalue };

            string returnheaders = ResourceURI + "\n";

            foreach (var row in listHeaders)
            {
                returnheaders += row.param.ToString() + ":" + row.value.ToString() + "\n";
            }

            return returnheaders.ToLower().ToString();
        }


    }

    public class VCCREST : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}