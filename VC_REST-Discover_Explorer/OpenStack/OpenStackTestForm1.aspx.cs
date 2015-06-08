using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OpenStack;
using OpenStack.Identity;
using OpenStack.Storage;

namespace VC_REST_Discover_Explorer.OpenStack
{
    public partial class OpenStackTestForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var authUri = new Uri(txt_URI.Text);
            var userName = txt_username.Text;
            var password = txt_password.Text;
            var tenantId = txt_tenantid.Text;

            var credential = new OpenStackCredential(authUri, userName, password, tenantId);
            var client = OpenStackClientFactory.CreateClient(credential);

            client.Connect();

            var storageServiceClient = client.CreateServiceClient<IStorageServiceClient>();
            var storageAccount = storageServiceClient.GetStorageAccount();
            //var storageContainers = StorageContainer StorageContainer<StorageContainer>();
            //foreach (var Containers in StorageContainer)
            //{
                
            //    Console.WriteLine(container.Name);
            //}
        }
    }
}