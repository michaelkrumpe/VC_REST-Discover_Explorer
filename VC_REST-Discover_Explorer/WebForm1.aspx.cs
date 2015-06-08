using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using VC_REST_Discover_Explorer.VCCREST;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace VC_REST_Discover_Explorer
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void btn_testconn_Click(object sender, EventArgs e)
        {
            lbl_connmsg.Visible = true;
            if ((txt_accesskey.Text != "") & (txt_secretkey.Text != "") & (rdo_dcuri.SelectedValue != ""))
            {
                if (rdo_dcuri.SelectedValue == "PrivateURI")
                {
                    if (txt_PrivateEndPoint.Text != "")
                    {
                        Session["accesskey"] = txt_accesskey.Text;
                        Session["secretkey"] = txt_secretkey.Text;
                        Session["cloudspace"] = txt_cloudspace.Text;
                        Session["acctid"] = txt_acctid.Text;
                        Session["dcuri"] = txt_PrivateEndPoint.Text;

                        lbl_connmsg.Text = testconnection().ToString();
                    }
                    else
                    {
                        pnl_privatetxt.Visible = true;
                        txt_PrivateEndPoint.ToolTip = "Please enter a fully qualified end point URI";
                    }
                }
                else
                {
                    pnl_privatetxt.Visible = false;
                    Session["accesskey"] = txt_accesskey.Text;
                    Session["secretkey"] = txt_secretkey.Text;
                    Session["cloudspace"] = txt_cloudspace.Text;
                    Session["acctid"] = txt_acctid.Text;
                    Session["dcuri"] = rdo_dcuri.SelectedValue;

                    lbl_connmsg.Text = testconnection().ToString();
                }
                
            }
            else
            {
                lbl_connmsg.Text = "You are missing a credential";
            }
            
        }

        public string testconnection()
        {
            restapi vcc = new restapi(); 
                vcc.accesskey = Session["accesskey"].ToString();
                vcc.secretkey = Session["secretkey"].ToString();
                vcc.cloudspace = Session["cloudspace"].ToString();
                vcc.acctid = Session["acctid"].ToString();
                vcc.uriroot_master = Session["dcuri"].ToString();

                String testresp = vcc.testconn();

                Session["stringtosign"] = vcc.strtosign;

                if (testresp != "OK") { div_explorer_options.Visible = false; } 
                else { 
                    div_explorer_options.Visible = true;
                    txt_baseuri.Text = rdo_dcuri.SelectedValue;
                    pnl_sts.Visible = true; txt_stringtosign.Text = Session["stringtosign"].ToString();
                }

                return testresp;
        }

        //protected void rdo_vccaction_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    rdo_option();
        //}
        //protected void btn_common_Click(object sender, EventArgs e)
        //{
        //    rdo_option();
        //}
        //public string rdo_option()
        //{
        //    div_machines_options.Visible = false; div_networks_options.Visible = false; div_storage_options.Visible = false; div_Users_Options.Visible = false; div_explorer_options.Visible = false;
        //    div_explorer.Visible = false;
        //    lbl_explorer.Text = String.Empty;

        //    if (rdo_vccaction.SelectedItem.Text == "Machines") { div_machines_options.Visible = true; }
        //    if (rdo_vccaction.SelectedItem.Text == "Networks") { div_networks_options.Visible = true; }
        //    if (rdo_vccaction.SelectedItem.Text == "Explorer") { div_explorer_options.Visible = true; txt_baseuri.Text = rdo_dcuri.SelectedValue; }

        //    return String.Empty;
        //}


        protected void btn_explore_get_Click(object sender, EventArgs e)
        {
            String resp_explore = String.Empty;

            if (Session["accesskey"].ToString() != "" & Session["secretkey"].ToString() != "")
            {
                div_explorer.Visible = true;
                resp_explore = getvccexplore();

                pnl_sts.Visible = true; txt_stringtosign.Text = Session["stringtosign"].ToString();

                dynamic parsedJson = JsonConvert.DeserializeObject(resp_explore);

                txt_multiline.Text = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
                                
            }
            else
            {
                div_explorer.Visible = false;
                pnl_sts.Visible = false;
            }
            
        }

        protected void btn_explore_options_Click(object sender, EventArgs e)
        {
            String resp_explore = String.Empty;

            if (Session["accesskey"].ToString() != "" & Session["secretkey"].ToString() != "")
            {
                div_explorer.Visible = true;
                resp_explore = getvccexplore("OPTIONS");

                dynamic parsedJson = JsonConvert.DeserializeObject(resp_explore);

                txt_multiline.Text = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
            }
            else
            {
                div_explorer.Visible = false;
                pnl_sts.Visible = false;
            }
        }


        public string getvccexplore(string method = "GET")
        {
            restapi vcc = new restapi();
            vcc.accesskey = Session["accesskey"].ToString();
            vcc.secretkey = Session["secretkey"].ToString();
            vcc.cloudspace = Session["cloudspace"].ToString();
            vcc.acctid = Session["acctid"].ToString();
            vcc.uriroot_master = Session["dcuri"].ToString();

            string listresponse = vcc.getlist(txt_exploreuri.Text, txt_contenttype.Text, method);

            Session["stringtosign"] = vcc.strtosign;

            return listresponse;
            
        }

        protected void btn_securedbcreds_Click(object sender, EventArgs e)
        {
            string authuid = "";
            var authUri = new Uri("https://api.securedb.co:443/securedbapi/account/utvJwUd7/mkvz1/authenticate");
            
            using (var pClient = new HttpClient())
            {
                pClient.BaseAddress = new Uri("https://api.securedb.co:443/securedbapi/account/utvJwUd7/mkvz1/");
                
                
                pClient.DefaultRequestHeaders.Accept.Clear();
                pClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                pClient.DefaultRequestHeaders.Add("Authorization","Basic OENWS0RPMUNTSTpGMEFBRkc5NTg1RUNLNVY=");

                var securedbusercreds = new securedbuser { userName = "webuser", password = "User12pass34!" };
                var postresponse = pClient.PostAsJsonAsync("authenticate", securedbusercreds).Result;

                string jsonresponse = postresponse.Content.ReadAsStringAsync().Result;
                JObject authjss = JObject.Parse(jsonresponse);
                                        
                authuid = (string)authjss["data"];

                //GET /account/{customerId}/{directoryId}/accountbasic/{uuid}
                string securedburi = "https://api.securedb.co:443/securedbapi/account/utvJwUd7/mkvz1/accountbasic/" + authuid;
                var vClient = new HttpClient();
                HttpRequestMessage vMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(securedburi));
                vMessage.Headers.Add("Authorization", "Basic OENWS0RPMUNTSTpGMEFBRkc5NTg1RUNLNVY=");

                var clientResponse = vClient.SendAsync(vMessage).Result;
                    
                if (clientResponse.IsSuccessStatusCode) {
                    string clientContent = clientResponse.Content.ReadAsStringAsync().Result;
                    JObject jss = JObject.Parse(clientContent);

                    string sdbaccesskey = (string)jss["data"]["firstName"];
                    string sdbsecretkey = (string)jss["data"]["lastName"];
                    string sdbcloudspace = (string)jss["data"]["email"];

                    txt_accesskey.Text = sdbaccesskey;
                    txt_secretkey.Text = sdbsecretkey;
                    txt_cloudspace.Text = sdbcloudspace;
                    }   

                else { 
                    lbl_sdberr.Text = clientResponse.StatusCode.ToString(); 
                }
            //}
            //else
            //{
            //    lbl_sdberr.Text = postresponse.StatusCode.ToString();
            //}
                
            }
            
            
            
            
        }
        class securedbuser
        {
            public string userName { get; set; }
            public string password { get; set; }
            public string rest_client_ip { get; set; }
            public string true_client_ip { get; set; }
            public string remote_ip { get; set; }
        }

        //static void Main()
        //{
        //    RunAsync().Wait();
        //}
        //static async Task RunAsync()
        //{
        //    using (var client = new HttpClient())
        //    {
        //        // TODO - Send HTTP requests
        //    }
        //}
    }
}